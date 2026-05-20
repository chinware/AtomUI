using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomSpin = AtomUI.Desktop.Controls.Spin;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateSpinScenarios()
    {
        return
        [
            new PerfScenario("SpinIndicator.BuiltIn.Middle", _ => CreateBuiltInSpinIndicator()),
            new PerfScenario("SpinIndicator.CustomIcon.Middle", _ => CreateCustomSpinIndicator()),
            new PerfScenario("Spin.Content.NotSpinning", _ => CreateContentSpin(isSpinning: false)),
            new PerfScenario("Spin.Content.Spinning.Tip", _ => CreateContentSpin(isSpinning: true, isTipVisible: true)),
            new PerfScenario("Spin.Content.Spinning.BlurMask", _ => CreateContentSpin(
                isSpinning: true,
                isTipVisible: true,
                isMaskBlurEnabled: true)),
            new PerfScenario("Spin.GalleryShape", _ => CreateSpinGalleryShape())
        ];
    }

    private static SpinIndicator CreateBuiltInSpinIndicator()
    {
        return new SpinIndicator
        {
            SizeType          = SizeType.Middle,
            VerticalAlignment = VerticalAlignment.Center
        };
    }

    private static SpinIndicator CreateCustomSpinIndicator()
    {
        return new SpinIndicator
        {
            SizeType          = SizeType.Middle,
            VerticalAlignment = VerticalAlignment.Center,
            CustomIndicator  = new LoadingOutlined
            {
                StrokeBrush = Brushes.DodgerBlue
            }
        };
    }

    private static AtomSpin CreateContentSpin(bool isSpinning, bool isTipVisible = false, bool isMaskBlurEnabled = false)
    {
        return new AtomSpin
        {
            Width             = 220,
            Height            = 120,
            IsSpinning        = isSpinning,
            IsTipVisible      = isTipVisible,
            Tip               = isTipVisible ? "Loading..." : null,
            IsMaskBlurEnabled = isMaskBlurEnabled,
            Content           = new Border
            {
                Width      = 220,
                Height     = 120,
                Background = new SolidColorBrush(Color.FromArgb(13, 0, 0, 0))
            }
        };
    }

    private static Control CreateSpinGalleryShape()
    {
        return new StackPanel
        {
            Spacing  = 10,
            Children =
            {
                new SpinIndicator(),
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing     = 10,
                    Children =
                    {
                        new SpinIndicator { SizeType = SizeType.Small },
                        new SpinIndicator { SizeType = SizeType.Middle },
                        new SpinIndicator { SizeType = SizeType.Large }
                    }
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing     = 10,
                    Children =
                    {
                        new SpinIndicator
                        {
                            SizeType         = SizeType.Small,
                            CustomIndicator = new LoadingOutlined { StrokeBrush = Brushes.DodgerBlue }
                        },
                        new SpinIndicator
                        {
                            SizeType         = SizeType.Middle,
                            CustomIndicator = new LoadingOutlined { StrokeBrush = Brushes.DodgerBlue }
                        },
                        new SpinIndicator
                        {
                            SizeType         = SizeType.Large,
                            CustomIndicator = new LoadingOutlined { StrokeBrush = Brushes.DodgerBlue }
                        }
                    }
                },
                new AtomSpin
                {
                    IsSpinning   = true,
                    IsTipVisible = true,
                    Tip          = "Loading...",
                    Content      = new Border
                    {
                        Width      = 100,
                        Height     = 100,
                        Background = new SolidColorBrush(Color.FromArgb(13, 0, 0, 0))
                    }
                },
                new AtomSpin
                {
                    IsSpinning        = false,
                    IsTipVisible      = true,
                    Tip               = "Loading...",
                    IsMaskBlurEnabled = true,
                    Content           = new AtomTextBlock
                    {
                        Text = "Embedded loading content"
                    }
                }
            }
        };
    }
}
