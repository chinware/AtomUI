using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Performance;

using AtomMenuFlyout = AtomUI.Desktop.Controls.MenuFlyout;
using AtomSplitButton = AtomUI.Desktop.Controls.SplitButton;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSplitButtonScenarios()
    {
        return
        [
            new PerfScenario("SplitButton.Default.Closed", _ => CreateDefaultSplitButton()),
            new PerfScenario("SplitButton.Primary.Icon.Closed", _ => CreatePrimaryIconSplitButton()),
            new PerfScenario("SplitButton.NoFlyout.Closed", _ => new AtomSplitButton
            {
                Content = "Split"
            }),
            new PerfScenario("SplitButton.Hover.Closed", _ => new AtomSplitButton
            {
                Content     = "Hover",
                TriggerType = FlyoutTriggerType.Hover,
                Flyout      = CreateSplitButtonMenuFlyout()
            }),
            new PerfScenario("SplitButton.Small.Danger.Closed", _ => new AtomSplitButton
            {
                Content  = "Danger",
                SizeType = SizeType.Small,
                IsDanger = true,
                Flyout   = CreateSplitButtonMenuFlyout()
            }),
            new PerfScenario("SplitButton.CompactSpace.Pair", _ => CreateSplitButtonCompactSpace()),
            new PerfScenario("SplitButton.Batch6.Closed", _ => CreateSplitButtonBatch())
        ];
    }

    private static AtomSplitButton CreateDefaultSplitButton()
    {
        return new AtomSplitButton
        {
            Content = "Split",
            Flyout  = CreateSplitButtonMenuFlyout()
        };
    }

    private static AtomSplitButton CreatePrimaryIconSplitButton()
    {
        return new AtomSplitButton
        {
            Content             = "Split",
            Icon                = new SearchOutlined(),
            IsPrimaryButtonType = true,
            Flyout              = CreateSplitButtonMenuFlyout()
        };
    }

    private static AtomMenuFlyout CreateSplitButtonMenuFlyout()
    {
        var flyout = new AtomMenuFlyout();
        flyout.Items.Add(new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "1st menu item"
        });
        flyout.Items.Add(new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "2nd menu item"
        });
        flyout.Items.Add(new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "3rd menu item"
        });
        return flyout;
    }

    private static CompactSpace CreateSplitButtonCompactSpace()
    {
        var compactSpace = new CompactSpace
        {
            Orientation = Orientation.Horizontal
        };
        compactSpace.Children.Add(CreateDefaultSplitButton());
        compactSpace.Children.Add(CreatePrimaryIconSplitButton());
        return compactSpace;
    }

    private static StackPanel CreateSplitButtonBatch()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 8
        };
        panel.Children.Add(CreateDefaultSplitButton());
        panel.Children.Add(CreatePrimaryIconSplitButton());
        panel.Children.Add(new AtomSplitButton
        {
            Content     = "Hover",
            TriggerType = FlyoutTriggerType.Hover,
            Flyout      = CreateSplitButtonMenuFlyout()
        });
        panel.Children.Add(new AtomSplitButton
        {
            Content  = "Small",
            SizeType = SizeType.Small,
            Flyout   = CreateSplitButtonMenuFlyout()
        });
        panel.Children.Add(new AtomSplitButton
        {
            Content = "NoFlyout"
        });
        panel.Children.Add(new AtomSplitButton
        {
            Content             = "Primary",
            IsPrimaryButtonType = true,
            Flyout              = CreateSplitButtonMenuFlyout()
        });
        return panel;
    }
}
