using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;
using AtomScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateScrollViewerScenarios()
    {
        return
        [
            new PerfScenario("ScrollViewer.Default.NoOverflow", _ => CreateAtomScrollViewer(
                width: 260,
                height: 120,
                content: CreateContentBlock(160, 72))),
            new PerfScenario("ScrollViewer.VerticalAuto.Overflow", _ => CreateAtomScrollViewer(
                width: 260,
                height: 120,
                content: CreateContentBlock(220, 900),
                verticalVisibility: ScrollBarVisibility.Auto)),
            new PerfScenario("ScrollViewer.BothAuto.Overflow", _ => CreateAtomScrollViewer(
                width: 260,
                height: 120,
                content: CreateContentBlock(640, 900),
                horizontalVisibility: ScrollBarVisibility.Auto,
                verticalVisibility: ScrollBarVisibility.Auto)),
            new PerfScenario("ScrollViewer.LiteAutoHide.Overflow", _ => CreateAtomScrollViewer(
                width: 260,
                height: 120,
                content: CreateContentBlock(640, 900),
                horizontalVisibility: ScrollBarVisibility.Auto,
                verticalVisibility: ScrollBarVisibility.Auto,
                isLiteMode: true,
                allowAutoHide: true)),
            new PerfScenario("ScrollViewer.NormalNoAutoHide.GalleryPanel", _ => CreateAtomScrollViewer(
                width: 760,
                height: 260,
                content: CreateGalleryPanelLikeContent(),
                horizontalVisibility: ScrollBarVisibility.Disabled,
                verticalVisibility: ScrollBarVisibility.Auto,
                isLiteMode: false,
                allowAutoHide: false)),
            new PerfScenario("ScrollViewer.HorizontalDisabled.VerticalAuto", _ => CreateAtomScrollViewer(
                width: 260,
                height: 120,
                content: CreateContentBlock(640, 900),
                horizontalVisibility: ScrollBarVisibility.Disabled,
                verticalVisibility: ScrollBarVisibility.Auto)),
            new PerfScenario("ScrollViewer.BothDisabled", _ => CreateAtomScrollViewer(
                width: 260,
                height: 120,
                content: CreateContentBlock(640, 900),
                horizontalVisibility: ScrollBarVisibility.Disabled,
                verticalVisibility: ScrollBarVisibility.Disabled)),
            new PerfScenario("ScrollViewer.OverlayScope.Requested", _ => CreateAtomScrollViewer(
                width: 260,
                height: 120,
                content: CreateOverlayRequesterContent(),
                verticalVisibility: ScrollBarVisibility.Auto)),
            new PerfScenario("ScrollViewer.Batch.TextBoxLike", _ => CreateScrollViewerBatch(
                count: 8,
                height: 32,
                isLiteMode: true,
                allowAutoHide: true)),
            new PerfScenario("ScrollViewer.Batch.PopupListLike", _ => CreateScrollViewerBatch(
                count: 4,
                height: 180,
                isLiteMode: true,
                allowAutoHide: true))
        ];
    }

    private static AtomScrollViewer CreateAtomScrollViewer(
        double width,
        double height,
        Control content,
        ScrollBarVisibility horizontalVisibility = ScrollBarVisibility.Disabled,
        ScrollBarVisibility verticalVisibility = ScrollBarVisibility.Auto,
        bool isLiteMode = false,
        bool allowAutoHide = true)
    {
        return new AtomScrollViewer
        {
            Width                         = width,
            Height                        = height,
            HorizontalScrollBarVisibility = horizontalVisibility,
            VerticalScrollBarVisibility   = verticalVisibility,
            IsLiteMode                    = isLiteMode,
            AllowAutoHide                 = allowAutoHide,
            Content                       = content
        };
    }

    private static Control CreateContentBlock(double width, double height)
    {
        return new Border
        {
            Width      = width,
            Height     = height,
            Child      = new TextBlock { Text = "ScrollViewer content" }
        };
    }

    private static Control CreateGalleryPanelLikeContent()
    {
        var panel = new StackPanel
        {
            Spacing = 10
        };
        for (var i = 0; i < 12; i++)
        {
            panel.Children.Add(new Border
            {
                Height = 42,
                Child  = new TextBlock
                {
                    Text              = $"Gallery row {i + 1}",
                    VerticalAlignment = VerticalAlignment.Center
                }
            });
        }
        return panel;
    }

    private static Control CreateOverlayRequesterContent()
    {
        return new StackPanel
        {
            Children =
            {
                new OverlayLayerRequester
                {
                    Width  = 120,
                    Height = 32
                },
                CreateContentBlock(220, 900)
            }
        };
    }

    private static Control CreateScrollViewerBatch(int count, double height, bool isLiteMode, bool allowAutoHide)
    {
        var panel = new StackPanel
        {
            Spacing = 6
        };
        for (var i = 0; i < count; i++)
        {
            panel.Children.Add(CreateAtomScrollViewer(
                width: 260,
                height: height,
                content: CreateContentBlock(640, 360),
                horizontalVisibility: ScrollBarVisibility.Auto,
                verticalVisibility: ScrollBarVisibility.Auto,
                isLiteMode: isLiteMode,
                allowAutoHide: allowAutoHide));
        }
        return panel;
    }

    private sealed class OverlayLayerRequester : Control
    {
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _ = ScopeAwareOverlayLayer.GetLayer(this);
        }
    }
}
