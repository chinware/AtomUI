# Drawer 自定义样式指南

Drawer 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过控件的公共属性来控制外观：

```xml
<!-- 基础抽屉 -->
<atom:Drawer Title="Basic Drawer">
    <TextBlock>Some contents...</TextBlock>
</atom:Drawer>

<!-- 自定义方向 -->
<atom:Drawer Placement="Left" Title="Left Drawer" />
<atom:Drawer Placement="Top" Title="Top Drawer" />
<atom:Drawer Placement="Bottom" Title="Bottom Drawer" />

<!-- 不同预设尺寸 -->
<atom:Drawer SizeType="Small" />   <!-- 378px -->
<atom:Drawer SizeType="Middle" />  <!-- 520px -->
<atom:Drawer SizeType="Large" />   <!-- 736px -->

<!-- 自定义尺寸（像素） -->
<atom:Drawer SizeType="Custom" DialogSize="400" />

<!-- 自定义尺寸（百分比） -->
<atom:Drawer SizeType="Custom" DialogSize="50%" />

<!-- 隐藏遮罩层 -->
<atom:Drawer IsShowMask="False" />

<!-- 隐藏关闭按钮 -->
<atom:Drawer IsShowCloseButton="False" />

<!-- 禁止点击遮罩关闭 -->
<atom:Drawer CloseWhenClickOnMask="False" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/DrawerShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Drawer 进行全局或局部样式覆盖：

### 全局设置方向

```xml
<Window.Styles>
    <Style Selector="atom|Drawer">
        <Setter Property="Placement" Value="Left" />
    </Style>
</Window.Styles>
```

### 按方向定制

```xml
<!-- 从底部滑入的抽屉增加额外间距 -->
<Style Selector="atom|Drawer[Placement=Bottom]">
    <Setter Property="Margin" Value="0,10" />
</Style>
```

---

## 3. 标题和额外操作区域

通过 `Title`、`Extra` 和 `Footer` 属性自定义标题栏和底部：

```xml
<atom:Drawer Title="Basic Drawer"
             Placement="{Binding DrawerPlacement}">
    <atom:Drawer.Extra>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:Button>Cancel</atom:Button>
            <atom:Button ButtonType="Primary">Ok</atom:Button>
        </StackPanel>
    </atom:Drawer.Extra>
    <atom:Drawer.Footer>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:Button>Edit</atom:Button>
            <atom:Button ButtonType="Primary">Upload</atom:Button>
            <atom:Button ButtonType="Primary" IsDanger="True">Delete</atom:Button>
        </StackPanel>
    </atom:Drawer.Footer>
    <StackPanel Orientation="Vertical" Spacing="5">
        <atom:TextBlock Text="Some contents..." />
        <atom:TextBlock Text="Some contents..." />
    </StackPanel>
</atom:Drawer>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/DrawerShowCase.axaml` 中 "Extra and Footer" 示例。

---

## 4. 作用域渲染

默认情况下 Drawer 在整个窗口上渲染。使用 `OpenOn` 可以限制渲染范围：

```xml
<gallery:ShowCaseItem Height="300">
    <Panel>
        <StackPanel Orientation="Vertical" Spacing="10">
            <atom:TextBlock>Render in this</atom:TextBlock>
            <atom:ToggleSwitch />
        </StackPanel>
        <atom:Drawer
            IsOpen="{Binding $parent[Panel].((atom:ToggleSwitch)Children[1]).IsChecked}"
            Title="Basic Drawer"
            OpenOn="{Binding $parent[gallery:ShowCaseItem]}">
            <StackPanel Orientation="Vertical" Spacing="5">
                <atom:TextBlock Text="Some contents..." />
            </StackPanel>
        </atom:Drawer>
    </Panel>
</gallery:ShowCaseItem>
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/DrawerShowCase.axaml` 中 "Render in current area" 示例。

---

## 5. 控制动画行为

```xml
<!-- 禁用滑入/滑出动画 -->
<atom:Drawer IsMotionEnabled="False" Title="No Animation" />
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Drawer` 语法引用 `atom` XML 命名空间下的 `Drawer` 类型，其中 `|` 是命名空间分隔符。由于 Drawer 的内容在 `DrawerContainer` / `DrawerInfoContainer` 中渲染，部分样式需要针对这些内部类型。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|Drawer` | 匹配所有 Drawer 实例 |
| `atom\|DrawerContainer` | 匹配抽屉容器（在 AdornerLayer 中渲染） |
| `atom\|DrawerInfoContainer` | 匹配抽屉信息容器（包含标题、内容、底部） |

### 按属性选择

| 选择器 | 说明 |
|---|---|
| `atom\|Drawer[Placement=Left]` | 匹配从左侧滑入的抽屉 |
| `atom\|Drawer[Placement=Right]` | 匹配从右侧滑入的抽屉 |
| `atom\|Drawer[Placement=Top]` | 匹配从顶部滑入的抽屉 |
| `atom\|Drawer[Placement=Bottom]` | 匹配从底部滑入的抽屉 |
| `atom\|Drawer[IsShowMask=False]` | 匹配无遮罩层的抽屉 |

### 按尺寸选择

| 选择器 | 说明 |
|---|---|
| `atom\|Drawer[SizeType=Small]` | 匹配小号抽屉（378px） |
| `atom\|Drawer[SizeType=Middle]` | 匹配中号抽屉（520px） |
| `atom\|Drawer[SizeType=Large]` | 匹配大号抽屉（736px） |
| `atom\|Drawer[SizeType=Custom]` | 匹配自定义尺寸抽屉 |

### 模板内部元素选择

| 选择器 | 说明 |
|---|---|
| `atom\|DrawerContainer /template/ atom\|DrawerInfoContainer#PART_InfoContainer` | 抽屉信息容器 |
| `atom\|DrawerInfoContainer /template/ atom\|IconButton#PART_CloseButton` | 关闭按钮 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Drawer[Placement=Right][SizeType=Large]` | 从右侧滑入的大号抽屉 |
| `atom\|Drawer[IsShowMask=False][Placement=Left]` | 无遮罩从左侧滑入的抽屉 |
