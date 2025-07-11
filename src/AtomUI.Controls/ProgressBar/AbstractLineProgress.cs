using AtomUI.Controls.Themes;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum LinePercentAlignment
{
    Start,
    Center,
    End
}

internal class SizeTypeThresholdValue
{
    internal double NormalStateValue { get; set; }
    internal double InnerStateValue { get; set; }
}

[PseudoClasses(ProgressBarPseudoClass.Vertical, ProgressBarPseudoClass.Horizontal)]
public abstract class AbstractLineProgress : AbstractProgressBar
{
    #region 公共属性定义
    /// <summary>
    /// Defines the <see cref="Orientation" /> property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<AbstractLineProgress, Orientation>(nameof(Orientation));

    /// <summary>
    /// Gets or sets the orientation of the <see cref="ProgressBar" />.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    #endregion
    
    #region 内部属性定义

    internal static readonly StyledProperty<double> LineProgressPaddingProperty =
        AvaloniaProperty.Register<AbstractLineProgress, double>(
            nameof(LineProgressPadding));

    internal static readonly StyledProperty<double> LineExtraInfoMarginProperty =
        AvaloniaProperty.Register<AbstractLineProgress, double>(
            nameof(LineExtraInfoMargin));

    internal static readonly StyledProperty<double> LineInfoIconSizeProperty =
        AvaloniaProperty.Register<AbstractLineProgress, double>(
            nameof(LineInfoIconSize));

    internal double LineProgressPadding
    {
        get => GetValue(LineProgressPaddingProperty);
        set => SetValue(LineProgressPaddingProperty, value);
    }

    internal double LineExtraInfoMargin
    {
        get => GetValue(LineExtraInfoMarginProperty);
        set => SetValue(LineExtraInfoMarginProperty, value);
    }

    internal double LineInfoIconSize
    {
        get => GetValue(LineInfoIconSizeProperty);
        set => SetValue(LineInfoIconSizeProperty, value);
    }

    #endregion

    internal Dictionary<SizeType, SizeTypeThresholdValue> _sizeTypeThresholdValue;
    protected Size _extraInfoSize = Size.Infinity;
    protected Rect _grooveRect;

    public AbstractLineProgress()
    {
        _sizeTypeThresholdValue = new Dictionary<SizeType, SizeTypeThresholdValue>();
    }

    // 根据当前的状态进行计算
    protected virtual Size CalculateExtraInfoSize(double fontSize)
    {
        if (Status == ProgressStatus.Exception || MathUtils.AreClose(Value, Maximum))
        {
            // 只要图标
            return new Size(LineInfoIconSize, LineInfoIconSize);
        }

        return TextUtils.CalculateTextSize(string.Format(ProgressTextFormat, Value), fontSize, FontFamily);
    }

    protected override void NotifyUiStructureReady()
    {
        base.NotifyUiStructureReady();
        var    calculateEffectiveSize = false;
        double sizeValue              = 0;
        if (Orientation == Orientation.Horizontal && !double.IsNaN(Height))
        {
            sizeValue              = Height;
            calculateEffectiveSize = true;
        }
        else if (Orientation == Orientation.Vertical && !double.IsNaN(Width))
        {
            sizeValue              = Width;
            calculateEffectiveSize = true;
        }

        if (calculateEffectiveSize)
        {
            EffectiveSizeType = CalculateEffectiveSizeType(sizeValue);
        }

        _extraInfoSize = CalculateExtraInfoSize(FontSize);
        NotifyOrientationChanged();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ProgressBarPseudoClass.Vertical, Orientation == Orientation.Vertical);
        PseudoClasses.Set(ProgressBarPseudoClass.Horizontal, Orientation == Orientation.Horizontal);
    }

    protected override void NotifyPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.NotifyPropertyChanged(e);

        if (this.IsAttachedToVisualTree())
        {
            if ((e.Property == WidthProperty && Orientation == Orientation.Vertical) ||
                (e.Property == HeightProperty && Orientation == Orientation.Horizontal))
            {
                EffectiveSizeType = CalculateEffectiveSizeType(e.GetNewValue<double>());
                CalculateStrokeThickness();
            }
            else if (e.Property == EffectiveSizeTypeProperty)
            {
                _extraInfoSize = CalculateExtraInfoSize(FontSize);
            }
            else if (e.Property == OrientationProperty)
            {
                NotifyOrientationChanged();
            }
        }
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _exceptionCompletedIcon = e.NameScope.Find<Icon>(ProgressBarThemeConstants.ExceptionCompletedIconPart);
        _successCompletedIcon   = e.NameScope.Find<Icon>(ProgressBarThemeConstants.SuccessCompletedIconPart);
    }

    protected virtual void NotifyOrientationChanged()
    {
        UpdatePseudoClasses();
    }

    private bool _lastCompletedStatus;

    protected override void NotifyHandleExtraInfoVisibility()
    {
        base.NotifyHandleExtraInfoVisibility();
        var currentStatus = false;
        if (MathUtils.AreClose(Value, Maximum))
        {
            currentStatus = true;
        }
        else
        {
            currentStatus = false;
        }

        if (currentStatus != _lastCompletedStatus)
        {
            _lastCompletedStatus = currentStatus;
            _extraInfoSize       = CalculateExtraInfoSize(FontSize);
        }
    }
}