using AtomUI;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateDrawerScenarios()
    {
        return
        [
            new PerfScenario("Drawer.Closed.Basic", _ => CreateDrawer()),
            new PerfScenario("Drawer.Closed.NoMask", _ => CreateDrawer(isShowMask: false)),
            new PerfScenario("Drawer.Closed.ExtraFooter", _ => CreateDrawer(includeExtra: true, includeFooter: true)),
            new PerfScenario("Drawer.Closed.Nested", _ => CreateNestedDrawer())
        ];
    }

    private static Drawer CreateDrawer(bool isShowMask = true,
                                       bool includeExtra = false,
                                       bool includeFooter = false)
    {
        var drawer = new Drawer
        {
            Title      = "Basic Drawer",
            DialogSize = new Dimension(50, DimensionUnitType.Percentage),
            IsShowMask = isShowMask,
            Content    = CreateDrawerContent()
        };

        if (includeExtra)
        {
            drawer.Extra = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing     = 10,
                Children =
                {
                    new AtomUI.Desktop.Controls.Button { Content = "Cancel" },
                    new AtomUI.Desktop.Controls.Button { Content = "Ok", ButtonType = ButtonType.Primary }
                }
            };
        }

        if (includeFooter)
        {
            drawer.Footer = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing     = 10,
                Children =
                {
                    new AtomUI.Desktop.Controls.Button { Content = "Edit" },
                    new AtomUI.Desktop.Controls.Button { Content = "Upload", ButtonType = ButtonType.Primary },
                    new AtomUI.Desktop.Controls.Button { Content = "Delete", ButtonType = ButtonType.Primary, IsDanger = true }
                }
            };
        }

        return drawer;
    }

    private static Drawer CreateNestedDrawer()
    {
        var childDrawer = CreateDrawer();
        childDrawer.Title = "Two-level Drawer";

        var parentDrawer = CreateDrawer();
        parentDrawer.Title = "First-level Drawer";
        parentDrawer.Content = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 5,
            Children =
            {
                new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." },
                new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." },
                new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." },
                new AtomUI.Desktop.Controls.Button { Content = "Two-level drawer", ButtonType = ButtonType.Primary },
                childDrawer
            }
        };
        return parentDrawer;
    }

    private static StackPanel CreateDrawerContent()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 5,
            Children =
            {
                new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." },
                new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." },
                new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." }
            }
        };
    }
}
