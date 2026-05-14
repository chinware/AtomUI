using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateIconScenarios()
    {
        return
        [
            new PerfScenario("Icon.SearchOutlined.Direct", _ => CreateSearchIcon()),
            new PerfScenario("Icon.SearchOutlined.Presenter", _ => new IconPresenter
            {
                Width = 16,
                Height = 16,
                Icon = CreateSearchIcon()
            }),
            new PerfScenario("Icon.SearchOutlined.Many10", _ => CreateIconBatch()),
            new PerfScenario("Icon.LoadingOutlined.Spin", _ => new LoadingOutlined
            {
                Width = 16,
                Height = 16,
                LoadingAnimation = IconAnimation.Spin
            }),
            new PerfScenario("Icon.TwoTone.Bulb", _ => new BulbTwoTone
            {
                Width = 16,
                Height = 16
            }),
            new PerfScenario("Icon.Provider.SearchOutlined", _ =>
                (Control)new AntDesignIconProvider(AntDesignIconKind.SearchOutlined).ProvideValue(null!)),
            new PerfScenario("Icon.HiddenSlots.SelectDefault", _ => new Select
            {
                Width = 260,
                PlaceholderText = "Select"
            }),
            new PerfScenario("Icon.HiddenSlots.MenuItemLeaf", _ => new AtomUI.Desktop.Controls.MenuItem
            {
                Header = "Leaf item"
            }),
            new PerfScenario("Icon.HiddenSlots.MenuItemSubmenu", _ => CreateMenuItemWithSubmenu()),
            new PerfScenario("Icon.HiddenSlots.ToggleIconButton", _ => CreateToggleIconButton()),
            new PerfScenario("Icon.HiddenSlots.NavMenuInlineLeaf", _ => new InlineNavMenuItemHeader
            {
                Header = "Leaf item"
            }),
            new PerfScenario("Icon.HiddenSlots.NavMenuInlineSubmenu", _ => new InlineNavMenuItemHeader
            {
                Header     = "Parent item",
                HasSubMenu = true
            }),
            new PerfScenario("Icon.HiddenSlots.ButtonDefault", _ => new AtomUI.Desktop.Controls.Button
            {
                Content = "Button"
            }),
            new PerfScenario("Icon.HiddenSlots.ButtonLoading", _ => new AtomUI.Desktop.Controls.Button
            {
                Content   = "Loading",
                IsLoading = true
            })
        ];
    }

    private static Icon CreateSearchIcon()
    {
        return new SearchOutlined
        {
            Width  = 16,
            Height = 16
        };
    }

    private static Panel CreateIconBatch()
    {
        var panel = new WrapPanel
        {
            Width = 240
        };
        for (var i = 0; i < 10; i++)
        {
            panel.Children.Add(CreateSearchIcon());
        }

        return panel;
    }

    private static AtomUI.Desktop.Controls.MenuItem CreateMenuItemWithSubmenu()
    {
        var menuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "Parent item"
        };
        menuItem.Items.Add(new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "Child item"
        });
        return menuItem;
    }

    private static ToggleIconButton CreateToggleIconButton()
    {
        return new ToggleIconButton
        {
            Width         = 32,
            Height        = 32,
            CheckedIcon   = new EyeOutlined(),
            UnCheckedIcon = new EyeInvisibleOutlined()
        };
    }
}
