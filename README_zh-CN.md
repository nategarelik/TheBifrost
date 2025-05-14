
# MyPersonalMcpServer 编辑器 (游戏引擎)

[![](https://img.shields.io/badge/Unity-000000?style=flat&logo=unity&logoColor=white 'Unity')](https://unity.com/releases/editor/archive)
[![](https://img.shields.io/badge/Node.js-339933?style=flat&logo=nodedotjs&logoColor=white 'Node.js')](https://nodejs.org/en/download/)
[![](https://img.shields.io/badge/License-MIT-red.svg 'MIT License')](License.md)

| [English](../README.md) | [🇨🇳简体中文](README_zh-CN.md) | [🇯🇵日本語](../README-ja.md) |
|----------------------|---------------------------------|----------------------|


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

MyPersonalMcpServer 是 Model Context Protocol 在 Unity 编辑器中的个性化实现，专为私人使用而设计，允许 AI 助手与您的 Unity 项目交互。这个包提供了 Unity 和实现 MCP 协议的 Node.js 服务器之间的桥梁，使 AI 代理能够在 Unity 编辑器中执行操作。

## 功能

### IDE 集成 - 包缓存访问

MCP Unity 通过将 Unity `Library/PackedCache` 文件夹添加到您的工作区，提供与 VSCode 类 IDE（Visual Studio Code、Cursor、Windsurf）的自动集成。此功能：

- 提高对 Unity 包的代码智能感知
- 为 Unity 包提供更好的自动完成和类型信息
- 帮助 AI 编码助手理解您项目的依赖关系

### MCP 服务器工具

- `execute_menu_item`: 执行 Unity 菜单项（用 MenuItem 属性标记的函数）
  > **示例提示:** "执行菜单项 'GameObject/Create Empty' 创建一个新的空 GameObject"

- `select_gameobject`: 通过路径或实例 ID 选择 Unity 层次结构中的游戏对象
  > **示例提示:** "选择场景中的 Main Camera 对象"

- `update_component`: 更新 GameObject 上的组件字段，如果 GameObject 不包含该组件则添加它
  > **示例提示:** "给 Player 对象添加 Rigidbody 组件并设置其质量为 5"

- `add_package`: 在 Unity 包管理器中安装新包
  > **示例提示:** "给我的项目添加 TextMeshPro 包"

- `run_tests`: 使用 Unity 测试运行器运行测试
  > **示例提示:** "运行我项目中所有的 EditMode 测试"

- `send_console_log`: 发送控制台日志到 Unity
  > **示例提示:** "发送控制台日志到 Unity 编辑器"

- `add_asset_to_scene`: 将 AssetDatabase 中的资源添加到 Unity 场景中
  > **示例提示:** "将我的项目中的 Player 预制体添加到当前场景"

### MCP 服务器资源

- `unity://menu-items`: 获取 Unity 编辑器中所有可用的菜单项列表，以方便 `execute_menu_item` 工具
  > **示例提示:** "显示与 GameObject 创建相关的所有可用菜单项"

- `unity://hierarchy`: 获取 Unity 层次结构中所有游戏对象的列表
  > **示例提示:** "显示当前场景的层次结构"

- `unity://gameobject/{id}`: 通过实例 ID 或场景层次结构中的对象路径获取特定 GameObject 的详细信息，包括所有 GameObject 组件及其序列化的属性和字段
  > **示例提示:** "获取 Player GameObject 的详细信息"

- `unity://logs`: 获取 Unity 控制台的所有日志列表
  > **示例提示:** "显示 Unity 控制台最近的错误消息"

- `unity://packages`: 从 Unity 包管理器获取已安装和可用包的信息
  > **示例提示:** "列出我 Unity 项目中当前安装的所有包"

- `unity://assets`: 获取 Unity 资产数据库中资产的信息
  > **示例提示:** "查找我项目中的所有纹理资产"

- `unity://tests/{testMode}`: 获取 Unity 测试运行器中测试的信息
  > **示例提示:** "列出我 Unity 项目中所有可用的测试"

## 要求
- Unity 2022.3 或更高版本 - 用于[安装服务器](#install-server)
- Node.js 18 或更高版本 - 用于[启动服务器](#start-server)
- npm 9 或更高版本 - 用于[调试服务器](#debug-server)

## <a name="install-server"></a>安装

安装 MCP Unity 服务器是一个多步骤过程：

### 步骤 1: 通过 Unity 包管理器安装 MyPersonalMcpServer 包
1. 打开 Unity 包管理器 (Window > Package Manager)
2. 点击左上角的 "+" 按钮
3. 选择 "Add package from git URL..."
4. 输入: `[您的 MyPersonalMcpServer 私人 Git URL]` (如果是本地包，则从磁盘添加)
5. 点击 "Add"

<!-- 移除包管理器图片 -->

### 步骤 2: 安装 Node.js
> 要运行 MCP Unity 服务器，您需要在计算机上安装 Node.js 18 或更高版本：

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Windows</span></summary>

1. 访问 [Node.js 下载页面](https://nodejs.org/en/download/)
2. 下载 Windows 安装程序 (.msi) 的 LTS 版本（推荐）
3. 运行安装程序并按照安装向导操作
4. 通过打开 PowerShell 并运行以下命令验证安装：
   ```bash
   node --version
   ```
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">macOS</span></summary>

1. 访问 [Node.js 下载页面](https://nodejs.org/en/download/)
2. 下载 macOS 安装程序 (.pkg) 的 LTS 版本（推荐）
3. 运行安装程序并按照安装向导操作
4. 或者，如果您已安装 Homebrew，可以运行：
   ```bash
   brew install node@18
   ```
5. 通过打开终端并运行以下命令验证安装：
   ```bash
   node --version
   ```
</details>

### 步骤 3: 配置 AI LLM 客户端

<details open>
<summary><span style="font-size: 1.1em; font-weight: bold;">选项 1: 使用 Unity 编辑器配置</span></summary>

1. 打开 Unity 编辑器
2. 导航到 Tools > MCP Unity > Server Window
3. 点击 "Configure" 按钮为您的 AI LLM 客户端配置。
<!-- 移除配置客户端图片 -->

4. 使用给定的弹出窗口确认配置安装。
<!-- 移除确认配置图片 -->

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">选项 2: 手动配置</span></summary>

打开您的 AI 客户端的 MCP 配置文件（例如 Claude Desktop 中的 claude_desktop_config.json）并复制以下文本：

> 将 `ABSOLUTE/PATH/TO` 替换为您的 MyPersonalMcpServer 安装的绝对路径，或者直接从 Unity 编辑器 MCP 服务器窗口（Tools > MCP Unity > Server Window）复制文本。

```json
{
  "mcpServers": {
    "mypersonalmcp-server": {
      "command": "node",
      "args": [
        "ABSOLUTE/PATH/TO/MyPersonalMcpServer/Server~/build/index.js"
      ]
    }
  }
}
```

</details>

## <a name="start-server"></a>启动服务器

启动 MCP Unity 服务器有两种方式：

### 选项 1: 通过 Unity 编辑器启动
1. 打开 Unity 编辑器
2. 导航到 Tools > MCP Unity > Server Window
3. 点击 "Start Server" 按钮

### 选项 2: 通过命令行启动
1. 打开终端或命令提示符
2. 导航到 MCP Unity 服务器目录
3. 运行以下命令：
   ```bash
   node Server~/build/index.js
   ```

## 可选：设置超时

默认情况下，MCP 服务器与 WebSocket 之间的超时时间为 10 秒。
您可以根据所使用的操作系统进行更改：

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 1: Windows OS</span></summary>

1. 打开 Unity 编辑器
2. 导航至 Tools > MCP Unity > Server Window
3. 将 "Request Timeout (seconds)" 值更改为所需的超时秒数
4. Unity 会将系统环境变量 UNITY_REQUEST_TIMEOUT 设置为新的超时值
5. 重启 Node.js 服务器
6. 再次点击“启动服务器”，将 Unity 编辑器 Web 套接字重新连接到 Node.js MCP 服务器

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 2: 非Windows操作系统</span></summary>

对于非Windows操作系统，需要配置两个地方：

### 编辑器进程超时

1. 打开 Unity 编辑器
2. 导航至 Tools > MCP Unity > Server Window
3. 将 "Request Timeout (seconds)" 值更改为所需的超时秒数

### WebSocket 超时

1. 在终端中设置 UNITY_REQUEST_TIMEOUT 环境变量
    - Powershell
   ```powershell
   $env:UNITY_REQUEST_TIMEOUT = "300"
   ```
    - Command Prompt/Terminal
   ```cmd
   set UNITY_REQUEST_TIMEOUT=300
   ```
2. 重启 Node.js 服务器
3. 再次点击“启动服务器”，将 Unity 编辑器 Web 套接字重新连接到 Node.js MCP 服务器

</details>

> [!TIP]  
> 您的 AI 编码 IDE（例如，Claude Desktop、Cursor IDE、Windsurf IDE）和 MCP 服务器之间的超时取决于 IDE。

## <a name="debug-server"></a>调试服务器

要调试 MCP Unity 服务器，您可以使用以下方法：

### 选项 1: 使用 Unity 编辑器调试
1. 打开 Unity 编辑器
2. 导航到 Tools > MCP Unity > Server Window
3. 点击 "Debug Server" 按钮

### 选项 2: 使用命令行调试
1. 打开终端或命令提示符
2. 导航到 MyPersonalMcpServer 服务器目录
3. 运行以下命令：
   ```bash
   npm run debug
   ```

## 故障排除

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">连接问题</span></summary>

- 确保 WebSocket 服务器正在运行（检查 Unity 的 Server Window）
- 检查是否有防火墙限制阻止连接
- 确认端口号正确（默认是 8080）
- 可在 Unity 编辑器 MCP Server 窗口更改端口号。(工具 > MCP Unity > Server Window)
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">服务器无法启动</span></summary>

- 检查 Unity 控制台是否有错误消息
- 确保 Node.js 已正确安装并可在 PATH 中访问
- 验证 Server 目录下所有依赖均已安装
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">运行播放模式测试时连接失败</span></summary>

`run_tests` 工具返回以下响应：
```
Error:
Connection failed: Unknown error
```

发生此错误的原因是在切换到播放模式时域重新加载，导致桥接连接丢失。  
解决方法是在 **Edit > Project Settings > Editor > "Enter Play Mode Settings"** 中关闭 **Reload Domain**。
</details>

## 支持与反馈

这是一个个人工具。支持由用户自行提供。

## 贡献

这是一个个人工具，不适用贡献。

## 许可证

本项目采用 [MIT License](License.md) 授权。

## 鸣谢

- [Model Context Protocol](https://modelcontextprotocol.io)
- [Unity Technologies](https://unity.com)
- [Node.js](https://nodejs.org)
- [WebSocket-Sharp](https://github.com/sta/websocket-sharp)
