using System;
using System.IO;
using TheBifrost.Utils;
using UnityEngine;
using UnityEditor;

namespace TheBifrost.Unity
{
    /// <summary>
    /// Handles persistence of TheBifrost settings
    /// </summary>
    [Serializable]
    public class TheBifrostSettings
    {
        // Constants
        public const string ServerVersion = "1.0.0";
        public const string PackageName = "com.nathanielgarelik.thebifrost";
        public const int RequestTimeoutMinimum = 10;

        private const string EnvUnityPort = "UNITY_PORT";
        private const string EnvUnityRequestTimeout = "UNITY_REQUEST_TIMEOUT";
        private const string SettingsPath = "ProjectSettings/TheBifrostSettings.json";
        
        private static TheBifrostSettings _instance;

        // Server settings
        public int Port { get; set; } = 8090;
        
        [Tooltip("Timeout in seconds for tool request")]
        public int RequestTimeoutSeconds { get; set; } = RequestTimeoutMinimum;
        
        [Tooltip("Whether to automatically start the MCP server when Unity opens")]
        public bool AutoStartServer = true;
        
        [Tooltip("Whether to show info logs in the Unity console")]
        public bool EnableInfoLogs = true;

        [Tooltip("User-defined path to the TheBifrost Node.js server directory (e.g., path/to/TheBifrost/Server~)")]
        public string UserDefinedServerPath = "";

        /// <summary>
        /// Singleton instance of settings
        /// </summary>
        public static TheBifrostSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TheBifrostSettings();
                }
                return _instance;
            }
        }

        /// <summary>
        /// Private constructor for singleton
        /// </summary>
        private TheBifrostSettings()
        { 
            LoadSettings();
            VsCodeWorkspaceUtils.AddPackageCacheToWorkspace();
        }

        /// <summary>
        /// Load settings from disk
        /// </summary>
        public void LoadSettings()
        {
            try
            {
                // Load settings from TheBifrostSettings.json
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    JsonUtility.FromJsonOverwrite(json, this);
                }
                
                // Check for environment variable PORT
                string envPort = System.Environment.GetEnvironmentVariable(EnvUnityPort);
                if (!string.IsNullOrEmpty(envPort) && int.TryParse(envPort, out int port))
                {
                    Port = port;
                }
                string envTimeout = System.Environment.GetEnvironmentVariable(EnvUnityRequestTimeout);
                if (!string.IsNullOrEmpty(envTimeout) && int.TryParse(envTimeout, out int timeout))
                {
                    RequestTimeoutSeconds = timeout;
                }
            }
            catch (Exception ex)
            {
                // Can't use LoggerService here as it depends on settings
                Debug.LogError($"[TheBifrost] Failed to load settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Save settings to disk
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                // Save settings to TheBifrostSettings.json
                string json = JsonUtility.ToJson(this, true);
                File.WriteAllText(SettingsPath, json);

                // Set environment variable PORT for the Node.js process
                // EnvironmentVariableTarget.User and EnvironmentVariableTarget.Machine should be used on .NET implementations running on Windows systems only.
                // For non-Windows systems, User and Machine are treated as Process.
                // Using Process target for broader compatibility.
                // see: https://learn.microsoft.com/en-us/dotnet/api/system.environmentvariabletarget?view=net-8.0#remarks
                Environment.SetEnvironmentVariable(EnvUnityPort, Port.ToString(), EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable(EnvUnityRequestTimeout, RequestTimeoutSeconds.ToString(), EnvironmentVariableTarget.Process);
            }
            catch (Exception ex)
            {
                // Can't use LoggerService here as it might create circular dependency
                Debug.LogError($"[TheBifrost] Failed to save settings: {ex.Message}");
            }
        }
    }
}
