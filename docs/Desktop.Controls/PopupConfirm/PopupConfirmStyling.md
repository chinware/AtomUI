# PopupConfirm 自定义样式指南

---

## 1. 通过属性直接控制

```xml
<atom:PopupConfirm />
```

---

## 2. 通过 Style 覆盖

```xml
<Window.Styles>
    <Style Selector="atom|PopupConfirm">
        <!-- 自定义属性 -->
    </Style>
</Window.Styles>
```

---

## 样式选择器速查

| 选择器 | 说明 |
|---|---|
| `atom\|PopupConfirm` | 匹配所有 PopupConfirm |
| `atom\|PopupConfirm:pointerover` | 鼠标悬浮 |
| `atom\|PopupConfirm:disabled` | 禁用 |

