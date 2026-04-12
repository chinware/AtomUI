# Cascader Design Token

Cascader 控件使用 `CascaderToken`（Token ID: `"Cascader"`）作为组件级 Design Token，所有视觉属性均从全局 `SharedToken` 派生，不硬编码任何具体值。

---

## Token 资源访问方式

在 AXAML 中使用组件级 Token：

```xml
{atom:CascaderTokenResource ControlWidth}
{atom:CascaderTokenResource OptionSelectedBg}
{atom:CascaderTokenResource DropdownHeight}
```

在 AXAML 中使用全局共享 Token：

```xml
{atom:SharedTokenResource ColorPrimary}
{atom:SharedTokenResource ControlHeight}
{atom:SharedTokenResource ColorBgElevated}
```

---

## 组件级 Token 一览

以下是 `CascaderToken` 定义的全部组件级 Token，按功能分组说明。

### 尺寸与布局 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `ControlWidth` | `double` | 固定 `184` | Cascader 选择器默认宽度 |
| `ControlItemWidth` | `double` | 固定 `111` | 单个选项列（级别列表）的宽度 |
| `DropdownHeight` | `double` | 固定 `180` | 下拉菜单最大高度 |
| `HeaderHeight` | `double` | `SharedToken.ControlHeightSM` | 节点标题行高度 |

### 颜色 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `OptionSelectedBg` | `Color` | `SharedToken.ControlItemBgActive` | 选项被选中时的背景色 |
| `OptionHoverBg` | `Color` | `SharedToken.ControlItemBgHover` | 选项鼠标悬浮时的背景色 |
| `OptionSelectedColor` | `Color` | `SharedToken.ColorText` | 选项被选中时的文本颜色 |
| `FilterHighlightColor` | `Color` | `SharedToken.ColorError` | 搜索过滤关键词的高亮颜色（默认使用错误色系红色） |

### 字体 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `OptionSelectedFontWeight` | `FontWeight` | `SharedToken.FontWeightStrong` | 选项被选中时的字体粗细 |

### 间距 Token

| Token 名 | 类型 | 来源 | 说明 |
|---|---|---|---|
| `OptionPadding` | `Thickness` | 基于 `SharedToken.UniformlyPaddingSM` 和 `(ControlHeight - FontHeight) / 2` | 单个选项的内间距 |
| `MenuPadding` | `Thickness` | `SharedToken.PaddingXXS` | 单级选项列表的内间距 |
| `ItemHeaderSpacing` | `double` | `SharedToken.SpacingXXS` | 选项头部元素之间的间距（如图标与文本之间） |

---

## 主题中使用的全局 SharedToken

除组件级 Token 外，Cascader 的 ControlTheme 还直接引用了以下全局 `SharedToken`：

| Token 资源键 | 使用场景 |
|---|---|
| `EnableMotion` | 全局动画开关 |
| `ControlHeight` | 选项项高度 |
| `ControlHeightSM` | 节点标题高度 |
| `FontSize` / `FontSizeLG` | 中号/大号字体大小 |
| `ColorText` | 默认文本颜色（单选结果文本） |
| `ColorTextPlaceholder` | 占位文本颜色、下拉打开时的路径文本颜色 |
| `ColorTextDisabled` | 禁用态文本颜色 |
| `ColorBgElevated` | 弹出框架背景色 |
| `SpacingXS` | 右侧附加区域子元素间距 |

### PopupHost Token

弹出层还引用了 `PopupHostToken` 中的以下 Token：

| Token 资源键 | 使用场景 |
|---|---|
| `MarginToAnchor` | 弹出层与锚点之间的间距 |
| `BoxShadows` | 弹出层的阴影效果 |
| `BorderRadius` | 弹出框架的圆角半径 |

---

## Token 对外观的具体影响

### 选项项状态与 Token 映射

| 状态 | 背景色 | 文本颜色 | 字体粗细 |
|---|---|---|---|
| **正常** | 透明 | 继承 | 正常 |
| **悬浮** | `OptionHoverBg`（`ControlItemBgHover`） | 继承 | 正常 |
| **选中** | `OptionSelectedBg`（`ControlItemBgActive`） | `OptionSelectedColor`（`ColorText`） | `OptionSelectedFontWeight`（`FontWeightStrong`） |
| **禁用** | 透明 | `ColorTextDisabled` | 正常 |

### 尺寸与 Token 映射

| 尺寸 | 控件字号 | 选项项高度 |
|---|---|---|
| `Large` | `FontSizeLG` | `ControlHeight` |
| `Middle` | `FontSize` | `ControlHeight` |
| `Small` | `FontSize` | `ControlHeight` |

### 宽度控制

- 当 `HorizontalAlignment` 不为 `Stretch` 时，Cascader 使用 `ControlWidth`（184px）作为默认宽度
- 弹出面板最小宽度为 `ControlItemWidth`（111px）
- 弹出面板高度限制为 `DropdownHeight`（180px）

### 搜索高亮

- 搜索匹配的关键词使用 `FilterHighlightColor` 高亮显示
- 默认为红色（`SharedToken.ColorError`），与 Ant Design 行为一致

### 图标默认值

- 展开箭头：`RightOutlined`（Ant Design 右箭头图标）
- 加载动画：`LoadingOutlined`（带 Spin 旋转动画）
- 后缀图标：继承自 `AbstractSelect` 的默认下拉箭头

---

## Token 覆盖示例

可通过自定义 `CascaderToken` 来覆盖默认的 Token 值。由于 Token 类是 `internal` 的，通常通过全局主题配置或 Style 中的资源覆盖来实现视觉定制：

```xml
<!-- 通过 Style Setter 覆盖弹出层相关视觉 -->
<Style Selector="atom|Cascader /template/ Border#PopupFrame">
    <Setter Property="Background" Value="#f5f5f5" />
</Style>

<!-- 通过 Style 覆盖选项悬浮效果 -->
<Style Selector="atom|CascaderViewItem:pointerover">
    <Setter Property="Background" Value="#e6f7ff" />
</Style>
```
