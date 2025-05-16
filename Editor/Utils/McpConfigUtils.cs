using System;
using System.IO;
using System.Collections.Generic;
using TheBifrost.Unity;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TheBifrost.Utils
{
    /// <summary>
    /// Utility class for MCP configuration operations
    /// </summary>
    public static class McpConfigUtils
    {
        /// <summary>
        /// Generates the MCP configuration JSON to setup TheBifrost server in different AI Clients
        /// </summary>
        /// <param name="serverPath">The absolute path to the server directory containing build/index.js</param>
        /// <param name="useTabsIndentation">Whether to use tabs for indentation</param>
        /// <returns>MCP configuration JSON string</returns>
        public static string GenerateMcpConfigJson(string serverPath, bool useTabsIndentation)
        {
            // Verify the server path exists
            if (string.IsNullOrEmpty(serverPath) || !Directory.Exists(serverPath))
            {
                Debug.LogError($"[TheBifrost] Invalid server path: {serverPath}");
                return "{}";
            }
            
            string serverIndexPath = Path.Combine(serverPath, "build", "index.js");
            
            // Verify the index.js file exists
            if (!File.Exists(serverIndexPath))
            {
                Debug.LogError($"[TheBifrost] Server index.js not found at: {serverIndexPath}. Make sure the server is built.");
                return "{}";
            }
            
            var config = new Dictionary<string, object>
            {
                { "mcpServers", new Dictionary<string, object>
                    {
                        { "thebifrost-server", new Dictionary<string, object>
                            {
                                { "command", "node" },
                                { "args", new[] { serverIndexPath } }
                            }
                        }
                    }
                }
            };
            
            // Initialize string writer with proper indentation
            var stringWriter = new StringWriter();
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                
                // Set indentation character and count
                if (useTabsIndentation)
                {
                    jsonWriter.IndentChar = '\t';
                    jsonWriter.Indentation = 1;
                }
                else
                {
                    jsonWriter.IndentChar = ' ';
                    jsonWriter.Indentation = 2;
                }
                
                // Serialize directly to the JsonTextWriter
                var serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, config);
            }
            
            return stringWriter.ToString().Replace("\\", "/").Replace("//", "/");
        }
        
        /// <summary>
        /// Gets the absolute path to the Server directory containing package.json
        /// Works whether TheBifrost is installed via Package Manager or directly in the Assets folder
        /// </summary>
        public static string GetServerPath()
        {
            // First, try to find the package info via Package Manager
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath($"Packages/{TheBifrostSettings.PackageName}");
                
            if (packageInfo != null && !string.IsNullOrEmpty(packageInfo.resolvedPath))
            {
                string serverPath = Path.Combine(packageInfo.resolvedPath, "Server~");
                if (Directory.Exists(serverPath))
                {
                    Debug.Log($"[TheBifrost] Found Server directory via Package Manager at: {serverPath}");
                    return serverPath;
                }
            }
            
            // Try to find the Server~ directory in the current project
            string projectServerPath = Path.Combine(Application.dataPath, "..", "TheBifrost", "Server~");
            if (Directory.Exists(projectServerPath))
            {
                Debug.Log($"[TheBifrost] Found Server directory in project at: {projectServerPath}");
                return Path.GetFullPath(projectServerPath);
            }
            
            // Try to find by searching for tsconfig.json files
            var assets = AssetDatabase.FindAssets("tsconfig");
            
            if (assets.Length > 0)
            {
                foreach (var assetGuid in assets)
                {
                    string relativePath = AssetDatabase.GUIDToAssetPath(assetGuid);
                    string fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", relativePath));
                    string directoryName = Path.GetDirectoryName(fullPath);
                    
                    if (Path.GetFileName(directoryName) == "Server~")
                    {
                        Debug.Log($"[TheBifrost] Found Server directory via tsconfig search at: {directoryName}");
                        return directoryName;
                    }
                }
            }
            
            // If we get here, we couldn't find the server path
            var errorString = "[TheBifrost] Could not locate Server directory. Please check the installation of TheBifrost package.";
            Debug.LogError(errorString);
            return errorString;
        }
        
        /// <summary>
        /// Adds the MCP configuration to the Windsurf MCP config file
        /// </summary>
        public static bool AddToWindsurfIdeConfig(bool useTabsIndentation)
        {
            string configFilePath = GetWindsurfMcpConfigPath();
            return AddToConfigFile(configFilePath, useTabsIndentation, "Windsurf");
        }
        
        /// <summary>
        /// Adds the MCP configuration to the Claude Desktop config file
        /// </summary>
        public static bool AddToClaudeDesktopConfig(bool useTabsIndentation)
        {
            string configFilePath = GetClaudeDesktopConfigPath();
            return AddToConfigFile(configFilePath, useTabsIndentation, "Claude Desktop");
        }
        
        /// <summary>
        /// Adds the MCP configuration to the Cursor config file
        /// </summary>
        public static bool AddToCursorConfig(bool useTabsIndentation)
        {
            string configFilePath = GetCursorConfigPath();
            return AddToConfigFile(configFilePath, useTabsIndentation, "Cursor");
        }

        /// <summary>
        /// Common method to add MCP configuration to a specified config file
        /// </summary>
        /// <param name="configFilePath">Path to the config file</param>
        /// <param name="useTabsIndentation">Whether to use tabs for indentation</param>
        /// <param name="productName">Name of the product (for error messages)</param>
        /// <returns>True if successfuly added the config, false otherwise</returns>
        private static bool AddToConfigFile(string configFilePath, bool useTabsIndentation, string productName)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                Debug.LogError($"{productName} config file not found. Please make sure {productName} is installed.");
                return false;
            }
                
            // Generate fresh MCP config JSON
            string mcpConfigJson = GenerateMcpConfigJson(GetServerPath(), useTabsIndentation);
                
            // Parse the MCP config JSON
            JObject mcpConfig = JObject.Parse(mcpConfigJson);
            
            try
            {
                // Check if the file exists
                if (File.Exists(configFilePath))
                {
                    // Read the existing config
                    string existingConfigJson = File.ReadAllText(configFilePath);
                    JObject existingConfig = string.IsNullOrEmpty(existingConfigJson) ? new JObject() : JObject.Parse(existingConfigJson);
                    
                    // Merge the mcpServers from our config into the existing config
                    if (mcpConfig["mcpServers"] != null && mcpConfig["mcpServers"] is JObject mcpServers)
                    {
                        // Create mcpServers object if it doesn't exist
                        if (existingConfig["mcpServers"] == null)
                        {
                            existingConfig["mcpServers"] = new JObject();
                        }
                        
                        // Add or update the thebifrost-server server config
                        if (mcpServers["thebifrost-server"] != null)
                        {
                            ((JObject)existingConfig["mcpServers"])["thebifrost-server"] = mcpServers["thebifrost-server"];
                        }
                        
                        // Write the updated config back to the file
                        File.WriteAllText(configFilePath, existingConfig.ToString(Formatting.Indented));
                        return true;
                    }
                }
                else if(Directory.Exists(Path.GetDirectoryName(configFilePath)))
                {
                    // Create a new config file with just our config
                    File.WriteAllText(configFilePath, mcpConfigJson);
                    return true;
                }
                else
                {
                    Debug.LogError($"Cannot find {productName} config file or {productName} is currently not installed. Expecting {productName} to be installed in the {configFilePath} path");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to add MCP configuration to {productName}: {ex}");
            }

            return false;
        }
        
        /// <summary>
        /// Gets the path to the Windsurf MCP config file based on the current OS
        /// </summary>
        /// <returns>The path to the Windsurf MCP config file</returns>
        private static string GetWindsurfMcpConfigPath()
        {
            // Base path depends on the OS
            string basePath;
            
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Windows: %USERPROFILE%/.codeium/windsurf
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".codeium/windsurf");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // macOS: ~/Library/Application Support/.codeium/windsurf
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                basePath = Path.Combine(homeDir, "Library", "Application Support", ".codeium/windsurf");
            }
            else
            {
                // Unsupported platform
                Debug.LogError("Unsupported platform for Windsurf MCP config");
                return null;
            }
            
            // Return the path to the mcp_config.json file
            return Path.Combine(basePath, "mcp_config.json");
        }
        
        /// <summary>
        /// Gets the path to the Claude Desktop config file based on the current OS
        /// </summary>
        /// <returns>The path to the Claude Desktop config file</returns>
        private static string GetClaudeDesktopConfigPath()
        {
            // Base path depends on the OS
            string basePath;
            
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Windows: %USERPROFILE%/AppData/Roaming/Claude
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Claude");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // macOS: ~/Library/Application Support/Claude
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                basePath = Path.Combine(homeDir, "Library", "Application Support", "Claude");
            }
            else
            {
                // Unsupported platform
                Debug.LogError("Unsupported platform for Claude Desktop config");
                return null;
            }
            
            // Return the path to the claude_desktop_config.json file
            return Path.Combine(basePath, "claude_desktop_config.json");
        }
        
        
        
        /// <summary>
        /// Gets the path to the Cursor config file based on the current OS
        /// </summary>
        /// <returns>The path to the Cursor config file</returns>
        private static string GetCursorConfigPath()
        {
            // Base path depends on the OS
            string basePath;
            
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                // Windows: %USERPROFILE%/.cursor
                basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cursor");
            }
            else if (Application.platform == RuntimePlatform.OSXEditor)
            {
                // macOS: ~/.cursor
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                basePath = Path.Combine(homeDir, ".cursor");
            }
            else
            {
                // Unsupported platform
                Debug.LogError("Unsupported platform for Cursor MCP config");
                return null;
            }
            
            // Return the path to the mcp_config.json file
            return Path.Combine(basePath, "mcp.json");
        }
    }
}
