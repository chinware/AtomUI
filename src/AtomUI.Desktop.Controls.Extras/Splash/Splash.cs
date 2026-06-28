using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(SplashPseudoClass.Loading,
    SplashPseudoClass.Success,
    SplashPseudoClass.Error,
    SplashPseudoClass.Indeterminate,
    SplashPseudoClass.Determinate)]
public partial class Splash : ContentControl
{
    #region 公共属性定义

    public static readonly StyledProperty<object?> LogoProperty =
        AvaloniaProperty.Register<Splash, object?>(nameof(Logo));

    public static readonly StyledProperty<IDataTemplate?> LogoTemplateProperty =
        AvaloniaProperty.Register<Splash, IDataTemplate?>(nameof(LogoTemplate));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Splash, string?>(nameof(Title));

    public static readonly StyledProperty<string?> SubtitleProperty =
        AvaloniaProperty.Register<Splash, string?>(nameof(Subtitle));

    public static readonly StyledProperty<string?> MessageProperty =
        AvaloniaProperty.Register<Splash, string?>(nameof(Message));

    public static readonly StyledProperty<string?> DetailProperty =
        AvaloniaProperty.Register<Splash, string?>(nameof(Detail));

    public static readonly StyledProperty<double?> ProgressProperty =
        AvaloniaProperty.Register<Splash, double?>(
            nameof(Progress),
            coerce: static (_, value) => value is null ? null : Math.Clamp(value.Value, 0d, 1d));

    public static readonly StyledProperty<bool> IsIndeterminateProperty =
        AvaloniaProperty.Register<Splash, bool>(nameof(IsIndeterminate), true);

    public static readonly StyledProperty<SplashStatus> StatusProperty =
        AvaloniaProperty.Register<Splash, SplashStatus>(nameof(Status), SplashStatus.Loading);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        AvaloniaProperty.Register<Splash, bool>(nameof(IsMotionEnabled), true);

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<Splash, object?>(nameof(Footer));

    public static readonly StyledProperty<IDataTemplate?> FooterTemplateProperty =
        AvaloniaProperty.Register<Splash, IDataTemplate?>(nameof(FooterTemplate));

    [DependsOn(nameof(LogoTemplate))]
    public object? Logo
    {
        get => GetValue(LogoProperty);
        set => SetValue(LogoProperty, value);
    }

    public IDataTemplate? LogoTemplate
    {
        get => GetValue(LogoTemplateProperty);
        set => SetValue(LogoTemplateProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string? Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public string? Detail
    {
        get => GetValue(DetailProperty);
        set => SetValue(DetailProperty, value);
    }

    public double? Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public bool IsIndeterminate
    {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    public SplashStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    [DependsOn(nameof(FooterTemplate))]
    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public IDataTemplate? FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> ProgressValueProperty =
        AvaloniaProperty.Register<Splash, double>(nameof(ProgressValue));

    internal static readonly StyledProperty<bool> IsProgressBarVisibleProperty =
        AvaloniaProperty.Register<Splash, bool>(nameof(IsProgressBarVisible));

    internal static readonly StyledProperty<bool> IsSpinVisibleProperty =
        AvaloniaProperty.Register<Splash, bool>(nameof(IsSpinVisible), true);

    internal double ProgressValue
    {
        get => GetValue(ProgressValueProperty);
        set => SetValue(ProgressValueProperty, value);
    }

    internal bool IsProgressBarVisible
    {
        get => GetValue(IsProgressBarVisibleProperty);
        set => SetValue(IsProgressBarVisibleProperty, value);
    }

    internal bool IsSpinVisible
    {
        get => GetValue(IsSpinVisibleProperty);
        set => SetValue(IsSpinVisibleProperty, value);
    }

    #endregion

    static Splash()
    {
        AffectsMeasure<Splash>(
            LogoProperty,
            LogoTemplateProperty,
            TitleProperty,
            SubtitleProperty,
            MessageProperty,
            DetailProperty,
            ProgressProperty,
            IsIndeterminateProperty,
            StatusProperty,
            FooterProperty,
            FooterTemplateProperty);
    }

    public Splash()
    {
        this.RegisterTokenResourceScope(SplashToken.ScopeProvider);
        UpdateVisualState();
    }

    public void SetMessage(string? message, string? detail = null)
    {
        Message = message;
        Detail  = detail;
    }

    public void SetProgress(double? progress, string? message = null, string? detail = null)
    {
        Status          = SplashStatus.Loading;
        Progress        = progress;
        IsIndeterminate = !progress.HasValue;

        if (message is not null)
        {
            Message = message;
        }
        if (detail is not null)
        {
            Detail = detail;
        }
    }

    public void SetStatus(SplashStatus status, string? message = null, string? detail = null)
    {
        Status = status;
        if (message is not null)
        {
            Message = message;
        }
        if (detail is not null)
        {
            Detail = detail;
        }
    }

    public void SetError(string message, string? detail = null)
    {
        ArgumentNullException.ThrowIfNull(message);

        Status          = SplashStatus.Error;
        IsIndeterminate = false;
        Progress        = null;
        Message         = message;
        Detail          = detail;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StatusProperty ||
            change.Property == ProgressProperty ||
            change.Property == IsIndeterminateProperty)
        {
            UpdateVisualState();
        }
    }

    private void UpdateVisualState()
    {
        var hasDeterminateProgress = Progress.HasValue && !IsIndeterminate;

        ProgressValue        = Progress ?? 0d;
        IsProgressBarVisible = hasDeterminateProgress;
        IsSpinVisible        = IsIndeterminate;

        PseudoClasses.Set(SplashPseudoClass.Loading, Status == SplashStatus.Loading);
        PseudoClasses.Set(SplashPseudoClass.Success, Status == SplashStatus.Success);
        PseudoClasses.Set(SplashPseudoClass.Error, Status == SplashStatus.Error);
        PseudoClasses.Set(SplashPseudoClass.Indeterminate, IsIndeterminate);
        PseudoClasses.Set(SplashPseudoClass.Determinate, hasDeterminateProgress);
    }
}
