using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Thumb = AtomUI.Controls.Primitives.Thumb;

namespace AtomUI.Controls;

internal class GradientColorSlider : AbstractColorSlider
{
    #region 公共属性定义
    
    public static readonly StyledProperty<LinearGradientBrush?> GradientValueProperty =
        AvaloniaProperty.Register<GradientColorSlider, LinearGradientBrush?>(nameof(GradientValue));
    
    internal static readonly StyledProperty<HsvColor> ActivatedHsvValueProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, HsvColor>(
            nameof(ActivatedHsvValue),
            Colors.White.ToHsv(),
            defaultBindingMode: BindingMode.TwoWay);
    
    public LinearGradientBrush? GradientValue
    {
        get => GetValue(GradientValueProperty);
        set => SetValue(GradientValueProperty, value);
    }
    
    internal HsvColor ActivatedHsvValue
    {
        get => GetValue(ActivatedHsvValueProperty);
        set => SetValue(ActivatedHsvValueProperty, value);
    }
    
    #endregion
    
    #region 公共事件定义
    public event EventHandler<GradientColorChangedEventArgs>? GradientValueChanged;
    #endregion
    
    private IDisposable? _pressDispose;
    private IDisposable? _releaseDispose;
    private IDisposable? _activatedThumbDispose;

    static GradientColorSlider()
    {
        Thumb.DragStartedEvent.AddClassHandler<GradientColorSlider>((x, e) => x.NotifyThumbDragStarted(e), RoutingStrategies.Bubble);
        Thumb.DragCompletedEvent.AddClassHandler<GradientColorSlider>((x, e) => x.NotifyThumbDragCompleted(e),
            RoutingStrategies.Bubble);
    }

    protected virtual void NotifyGradientValueChanged(GradientColorChangedEventArgs e)
    {
        GradientValueChanged?.Invoke(this, e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _pressDispose?.Dispose();
        _releaseDispose?.Dispose();
        _activatedThumbDispose?.Dispose();
        Track = e.NameScope.Find<GradientColorPickerTrack>(ColorSliderThemeConstants.TrackPart);

        if (Track is GradientColorPickerTrack track)
        {
            track.IgnoreThumbDrag = true;
            _pressDispose = this.AddDisposableHandler(PointerPressedEvent, TrackPressed, RoutingStrategies.Tunnel);
            _releaseDispose = this.AddDisposableHandler(PointerReleasedEvent, TrackReleased, RoutingStrategies.Tunnel);
            _activatedThumbDispose = GradientColorPickerTrack.ActivatedThumbProperty.Changed.Subscribe(HandleActivatedThumbChanged);
            if (track.ActivatedThumb != null)
            {
                SyncValueFromActivatedThumb(track.ActivatedThumb);
            }
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _activatedThumbDispose = GradientColorPickerTrack.ActivatedThumbProperty.Changed.Subscribe(HandleActivatedThumbChanged);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _activatedThumbDispose?.Dispose();
    }

    private void HandleActivatedThumbChanged(AvaloniaPropertyChangedEventArgs args)
    {
        if (Track is GradientColorPickerTrack track && track.ActivatedThumb != null)
        {
            SyncValueFromActivatedThumb(track.ActivatedThumb);
        }
    }

    private void SyncValueFromActivatedThumb(GradientColorSliderThumb thumb)
    {
        SetCurrentValue(ActivatedHsvValueProperty, thumb.Color.ToHsv());
    }
    
    protected override void TrackPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            ColorSliderThumb? thumb = null;
            if (Track is GradientColorPickerTrack track)
            {
                thumb = track.NotifyTrackPressed(e);
            }

            if (thumb != null)
            {
                IsDragging = true;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == GradientValueProperty)
        {
            NotifyGradientValueChanged(new GradientColorChangedEventArgs(change.GetOldValue<LinearGradientBrush>(), change.GetNewValue<LinearGradientBrush>()));
        }
        else if (change.Property == ActivatedHsvValueProperty)
        {
            if (Track is GradientColorPickerTrack track)
            {
                track.NotifyActivateStopColorChanged(ActivatedHsvValue);
            }
        }
    }
    
    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        if (Track is GradientColorPickerTrack track)
        {
            if (track.Thumbs.Count > 0)
            {
                ThumbSize = track.Thumbs[0].DesiredSize.Width;
            }
        }
        return size;
    }
}