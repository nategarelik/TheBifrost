// using UnityEngine;
// using TheBifrost.Unity;
//
// namespace TheBifrost.Utils
// {
//     /// <summary>
//     /// Special logger to use inside the MCP Unity Editor project
//     /// </summary>
//     public static class McpLogger
//     {
//         private const string LogPrefix = "[The Bifrost] ";
//         
//         /// <summary>
//         /// Log an info message if info logs are enabled
//         /// </summary>
//         /// <param name="message">Message to log</param>
//         public static void LogInfo(string message)
//         {
//             if (TheBifrostSettings.Instance.EnableInfoLogs)
//             {
//                 Debug.Log($"{LogPrefix}{message}");
//             }
//         }
//         
//         /// <summary>
//         /// Log a warning message
//         /// </summary>
//         /// <param name="message">Message to log</param>
//         public static void LogWarning(string message)
//         {
//             Debug.LogWarning($"{LogPrefix}{message}");
//         }
//         
//         /// <summary>
//         /// Log an error message
//         /// </summary>
//         /// <param name="message">Message to log</param>
//         public static void LogError(string message)
//         {
//             Debug.LogError($"{LogPrefix}{message}");
//         }
//     }
// }
