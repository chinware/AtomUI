using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomColorPicker = AtomUI.Desktop.Controls.ColorPicker;
using AtomGradientColorPicker = AtomUI.Desktop.Controls.GradientColorPicker;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static readonly Color ColorPickerDefaultColor = Color.Parse("#1677ff");

    private static IReadOnlyList<PerfScenario> CreateColorPickerScenarios()
    {
        return
        [
            new PerfScenario("ColorPicker.Default", _ => CreateColorPicker()),
            new PerfScenario("ColorPicker.Empty", _ => CreateEmptyColorPicker()),
            new PerfScenario("ColorPicker.Empty.Text", _ => CreateEmptyColorPicker(isTextVisible: true)),
            new PerfScenario("ColorPicker.Text", _ => CreateColorPicker(isTextVisible: true)),
            new PerfScenario("ColorPicker.Small.Text", _ => CreateColorPicker(sizeType: SizeType.Small, isTextVisible: true)),
            new PerfScenario("ColorPicker.Large.Text", _ => CreateColorPicker(sizeType: SizeType.Large, isTextVisible: true)),
            new PerfScenario("ColorPicker.Clear.Text", _ => CreateColorPicker(isTextVisible: true, isClearEnabled: true)),
            new PerfScenario("ColorPicker.Format.Rgba", _ => CreateColorPicker(isTextVisible: true, format: ColorFormat.Rgba)),
            new PerfScenario("ColorPicker.Format.Hsva", _ => CreateColorPicker(isTextVisible: true, format: ColorFormat.Hsva)),
            new PerfScenario("ColorPicker.Disabled", _ => CreateColorPicker(isTextVisible: true, isEnabled: false)),
            new PerfScenario("ColorPickerView.Default", _ => CreateColorPickerView()),
            new PerfScenario("ColorPickerView.NoAlpha", _ => CreateColorPickerView(isAlphaEnabled: false)),
            new PerfScenario("GradientColorPickerView.Default", _ => CreateGradientColorPickerView()),
            new PerfScenario("GradientColorPicker.Text", _ => CreateGradientColorPicker(isTextVisible: true)),
            new PerfScenario("GradientColorPicker.Clear.Text", _ => CreateGradientColorPicker(isTextVisible: true, isClearEnabled: true)),
            new PerfScenario("ColorPicker.GalleryShape", _ => CreateColorPickerGalleryShape())
        ];
    }

    private static AtomColorPicker CreateEmptyColorPicker(bool isTextVisible = false)
    {
        return new AtomColorPicker
        {
            IsTextVisible = isTextVisible,
            IsClearEnabled = true
        };
    }

    private static AtomColorPicker CreateColorPicker(
        SizeType sizeType = SizeType.Middle,
        bool isTextVisible = false,
        bool isClearEnabled = false,
        bool isEnabled = true,
        ColorFormat format = ColorFormat.Hex,
        ColorPickerValueSyncMode valueSyncStrategy = ColorPickerValueSyncMode.Immediate)
    {
        return new AtomColorPicker
        {
            DefaultValue      = ColorPickerDefaultColor,
            SizeType          = sizeType,
            IsTextVisible     = isTextVisible,
            IsClearEnabled    = isClearEnabled,
            IsEnabled         = isEnabled,
            Format            = format,
            ValueSyncStrategy = valueSyncStrategy
        };
    }

    private static AtomGradientColorPicker CreateGradientColorPicker(
        bool isTextVisible = false,
        bool isClearEnabled = false,
        bool isPaletteGroupEnabled = false)
    {
        return new AtomGradientColorPicker
        {
            DefaultValue          = CreateColorPickerGradient(),
            IsTextVisible         = isTextVisible,
            IsClearEnabled        = isClearEnabled,
            IsPaletteGroupEnabled = isPaletteGroupEnabled
        };
    }

    private static LinearGradientBrush CreateColorPickerGradient(string startColor = "#108ee9",
                                                                string endColor = "#87d068")
    {
        return new LinearGradientBrush
        {
            GradientStops =
            {
                new GradientStop(Color.Parse(startColor), 0),
                new GradientStop(Color.Parse(endColor), 1)
            }
        };
    }

    private static ColorPickerView CreateColorPickerView(bool isAlphaEnabled = true)
    {
        return new ColorPickerView
        {
            Value                 = ColorPickerDefaultColor,
            IsAlphaEnabled        = isAlphaEnabled,
            IsPaletteGroupEnabled = false
        };
    }

    private static GradientColorPickerView CreateGradientColorPickerView()
    {
        return new GradientColorPickerView
        {
            DefaultValue          = CreateColorPickerGradient(),
            IsPaletteGroupEnabled = false
        };
    }

    private static Control CreateColorPickerGalleryShape()
    {
        var root = CreateColorPickerVerticalPanel();

        root.Children.Add(CreateColorPicker());
        root.Children.Add(CreateColorPickerVerticalPanel(
            CreateColorPickerRow(
                CreateColorPicker(sizeType: SizeType.Small),
                CreateColorPicker(sizeType: SizeType.Small, isTextVisible: true)),
            CreateColorPickerRow(
                CreateColorPicker(sizeType: SizeType.Middle),
                CreateColorPicker(sizeType: SizeType.Middle, isTextVisible: true)),
            CreateColorPickerRow(
                CreateColorPicker(sizeType: SizeType.Large),
                CreateColorPicker(sizeType: SizeType.Large, isTextVisible: true))));
        root.Children.Add(CreateColorPickerVerticalPanel(CreateGradientColorPicker(isTextVisible: true)));
        root.Children.Add(CreateColorPickerVerticalPanel(
            CreateColorPicker(isTextVisible: true, isClearEnabled: true),
            CreateColorPicker(isTextVisible: true)));
        root.Children.Add(CreateColorPickerVerticalPanel(
            CreateColorPicker(isTextVisible: true, isEnabled: false)));
        root.Children.Add(CreateColorPickerVerticalPanel(
            CreateColorPicker()));
        root.Children.Add(CreateColorPickerVerticalPanel(
            CreateColorPicker(isTextVisible: true, isClearEnabled: true),
            CreateGradientColorPicker(isTextVisible: true, isClearEnabled: true)));
        root.Children.Add(CreateColorPickerRow(
            CreateColorPicker(),
            CreateColorPicker(valueSyncStrategy: ColorPickerValueSyncMode.OnCompleted)));
        root.Children.Add(CreateColorPickerRow(
            CreateColorPicker(),
            new AtomColorPicker
            {
                DefaultValue = ColorPickerDefaultColor,
                TriggerType  = FlyoutTriggerType.Hover
            }));
        root.Children.Add(CreateColorPickerVerticalPanel(
            CreateColorPicker(isTextVisible: true, format: ColorFormat.Hex),
            CreateColorPicker(isTextVisible: true, format: ColorFormat.Hsva),
            CreateColorPicker(isTextVisible: true, format: ColorFormat.Rgba)));
        root.Children.Add(CreateColorPickerRow(
            new AtomColorPicker
            {
                DefaultValue          = ColorPickerDefaultColor,
                IsPaletteGroupEnabled = true
            },
            CreateGradientColorPicker(isPaletteGroupEnabled: true)));

        return root;
    }

    private static StackPanel CreateColorPickerVerticalPanel(params Control[] children)
    {
        var panel = new StackPanel
        {
            Spacing      = 10,
            Orientation = Orientation.Vertical
        };
        foreach (var child in children)
        {
            panel.Children.Add(child);
        }
        return panel;
    }

    private static StackPanel CreateColorPickerRow(params Control[] children)
    {
        var panel = new StackPanel
        {
            Spacing      = 10,
            Orientation = Orientation.Horizontal
        };
        foreach (var child in children)
        {
            panel.Children.Add(child);
        }
        return panel;
    }
}
