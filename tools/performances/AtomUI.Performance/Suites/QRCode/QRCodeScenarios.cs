using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using AtomButton = AtomUI.Desktop.Controls.Button;
using AtomFlyout = AtomUI.Desktop.Controls.Flyout;
using AtomListBox = AtomUI.Desktop.Controls.ListBox;
using AtomQRCode = AtomUI.Desktop.Controls.QRCode;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static readonly IImage QRCodeDemoIcon = CreateQRCodeDemoIcon();

    private static IReadOnlyList<PerfScenario> CreateQRCodeScenarios()
    {
        return
        [
            new PerfScenario("QRCode.Basic", _ => CreateQRCode()),
            new PerfScenario("QRCode.WithIcon", _ => CreateQRCode(icon: QRCodeDemoIcon)),
            new PerfScenario("QRCode.Status.Loading", _ => CreateQRCode(status: QRCodeStatus.Loading)),
            new PerfScenario("QRCode.Status.Expired", _ => CreateQRCode(status: QRCodeStatus.Expired)),
            new PerfScenario("QRCode.Status.Scanned", _ => CreateQRCode(status: QRCodeStatus.Scanned)),
            new PerfScenario("QRCode.Status.CustomContent", _ => CreateQRCodeCustomStatusSet()),
            new PerfScenario("QRCode.EmptyValue", _ => CreateQRCode(value: string.Empty)),
            new PerfScenario("QRCode.GalleryShape.QRCodeShowCase", _ => CreateQRCodeShowCaseShape())
        ];
    }

    private static AtomQRCode CreateQRCode(string value = "https://atomui.net",
                                           QRCodeStatus status = QRCodeStatus.Active,
                                           IImage? icon = null,
                                           IBrush? color = null,
                                           IBrush? background = null,
                                           int size = 160)
    {
        return new AtomQRCode
        {
            Value      = value,
            Status     = status,
            Icon       = icon,
            Color      = color,
            Background = background,
            Size       = size,
            IconSize   = Math.Max(16, size / 4)
        };
    }

    private static Control CreateQRCodeCustomStatusSet()
    {
        return CreateQRCodeWrapPanel(
            CreateQRCodeWithCustomLoadingContent(),
            CreateQRCodeWithCustomExpiredContent(),
            CreateQRCodeWithCustomScannedContent());
    }

    private static AtomQRCode CreateQRCodeWithCustomLoadingContent()
    {
        return new AtomQRCode
        {
            Value          = "https://atomui.net",
            Status         = QRCodeStatus.Loading,
            LoadingContent = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Children =
                {
                    new Spin
                    {
                        IsSpinning          = true,
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new AtomTextBlock
                    {
                        Text = "Loading..."
                    }
                }
            }
        };
    }

    private static AtomQRCode CreateQRCodeWithCustomExpiredContent()
    {
        return new AtomQRCode
        {
            Value          = "https://atomui.net",
            Status         = QRCodeStatus.Expired,
            ExpiredContent = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center,
                Children =
                {
                    new AtomTextBlock
                    {
                        Text                = "二维码过期",
                        HorizontalAlignment = HorizontalAlignment.Center
                    },
                    new AtomButton
                    {
                        Content             = "点击刷新",
                        ButtonType          = ButtonType.Link,
                        HorizontalAlignment = HorizontalAlignment.Center
                    }
                }
            }
        };
    }

    private static AtomQRCode CreateQRCodeWithCustomScannedContent()
    {
        return new AtomQRCode
        {
            Value          = "https://atomui.net",
            Status         = QRCodeStatus.Scanned,
            ScannedContent = new AtomTextBlock
            {
                Text                = "已扫描",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment   = VerticalAlignment.Center
            }
        };
    }

    private static Control CreateQRCodeShowCaseShape()
    {
        return new StackPanel
        {
            Spacing = 24,
            Children =
            {
                new StackPanel
                {
                    Children =
                    {
                        CreateQRCode(),
                        new LineEdit
                        {
                            Text   = "https://atomui.net",
                            Margin = new Avalonia.Thickness(0, 20, 0, 0)
                        },
                        new AtomListBox()
                    }
                },
                CreateQRCode(icon: QRCodeDemoIcon),
                CreateQRCodeWrapPanel(
                    CreateQRCode(status: QRCodeStatus.Loading),
                    CreateQRCode(status: QRCodeStatus.Expired),
                    CreateQRCode(status: QRCodeStatus.Scanned)),
                CreateQRCodeCustomStatusSet(),
                CreateQRCode(size: 200, icon: QRCodeDemoIcon),
                CreateQRCodeWrapPanel(
                    CreateQRCode(color: Brushes.Green),
                    CreateQRCode(color: Brushes.DodgerBlue, background: Brushes.WhiteSmoke)),
                new StackPanel
                {
                    Spacing = 10,
                    Children =
                    {
                        CreateQRCode(value: "https://gw.alipayobjects.com/zos/rmsportal/KDpgvguMpGfqaHPjicRK.svg"),
                        new Segmented
                        {
                            ItemsSource = new[]
                            {
                                QRCodeEccLevel.L,
                                QRCodeEccLevel.M,
                                QRCodeEccLevel.Q,
                                QRCodeEccLevel.H
                            }
                        }
                    }
                },
                new FlyoutHost
                {
                    Content = new AtomButton
                    {
                        ButtonType = ButtonType.Primary,
                        Content    = "Hover me"
                    },
                    Flyout = new AtomFlyout
                    {
                        Content = CreateQRCode(isBordered: false)
                    }
                }
            }
        };
    }

    private static AtomQRCode CreateQRCode(bool isBordered)
    {
        var qrCode = CreateQRCode();
        qrCode.IsBordered = isBordered;
        return qrCode;
    }

    private static WrapPanel CreateQRCodeWrapPanel(params Control[] children)
    {
        var panel = new WrapPanel
        {
            ItemSpacing = 20,
            LineSpacing = 20,
            Orientation = Orientation.Horizontal
        };
        foreach (var child in children)
        {
            panel.Children.Add(child);
        }
        return panel;
    }

    private static IImage CreateQRCodeDemoIcon()
    {
        return new DrawingImage(new GeometryDrawing
        {
            Brush    = Brushes.DeepSkyBlue,
            Geometry = new EllipseGeometry(new Avalonia.Rect(0, 0, 24, 24))
        })
        {
            Viewbox = new Avalonia.Rect(0, 0, 24, 24)
        };
    }
}
