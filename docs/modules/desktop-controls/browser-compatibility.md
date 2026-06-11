# Desktop Controls 浏览器兼容策略

AtomUI 的 Browser Gallery 使用 `Avalonia.Browser` 运行。浏览器环境没有 Native Window，因此桌面控件包需要显式降级。

## 平台判断

核心判断来自：

```csharp
RuntimePlatform.Features.SupportsNativeWindow
```

当该值为 false 时：

- 使用 `BrowserCommonControlThemesProvider`。
- 使用 `BrowserDesktopControlThemesProvider`。
- `UseDesktopControls()` 注册浏览器安全 Token 子集。
- 跳过桌面 Tooltip 服务和媒体断点窗口引导中依赖 Native Window 的部分。

## 兼容原则

- 控件示例可以在浏览器展示，但不能默认具备桌面窗口能力。
- 与 Native Window、Window Host、系统级 Overlay 强相关的控件需要单独判断。
- 新增桌面控件时，需要确认它是否能进入浏览器 Token 子集。
- 如果控件需要浏览器专用主题，应通过 Browser Provider 聚合，而不是在桌面主题里混入平台判断。

## 相关文档

- [../../gallery/browser-gallery-porting-notes.md](../../gallery/browser-gallery-porting-notes.md)
- [../../architecture/runtime-platforms.md](../../architecture/runtime-platforms.md)
