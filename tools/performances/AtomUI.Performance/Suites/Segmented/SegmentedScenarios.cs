using AtomUI.Icons.AntDesign;
using AtomUI.Controls;
using Avalonia.Controls;
using AtomSegmented = AtomUI.Desktop.Controls.Segmented;
using AtomSegmentedItem = AtomUI.Desktop.Controls.SegmentedItem;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSegmentedScenarios()
    {
        return
        [
            new PerfScenario("Segmented.TextOnly5", _ => CreateTextOnlySegmented()),
            new PerfScenario("Segmented.IconOnly2", _ => CreateIconOnlySegmented()),
            new PerfScenario("Segmented.IconAndText3", _ => CreateIconAndTextSegmented()),
            new PerfScenario("Segmented.DisabledMixed5", _ => CreateDisabledMixedSegmented()),
            new PerfScenario("Segmented.Expanding3", _ => CreateExpandingSegmented()),
            new PerfScenario("Segmented.ItemsSource4", _ => CreateItemsSourceSegmented()),
            new PerfScenario("Segmented.HiddenMixed5", _ => CreateHiddenMixedSegmented()),
            new PerfScenario("Segmented.GalleryShape.SegmentedShowCase", _ => CreateSegmentedGalleryShape()),
            new PerfScenario("Segmented.Batch.FlexPanelControls", _ => CreateSegmentedFlexPanelBatch())
        ];
    }

    private static AtomSegmented CreateTextOnlySegmented()
    {
        return CreateSegmented(
            CreateSegmentedItem("Daily"),
            CreateSegmentedItem("Weekly"),
            CreateSegmentedItem("Monthly"),
            CreateSegmentedItem("Quarterly"),
            CreateSegmentedItem("Yearly"));
    }

    private static AtomSegmented CreateIconOnlySegmented()
    {
        return CreateSegmented(
            CreateSegmentedItem(icon: new BarsOutlined()),
            CreateSegmentedItem(icon: new AppstoreOutlined()));
    }

    private static AtomSegmented CreateIconAndTextSegmented()
    {
        return CreateSegmented(
            CreateSegmentedItem("Ava", new BarsOutlined()),
            CreateSegmentedItem("Wechat", new WechatOutlined()),
            CreateSegmentedItem("Windows", new WindowsOutlined()));
    }

    private static AtomSegmented CreateDisabledMixedSegmented()
    {
        return CreateSegmented(
            CreateSegmentedItem("Daily"),
            CreateSegmentedItem("Weekly", isEnabled: false),
            CreateSegmentedItem("Monthly"),
            CreateSegmentedItem("Quarterly", isEnabled: false),
            CreateSegmentedItem("Yearly"));
    }

    private static AtomSegmented CreateExpandingSegmented()
    {
        return CreateSegmented(
            isExpanding: true,
            CreateSegmentedItem("123"),
            CreateSegmentedItem("456"),
            CreateSegmentedItem("longtext-longtext-longtext-longtext"));
    }

    private static AtomSegmented CreateItemsSourceSegmented()
    {
        return new AtomSegmented
        {
            ItemsSource = new[]
            {
                "Low",
                "Middle",
                "High",
                "Ultra"
            }
        };
    }

    private static AtomSegmented CreateHiddenMixedSegmented()
    {
        return CreateSegmented(
            isExpanding: true,
            CreateSegmentedItem("Visible 1"),
            CreateSegmentedItem("Hidden", isVisible: false),
            CreateSegmentedItem("Visible 2"),
            CreateSegmentedItem("Hidden icon", new BarsOutlined(), isVisible: false),
            CreateSegmentedItem("Visible 3"));
    }

    private static Control CreateSegmentedGalleryShape()
    {
        return new StackPanel
        {
            Spacing = 10,
            Children =
            {
                CreateTextOnlySegmented(),
                CreateExpandingSegmented(),
                CreateSegmented(
                    CreateSegmentedItem("Map", isEnabled: false),
                    CreateSegmentedItem("Transit", isEnabled: false),
                    CreateSegmentedItem("Satellite", isEnabled: false)),
                CreateDisabledMixedSegmented(),
                CreateSegmented(
                    sizeType: SizeType.Large,
                    CreateSegmentedItem("Daily"),
                    CreateSegmentedItem("Weekly"),
                    CreateSegmentedItem("Monthly"),
                    CreateSegmentedItem("Quarterly"),
                    CreateSegmentedItem("Yearly")),
                CreateTextOnlySegmented(),
                CreateSegmented(
                    sizeType: SizeType.Small,
                    CreateSegmentedItem("Daily"),
                    CreateSegmentedItem("Weekly"),
                    CreateSegmentedItem("Monthly"),
                    CreateSegmentedItem("Quarterly"),
                    CreateSegmentedItem("Yearly")),
                CreateIconOnlySegmented(),
                CreateSegmented(
                    CreateSegmentedItem("List", new BarsOutlined()),
                    CreateSegmentedItem("Kanban", new AppstoreOutlined())),
                CreateSegmented(
                    CreateSegmentedItem("Ava", new BarsOutlined()),
                    CreateSegmentedItem("Wechat", new WechatOutlined(), isEnabled: false),
                    CreateSegmentedItem("Windows", new WindowsOutlined())),
                CreateSegmented(
                    sizeType: SizeType.Large,
                    CreateSegmentedItem("Ava", new BarsOutlined()),
                    CreateSegmentedItem("Wechat", new WechatOutlined()),
                    CreateSegmentedItem("Windows", new WindowsOutlined()))
            }
        };
    }

    private static Control CreateSegmentedFlexPanelBatch()
    {
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                CreateSegmented(
                    CreateSegmentedItem("flex-start"),
                    CreateSegmentedItem("center"),
                    CreateSegmentedItem("flex-end"),
                    CreateSegmentedItem("space-between"),
                    CreateSegmentedItem("space-around"),
                    CreateSegmentedItem("space-evenly")),
                CreateSegmented(
                    CreateSegmentedItem("flex-start"),
                    CreateSegmentedItem("center"),
                    CreateSegmentedItem("flex-end"),
                    CreateSegmentedItem("stretch")),
                CreateSegmented(
                    CreateSegmentedItem("default"),
                    CreateSegmentedItem("reverse"),
                    CreateSegmentedItem("custom")),
                CreateSegmented(
                    CreateSegmentedItem("auto"),
                    CreateSegmentedItem("absolute"),
                    CreateSegmentedItem("relative"))
            }
        };
    }

    private static AtomSegmented CreateSegmented(
        params AtomSegmentedItem[] items)
    {
        return CreateSegmented(isExpanding: false, sizeType: null, items);
    }

    private static AtomSegmented CreateSegmented(
        bool isExpanding,
        params AtomSegmentedItem[] items)
    {
        return CreateSegmented(isExpanding, sizeType: null, items);
    }

    private static AtomSegmented CreateSegmented(
        SizeType sizeType,
        params AtomSegmentedItem[] items)
    {
        return CreateSegmented(isExpanding: false, sizeType, items);
    }

    private static AtomSegmented CreateSegmented(
        bool isExpanding,
        SizeType? sizeType,
        params AtomSegmentedItem[] items)
    {
        var segmented = new AtomSegmented
        {
            IsExpanding = isExpanding
        };
        if (sizeType is { } value)
        {
            segmented.SizeType = value;
        }
        foreach (var item in items)
        {
            segmented.Items.Add(item);
        }
        return segmented;
    }

    private static AtomSegmentedItem CreateSegmentedItem(
        string? content = null,
        PathIcon? icon = null,
        bool isEnabled = true,
        bool isVisible = true)
    {
        return new AtomSegmentedItem
        {
            Content   = content,
            Icon      = icon,
            IsEnabled = isEnabled,
            IsVisible = isVisible
        };
    }
}
