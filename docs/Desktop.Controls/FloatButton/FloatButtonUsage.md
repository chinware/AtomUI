# FloatButton 使用文档

本文档介绍 AtomUI FloatButton 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/General/FloatButtonShowCase.axaml`

---

## 前置准备

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

```csharp
using AtomUI.Desktop.Controls;
```

---

## 1. 基本用法

最基本的悬浮按钮，放在 `ScrollViewer` 的 `Panel` 内：

```xml
<atom:ScrollViewer Height="300">
    <Panel Height="500">
        <atom:FloatButtonHost />
    </Panel>
</atom:ScrollViewer>
```

**要点**：
- `FloatButtonHost` 自动将悬浮按钮放置在 OverlayLayer 上
- 默认图标为 `FileTextOutlined`
- 默认位置为右下角

---

## 2. 按钮类型

通过 `ButtonType` 切换 Default 和 Primary 样式：

```xml
<Panel Height="500">
    <atom:FloatButtonHost ButtonType="Default"
                           Icon="{antdicons:AntDesignIconProvider Kind=QuestionCircleOutlined}"
                           FloatOffsetX="80" />
    <atom:FloatButtonHost ButtonType="Primary"
                           Icon="{antdicons:AntDesignIconProvider Kind=QuestionCircleOutlined}" />
</Panel>
```

---

## 3. 按钮形状

通过 `Shape` 切换圆形和方形：

```xml
<Panel Height="500">
    <atom:FloatButtonHost ButtonType="Primary"
                           Shape="Circle"
                           Icon="{antdicons:AntDesignIconProvider Kind=CustomerServiceOutlined}"
                           FloatOffsetX="80" />
    <atom:FloatButtonHost ButtonType="Primary"
                           Shape="Square"
                           Icon="{antdicons:AntDesignIconProvider Kind=CustomerServiceOutlined}" />
</Panel>
```

---

## 4. 带 Tooltip

通过 `Tooltip` 属性设置鼠标悬浮提示，`TooltipColor` 可自定义颜色：

```xml
<Panel Height="500">
    <atom:FloatButtonHost FloatOffsetX="80"
                           Tooltip="Since 5.25.0+" TooltipColor="blue" />
    <atom:FloatButtonHost Tooltip="Documents" />
</Panel>
```

---

## 5. 带描述文本

`Shape="Square"` 时可以通过 `Description` 显示描述：

```xml
<Panel Height="500">
    <atom:FloatButtonHost Shape="Square"
                           Icon="{antdicons:AntDesignIconProvider Kind=FileTextOutlined}"
                           FloatOffsetX="145"
                           Description="HELP INFO" />
    <atom:FloatButtonHost Shape="Square"
                           FloatOffsetX="80"
                           Description="HELP INFO" />
    <atom:FloatButtonHost Shape="Square"
                           Icon="{antdicons:AntDesignIconProvider Kind=FileTextOutlined}"
                           Description="HELP INFO" />
</Panel>
```

---

## 6. 按钮组

将多个 FloatButton 放在 `FloatButtonGroupHost` 中。子按钮的 `Shape` 和 `IsMotionEnabled` 会自动从 Group 继承：

```xml
<Panel Height="500">
    <!-- 方形分组 -->
    <atom:FloatButtonGroupHost Shape="Square" FloatOffsetX="80">
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=QuestionCircleOutlined}" />
        <atom:FloatButton />
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=SyncOutlined}" />
        <atom:BackTopFloatButton />
    </atom:FloatButtonGroupHost>

    <!-- 圆形分组 -->
    <atom:FloatButtonGroupHost Shape="Circle">
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=QuestionCircleOutlined}" />
        <atom:FloatButton />
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=SyncOutlined}" />
        <atom:BackTopFloatButton />
    </atom:FloatButtonGroupHost>
</Panel>
```

---

## 7. 触发模式

通过 `Trigger` 属性控制展开方式：

```xml
<Panel Height="500">
    <!-- 悬浮触发 -->
    <atom:FloatButtonGroupHost Shape="Circle"
                                Trigger="Hover"
                                Icon="{antdicons:AntDesignIconProvider Kind=CustomerServiceOutlined}"
                                ButtonType="Primary"
                                FloatOffsetX="80">
        <atom:FloatButton />
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=CommentOutlined}" />
    </atom:FloatButtonGroupHost>

    <!-- 点击触发 -->
    <atom:FloatButtonGroupHost Shape="Circle"
                                Trigger="Click"
                                Icon="{antdicons:AntDesignIconProvider Kind=CustomerServiceOutlined}"
                                ButtonType="Primary">
        <atom:FloatButton />
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=CommentOutlined}" />
    </atom:FloatButtonGroupHost>
</Panel>
```

**要点**：
- `Click` 模式下，点击外部区域自动收起
- `Hover` 模式下，鼠标离开自动收起
- `Default` 模式无触发按钮，所有子按钮始终可见

---

## 8. 展开方向

通过 `MenuPlacement` 控制子按钮展开方向：

```xml
<Panel Height="600">
    <!-- 向上展开 -->
    <atom:FloatButtonGroupHost Trigger="Click" Placement="Center"
                                MenuPlacement="Top" FloatOffsetY="-60"
                                Icon="{antdicons:AntDesignIconProvider Kind=UpOutlined}">
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=WechatOutlined}" />
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=QqOutlined}" />
    </atom:FloatButtonGroupHost>

    <!-- 向右展开 -->
    <atom:FloatButtonGroupHost Trigger="Click" Placement="Center"
                                MenuPlacement="Right" FloatOffsetX="60"
                                Icon="{antdicons:AntDesignIconProvider Kind=RightOutlined}">
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=WechatOutlined}" />
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=QqOutlined}" />
    </atom:FloatButtonGroupHost>
</Panel>
```

**要点**：
- 展开使用 `MoveIn*` 系列动画，收起使用 `MoveOut*` 系列动画
- 方向与 `MenuPlacement` 对应：`Top` 使用 `MoveDownIn/Out`，`Bottom` 使用 `MoveUpIn/Out`

---

## 9. 徽标

通过 `IsBadgeEnabled`、`BadgeCount` 等属性显示徽标：

```xml
<Panel Height="500">
    <!-- 圆点徽标 -->
    <atom:FloatButtonHost FloatOffsetX="145" IsBadgeEnabled="True" IsDotBadge="True" />

    <!-- 数字徽标 -->
    <atom:FloatButtonGroupHost Shape="Circle" ButtonType="Primary" FloatOffsetX="80">
        <atom:FloatButton IsBadgeEnabled="True" IsDotBadge="False"
                           BadgeCount="5" BadgeColor="blue" />
        <atom:FloatButton Icon="{antdicons:AntDesignIconProvider Kind=CommentOutlined}"
                           IsBadgeEnabled="True" IsDotBadge="False" BadgeCount="5" />
    </atom:FloatButtonGroupHost>
</Panel>
```

**要点**：
- `IsDotBadge="True"` 使用圆点徽标（`DotBadgeAdorner`），否则使用数字徽标（`CountBadgeAdorner`）
- `BadgeOverflowCount` 默认 99，超过时显示 `99+`
- `BadgeColor` 支持预设色名（如 `"blue"`、`"red"`）和自定义 hex 值

---

## 10. 返回顶部

```xml
<atom:ScrollViewer Height="300">
    <Panel Height="1000">
        <StackPanel>
            <TextBlock>Scroll to bottom</TextBlock>
            <!-- 更多内容... -->
        </StackPanel>
        <atom:BackTopFloatButtonHost ButtonType="Default" />
    </Panel>
</atom:ScrollViewer>
```

**要点**：
- 滚动超过 `VisibilityHeight`（默认 400px）时按钮显示
- 点击后平滑滚动到顶部，时长由 `ToTopDuration`（默认 450ms）控制
- 未指定 `Target` 时自动查找父级 `ScrollViewer`
- `BackTopFloatButton` 的默认图标为 `VerticalAlignTopOutlined`
