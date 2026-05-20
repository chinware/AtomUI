using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateCollapseScenarios()
    {
        return
        [
            new PerfScenario("Collapse.SingleClosed", _ => CreateCollapse(itemCount: 1)),
            new PerfScenario("Collapse.SingleOpen", _ => CreateCollapse(itemCount: 1, selectedIndexes: new HashSet<int> { 0 })),
            new PerfScenario("Collapse.Basic3Closed", _ => CreateCollapse(itemCount: 3)),
            new PerfScenario("Collapse.Basic3FirstOpen", _ => CreateCollapse(itemCount: 3, selectedIndexes: new HashSet<int> { 0 })),
            new PerfScenario("Collapse.Accordion3Closed", _ => CreateCollapse(itemCount: 3, isAccordion: true)),
            new PerfScenario("Collapse.NoArrow2", _ => CreateNoArrowCollapse()),
            new PerfScenario("Collapse.AddOn3", _ => CreateCollapse(itemCount: 3, hasAddOn: true)),
            new PerfScenario("Collapse.Nested", _ => CreateNestedCollapse()),
            new PerfScenario("Collapse.CustomPadding3", _ => CreateCollapse(
                itemCount: 3,
                itemHeaderPadding: new Avalonia.Thickness(5),
                itemContentPadding: new Avalonia.Thickness(5))),
            new PerfScenario("Collapse.GalleryShape", _ => CreateCollapseGalleryShape())
        ];
    }

    private static Collapse CreateCollapse(
        int itemCount,
        IReadOnlySet<int>? selectedIndexes = null,
        bool isAccordion = false,
        bool isBorderless = false,
        bool isGhostStyle = false,
        bool hasAddOn = false,
        Avalonia.Thickness? itemHeaderPadding = null,
        Avalonia.Thickness? itemContentPadding = null,
        SizeType sizeType = SizeType.Middle,
        CollapseExpandIconPosition expandIconPosition = CollapseExpandIconPosition.Start,
        CollapseTriggerType triggerType = CollapseTriggerType.Header,
        bool isEnabled = true)
    {
        var collapse = new Collapse
        {
            HorizontalAlignment  = HorizontalAlignment.Stretch,
            IsAccordion          = isAccordion,
            IsBorderless         = isBorderless,
            IsGhostStyle         = isGhostStyle,
            SizeType             = sizeType,
            ExpandIconPosition   = expandIconPosition,
            TriggerType          = triggerType,
            IsEnabled            = isEnabled
        };

        if (itemHeaderPadding is { } headerPadding)
        {
            collapse.ItemHeaderPadding = headerPadding;
        }
        if (itemContentPadding is { } contentPadding)
        {
            collapse.ItemContentPadding = contentPadding;
        }

        for (var i = 0; i < itemCount; i++)
        {
            collapse.Items.Add(CreateCollapseItem(
                $"This is panel header {i + 1}",
                selectedIndexes?.Contains(i) == true,
                hasAddOn));
        }
        return collapse;
    }

    private static CollapseItem CreateCollapseItem(string header, bool isSelected = false, bool hasAddOn = false)
    {
        var item = new CollapseItem
        {
            Header     = header,
            IsSelected = isSelected,
            Content    = CreateCollapseText()
        };
        if (hasAddOn)
        {
            item.AddOnContent = new SettingOutlined();
        }
        return item;
    }

    private static AtomTextBlock CreateCollapseText()
    {
        return new AtomTextBlock
        {
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Text = "A dog is a type of domesticated animal. Known for its loyalty and faithfulness, " +
                   "it can be found as a welcome guest in many households across the world."
        };
    }

    private static Collapse CreateNoArrowCollapse()
    {
        var collapse = CreateCollapse(itemCount: 1);
        collapse.Items.Add(new CollapseItem
        {
            Header           = "This is panel header 2",
            IsShowExpandIcon = false,
            Content          = CreateCollapseText()
        });
        return collapse;
    }

    private static Collapse CreateNestedCollapse()
    {
        var nested = CreateCollapse(itemCount: 1);
        return new Collapse
        {
            Items =
            {
                new CollapseItem
                {
                    Header  = "This is panel header 1",
                    Content = nested
                },
                CreateCollapseItem("This is panel header 2"),
                CreateCollapseItem("This is panel header 3")
            }
        };
    }

    private static Control CreateCollapseGalleryShape()
    {
        return new StackPanel
        {
            Spacing = 20,
            Width   = 980,
            Children =
            {
                CreateCollapse(itemCount: 3),
                CreateCollapseSizeExamples(),
                CreateCollapse(itemCount: 3, isAccordion: true),
                CreateNestedCollapse(),
                CreateCollapse(itemCount: 3, isBorderless: true),
                CreateNoArrowCollapse(),
                CreateExpandIconPositionExample(),
                CreateCollapse(itemCount: 3, isGhostStyle: true),
                CreateCollapsibleExamples(),
                new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Spacing     = 20,
                    Children =
                    {
                        CreateCollapse(
                            itemCount: 3,
                            itemHeaderPadding: new Avalonia.Thickness(5),
                            itemContentPadding: new Avalonia.Thickness(5)),
                        CreateCollapse(
                            itemCount: 3,
                            itemHeaderPadding: new Avalonia.Thickness(0),
                            itemContentPadding: new Avalonia.Thickness(0),
                            isGhostStyle: true)
                    }
                }
            }
        };
    }

    private static Control CreateCollapseSizeExamples()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 20,
            Children =
            {
                CreateCollapse(itemCount: 1, sizeType: SizeType.Middle),
                CreateCollapse(itemCount: 1, sizeType: SizeType.Small),
                CreateCollapse(itemCount: 1, sizeType: SizeType.Large)
            }
        };
    }

    private static Control CreateExpandIconPositionExample()
    {
        var positionOptions = new OptionButtonGroup
        {
            ButtonStyle = OptionButtonStyle.Outline
        };
        positionOptions.Items.Add(new OptionButton { Content = "Start", IsChecked = true });
        positionOptions.Items.Add(new OptionButton { Content = "End" });

        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 20,
            Children =
            {
                CreateCollapse(
                    itemCount: 3,
                    hasAddOn: true,
                    expandIconPosition: CollapseExpandIconPosition.Start),
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing     = 5,
                    Children =
                    {
                        new AtomTextBlock
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            Text              = "Expand Icon Position:"
                        },
                        positionOptions
                    }
                }
            }
        };
    }

    private static Control CreateCollapsibleExamples()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 10,
            Children =
            {
                CreateCollapse(itemCount: 1),
                CreateCollapse(itemCount: 1, triggerType: CollapseTriggerType.Icon),
                CreateCollapse(itemCount: 1, isEnabled: false),
                CreateCollapse(itemCount: 1, selectedIndexes: new HashSet<int> { 0 }, isEnabled: false)
            }
        };
    }
}
