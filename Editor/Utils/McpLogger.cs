using UnityEngine;

namespace MyPersonalMcp.Utils
{
    /// <summary>
    /// Logger utility for MCP related logging
    /// </summary>
    public static class McpLogger
    {
        [System.Diagnostics.Conditional("MCP_DEBUG")]
        public static void Log(string message)
        {
            Debug.Log($"[MCP] {message}");
        }

        [System.Diagnostics.Conditional("MCP_DEBUG")]
        public static void LogInfo(string message)
        {
            Debug.Log($"[MCP] [INFO] {message}");
        }

        [System.Diagnostics.Conditional("MCP_DEBUG")]
        public static void LogWarning(string message)
        {
            Debug.LogWarning($"[MCP] [WARNING] {message}");
        }

        [System.Diagnostics.Conditional("MCP_DEBUG")]
        public static void LogError(string message)
        {
            Debug.LogError($"[MCP] [ERROR] {message}");
        }
    }
}
