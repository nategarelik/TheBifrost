# The Bifrost エディター (ゲームエンジン)

[![](https://img.shields.io/badge/Unity-000000?style=flat&logo=unity&logoColor=white 'Unity')](https://unity.com/releases/editor/archive)
[![](https://img.shields.io/badge/Node.js-339933?style=flat&logo=nodedotjs&logoColor=white 'Node.js')](https://nodejs.org/en/download/)
[![](https://img.shields.io/badge/License-MIT-red.svg 'MIT License')](License.md)

| [English](../README.md) | [🇨🇳简体中文](../README_zh-CN.md) | [🇯🇵日本語](README-ja.md) |
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

The Bifrostは、Unityエディター向けのModel Context Protocolのパーソナライズされた実装であり、Nathaniel Garelikによって、AIアシスタントがUnityプロジェクトと対話できるようにするためのプライベート利用を目的としています。このパッケージは、UnityとMCPプロトコルを実装するNode.jsサーバー間のブリッジを提供し、AIエージェントがUnityエディター内で操作を実行できるようにします。

## 機能

### IDE統合 - パッケージキャッシュアクセス

The Bifrostは、Unityの`Library/PackedCache`フォルダーをワークスペースに追加することで、VSCode系IDE（Visual Studio Code、Cursor、Windsurf）との自動統合を提供します。この機能により：

- Unityパッケージのコードインテリジェンスが向上
- Unityパッケージのより良いオートコンプリートと型情報が有効化
- AIコーディングアシスタントがプロジェクトの依存関係を理解するのに役立つ

### MCPサーバーツール

- `execute_menu_item`: Unityメニュー項目（MenuItem属性でタグ付けされた関数）を実行
  > **例:** "新しい空のGameObjectを作成するためにメニュー項目'GameObject/Create Empty'を実行"

- `select_gameobject`: パスまたはインスタンスIDでUnity階層内のゲームオブジェクトを選択
  > **例:** "シーン内のMain Cameraオブジェクトを選択"

- `update_component`: GameObject上のコンポーネントフィールドを更新、またはGameObjectに含まれていない場合は追加
  > **例:** "PlayerオブジェクトにRigidbodyコンポーネントを追加し、その質量を5に設定"

- `add_package`: Unityパッケージマネージャーに新しいパッケージをインストール
  > **例:** "プロジェクトにTextMeshProパッケージを追加"

- `run_tests`: Unityテストランナーを使用してテストを実行
  > **例:** "プロジェクト内のすべてのEditModeテストを実行"

- `send_console_log`: Unityにコンソールログを送信
  > **例:** "Unity Editorにコンソールログを送信"

- `add_asset_to_scene`: AssetDatabaseからアセットをUnityシーンに追加
  > **例:** "プロジェクトからPlayerプレハブを現在のシーンに追加"

### MCPサーバーリソース

- `unity://menu-items`: `execute_menu_item`ツールを容易にするために、Unityエディターで利用可能なすべてのメニュー項目のリストを取得
  > **例:** "GameObject作成に関連する利用可能なすべてのメニュー項目を表示"

- `unity://hierarchy`: Unity階層内のすべてのゲームオブジェクトのリストを取得
  > **例:** "現在のシーンの階層構造を表示"

- `unity://gameobject/{id}`: シーン階層内のインスタンスIDまたはオブジェクトパスで特定のGameObjectに関する詳細情報を取得
  > **例:** "Player GameObjectに関する詳細情報を取得"

- `unity://logs`: Unityコンソールからのすべてのログのリストを取得
  > **例:** "Unityコンソールからの最近のエラーメッセージを表示"

- `unity://packages`: Unityパッケージマネージャーからインストール済みおよび利用可能なパッケージに関する情報を取得
  > **例:** "Unityプロジェクトに現在インストールされているすべてのパッケージをリスト"

- `unity://assets`: Unityアセットデータベース内のアセットに関する情報を取得
  > **例:** "プロジェクト内のすべてのテクスチャアセットを検索"

- `unity://tests/{testMode}`: Unityテストランナー内のテストに関する情報を取得
  > **例:** "Unityプロジェクトで利用可能なすべてのテストをリスト"

## 要件
- Unity 2022.3以降 - [サーバーをインストール](#install-server)するため
- Node.js 18以降 - [サーバーを起動](#start-server)するため
- npm 9以降 - [サーバーをデバッグ](#debug-server)するため

## <a name="install-server"></a>インストール

このMCP Unityサーバーのインストールは複数ステップのプロセスです：

### ステップ1: Unityパッケージマネージャー経由でThe Bifrostパッケージをインストール
1. Unityパッケージマネージャーを開く（Window > Package Manager）
2. 左上隅の"+"ボタンをクリック
3. "Add package from git URL..."を選択
4. 入力: `[あなたのThe BifrostプライベートGit URL]` (ローカルパッケージの場合はディスクから追加)
5. "Add"をクリック

<!-- パッケージマネージャー画像を削除 -->

### ステップ2: Node.jsをインストール
> The Bifrostサーバーを実行するには、コンピューターにNode.js 18以降がインストールされている必要があります：

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Windows</span></summary>

1. [Node.jsダウンロードページ](https://nodejs.org/en/download/)にアクセス
2. LTSバージョンのWindowsインストーラー（.msi）をダウンロード（推奨）
3. インストーラーを実行し、インストールウィザードに従う
4. PowerShellを開いて以下を実行してインストールを確認：
   ```bash
   node --version
   ```
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">macOS</span></summary>

1. [Node.jsダウンロードページ](https://nodejs.org/en/download/)にアクセス
2. LTSバージョンのmacOSインストーラー（.pkg）をダウンロード（推奨）
3. インストーラーを実行し、インストールウィザードに従う
4. または、Homebrewがインストールされている場合は以下を実行：
   ```bash
   brew install node@18
   ```
5. ターミナルを開いて以下を実行してインストールを確認：
   ```bash
   node --version
   ```
</details>

### ステップ3: AI LLMクライアントを設定

<details open>
<summary><span style="font-size: 1.1em; font-weight: bold;">オプション1: Unityエディターを使用して設定</span></summary>

1. Unityエディターを開く
2. Navigate to Tools > TheBifrost > Server Window
3. AI LLMクライアントの"Configure"ボタンをクリック
<!-- 設定クライアント画像を削除 -->

4. 表示されるポップアップで設定インストールを確認
<!-- 設定確認画像を削除 -->

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">オプション2: 手動設定</span></summary>

AIクライアントのMCP設定ファイル（例：Claude Desktopのclaude_desktop_config.json）を開き、以下のテキストをコピー：

> `ABSOLUTE/PATH/TO`をThe Bifrostインストールの絶対パスに置き換えるか、UnityエディターMCPサーバーウィンドウ（Tools > TheBifrost > Server Window）からテキストをコピー

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

## <a name="start-server"></a>サーバーの起動

The Bifrostサーバーを起動するには2つの方法があります：

### オプション1: Unityエディター経由で起動
1. Unityエディターを開く
2. Tools > MCP Unity > Server Windowに移動
3. "Start Server"ボタンをクリック

### オプション2: コマンドラインから起動
1. ターミナルまたはコマンドプロンプトを開く
2. MCP Unityサーバーディレクトリに移動
3. 以下のコマンドを実行：
   ```bash
   node Server~/build/index.js
   ```

## オプション: タイムアウト設定

デフォルトでは、MCPサーバーとWebSocket間のタイムアウトは 10 秒です。
お使いのOSに応じて変更できます。

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 1: Windows OS</span></summary>

1. Unityエディターを開きます
2. **Tools > TheBifrost > Server Window** に移動します
3. **Request Timeout (seconds)** の値を希望のタイムアウト秒数に変更します
4. Unityはシステム環境変数UNITY_REQUEST_TIMEOUTに新しいタイムアウト値を設定します
5. Node.jsサーバーを再起動します
6. **Start Server** をもう一度クリックして、UnityエディターのWebソケットをNode.js MCPサーバーに再接続します

</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Option 2: Windows以外のOS</span></summary>

Windows 以外の OS の場合は、次の 2 か所で設定する必要があります。

### エディター内プロセスのタイムアウト
1. Unityエディターを開きます
2. **Tools > TheBifrost > Server Window** に移動します
3. **Request Timeout (seconds)** の値を希望のタイムアウト秒数に変更します

### WebSocketのタイムアウト
1. ターミナルで UNITY_REQUEST_TIMEOUT 環境変数を設定します
   - Powershell
   ```powershell
   $env:UNITY_REQUEST_TIMEOUT = "300"
   ```
   - Command Prompt/Terminal
   ```cmd
   set UNITY_REQUEST_TIMEOUT=300
   ```
2. Node.jsサーバーを再起動します
3. **Start Server** をもう一度クリックして、UnityエディターのWebソケットをNode.js MCPサーバーに再接続します

</details>

> [!TIP]
> AIコーディングIDE（Claude Desktop、Cursor IDE、Windsurf IDE など）とMCPサーバー間のタイムアウト設定は、IDEによって異なります。

## <a name="debug-server"></a>サーバーのデバッグ

The Bifrostサーバーをデバッグするには、以下の方法を使用できます：

### オプション1: Unityエディターを使用してデバッグ
1. Unityエディターを開く
2. Tools > MCP Unity > Server Windowに移動
3. "Debug Server"ボタンをクリック

### オプション2: コマンドラインを使用してデバッグ
1. ターミナルまたはコマンドプロンプトを開く
2. MyPersonalMcpServerサーバーディレクトリに移動
3. 以下のコマンドを実行：
   ```bash
   npm run debug
   ```

## トラブルシューティング

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">接続の問題</span></summary>

- WebSocketサーバーが実行中であることを確認してください（UnityのServer Windowを確認）
- Send a console log message from MCP client to force a reconnection between MCP client and Unity server
- ポート番号が正しいことを確認してください（デフォルトは8090）
- UnityエディターのThe Bifrost Serverウィンドウでポート番号を変更できます（ツール > TheBifrost > Server Window）
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">サーバーが起動しない</span></summary>

- Check the Unity Console for error messages
- Ensure Node.js is properly installed and accessible in your PATH
- Verify that all dependencies are installed in the Server directory
</details>

<details>
<summary><span style="font-size: 1.1em; font-weight: bold;">Play Modeテスト実行時の接続失敗</span></summary>

The `run_tests` tool returns the following response:
```
Error:
Connection failed: Unknown error
```

This error occurs because the bridge connection is lost when the domain reloads upon switching to Play Mode.
The workaround is to turn off **Reload Domain** in **Edit > Project Settings > Editor > "Enter Play Mode Settings"**.
</details>

## サポート・フィードバック

これは個人用ツールです。サポートは自己提供となります。

## コントリビューション

これは個人用ツールのため、コントリビューションは適用されません。

## ライセンス

本プロジェクトは [MIT License](License.md) の下で提供されています。

## 謝辞

- [Model Context Protocol](https://modelcontextprotocol.io)
- [Unity Technologies](https://unity.com)
- [Node.js](https://nodejs.org)
- [WebSocket-Sharp](https://github.com/sta/websocket-sharp)
