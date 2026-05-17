using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomGroupBox = AtomUI.Desktop.Controls.GroupBox;
using AtomGroupBoxTitlePosition = AtomUI.Desktop.Controls.GroupBoxTitlePosition;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateGroupBoxScenarios()
    {
        return
        [
            new PerfScenario("GroupBox.Basic", _ => CreateGroupBox()),
            new PerfScenario("GroupBox.NoHeader", _ => CreateGroupBox(headerTitle: null)),
            new PerfScenario("GroupBox.WithIcon", _ => CreateGroupBox(headerIcon: new GithubOutlined())),
            new PerfScenario("GroupBox.HeaderCenter", _ => CreateGroupBox(position: AtomGroupBoxTitlePosition.Center)),
            new PerfScenario("GroupBox.HeaderRight", _ => CreateGroupBox(position: AtomGroupBoxTitlePosition.Right)),
            new PerfScenario("GroupBox.HeaderStyle", _ => CreateGroupBox(
                position: AtomGroupBoxTitlePosition.Center,
                headerTitleColor: Brushes.Coral,
                fontStyle: FontStyle.Oblique,
                fontWeight: FontWeight.Medium)),
            new PerfScenario("GroupBox.GalleryShape", _ => CreateGroupBoxGalleryShape())
        ];
    }

    private static AtomGroupBox CreateGroupBox(
        string? headerTitle = "Title Info",
        PathIcon? headerIcon = null,
        AtomGroupBoxTitlePosition position = AtomGroupBoxTitlePosition.Left,
        IBrush? headerTitleColor = null,
        FontStyle fontStyle = FontStyle.Normal,
        FontWeight? fontWeight = null,
        double height = 100)
    {
        return new AtomGroupBox
        {
            HeaderTitle         = headerTitle,
            HeaderIcon          = headerIcon,
            HeaderTitlePosition = position,
            HeaderTitleColor    = headerTitleColor,
            HeaderFontStyle     = fontStyle,
            HeaderFontWeight    = fontWeight ?? FontWeight.Normal,
            Width               = 980,
            Content             = CreateGroupBoxContent(height)
        };
    }

    private static Control CreateGroupBoxContent(double height)
    {
        return new Panel
        {
            Height = height,
            Children =
            {
                new AtomTextBlock
                {
                    Text                = "Content of group box",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment   = VerticalAlignment.Center
                }
            }
        };
    }

    private static Control CreateGroupBoxGalleryShape()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 20,
            Width       = 980,
            Children =
            {
                CreateGroupBox(),
                new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Spacing     = 10,
                    Children =
                    {
                        CreateGroupBox(height: 40),
                        CreateGroupBox(position: AtomGroupBoxTitlePosition.Center, height: 40),
                        CreateGroupBox(position: AtomGroupBoxTitlePosition.Right, height: 40)
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Spacing     = 10,
                    Children =
                    {
                        CreateGroupBox(fontStyle: FontStyle.Italic, height: 40),
                        CreateGroupBox(position: AtomGroupBoxTitlePosition.Center, fontWeight: FontWeight.Bold, height: 40),
                        CreateGroupBox(position: AtomGroupBoxTitlePosition.Right, fontStyle: FontStyle.Oblique, height: 40),
                        CreateGroupBox(
                            position: AtomGroupBoxTitlePosition.Center,
                            headerTitleColor: Brushes.Coral,
                            fontStyle: FontStyle.Oblique,
                            fontWeight: FontWeight.Medium,
                            height: 40)
                    }
                },
                CreateGroupBox(headerIcon: new GithubOutlined())
            }
        };
    }
}
