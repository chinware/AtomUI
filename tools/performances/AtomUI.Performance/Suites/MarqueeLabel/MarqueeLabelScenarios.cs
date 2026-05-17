using AtomUI;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomButton = AtomUI.Desktop.Controls.Button;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateMarqueeLabelScenarios()
    {
        return
        [
            new PerfScenario("MarqueeLabel.ShortText", _ => CreateMarqueeLabel("Short text", 360)),
            new PerfScenario("MarqueeLabel.LongText", _ => CreateMarqueeLabel(LongMarqueeText, 180)),
            new PerfScenario("Alert.Default", _ => CreateAlert()),
            new PerfScenario("Alert.ShowIcon", _ => CreateAlert(isShowIcon: true)),
            new PerfScenario("Alert.Description", _ => CreateAlert(
                message: "Success Text",
                description: "Success Description Success Description Success Description")),
            new PerfScenario("Alert.Marquee", _ => CreateAlert(
                message: LongMarqueeText,
                isShowIcon: true,
                isMessageMarqueeEnabled: true)),
            new PerfScenario("Alert.GalleryShape", _ => CreateAlertGalleryShape())
        ];
    }

    private const string LongMarqueeText =
        "I can be a React component, multiple React components, or just some text, Info Description Info Description Info Description Info Description";

    private static MarqueeLabel CreateMarqueeLabel(string text, double width)
    {
        return new MarqueeLabel
        {
            Width = width,
            Text  = text
        };
    }

    private static Alert CreateAlert(
        AlertType type = AlertType.Success,
        string message = "Success Text",
        string? description = null,
        bool isShowIcon = false,
        bool isClosable = false,
        bool isMessageMarqueeEnabled = false,
        Control? extraAction = null)
    {
        return new Alert
        {
            Width                   = 640,
            Type                    = type,
            Message                 = message,
            Description             = description,
            IsShowIcon              = isShowIcon,
            IsClosable              = isClosable,
            IsMessageMarqueeEnabled = isMessageMarqueeEnabled,
            ExtraAction             = extraAction
        };
    }

    private static Control CreateAlertGalleryShape()
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 10
        };

        panel.Children.Add(CreateAlert());

        panel.Children.Add(CreateAlert(AlertType.Success, "Success Text"));
        panel.Children.Add(CreateAlert(AlertType.Info, "Info Text"));
        panel.Children.Add(CreateAlert(AlertType.Warning, "Warning Text"));
        panel.Children.Add(CreateAlert(AlertType.Error, "Error Text"));

        panel.Children.Add(CreateAlert(AlertType.Warning,
            "Warning Text Warning Text Warning Text Warning Text Warning Text Warning TextWarning Text",
            isClosable: true));
        panel.Children.Add(CreateAlert(AlertType.Error,
            "Error Text",
            "Error Description Error Description Error Description Error Description Error Description Error Description",
            isClosable: true));
        panel.Children.Add(CreateAlert(AlertType.Error,
            "Error Text",
            "Error Description Error Description Error Description Error Description Error Description Error Description",
            isClosable: true));

        panel.Children.Add(CreateAlert(AlertType.Success,
            "Success Text",
            "Success Description Success Description Success Description"));
        panel.Children.Add(CreateAlert(AlertType.Info,
            "Info Text",
            "Info Description Info Description Info Description Info Description"));
        panel.Children.Add(CreateAlert(AlertType.Warning,
            "Warning Text",
            "Warning Description Warning Description Warning Description Warning Description"));
        panel.Children.Add(CreateAlert(AlertType.Error,
            "Error Text",
            "Error Description Error Description Error Description Error Description"));

        panel.Children.Add(CreateAlert(AlertType.Success, "Success Tips", isShowIcon: true));
        panel.Children.Add(CreateAlert(AlertType.Info, "Informational Notes", isShowIcon: true));
        panel.Children.Add(CreateAlert(AlertType.Warning, "Warning", isShowIcon: true, isClosable: true));
        panel.Children.Add(CreateAlert(AlertType.Error, "Error", isShowIcon: true));

        panel.Children.Add(CreateAlert(AlertType.Success,
            "Success Tips",
            "Detailed description and advice about successful copywriting.",
            isShowIcon: true));
        panel.Children.Add(CreateAlert(AlertType.Info,
            "Informational Notes",
            "Additional description and information about copywriting.",
            isShowIcon: true));
        panel.Children.Add(CreateAlert(AlertType.Warning,
            "Warning",
            "This is a warning notice about copywriting.",
            isShowIcon: true,
            isClosable: true));
        panel.Children.Add(CreateAlert(AlertType.Error,
            "Error",
            "This is an error message about copywriting.",
            isShowIcon: true));

        panel.Children.Add(CreateAlert(AlertType.Success,
            "Success Tips",
            isShowIcon: true,
            isClosable: true,
            extraAction: new AtomButton { ButtonType = ButtonType.Text, SizeType = SizeType.Small, Content = "UNDO" }));
        panel.Children.Add(CreateAlert(AlertType.Error,
            "Error Text",
            "Error Description Error Description Error Description Error Description",
            isShowIcon: true,
            extraAction: new AtomButton { ButtonType = ButtonType.Default, SizeType = SizeType.Small, IsDanger = true, Content = "Detail" }));
        panel.Children.Add(CreateAlert(AlertType.Warning,
            "Warning Text",
            isClosable: true,
            extraAction: new AtomButton { ButtonType = ButtonType.Text, SizeType = SizeType.Small, Content = "Done" }));
        panel.Children.Add(CreateAlert(AlertType.Info,
            "Info Text",
            "Info Description Info Description Info Description Info Description",
            isClosable: true,
            extraAction: new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing     = 5,
                Children =
                {
                    new AtomButton { ButtonType = ButtonType.Primary, SizeType = SizeType.Small, Content = "Accept" },
                    new AtomButton { SizeType = SizeType.Small, IsDanger = true, IsGhost = true, Content = "Decline" }
                }
            }));

        panel.Children.Add(CreateAlert(AlertType.Warning,
            LongMarqueeText,
            isShowIcon: true,
            isMessageMarqueeEnabled: true));

        return panel;
    }
}
