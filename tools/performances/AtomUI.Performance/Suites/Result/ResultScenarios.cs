using AtomUI.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomButton = AtomUI.Desktop.Controls.Button;
using AtomResult = AtomUI.Desktop.Controls.Result;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateResultScenarios()
    {
        return
        [
            new PerfScenario("Result.Status.Success", _ => CreateResult(ResultStatus.Success, hasSubHeader: true, hasExtra: true)),
            new PerfScenario("Result.Status.Info", _ => CreateResult(ResultStatus.Info, hasExtra: true)),
            new PerfScenario("Result.Status.Warning", _ => CreateResult(ResultStatus.Warning, hasExtra: true)),
            new PerfScenario("Result.Status.Error", _ => CreateResult(ResultStatus.Error, hasSubHeader: true, hasExtra: true, hasContent: true)),
            new PerfScenario("Result.ErrorCode.403", _ => CreateResult(ResultStatus.ErrorCode403, hasSubHeader: true, hasExtra: true)),
            new PerfScenario("Result.ErrorCode.404", _ => CreateResult(ResultStatus.ErrorCode404, hasSubHeader: true, hasExtra: true)),
            new PerfScenario("Result.ErrorCode.500", _ => CreateResult(ResultStatus.ErrorCode500, hasSubHeader: true, hasExtra: true)),
            new PerfScenario("Result.CustomIcon", _ => CreateResult(ResultStatus.Info, hasExtra: true, customIcon: new SmileOutlined())),
            new PerfScenario("Result.GalleryShape.ResultShowCase", _ => CreateResultGalleryShape())
        ];
    }

    private static AtomResult CreateResult(ResultStatus status,
                                           bool hasSubHeader = false,
                                           bool hasExtra = false,
                                           bool hasContent = false,
                                           PathIcon? customIcon = null)
    {
        var result = new AtomResult
        {
            Status = status,
            Header = status switch
            {
                ResultStatus.ErrorCode403 => "403",
                ResultStatus.ErrorCode404 => "404",
                ResultStatus.ErrorCode500 => "500",
                ResultStatus.Error => "Submission Failed",
                _ => "Successfully Purchased Cloud Server ECS!"
            },
            Icon = customIcon
        };

        if (hasSubHeader)
        {
            result.SubHeader = status switch
            {
                ResultStatus.ErrorCode403 => "Sorry, you are not authorized to access this page.",
                ResultStatus.ErrorCode404 => "Sorry, the page you visited does not exist.",
                ResultStatus.ErrorCode500 => "Sorry, something went wrong.",
                ResultStatus.Error => "Please check and modify the following information before resubmitting.",
                _ => "Order number: 2017182818828182881 Cloud server configuration takes 1-5 minutes, please wait."
            };
        }

        if (hasExtra)
        {
            result.Extra = CreateResultExtra(status is ResultStatus.Success or ResultStatus.Error);
        }

        if (hasContent)
        {
            result.Content = CreateResultErrorContent();
        }

        return result;
    }

    private static Control CreateResultExtra(bool twoButtons)
    {
        if (!twoButtons)
        {
            return new AtomButton
            {
                ButtonType = AtomUI.Desktop.Controls.ButtonType.Primary,
                Content    = "Go Console"
            };
        }

        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 10,
            Children =
            {
                new AtomButton
                {
                    ButtonType = AtomUI.Desktop.Controls.ButtonType.Primary,
                    Content    = "Go Console"
                },
                new AtomButton
                {
                    Content = "Buy Again"
                }
            }
        };
    }

    private static Control CreateResultErrorContent()
    {
        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock
                {
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    FontSize   = 16,
                    Text       = "The content you submitted has the following error:"
                },
                CreateResultErrorRow("Your account has been frozen.", "Thaw immediately >"),
                CreateResultErrorRow("Your account is not yet eligible to apply.", "Apply Unlock >")
            }
        };
    }

    private static Control CreateResultErrorRow(string text, string action)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing     = 8,
            Children =
            {
                new CloseCircleOutlined(),
                new TextBlock
                {
                    Text = text
                },
                new TextBlock
                {
                    Text = action
                }
            }
        };
    }

    private static Control CreateResultGalleryShape()
    {
        return new StackPanel
        {
            Spacing = 24,
            Children =
            {
                CreateResult(ResultStatus.Success, hasSubHeader: true, hasExtra: true),
                CreateResult(ResultStatus.Info, hasExtra: true),
                CreateResult(ResultStatus.Warning, hasExtra: true),
                CreateResult(ResultStatus.ErrorCode403, hasSubHeader: true, hasExtra: true),
                CreateResult(ResultStatus.ErrorCode404, hasSubHeader: true, hasExtra: true),
                CreateResult(ResultStatus.ErrorCode500, hasSubHeader: true, hasExtra: true),
                CreateResult(ResultStatus.Error, hasSubHeader: true, hasExtra: true, hasContent: true),
                CreateResult(ResultStatus.Info, hasExtra: true, customIcon: new SmileOutlined())
            }
        };
    }
}
