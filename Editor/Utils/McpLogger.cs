using UnityEngine;

namespace TheBifrost.Utils
{
    /// <summary>
    /// Logger utility for MCP related logging
    /// </summary>
    public static class McpLogger
    {
        public static void Log(string message)
        {
            // Basic log, could be controlled by a general "EnableDebugLogs" if needed
            if (TheBifrost.Unity.TheBifrostSettings.Instance.EnableInfoLogs) // Example: Tie to existing setting for now
            {
                 Debug.Log($"[TheBifrost] {message}");
            }
        }

        public static void LogInfo(string message)
        {
            // Info logs should appear if EnableInfoLogs is true
            if (TheBifrost.Unity.TheBifrostSettings.Instance.EnableInfoLogs)
            {
                Debug.Log($"[TheBifrost] [INFO] {message}");
            }
        }

        public static void LogWarning(string message)
        {
            // Warnings should always appear
            Debug.LogWarning($"[TheBifrost] [WARNING] {message}");
        }

        public static void LogError(string message)
        {
            // Errors should always appear
            Debug.LogError($"[TheBifrost] [ERROR] {message}");
        }
    }
}
