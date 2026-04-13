# ImagePreviewer 使用文档

本文档介绍 AtomUI ImagePreviewer 控件的使用方式。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/ImagePreviewerShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 ImagePreviewer，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // ImagePreviewer, ImageGroupPreviewer
```

---

## 1. 基本使用 — 单图预览

最基本的用法是展示一张图片，点击后弹出预览对话框（来自 Gallery 示例 `ImagePreviewerShowCase.axaml`）：

```xml
<atom:ImagePreviewer Name="BasicPreviewer" Width="200" />
```

```csharp
// Code-behind (ReactiveUI)
this.WhenActivated(disposables =>
{
    viewModel.DefaultImages = [
        "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png"
    ];
    this.OneWayBind(viewModel, vm => vm.DefaultImages, v => v.BasicPreviewer.ItemsSource)
        .DisposeWith(disposables);
});
```

---

## 2. 容错图片

当 `ItemsSource` 为空或加载失败时，使用 `FallbackImageSrc` 显示占位图：

```xml
<atom:ImagePreviewer Name="FaultTolerantPreviewer" Width="200" />
```

```csharp
viewModel.FallbackImage = "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Fallback.png";
this.OneWayBind(viewModel, vm => vm.FallbackImage, v => v.FaultTolerantPreviewer.FallbackImageSrc)
    .DisposeWith(disposables);
```

---

## 3. 从一张封面预览多张图片

`ItemsSource` 包含多张图片路径，但 `ImagePreviewer` 仅展示首张作为封面，点击后可左右切换浏览所有图片：

```xml
<atom:ImagePreviewer Name="FromOnImagePreviewer" Width="200" />
```

```csharp
viewModel.ThreeImages = [
    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/4.webp",
    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/5.webp",
    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/6.webp"
];
this.OneWayBind(viewModel, vm => vm.ThreeImages, v => v.FromOnImagePreviewer.ItemsSource)
    .DisposeWith(disposables);
```

---

## 4. 自定义封面图

通过 `CoverImageSrc` 指定与预览图不同的封面（如模糊缩略图）：

```xml
<atom:ImagePreviewer Name="CustomImagePreviewer" Width="200" />
```

```csharp
viewModel.DefaultImages = ["avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png"];
viewModel.BlurImage = "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Blur.png";
this.OneWayBind(viewModel, vm => vm.DefaultImages, v => v.CustomImagePreviewer.ItemsSource)
    .DisposeWith(disposables);
this.OneWayBind(viewModel, vm => vm.BlurImage, v => v.CustomImagePreviewer.CoverImageSrc)
    .DisposeWith(disposables);
```

---

## 5. 多图组预览

`ImageGroupPreviewer` 展示多张缩略图，点击任意一张直接跳转到该图片的预览：

```xml
<atom:ImageGroupPreviewer Name="MultiImagesPreviewer"
                          CoverWidth="200" CoverHeight="200" />
```

```csharp
viewModel.TwoImages = [
    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/2.svg",
    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/3.svg"
];
this.OneWayBind(viewModel, vm => vm.TwoImages, v => v.MultiImagesPreviewer.ItemsSource)
    .DisposeWith(disposables);
```

> 💡 注意 SVG 格式的图片（`.svg` 后缀）会自动走 SVG 渲染路径。

---

## 6. 隐藏封面遮罩

```xml
<atom:ImagePreviewer IsShowCoverMask="False" Width="200" />
```

隐藏遮罩后，封面仍可点击打开预览，但不显示悬浮提示。

---

## 7. 自定义遮罩指示器

```xml
<atom:ImagePreviewer Width="200">
    <atom:ImagePreviewer.CoverIndicatorContentTemplate>
        <DataTemplate>
            <TextBlock Text="查看大图"
                       HorizontalAlignment="Center"
                       VerticalAlignment="Center"
                       Foreground="White"
                       FontSize="14" />
        </DataTemplate>
    </atom:ImagePreviewer.CoverIndicatorContentTemplate>
</atom:ImagePreviewer>
```

---

## 8. 对话框定位

```xml
<!-- 对话框居中显示 -->
<atom:ImagePreviewer DialogHorizontalStartupLocation="Center"
                     DialogVerticalStartupLocation="Center" />

<!-- 对话框置顶 + 模态 -->
<atom:ImagePreviewer IsDialogModal="True"
                     IsDialogTopmost="True" />
```

---

## 9. 缩放配置

```xml
<!-- 更细粒度的缩放（每次 25%），最大 10 倍 -->
<atom:ImagePreviewer ImageScaleStep="0.25"
                     ImageMinScale="0.5"
                     ImageMaxScale="10.0" />
```

---

## 10. 禁用拖拽

```xml
<atom:ImagePreviewer IsImageMovable="False" />
```

---

## 11. 编程方式控制对话框

### 通过 IsOpen 属性

```csharp
// 打开
previewer.IsOpen = true;

// 关闭
previewer.IsOpen = false;
```

### 通过方法调用

```csharp
// 打开
previewer.OpenDialog();

// 监听关闭事件
previewer.DialogClosing += (sender, e) =>
{
    // 阻止关闭（如有未保存操作）
    if (hasUnsavedChanges)
    {
        e.Cancel = true;
    }
};

previewer.DialogClosed += (sender, e) =>
{
    // 对话框已关闭
};
```

---

## 12. C# 动态创建

```csharp
using AtomUI.Desktop.Controls;

var previewer = new ImagePreviewer
{
    Width = 200,
    ItemsSource = new List<string>
    {
        "avares://MyApp/Assets/photo1.png",
        "avares://MyApp/Assets/photo2.webp",
        "avares://MyApp/Assets/icon.svg"
    },
    ImageScaleStep = 0.3,
    IsDialogModal = false
};

parentPanel.Children.Add(previewer);
```

---

## 预览对话框交互速查

| 操作 | 方式 |
|---|---|
| 打开预览 | 点击封面图 / 设置 `IsOpen = true` / 调用 `OpenDialog()` |
| 关闭预览 | 点击窗口关闭按钮 / 设置 `IsOpen = false` |
| 缩放 | 工具栏按钮 / 鼠标滚轮 |
| 旋转 | 标题栏或浮动工具栏的旋转按钮 |
| 翻转 | 标题栏或浮动工具栏的翻转按钮 |
| 切换图片 | 导航按钮 / 键盘 `←` `→` / 工具栏前/后按钮 |
| 适应窗口/原始尺寸 | 工具栏切换按钮 |
| 拖拽移动 | 缩放后鼠标左键拖拽 |
