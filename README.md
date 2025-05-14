# The Bifrost (Game Engine)

[![](https://img.shields.io/badge/Unity-000000?style=flat&logo=unity&logoColor=white 'Unity')](https://unity.com/releases/editor/archive)
[![](https://img.shields.io/badge/Node.js-339933?style=flat&logo=nodedotjs&logoColor=white 'Node.js')](https://nodejs.org/en/download/)
[![](https://img.shields.io/badge/License-MIT-red.svg 'MIT License')](License.md)

```                                                                        
                              ,/(/.   *(/,                                  
                          */(((((/.   *((((((*.                             
                     .*((((((((((/.   *((((((((((/.                         
                 ./((((((((((((((/    *((((((((((((((/,                     
             ,/(((((((((((((/*.           */(((((((((((((/*.                
            ,%%#((/((((((*                    ,/(((((/(#&@@(                
            ,%%##%%##((((((/*.             ,/((((/(#&@@@@@@(                
            ,%%######%%##((/(((/*.    .*/(((//(%@@@@@@@@@@@(                
            ,%%####%#(%%#%%##((/((((((((//#&@@@@@@&@@@@@@@@(                
            ,%%####%(    /#%#%%%##(//(#@@@@@@@%,   #@@@@@@@(                
            ,%%####%(        *#%###%@@@@@@(        #@@@@@@@(                
            ,%%####%(           #%#%@@@@,          #@@@@@@@(                
            ,%%##%%%(           #%#%@@@@,          #@@@@@@@(                
            ,%%%#*              #%#%@@@@,             *%@@@(                
            .,      ,/##*.      #%#%@@@@,     ./&@#*      *`                
                ,/#%#####%%#/,  #%#%@@@@, ,/&@@@@@@@@@&\.                    
                 `*#########%%%%###%@@@@@@@@@@@@@@@@@@&*´                   
                    `*%%###########%@@@@@@@@@@@@@@&*´                        
                        `*%%%######%@@@@@@@@@@&*´                            
                            `*#%%##%@@@@@&*´                                 
                               `*%#%@&*´                                     
                                                       
     ███╗   ███╗ ██████╗██████╗         ██╗   ██╗███╗   ██╗██╗████████╗██╗   ██╗
     ████╗ ████║██╔════╝██╔══██╗        ██║   ██║████╗  ██║██║╚══██╔══╝╚██╗ ██╔╝
     ██╔████╔██║██║     ██████╔╝        ██║   ██║██╔██╗ ██║██║   ██║    ╚████╔╝ 
     ██║╚██╔╝██║██║     ██╔═══╝         ██║   ██║██║╚██╗██║██║   ██║     ╚██╔╝  
     ██║ ╚═╝ ██║╚██████╗██║             ╚██████╔╝██║ ╚████║██║   ██║      ██║
     ╚═╝     ╚═╝ ╚═════╝╚═╝              ╚═════╝ ╚═╝  ╚═══╝╚═╝   ╚═╝      ╚═╝
```

The Bifrost is a personalized implementation of the Model Context Protocol for Unity Editor, designed for private use to allow AI assistants to interact with your Unity projects. This package provides a bridge between Unity and a Node.js server that implements the MCP protocol, enabling AI agents to execute operations within the Unity Editor.

## Features

### IDE Integration - Package Cache Access

MCP Unity provides automatic integration with VSCode-like IDEs (Visual Studio Code, Cursor, Windsurf) by adding the Unity `Library/PackedCache` folder to your workspace. This feature:

- Improves code intelligence for Unity packages
- Enables better autocompletion and type information for Unity packages
- Helps AI coding assistants understand your project's dependencies

### MCP Server Tools

- `execute_menu_item`: Executes Unity menu items (functions tagged with the MenuItem attribute)
  > **Example prompt:** "Execute the menu item 'GameObject/Create Empty' to create a new empty GameObject"

- `select_gameobject`: Selects game objects in the Unity hierarchy by path or instance ID
  > **Example prompt:** "Select the Main Camera object in my scene"

- `update_component`: Updates component fields on a GameObject or adds it to the GameObject if it does not contain the component
  > **Example prompt:** "Add a Rigidbody component to the Player object and set its mass to 5"

- `add_package`: Installs new packages in the Unity Package Manager
  > **Example prompt:** "Add the TextMeshPro package to my project"

- `run_tests`: Runs tests using the Unity Test Runner
  > **Example prompt:** "Run all the EditMode tests in my project"

- `send_console_log`: Send a console log to Unity
  > **Example prompt:** "Send a console log to Unity Editor"

- `add_asset_to_scene`: Adds an asset from the AssetDatabase to the Unity scene
  > **Example prompt:** "Add the Player prefab from my project to the current scene"

### MCP Server Resources

- `unity://menu-items`: Retrieves a list of all available menu items in the Unity Editor to facilitate `execute_menu_item` tool
  > **Example prompt:** "Show me all available menu items related to GameObject creation"

- `unity://hierarchy`: Retrieves a list of all game objects in the Unity hierarchy
  > **Example prompt:** "Show me the current scene hierarchy structure"

- `unity://gameobject/{id}`: Retrieves detailed information about a specific GameObject by instance ID or object path in the scene hierarchy, including all GameObject components with it's serialized properties and fields
  > **Example prompt:** "Get me detailed information about the Player GameObject"

- `unity://logs`: Retrieves a list of all logs from the Unity console
  > **Example prompt:** "Show me the recent error messages from the Unity console"

- `unity://packages`: Retrieves information about installed and available packages from the Unity Package Manager
  > **Example prompt:** "List all the packages currently installed in my Unity project"

- `unity://assets`: Retrieves information about assets in the Unity Asset Database
  > **Example prompt:** "Find all texture assets in my project"

- `unity://tests/{testMode}`: Retrieves information about tests in the Unity Test Runner
  > **Example prompt:** "List all available tests in my Unity project"

## Requirements
- Unity 2022.3 or later - to [install the server](#install-server)
- Node.js 18 or later - to [start the server](#start-server)
- npm 9 or later - to [debug the server](#debug-server)

## <a name="install-server"></a>Installation

Installing this MCP Unity Server is a multi-step process:

### Step 1: Install The Bifrost package via Unity Package Manager
1. Open the Unity Package Manager (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter: `[Your Private Git URL for TheBifrost]`. If you have cloned the repository locally, you can also select "Add package from disk..." and navigate to the `TheBifrost` directory.
5. Click "Add"

<!-- Removed package manager image -->

### Step 2: Install Node.js
> To run MCP Unity server, you'll need to have Node.js 18 or later installed on your computer:

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Windows</span></summary>

1. Visit the [Node.js download page](https://nodejs.org/en/download/)
2. Download the Windows Installer (.msi) for the LTS version (recommended)
3. Run the installer and follow the installation wizard
4. Verify the installation by opening PowerShell and running:
   ```bash
   node --version
   ```
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">macOS</span></summary>

1. Visit the [Node.js download page](https://nodejs.org/en/download/)
2. Download the macOS Installer (.pkg) for the LTS version (recommended)
3. Run the installer and follow the installation wizard
4. Alternatively, if you have Homebrew installed, you can run:
   ```bash
   brew install node@18
   ```
5. Verify the installation by opening Terminal and running:
   ```bash
   node --version
   ```
</details>

### Step 3: Configure AI LLM Client

<details open>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 1: Configure using Unity Editor</span></summary>

1. Open the Unity Editor
2. Navigate to Tools > MyPersonalMcp Server > Server Window
3. Click on the "Configure" button for your AI LLM client.
<!-- Removed configure client image -->

4. Confirm the configuration installation with the given popup.
<!-- Removed confirm config image -->

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 2: Configure Manually</span></summary>

Open the MCP configuration file of your AI client (e.g. claude_desktop_config.json in Claude Desktop) and copy the following text:

> Replace `ABSOLUTE/PATH/TO` with the absolute path to your TheBifrost installation or just copy the text from the Unity Editor MCP Server window (Tools > MyPersonalMcp Server > Server Window).

```json
{
   "mcpServers": {
       "thebifrost-server": {
          "command": "node",
          "args": [
             "ABSOLUTE/PATH/TO/TheBifrost/Server~/build/index.js"
          ]
       }
   }
}
```

</details>

## <a name="start-server"></a>Start Unity Editor MCP Server
1. Open the Unity Editor
2. Navigate to Tools > MCP Unity > Server Window
3. Click "Start Server" to start the WebSocket server
4. Open Claude Desktop or your AI Coding IDE (e.g. Cursor IDE, Windsurf IDE, etc.) and start executing Unity tools.
<!-- Removed connect image -->

> When the AI client connects to the WebSocket server, it will automatically show in the green box in the window.

## Optional: Set WebSocket Port
By default, the WebSocket server runs on port 8090. You can change this port in two ways:

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 1: Using the Unity Editor</span></summary>

1. Open the Unity Editor
2. Navigate to Tools > MCP Unity > Server Window
3. Change the "WebSocket Port" value to your desired port number
4. Unity will setup the system environment variable UNITY_PORT to the new port number
5. Restart the Node.js server
6. Click again on "Start Server" to reconnect the Unity Editor web socket to the Node.js MCP Server

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 2: Using the terminal</span></summary>

1. Set the UNITY_PORT environment variable in the terminal
   - Powershell
   ```powershell
   $env:UNITY_PORT = "8090"
   ```
   - Command Prompt/Terminal
   ```cmd
   set UNITY_PORT=8090
   ```
2. Restart the Node.js server
3. Click again on "Start Server" to reconnect the Unity Editor web socket to the Node.js MCP Server

</details>

## Optional: Set Timeout

By default, the timeout between the MCP server and the WebSocket is 10 seconds.
You can change depending on the OS you are using:

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 1: Windows OS</span></summary>

1. Open the Unity Editor
2. Navigate to Tools > MCP Unity > Server Window
3. Change the "Request Timeout (seconds)" value to your desired timeout seconds
4. Unity will setup the system environment variable UNITY_REQUEST_TIMEOUT to the new timeout value
5. Restart the Node.js server
6. Click again on "Start Server" to reconnect the Unity Editor web socket to the Node.js MCP Server

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 2: Non-Windows OS</span></summary>

For non-Windows OS, you need to configure two places:

### In Editor Process Timeout

1. Open the Unity Editor
2. Navigate to Tools > MCP Unity > Server Window
3. Change the "Request Timeout (seconds)" value to your desired timeout seconds

### WebSocket Timeout

1. Set the UNITY_REQUEST_TIMEOUT environment variable in the terminal
    - Powershell
   ```powershell
   $env:UNITY_REQUEST_TIMEOUT = "300"
   ```
    - Command Prompt/Terminal
   ```cmd
   set UNITY_REQUEST_TIMEOUT=300
   ```
2. Restart the Node.js server
3. Click again on "Start Server" to reconnect the Unity Editor web socket to the Node.js MCP Server

</details>

> [!TIP]  
> The timeout between your AI Coding IDE (e.g., Claude Desktop, Cursor IDE, Windsurf IDE) and the MCP Server depends on the IDE.

## <a name="debug-server"></a>Debugging the Server

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Building the Node.js Server</span></summary>

The MCP Unity server is built using Node.js . It requires to compile the TypeScript code to JavaScript in the `build` directory.
To build the server, open a terminal and:

1. Navigate to the Server directory:
   ```bash
   cd ABSOLUTE/PATH/TO/TheBifrost/Server~
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Build the server:
   ```bash
   npm run build
   ```

4. Run the server:
   ```bash
   node build/index.js
   ```

</details>
   
<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Debugging with MCP Inspector</span></summary>

Debug the server with [@modelcontextprotocol/inspector](https://github.com/modelcontextprotocol/inspector) (if applicable for private use):
   - Powershell
   ```powershell
   npx @modelcontextprotocol/inspector node TheBifrost/Server~/build/index.js
   ```
   - Command Prompt/Terminal
   ```cmd
   npx @modelcontextprotocol/inspector node TheBifrost/Server~/build/index.js
   ```

Don't forget to shutdown the server with `Ctrl + C` before closing the terminal or debugging it with the [MCP Inspector](https://github.com/modelcontextprotocol/inspector) (if applicable).

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Enable Console Logs</span></summary>

1. Enable logging on your terminal or into a log.txt file:
   - Powershell
   ```powershell
   $env:LOGGING = "true"
   $env:LOGGING_FILE = "true"
   ```
   - Command Prompt/Terminal
   ```cmd
   set LOGGING=true
   set LOGGING_FILE=true
   ```

</details>

## Troubleshooting

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Connection Issues</span></summary>

- Ensure the WebSocket server is running (check the Server Window in Unity)
- Send a console log message from MCP client to force a reconnection between MCP client and Unity server
- Change the port number in the Unity Editor MCP Server window. (Tools > MCP Unity > Server Window)
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Server Not Starting</span></summary>

- Check the Unity Console for error messages
- Ensure Node.js is properly installed and accessible in your PATH
- Verify that all dependencies are installed in the Server directory
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Connection failed when running Play Mode tests</span></summary>

The `run_tests` tool returns the following response:
```
Error:
Connection failed: Unknown error
```

This error occurs because the bridge connection is lost when the domain reloads upon switching to Play Mode.  
The workaround is to turn off **Reload Domain** in **Edit > Project Settings > Editor > "Enter Play Mode Settings"**.
</details>

## Support & Feedback

This is a personal tool. Support is self-provided.

## Contributing

This is a personal tool, contributions are not applicable.

## License

This project is under [MIT License](License.md)

## Acknowledgements

- [Model Context Protocol](https://modelcontextprotocol.io)
- [Unity Technologies](https://unity.com)
- [Node.js](https://nodejs.org)
- [WebSocket-Sharp](https://github.com/sta/websocket-sharp)
