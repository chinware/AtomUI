# Gallery NativeAOT Release Workflow

这份文档记录 Gallery 发布 workflow 的 NativeAOT 开关、发布脚本职责和验证规则。平台 NativeAOT 工具链细节仍以 [windows-native-aot-publish.md](windows-native-aot-publish.md)、[linux-native-aot-publish.md](linux-native-aot-publish.md) 和 [aot-programming-guidelines.md](aot-programming-guidelines.md) 为准。

## 目标

Gallery 发布管理员手动运行 `.github/workflows/release-gallery.yml` 时，可以选择是否使用 NativeAOT 发布。正式发布默认使用 NativeAOT；取消勾选只用于临时排障或兼容性验证。

## Workflow 输入

`release-gallery.yml` 的 `workflow_dispatch.inputs` 包含：

```yaml
PublishAot:
   description: 'Publish Gallery with NativeAOT'
   type: boolean
   default: true
```

所有 Gallery 平台发布任务都把该输入传给 `controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1`：

```powershell
-publishAot '${{ inputs.PublishAot }}'
```

## AtomUITools Checkout

发布流程会拉取 private 仓库 `AtomUI/AtomUITools` 的 `develop` 分支来构建打包工具。由于这是跨仓库 checkout，`GITHUB_TOKEN` 不能默认读取该仓库，必须传入 `secrets.ACCESS_TOKEN`。

`AtomUITools` 和主仓库同属 `AtomUI` 组织，但标准 `GITHUB_TOKEN` 仍不应假定可读取另一个 private 仓库。`ACCESS_TOKEN` 应是 GitHub PAT 或 fine-grained token，并至少具备 `AtomUI/AtomUITools` 的只读 Contents 权限。这个 token 过期、未授权到该仓库，或没有配置到当前发布仓库的 Actions secrets 时，`actions/checkout` 会在 `Fetching the repository` 阶段失败。

`AtomUITools` 源码 checkout 到 `AtomUIToolsSourceDir`，构建输出写入 `AtomUIToolsBinDir`。这两个目录不能只靠大小写区分；Windows 和 macOS runner 的文件系统通常大小写不敏感，`AtomUITools` 与 `atomuitools` 会指向同一个目录。

## 发布脚本职责

`PublishToLocal.ps1` 统一处理 AOT 和非 AOT 两条路径。

当 `publishAot` 为 `true`：

- 要求 `buildType` 为 `Release`。
- 先执行显式 restore，并传入 `-p:Configuration=Release -p:GalleryPublishAot=true`。
- `AtomUIGallery.Desktop.csproj` 将 `GalleryPublishAot` 映射到本项目的 `PublishAot`。不要在命令行直接传全局 `PublishAot=true`，否则 source generator 等非最终可执行项目也会收到该属性，并可能触发 `NETSDK1207`。
- 检查 `project.assets.json` 包含 `Microsoft.DotNet.ILCompiler`，并存在目标 RID 对应的 restore target。
- 再执行 `dotnet publish --no-restore`，并显式传入 `-p:GalleryPublishAot=true`。
- 检查输出目录里不存在普通 self-contained runtime 标志文件。

当 `publishAot` 为 `false`：

- 保留原有普通 self-contained single-file 发布路径。
- 该模式不作为默认正式发布形态。

## 产物校验规则

NativeAOT 发布成功不只看 `dotnet publish` 退出码。脚本还会拒绝包含下列文件的输出目录：

```text
coreclr.dll
clrjit.dll
libcoreclr.so
libcoreclr.dylib
libclrjit.so
libclrjit.dylib
System.Private.CoreLib.dll
AtomUIGallery.Desktop.dll
```

这些文件出现时，说明产物很可能退化成普通 self-contained 发布。

## 平台前置条件

macOS 任务安装：

```bash
brew install create-dmg openssl@3
```

`AtomUIGallery.Desktop.csproj` 保留 Apple Silicon 和 Intel Homebrew OpenSSL linker 搜索路径。

Linux AppImage 任务安装：

```bash
sudo apt-get install -y fuse libfuse2 patchelf desktop-file-utils file clang zlib1g-dev
```

`clang` 和 `zlib1g-dev` 是 Linux NativeAOT 链接所需的本地工具链依赖。

## 维护检查

修改 Gallery 发布 workflow 或 `PublishToLocal.ps1` 后至少运行：

```bash
ruby -e 'require "yaml"; YAML.load_file(".github/workflows/release-gallery.yml"); puts "ok"'
pwsh -NoLogo -NoProfile -Command '$errors = $null; $null = [System.Management.Automation.Language.Parser]::ParseFile("controlgallery/AtomUIGallery.Desktop/scripts/PublishToLocal.ps1", [ref]$null, [ref]$errors); if ($errors.Count) { $errors | Format-List; exit 1 }'
pwsh -NoLogo -NoProfile -Command '[xml](Get-Content -Path "controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj" -Raw) | Out-Null'
git diff --check
```

真实 NativeAOT publish 仍需要对应 GitHub runner、签名 secrets 和平台工具链。
