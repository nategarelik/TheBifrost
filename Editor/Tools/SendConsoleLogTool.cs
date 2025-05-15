using System.Threading.Tasks;
using TheBifrost.Unity;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace TheBifrost.Tools
{
    /// <summary>
    /// Tool for sending notification messages to the Unity console
    /// </summary>
    public class SendConsoleLogTool : McpToolBase
    {
        public SendConsoleLogTool()
        {
            Name = "send_console_log";
            Description = "Sends a message to the Unity console";
        }
        
        /// <summary>
        /// Execute the NotifyMessage tool with the provided parameters synchronously
        /// </summary>
        /// <param name="parameters">Tool parameters as a JObject</param>
        public override JObject Execute(JObject parameters)
        {
            // Extract parameters
            string message = parameters["message"]?.ToObject<string>();
            string type = parameters["type"]?.ToObject<string>()?.ToLower() ?? "info";
 
            if (string.IsNullOrEmpty(message))
            {
                return TheBifrostSocketHandler.CreateErrorResponse(
                    "Required parameter 'message' not provided", 
                    "validation_error"
                );
            }
 
            // Log the message based on type
            switch (type)
            {
                case "error":
                    Debug.LogError($"[The Bifrost]: {message}");
                    break;
                case "warning":
                    Debug.LogWarning($"[The Bifrost]: {message}");
                    break;
                default:
                    Debug.Log($"[The Bifrost]: {message}");
                    break;
            }
 
            // Create the response
            return new JObject
            {
                ["success"] = true,
                ["type"] = "text",
                ["message"] = $"Message displayed: {message}"
            };
        }
    }
}
