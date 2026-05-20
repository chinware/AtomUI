using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomButton = AtomUI.Desktop.Controls.Button;
using AtomRadioButton = AtomUI.Desktop.Controls.RadioButton;
using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static readonly DescriptionsMediaBreakInfo DescriptionSpan2 = new(2);
    private static readonly DescriptionsMediaBreakInfo DescriptionSpan3 = new(3);
    private static readonly DescriptionsMediaBreakInfo DescriptionResponsiveColumns = new(1, 2, 3, 3, 4, 4);
    private static readonly DescriptionsMediaBreakInfo DescriptionResponsiveHalfSpan = new(1, 1, 1, 1, 2, 2);
    private static readonly DescriptionsMediaBreakInfo DescriptionResponsiveFullSpan = new(1, 2, 3, 3, 2, 2);

    private static IReadOnlyList<PerfScenario> CreateDescriptionsScenarios()
    {
        return
        [
            new PerfScenario("Descriptions.Basic5", _ => CreateBasicDescriptions()),
            new PerfScenario("Descriptions.Bordered10", _ => CreateBorderedDescriptions()),
            new PerfScenario("Descriptions.HeaderExtra10", _ => CreateBorderedDescriptions(
                header: "Custom Size",
                extra: new AtomButton
                {
                    ButtonType = ButtonType.Primary,
                    Content    = "Edit"
                })),
            new PerfScenario("Descriptions.Responsive8", _ => CreateResponsiveDescriptions()),
            new PerfScenario("Descriptions.Vertical5", _ => CreateBasicDescriptions(Orientation.Vertical)),
            new PerfScenario("Descriptions.VerticalBordered10", _ => CreateBorderedDescriptions(
                isVertical: true,
                header: null)),
            new PerfScenario("Descriptions.RowFilled4", _ => CreateRowFilledDescriptions()),
            new PerfScenario("Descriptions.GalleryShape.Batch8", _ => CreateDescriptionsGalleryShape())
        ];
    }

    private static Descriptions CreateBasicDescriptions(Orientation layout = Orientation.Horizontal)
    {
        var descriptions = new Descriptions
        {
            Header = "User Info",
            Layout = layout
        };
        AddBasicUserItems(descriptions);
        return descriptions;
    }

    private static Descriptions CreateBorderedDescriptions(object? header = null,
                                                           object? extra = null,
                                                           bool isVertical = false)
    {
        var descriptions = new Descriptions
        {
            Header     = header,
            Extra      = extra,
            IsBordered = true,
            Layout     = isVertical ? Orientation.Vertical : Orientation.Horizontal
        };
        AddProductItems(descriptions);
        return descriptions;
    }

    private static Descriptions CreateResponsiveDescriptions()
    {
        var descriptions = new Descriptions
        {
            IsBordered  = true,
            SizeType    = SizeType.Large,
            Header      = "Responsive Descriptions",
            ColumnInfo  = DescriptionResponsiveColumns
        };
        AddResponsiveItems(descriptions);
        return descriptions;
    }

    private static Descriptions CreateRowFilledDescriptions()
    {
        var descriptions = new Descriptions
        {
            Header     = "User Info",
            IsBordered = true
        };
        descriptions.Items.Add(new DescriptionItem { Label = "UserName", Content = "Zhou Maomao" });
        descriptions.Items.Add(new DescriptionItem { Label = "Live", Content = "Hangzhou, Zhejiang", IsFilled = true });
        descriptions.Items.Add(new DescriptionItem { Label = "Remark", Content = "empty", IsFilled = true });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Address",
            Content = "No. 18, Wantang Road, Xihu District, Hangzhou, Zhejiang, China"
        });
        return descriptions;
    }

    private static Control CreateDescriptionsGalleryShape()
    {
        var root = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 24,
            Width       = 980
        };

        root.Children.Add(CreateBasicDescriptions());
        root.Children.Add(CreateBorderedDescriptions());
        root.Children.Add(new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 30,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing     = 10,
                    Children =
                    {
                        new AtomRadioButton { Content = "Large", IsChecked = true },
                        new AtomRadioButton { Content = "Middle" },
                        new AtomRadioButton { Content = "Small" }
                    }
                },
                CreateBorderedDescriptions(
                    header: "Custom Size",
                    extra: new AtomButton
                    {
                        ButtonType = ButtonType.Primary,
                        Content    = "Edit"
                    }),
                CreateBasicDescriptionsWithExtra()
            }
        });
        root.Children.Add(CreateResponsiveDescriptions());
        root.Children.Add(CreateBasicDescriptions(Orientation.Vertical));
        root.Children.Add(CreateBorderedDescriptions(isVertical: true));
        root.Children.Add(CreateRowFilledDescriptions());

        return root;
    }

    private static Descriptions CreateBasicDescriptionsWithExtra()
    {
        var descriptions = CreateBasicDescriptions();
        descriptions.Header = "Custom Size";
        descriptions.Extra  = new AtomButton
        {
            ButtonType = ButtonType.Primary,
            Content    = "Edit"
        };
        return descriptions;
    }

    private static void AddBasicUserItems(Descriptions descriptions)
    {
        descriptions.Items.Add(new DescriptionItem { Label = "UserName", Content = "Zhou Maomao" });
        descriptions.Items.Add(new DescriptionItem { Label = "Telephone", Content = "1810000000" });
        descriptions.Items.Add(new DescriptionItem { Label = "Live", Content = "Hangzhou, Zhejiang" });
        descriptions.Items.Add(new DescriptionItem { Label = "Remark", Content = "empty" });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Address",
            Content = "No. 18, Wantang Road, Xihu District, Hangzhou, Zhejiang, China"
        });
    }

    private static void AddProductItems(Descriptions descriptions)
    {
        descriptions.Items.Add(new DescriptionItem { Label = "Product", Content = "Cloud Database" });
        descriptions.Items.Add(new DescriptionItem { Label = "Billing Mode", Content = "Prepaid" });
        descriptions.Items.Add(new DescriptionItem { Label = "Automatic Renewal", Content = "YES" });
        descriptions.Items.Add(new DescriptionItem { Label = "Order time", Content = "2018-04-24 18:00:00" });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Usage Time",
            Content = "2019-04-24 18:00:00",
            Span    = DescriptionSpan2
        });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Status",
            Content = "Running",
            Span    = DescriptionSpan3
        });
        descriptions.Items.Add(new DescriptionItem { Label = "Negotiated Amount", Content = "$80.00" });
        descriptions.Items.Add(new DescriptionItem { Label = "Discount", Content = "$20.00" });
        descriptions.Items.Add(new DescriptionItem { Label = "Official Receipts", Content = "$60.00" });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Config Info",
            Content = CreateConfigInfoStack(includeStorage: true)
        });
    }

    private static void AddResponsiveItems(Descriptions descriptions)
    {
        descriptions.Items.Add(new DescriptionItem { Label = "Product", Content = "Cloud Database" });
        descriptions.Items.Add(new DescriptionItem { Label = "Billing", Content = "Prepaid" });
        descriptions.Items.Add(new DescriptionItem { Label = "Time", Content = "18:00:00" });
        descriptions.Items.Add(new DescriptionItem { Label = "Amount", Content = "$80.00" });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Discount",
            Content = "$20.00",
            Span    = DescriptionResponsiveHalfSpan
        });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Official",
            Content = "$60.00",
            Span    = DescriptionResponsiveHalfSpan
        });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Config Info",
            Content = CreateConfigInfoStack(includeStorage: false),
            Span    = DescriptionResponsiveFullSpan
        });
        descriptions.Items.Add(new DescriptionItem
        {
            Label   = "Hardware Info",
            Content = CreateHardwareInfoStack(),
            Span    = DescriptionResponsiveFullSpan
        });
    }

    private static StackPanel CreateConfigInfoStack(bool includeStorage)
    {
        var stack = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = includeStorage ? 5 : 0
        };
        stack.Children.Add(new AvaloniaTextBlock { Text = "Data disk type: MongoDB" });
        stack.Children.Add(new AvaloniaTextBlock { Text = "Database version: 3.4" });
        stack.Children.Add(new AvaloniaTextBlock { Text = "Package: dds.mongo.mid" });
        if (includeStorage)
        {
            stack.Children.Add(new AvaloniaTextBlock { Text = "Storage space: 10 GB" });
            stack.Children.Add(new AvaloniaTextBlock { Text = "Replication factor: 3" });
            stack.Children.Add(new AvaloniaTextBlock { Text = "Region: East China 1" });
        }
        return stack;
    }

    private static StackPanel CreateHardwareInfoStack()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Children =
            {
                new AvaloniaTextBlock { Text = "CPU: 6 Core 3.5 GHz" },
                new AvaloniaTextBlock { Text = "Replication factor: 3" },
                new AvaloniaTextBlock { Text = "Region: East China 1" }
            }
        };
    }
}
