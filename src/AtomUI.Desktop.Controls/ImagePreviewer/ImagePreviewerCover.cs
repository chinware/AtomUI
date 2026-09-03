using AtomUI.Animations;
using Avalonia.Threading;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class ImagePreviewerCover : ContentControl, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<IImage?> ImageSourceProperty =
        AvaloniaProperty.Register<ImagePreviewerCover, IImage?>(nameof(ImageSource));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<ImagePreviewerCover>();
    
    public static readonly StyledProperty<bool> IsShowCoverMaskProperty =
        ImagePreviewer.IsShowCoverMaskProperty.AddOwner<ImagePreviewerCover>();

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<ImagePreviewerCover, bool>(nameof(IsLoading));

    public static readonly StyledProperty<bool> IsFailedProperty =
        AvaloniaProperty.Register<ImagePreviewerCover, bool>(nameof(IsFailed));

    internal static readonly DirectProperty<ImagePreviewerCover, bool> HasErrorProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerCover, bool>(
            nameof(HasError),
            control => control.HasError);

    public static readonly StyledProperty<object?> LoadingContentProperty =
        AvaloniaProperty.Register<ImagePreviewerCover, object?>(nameof(LoadingContent));

    public static readonly StyledProperty<IDataTemplate?> LoadingContentTemplateProperty =
        AvaloniaProperty.Register<ImagePreviewerCover, IDataTemplate?>(nameof(LoadingContentTemplate));

    public static readonly StyledProperty<object?> ErrorContentProperty =
        AvaloniaProperty.Register<ImagePreviewerCover, object?>(nameof(ErrorContent));

    public static readonly StyledProperty<IDataTemplate?> ErrorContentTemplateProperty =
        AvaloniaProperty.Register<ImagePreviewerCover, IDataTemplate?>(nameof(ErrorContentTemplate));
    
    public IImage? ImageSource
    {
        get => GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsShowCoverMask
    {
        get => GetValue(IsShowCoverMaskProperty);
        set => SetValue(IsShowCoverMaskProperty, value);
    }

    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public bool IsFailed
    {
        get => GetValue(IsFailedProperty);
        set => SetValue(IsFailedProperty, value);
    }

    public object? LoadingContent
    {
        get => GetValue(LoadingContentProperty);
        set => SetValue(LoadingContentProperty, value);
    }

    public IDataTemplate? LoadingContentTemplate
    {
        get => GetValue(LoadingContentTemplateProperty);
        set => SetValue(LoadingContentTemplateProperty, value);
    }

    public object? ErrorContent
    {
        get => GetValue(ErrorContentProperty);
        set => SetValue(ErrorContentProperty, value);
    }

    public IDataTemplate? ErrorContentTemplate
    {
        get => GetValue(ErrorContentTemplateProperty);
        set => SetValue(ErrorContentTemplateProperty, value);
    }
    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> MaskOpacityProperty =
        AvaloniaProperty.Register<ImagePreviewerCover, double>(nameof(MaskOpacity), 0.0);

    internal static readonly DirectProperty<ImagePreviewerCover, bool> IsCoverMaskVisibleProperty =
        AvaloniaProperty.RegisterDirect<ImagePreviewerCover, bool>(
            nameof(IsCoverMaskVisible),
            o => o.IsCoverMaskVisible,
            (o, v) => o.IsCoverMaskVisible = v);
    
    internal double MaskOpacity
    {
        get => GetValue(MaskOpacityProperty);
        set => SetValue(MaskOpacityProperty, value);
    }

    private bool _isCoverMaskVisible = true;
    private bool _hasError;

    internal bool HasError => _hasError;

    internal bool IsCoverMaskVisible
    {
        get => _isCoverMaskVisible;
        set => SetAndRaise(IsCoverMaskVisibleProperty, ref _isCoverMaskVisible, value);
    }

    #endregion

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ImageSourceProperty ||
            change.Property == IsShowCoverMaskProperty ||
            change.Property == IsLoadingProperty ||
            change.Property == IsFailedProperty ||
            change.Property == LoadingContentProperty ||
            change.Property == LoadingContentTemplateProperty)
        {
            UpdateVisualState();
        }
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        UpdateVisualState();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.Post(this.EnableTransitions);
    }

    private void UpdateVisualState()
    {
        PseudoClasses.Set(":has-image", ImageSource is not null);
        PseudoClasses.Set(":loading", IsLoading);
        PseudoClasses.Set(":failed", IsFailed);
        PseudoClasses.Set(":loading-skeleton", IsLoading && ImageSource is null && LoadingContent is null);
        SetAndRaise(HasErrorProperty, ref _hasError, IsFailed && ImageSource is null);
        // mask 是悬停操作层（点击打开预览在任何加载态都有效），只由 IsShowCoverMask
        // 决定，与加载/图片/失败状态解耦：透明度完全交给主题（基础 0，悬停 1），
        // 避免 C# 强制值与悬停样式/过渡互相打断造成闪烁
        SetCurrentValue(IsCoverMaskVisibleProperty, IsShowCoverMask);
    }
}
