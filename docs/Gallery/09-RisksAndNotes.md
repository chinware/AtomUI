# 潜在风险与迭代注意事项

## 1. 架构风险

### 1.1 Service Locator 反模式

**风险等级**：⚠️ 中

`CaseNavigationViewModel` 构造函数中集中注册了 72 个 ViewModel 工厂，使用 `Locator.CurrentMutable.Register` 实现服务定位。

**问题**：
- 注册代码集中在一个构造函数中，随 ShowCase 数量增长会越来越臃肿
- 字符串 Key（`EntityKey`）缺乏编译时检查，拼写错误不会报错
- 难以进行单元测试（需要 mock `Locator`）

**建议**：
- 考虑使用 Source Generator 自动生成注册代码
- 或使用模块化注册（每个 Category 独立注册）

### 1.2 ViewModel 与 View 强耦合

**风险等级**：⚠️ 中

虽然使用了 MVVM 模式，但大量 ShowCase View 的交互逻辑在 Code-Behind 中实现，直接操作控件而非通过 ViewModel。

**问题**：
- 难以对 UI 交互逻辑进行单元测试
- ViewModel 的 `RaiseAndSetIfChanged` 属性使用不充分
- 部分事件处理器包含业务逻辑（如 `Dispatcher.UIThread.InvokeAsync` + `Task.Delay`）

**建议**：
- 将可测试的逻辑迁移到 ViewModel
- 使用 ReactiveUI Command 替代 Click 事件处理器
- 保持 View Code-Behind 仅处理纯 UI 操作

### 1.3 IScreen 查找方式

**风险等级**：⚠️ 低

`CaseNavigation` 通过 `OnAttachedToLogicalTree` 向上遍历可视化树查找 `IScreen`：

```csharp
var screen = this.FindAncestorOfType<WorkspaceWindow>()?.ViewModel;
```

**问题**：
- 依赖具体的 `WorkspaceWindow` 类型而非 `IScreen` 接口
- 如果窗口类型变更，此处需要同步修改

**建议**：
- 使用 `this.FindAncestorOfType<IScreen>()` 或通过 DataContext 传递

## 2. 性能风险

### 2.1 72 个 ViewModel 同时注册

**风险等级**：⚠️ 低

所有 ViewModel 工厂在 `CaseNavigationViewModel` 构造时注册，虽然使用工厂 Lambda 延迟创建，但 `Locator` 的注册表会持续增长。

**当前缓解**：工厂 Lambda 是轻量级的，不会立即创建 ViewModel 实例。

### 2.2 ShowCase View 未缓存

**风险等级**：⚠️ 中

使用 `NavigateAndReset` 导航，每次切换 ShowCase 都会创建新的 View 和 ViewModel 实例，旧实例被丢弃。

**问题**：
- 频繁导航时产生大量 GC 压力
- 复杂 ShowCase（如 DataGrid、TreeView）初始化较慢

**建议**：
- 考虑实现 View 缓存池
- 或使用 `Navigate` 而非 `NavigateAndReset` 保持导航堆栈

### 2.3 自动测试导航间隔

**风险等级**：⚠️ 低

自动导航测试使用 300ms 固定间隔，复杂 ShowCase 可能来不及完成初始化。

**建议**：
- 根据导航目标复杂度动态调整间隔
- 或等待 View 的 `WhenActivated` 完成后再导航

## 3. 硬编码问题

### 3.1 默认导航目标

**位置**：`CaseNavigation.OnAttachedToLogicalTree`

```csharp
_navigationViewModel.NavigateTo(AboutUsViewModel.ID.ToString());
```

**问题**：默认导航到 `AboutUs` 页面，如果 `AboutUsViewModel` 被移除或重命名，此处会静默失败。

### 3.2 主题切换菜单项

**位置**：`WorkspaceWindow.HandleMenuItemCheckChanged`

菜单项通过 `MenuItem.Tag` 存储枚举值，XAML 中硬编码：

```xml
<MenuItem Tag="{x:Static local:WindowMenuItemKind.DarkMode}"/>
```

**问题**：新增菜单项需要同时修改 XAML、枚举和事件处理器，三处需保持同步。

### 3.3 测试导航间隔

**位置**：`CaseNavigationViewModel.TestNavigatePages`

```csharp
await Task.Delay(TimeSpan.FromMilliseconds(300));
```

**问题**：300ms 硬编码，无法配置。

### 3.4 语言变体硬编码

**位置**：`WorkspaceWindow.HandleMenuItemCheckChanged`

```csharp
case WindowMenuItemKind.LanguageZhCN:
    application.SetLanguageVariant(LanguageVariant.zh_CN);
    break;
```

**问题**：新增语言变体需要修改枚举、XAML 和事件处理器。

## 4. 遗留技术债

### 4.1 BoxPanel 已废弃

**位置**：`ShowCases/ViewModels/Layout/BoxPanelViewModel.cs`

`BoxPanel` 布局控件已废弃，但对应的 ShowCase 仍然存在。

**建议**：移除或标记为废弃。

### 4.2 CommunityToolkit.Mvvm 使用不充分

项目引用了 `CommunityToolkit.Mvvm` 包，但实际使用较少，主要依赖 ReactiveUI。

**建议**：
- 如果不需要 CommunityToolkit.Mvvm 的功能，移除引用以减少依赖
- 如果需要，统一 MVVM 框架选择

### 4.3 AOT/Trimming 未启用

**位置**：`AtomUIGallery.Desktop.csproj`

```xml
<!-- <IsTrimmable>true</IsTrimmable> -->
<!-- <PublishTrimmed>true</PublishTrimmed> -->
<!-- <PublishAot>true</PublishAot> -->
```

**问题**：
- 发布包体积较大
- 未来 .NET 趋势是 AOT 优先

**建议**：逐步启用 Trimming，添加必要的 Trimmer 根描述符。

### 4.4 缺少单元测试

Gallery 项目没有专门的测试项目，完全依赖手动测试和 F5 自动导航测试。

**建议**：
- 为 ViewModel 添加单元测试
- 为自定义控件（ShowCasePanel、ShowCaseItem 等）添加控件测试
- 使用 Avalonia.Headless 进行 UI 自动化测试

## 5. 迭代注意事项

### 5.1 新增 ShowCase 检查清单

每次新增 ShowCase 时，确保完成以下步骤：

- [ ] 创建 ViewModel（实现 `IRoutableViewModel` + `IActivatableViewModel`）
- [ ] 创建 View（继承 `ReactiveUserControl<T>`）
- [ ] 在 `CaseNavigationViewModel` 中注册工厂
- [ ] 在 `CaseNavigation.axaml` 的 `NavMenu` 中添加导航项
- [ ] 在 `CaseNavigationLang` 中添加中英文翻译
- [ ] 验证 Source Generator 正确生成枚举和扩展
- [ ] 测试导航和语言切换

### 5.2 新增控件主题检查清单

- [ ] 创建控件类（继承 `TemplatedControl`）
- [ ] 创建 ControlTheme XAML 文件
- [ ] 在 `GalleryControlThemesProvider` 中注册主题
- [ ] 在 `ThemeManagerBuilderExtensions` 中添加注册扩展

### 5.3 新增语言变体检查清单

- [ ] 为每个 `LanguageProvider` 添加新语言类
- [ ] 使用相同的 `[LanguageProvider("Name")]` 标记
- [ ] 确保所有 `const string` 字段名与现有语言类一致
- [ ] 在 `WorkspaceWindow` 菜单中添加语言切换项
- [ ] 在 `WindowMenuItemKind` 枚举中添加新值
- [ ] 在 `HandleMenuItemCheckChanged` 中添加处理逻辑

### 5.4 升级 AtomUI 版本注意事项

- Debug 配置使用 `ProjectReference`，升级源码即可
- Release 配置使用 `PackageReference`，需更新 `Directory.Packages.props` 中的版本号
- Source Generator 可能生成不同的代码，需检查 `GeneratedFiles/` 目录
- 新控件可能需要新的 ShowCase

### 5.5 升级 .NET 版本注意事项

- 修改 `Directory.Build.props` 中的 `AtomUITargetFrameworks`
- 检查 `Roots.xml` 是否需要更新（Trimmer 根描述符）
- 检查发布脚本 `PublishToLocal.ps1` 是否需要更新
- 验证跨平台兼容性

## 6. 代码质量建议

### 6.1 短期改进

1. **添加 XML 文档注释**：至少为公共 API 添加 `/// <summary>` 注释
2. **移除废弃代码**：清理 `BoxPanelViewModel` 等已废弃的 ShowCase
3. **统一 MVVM 模式**：决定使用 ReactiveUI 还是 CommunityToolkit.Mvvm，避免混用
4. **提取硬编码常量**：将 300ms 等魔法数字提取为命名常量

### 6.2 中期改进

1. **添加单元测试**：为 ViewModel 和自定义控件添加测试
2. **实现 View 缓存**：减少导航时的 GC 压力
3. **模块化注册**：将 ShowCase 注册拆分为按 Category 独立注册
4. **启用 Trimming**：减少发布包体积

### 6.3 长期改进

1. **Source Generator 自动注册**：自动扫描 ShowCase 并生成注册代码
2. **Headless UI 测试**：使用 Avalonia.Headless 进行自动化 UI 测试
3. **AOT 支持**：逐步启用 AOT 编译
4. **性能监控**：添加导航性能指标和内存使用监控

## 7. 依赖风险

### 7.1 .NET 10 预览版

当前目标框架为 `net10.0`，.NET 10 仍为预览版，可能存在 API 变更。

**缓解**：同时支持 `net8.0` 作为备选目标框架。

### 7.2 Avalonia 版本升级

Avalonia 11.x 仍在积极开发中，小版本升级可能引入破坏性变更。

**缓解**：使用 `Directory.Packages.props` 统一管理版本号。

### 7.3 ReactiveUI 与 Avalonia 集成

ReactiveUI.Avalonia 的版本需要与 Avalonia 版本兼容。

**缓解**：通过 Avalonia 包间接引用 ReactiveUI，确保版本兼容。