# GalleryBase 迁移与测试设计

本文档细化 `AtomUI.Toolkits.GalleryBase` 的落地迁移顺序、文件移动原则、测试拆分和验收命令。迁移必须保持现有 `AtomUIGallery` 行为稳定，避免一次性大重写。

## 迁移原则

- 先抽底层，后改 Shell。
- 先保证现有 `AtomUIGallery` 行为不变，再引入新产品复用能力。
- 每个阶段都能构建和测试。
- 移动文件和行为修改分开提交。
- 不在迁移中顺手改 ShowCase demo 内容。
- 不为了命名中立做第一阶段大规模控件重命名。

## 阶段 1：建立项目骨架

新增：

```text
src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj
tests/AtomUI.Toolkits.GalleryBase.Tests/AtomUI.Toolkits.GalleryBase.Tests.csproj
docs/modules/toolkits-gallery-base/
```

项目引用：

```text
AtomUI.Toolkits.GalleryBase
  -> AtomUI.Desktop.Controls
  -> AtomUI.Generator
  -> Avalonia
```

如果后续拆出更轻量的非 Desktop Shell，可以再把控件层和 Shell 层拆包；第一阶段不拆，避免包数量和迁移复杂度过高。

验收：

- 新项目能加入 solution。
- 空项目 build 通过。
- 不影响现有 Gallery build。

## 阶段 2：迁移展示控件

从 `controlgallery/AtomUIGallery/Controls` 迁入：

```text
ShowCaseItem*
ShowCasePanel*
ShowCaseMasonryPanel.cs
GalleryStickyTabsHost*
GalleryStickyTabsPanel.cs
GalleryShowCaseScenarioController.cs
GalleryReactiveUserControl.cs
GalleryShowCaseRuntimeOptions.cs
GalleryControlThemesProvider*
相关 Token
```

处理：

- namespace 改为 `AtomUI.Toolkits.GalleryBase.Controls` 或 `.Runtime`。
- XAML namespace 改为 `https://atomui.net/toolkits/gallery-base`。
- GalleryBase AssemblyInfo 只暴露 GalleryBase namespaces。
- `AtomUIGallery` 引用 GalleryBase。
- `ShowCaseControlAliases.cs` 改为 global using GalleryBase controls。

测试迁移：

- `ShowCaseMasonryPanelTests`
- `ShowCasePanelStructureTests`
- `GalleryStickyTabsHostTests`
- `GalleryReactiveUserControlTests`

验收：

```bash
dotnet test tests/AtomUI.Toolkits.GalleryBase.Tests/AtomUI.Toolkits.GalleryBase.Tests.csproj --nologo /nr:false
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false
```

## 阶段 3：引入配置和路由注册

新增：

```text
Configuration/
  GalleryBaseOptions.cs
  GalleryBaseConfiguration.cs
  GalleryBrandingOptions.cs
  GalleryShellOptions.cs
  GalleryPlatformOptions.cs

Routing/
  GalleryRouteRegistry.cs
  GalleryRouteDescriptor.cs

Navigation/
  GalleryNavigationBuilder.cs
  GalleryNavigationNode.cs
```

`AtomUIGallery` 新增产品模块：

```text
controlgallery/AtomUIGallery/AtomUIGalleryModule.cs
```

该模块复刻当前：

- Overview
- Community
- Components
- 所有控件 ShowCase 导航
- 所有 ViewModel/View route

阶段 3 可以先保留旧 `CaseNavigationViewModel`，但必须有测试证明新 registry 覆盖所有现有页面。

测试：

- route key 唯一性。
- navigation key 唯一性。
- navigation 页面都有 route。
- 所有旧 `ShowCaseViewModule` 注册迁入新 registry。

## 阶段 4：替换导航 ViewModel 和 XAML

删除硬编码导航：

- `CaseNavigation.axaml` 中手写 `NavMenuNode`。
- `CaseNavigationViewModel.RegisterShowCaseViewModels()` 中所有产品 ViewModel factory。

替换为：

```text
GalleryNavigationView
GalleryNavigationViewModel
GalleryNavigationMenuAdapter
```

`AtomUIGallery` 只提供导航配置，不提供导航控件实现。

验收：

- 左侧导航结构和当前视觉一致。
- 默认页面仍是 Overview。
- 点击所有导航项能进入对应页面。
- F5/F6 自动切页诊断保留。

## 阶段 5：抽出共享 Shell

迁移：

```text
WorkspaceWindow -> GalleryWorkspaceWindow
WorkspaceWindowViewModel -> GalleryWorkspaceViewModel
BrowserGalleryView -> GalleryBrowserView
GalleryWindowTitleBar -> GalleryBase title bar control
```

保留在产品侧：

- `GalleryApplication`
- `BrowserGalleryApplication`
- `Program`
- 产品字体注册
- 产品模块注册
- 产品崩溃日志目录名配置

处理 Browser：

- 删除重复 sidebar/footer 构造代码。
- 保留 Browser overlay layer 初始化，但迁入 GalleryBase。
- Browser `MainView = new GalleryBrowserView(configuration)`。

验收：

- Desktop 和 Browser 侧边栏一致。
- Desktop 标题栏菜单可用。
- Browser Popup/Flyout/Tour overlay 正常。
- 产品品牌来自配置。

## 阶段 6：清理产品项目

`AtomUIGallery` 中应只剩：

```text
Assets/
ShowCases/
GalleryVersionInfo.cs
AtomUIGalleryModule.cs
BaseGalleryApplication 或产品 Application
```

清理：

- 移除已迁移 Controls。
- 移除旧 Workspace。
- 移除产品 AssemblyInfo 中对 GalleryBase controls 的 XML namespace 定义。
- 更新 docs/architecture/dependency-graph.md。
- 更新 docs/modules 索引或相关模块概览。

## 测试拆分

迁移后测试归属：

| 测试 | 目标项目 |
|---|---|
| ShowCase 控件布局和延迟创建 | `AtomUI.Toolkits.GalleryBase.Tests` |
| Sticky host 行为 | `AtomUI.Toolkits.GalleryBase.Tests` |
| Gallery route/navigation 配置校验 | `AtomUI.Toolkits.GalleryBase.Tests` |
| Shell 不含产品硬编码 | `AtomUI.Toolkits.GalleryBase.Tests` |
| AtomUI 具体导航树 | `AtomUIGallery.Tests` |
| AtomUI Overview/Community 页面 | `AtomUIGallery.Tests` |
| 控件 ShowCase 快照 | `AtomUIGallery.Tests` |
| Browser 使用产品模块注册 | `AtomUIGallery.Tests` |

## 验证命令

每个阶段至少运行：

```bash
dotnet test tests/AtomUIGallery.Tests/AtomUIGallery.Tests.csproj --nologo /nr:false
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj --nologo -v:minimal /nr:false
```

涉及 GalleryBase 测试后增加：

```bash
dotnet test tests/AtomUI.Toolkits.GalleryBase.Tests/AtomUI.Toolkits.GalleryBase.Tests.csproj --nologo /nr:false
```

涉及复杂控件示例或 popup 行为后增加：

```bash
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --nologo /nr:false
```

每次提交前：

```bash
git diff --check
```

## 回滚策略

每个阶段都应保持可回滚：

- 阶段 2 只移动控件，不改导航。
- 阶段 3 只建立新 registry，不删除旧导航。
- 阶段 4 替换导航，但不改 Shell。
- 阶段 5 替换 Shell，但不改 ShowCase 页面。
- 阶段 6 只清理死代码。

如果某阶段失败，应回滚该阶段提交，而不是混合回滚多个阶段。

## 最终验收

- `AtomUI.Toolkits.GalleryBase` 不引用任何产品项目。
- `AtomUIGallery` 引用 GalleryBase 并作为第一个产品消费方。
- Desktop 和 Browser 共用同一份产品配置。
- 新产品能通过最小 demo 注册一个 Overview 页面和一个 ShowCase 页面。
- 所有 Gallery 和 Desktop 控件相关测试通过。
