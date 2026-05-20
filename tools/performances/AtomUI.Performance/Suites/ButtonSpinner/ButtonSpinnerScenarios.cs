using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomButtonSpinner = AtomUI.Desktop.Controls.ButtonSpinner;
using AtomNumericUpDown = AtomUI.Desktop.Controls.NumericUpDown;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateButtonSpinnerScenarios()
    {
        return
        [
            new PerfScenario("ButtonSpinner.Default", _ => CreateButtonSpinner()),
            new PerfScenario("ButtonSpinner.HiddenHandle", _ => CreateButtonSpinner(isButtonSpinnerVisible: false)),
            new PerfScenario("ButtonSpinner.Floatable", _ => CreateButtonSpinner(isButtonSpinnerFloatable: true)),
            new PerfScenario("ButtonSpinner.Disabled", _ => CreateButtonSpinner(isEnabled: false)),
            new PerfScenario("ButtonSpinner.LeftHandle", _ => CreateButtonSpinner(location: ButtonSpinnerLocation.Left)),
            new PerfScenario("ButtonSpinner.LeftRightAddOn", _ => CreateButtonSpinner(leftAddOn: "http://", rightAddOn: ".com", width: 400)),
            new PerfScenario("ButtonSpinner.RightIconAddOn", _ => CreateButtonSpinner(rightAddOn: new SettingOutlined(), width: 400)),
            new PerfScenario("ButtonSpinner.InnerTextAddOns", _ => CreateButtonSpinner(innerLeft: "￥", innerRight: "RMB", width: 400)),
            new PerfScenario("ButtonSpinner.InnerIconAddOns", _ => CreateButtonSpinner(innerLeft: new UserOutlined(), innerRight: new InfoCircleOutlined(), width: 400)),
            new PerfScenario("ButtonSpinner.StatusIcon", _ => CreateButtonSpinner(status: InputControlStatus.Error, innerLeft: new ClockCircleOutlined(), width: 400)),
            new PerfScenario("ButtonSpinner.GalleryShape.Batch24", _ => CreateGalleryShapeBatch()),
            new PerfScenario("NumericUpDown.Default", _ => new AtomNumericUpDown
            {
                Width = 160,
                Value = 3
            })
        ];
    }

    private static AtomButtonSpinner CreateButtonSpinner(
        double width = 160,
        SizeType sizeType = SizeType.Middle,
        InputControlStyleVariant styleVariant = InputControlStyleVariant.Outlined,
        InputControlStatus status = InputControlStatus.Default,
        bool isEnabled = true,
        bool isButtonSpinnerVisible = true,
        bool isButtonSpinnerFloatable = false,
        ButtonSpinnerLocation location = ButtonSpinnerLocation.Right,
        object? leftAddOn = null,
        object? rightAddOn = null,
        object? innerLeft = null,
        object? innerRight = null)
    {
        return new AtomButtonSpinner
        {
            Width                    = width,
            SizeType                 = sizeType,
            StyleVariant             = styleVariant,
            Status                   = status,
            IsEnabled                = isEnabled,
            IsButtonSpinnerVisible   = isButtonSpinnerVisible,
            IsButtonSpinnerFloatable = isButtonSpinnerFloatable,
            ButtonSpinnerLocation    = location,
            LeftAddOn                = leftAddOn,
            RightAddOn               = rightAddOn,
            InnerLeftContent         = innerLeft,
            InnerRightContent        = innerRight,
            HorizontalAlignment      = HorizontalAlignment.Left,
            Content                  = CreateButtonSpinnerText()
        };
    }

    private static Control CreateGalleryShapeBatch()
    {
        var panel = new StackPanel
        {
            Spacing = 10,
            Width   = 460
        };

        panel.Children.Add(CreateButtonSpinner(width: 400));

        panel.Children.Add(CreateButtonSpinner(sizeType: SizeType.Large, width: 400));
        panel.Children.Add(CreateButtonSpinner(sizeType: SizeType.Middle, width: 400));
        panel.Children.Add(CreateButtonSpinner(sizeType: SizeType.Small, width: 400));

        panel.Children.Add(CreateButtonSpinner(styleVariant: InputControlStyleVariant.Outlined, width: 400));
        panel.Children.Add(CreateButtonSpinner(styleVariant: InputControlStyleVariant.Filled, width: 400));
        panel.Children.Add(CreateButtonSpinner(styleVariant: InputControlStyleVariant.Borderless, width: 400));

        panel.Children.Add(CreateButtonSpinner(styleVariant: InputControlStyleVariant.Outlined, isEnabled: false, width: 400));
        panel.Children.Add(CreateButtonSpinner(styleVariant: InputControlStyleVariant.Filled, isEnabled: false, width: 400));
        panel.Children.Add(CreateButtonSpinner(styleVariant: InputControlStyleVariant.Borderless, isEnabled: false, width: 400));

        panel.Children.Add(CreateButtonSpinner(leftAddOn: "http://", rightAddOn: ".com", width: 400));
        panel.Children.Add(CreateButtonSpinner(rightAddOn: new SettingOutlined(), width: 400));
        panel.Children.Add(CreateButtonSpinner(leftAddOn: "http://", innerRight: ".com", width: 400));

        panel.Children.Add(CreateButtonSpinner(innerLeft: new UserOutlined(), innerRight: new InfoCircleOutlined(), width: 400));
        panel.Children.Add(CreateButtonSpinner(innerLeft: "￥", innerRight: "RMB", width: 400));
        panel.Children.Add(CreateButtonSpinner(innerLeft: "￥", innerRight: "RMB", isEnabled: false, width: 400));

        AddStatusExamples(panel, InputControlStatus.Error);
        AddStatusExamples(panel, InputControlStatus.Warning);

        return panel;
    }

    private static void AddStatusExamples(StackPanel panel, InputControlStatus status)
    {
        panel.Children.Add(CreateButtonSpinner(status: status, width: 400));
        panel.Children.Add(CreateButtonSpinner(status: status, innerLeft: new ClockCircleOutlined(), width: 400));
        panel.Children.Add(CreateButtonSpinner(status: status, styleVariant: InputControlStyleVariant.Filled, innerLeft: new ClockCircleOutlined(), width: 400));
        panel.Children.Add(CreateButtonSpinner(status: status, styleVariant: InputControlStyleVariant.Borderless, innerLeft: new ClockCircleOutlined(), width: 400));
    }

    private static AtomTextBlock CreateButtonSpinnerText()
    {
        return new AtomTextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment   = VerticalAlignment.Center,
            Text                = "床前明月光"
        };
    }
}
