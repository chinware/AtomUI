# Scoped Resource Host Source Generator 范式

本文定义 AtomUI 中“非 Visual `AvaloniaObject` 需要承载 Avalonia 属性绑定和动态资源”的统一开发范式。新增或改造同类需求时，默认使用 `AtomUI.Generator` 生成 scoped resource host 样板代码，不再在每个数据对象中手写完整 `IResourceHost` / `IThemeVariantHost` 生命周期。

相关背景见 [Avalonia DynamicResource 内存泄露案例](../../engineering/avalonia-dynamic-resource-memory-leak-case-study.md)，AOT 约束见 [AtomUI AOT 编程规范](../../engineering/aot-programming-guidelines.md)，控件代码边界见 [AtomUI 控件研发标准规范](../../engineering/control-development-guidelines.md)。

## 1. 适用场景

当一个类型同时满足以下条件时，必须按本文范式处理：

- 类型继承 `AvaloniaObject`，但不是 `Control`、`StyledElement` 或 `Visual`。
- 类型暴露 `AvaloniaProperty`，用于 XAML、binding、`DynamicResource`、token-resource binding 或生成控件属性转接。
- 类型由另一个控件、容器或 owner 管理生命周期，例如 `Descriptions` 管理 `DescriptionItem`，`NavMenu` / `NavMenuItem` 管理 `NavMenuNode`，`DataGrid` 管理 column 对象。
- 类型可能在 Gallery、模板、用户 XAML 或控件内部持有内容、Header、Icon、Template、Style、Theme 等可被动态资源绑定的属性。

典型对象：

- 描述项：`DescriptionItem`
- 菜单数据节点：`NavMenuNode`
- DataGrid column / group item
- 其他 owner-managed 非 Visual 描述对象

不适用场景：

- 普通 POCO 数据模型，不继承 `AvaloniaObject`。
- 真实 Visual / Control，它们应依赖视觉树资源宿主和控件生命周期。
- 只使用普通 CLR 属性且不承载 Avalonia binding target 语义的内部结构。
- 全局单例资源绑定，例如 Flyout 全局 token binding；这类使用显式 create/release 生命周期，不使用本文 generator。

## 2. 统一原则

新增同类对象时遵守以下原则：

- 对象主文件只保留业务 API、Avalonia 属性注册和 CLR wrapper。
- scoped resource host 样板代码由 Source Generator 生成。
- owner 负责 attach/detach；非 Visual 对象不能永久持有 generated control、container 或 ShowCase。
- 动态资源查找顺序必须是 owner resource host 优先，再 fallback 到 `Application.Current`。
- owner 变化、item remove、collection reset、container clear、template reapply、detach、unregister 必须释放资源宿主订阅。
- 不允许通过清空 DataContext、强制路由释放、改静态资源值来掩盖泄露。
- 不允许复制粘贴 DataGridColumn、NavMenuNode 或其他类中的 resource host 样板代码。

## 3. 推荐 API 形态

目标类型应声明为 `partial`，并使用 generator 标记：

```csharp
[GenerateScopedResourceHost]
public partial class DescriptionItem : AvaloniaObject
{
    public static readonly DirectProperty<DescriptionItem, string> LabelProperty =
        AvaloniaProperty.RegisterDirect<DescriptionItem, string>(
            nameof(Label),
            o => o.Label,
            (o, v) => o.Label = v);

    private string _label = string.Empty;

    public string Label
    {
        get => _label;
        set => SetAndRaise(LabelProperty, ref _label, value);
    }
}
```

生成器负责补齐以下成员：

```csharp
public partial class DescriptionItem : IResourceHost, IThemeVariantHost
{
    public event EventHandler<ResourcesChangedEventArgs>? ResourcesChanged;
    public event EventHandler? ActualThemeVariantChanged;

    public bool HasResources { get; }
    public ThemeVariant ActualThemeVariant { get; }

    public bool TryGetResource(object key, ThemeVariant? theme, out object? value);

    internal IDisposable AttachResourceHost(IResourceHost resourceHost);
}
```

业务主文件不再手写：

- `_resourceHost`
- `_resourceHostAttachmentCount`
- `ResourcesChanged` 转发
- `ActualThemeVariantChanged` 转发
- `TryGetResource` owner fallback
- `AttachResourceHost` / `DetachResourceHost`
- owner event subscribe / unsubscribe

这些成员属于统一生命周期模板，必须由 generator 生成。

## 4. Attribute 范式

推荐 attribute 名称：

```csharp
[GenerateScopedResourceHost]
```

Attribute 默认由 `AtomUI.Generator` 通过 post-initialization 注入为 `internal sealed`，避免污染运行时公共 API。只有确实需要跨 assembly 手写引用该 attribute 时，才考虑把 attribute 移入共享运行时包；这属于 API 扩展，必须单独评审。

生成器识别规则：

- 使用 `ForAttributeWithMetadataName` 识别目标类型。
- 目标类型必须是非泛型 `partial class`。
- 目标类型必须继承 `AvaloniaObject`。
- 目标类型不能继承 `Control`、`StyledElement` 或 `Visual`。
- 目标类型不能已经实现 `IResourceHost` 或 `IThemeVariantHost`。
- 目标类型所在 assembly 必须能引用 `Avalonia.Controls` 和 `Avalonia.Styling`。

不满足规则时，必须报告编译期 diagnostic，不生成半套代码。

## 5. 生成代码职责

生成代码必须包含完整 acquire/release 对称逻辑：

- `AttachResourceHost(IResourceHost resourceHost)` 返回 `IDisposable`。
- 同一个对象重复 attach 到同一个 host 时使用 attachment count。
- attach 到新 host 前先释放旧 host。
- subscribe owner `ResourcesChanged`。
- 当 owner 实现 `IThemeVariantHost` 时 subscribe `ActualThemeVariantChanged`。
- detach 时解除同一组事件订阅。
- `TryGetResource` 先调用 owner host，再 fallback 到 `Application.Current`。
- owner 资源或主题变化时转发到当前对象。
- detach 后触发资源和主题变化通知，让动态资源重新解析或停止持有旧 owner。

生成代码禁止：

- 持有 `Control`、container、generated visual 或 ShowCase 引用。
- 创建全局 token binding。
- 使用反射、字符串 path binding、runtime assembly scan。
- 依赖 `Dispatcher` 延迟清理。
- 吞掉 release 异常或通过状态标记掩盖生命周期错误。

## 6. Owner 侧范式

Generator 只生成非 Visual 对象自身的 resource host 能力。owner 控件仍必须管理业务生命周期。

标准 owner 流程：

```text
item enters owner collection/container
  -> item.AttachResourceHost(owner)
  -> subscribe item property changed
  -> generate or update visual

item leaves owner collection/container
  -> dispose resource host attachment
  -> unsubscribe item property changed
  -> clear generated visual mapping
```

owner 的责任：

- 把 `AttachResourceHost(...)` 返回的 disposable 存入同一条 item/container 生命周期。
- remove/reset/Items replacement/template reapply/detach 时释放 disposable。
- item 属性变化时只更新当前 generated visual 或触发布局，不把 generated visual 挂回 item。
- item 到 visual 的映射只服务当前生成周期，重建前必须清空。
- 如果 owner 本身不是 `IResourceHost`，应改 owner 或选择更合适的局部 resource host；不能让 item 直接 fallback 到 `Application` 作为主要宿主。

## 7. Generator 模块落地规则

实现该范式时，`AtomUI.Generator` 中使用独立目录：

```text
src/AtomUI.Generator/ResourceHost/
├── ScopedResourceHostGenerator.cs
├── ScopedResourceHostSourceWriter.cs
└── ScopedResourceHostTypeInfo.cs
```

维护规则：

- generator 只生成生命周期样板代码，不生成业务属性。
- writer 输出必须稳定，避免无意义 diff。
- generated namespace 必须与目标类型 namespace 一致。
- generated partial class 必须使用目标类型完整类型参数约束；当前范式默认不支持 generic target。
- 生成文件名使用 `<TypeName>.ScopedResourceHost.g.cs`。
- 诊断 ID、category、severity 统一按 [Compiler Diagnostics Guidelines](../../engineering/compiler-diagnostics-guidelines.md) 分配。
- 新增 generator 后必须检查 `GeneratedFiles/AtomUI.Generator/` 输出是否可重复生成，不能手改生成物。

## 8. 测试范式

每个使用 `[GenerateScopedResourceHost]` 的对象至少覆盖以下测试：

- DynamicResource 不 root 已移除对象：使用 WeakReference + GC，并模拟 XAML `DynamicResourceExtension` anchor。
- owner resource 优先：owner resources 中的值优先于 `Application.Current.Resources`。
- resource update：owner 或 application resource 更新后目标属性能重新发布。
- detach release：remove/reset/container clear/template reapply/detach 后旧 owner 不再被订阅。
- repeated attach：同一 item 被同一 owner 重复 attach 时，attachment count 能正确释放。

Generator 本身至少覆盖以下测试：

- 正常目标生成 `IResourceHost` / `IThemeVariantHost` 和 `AttachResourceHost`。
- 非 partial class 报 diagnostic。
- 非 `AvaloniaObject` 报 diagnostic。
- `Control` / `StyledElement` / `Visual` 目标报 diagnostic。
- 已手写 `IResourceHost` / `IThemeVariantHost` 的目标报 diagnostic。
- generic target 报 diagnostic。
- generated source 不包含反射、字符串 binding 或全局资源 binding。

验证命令按影响面选择：

```bash
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --no-restore
dotnet test tests/AtomUI.Desktop.Controls.Tests/AtomUI.Desktop.Controls.Tests.csproj --framework net10.0 --no-restore
dotnet build controlgallery/AtomUIGallery/AtomUIGallery.csproj --no-restore
git diff --check
```

涉及 AOT 发布路径时，按 [AtomUI AOT 编程规范](../../engineering/aot-programming-guidelines.md) 增加 analyzer 或真实 NativeAOT publish。

## 9. Review 清单

评审同类需求时必须确认：

- 是否真的需要 `AvaloniaObject`；如果普通 POCO 足够，不要引入 Avalonia 属性系统。
- 是否真的可能承载 `DynamicResource` / token-resource binding；如果是，必须使用本文范式。
- 业务属性是否仍在主文件中清晰可见。
- generated resource host 是否没有进入 public API，除非已有授权。
- owner 是否有明确 attach/release 路径。
- release 路径是否覆盖 remove/reset/replacement/template reapply/detach/container clear。
- 是否有 WeakReference 生命周期测试。
- 是否有 owner resource 优先级和资源更新测试。
- 是否没有新增反射、字符串 binding、全局 token binding 或 `Dispatcher` 延迟补丁。

## 10. 迁移既有手写实现

既有手写 `IResourceHost` / `IThemeVariantHost` 类型可以分阶段迁移：

1. 先补或确认现有 WeakReference、owner resource、resource update 测试。
2. 将类型改为 `partial` 并加 `[GenerateScopedResourceHost]`。
3. 删除手写 resource host 字段、事件和 attach/detach 方法。
4. 保留业务 API、Avalonia 属性和 owner 侧 attach/release 调用。
5. 对比 generated source 与旧行为，确认资源查找顺序、事件转发和 release 时机不变。
6. 跑目标控件测试、Gallery build 和 `git diff --check`。

迁移不能顺手改变控件 API、默认值、binding priority、视觉树、主题资源 key 或用户可观察行为。
