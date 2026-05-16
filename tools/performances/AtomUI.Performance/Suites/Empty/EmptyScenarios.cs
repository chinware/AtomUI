using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;

namespace AtomUI.Performance;

internal static partial class Program
{
    private const string EmptyInlineSvgSource =
        """
        <svg width="64" height="41" viewBox="0 0 64 41" xmlns="http://www.w3.org/2000/svg">
          <ellipse fill="#f5f5f5" cx="32" cy="33" rx="32" ry="7" />
          <path d="M9 13h46v18H9z" fill="#fafafa" stroke="#d9d9d9" />
        </svg>
        """;

    private static IReadOnlyList<PerfScenario> CreateEmptyScenarios()
    {
        return
        [
            new PerfScenario("Empty.DefaultPreset", _ => new Empty { PresetImage = PresetEmptyImage.Default }),
            new PerfScenario("Empty.Simple.Small", _ => new Empty { PresetImage = PresetEmptyImage.Simple, SizeType = SizeType.Small }),
            new PerfScenario("Empty.Simple.Middle", _ => new Empty { PresetImage = PresetEmptyImage.Simple, SizeType = SizeType.Middle }),
            new PerfScenario("Empty.Simple.Large", _ => new Empty { PresetImage = PresetEmptyImage.Simple, SizeType = SizeType.Large }),
            new PerfScenario("Empty.ImagePath", _ => new Empty { ImagePath = GetEmptySvgPath(), SizeType = SizeType.Large }),
            new PerfScenario("Empty.ImageSource", _ => new Empty { ImageSource = EmptyInlineSvgSource }),
            new PerfScenario("Empty.NoDescription", _ => new Empty { PresetImage = PresetEmptyImage.Default, IsDescriptionVisible = false }),
            new PerfScenario("Empty.DescriptionOnly", _ => new Empty { Description = "No data" }),
            new PerfScenario("Empty.GalleryShape", _ => CreateEmptyGalleryShape())
        ];
    }

    private static Control CreateEmptyGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 12
        };

        root.Children.Add(new Empty { PresetImage = PresetEmptyImage.Default });
        root.Children.Add(CreateEmptyRow(
            new Empty { PresetImage = PresetEmptyImage.Simple, SizeType = SizeType.Small },
            new Empty { PresetImage = PresetEmptyImage.Simple, SizeType = SizeType.Middle },
            new Empty { PresetImage = PresetEmptyImage.Simple, SizeType = SizeType.Large }));
        root.Children.Add(new StackPanel
        {
            Spacing = 10,
            Children =
            {
                new Empty
                {
                    ImagePath   = GetEmptySvgPath(),
                    SizeType    = SizeType.Large,
                    Description = "Customize Description"
                },
                new AtomUI.Desktop.Controls.Button
                {
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    ButtonType          = ButtonType.Primary,
                    Content             = "Create Now"
                }
            }
        });
        root.Children.Add(new Empty { PresetImage = PresetEmptyImage.Default, IsDescriptionVisible = false });

        return root;
    }

    private static StackPanel CreateEmptyRow(params Control[] controls)
    {
        var row = new StackPanel
        {
            Orientation = Avalonia.Layout.Orientation.Horizontal,
            Spacing     = 10
        };
        foreach (var control in controls)
        {
            row.Children.Add(control);
        }

        return row;
    }

    private static string GetEmptySvgPath()
    {
        return Path.GetFullPath("controlgallery/AtomUIGallery/Assets/EmptyShowCase/empty.svg");
    }
}
