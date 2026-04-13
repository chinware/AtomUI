# MessageBox 自定义样式指南

MessageBox 基于 AtomUI 的 Dialog 系统构建，支持声明式（AXAML）和命令式（静态 API）两种使用方式。由于 MessageBox 内部使用 `MessageBoxDialogHost`（Window 模式）或 `MessageBoxOverlayDialogHost`（Overlay 模式）渲染，自定义样式需针对对应的宿主控件进行。

---

## 1. 通过属性直接控制

最常用的方式是通过 MessageBox 的公共属性控制外观和行为：

```xml
<!-- 不同样式的 MessageBox -->
<atom:MessageBox Title="确认删除？" Style="Confirm"
                 IsOpen="{Binding IsOpen, Mode=TwoWay}">
    <TextBlock>确定要删除这些项目吗？</TextBlock>
</atom:MessageBox>

<atom:MessageBox Title="操作成功" Style="Success"
                 IsOpen="{Binding IsSuccessOpen, Mode=TwoWay}">
    <TextBlock>数据已保存。</TextBlock>
</atom:MessageBox>

<!-- 使用 Window 宿主模式 -->
<atom:MessageBox Title="提示" Style="Information"
                 HostType="Window"
                 IsDragMovable="True"
                 IsOpen="{Binding IsOpen, Mode=TwoWay}">
    <TextBlock>这是一个原生窗口弹框。</TextBlock>
</atom:MessageBox>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml`（"MessageBox Style" 部分）

---

## 2. 通过静态 API 参数控制

使用静态方法时，通过 `MessageBoxOptions` 控制外观：

```csharp
var options = new MessageBoxOptions
{
    Title             = "Do you want to delete these items?",
    Style             = MessageBoxStyle.Confirm,
    IsDragMovable     = true,
    IsCenterOnStartup = true,
    HostType          = DialogHostType.Overlay
};
await MessageBox.ShowMessageModalAsync(content, null, options);
```

> 📖 参考：`controlgallery/AtomUIGallery/ShowCases/Views/Feedback/ModalShowCase.axaml.cs` 中 `HandleCreateConfirmMessageBox` 等方法

---

## 3. 通过 Style 覆盖宿主主题

MessageBox 的实际视觉渲染由内部的 `MessageBoxDialogHost`（Window 模式）或 `MessageBoxOverlayDialogHost`（Overlay 模式）控件完成。可以通过 Style 对这些内部控件进行样式覆盖：

### 自定义 Overlay 模式下的语义图标大小

```xml
<Window.Styles>
    <Style Selector="atom|MessageBoxOverlayDialogHost /template/ atom|IconPresenter#StyleIconPresenter">
        <Setter Property="Width" Value="32" />
        <Setter Property="Height" Value="32" />
    </Style>
</Window.Styles>
```

### 按消息样式自定义图标颜色

```xml
<Window.Styles>
    <!-- 自定义 Success 样式的图标颜色 -->
    <Style Selector="atom|MessageBoxOverlayDialogHost[Style=Success] /template/ atom|IconPresenter#StyleIconPresenter">
        <Setter Property="IconBrush" Value="#73d13d" />
    </Style>
</Window.Styles>
```

---

## 4. 控制动画行为

```xml
<!-- 禁用弹框开关动画 -->
<atom:MessageBox IsMotionEnabled="False"
                 Title="无动画" Style="Information"
                 IsOpen="{Binding IsOpen, Mode=TwoWay}">
    <TextBlock>此弹框无动画效果。</TextBlock>
</atom:MessageBox>
```

---

## 5. 控制按钮样式

```xml
<!-- 确认按钮使用 Default 样式（非 Primary） -->
<atom:MessageBox Title="提示" Style="Information"
                 OkButtonStyle="Default"
                 IsOpen="{Binding IsOpen, Mode=TwoWay}">
    <TextBlock>确认按钮将使用默认样式。</TextBlock>
</atom:MessageBox>
```

---

## 样式选择器速查

> 说明：由于 MessageBox 的实际渲染由内部宿主控件完成，样式选择器需要针对 `MessageBoxDialogHost` 或 `MessageBoxOverlayDialogHost`。

### 按控件类型选择

| 选择器 | 说明 |
|---|---|
| `atom\|MessageBox` | 匹配 MessageBox 外层控件（通常无需直接样式化） |
| `atom\|MessageBoxDialogHost` | 匹配 Window 宿主模式下的弹窗渲染 |
| `atom\|MessageBoxOverlayDialogHost` | 匹配 Overlay 宿主模式下的弹窗渲染 |

### 按消息样式选择

| 选择器 | 说明 |
|---|---|
| `atom\|MessageBoxDialogHost[Style=Confirm]` | Window 模式确认样式 |
| `atom\|MessageBoxDialogHost[Style=Information]` | Window 模式信息样式 |
| `atom\|MessageBoxDialogHost[Style=Success]` | Window 模式成功样式 |
| `atom\|MessageBoxDialogHost[Style=Warning]` | Window 模式警告样式 |
| `atom\|MessageBoxDialogHost[Style=Error]` | Window 模式错误样式 |
| `atom\|MessageBoxOverlayDialogHost[Style=Confirm]` | Overlay 模式确认样式 |
| `atom\|MessageBoxOverlayDialogHost[Style=Information]` | Overlay 模式信息样式 |
| `atom\|MessageBoxOverlayDialogHost[Style=Success]` | Overlay 模式成功样式 |
| `atom\|MessageBoxOverlayDialogHost[Style=Warning]` | Overlay 模式警告样式 |
| `atom\|MessageBoxOverlayDialogHost[Style=Error]` | Overlay 模式错误样式 |

### 模板部件选择（宿主控件内部）

| 选择器 | 说明 |
|---|---|
| `atom\|MessageBoxDialogHost /template/ atom\|IconPresenter#StyleIconPresenter` | Window 模式下的语义图标 |
| `atom\|MessageBoxOverlayDialogHost /template/ atom\|IconPresenter#StyleIconPresenter` | Overlay 模式下的语义图标 |
| `atom\|MessageBoxDialogHost /template/ Border#ContentFrame` | Window 模式下的内容区框架 |
| `atom\|MessageBoxOverlayDialogHost /template/ Border#ContentFrame` | Overlay 模式下的内容区框架 |
| `atom\|MessageBoxDialogHost /template/ Border#FooterFrame` | Window 模式下的底部按钮区框架 |
| `atom\|MessageBoxOverlayDialogHost /template/ Border#FooterFrame` | Overlay 模式下的底部按钮区框架 |
