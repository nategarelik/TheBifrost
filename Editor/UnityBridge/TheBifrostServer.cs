using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using WebSocketSharp.Server;
using Debug = UnityEngine.Debug;
using TheBifrost.Tools;
using TheBifrost.Resources;
using TheBifrost.Services;
using TheBifrost.Utils;

namespace TheBifrost.Unity
{
    /// <summary>
    /// TheBifrost Server to communicate Node.js MCP server.
    /// Uses WebSockets to communicate with Node.js.
    /// </summary>
    [InitializeOnLoad]
    public class TheBifrostServer
    {
        private static TheBifrostServer _instance;
        
        private readonly Dictionary<string, TheBifrost.Tools.McpToolBase> _tools = new Dictionary<string, TheBifrost.Tools.McpToolBase>();
        private readonly Dictionary<string, TheBifrost.Resources.McpResourceBase> _resources = new Dictionary<string, TheBifrost.Resources.McpResourceBase>();
        
        private WebSocketServer _webSocketServer;
        private CancellationTokenSource _cts;
        private TheBifrost.Services.TestRunnerService _testRunnerService;
        private TheBifrost.Services.ConsoleLogsService _consoleLogsService;

        /// <summary>
        /// Static constructor that gets called when Unity loads due to InitializeOnLoad attribute
        /// </summary>
        static TheBifrostServer()
        {
            // Initialize the singleton instance when Unity loads
            // This ensures the bridge is available as soon as Unity starts
            EditorApplication.quitting += Instance.StopServer;

            // Auto-restart server after domain reload
            if (TheBifrostSettings.Instance.AutoStartServer)
            {
                Instance.StartServer();
            }
        }
        
        /// <summary>
        /// Singleton instance accessor
        /// </summary>
        public static TheBifrostServer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TheBifrostServer();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Current Listening state
        /// </summary>
        public bool IsListening => _webSocketServer?.IsListening ?? false;

        /// <summary>
        /// Dictionary of connected clients with this server
        /// </summary>
        public Dictionary<string, string> Clients { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Private constructor to enforce singleton pattern
        /// </summary>
        private TheBifrostServer()
        {
            InitializeServices();
            RegisterResources();
            RegisterTools();
        }
        
        /// <summary>
        /// Start the WebSocket Server to communicate with Node.js
        /// </summary>
        public void StartServer()
        {
            if (IsListening) return;
            
            try
            {
                // Ensure dependencies are installed
                string serverPath = McpConfigUtils.GetServerPath();
                if (!string.IsNullOrEmpty(serverPath) && Directory.Exists(serverPath))
                {
                    // Run npm install in the server directory
                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = "npm",
                        Arguments = "install",
                        WorkingDirectory = serverPath,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    using (var process = new Process { StartInfo = processStartInfo })
                    {
                        process.Start();
                        process.WaitForExit();
                        
                        if (process.ExitCode != 0)
                        {
                            McpLogger.LogError($"Failed to install server dependencies: {process.StandardError.ReadToEnd()}");
                            return;
                        }
                        McpLogger.LogInfo("Server dependencies installed successfully");
                    }
                }
                else
                {
                    McpLogger.LogError("Could not find server path to install dependencies");
                    return;
                }

                // Check if the server needs to be built
                if (CheckAndBuildServerIfNeeded())
                {
                    McpLogger.LogInfo("Server built successfully");
                }
                
                // Create a new WebSocket server
                _webSocketServer = new WebSocketServer($"ws://localhost:{TheBifrostSettings.Instance.Port}");
                // Add the MCP service endpoint with a handler that references this server
                _webSocketServer.AddWebSocketService("/TheBifrost", () => new TheBifrostSocketHandler(this));
                
                // Start the server
                _webSocketServer.Start();
                
                McpLogger.LogInfo($"WebSocket server started on port {TheBifrostSettings.Instance.Port}");
            }
            catch (Exception ex)
            {
                McpLogger.LogError($"Failed to start WebSocket server: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Stop the WebSocket server
        /// </summary>
        public void StopServer()
        {
            if (!IsListening) return;
            
            try
            {
                _webSocketServer?.Stop();
                
                McpLogger.LogInfo("WebSocket server stopped");
            }
            catch (Exception ex)
            {
                McpLogger.LogError($"Error stopping WebSocket server: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Try to get a tool by name
        /// </summary>
        public bool TryGetTool(string name, out McpToolBase tool)
        {
            return _tools.TryGetValue(name, out tool);
        }
        
        /// <summary>
        /// Try to get a resource by name
        /// </summary>
        public bool TryGetResource(string name, out McpResourceBase resource)
        {
            return _resources.TryGetValue(name, out resource);
        }
        
        /// <summary>
        /// Register all available tools
        /// </summary>
        private void RegisterTools()
        {
            // Register MenuItemTool
            MenuItemTool menuItemTool = new MenuItemTool();
            _tools.Add(menuItemTool.Name, menuItemTool);
            
            // Register SelectGameObjectTool
            SelectGameObjectTool selectGameObjectTool = new SelectGameObjectTool();
            _tools.Add(selectGameObjectTool.Name, selectGameObjectTool);
            
            // Register PackageManagerTool
            AddPackageTool addPackageTool = new AddPackageTool();
            _tools.Add(addPackageTool.Name, addPackageTool);
            
            // Register RunTestsTool
            RunTestsTool runTestsTool = new RunTestsTool(_testRunnerService);
            _tools.Add(runTestsTool.Name, runTestsTool);
            
            // Register SendConsoleLogTool
            SendConsoleLogTool sendConsoleLogTool = new SendConsoleLogTool();
            _tools.Add(sendConsoleLogTool.Name, sendConsoleLogTool);
            
            // Register UpdateComponentTool
            UpdateComponentTool updateComponentTool = new UpdateComponentTool();
            _tools.Add(updateComponentTool.Name, updateComponentTool);
            
            // Register AddAssetToSceneTool
            AddAssetToSceneTool addAssetToSceneTool = new AddAssetToSceneTool();
            _tools.Add(addAssetToSceneTool.Name, addAssetToSceneTool);
        }
        
        /// <summary>
        /// Register all available resources
        /// </summary>
        private void RegisterResources()
        {
            // Register GetMenuItemsResource
            GetMenuItemsResource getMenuItemsResource = new GetMenuItemsResource();
            _resources.Add(getMenuItemsResource.Name, getMenuItemsResource);
            
            // Register GetConsoleLogsResource
            GetConsoleLogsResource getConsoleLogsResource = new GetConsoleLogsResource(_consoleLogsService);
            _resources.Add(getConsoleLogsResource.Name, getConsoleLogsResource);
            
            // Register GetHierarchyResource
            GetHierarchyResource getHierarchyResource = new GetHierarchyResource();
            _resources.Add(getHierarchyResource.Name, getHierarchyResource);
            
            // Register GetPackagesResource
            GetPackagesResource getPackagesResource = new GetPackagesResource();
            _resources.Add(getPackagesResource.Name, getPackagesResource);
            
            // Register GetAssetsResource
            GetAssetsResource getAssetsResource = new GetAssetsResource();
            _resources.Add(getAssetsResource.Name, getAssetsResource);
            
            // Register GetTestsResource
            GetTestsResource getTestsResource = new GetTestsResource(_testRunnerService);
            _resources.Add(getTestsResource.Name, getTestsResource);
            
            // Register GetGameObjectResource
            GetGameObjectResource getGameObjectResource = new GetGameObjectResource();
            _resources.Add(getGameObjectResource.Name, getGameObjectResource);
        }
        
        /// <summary>
        /// Initialize services used by the server
        /// </summary>
        private void InitializeServices()
        {
            // Initialize the test runner service
            _testRunnerService = new TestRunnerService();
            
            // Initialize the console logs service
            _consoleLogsService = new ConsoleLogsService();
        }
        
        /// <summary>
        /// Check if the server needs to be built and build it if necessary
        /// </summary>
        /// <returns>True if the server was built, false otherwise</returns>
        private bool CheckAndBuildServerIfNeeded()
        {
            try
            {
                // Get the server path
                string serverPath = McpConfigUtils.GetServerPath();
                
                // Check if the server path is valid
                if (serverPath.StartsWith("[TheBifrost] Could not locate Server directory"))
                {
                    McpLogger.LogError("Could not locate Server directory. Please check the installation of TheBifrost package.");
                    return false;
                }
                
                // Check if the build directory exists
                string buildDir = Path.Combine(serverPath, "build");
                string indexJsPath = Path.Combine(buildDir, "index.js");
                
                if (Directory.Exists(buildDir) && File.Exists(indexJsPath))
                {
                    // Server is already built
                    McpLogger.LogInfo("Server is already built");
                    return false;
                }
                
                // Server needs to be built
                McpLogger.LogInfo("Server needs to be built. Building now...");
                
                // Run npm install
                if (!RunNpmCommand(serverPath, "install"))
                {
                    McpLogger.LogError("Failed to run npm install");
                    return false;
                }
                
                // Run npm run build
                if (!RunNpmCommand(serverPath, "run build"))
                {
                    McpLogger.LogError("Failed to run npm run build");
                    return false;
                }
                
                // Check if the build was successful
                if (!Directory.Exists(buildDir) || !File.Exists(indexJsPath))
                {
                    McpLogger.LogError("Build failed. Build directory or index.js not found.");
                    return false;
                }
                
                return true;
            }
            catch (Exception ex)
            {
                McpLogger.LogError($"Error checking or building server: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Run an npm command in the specified directory
        /// </summary>
        /// <param name="workingDirectory">The directory to run the command in</param>
        /// <param name="arguments">The npm command arguments</param>
        /// <returns>True if the command was successful, false otherwise</returns>
        private bool RunNpmCommand(string workingDirectory, string arguments)
        {
            try
            {
                McpLogger.LogInfo($"Running npm {arguments} in {workingDirectory}");
                
                // Create process start info
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "npm",
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                
                // Start the process
                using (Process process = Process.Start(startInfo))
                {
                    if (process == null)
                    {
                        McpLogger.LogError("Failed to start npm process");
                        return false;
                    }
                    
                    // Read the output
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    
                    // Wait for the process to exit
                    process.WaitForExit();
                    
                    // Check if the process exited successfully
                    if (process.ExitCode != 0)
                    {
                        McpLogger.LogError($"npm {arguments} failed with exit code {process.ExitCode}");
                        McpLogger.LogError($"Error: {error}");
                        return false;
                    }
                    
                    McpLogger.LogInfo($"npm {arguments} completed successfully");
                    return true;
                }
            }
            catch (Exception ex)
            {
                McpLogger.LogError($"Error running npm {arguments}: {ex.Message}");
                return false;
            }
        }
    }
}
