using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;

namespace AtomUI.Performance;

using AvaScrollViewer = Avalonia.Controls.ScrollViewer;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateFloatButtonScenarios()
    {
        return
        [
            new PerfScenario("FloatButtonHost.Default", _ => CreateFloatButtonOverlay(new FloatButtonHost())),
            new PerfScenario("FloatButtonHost.PrimaryIcon", _ => CreateFloatButtonOverlay(new FloatButtonHost
            {
                ButtonType = FloatButtonType.Primary,
                Icon       = new QuestionCircleOutlined()
            })),
            new PerfScenario("FloatButtonHost.SquareDescription", _ => CreateFloatButtonOverlay(new FloatButtonHost
            {
                Shape       = FloatButtonShape.Square,
                Icon        = new FileTextOutlined(),
                Description = "HELP INFO"
            })),
            new PerfScenario("FloatButtonHost.DotBadge", _ => CreateFloatButtonOverlay(new FloatButtonHost
            {
                IsBadgeEnabled = true,
                IsDotBadge     = true
            })),
            new PerfScenario("FloatButtonGroupHost.DefaultCircle", _ => CreateFloatButtonOverlay(CreateDefaultGroupHost(FloatButtonShape.Circle))),
            new PerfScenario("FloatButtonGroupHost.DefaultSquare", _ => CreateFloatButtonOverlay(CreateDefaultGroupHost(FloatButtonShape.Square))),
            new PerfScenario("FloatButtonGroupHost.TriggerClickClosed", _ => CreateFloatButtonOverlay(CreateTriggerGroupHost(FloatButtonGroupTrigger.Click, false))),
            new PerfScenario("FloatButtonGroupHost.TriggerHoverClosed", _ => CreateFloatButtonOverlay(CreateTriggerGroupHost(FloatButtonGroupTrigger.Hover, false))),
            new PerfScenario("FloatButtonGroupHost.TriggerClickOpen", _ => CreateFloatButtonOverlay(CreateTriggerGroupHost(FloatButtonGroupTrigger.Click, true))),
            new PerfScenario("BackTopFloatButtonHost.Default", _ => CreateBackTopScenario()),
            new PerfScenario("FloatButton.GalleryShape", _ => CreateFloatButtonGalleryShape())
        ];
    }

    private static Control CreateFloatButtonOverlay(Control child)
    {
        return new ScopeAwareOverlayLayerPanel
        {
            Width    = 360,
            Height   = 260,
            Children =
            {
                new Panel
                {
                    Width    = 360,
                    Height   = 260,
                    Children = { child }
                }
            }
        };
    }

    private static FloatButtonGroupHost CreateDefaultGroupHost(FloatButtonShape shape)
    {
        return new FloatButtonGroupHost
        {
            Shape    = shape,
            Children =
            {
                new FloatButton { Icon = new QuestionCircleOutlined() },
                new FloatButton(),
                new FloatButton { Icon = new SyncOutlined() },
                new BackTopFloatButton()
            }
        };
    }

    private static FloatButtonGroupHost CreateTriggerGroupHost(FloatButtonGroupTrigger trigger, bool isOpen)
    {
        return new FloatButtonGroupHost
        {
            Shape      = FloatButtonShape.Circle,
            Trigger    = trigger,
            IsOpen     = isOpen,
            Icon       = new CustomerServiceOutlined(),
            ButtonType = FloatButtonType.Primary,
            Children =
            {
                new FloatButton(),
                new FloatButton { Icon = new CommentOutlined() }
            }
        };
    }

    private static Control CreateBackTopScenario()
    {
        return new ScopeAwareOverlayLayerPanel
        {
            Width    = 360,
            Height   = 260,
            Children =
            {
                new AvaScrollViewer
                {
                    Width   = 360,
                    Height  = 260,
                    Content = new Panel
                    {
                        Height   = 800,
                        Children = { new BackTopFloatButtonHost() }
                    }
                }
            }
        };
    }

    private static Control CreateFloatButtonGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 8
        };

        root.Children.Add(CreateFloatButtonOverlay(new FloatButtonHost()));
        root.Children.Add(CreateFloatButtonOverlay(new FloatButtonHost
        {
            ButtonType   = FloatButtonType.Primary,
            Icon         = new CustomerServiceOutlined(),
            FloatOffsetX = 80
        }));
        root.Children.Add(CreateFloatButtonOverlay(new FloatButtonHost
        {
            Shape       = FloatButtonShape.Square,
            Icon        = new FileTextOutlined(),
            Description = "HELP INFO"
        }));
        root.Children.Add(CreateFloatButtonOverlay(CreateDefaultGroupHost(FloatButtonShape.Square)));
        root.Children.Add(CreateFloatButtonOverlay(CreateDefaultGroupHost(FloatButtonShape.Circle)));
        root.Children.Add(CreateFloatButtonOverlay(CreateTriggerGroupHost(FloatButtonGroupTrigger.Hover, false)));
        root.Children.Add(CreateFloatButtonOverlay(CreateTriggerGroupHost(FloatButtonGroupTrigger.Click, false)));
        root.Children.Add(CreateFloatButtonOverlay(new FloatButtonHost
        {
            FloatOffsetX    = 145,
            IsBadgeEnabled = true,
            IsDotBadge     = true
        }));
        root.Children.Add(CreateBackTopScenario());

        return root;
    }
}
