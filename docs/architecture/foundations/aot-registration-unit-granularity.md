# AOT Registration Unit 粒度

> 状态：截至 2026-08-15，`ControlPackageRegistrationEntry`、默认 `Package` 粒度和显式 `Directory` 粒度均已实现并完成完整发布验证。

本文是 AtomUI Control Package 的 Registration Unit 粒度、资源归属和第三方包接入边界的正式所有者。

整体 linked publish 模式、Application Plan、Package Core、动态 fallback 和 ABI 由
[AOT 与裁剪架构](aot-and-trimming.md)定义。日常编码规则见
[AOT 编程规范](../../engineering/development/aot-programming-guidelines.md)，第三方作者的操作步骤见
[第三方 AtomUI Control Package 指南](../../guides/theming/third-party-control-packages.md)。

## 1. 为什么不能默认按目录拆分

源码目录表达代码组织，不天然表达运行时独立性。一个公开 Control 经常在 C# 中创建其他目录中的 internal View、Presenter、
Cell 或 Track；主题文件也可能引用同包其他控件族的资源。

如果把所有顶层目录自动解释成独立 Unit，正确性会依赖包作者补写 ownership metadata。metadata 遗漏通常不会影响普通构建，
而是在 trimmed 或 NativeAOT 应用中表现为缺失主题、descriptor 或内部 Control。这不适合作为普通第三方包的默认契约。

AtomUI 因此采用保守默认：

```text
普通 Package
└── 一个完整 Registration Unit
    ├── public Controls
    ├── internal View / Presenter / Cell
    ├── Control descriptors
    ├── Own Token schemas
    └── Control-owned Theme Assets
```

只有大型多控件包明确证明目录级拆分具有体积收益，并建立完整发布验证后，才主动选择多个 Unit。

## 2. 粒度属性

Control Package 使用以下 MSBuild 属性选择粒度：

```xml
<AtomUIRegistrationGranularity>Package</AtomUIRegistrationGranularity>
```

合法值只有：

| 值 | 含义 | 适用对象 |
| --- | --- | --- |
| `Package` | 当前 Control Package 生成一个完整 Registration Unit | 默认值；第三方包和单一控件族包 |
| `Directory` | 按稳定控件族目录生成多个 Registration Unit | 显式高级优化；大型多控件包 |

属性未声明时必须等价于 `Package`。未知值必须在 Package 自身编译阶段报错，不得静默选择任一模式。

Theme Schema、Theme Asset、ControlMap 和 linked registration Generator 必须读取同一个规范化粒度策略。任何 Generator
不得根据自己的输入重新猜测粒度。

## 3. Package 模式

Package 模式是所有 Control Package 的默认行为。当前程序集定义的以下内容进入同一个内部 Unit：

- 所有参与 linked registration 的 public Control。
- internal Presenter、View、Cell、Item、Track、Decorator 和辅助 Control。
- Control descriptor 与可选 Own Token schema。
- 所有 Control-owned Theme Asset factory。
- 包内 C#、AXAML 和生成代码形成的 Control 实现依赖。

Unit identity 由 Generator 从 Package identity 稳定派生，对普通包作者不可见。应用使用包内任一 Control 时，整个 Package
Unit 被保留；应用仍必须显式调用 `UseXxxControls()`，静态使用不能自动启用可选包。

Package 模式不得要求作者：

- 声明 `AtomUIRegistrationUnit`。
- 维护 Unit dependency 列表。
- 使用 `AvaloniaXaml Update="..."` 修补 ownership。
- 为内部 Control 或 Theme 添加 linker XML。
- 把 owner 解析失败的资源伪装成 PackageShared。

Package 模式的目标是让小型包和单一控件族包把正确性建立在 Package 产品边界上，而不是建立在内部目录结构上。

## 4. Directory 模式

大型多控件包可以显式启用：

```xml
<AtomUIRegistrationGranularity>Directory</AtomUIRegistrationGranularity>
```

Directory 模式使用稳定控件族目录作为默认 Unit 边界。这里的目录必须代表可独立运行的公开产品能力，例如 Button、
DatePicker 或 Tree，而不能只是 `Cell`、`Utils`、`View`、`Internal` 等代码分类。

启用 Directory 模式后仍必须满足：

- Presenter、Cell、View、Semantic Part 和基础主题跟随其所属公开控件族。
- internal 子目录不能继续自动拆成更小 Unit。
- Generator 从可以证明的 C# 和 AXAML 使用生成同 Package Unit 依赖。
- 作者不手写 Unit dependency 列表。
- 已知循环依赖由 fragment 的 `TryEnterUnit` 去重，循环不能导致漏注册。
- owner 或依赖无法证明时，当前 Package 必须 full fallback。

Directory 模式是体积优化承诺，不是 AOT 正确性的前置条件。没有真实体积收益、行为测试和发布验证的包不得为了目录整齐
启用该模式。

## 5. ControlMap 与资源归属

ControlMap 表示 CLR Control 的定义程序集 ownership，不等于 Theme descriptor 清单。当前 Package 定义的每个 public、
非泛型 Control 都必须映射到当前粒度策略生成的 Unit；只有原本可主题化的 Control 才进入 descriptor 注册。

主题资源只能属于：

| 分类 | 行为 |
| --- | --- |
| `ControlOwned` | 跟随 Package Unit 或 Directory Unit 保留 |
| `PackageShared` | 调用 Package 入口时始终保留 |
| `Unknown` | 禁止猜测；当前 Package 使用 full fallback |

`PackageShared` 只用于以下资源：

- 没有选择任何 Control Unit 时仍必须随 Package 入口加载的基础资源。
- Directory 模式下确实跨多个 Unit、没有合理单一 owner 的资源。

Popup/Overlay 基础设施、Window decoration、全局 Brush/Typography 或真正通用的基础主题可能属于 PackageShared。
Presenter、Item、Cell 和只服务一个控件族的命名 Theme 不属于 PackageShared。

显式共享资源使用：

```xml
<ItemGroup>
  <AtomUIPackageSharedTheme Include="Popup/Themes/OverlayPopupHostTheme.axaml" />
</ItemGroup>
```

Package 模式的普通 Control Theme 不需要该 item。Generator 不得把 owner 解析失败自动归类为共享资源。

## 6. `AtomUIRegistrationUnit` 边界

`AtomUIRegistrationUnit` 只允许出现在 Directory 模式，用于无法从 public owner 和目录约定推导、但明确只服务一个 Unit 的
resource-only Theme：

```xml
<AvaloniaXaml Update="Themes/InternalPresenterTheme.axaml"
               AtomUIRegistrationUnit="DatePicker" />
```

它是高级归属逃生口，不是普通第三方接入步骤，也不能用于维护跨 Unit 依赖图。

Package 模式忽略目录拆分，因而不得为内部 Theme 添加这类 metadata。DataGrid、ColorPicker 等单一控件族包中用于修补目录
拆分的现有 `AvaloniaXaml Update` 必须在迁移完成后删除。

## 7. Package Core 不受粒度影响

以下内容属于 Package Core，不参与 Unit 拆分：

- Language Catalog 和内置 Translation Bundle。
- Package Provider 与平台 Theme Asset selector。
- Global Token 和 Theme Algorithm。
- Dialog、Tooltip、Motion、Responsive 等 initializer。
- 显式 PackageShared 资源。

`UseXxxControls()` 仍由包作者维护，因为该方法拥有 Provider、Localization 和 initializer 的执行顺序。Generator 只从
`[ControlPackageRegistrationEntry]` 获取入口身份，并生成 full/generated registration helper；它不解析方法体猜测顺序。

粒度迁移不得改变普通 full 注册的 descriptor、asset、Catalog、Bundle、Provider、initializer 集合和执行顺序。

## 8. 不确定性与 fallback

正确性策略必须单调扩大：

| 情况 | 结果 |
| --- | --- |
| Package 模式中的静态 Control 使用 | 保留完整 Package Unit |
| Directory 模式中的静态 Control 使用 | 保留对应控件族 Unit |
| 可证明的同 Package 依赖 | 生成强类型 Unit 直接调用 |
| 已知 Unit 循环 | `TryEnterUnit` 去重并完整保留循环 |
| owner、C#/AXAML 依赖或动态输入无法证明 | 当前 Package full fallback |
| manifest major version 不兼容 | 构建错误 |
| 检测到 Control 使用但没有调用 Package 入口 | 构建错误 |

不允许从异常恢复、运行时反射、late registration 或资源缺失后重试 full registrar。

## 9. 第一方 Package 策略

| Package | 粒度 | 原因 |
| --- | --- | --- |
| `AtomUI.Desktop.Controls` | `Directory` | 包含大量相对独立的公开控件族，细粒度裁剪收益明确 |
| `AtomUI.Desktop.Controls.DataGrid` | `Package` | 单一产品控件族，内部目录紧密协作并存在循环 |
| `AtomUI.Desktop.Controls.ColorPicker` | `Package` | 单一产品控件族，公开 Control 会创建跨目录内部 View |
| `AtomUI.Desktop.Controls.Extras` | `Package` | 可选补充包，默认正确性优先 |
| `AtomUI.Toolkits.GalleryBase` | `Package` | Gallery 工具内部协作紧密，不作为目录级第三方样板 |

`AtomUI.Controls` Common 层仍由 Desktop 完整注册，不是独立 linked Package，也不声明粒度。

## 10. 第三方 Package 契约

普通第三方作者只需要：

1. 引用兼容版本的 AtomUI 产品包，由产品包提供同版本 Generator、Build Tasks 和 buildTransitive 资产。
2. 声明稳定的 `AtomUIRegistrationPackageId`。
3. 按 Control、可选 Own Token 和 `Themes/` 约定组织源码。
4. 在真实 public `UseXxxControls()` 方法上添加 `[ControlPackageRegistrationEntry]`。
5. 在入口中保持 full/generated 分支和 Package Core 顺序。
6. 让应用显式调用 `UseXxxControls()`。

普通作者不声明 Unit、依赖图、ownership 修补或 linker XML。只有真正的大型多控件包才启用 Directory 模式，并承担对应的
严格诊断、行为测试和发布体积验证。

完整代码示例由
[第三方 AtomUI Control Package 指南](../../guides/theming/third-party-control-packages.md)维护，本文不复制完整教程。

## 11. 验证契约

粒度实现或修改必须验证：

1. 未声明属性的第三方包生成单一 Package Unit，且不需要 `AtomUIRegistrationUnit`。
2. ColorPicker 只静态使用公开 `ColorPicker` 时仍保留其 C# 创建的内部 View 和主题。
3. DataGrid 不按 Cell、Column、Row、Utils 等实现目录拆分。
4. Desktop 显式 Directory 模式仍按控件族裁剪。
5. Package 和 Directory 模式的 ordinary/generated descriptor、asset、Language、Provider 和 initializer 行为一致。
6. 不确定 owner 或依赖只扩大为当前 Package full fallback。
7. trimmed JIT、NativeAOT、WebAssembly AOT 和 Gallery NativeAOT smoke 通过。
8. Desktop 的体积门槛继续满足
   [AOT 与裁剪架构](aot-and-trimming.md#16-验证与体积门槛)定义的要求。

本轮 2026-08-15 验证基线：macOS `osx-arm64`、Release、self-contained NativeAOT 下，Minimal fixture 总产物
`67,933,041` bytes，Full fixture `172,127,316` bytes，缩减约 `60.5%`；加入一个未使用 Unit 后增量为
`13,992` bytes，低于 `256 KiB` 门槛。后续修改必须重新运行统一脚本，不得直接沿用这些数字。
