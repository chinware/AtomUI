using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomNumericUpDown = AtomUI.Desktop.Controls.NumericUpDown;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateNumericUpDownScenarios()
    {
        return
        [
            new PerfScenario("NumericUpDown.Default", _ => CreateNumericUpDown()),
            new PerfScenario("NumericUpDown.AllowClear.Empty", _ => CreateNumericUpDown(isAllowClear: true)),
            new PerfScenario("NumericUpDown.AllowClear.Value", _ => CreateNumericUpDown(value: 12, isAllowClear: true)),
            new PerfScenario("NumericUpDown.StringMode", _ => CreateNumericUpDown(isStringMode: true, stringValue: "0.123456789012345678901234")),
            new PerfScenario("NumericUpDown.KeyboardDisabled", _ => CreateNumericUpDown(value: 1.2m, increment: 0.1m, isKeyboardEnabled: false)),
            new PerfScenario("NumericUpDown.SizeLarge", _ => CreateNumericUpDown(sizeType: SizeType.Large)),
            new PerfScenario("NumericUpDown.SizeSmall", _ => CreateNumericUpDown(sizeType: SizeType.Small)),
            new PerfScenario("NumericUpDown.Filled", _ => CreateNumericUpDown(styleVariant: InputControlStyleVariant.Filled)),
            new PerfScenario("NumericUpDown.Borderless", _ => CreateNumericUpDown(styleVariant: InputControlStyleVariant.Borderless)),
            new PerfScenario("NumericUpDown.Disabled", _ => CreateNumericUpDown(isEnabled: false)),
            new PerfScenario("NumericUpDown.OuterAddOns", _ => CreateNumericUpDown(leftAddOn: "http://", rightAddOn: ".com", width: 400)),
            new PerfScenario("NumericUpDown.InnerTextAddOns", _ => CreateNumericUpDown(innerLeftContent: "￥", innerRightContent: "RMB", width: 400)),
            new PerfScenario("NumericUpDown.InnerIconAddOns", _ => CreateNumericUpDown(
                innerLeftContent: new UserOutlined(),
                innerRightContent: new InfoCircleOutlined(),
                width: 400)),
            new PerfScenario("NumericUpDown.StatusError", _ => CreateNumericUpDown(status: InputControlStatus.Error)),
            new PerfScenario("NumericUpDown.StatusIcon", _ => CreateNumericUpDown(
                status: InputControlStatus.Error,
                innerLeftContent: new ClockCircleOutlined(),
                width: 400)),
            new PerfScenario("NumericUpDown.GalleryShape.Batch30", _ => CreateNumericUpDownGalleryShape())
        ];
    }

    private static AtomNumericUpDown CreateNumericUpDown(
        decimal? value = 3,
        double width = 160,
        decimal increment = 1,
        SizeType sizeType = SizeType.Middle,
        InputControlStyleVariant styleVariant = InputControlStyleVariant.Outlined,
        InputControlStatus status = InputControlStatus.Default,
        bool isEnabled = true,
        bool isAllowClear = false,
        bool isStringMode = false,
        string? stringValue = null,
        bool isKeyboardEnabled = true,
        object? leftAddOn = null,
        object? rightAddOn = null,
        object? innerLeftContent = null,
        object? innerRightContent = null)
    {
        return new AtomNumericUpDown
        {
            Width             = width,
            Value             = value,
            Increment         = increment,
            SizeType          = sizeType,
            StyleVariant      = styleVariant,
            Status            = status,
            IsEnabled         = isEnabled,
            IsAllowClear      = isAllowClear,
            IsStringMode      = isStringMode,
            StringValue       = stringValue,
            IsKeyboardEnabled = isKeyboardEnabled,
            LeftAddOn         = leftAddOn,
            RightAddOn        = rightAddOn,
            InnerLeftContent  = innerLeftContent,
            InnerRightContent = innerRightContent,
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private static Control CreateNumericUpDownGalleryShape()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 8,
            Width       = 460
        };

        panel.Children.Add(CreateNumericUpDown());
        panel.Children.Add(CreateNumericUpDown(isStringMode: true, stringValue: "0.123456789012345678901234", increment: 0.0001m));
        panel.Children.Add(CreateNumericUpDown(value: 1.2m, increment: 0.1m));
        panel.Children.Add(CreateNumericUpDown(value: 2.5m, increment: 0.5m));
        panel.Children.Add(CreateNumericUpDown(value: 3, increment: 1));
        panel.Children.Add(CreateNumericUpDown(value: 1.2m, increment: 0.25m));

        panel.Children.Add(CreateNumericUpDown(sizeType: SizeType.Large));
        panel.Children.Add(CreateNumericUpDown(sizeType: SizeType.Middle));
        panel.Children.Add(CreateNumericUpDown(sizeType: SizeType.Small));

        panel.Children.Add(CreateNumericUpDown(styleVariant: InputControlStyleVariant.Outlined));
        panel.Children.Add(CreateNumericUpDown(styleVariant: InputControlStyleVariant.Filled));
        panel.Children.Add(CreateNumericUpDown(styleVariant: InputControlStyleVariant.Borderless));

        panel.Children.Add(CreateNumericUpDown(styleVariant: InputControlStyleVariant.Outlined, isEnabled: false));
        panel.Children.Add(CreateNumericUpDown(styleVariant: InputControlStyleVariant.Filled, isEnabled: false));
        panel.Children.Add(CreateNumericUpDown(styleVariant: InputControlStyleVariant.Borderless, isEnabled: false));

        panel.Children.Add(CreateNumericUpDown(leftAddOn: "http://", rightAddOn: ".com", width: 400));
        panel.Children.Add(CreateNumericUpDown(rightAddOn: new SettingOutlined(), width: 400));
        panel.Children.Add(CreateNumericUpDown(leftAddOn: "http://", innerRightContent: ".com", width: 400));

        panel.Children.Add(CreateNumericUpDown(value: null, isAllowClear: true, width: 400));

        panel.Children.Add(CreateNumericUpDown(innerLeftContent: new UserOutlined(), innerRightContent: new InfoCircleOutlined(), width: 400));
        panel.Children.Add(CreateNumericUpDown(innerLeftContent: "￥", innerRightContent: "RMB", width: 400));
        panel.Children.Add(CreateNumericUpDown(innerLeftContent: "￥", innerRightContent: "RMB", isEnabled: false, width: 400));

        AddNumericUpDownStatusExamples(panel, InputControlStatus.Error);
        AddNumericUpDownStatusExamples(panel, InputControlStatus.Warning);

        return panel;
    }

    private static void AddNumericUpDownStatusExamples(StackPanel panel, InputControlStatus status)
    {
        panel.Children.Add(CreateNumericUpDown(status: status, width: 400));
        panel.Children.Add(CreateNumericUpDown(status: status, innerLeftContent: new ClockCircleOutlined(), width: 400));
        panel.Children.Add(CreateNumericUpDown(status: status, styleVariant: InputControlStyleVariant.Filled, innerLeftContent: new ClockCircleOutlined(), width: 400));
        panel.Children.Add(CreateNumericUpDown(status: status, styleVariant: InputControlStyleVariant.Borderless, innerLeftContent: new ClockCircleOutlined(), width: 400));
    }
}
