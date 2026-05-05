using Avalonia;

namespace AtomUI.Desktop.Controls;

public class TextTourIndicator : TourIndicator
{
    #region 内部属性定义

    internal static readonly DirectProperty<TextTourIndicator, string?> IndicatorTextProperty =
        AvaloniaProperty.RegisterDirect<TextTourIndicator, string?>(nameof(IndicatorText),
            o => o.IndicatorText,
            (o, v) => o.IndicatorText = v);
    
    private string? _indicatorText;

    internal string? IndicatorText
    {
        get => _indicatorText;
        private set => SetAndRaise(IndicatorTextProperty, ref _indicatorText, value);
    }

    #endregion

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ActiveIndexProperty ||
            change.Property == StepCountProperty)
        {
            NotifyBuildIndicatorText();
        }
    }

    protected virtual void NotifyBuildIndicatorText()
    {
        SetCurrentValue(IndicatorTextProperty, $"{ActiveIndex + 1} / {StepCount}");
    }
}