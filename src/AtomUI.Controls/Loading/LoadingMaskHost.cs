using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class LoadingMaskHost : Control, IControlCustomStyle
{
    private IControlCustomStyle _customStyle;
    private bool _initialized;

    private LoadingMask? _loadingMask;

    public LoadingMaskHost()
    {
        _customStyle = this;
    }

    public void ShowLoading()
    {
        if (_loadingMask is not null && _loadingMask.IsLoading) return;
        if (_loadingMask is null && MaskTarget is not null)
        {
            _loadingMask = new LoadingMask();
            _loadingMask.Attach(this);
            BindUtils.RelayBind(this, SizeTypeProperty, _loadingMask, SizeTypeProperty);
            BindUtils.RelayBind(this, LoadingMsgProperty, _loadingMask, LoadingMsgProperty);
            BindUtils.RelayBind(this, IsShowLoadingMsgProperty, _loadingMask, IsShowLoadingMsgProperty);
            BindUtils.RelayBind(this, CustomIndicatorIconProperty, _loadingMask, CustomIndicatorIconProperty);
            BindUtils.RelayBind(this, MotionDurationProperty, _loadingMask, MotionDurationProperty);
            BindUtils.RelayBind(this, MotionEasingCurveProperty, _loadingMask, MotionEasingCurveProperty);
        }

        _loadingMask!.Show();
    }

    public void HideLoading()
    {
        if (_loadingMask is null ||
            (_loadingMask is not null && !_loadingMask.IsLoading))
            return;
        _loadingMask?.Hide();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (IsLoading && MaskTarget is not null) ShowLoading();
    }

    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        if (!_initialized)
        {
            if (MaskTarget is not null)
            {
                ((ISetLogicalParent)MaskTarget).SetParent(this);
                VisualChildren.Add(MaskTarget);
            }

            _initialized = true;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        HideLoading();
        _loadingMask?.Dispose();
        _loadingMask = null;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (_initialized && VisualRoot is not null)
            if (IsLoadingProperty == change.Property)
            {
                if (IsLoading)
                    ShowLoading();
                else
                    HideLoading();
            }
    }



    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        LoadingIndicator.SizeTypeProperty.AddOwner<LoadingMaskHost>();

    public static readonly StyledProperty<string?> LoadingMsgProperty =
        LoadingIndicator.LoadingMsgProperty.AddOwner<LoadingMaskHost>();

    public static readonly StyledProperty<bool> IsShowLoadingMsgProperty =
        LoadingIndicator.IsShowLoadingMsgProperty.AddOwner<LoadingMaskHost>();

    public static readonly StyledProperty<PathIcon?> CustomIndicatorIconProperty =
        LoadingIndicator.CustomIndicatorIconProperty.AddOwner<LoadingMaskHost>();

    public static readonly StyledProperty<TimeSpan?> MotionDurationProperty =
        LoadingIndicator.MotionDurationProperty.AddOwner<LoadingMaskHost>();

    public static readonly StyledProperty<Easing?> MotionEasingCurveProperty =
        LoadingIndicator.MotionEasingCurveProperty.AddOwner<LoadingMaskHost>();

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<LoadingMaskHost, bool>(nameof(IsLoading));

    public static readonly StyledProperty<Control?> MaskTargetProperty =
        AvaloniaProperty.Register<CountBadge, Control?>(nameof(MaskTarget));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public string? LoadingMsg
    {
        get => GetValue(LoadingMsgProperty);
        set => SetValue(LoadingMsgProperty, value);
    }

    public bool IsShowLoadingMsg
    {
        get => GetValue(IsShowLoadingMsgProperty);
        set => SetValue(IsShowLoadingMsgProperty, value);
    }

    public PathIcon? CustomIndicatorIcon
    {
        get => GetValue(CustomIndicatorIconProperty);
        set => SetValue(CustomIndicatorIconProperty, value);
    }

    public TimeSpan? MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    public Easing? MotionEasingCurve
    {
        get => GetValue(MotionEasingCurveProperty);
        set => SetValue(MotionEasingCurveProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    [Content]
    public Control? MaskTarget
    {
        get => GetValue(MaskTargetProperty);
        set => SetValue(MaskTargetProperty, value);
    }

    #endregion
}