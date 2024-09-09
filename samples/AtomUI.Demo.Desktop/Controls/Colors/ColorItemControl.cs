using AtomUI.Demo.Desktop.ViewModels;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AtomUI.Demo.Desktop.Controls;

public class ColorItemControl : TemplatedControl
{
    public static readonly StyledProperty<string?> ColorNameProperty =
        AvaloniaProperty.Register<ColorItemControl, string?>(
            nameof(ColorName));

    public static readonly StyledProperty<string?> HexProperty = AvaloniaProperty.Register<ColorItemControl, string?>(
        nameof(Hex));

    public string? ColorName
    {
        get => GetValue(ColorNameProperty);
        set => SetValue(ColorNameProperty, value);
    }

    public string? Hex
    {
        get => GetValue(HexProperty);
        set => SetValue(HexProperty, value);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (DataContext is ColorItemViewModel v) WeakReferenceMessenger.Default.Send(v);
    }
}