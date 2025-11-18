using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class SelectMaxCountIndicator : TemplatedControl
{
    public static readonly DirectProperty<SelectMaxCountIndicator, string?> TextProperty =
        AvaloniaProperty.RegisterDirect<SelectMaxCountIndicator, string?>(nameof(Text),
            o => o.Text,
            (o, v) => o.Text = v);
    
    internal static readonly DirectProperty<SelectMaxCountIndicator, int> MaxCountProperty =
        AvaloniaProperty.RegisterDirect<SelectMaxCountIndicator, int>(nameof(MaxCount),
            o => o.MaxCount,
            (o, v) => o.MaxCount = v);
    
    internal static readonly DirectProperty<SelectMaxCountIndicator, int> SelectedCountProperty =
        AvaloniaProperty.RegisterDirect<SelectMaxCountIndicator, int>(nameof(SelectedCount),
            o => o.SelectedCount,
            (o, v) => o.SelectedCount = v);
    
    private string? _text;

    internal string? Text
    {
        get => _text;
        set => SetAndRaise(TextProperty, ref _text, value);
    }
    
    private int _maxCount;

    internal int MaxCount
    {
        get => _maxCount;
        set => SetAndRaise(MaxCountProperty, ref _maxCount, value);
    }
    
    private int _selectedCount;

    internal int SelectedCount
    {
        get => _selectedCount;
        set => SetAndRaise(SelectedCountProperty, ref _selectedCount, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == MaxCountProperty ||
            change.Property == SelectedCountProperty)
        {
            SetCurrentValue(TextProperty, $"{SelectedCount} / {MaxCount}");
        }
    }
}