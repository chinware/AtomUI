# Upload 自定义样式指南

Upload 的视觉表现通过 `ControlTheme` + Design Token 系统控制。Upload 是一个复合控件，包含多个内部子组件，以下介绍常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 Upload 的公共属性来控制外观和行为：

```xml
<!-- 不同列表类型 -->
<atom:Upload ListType="Text">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">Upload</atom:Button>
</atom:Upload>

<atom:Upload ListType="Picture">
    <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}" ButtonType="Primary">Upload</atom:Button>
</atom:Upload>

<atom:Upload ListType="PictureCard">
    <StackPanel Orientation="Vertical">
        <antdicons:PlusOutlined />
        <TextBlock Margin="0, 8, 0, 0">Upload</TextBlock>
    </StackPanel>
</atom:Upload>

<atom:Upload ListType="PictureCircle">
    <StackPanel Orientation="Vertical">
        <antdicons:PlusOutlined />
        <TextBlock Margin="0, 8, 0, 0">Upload</TextBlock>
    </StackPanel>
</atom:Upload>

<!-- 控制显示/隐藏 -->
<atom:Upload IsShowUploadList="False">
    <atom:Button>Upload (no list)</atom:Button>
</atom:Upload>

<atom:Upload IsShowUploadTrigger="False" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/UploadShowCase.axaml`

---

## 2. 自定义拖拽区域

`UploadDefaultDropArea` 提供了多个可自定义的属性：

```xml
<!-- 自定义图标、标题和副标题 -->
<atom:Upload>
    <atom:UploadDefaultDropArea
        Header="拖拽文件到此处上传"
        SubHeader="支持单个或批量上传，严禁上传公司内部资料或其他违禁文件">
        <!-- DropIcon 默认为 InboxOutlined -->
    </atom:UploadDefaultDropArea>
</atom:Upload>

<!-- 使用模板自定义标题内容 -->
<atom:Upload>
    <atom:UploadDefaultDropArea>
        <atom:UploadDefaultDropArea.HeaderTemplate>
            <DataTemplate>
                <TextBlock Text="Custom Header" FontWeight="Bold" />
            </DataTemplate>
        </atom:UploadDefaultDropArea.HeaderTemplate>
    </atom:UploadDefaultDropArea>
</atom:Upload>
```

---

## 3. 自定义 PictureCard/PictureCircle 触发内容

PictureCard 和 PictureCircle 类型的触发区完全可自定义：

```xml
<!-- 带加载状态切换的触发按钮 -->
<atom:Upload ListType="PictureCard">
    <StackPanel Orientation="Vertical">
        <Panel>
            <antdicons:PlusOutlined
                IsVisible="{Binding IsTaskRunning,
                            RelativeSource={RelativeSource AncestorType=atom:Upload},
                            Converter={x:Static BoolConverters.Not}}" />
            <antdicons:LoadingOutlined
                IsVisible="{Binding IsTaskRunning,
                            RelativeSource={RelativeSource AncestorType=atom:Upload}}"
                LoadingAnimation="Spin" />
        </Panel>
        <TextBlock Margin="0, 8, 0, 0">Upload</TextBlock>
    </StackPanel>
</atom:Upload>
```

---

## 4. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 Upload 及其子组件进行全局或局部样式覆盖：

### 全局统一间距

```xml
<Window.Styles>
    <Style Selector="atom|Upload">
        <Setter Property="Margin" Value="0, 10" />
    </Style>
</Window.Styles>
```

### 按列表类型定制

```xml
<!-- 禁用 PictureCard 类型的内容对齐 -->
<Style Selector="atom|Upload[ListType=PictureCard]">
    <Setter Property="HorizontalContentAlignment" Value="Left" />
</Style>
```

### 自定义拖拽区域样式

```xml
<!-- 自定义拖拽区域的背景和边框 -->
<Style Selector="atom|UploadDefaultDropArea">
    <Setter Property="Background" Value="#f0f5ff" />
    <Setter Property="BorderBrush" Value="#1677ff" />
</Style>

<!-- 自定义悬浮状态 -->
<Style Selector="atom|UploadDefaultDropArea:pointerover">
    <Setter Property="BorderBrush" Value="#4096ff" />
</Style>
```

---

## 5. 通过 ControlTheme 完全替换主题

如果需要彻底替换 Upload 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomUpload" TargetType="atom:Upload">
    <Setter Property="Template">
        <ControlTemplate>
            <!-- 自定义模板结构 -->
        </ControlTemplate>
    </Setter>
</ControlTheme>

<atom:Upload Theme="{StaticResource MyCustomUpload}" />
```

> ⚠️ 注意：Upload 是一个复杂的复合控件，完全替换 ControlTheme 需要重新实现触发区、文件列表、图片预览等所有子组件的模板。建议优先使用 Style 覆盖和属性配置。

---

## 6. 控制动画行为

```xml
<!-- 禁用过渡动画 -->
<atom:Upload IsMotionEnabled="False">
    <atom:Button>Upload</atom:Button>
</atom:Upload>
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Upload` 语法引用 `atom` XML 命名空间下的 `Upload` 类型，其中 `|` 是命名空间分隔符。

### Upload 主控件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Upload` | 匹配所有 Upload 实例 |
| `atom\|Upload[ListType=Text]` | 匹配文本列表类型的 Upload |
| `atom\|Upload[ListType=Picture]` | 匹配图片列表类型的 Upload |
| `atom\|Upload[ListType=PictureCard]` | 匹配图片卡片墙类型的 Upload |
| `atom\|Upload[ListType=PictureCircle]` | 匹配圆形图片墙类型的 Upload |
| `atom\|Upload:disabled` | 匹配禁用状态的 Upload |

### UploadDefaultDropArea 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|UploadDefaultDropArea` | 匹配所有拖拽上传区域实例 |
| `atom\|UploadDefaultDropArea:pointerover` | 拖拽区域鼠标悬浮状态（边框高亮为主色调） |
| `atom\|UploadDefaultDropArea:disabled` | 拖拽区域禁用状态（图标和文字变灰） |
| `atom\|UploadDefaultDropArea /template/ atom\|DashedBorder#Frame` | 访问拖拽区域模板内的虚线边框 |
| `atom\|UploadDefaultDropArea /template/ atom\|IconPresenter#IconPresenter` | 访问拖拽区域模板内的图标 |
| `atom\|UploadDefaultDropArea /template/ ContentPresenter#HeaderContentPresenter` | 访问主标题内容展示器 |
| `atom\|UploadDefaultDropArea /template/ ContentPresenter#SubHeaderContentPresenter` | 访问副标题内容展示器 |

### UploadTriggerContent 选择器（内部组件）

| 选择器 | 说明 |
|---|---|
| `atom\|UploadTriggerContent[ListType=PictureCard]` | PictureCard 类型的触发区（虚线边框卡片） |
| `atom\|UploadTriggerContent[ListType=PictureCircle]` | PictureCircle 类型的触发区（虚线边框圆形） |
| `atom\|UploadTriggerContent[ListType=PictureCard]:pointerover` | PictureCard 触发区悬浮状态 |
| `atom\|UploadTriggerContent[ListType=PictureCircle]:pointerover` | PictureCircle 触发区悬浮状态 |
| `atom\|UploadTriggerContent:disabled` | 禁用状态的触发区 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|Upload[ListType=Text]:not(:disabled)` | 非禁用状态的文本列表 Upload |
| `atom\|Upload[ListType=PictureCard] /template/ atom\|UploadPictureShapeList` | 访问 PictureCard 模板内的图片列表 |
| `atom\|UploadDefaultDropArea /template/ atom\|DashedBorder#Frame` | 自定义拖拽区域的虚线边框样式 |
