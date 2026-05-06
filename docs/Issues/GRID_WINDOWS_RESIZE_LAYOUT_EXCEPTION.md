# Grid 在 Windows 窗口 resize 时偶发布局异常分析

## 现象

- `Grid`（`Row`/`Col`）在 Windows 上随着窗口持续 resize，偶发出现布局异常。
- 该问题更容易在窗口宽度接近响应式断点时复现，例如 `576 / 768 / 992 / 1200 / 1600` 附近。
- 从代码路径看，更可能出现的是 Avalonia 布局循环类问题，例如 `Layout cycle detected` 一类告警/异常，而不是单纯的宽高计算越界。

## 结论

当前更可能的根因不是 `Row`/`Col` 的几何计算本身，而是：

**窗口内容区宽度变化触发 `ContainerQuery` 断点切换，断点切换又同步触发 `Row.InvalidateMeasure()`，从而形成布局反馈环。**

简化后的链路如下：

1. Windows 窗口 resize，内容区宽度变化。
2. `WindowTheme.axaml` 中的 `ContainerQuery` 命中状态变化。
3. `WindowMediaQueryIndicator.MediaBreakPoint` 被样式改写。
4. `Window.NotifyMediaBreakPointChanged()` 同步更新 `Window.MediaBreakPoint` 并触发 `MediaBreakPointChanged`。
5. 每个 `Row` 监听到断点变化后立即执行 `_orderedChildrenDirty = true; InvalidateMeasure();`
6. `Row` 重新测量后，布局结果又可能改变内容区宽度或命中状态。
7. 在断点边界附近，上述过程可能短时间内重复发生，最终触发 Avalonia 布局循环保护。

## 关键代码位置

### 1. Window 的断点查询定义

文件：`src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml`

```xml
<ContainerQuery Name="{x:Static atom:IMediaBreakAwareControl.GlobalQueryContainerName}" Query="max-width:575">
    <ContainerQuery.Children>
        <Style Selector="atom|WindowMediaQueryIndicator#PART_MediaQueryIndicator">
            <Setter Property="MediaBreakPoint" Value="ExtraSmall" />
        </Style>
    </ContainerQuery.Children>
</ContainerQuery>

<ContainerQuery Name="{x:Static atom:IMediaBreakAwareControl.GlobalQueryContainerName}" Query="min-width:576">
    ...
    <Setter Property="MediaBreakPoint" Value="Small" />
</ContainerQuery>

<ContainerQuery Name="{x:Static atom:IMediaBreakAwareControl.GlobalQueryContainerName}" Query="min-width:768">
    ...
    <Setter Property="MediaBreakPoint" Value="Medium" />
</ContainerQuery>
```

这里的查询是**重叠的**，不是互斥区间。  
例如宽度为 `1000` 时，`min-width:576`、`min-width:768`、`min-width:992` 会同时命中。

### 2. WindowMediaQueryIndicator 同步通知 Window

文件：`src/AtomUI.Desktop.Controls/Window/WindowMediaQueryIndicator.cs`

```csharp
protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
{
    base.OnPropertyChanged(change);
    if (change.Property == MediaBreakPointProperty)
    {
        OwnerWindow?.NotifyMediaBreakPointChanged(MediaBreakPoint);
    }
}
```

### 3. Window 同步广播断点变化

文件：`src/AtomUI.Desktop.Controls/Window/Window.cs`

```csharp
internal void NotifyMediaBreakPointChanged(MediaBreakPoint breakPoint)
{
    SetCurrentValue(MediaBreakPointProperty, breakPoint);
    MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(breakPoint));
}
```

这里没有判断**新旧断点是否相同**，即使最终断点未变，也可能重复广播。

### 4. Row 收到断点变化后立即重新测量

文件：`src/AtomUI.Controls/Grid/Row.cs`

```csharp
private void HandleMediaBreakChanged(object? sender, MediaBreakPointChangedEventArgs args)
{
    _breakPoint = args.MediaBreakPoint;
    _orderedChildrenDirty = true;
    InvalidateMeasure();
}
```

这会让所有依赖响应式列配置的 `Row` 在 resize 过程中被高频重新入队。

## 为什么 Windows 上更容易暴露

1. Windows 拖拽窗口边框时，宽度变化频率高。
2. `Grid` 的响应式能力直接依赖断点切换，窗口宽度一旦跨过边界就会重新布局。
3. `Row` 的布局结果又会影响内容区的实际测量宽度，可能导致断点再次反向变化。
4. Avalonia 布局队列对单个元素单轮最多只允许有限次重复入队；超过阈值后会报告布局循环问题。

参考 Avalonia 源码：

- `Avalonia.Base/Layout/LayoutManager.cs`：`MaxPasses = 10`
- `Avalonia.Base/Layout/LayoutQueue.cs`：超出入队阈值时记录 `Layout cycle detected`

## 当前判断

`Grid` 不是唯一问题点，但它是最容易被放大的消费方，因为：

- `Row` 直接订阅了 `Window.MediaBreakPointChanged`
- `Row` 的响应式列、偏移、顺序、gutter 都会影响测量结果
- 断点切换越频繁，`Row` 被重复测量的概率越高

因此这更像是：

**Window 断点系统与 Grid 响应式布局之间的耦合，在 resize 边界处产生了反馈循环。**

## 后续修复建议

建议按下面顺序修复：

1. **给 `Window.NotifyMediaBreakPointChanged` 加去重**
   - 若 `breakPoint == MediaBreakPoint`，直接返回，不再广播事件。

2. **把 `ContainerQuery` 改成互斥区间**
   - 避免多条 `min-width` 查询同时命中同一个宽度。
   - 例如改成：
     - `max-width:575`
     - `min-width:576 and max-width:767`
     - `min-width:768 and max-width:991`
     - `min-width:992 and max-width:1199`
     - `min-width:1200 and max-width:1599`
     - `min-width:1600`

3. **必要时延迟断点广播**
   - 不在样式切换当下同步触发 `InvalidateMeasure`，而是在当前布局帧结束后统一发布。

4. **如果问题仍存在，再评估 `Row` 端节流**
   - 例如仅当 `_breakPoint` 实际变化时才标记 dirty 并重新测量。

## 备注

本文件仅记录当前分析结论，暂未实施修复。
