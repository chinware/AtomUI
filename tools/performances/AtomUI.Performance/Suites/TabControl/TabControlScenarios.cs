using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomTabControl = AtomUI.Desktop.Controls.TabControl;
using AtomTabItem = AtomUI.Desktop.Controls.TabItem;
using AtomTabStrip = AtomUI.Desktop.Controls.TabStrip;
using AtomTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateTabControlScenarios()
    {
        return
        [
            new PerfScenario("TabControl.Line.Basic3", _ => CreateTabControl()),
            new PerfScenario("TabControl.Line.Icon3", _ => CreateTabControl(iconCount: 3)),
            new PerfScenario("TabControl.Line.Closable3", _ => CreateTabControl(isClosable: true)),
            new PerfScenario("TabControl.Line.Overflow20", _ => CreateTabControl(itemCount: 20, width: 520)),
            new PerfScenario("TabControl.Card.Basic3", _ => CreateCardTabControl()),
            new PerfScenario("TabControl.Card.Icon3", _ => CreateCardTabControl(iconCount: 3)),
            new PerfScenario("TabControl.Card.Closable3", _ => CreateCardTabControl(isClosable: true)),
            new PerfScenario("TabControl.Card.Overflow20", _ => CreateCardTabControl(itemCount: 20, width: 520)),
            new PerfScenario("TabControl.Card.AddButton", _ => CreateCardTabControl(isShowAddButton: true)),
            new PerfScenario("TabStrip.Line.Basic3", _ => CreateTabStrip()),
            new PerfScenario("TabStrip.Line.Icon3", _ => CreateTabStrip(iconCount: 3)),
            new PerfScenario("TabStrip.Line.Closable3", _ => CreateTabStrip(isClosable: true)),
            new PerfScenario("TabStrip.Card.Mixed4", _ => CreateCardTabStrip(iconCount: 4, isClosable: true)),
            new PerfScenario("TabControl.GalleryShape.FirstSection", _ => CreateTabControlGalleryShape())
        ];
    }

    private static AtomTabControl CreateTabControl(int itemCount = 3,
                                                   int iconCount = 0,
                                                   bool isClosable = false,
                                                   double width = 720,
                                                   Dock placement = Dock.Top)
    {
        var tabControl = new AtomTabControl
        {
            Width             = width,
            TabStripPlacement = placement,
            IsTabClosable     = isClosable
        };

        AddTabItems(tabControl, itemCount, iconCount, isClosable);
        return tabControl;
    }

    private static CardTabControl CreateCardTabControl(int itemCount = 3,
                                                       int iconCount = 0,
                                                       bool isClosable = false,
                                                       bool isShowAddButton = false,
                                                       double width = 720,
                                                       Dock placement = Dock.Top)
    {
        var tabControl = new CardTabControl
        {
            Width              = width,
            TabStripPlacement  = placement,
            IsTabClosable      = isClosable,
            IsShowAddTabButton = isShowAddButton
        };

        AddTabItems(tabControl, itemCount, iconCount, isClosable);
        return tabControl;
    }

    private static AtomTabStrip CreateTabStrip(int itemCount = 3,
                                               int iconCount = 0,
                                               bool isClosable = false,
                                               double width = 720,
                                               Dock placement = Dock.Top)
    {
        var tabStrip = new AtomTabStrip
        {
            Width             = width,
            TabStripPlacement = placement,
            IsTabClosable     = isClosable
        };

        AddTabStripItems(tabStrip, itemCount, iconCount, isClosable);
        return tabStrip;
    }

    private static CardTabStrip CreateCardTabStrip(int itemCount = 4,
                                                   int iconCount = 0,
                                                   bool isClosable = false,
                                                   bool isShowAddButton = false,
                                                   double width = 720,
                                                   Dock placement = Dock.Top)
    {
        var tabStrip = new CardTabStrip
        {
            Width              = width,
            TabStripPlacement  = placement,
            IsTabClosable      = isClosable,
            IsShowAddTabButton = isShowAddButton
        };

        AddTabStripItems(tabStrip, itemCount, iconCount, isClosable);
        return tabStrip;
    }

    private static void AddTabItems(BaseTabControl tabControl, int itemCount, int iconCount, bool isClosable)
    {
        for (var i = 0; i < itemCount; i++)
        {
            tabControl.Items.Add(new AtomTabItem
            {
                Header     = $"Tab {i + 1}",
                Content    = $"Content of Tab Pane {i + 1}",
                Icon       = i < iconCount ? CreateTabIcon(i) : null,
                IsClosable = isClosable && i != 1
            });
        }
    }

    private static void AddTabStripItems(BaseTabStrip tabStrip, int itemCount, int iconCount, bool isClosable)
    {
        for (var i = 0; i < itemCount; i++)
        {
            tabStrip.Items.Add(new AtomTabStripItem
            {
                Content    = $"Tab {i + 1}",
                Icon       = i < iconCount ? CreateTabIcon(i) : null,
                IsClosable = isClosable && i != 1
            });
        }
    }

    private static PathIcon CreateTabIcon(int index)
    {
        return (index % 4) switch
        {
            0 => new AppleOutlined(),
            1 => new AndroidOutlined(),
            2 => new WechatOutlined(),
            _ => new GithubOutlined()
        };
    }

    private static Control CreateTabControlGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 12
        };

        root.Children.Add(CreateTabControl());
        root.Children.Add(CreateTabControl(iconCount: 3));
        root.Children.Add(CreateTabControl(itemCount: 20, width: 520));
        root.Children.Add(CreateCardTabControl());
        root.Children.Add(CreateCardTabControl(iconCount: 3));
        root.Children.Add(CreateCardTabControl(isClosable: true, iconCount: 3));
        root.Children.Add(CreateCardTabControl(itemCount: 20, width: 520));
        root.Children.Add(CreateCardTabControl(isShowAddButton: true, iconCount: 2));

        var positionedLine = CreateTabControl(itemCount: 8, iconCount: 3, width: 600, placement: Dock.Left);
        positionedLine.Height = 260;
        root.Children.Add(positionedLine);

        var positionedCard = CreateCardTabControl(itemCount: 12, iconCount: 3, width: 600, placement: Dock.Right);
        positionedCard.Height = 280;
        root.Children.Add(positionedCard);

        return root;
    }
}
