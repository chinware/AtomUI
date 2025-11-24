using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using AtomUI.Icons.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls;

using IconControl = AtomUI.Controls.Icon;
using SvgControl = Avalonia.Svg.Svg;

public enum ResultStatus
{
    Info,
    Success,
    Error,
    Warning,
    ErrorCode404,
    ErrorCode403,
    ErrorCode500
}

public class Result : ContentControl, IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> ExtraProperty =
        AvaloniaProperty.Register<Result, object?>(nameof(Extra));

    public static readonly StyledProperty<IDataTemplate?> ExtraTemplateProperty =
        AvaloniaProperty.Register<Result, IDataTemplate?>(nameof(ExtraTemplate));
    
    public static readonly StyledProperty<PathIcon?> IconProperty =
        AvaloniaProperty.Register<Result, PathIcon?>(nameof(Icon));
    
    public static readonly StyledProperty<ResultStatus> StatusProperty =
        AvaloniaProperty.Register<Result, ResultStatus>(nameof(Status));
    
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<Result, object?>(nameof(Header));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<Result, IDataTemplate?>(nameof(HeaderTemplate));
    
    public static readonly StyledProperty<object?> SubHeaderProperty =
        AvaloniaProperty.Register<Result, object?>(nameof(SubHeader));

    public static readonly StyledProperty<IDataTemplate?> SubHeaderTemplateProperty =
        AvaloniaProperty.Register<Result, IDataTemplate?>(nameof(SubHeaderTemplate));
    
    public static readonly StyledProperty<double> HeaderFontSizeProperty =
        AvaloniaProperty.Register<Result, double>(nameof(HeaderFontSize));
    
    public static readonly StyledProperty<double> SubHeaderFontSizeProperty =
        AvaloniaProperty.Register<Result, double>(nameof(SubHeaderFontSize));
    
    [DependsOn(nameof(ExtraTemplate))]
    public object? Extra
    {
        get => GetValue(ExtraProperty);
        set => SetValue(ExtraProperty, value);
    }
    
    public IDataTemplate? ExtraTemplate
    {
        get => GetValue(ExtraTemplateProperty);
        set => SetValue(ExtraTemplateProperty, value);
    }
    
    public PathIcon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public ResultStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    [DependsOn(nameof(HeaderTemplate))]
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
    
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
    
    [DependsOn(nameof(SubHeaderTemplate))]
    public object? SubHeader
    {
        get => GetValue(SubHeaderProperty);
        set => SetValue(SubHeaderProperty, value);
    }
    
    public IDataTemplate? SubHeaderTemplate
    {
        get => GetValue(SubHeaderTemplateProperty);
        set => SetValue(SubHeaderTemplateProperty, value);
    }
    
    public double HeaderFontSize
    {
        get => GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }
    
    public double SubHeaderFontSize
    {
        get => GetValue(SubHeaderFontSizeProperty);
        set => SetValue(SubHeaderFontSizeProperty, value);
    }
    
    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<double> RelativeHeaderLineHeightProperty =
        AvaloniaProperty.Register<Result, double>(nameof(RelativeHeaderLineHeight));
    
    internal static readonly StyledProperty<double> RelativeSubHeaderLineHeightProperty =
        AvaloniaProperty.Register<Result, double>(nameof(RelativeSubHeaderLineHeight));
    
    internal static readonly DirectProperty<Result, double> HeaderLineHeightProperty =
        AvaloniaProperty.RegisterDirect<Result, double>(
            nameof(HeaderLineHeight),
            o => o.HeaderLineHeight,
            (o, v) => o.HeaderLineHeight = v);
    
    internal static readonly DirectProperty<Result, double> SubHeaderLineHeightProperty =
        AvaloniaProperty.RegisterDirect<Result, double>(
            nameof(SubHeaderLineHeight),
            o => o.SubHeaderLineHeight,
            (o, v) => o.SubHeaderLineHeight = v);
    
    internal static readonly DirectProperty<Result, PathIcon?> StatusIconProperty =
        AvaloniaProperty.RegisterDirect<Result, PathIcon?>(
            nameof(StatusIcon),
            o => o.StatusIcon,
            (o, v) => o.StatusIcon = v);
    
    internal double RelativeHeaderLineHeight
    {
        get => GetValue(RelativeHeaderLineHeightProperty);
        set => SetValue(RelativeHeaderLineHeightProperty, value);
    }
    
    internal double RelativeSubHeaderLineHeight
    {
        get => GetValue(RelativeSubHeaderLineHeightProperty);
        set => SetValue(RelativeSubHeaderLineHeightProperty, value);
    }

    private double _headerLineHeight;

    internal double HeaderLineHeight
    {
        get => _headerLineHeight;
        set => SetAndRaise(HeaderLineHeightProperty, ref _headerLineHeight, value);
    }

    private double _subHeaderLineHeight;

    internal double SubHeaderLineHeight
    {
        get => _subHeaderLineHeight;
        set => SetAndRaise(SubHeaderLineHeightProperty, ref _subHeaderLineHeight, value);
    }
    
    private PathIcon? _statusIcon;

    internal PathIcon? StatusIcon
    {
        get => _statusIcon;
        set => SetAndRaise(StatusIconProperty, ref _statusIcon, value);
    }
    
    string IControlSharedTokenResourcesHost.TokenId => ResultToken.ID;
    Control IControlSharedTokenResourcesHost.HostControl => this;

    #endregion

    private SvgControl? _statusImage;

    static Result()
    {
        AffectsMeasure<Empty>(StatusProperty);
    }
    
    public Result()
    {
        this.RegisterResources();
        ConfigureInstanceStyle();
    }

    private void ConfigureInstanceStyle()
    {
        {
            var iconStyle = new Style(x => x.Is<Result>().Descendant().Name(ResultThemeConstants.StatusIconPresenterPart).Child());
            iconStyle.Add(WidthProperty, ResultTokenKey.IconSize);
            iconStyle.Add(HeightProperty, ResultTokenKey.IconSize);
            Styles.Add(iconStyle);
        }
        
        var infoStyle = new Style(x => x.PropertyEquals(StatusProperty, ResultStatus.Info));
        
        {
            var iconStyle = new Style(x => x.Nesting().Descendant().Name(ResultThemeConstants.StatusIconPresenterPart).Child());
            iconStyle.Add(IconControl.NormalFilledBrushProperty, ResultTokenKey.ResultInfoIconColor);
            iconStyle.Add(ForegroundProperty, ResultTokenKey.ResultInfoIconColor);
            infoStyle.Add(iconStyle);
        }
        
        Styles.Add(infoStyle);
        
        var successStyle = new Style(x => x.PropertyEquals(StatusProperty, ResultStatus.Success));
        
        {
            var iconStyle = new Style(x => x.Nesting().Descendant().Name(ResultThemeConstants.StatusIconPresenterPart).Child());
            iconStyle.Add(IconControl.NormalFilledBrushProperty, ResultTokenKey.ResultSuccessIconColor);
            iconStyle.Add(ForegroundProperty, ResultTokenKey.ResultSuccessIconColor);
            successStyle.Add(iconStyle);
        }
        
        Styles.Add(successStyle);
        
        var warningStyle = new Style(x => x.PropertyEquals(StatusProperty, ResultStatus.Warning));
        
        {
            var iconStyle = new Style(x => x.Nesting().Descendant().Name(ResultThemeConstants.StatusIconPresenterPart).Child());
            iconStyle.Add(IconControl.NormalFilledBrushProperty, ResultTokenKey.ResultWarningIconColor);
            iconStyle.Add(ForegroundProperty, ResultTokenKey.ResultWarningIconColor);
            warningStyle.Add(iconStyle);
        }
        
        Styles.Add(warningStyle);
        
        var errorStyle = new Style(x => x.PropertyEquals(StatusProperty, ResultStatus.Error));
        
        {
            var iconStyle = new Style(x => x.Nesting().Descendant().Name(ResultThemeConstants.StatusIconPresenterPart).Child());
            iconStyle.Add(IconControl.NormalFilledBrushProperty, ResultTokenKey.ResultErrorIconColor);
            iconStyle.Add(ForegroundProperty, ResultTokenKey.ResultErrorIconColor);
            errorStyle.Add(iconStyle);
        }
        
        Styles.Add(errorStyle);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == StatusProperty ||
                change.Property == IconProperty)
            {
                ConfigureStatusImage();
            }
        }

        if (change.Property == FontSizeProperty ||
            change.Property == RelativeHeaderLineHeightProperty)
        {
            ConfigureHeaderLineHeight();
        }
        
        if (change.Property == FontSizeProperty ||
            change.Property == RelativeSubHeaderLineHeightProperty)
        {
            ConfigureSubHeaderLineHeight();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _statusImage = e.NameScope.Find<SvgControl>(ResultThemeConstants.ErrorCodeImagePart);
        ConfigureStatusImage();
        ConfigureHeaderLineHeight();
        ConfigureSubHeaderLineHeight();
    }

    private void ConfigureStatusImage()
    {
        if (Status == ResultStatus.Info || 
            Status == ResultStatus.Success ||
            Status == ResultStatus.Warning ||
            Status == ResultStatus.Error)
        {
            if (Icon != null)
            {
                SetCurrentValue(StatusIconProperty, Icon);
            }
            else
            {
                if (Status == ResultStatus.Info)
                {
                    SetCurrentValue(StatusIconProperty, new ExclamationCircleFilled());
                }
                else if (Status == ResultStatus.Success)
                {
                    SetCurrentValue(StatusIconProperty, new CheckCircleFilled());
                }
                else if (Status == ResultStatus.Warning)
                {
                    SetCurrentValue(StatusIconProperty, new WarningFilled());
                }
                else if (Status == ResultStatus.Error)
                {
                    SetCurrentValue(StatusIconProperty, new CloseCircleFilled());
                }
            }
        }

        if (_statusImage != null)
        {
            if (Status == ResultStatus.ErrorCode403)
            {
                _statusImage.Source = ResultIndicator.UnauthorizedImageSource();
            }
            else if (Status == ResultStatus.ErrorCode404)
            {
                _statusImage.Source = ResultIndicator.NotFoundImageSource();
            }
            else if (Status == ResultStatus.ErrorCode500)
            {
                _statusImage.Source = ResultIndicator.ServerErrorImageSource();
            }
        }
    }

    private void ConfigureHeaderLineHeight()
    {
        SetCurrentValue(HeaderLineHeightProperty, RelativeHeaderLineHeight * HeaderFontSize);
    }

    private void ConfigureSubHeaderLineHeight()
    {
        SetCurrentValue(SubHeaderLineHeightProperty, RelativeSubHeaderLineHeight * SubHeaderFontSize);
    }
}