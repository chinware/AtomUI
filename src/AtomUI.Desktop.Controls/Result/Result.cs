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
    
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Result, string?>(nameof(Title));
    
    public static readonly StyledProperty<string?> SubTitleProperty =
        AvaloniaProperty.Register<Result, string?>(nameof(SubTitle));
    
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
    
    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public string? SubTitle
    {
        get => GetValue(SubTitleProperty);
        set => SetValue(SubTitleProperty, value);
    }
    
    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<double> RelativeTitleLineHeightProperty =
        AvaloniaProperty.Register<Result, double>(nameof(RelativeTitleLineHeight));
    
    internal static readonly StyledProperty<double> RelativeSubTitleLineHeightProperty =
        AvaloniaProperty.Register<Result, double>(nameof(RelativeSubTitleLineHeight));
    
    internal static readonly DirectProperty<Result, double> TitleLineHeightProperty =
        AvaloniaProperty.RegisterDirect<Result, double>(
            nameof(TitleLineHeight),
            o => o.TitleLineHeight,
            (o, v) => o.TitleLineHeight = v);
    
    internal static readonly DirectProperty<Result, double> SubTitleLineHeightProperty =
        AvaloniaProperty.RegisterDirect<Result, double>(
            nameof(SubTitleLineHeight),
            o => o.SubTitleLineHeight,
            (o, v) => o.SubTitleLineHeight = v);
    
    internal static readonly DirectProperty<Result, PathIcon?> StatusIconProperty =
        AvaloniaProperty.RegisterDirect<Result, PathIcon?>(
            nameof(StatusIcon),
            o => o.StatusIcon,
            (o, v) => o.StatusIcon = v);
    
    internal double RelativeTitleLineHeight
    {
        get => GetValue(RelativeTitleLineHeightProperty);
        set => SetValue(RelativeTitleLineHeightProperty, value);
    }
    
    internal double RelativeSubTitleLineHeight
    {
        get => GetValue(RelativeSubTitleLineHeightProperty);
        set => SetValue(RelativeSubTitleLineHeightProperty, value);
    }

    private double _titleLineHeight;

    internal double TitleLineHeight
    {
        get => _titleLineHeight;
        set => SetAndRaise(TitleLineHeightProperty, ref _titleLineHeight, value);
    }

    private double _subTitleLineHeight;

    internal double SubTitleLineHeight
    {
        get => _subTitleLineHeight;
        set => SetAndRaise(SubTitleLineHeightProperty, ref _subTitleLineHeight, value);
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
            change.Property == RelativeTitleLineHeightProperty)
        {
            ConfigureTitleLineHeight();
        }
        
        if (change.Property == FontSizeProperty ||
            change.Property == RelativeSubTitleLineHeightProperty)
        {
            ConfigureSubTitleLineHeight();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _statusImage = e.NameScope.Find<SvgControl>(ResultThemeConstants.ErrorCodeImagePart);
        ConfigureStatusImage();
        ConfigureTitleLineHeight();
        ConfigureSubTitleLineHeight();
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

    private void ConfigureTitleLineHeight()
    {
        SetCurrentValue(TitleLineHeightProperty, RelativeTitleLineHeight * FontSize);
    }

    private void ConfigureSubTitleLineHeight()
    {
        SetCurrentValue(SubTitleLineHeightProperty, RelativeSubTitleLineHeight * FontSize);
    }
}