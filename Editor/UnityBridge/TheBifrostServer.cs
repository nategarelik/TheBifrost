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
        /// <param name="serverPath">The absolute path to the server directory containing build/index.js</param>
        public void StartServer(string serverPath)
        {
            if (IsListening)
            {
                McpLogger.LogInfo("StartServer called but server is already listening.");
                return;
            }
            McpLogger.LogInfo($"Attempting to start server with path: {serverPath}");
            
            try
            {
                // Verify the server path exists
                if (string.IsNullOrEmpty(serverPath) || !Directory.Exists(serverPath))
                {
                    McpLogger.LogError($"StartServer Error: Invalid server path provided: '{serverPath}'. Path is null, empty, or directory does not exist.");
                    return;
                }
                McpLogger.LogInfo($"Server path '{serverPath}' is valid.");

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
                        McpLogger.LogError($"npm install failed. Exit Code: {process.ExitCode}. Error: {process.StandardError.ReadToEnd()}");
                        return;
                    }
                    McpLogger.LogInfo("npm install completed successfully.");
                }

                McpLogger.LogInfo("Proceeding to CheckAndBuildServerIfNeeded...");
                // Check if the server needs to be built
                bool buildAttempted = CheckAndBuildServerIfNeeded(serverPath);
                if (buildAttempted) // This means a build was needed and attempted
                {
                    // Further checks for build success are inside CheckAndBuildServerIfNeeded
                    // If it returns true, it implies build was successful or not needed.
                    // If it returns false, an error was logged inside.
                     McpLogger.LogInfo("CheckAndBuildServerIfNeeded completed. If build was needed, it was attempted.");
                } else {
                     McpLogger.LogInfo("CheckAndBuildServerIfNeeded determined no build was necessary.");
                }

                // Ensure build/index.js exists before starting WebSocketServer
                string indexJsPath = Path.Combine(serverPath, "build", "index.js");
                if (!File.Exists(indexJsPath))
                {
                    McpLogger.LogError($"StartServer Error: build/index.js not found at '{indexJsPath}' after build attempt. Cannot start WebSocket server.");
                    return;
                }
                McpLogger.LogInfo($"build/index.js confirmed at '{indexJsPath}'.");
                
                // Create a new WebSocket server
                _webSocketServer = new WebSocketServer($"ws://localhost:{TheBifrostSettings.Instance.Port}");
                // Add the MCP service endpoint with a handler that references this server
                _webSocketServer.AddWebSocketService("/TheBifrost", () => new TheBifrostSocketHandler(this));
                
                // Start the server
                _webSocketServer.Start();
                if (_webSocketServer.IsListening)
                {
                    McpLogger.LogInfo($"WebSocket server started successfully on port {TheBifrostSettings.Instance.Port}. Listening for connections at ws://localhost:{TheBifrostSettings.Instance.Port}/TheBifrost");
                }
                else
                {
                    McpLogger.LogError("WebSocketServer.Start() was called, but server is not listening. Check for other errors or port conflicts.");
                }
            }
            catch (Exception ex)
            {
                McpLogger.LogError($"StartServer Exception: Failed to start WebSocket server: {ex.Message}\nStackTrace: {ex.StackTrace}");
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
        /// <param name="serverPath">The absolute path to the server directory.</param>
        /// <returns>True if the server was built, false otherwise</returns>
        private bool CheckAndBuildServerIfNeeded(string serverPath)
        {
            try
            {
                // Check if the server path is valid
                McpLogger.LogInfo($"CheckAndBuildServerIfNeeded called with serverPath: {serverPath}");
                if (string.IsNullOrEmpty(serverPath) || !Directory.Exists(serverPath) || serverPath.StartsWith("["))
                {
                    McpLogger.LogError($"CheckAndBuildServerIfNeeded Error: Invalid serverPath: '{serverPath}'.");
                    return false;
                }
                McpLogger.LogInfo($"CheckAndBuildServerIfNeeded: serverPath '{serverPath}' is valid.");
                
                // Check if the build directory exists
                string buildDir = Path.Combine(serverPath, "build");
                string indexJsPath = Path.Combine(buildDir, "index.js");
                
                if (Directory.Exists(buildDir) && File.Exists(indexJsPath))
                {
                    McpLogger.LogInfo($"CheckAndBuildServerIfNeeded: Server already built at '{indexJsPath}'. No build needed.");
                    return false; // Returning false indicates build was not *performed*, true indicates it was *attempted/successful*
                }
                
                McpLogger.LogInfo("CheckAndBuildServerIfNeeded: Server needs to be built. Attempting build...");
                
                McpLogger.LogInfo("CheckAndBuildServerIfNeeded: Running npm install...");
                if (!RunNpmCommand(serverPath, "install"))
                {
                    McpLogger.LogError("CheckAndBuildServerIfNeeded: npm install failed.");
                    return false; // Build process failed
                }
                McpLogger.LogInfo("CheckAndBuildServerIfNeeded: npm install successful.");
                
                McpLogger.LogInfo("CheckAndBuildServerIfNeeded: Running npm run build...");
                if (!RunNpmCommand(serverPath, "run build"))
                {
                    McpLogger.LogError("CheckAndBuildServerIfNeeded: npm run build failed.");
                    return false; // Build process failed
                }
                McpLogger.LogInfo("CheckAndBuildServerIfNeeded: npm run build successful.");
                
                // Check if the build was successful
                if (!Directory.Exists(buildDir) || !File.Exists(indexJsPath))
                {
                    McpLogger.LogError($"CheckAndBuildServerIfNeeded: Build attempt finished, but build directory ('{buildDir}') or index.js ('{indexJsPath}') not found.");
                    return false; // Build process failed
                }
                McpLogger.LogInfo($"CheckAndBuildServerIfNeeded: Build successful. index.js found at '{indexJsPath}'.");
                return true; // Build was needed and successfully performed
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
