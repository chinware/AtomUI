# Result 自定义样式指南

---

## 1. 通过属性直接控制

Result 控件的主要外观由 `Status` 属性驱动。设置 `Status` 后，图标类型、图标颜色、SVG 插画等均自动匹配。

### 基本状态控制

```xml
<!-- 成功状态 -->
<atom:Result Status="Success"
             Header="操作成功"
             SubHeader="详细描述信息" />

<!-- 错误状态 -->
<atom:Result Status="Error"
             Header="操作失败" />

<!-- HTTP 404 页面 -->
<atom:Result Status="ErrorCode404"
             Header="404"
             SubHeader="页面未找到" />
```

### 自定义图标

通过 `Icon` 属性覆盖默认状态图标（仅对 Info/Success/Warning/Error 有效）：

```xml
<atom:Result Status="Info"
             Icon="{antdicons:AntDesignIconProvider SmileOutlined}"
             Header="自定义图标的结果页" />
```

### 自定义标题和副标题模板

```xml
<atom:Result Status="Success">
    <atom:Result.HeaderTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="8">
                <antdicons:CheckCircleFilled />
                <TextBlock Text="自定义标题模板" FontWeight="Bold" />
            </StackPanel>
        </DataTemplate>
    </atom:Result.HeaderTemplate>
</atom:Result>
```

### 额外操作区

通过 `Extra` 属性放置操作按钮：

```xml
<atom:Result Status="Success" Header="购买成功">
    <atom:Result.Extra>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:Button ButtonType="Primary">去控制台</atom:Button>
            <atom:Button>再次购买</atom:Button>
        </StackPanel>
    </atom:Result.Extra>
</atom:Result>
```

### 子内容区域

通过 `Content`（继承自 `ContentControl`）在结果下方展示详细信息：

```xml
<atom:Result Status="Error"
             Header="提交失败"
             SubHeader="请检查以下错误信息后重试">
    <atom:Result.Extra>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:Button ButtonType="Primary">返回修改</atom:Button>
            <atom:Button>取消</atom:Button>
        </StackPanel>
    </atom:Result.Extra>
    <!-- Content 子内容：带背景色的详细信息区域 -->
    <StackPanel Spacing="8">
        <TextBlock FontWeight="Bold" FontSize="16">
            提交内容存在以下错误：
        </TextBlock>
        <StackPanel Orientation="Horizontal" Spacing="8">
            <antdicons:CloseCircleOutlined Foreground="{atom:SharedTokenResource ColorError}" />
            <TextBlock>您的账户已被冻结。</TextBlock>
            <TextBlock Foreground="{atom:SharedTokenResource ColorPrimary}">立即解冻 ></TextBlock>
        </StackPanel>
    </StackPanel>
</atom:Result>
```

> 以上示例参考自 Gallery：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ResultShowCase.cs.axaml`

---

## 2. 通过 Style 覆盖

### 全局样式覆盖

```xml
<Window.Styles>
    <!-- 修改所有 Result 的整体内边距 -->
    <Style Selector="atom|Result">
        <Setter Property="Padding" Value="48,32" />
    </Style>
    
    <!-- 修改标题样式 -->
    <Style Selector="atom|Result /template/ ContentPresenter#Header">
        <Setter Property="Foreground" Value="DarkBlue" />
        <Setter Property="FontWeight" Value="Bold" />
    </Style>
    
    <!-- 修改副标题样式 -->
    <Style Selector="atom|Result /template/ ContentPresenter#SubHeader">
        <Setter Property="Foreground" Value="Gray" />
    </Style>
    
    <!-- 修改子内容区域背景 -->
    <Style Selector="atom|Result /template/ ContentPresenter#Content">
        <Setter Property="Background" Value="#F5F5F5" />
    </Style>
</Window.Styles>
```

### 按状态定制样式

Result 使用属性选择器 `[Status=xxx]` 区分不同状态的样式：

```xml
<Window.Styles>
    <!-- 仅对 Success 状态的标题加粗 -->
    <Style Selector="atom|Result[Status=Success] /template/ ContentPresenter#Header">
        <Setter Property="FontWeight" Value="Bold" />
    </Style>
    
    <!-- 仅对 Error 状态添加红色边框 -->
    <Style Selector="atom|Result[Status=Error] /template/ Border#Frame">
        <Setter Property="BorderBrush" Value="{atom:SharedTokenResource ColorError}" />
        <Setter Property="BorderThickness" Value="1" />
    </Style>
</Window.Styles>
```

---

## 3. 属性选择器速查

| 选择器 | 说明 |
|---|---|
| `atom\|Result` | 匹配所有 Result |
| `atom\|Result[Status=Info]` | 匹配 Info 状态 |
| `atom\|Result[Status=Success]` | 匹配 Success 状态 |
| `atom\|Result[Status=Warning]` | 匹配 Warning 状态 |
| `atom\|Result[Status=Error]` | 匹配 Error 状态 |
| `atom\|Result[Status=ErrorCode403]` | 匹配 403 状态 |
| `atom\|Result[Status=ErrorCode404]` | 匹配 404 状态 |
| `atom\|Result[Status=ErrorCode500]` | 匹配 500 状态 |

---

## 4. 模板部件选择器速查

| 选择器 | 说明 |
|---|---|
| `atom\|Result /template/ Border#Frame` | 根框架 |
| `atom\|Result /template/ StackPanel#RootLayout` | 布局容器 |
| `atom\|Result /template/ ContentPresenter#PART_StatusIconPresenter` | 状态图标容器 |
| `atom\|Result /template/ Svg#PART_ErrorCodeImage` | HTTP 错误码 SVG 插画 |
| `atom\|Result /template/ ContentPresenter#Header` | 标题 |
| `atom\|Result /template/ ContentPresenter#SubHeader` | 副标题 |
| `atom\|Result /template/ ContentPresenter#ExtraContent` | 额外操作区 |
| `atom\|Result /template/ ContentPresenter#Content` | 子内容区域 |

---

## 5. 注意事项

- **状态图标颜色**通过桌面端 `Result.cs` 的实例样式（C# 代码）设置，而非 AXAML ControlTheme。如需覆盖，需使用优先级更高的样式选择器。
- **图标尺寸**由 `ResultToken.IconSize` 控制，在实例样式中绑定到 `PART_StatusIconPresenter` 的子元素。
- HTTP 错误码状态（403/404/500）的 SVG 插画尺寸由 `ImageWidth` 和 `ImageHeight` Token 控制，默认为 250×295。
- `Icon` 属性仅对四种基本状态生效，对 HTTP 错误码状态无效。
