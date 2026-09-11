# Scoped Resource Host 开发规范

本规范定义 owner-managed、非 Visual `AvaloniaObject` 承载 Avalonia binding 和动态资源时的开发规则。
生命周期样板代码由 `ScopedResourceHostGenerator` 生成；Control 与容器仍负责确定 owner、保存 attachment token
并在所有退出路径释放。

生成器输入、输出和 diagnostic 见
[Scoped Resource Host Generator](../../modules/generator/scoped-resource-host-generator.md)。

## 适用场景

一个类型同时满足以下条件时，应使用 `[GenerateScopedResourceHost]`：

- 类型继承 `AvaloniaObject`，但不是 `Control`、`StyledElement` 或 `Visual`。
- 类型暴露 Avalonia 属性，并可能承载 XAML binding、`DynamicResource` 或 token-resource binding。
- 类型由 Control、collection、container 或其他 owner 管理生命周期。
- 对象离开 owner 后必须停止资源和主题通知。

典型对象包括 `DescriptionItem`、`NavMenuNode`、DataGrid column/group item 和其他描述型对象。

以下对象不使用该机制：

- 普通 POCO 数据模型。
- 已位于 Visual / StyledElement 树中的真实控件。
- 只包含 CLR 属性且不作为 Avalonia binding target 的内部结构。
- 应用级单例资源绑定；这类对象必须提供显式 create/release 生命周期。

## 类型声明

业务主文件只保留业务 API、Avalonia 属性和 CLR wrapper：

```csharp
[GenerateScopedResourceHost]
public partial class DescriptionItem : AvaloniaObject
{
    public static readonly DirectProperty<DescriptionItem, string> LabelProperty =
        AvaloniaProperty.RegisterDirect<DescriptionItem, string>(
            nameof(Label),
            item => item.Label,
            (item, value) => item.Label = value);

    private string _label = string.Empty;

    public string Label
    {
        get => _label;
        set => SetAndRaise(LabelProperty, ref _label, value);
    }
}
```

业务主文件不得复制以下样板状态：

- `_resourceHost`、attachment count 或 generation。
- `ResourcesChanged` / `ActualThemeVariantChanged` 转发。
- `TryGetResource` owner fallback。
- owner event subscribe/unsubscribe。
- `AttachResourceHost` / detach token 实现。

## Owner 生命周期

标准 owner 流程：

```text
item enters owner collection or container
  -> item.AttachResourceHost(owner)
  -> store attachment token with the item/container lifetime
  -> subscribe business property changes

item leaves owner collection or container
  -> dispose attachment token
  -> unsubscribe business property changes
  -> clear generated visual mapping
```

Owner 必须：

- 保存每次 `AttachResourceHost(...)` 返回的 disposable。
- 在 remove、reset、Items replacement、container clear、template reapply、owner change 和 detach 时释放。
- 在重新 attach 前先完成旧 owner 的业务订阅和 visual mapping 清理。
- 只在当前生成周期内保存 item 到 visual 的映射，不能把 generated visual 挂回 item。
- 自身不能提供 `IResourceHost` 时，选择明确的局部 resource host；不能把 `Application.Current` 当作主要 owner。

同一对象允许在一个 owner 内重复 attach，但每个 token 都必须由获得它的生命周期负责释放。不得缓存一个全局 token
跨 container 或 owner 复用。

## 资源查找与通知

- 当前 owner resources 优先于 `Application.Current.Resources`。
- Owner 实现 `IThemeVariantHost` 时，ActualThemeVariant 跟随 owner。
- Owner 资源或主题变化只影响当前 attachment generation。
- Detach 后旧 owner 的事件不得继续触达 item。
- 动态资源失效通过正常资源通知完成，不使用清空 DataContext、强制路由或 Dispatcher 延时补丁。

## 生命周期验证

每个使用 `[GenerateScopedResourceHost]` 的对象至少覆盖：

- DynamicResource 不 root 已移除对象：WeakReference + GC，并模拟真实 DynamicResource anchor。
- Owner resource 优先于 Application resource。
- Owner 或 Application resource 更新后属性重新发布。
- remove/reset/container clear/template reapply/owner change/detach 后释放旧 owner。
- 同一 owner repeated attach 的计数释放。
- `A -> B -> A` 后，第一轮 A token 不影响当前 attachment。

涉及具体 Control 时，还应运行相邻 Control 测试、Gallery build，并根据影响面执行 NativeAOT 验证。

## Review 清单

- 普通 POCO 是否已经足够，是否确实需要 Avalonia 属性系统。
- 类型是否可能承载 DynamicResource、token-resource binding 或 XAML binding target。
- 业务属性是否仍在主文件中清晰可见。
- Generated resource host 是否保持 internal 实现边界。
- Owner 是否覆盖所有 attach/release 对称路径。
- Item、container、visual mapping 和事件订阅是否一起释放。
- 是否具有 WeakReference、资源优先级、更新和 host reentry 测试。
- 是否没有新增反射、字符串 binding、全局 token binding 或延迟清理。

## 迁移手写实现

迁移既有手写 `IResourceHost` / `IThemeVariantHost` 类型时：

1. 先建立 WeakReference、owner resource、resource update 和 host reentry 测试。
2. 将目标类型声明为 partial 并添加 `[GenerateScopedResourceHost]`。
3. 删除手写资源宿主字段、事件和 attach/detach 实现。
4. 保留业务 API、Avalonia 属性和 owner 侧 token 管理。
5. 对比生成后的资源查找顺序、通知和 release 时机。
6. 运行目标 Control 测试、Gallery build 和 `git diff --check`。

迁移不得顺手改变 Control API、默认值、binding priority、视觉树、主题资源 key 或用户可观察行为。

## 相关文档

- [Scoped Resource Host Generator](../../modules/generator/scoped-resource-host-generator.md)
- [DynamicResource 内存泄露案例](../case-studies/avalonia-dynamic-resource-memory-leak-case-study.md)
- [AOT 编程规范](aot-programming-guidelines.md)
- [Control 研发规范](control-development-guidelines.md)
