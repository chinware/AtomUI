# 架构设计与原理

> 本文档详细解释 AtomUI 为什么需要反射访问 Avalonia 内部成员，以及围绕这一需求建立的安全保障机制。

---

## 1. 为什么需要反射 Hook Avalonia 内部成员

### 1.1 问题背景

AtomUI 的目标是在 Avalonia 之上忠实复现 Ant Design 5.0 的全部组件。这要求对 Avalonia 内置控件进行 **深度定制**，而 Avalonia 框架出于封装性考虑，将部分关键能力标记为 `private`、`internal` 或 `protected`，不对外暴露。

典型场景包括：

| 场景 | 需要访问的成员 | 原因 |
|---|---|---|
| 手动管理可视树 | `Visual.SetVisualParent()` (private) | AtomUI 需要在不触发标准可视化树流程的情况下重新组织控件层级 |
| 手动管理逻辑树 | `StyledElement.LogicalChildren` (protected) | 需要直接操作逻辑子项列表以实现自定义控件组合 |
| 禁用/启用过渡动画 | `Animatable.EnableTransitions()` / `DisableTransitions()` (private) | 在特定场景下需要临时关闭过渡动画以避免视觉闪烁 |
| 获取布局管理器 | `ILayoutRoot.LayoutManager` (internal) | 需要手动触发布局计算（如 `ExecuteInitialLayoutPass`） |
| 定制弹出层定位 | `ManagedPopupPositioner._popup` (private) | 需要直接操作底层弹出层定位器以实现自定义定位逻辑 |
| 控制窗口显示流程 | `WindowBase` 的多个私有方法 | 实现无激活窗口显示等高级窗口管理功能 |
| 操作选择模型 | `SelectingItemsControl._selection` (private) | 需要替换或直接操作选择模型以实现自定义选择行为 |
| 控制文本框内部状态 | `TextBox._scrollViewer` / `_presenter` (private) | 需要访问内部组件以实现增强的文本输入交互 |

### 1.2 为什么不用继承 + `protected`

部分成员虽然是 `protected`，但 AtomUI 的控件可能并不直接继承自该 Avalonia 类型。例如 `AtomUI.Controls.Popup` 继承了 `Avalonia.Controls.Primitives.Popup`，但某些操作需要在非继承链上下文中执行。此外，许多关键成员是 `private` 或 `internal` 的，无论是否继承都无法直接访问。

### 1.3 为什么不 Fork Avalonia

Fork 维护成本极高，且会导致与 Avalonia 生态系统的不兼容。反射虽有脆弱性，但通过规范化管理可以将风险控制在可接受范围内。

---

## 2. `[DynamicDependency]` 注解与 Native AOT 支持

### 2.1 AtomUI 的 AOT 战略目标

AtomUI 的长期目标之一是支持 [.NET Native AOT 发布](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)。Native AOT 将 .NET 应用编译为完全原生的机器码，带来显著的启动速度提升、更小的内存占用和无需 JIT 的部署体验。这对桌面应用尤其重要——用户期望应用点击即开，而非等待 JIT 预热。

然而，Native AOT 对代码有严格的静态分析要求。AOT 编译器必须在编译时完整确定所有可能被调用的代码路径，而 **反射是 AOT 的天然敌人**——反射在运行时动态发现和调用成员，AOT 编译器无法预知这些访问路径。

### 2.2 .NET Trimming 与 AOT 的关系

Native AOT 发布流程内置了 [IL Trimming](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trim-self-contained)。Trimming 和 AOT 是紧密耦合的两个阶段：

```
源代码 → IL 编译 → IL Trimming（移除未引用成员）→ AOT 编译（生成原生代码）→ 原生二进制
```

Trimmer 通过静态分析代码的依赖关系图，移除所有"看起来未被使用"的类型和成员，以减小最终二进制体积。问题在于：

- **反射调用对 Trimmer 是不可见的**。当 AtomUI 通过 `typeof(Visual).GetMethod("SetVisualParent", ...)` 获取一个私有方法时，Trimmer 的静态分析无法发现 `Visual.SetVisualParent` 被引用。
- **Trimmer 会裁剪掉这些"未使用"的成员**。被反射访问的 Avalonia 私有方法、字段、属性在 Trimming 阶段被移除。
- **AOT 编译后运行时崩溃**。应用在 AOT 模式下运行时，反射查找会抛出 `MissingMethodException` 或 `MissingFieldException`，因为目标成员已被裁剪。

即使不使用 Native AOT，常规的 [Self-Contained Trimmed 发布](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trim-self-contained) 也会触发同样的问题。因此，**所有反射访问都必须从现在开始就标注 Trimming 保护注解**，为 AOT 做好准备。

### 2.3 `[DynamicDependency]` 的保护机制

`System.Diagnostics.CodeAnalysis.DynamicDependencyAttribute` 是 .NET 提供的 Trimmer / AOT 保护注解。当标注在某个成员上时，它向 Trimmer 声明一条 **显式的依赖关系**：**"即使静态分析看不到，我标注的目标成员在运行时会被使用，必须保留"**。

```csharp
// 告诉 Trimmer/AOT：保留 Visual 类型的所有非公开方法，
// 因为下面的 Lazy 初始化器会在运行时反射访问 Visual.SetVisualParent
[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(Visual))]
private static readonly Lazy<MethodInfo> SetVisualParentMethodInfo = ...;
```

**工作原理**：Trimmer 在分析到带有 `[DynamicDependency]` 注解的成员时，会将注解中指定的目标类型及其成员标记为"已根引用（rooted）"，从而在 Trimming 阶段保留这些成员，确保它们在 AOT 编译后的原生二进制中仍然存在。

### 2.4 如果缺少 `[DynamicDependency]` 会发生什么

| 发布模式 | 缺少注解的后果 |
|---|---|
| **Debug / 常规 Release** | 正常工作（不触发 Trimming，JIT 在运行时解析反射） |
| **Self-Contained Trimmed** | 运行时 `MissingMethodException` / `MissingFieldException`（目标成员被裁剪） |
| **Native AOT** | 运行时崩溃（目标成员被裁剪 + 无 JIT 回退） |

这意味着一个在开发阶段运行良好的反射调用，在 Trimmed 或 AOT 发布后可能 **静默失败**。`[DynamicDependency]` 注解将这一隐患在编码阶段就消除。

### 2.5 `DynamicallyAccessedMemberTypes` 常用值

| 值 | 保护目标 | 典型场景 |
|---|---|---|
| `NonPublicMethods` | 所有非公开方法（private、internal、protected） | 反射调用私有方法 |
| `NonPublicProperties` | 所有非公开属性 | 反射读写私有属性 |
| `NonPublicFields` | 所有非公开字段 | 反射读写私有字段 |
| `NonPublicEvents` | 所有非公开事件 | 反射订阅/取消订阅私有事件 |

> **注意**：`DynamicDependency` 的保护粒度是 **按成员类别** 的（如保留所有非公开方法），而非单个成员。这是因为 `DynamicallyAccessedMemberTypes` 枚举不支持按名称指定单个成员（虽然存在按字符串指定的重载，但 AtomUI 统一使用类型安全的枚举版本）。

### 2.6 AOT 就绪原则

AtomUI 的反射访问代码遵循 **AOT-Ready（AOT 就绪）** 原则：

1. **每个反射缓存字段必须标注 `[DynamicDependency]`** —— 这是强制要求，不是可选的最佳实践。
2. **注解必须与实际访问的成员类型匹配** —— 访问字段用 `NonPublicFields`，访问方法用 `NonPublicMethods`，不可混淆。
3. **`typeof()` 中的目标类型必须是反射实际操作的 Avalonia 类型** —— 确保 Trimmer 能正确定位保护目标。
4. **新增任何反射访问时，开发者必须同时添加对应的 `[DynamicDependency]`** —— Code Review 时应将此作为必检项。

这些措施确保 AtomUI 在未来启用 Native AOT 发布时，所有反射访问路径已经被正确保护，无需进行大规模的回溯修复。

---

## 3. `Lazy<T>` 延迟初始化的设计决策

所有反射信息均通过 `Lazy<XxxInfo>` 进行缓存：

```csharp
private static readonly Lazy<MethodInfo> SetVisualParentMethodInfo = new Lazy<MethodInfo>(() =>
    typeof(Visual).GetMethodInfoOrThrow("SetVisualParent",
        BindingFlags.Instance | BindingFlags.NonPublic));
```

这一设计带来三重好处：

1. **性能**：反射查找只执行一次，后续调用直接使用缓存的 `MethodInfo` / `PropertyInfo` / `FieldInfo`。
2. **线程安全**：`Lazy<T>` 默认使用 `LazyThreadSafetyMode.ExecutionAndPublication`，保证多线程环境下只初始化一次。
3. **启动时间**：延迟到首次使用时才执行反射查找，不影响应用启动性能。

---

## 4. `GetXxxInfoOrThrow` 的快速失败设计

AtomUI 在 `AtomUI.Reflection.TypeMemberExtension` 中提供了一组 `OrThrow` 后缀的反射查找方法：

```csharp
public static MethodInfo GetMethodInfoOrThrow(this Type type, string name, BindingFlags flags = ...)
{
    if (!type.TryGetMethodInfo(name, out var info, flags))
    {
        throw new MissingMethodException($"Can not find the '{name}' from type '{type}'. We can not reflect it.");
    }
    return info;
}
```

**快速失败（Fail-Fast）** 策略确保：
- 如果 Avalonia 升级后某个成员被重命名或移除，应用会在 **首次访问** 时立即抛出清晰的异常。
- 开发者在开发和测试阶段就能发现兼容性问题，而不是在生产环境中遇到隐晦的 `NullReferenceException`。

---

## 5. 扩展方法封装的隔离设计

反射访问通过扩展方法（Extension Methods）封装，实现了 **调用者无感知** 的设计：

```csharp
// 调用者代码 —— 看起来就像调用普通方法
visual.SetVisualParent(null);
animatable.EnableTransitions();
styledElement.AddToLogicalChildren(child);

// 实际通过反射执行
SetVisualParentMethodInfo.Value.Invoke(visual, [null]);
```

好处：
- 业务代码不需要知道底层是反射实现
- 如果未来 Avalonia 将该成员公开，只需修改扩展类内部实现，调用方无需变更
- 编译时类型检查（扩展方法的 `this` 参数类型约束）

---

## 6. Avalonia 版本升级的兼容性管理

### 6.1 风险评估

反射访问的成员 **不在 Avalonia 的公开 API 契约内**，因此：
- 可能在任何版本（包括 patch 版本）中被修改
- 成员名称、参数签名、返回类型都可能变更
- 成员可能被完全移除

### 6.2 升级检查清单

每次升级 Avalonia 版本时，需执行以下检查：

1. **构建验证**：确保项目正常编译（反射代码本身编译不会失败）。
2. **运行时验证**：运行所有控件的 ShowCase，触发每个反射扩展的 `Lazy<T>` 初始化。`GetXxxInfoOrThrow` 会在成员缺失时立即抛出异常。
3. **逐一对照**：查看 [现有反射扩展清单](./ExistingExtensions.md)，对照 Avalonia 的 CHANGELOG 和源码差异，确认每个 Hook 目标是否仍然存在。
4. **修复与替换**：如果某个成员被移除或重命名，更新对应的反射扩展类。如果 Avalonia 新版本提供了公开 API 替代方案，应迁移到公开 API 并移除反射代码。

