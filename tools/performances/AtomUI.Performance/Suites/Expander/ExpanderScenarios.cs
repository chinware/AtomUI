using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomExpander = AtomUI.Desktop.Controls.Expander;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateExpanderScenarios()
    {
        return
        [
            new PerfScenario("Expander.Closed.Basic", _ => CreateExpander()),
            new PerfScenario("Expander.Closed.NoArrow", _ => CreateExpander(isShowExpandIcon: false)),
            new PerfScenario("Expander.Closed.WithAddOn", _ => CreateExpander(hasAddOn: true)),
            new PerfScenario("Expander.Expanded.Basic", _ => CreateExpander(isExpanded: true)),
            new PerfScenario("Expander.Direction.LeftClosed", _ => CreateExpander(expandDirection: ExpandDirection.Left)),
            new PerfScenario("Expander.GalleryShape", _ => CreateExpanderGalleryShape())
        ];
    }

    private static AtomExpander CreateExpander(
        bool isExpanded = false,
        bool isShowExpandIcon = true,
        bool hasAddOn = false,
        bool isBorderless = false,
        bool isGhostStyle = false,
        SizeType sizeType = SizeType.Middle,
        ExpandDirection expandDirection = ExpandDirection.Down,
        ExpanderIconPosition expandIconPosition = ExpanderIconPosition.Start,
        ExpanderTriggerType triggerType = ExpanderTriggerType.Header,
        bool isEnabled = true,
        Avalonia.Thickness? headerPadding = null,
        Avalonia.Thickness? contentPadding = null)
    {
        var expander = new AtomExpander
        {
            Header             = "This is panel header 1",
            Content            = CreateExpanderText(),
            IsExpanded         = isExpanded,
            IsShowExpandIcon   = isShowExpandIcon,
            IsBorderless       = isBorderless,
            IsGhostStyle       = isGhostStyle,
            SizeType           = sizeType,
            ExpandDirection    = expandDirection,
            ExpandIconPosition = expandIconPosition,
            TriggerType        = triggerType,
            IsEnabled          = isEnabled
        };

        if (hasAddOn)
        {
            expander.AddOnContent = new SettingOutlined();
        }
        if (headerPadding is { } resolvedHeaderPadding)
        {
            expander.HeaderPadding = resolvedHeaderPadding;
        }
        if (contentPadding is { } resolvedContentPadding)
        {
            expander.ContentPadding = resolvedContentPadding;
        }
        return expander;
    }

    private static AtomTextBlock CreateExpanderText()
    {
        return new AtomTextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Text = "A dog is a type of domesticated animal. Known for its loyalty and faithfulness, " +
                   "it can be found as a welcome guest in many households across the world."
        };
    }

    private static Control CreateExpanderGalleryShape()
    {
        return new StackPanel
        {
            Spacing = 20,
            Width   = 980,
            Children =
            {
                CreateExpander(),
                CreateExpanderSizeExamples(),
                CreateExpanderDirectionExample(),
                new AtomExpander
                {
                    Header = "This is panel header 1",
                    Content = CreateExpander()
                },
                CreateExpander(isBorderless: true),
                CreateExpander(isShowExpandIcon: false),
                CreateExpander(hasAddOn: true),
                CreateExpander(isGhostStyle: true),
                CreateExpanderCollapsibleExamples(),
                new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Spacing     = 20,
                    Children =
                    {
                        CreateExpander(headerPadding: new Avalonia.Thickness(5), contentPadding: new Avalonia.Thickness(5)),
                        CreateExpander(
                            isGhostStyle: true,
                            headerPadding: new Avalonia.Thickness(5),
                            contentPadding: new Avalonia.Thickness(5))
                    }
                }
            }
        };
    }

    private static Control CreateExpanderSizeExamples()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 20,
            Children =
            {
                CreateExpander(sizeType: SizeType.Middle),
                CreateExpander(sizeType: SizeType.Small),
                CreateExpander(sizeType: SizeType.Large)
            }
        };
    }

    private static Control CreateExpanderDirectionExample()
    {
        return new DockPanel
        {
            Height = 300,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing     = 5,
                    Children =
                    {
                        new AtomTextBlock
                        {
                            Text              = "Expand direction:",
                            VerticalAlignment = VerticalAlignment.Center
                        },
                        new OptionButtonGroup
                        {
                            ButtonStyle = OptionButtonStyle.Outline,
                            Items =
                            {
                                new OptionButton { Content = "Down", IsChecked = true },
                                new OptionButton { Content = "Up" },
                                new OptionButton { Content = "Left" },
                                new OptionButton { Content = "Right" }
                            }
                        }
                    }
                },
                new Panel
                {
                    Margin = new Avalonia.Thickness(0, 20, 0, 0),
                    Children =
                    {
                        CreateExpander()
                    }
                }
            }
        };
    }

    private static Control CreateExpanderCollapsibleExamples()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 10,
            Children =
            {
                CreateExpander(),
                CreateExpander(triggerType: ExpanderTriggerType.Icon),
                CreateExpander(isEnabled: false),
                CreateExpander(isEnabled: false, isExpanded: true)
            }
        };
    }
}
