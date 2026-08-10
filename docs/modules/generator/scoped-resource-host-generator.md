# Scoped Resource Host Generator

`ScopedResourceHostGenerator` 为 owner-managed、非 Visual 的 `AvaloniaObject` 生成 scoped
`IResourceHost` / `IThemeVariantHost` 实现。本文件定义生成器的输入、输出、诊断、增量生成和测试契约；
Control 作者如何判断适用场景、管理 owner 生命周期和评审资源释放，见
[Scoped Resource Host 开发规范](../../engineering/development/scoped-resource-host.md)。

## 输入契约

目标类型使用生成器注入的标记：

```csharp
[GenerateScopedResourceHost]
public partial class DescriptionItem : AvaloniaObject
{
}
```

生成器通过 `ForAttributeWithMetadataName` 识别目标，并要求：

- 目标是非泛型 `partial class`。
- 目标继承 `AvaloniaObject`。
- 目标不继承 `Control`、`StyledElement` 或 `Visual`。
- 目标未自行实现 `IResourceHost` 或 `IThemeVariantHost`。
- 目标 assembly 可以引用 `Avalonia.Controls` 和 `Avalonia.Styling`。

`GenerateScopedResourceHostAttribute` 由 Generator post-initialization 注入为 `internal sealed`。它只用于编译期
标记，不形成运行时公共 API。

## 生成输出

生成的 partial class 实现：

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

输出文件名为 `<TypeName>.ScopedResourceHost.g.cs`，namespace 与目标类型一致。生成代码只补充资源宿主生命周期，
不生成业务属性、Control 行为或视觉对象。

## 生命周期状态机

生成代码必须保持 acquire/release 对称：

- `AttachResourceHost(...)` 返回可释放 token。
- 同一对象重复 attach 到同一 host 时使用 attachment count。
- host 切换时递增 generation，使 `A -> B -> A` 场景中的旧 token 不能释放新一轮 attachment。
- attach 到新 host 前先解绑旧 host 的资源与主题事件。
- `TryGetResource` 先查询当前 owner host，再 fallback 到 `Application.Current`。
- owner 资源或主题变化时，只向当前 attachment 转发通知。
- 最后一个 attachment 释放后解除全部 owner 订阅，并发布资源与主题失效通知。

生成代码不得：

- 持有 Control、container、generated visual 或 Gallery 页面引用。
- 创建全局 token binding。
- 使用反射、字符串 path binding 或运行时 assembly scan。
- 通过 Dispatcher 延迟释放生命周期状态。

## 实现布局

```text
src/AtomUI.Generator/ResourceHost/
|-- ScopedResourceHostGenerator.cs
|-- ScopedResourceHostSourceWriter.cs
`-- ScopedResourceHostTypeInfo.cs
```

- Generator 负责语义分析、增量输入和 diagnostic。
- TypeInfo 保存稳定、可比较的生成输入。
- SourceWriter 只消费 TypeInfo，输出顺序和格式必须稳定。
- 当前契约不支持 generic target。
- 诊断 ID、category 和 severity 遵守
  [编译期诊断规范](../../engineering/development/compiler-diagnostics-guidelines.md)。

## 诊断

以下输入必须报告编译期 diagnostic，且不生成半套代码：

- 目标不是 partial class。
- 目标不是 `AvaloniaObject`。
- 目标属于 Visual / StyledElement / Control 树。
- 目标已实现目标资源宿主接口。
- 目标是泛型类型。
- 必要 Avalonia 类型不可解析。

Diagnostic 的位置应尽量指向 Attribute 或目标类型声明，消息必须说明修复条件，不能只报告生成失败。

## AOT 与增量生成

- 正常路径不使用反射、动态代码或运行时类型发现。
- 增量输入只包含生成所需的符号信息，不能把整个 Compilation 或 SyntaxTree 长期保存在模型中。
- 相同输入必须产生字节稳定的生成结果，避免无意义 rebuild 和 diff。
- Attribute 注入、目标发现和输出 hint name 必须保持确定性。

## 测试契约

Generator 测试至少覆盖：

- 正常目标生成两个资源宿主接口和 `AttachResourceHost`。
- 非 partial、非 `AvaloniaObject`、Visual 派生、已实现接口和泛型目标的 diagnostic。
- repeated attach、host reentry 和 detach 相关状态代码完整生成。
- generated source 不包含反射、字符串 binding、全局资源 binding 或业务类型引用。
- 输出 hint name、namespace 和成员顺序稳定。

验证命令：

```bash
dotnet test tests/AtomUI.Generator.Tests/AtomUI.Generator.Tests.csproj --no-restore
git diff --check
```

## 相关文档

- [Scoped Resource Host 开发规范](../../engineering/development/scoped-resource-host.md)
- [DynamicResource 内存泄露案例](../../engineering/case-studies/avalonia-dynamic-resource-memory-leak-case-study.md)
- [AOT 编程规范](../../engineering/development/aot-programming-guidelines.md)
