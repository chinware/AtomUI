# Grid Design Token

Grid 栅格系统（`Row` / `Col`）是一个**纯布局控件**，不定义任何组件级 Design Token（无 `GridToken` 类），也不消费全局 `SharedToken` 中的视觉属性。

---

## 设计原因

栅格系统的唯一职责是**空间分配与子元素排列**——它将容器宽度等分为 24 份，按照 `Span`、`Offset`、`Gutter`、`Justify`、`Align` 等参数计算每个列的位置和尺寸。栅格自身不渲染任何可见的背景色、边框、字体或间距 Token，所有视觉样式由放置在 `Col` 内的子控件自行管理。

这与 Ant Design 的 Grid 组件一致——React 版本的 Grid 也没有独立的 Design Token。

---

## 间距配置 vs Design Token

虽然 Grid 不使用 Design Token，但 `Row.Gutter` 属性提供了等效的间距控制能力：

| 配置方式 | 示例 | 说明 |
|---|---|---|
| 固定间距 | `Gutter="16"` | 所有断点使用相同间距 |
| 水平+垂直 | `Gutter="16,24"` | 水平 16px，垂直 24px |
| 响应式间距 | `Gutter="xs:8,sm:16,md:24,lg:32"` | 不同断点使用不同间距 |

这些间距值由开发者直接指定，而非从 Token 系统派生。如果希望保持与主题的一致性，建议使用 `SharedToken` 中的间距值：

```xml
<!-- 使用 SharedToken 间距值保持主题一致性 -->
<atom:Row Gutter="{atom:SharedTokenResource Spacing}">
    <!-- ... -->
</atom:Row>
```

---

## 子控件的 Token 使用

放置在 `Col` 内的子控件正常使用各自的 Design Token。例如：

```xml
<atom:Row Gutter="16,16">
    <atom:Col Span="12">
        <!-- Button 使用 ButtonToken -->
        <atom:Button ButtonType="Primary">Submit</atom:Button>
    </atom:Col>
    <atom:Col Span="12">
        <!-- Tag 使用 TagToken -->
        <atom:Tag TagColor="Processing">Processing</atom:Tag>
    </atom:Col>
</atom:Row>
```

Grid 不会干扰子控件的 Token 作用域。
