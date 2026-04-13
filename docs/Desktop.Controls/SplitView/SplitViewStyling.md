# SplitView 自定义样式指南

SplitView 的视觉表现通过 `ControlTheme` + Design Token 系统控制。以下介绍几种常见的自定义方式。

---

## 1. 使用属性直接控制

最简单的方式是通过 SplitView 的公共属性来控制外观和行为：

```xml
<!-- 基础用法：左侧面板，Inline 模式 -->
<atom:SplitView DisplayMode="Inline"
                IsPaneOpen="True"
                OpenPaneLength="250">
    <atom:SplitView.Pane>
        <TextBlock Text="侧边栏内容" Padding="10" />
    </atom:SplitView.Pane>
    <TextBlock Text="主内容区域" Padding="10" />
</atom:SplitView>

<!-- 不同显示模式 -->
<atom:SplitView DisplayMode="Overlay" IsPaneOpen="True" />
<atom:SplitView DisplayMode="CompactInline" CompactPaneLength="48" />
<atom:SplitView DisplayMode="CompactOverlay" CompactPaneLength="48" />

<!-- 不同面板位置 -->
<atom:SplitView PanePlacement="Right" DisplayMode="Inline" />
<atom:SplitView PanePlacement="Top" DisplayMode="Inline" />
<atom:SplitView PanePlacement="Bottom" DisplayMode="Overlay" />

<!-- 自定义动画参数 -->
<atom:SplitView PaneOpenMotionDuration="0:0:0.3"
                PaneCloseMotionDuration="0:0:0.15" />

<!-- 禁用动画 -->
<atom:SplitView IsMotionEnabled="False" />

<!-- 启用轻量关闭遮罩 -->
<atom:SplitView DisplayMode="Overlay"
                UseLightDismissOverlayMode="True" />
```

---

## 2. 通过 Style 覆盖样式

可以在 AXAML 中使用 `Style` 对 SplitView 进行全局或局部样式覆盖：

### 全局统一面板宽度

```xml
<Window.Styles>
    <Style Selector="atom|SplitView">
        <Setter Property="OpenPaneLength" Value="280" />
        <Setter Property="CompactPaneLength" Value="56" />
    </Style>
</Window.Styles>
```

### 自定义面板背景色

```xml
<Style Selector="atom|SplitView">
    <Setter Property="PaneBackground" Value="#F5F5F5" />
</Style>
```

### 按显示模式定制

```xml
<!-- CompactInline 模式下使用更窄的紧凑宽度 -->
<Style Selector="atom|SplitView:compactinline">
    <Setter Property="CompactPaneLength" Value="40" />
</Style>
```

### 按面板位置定制分隔线

```xml
<!-- 右侧面板时使用更粗的分隔线 -->
<Style Selector="atom|SplitView:right /template/ Rectangle#HCPaneBorder">
    <Setter Property="Width" Value="2" />
</Style>
```

---

## 3. 通过 ControlTheme 完全替换主题

如果需要彻底替换 SplitView 的模板和样式，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomSplitView" TargetType="atom:SplitView">
    <Setter Property="Template">
        <ControlTemplate>
            <Grid>
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="Auto" />
                    <ColumnDefinition Width="*" />
                </Grid.ColumnDefinitions>
                <Panel Name="PART_PaneRoot"
                       Background="{TemplateBinding PaneBackground}">
                    <ContentPresenter x:Name="PART_PanePresenter"
                                      Content="{TemplateBinding Pane}" />
                </Panel>
                <Panel Name="ContentRoot" Grid.Column="1">
                    <ContentPresenter x:Name="PART_ContentPresenter"
                                      Content="{TemplateBinding Content}" />
                </Panel>
            </Grid>
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:SplitView Theme="{StaticResource MyCustomSplitView}">
    <!-- ... -->
</atom:SplitView>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的过渡动画、分隔线、遮罩层等功能。建议优先使用 Style 覆盖。

---

## 4. 控制动画行为

```xml
<!-- 禁用过渡动画（面板展开/收起瞬间完成） -->
<atom:SplitView IsMotionEnabled="False" />

<!-- 自定义展开/收起时长 -->
<atom:SplitView PaneOpenMotionDuration="0:0:0.5"
                PaneCloseMotionDuration="0:0:0.2" />
```

---

## 5. 面板展开/收起的数据绑定

SplitView 的 `IsPaneOpen` 属性支持双向数据绑定，方便在 MVVM 模式中使用：

```xml
<atom:SplitView IsPaneOpen="{Binding IsSidebarOpen}"
                DisplayMode="CompactInline">
    <atom:SplitView.Pane>
        <!-- 导航菜单 -->
    </atom:SplitView.Pane>
    <!-- 主内容 -->
</atom:SplitView>
```

```csharp
// ViewModel
public class MainViewModel : ReactiveObject
{
    private bool _isSidebarOpen = true;
    public bool IsSidebarOpen
    {
        get => _isSidebarOpen;
        set => this.RaiseAndSetIfChanged(ref _isSidebarOpen, value);
    }
}
```

---

## 6. 响应面板关闭事件

```xml
<atom:SplitView PaneClosing="OnPaneClosing"
                PaneClosed="OnPaneClosed"
                DisplayMode="Overlay">
    <!-- ... -->
</atom:SplitView>
```

```csharp
private void OnPaneClosing(object? sender, CancelRoutedEventArgs e)
{
    // 可以在此取消关闭
    if (HasUnsavedChanges)
    {
        e.Cancel = true;
    }
}

private void OnPaneClosed(object? sender, RoutedEventArgs e)
{
    // 面板已关闭
    Debug.WriteLine("Pane closed.");
}
```

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|SplitView` 语法引用 `atom` XML 命名空间下的 `SplitView` 类型，其中 `|` 是命名空间分隔符。

### 按面板状态选择

| 选择器 | 说明 |
|---|---|
| `atom\|SplitView:open` | 匹配面板展开状态的 SplitView |
| `atom\|SplitView:closed` | 匹配面板收起状态的 SplitView |

### 按面板位置选择

| 选择器 | 说明 |
|---|---|
| `atom\|SplitView:left` | 匹配面板在左侧的 SplitView |
| `atom\|SplitView:right` | 匹配面板在右侧的 SplitView |
| `atom\|SplitView:top` | 匹配面板在顶部的 SplitView |
| `atom\|SplitView:bottom` | 匹配面板在底部的 SplitView |

### 按显示模式选择

| 选择器 | 说明 |
|---|---|
| `atom\|SplitView:overlay` | 匹配 Overlay 显示模式 |
| `atom\|SplitView:inline` | 匹配 Inline 显示模式 |
| `atom\|SplitView:compactinline` | 匹配 CompactInline 显示模式 |
| `atom\|SplitView:compactoverlay` | 匹配 CompactOverlay 显示模式 |

### 模板部件选择器

| 选择器 | 说明 |
|---|---|
| `atom\|SplitView /template/ Panel#PART_PaneRoot` | 面板容器 |
| `atom\|SplitView /template/ ContentPresenter#PART_PanePresenter` | 面板内容展示器 |
| `atom\|SplitView /template/ ContentPresenter#PART_ContentPresenter` | 主内容展示器 |
| `atom\|SplitView /template/ Rectangle#HCPaneBorder` | 面板分隔线 |
| `atom\|SplitView /template/ Rectangle#LightDismissLayer` | 轻量关闭遮罩层 |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|SplitView:left:open` | 左侧面板展开时 |
| `atom\|SplitView:left:closed` | 左侧面板收起时 |
| `atom\|SplitView:overlay:open /template/ Rectangle#LightDismissLayer` | Overlay 模式展开时的遮罩层 |
| `atom\|SplitView:compactinline /template/ Panel#PART_PaneRoot` | CompactInline 模式下的面板容器 |
