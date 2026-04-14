# DataGrid 过滤功能

## 概述

DataGrid 内置列头过滤功能，支持多种过滤模式和多选过滤。过滤面板以弹出层形式显示在列头下方。

---

## 过滤控制属性

| 属性 | 位置 | 默认值 | 说明 |
|------|------|--------|------|
| `CanUserFilter` | DataGridColumn | `true` | 是否允许过滤此列 |
| `FilterMode` | DataGridColumn | - | 过滤模式 |
| `FilterMultiple` | DataGridColumn | `true` | 是否支持多选过滤 |
| `FilterOnClose` | DataGridColumn | `false` | 关闭面板时触发过滤 |

---

## 过滤模式

`DataGridFilterMode` 支持以下模式：

| 模式 | 说明 |
|------|------|
| `Equals` | 等于 |
| `Contains` | 包含 |
| `StartsWith` | 开头匹配 |
| `EndsWith` | 结尾匹配 |
| `GreaterThan` | 大于 |
| `LessThan` | 小于 |
| `GreaterThanOrEqual` | 大于等于 |
| `LessThanOrEqual` | 小于等于 |

---

## 基础过滤

点击列头的过滤图标打开过滤面板：

```xml
<atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" CanUserFilter="True" />
```

---

## 多选过滤

`FilterMultiple="True"` 时，过滤面板显示为多选列表，用户可同时选择多个过滤值：

```xml
<atom:DataGridTextColumn Header="Status" Binding="{Binding Status}"
    CanUserFilter="True" FilterMultiple="True" />
```

---

## 过滤触发时机

- `FilterOnClose="False"`（默认）：选择过滤值后立即触发过滤
- `FilterOnClose="True"`：关闭过滤面板时才触发过滤，允许用户选择多个值后再统一过滤

---

## 自定义过滤

监听 `Filtering` 事件实现自定义过滤逻辑：

```csharp
dataGrid.Filtering += (s, e) => {
    var column = e.Column;
    // 自定义过滤逻辑
};
```

---

## 过滤面板视觉

| 元素 | Token | 说明 |
|------|-------|------|
| 面板背景 | `FilterDropdownBg` | 弹出层背景色 |
| 选项背景 | `FilterDropdownMenuBg` | 选项项背景色 |
| 按钮悬浮 | `HeaderFilterHoverBg` | 过滤按钮悬浮背景 |

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Filter And Sorter / Filter In Tree