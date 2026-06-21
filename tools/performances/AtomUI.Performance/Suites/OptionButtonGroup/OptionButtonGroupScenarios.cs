using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateOptionButtonGroupScenarios()
    {
        return
        [
            new PerfScenario("OptionButtonGroup.Text.Solid3", _ => CreateTextGroup(OptionButtonStyle.Solid, 3)),
            new PerfScenario("OptionButtonGroup.Text.Outline3", _ => CreateTextGroup(OptionButtonStyle.Outline, 3)),
            new PerfScenario("OptionButtonGroup.Text.Solid4", _ => CreateTextGroup(OptionButtonStyle.Solid, 4)),
            new PerfScenario("OptionButtonGroup.Text.Outline4", _ => CreateTextGroup(OptionButtonStyle.Outline, 4)),
            new PerfScenario("OptionButtonGroup.Text.Outline4.Disabled", _ => CreateTextGroup(OptionButtonStyle.Outline, 4, disabledIndex: 2)),
            new PerfScenario("OptionButtonGroup.Icon.Solid3", _ => CreateIconGroup(OptionButtonStyle.Solid, CustomizableSizeType.Middle)),
            new PerfScenario("OptionButtonGroup.Icon.Outline3", _ => CreateIconGroup(OptionButtonStyle.Outline, CustomizableSizeType.Middle)),
            new PerfScenario("OptionButtonGroup.Icon.Outline3.Large", _ => CreateIconGroup(OptionButtonStyle.Outline, CustomizableSizeType.Large)),
            new PerfScenario("OptionButtonGroup.Icon.Outline3.Small", _ => CreateIconGroup(OptionButtonStyle.Outline, CustomizableSizeType.Small)),
            new PerfScenario("OptionButtonGroup.GalleryShape.RadioButtonShowCase", _ => CreateRadioButtonShowCaseOptionShape())
        ];
    }

    private static OptionButtonGroup CreateTextGroup(OptionButtonStyle style,
                                                     int count,
                                                     int checkedIndex = 0,
                                                     int disabledIndex = -1,
                                                     CustomizableSizeType sizeType = CustomizableSizeType.Middle)
    {
        var group = new OptionButtonGroup
        {
            ButtonStyle = style,
            SizeType    = sizeType
        };

        for (var i = 0; i < count; i++)
        {
            group.Items.Add(new OptionButton
            {
                Content   = CreateOptionLabel(i),
                IsChecked = i == checkedIndex,
                IsEnabled = i != disabledIndex
            });
        }

        return group;
    }

    private static OptionButtonGroup CreateIconGroup(OptionButtonStyle style, CustomizableSizeType sizeType)
    {
        var group = new OptionButtonGroup
        {
            ButtonStyle = style,
            SizeType    = sizeType
        };

        group.Items.Add(new OptionButton
        {
            Content   = "macOS",
            Icon      = new AppleOutlined(),
            IsChecked = true
        });
        group.Items.Add(new OptionButton
        {
            Content = "Linux",
            Icon    = new LinuxOutlined()
        });
        group.Items.Add(new OptionButton
        {
            Content = "Windows",
            Icon    = new WindowsOutlined()
        });

        return group;
    }

    private static Control CreateRadioButtonShowCaseOptionShape()
    {
        var panel = new StackPanel
        {
            Spacing = 10
        };

        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Solid, 3));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Outline, 3, checkedIndex: 1, disabledIndex: 2));
        panel.Children.Add(CreateIconGroup(OptionButtonStyle.Solid, CustomizableSizeType.Middle));
        panel.Children.Add(CreateIconGroup(OptionButtonStyle.Outline, CustomizableSizeType.Middle));

        var disabledSolid = CreateIconGroup(OptionButtonStyle.Solid, CustomizableSizeType.Middle);
        disabledSolid.IsEnabled = false;
        panel.Children.Add(disabledSolid);

        var disabledOutline = CreateIconGroup(OptionButtonStyle.Outline, CustomizableSizeType.Middle);
        disabledOutline.IsEnabled = false;
        panel.Children.Add(disabledOutline);

        panel.Children.Add(CreateIconGroup(OptionButtonStyle.Outline, CustomizableSizeType.Large));
        panel.Children.Add(CreateIconGroup(OptionButtonStyle.Outline, CustomizableSizeType.Middle));
        panel.Children.Add(CreateIconGroup(OptionButtonStyle.Outline, CustomizableSizeType.Small));

        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Outline, 4));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Outline, 4, disabledIndex: 1));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Outline, 4, disabledIndex: 0));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Solid, 4));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Solid, 4, disabledIndex: 1));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Solid, 4, disabledIndex: 0));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Outline, 4, sizeType: CustomizableSizeType.Large));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Outline, 4, sizeType: CustomizableSizeType.Middle));
        panel.Children.Add(CreateTextGroup(OptionButtonStyle.Outline, 4, sizeType: CustomizableSizeType.Small));

        return panel;
    }

    private static string CreateOptionLabel(int index)
    {
        return index switch
        {
            0 => "Hangzhou",
            1 => "Shanghai",
            2 => "Beijing",
            _ => "Chengdu"
        };
    }
}
