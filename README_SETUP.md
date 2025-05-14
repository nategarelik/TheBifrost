# TheBifrost MCP Server Setup Guide

This guide explains how to set up and use the TheBifrost MCP server with your Unity projects.

## Installation

1. Add the TheBifrost package to your Unity project:
   - Via Package Manager: Add package from git URL `https://github.com/nategarelik/TheBifrost.git`
   - Or copy the TheBifrost folder directly into your project's Assets folder

> **Note:** The server will be automatically built when you start it for the first time. You don't need to manually build it.

## Starting the Server

### Option 1: From Unity Editor (Recommended)

1. Open your Unity project
2. Navigate to Tools > TheBifrost > Server Window
3. Click "Start Server"
4. The server will automatically build (if needed) and start

### Option 2: Using the Batch File

1. Run the `start_mcp_server.bat` file in your Unity project root
2. The server will automatically build (if needed) and start

### Option 3: Manual Start

1. Open a terminal/command prompt
2. Navigate to your Unity project directory
3. Run `node TheBifrost/Server~/build/index.js`

## Configuring AI Clients

### Claude Desktop

1. Open your Unity project
2. Navigate to Tools > TheBifrost > Server Window
3. Click "Configure" under Claude Desktop
4. The configuration will be automatically added to Claude Desktop

### Manual Configuration

If you need to manually configure your AI client, use the following configuration:

```json
{
  "mcpServers": {
    "thebifrost-server": {
      "command": "node",
      "args": [
        "PATH_TO_YOUR_UNITY_PROJECT/TheBifrost/Server~/build/index.js"
      ]
    }
  }
}
```

Replace `PATH_TO_YOUR_UNITY_PROJECT` with the absolute path to your Unity project.

## Troubleshooting

### Server Not Found

If you see the error "[MCP Unity] Could not locate Server directory":

1. Make sure the TheBifrost package is properly installed in your Unity project
2. Check that the package name in `TheBifrost/Editor/UnityBridge/McpUnitySettings.cs` matches the name in `TheBifrost/package.json`
3. Verify that the Server~ directory exists

### Build Issues

If you encounter issues with the automatic server build:

1. Check that Node.js and npm are properly installed and accessible in your PATH
2. Try building the server manually:
   - Navigate to the TheBifrost/Server~ directory
   - Run `npm install` to install dependencies
   - Run `npm run build` to build the server
3. Check the Unity console for detailed error messages

### Connection Issues

If the AI client cannot connect to the server:

1. Make sure the server is running (check the terminal output)
2. Verify that the port number is correct (default is 8090)
3. Check that the path in your AI client configuration points to the correct location