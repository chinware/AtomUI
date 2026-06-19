# Button 桌面版实现原理

本文档描述 Button 桌面版的内部实现组织、状态归一、模板接入和维护边界。公共设计与 API 契约见 [Button 桌面版架构设计](overview.md)，Token 语义见 [Button Token 设计](token.md)，变化记录见 [Button Changelog](changelog.md)。

## 1. 实现定位

Button 的实现目标是把兼容 API、正交 `Color + Variant` API、loading、shape、compact、wave 和自定义背景覆层归一为模板可消费的 stable state。实现文档只描述维护者必须理解的内部结构，不逐个复述私有方法。

Button.cs 保留公共属性、事件和接口实现入口；内部 helper 可以按职责拆分，但拆分不能改变 public API、模板契约、伪类或渲染结果。

## 2. 源码文件结构

主要源码：

- `src/AtomUI.Desktop.Controls/Buttons/Button.cs`：Button public API、Avalonia 属性注册、effective state、伪类同步、CompactSpace / Form / Wave 接口实现。
- `src/AtomUI.Desktop.Controls/Buttons/ButtonToken.cs`：Button 组件 Token 定义与派生。
- `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.axaml`：桌面 Button 模板、状态 selector 和主题变量映射。
- `src/AtomUI.Desktop.Controls/Buttons/Themes/BrowserButtonThemes.axaml`：Browser 风格 Button 主题。
- `src/AtomUI.Desktop.Controls/Buttons/Themes/ButtonTheme.cs`：主题资源注册辅助。
- `src/AtomUI.Controls/Buttons/ButtonPseudoClass.cs`：共享 Button 伪类定义。
- `src/AtomUI.Controls/Buttons/Converters/ButtonIconVisibleConverter.cs`：icon 可见性转换辅助。
- `src/AtomUI.Desktop.Controls/Buttons/DropdownButton.cs`、`SplitButton.cs`、`IconButton.cs`、`HyperLinkButton.cs`：Button 家族控件。

## 3. 核心类职责

`Button` 是状态归一中心。它同时承担 Avalonia Button 基类扩展、AtomUI 主题状态同步、CompactSpace 圆角和边框协同、Form 语义转发、Wave 几何和颜色感知。

`ButtonToken` 只提供组件级语义值，不保存实例状态。主题变量由 Button internal `StyledProperty` 承载，使 AXAML selector 和动态资源可以消费状态结果。

Button 家族控件复用 Button 的动作语义。派生控件可以替换模板或增加行为入口，但不得重新解释 `ButtonType`、`Color`、`Variant`、`IsDanger`、`IsGhost`、loading 和 disabled 语义。

## 4. 状态与数据流

Button 状态流：

```text
ButtonType / IsDanger / IsGhost / IsLoading / Shape / SizeType / Icon
Color? / Variant? / CustomBackground
      ↓
Normalize semantic state
      ↓
EffectiveColor / EffectiveVariant / EffectiveIsDanger / EffectiveIsGhost
EffectiveBorderThickness / EffectiveCornerRadius / WaveSpiritType
      ↓
Pseudo-classes + internal StyledProperty theme variables
      ↓
ButtonTheme.axaml visual states
```

`Color` 与 `Variant` 为 nullable，是兼容优先级的关键。归一逻辑必须能区分“用户未设置正交 API”和“用户显式设置默认值”。

自定义背景只影响 normal 状态的视觉覆层可见性。它不参与颜色语义、文字色、边框色、阴影和 wave brush 计算。

## 5. 生命周期与模板接入

Button 在静态构造中注册属性、伪类和主题关联，在实例构造中完成需要的状态订阅。模板应用时读取稳定 template part，并把状态同步到视觉节点。

维护顺序应遵守控件代码规范：

1. 字段与常量。
2. `StyledProperty` / `DirectProperty` / RoutedEvent 及其支持字段。
3. CLR wrapper。
4. 构造函数与静态构造函数。
5. 公共方法与接口显式实现。
6. protected 生命周期方法。
7. private 状态同步和计算方法。

显示接口实现属于公共能力的低一级入口，应靠近公共方法区域，不应被散落到文件末尾的纯 private 实现区。

## 6. 交互与事件处理

Button 交互沿 Avalonia Button 基类处理点击、命令、键盘和 enabled 状态。AtomUI 只在状态同步层补充 loading、wave、icon-only、shape、compact 和 custom background 可见性。

Wave 播放条件必须同时考虑 `IsWaveSpiritEnabled`、`IsMotionEnabled`、disabled、loading 和 Button 语义状态。Danger 和预设色 wave brush 必须来自 effective semantic state，而不是从 `CustomBackground` 读取。

Loading 状态影响 loading icon、原 icon 可见性和交互反馈，但不应通过直接禁用控件来模拟。

## 7. 内部算法与关键流程

关键流程：

- API 归一：显式 `Color + Variant` 优先，其次兼容 API 映射，再回退到默认语义。
- 伪类同步：当 public API、content、icon、loading、shape、enabled 或 compact 状态变化时同步模板可见状态。
- 有效边框：由 Button 类型、variant、enabled、bordered 状态和 compact 状态共同决定。
- 有效圆角：由 `CornerRadius`、`Shape`、`SizeType` 和 CompactSpace 位置共同决定。
- 自定义背景：由 `CustomBackground`、`EffectiveVariant`、危险态和 enabled 状态决定覆层是否参与显示。
- Wave 几何：Button 暴露 wave 所需边框和圆角，使 wave 与最终按钮边界一致。

这些流程必须保持 C# 层归一、AXAML 层消费的分工。不得把 API 优先级判断下沉到大量 AXAML selector 组合中。

## 8. 资源、性能与 AOT 边界

Button 主题变量使用 Avalonia 属性和动态资源，不使用反射读取模板状态。Token 资源由 ButtonToken scope 提供，并跟随主题切换。

Custom background 覆层是现有模板内的一层视觉节点，启用时不应增加额外控件实例或重建模板。未设置 `CustomBackground` 时，覆层保持不可见，不应影响默认路径的命中测试、wave 或内容布局。

Button 实现不得引入运行时反射、动态代码生成或非 AOT 友好的资源查找路径。

## 9. 维护不变量

内部重构必须保持以下不变量：

- public API、Avalonia 属性字段和 CLR wrapper 名称不变。
- StyledProperty / DirectProperty / RoutedEvent 与支持字段的定义顺序符合控件代码规范。
- `Button.cs` 保留公共属性、事件、方法和接口入口；实现拆分只承载内部逻辑。
- `Color + Variant` 优先级和旧 API 映射结果不变。
- `CustomBackgroundLayer` 不成为用户可依赖 template part。
- wave brush 不从 `CustomBackground`、模板背景或 hover 背景反推。
- CompactSpace 圆角和边框折叠行为不变。
- Browser 主题与桌面主题在同一 API 下语义一致。

## 10. 测试与验证

验证范围：

- API 归一：覆盖 `ButtonType`、`IsDanger`、`Color`、`Variant`、`IsGhost` 的组合。
- 状态同步：覆盖 disabled、loading、hover、pressed、icon-only、circle、round。
- Wave：覆盖危险态、预设色、custom background 与 disabled / loading 播放条件。
- Theme：检查 default、primary、dashed、text、link、solid、outlined、filled、danger 和 custom background 视觉。
- 家族控件：检查 DropdownButton、SplitButton、IconButton、HyperLinkButton 是否继承或明确处理 Button 语义。
- 文档改动：运行 `git diff --check`。
