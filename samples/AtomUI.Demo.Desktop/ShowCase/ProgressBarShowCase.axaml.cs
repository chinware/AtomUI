using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class ProgressBarShowCase : UserControl
{
    public static readonly StyledProperty<double> ProgressValueProperty =
        AvaloniaProperty.Register<ProgressBarShowCase, double>(nameof(ProgressValue), 30);

    public static readonly StyledProperty<string> ToggleDisabledTextProperty =
        AvaloniaProperty.Register<ProgressBarShowCase, string>(nameof(ToggleDisabledText), "Disable");

    public static readonly StyledProperty<bool> ToggleStatusProperty =
        AvaloniaProperty.Register<ProgressBarShowCase, bool>(nameof(ToggleStatus), true);

    public ProgressBarShowCase()
    {
        InitializeComponent();
        DataContext = this;

        TwoStopsGradientStrokeColor = new LinearGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Color.Parse("#108ee9"), 0),
                new GradientStop(Color.Parse("#87d068"), 1)
            }
        };
        ThreeStopsGradientStrokeColor = new LinearGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Color.Parse("#87d068"), 0),
                new GradientStop(Color.Parse("#ffe58f"), 0.5),
                new GradientStop(Color.Parse("#ffccc7"), 1)
            }
        };
        StepsChunkBrushes = new List<IBrush>
        {
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Red)
        };

        InnerStartPercentPosition = new PercentPosition
        {
            IsInner   = true,
            Alignment = LinePercentAlignment.Start
        };
        InnerCenterPercentPosition = new PercentPosition
        {
            IsInner   = true,
            Alignment = LinePercentAlignment.Center
        };
        InnerEndPercentPosition = new PercentPosition
        {
            IsInner   = true,
            Alignment = LinePercentAlignment.End
        };

        OutterStartPercentPosition = new PercentPosition
        {
            IsInner   = false,
            Alignment = LinePercentAlignment.Start
        };
        OutterCenterPercentPosition = new PercentPosition
        {
            IsInner   = false,
            Alignment = LinePercentAlignment.Center
        };
        OutterEndPercentPosition = new PercentPosition
        {
            IsInner   = false,
            Alignment = LinePercentAlignment.End
        };
    }

    public LinearGradientBrush TwoStopsGradientStrokeColor { get; set; }

    public LinearGradientBrush ThreeStopsGradientStrokeColor { get; set; }

    public List<IBrush> StepsChunkBrushes { get; set; }

    public PercentPosition InnerStartPercentPosition { get; set; }
    public PercentPosition InnerCenterPercentPosition { get; set; }
    public PercentPosition InnerEndPercentPosition { get; set; }

    public PercentPosition OutterStartPercentPosition { get; set; }
    public PercentPosition OutterCenterPercentPosition { get; set; }
    public PercentPosition OutterEndPercentPosition { get; set; }

    public double ProgressValue
    {
        get => GetValue(ProgressValueProperty);
        set => SetValue(ProgressValueProperty, value);
    }

    public string ToggleDisabledText
    {
        get => GetValue(ToggleDisabledTextProperty);
        set => SetValue(ToggleDisabledTextProperty, value);
    }

    public bool ToggleStatus
    {
        get => GetValue(ToggleStatusProperty);
        set => SetValue(ToggleStatusProperty, value);
    }

    public void AddProgressValue()
    {
        var value = ProgressValue;
        value         += 10;
        ProgressValue =  Math.Min(value, 100);
    }

    public void SubProgressValue()
    {
        var value = ProgressValue;
        value         -= 10;
        ProgressValue =  Math.Max(value, 0);
    }

    public void ToggleEnabledStatus()
    {
        ToggleStatus = !ToggleStatus;
        if (ToggleStatus)
        {
            ToggleDisabledText = "Disable";
        }
        else
        {
            ToggleDisabledText = "Enable";
        }
    }
}