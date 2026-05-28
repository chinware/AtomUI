using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateWatermarkScenarios()
    {
        return
        [
            new PerfScenario("Watermark.Text.Basic", _ => CreateWatermarkHost()),
            new PerfScenario("Watermark.Text.PendingReplace4", _ => CreateWatermarkPendingReplaceHost(4)),
            new PerfScenario("Watermark.Text.ArrangedReplace4", _ => new VisualLayerManager
            {
                Child = new WatermarkArrangedReplacePanel(4)
            })
        ];
    }

    private static VisualLayerManager CreateWatermarkHost()
    {
        var target = CreateWatermarkTarget();
        Watermark.SetGlyph(target, CreateTextWatermarkGlyph("AtomUI"));

        return new VisualLayerManager
        {
            Child = target
        };
    }

    private static VisualLayerManager CreateWatermarkPendingReplaceHost(int replaceCount)
    {
        var target = CreateWatermarkTarget();
        return new VisualLayerManager
        {
            Child = new WatermarkPendingReplacePanel(target, replaceCount)
        };
    }

    private static Border CreateWatermarkTarget()
    {
        return new Border
        {
            Width      = 420,
            Height     = 180,
            Background = Brushes.Transparent
        };
    }

    private static TextGlyph CreateTextWatermarkGlyph(string text)
    {
        return new TextGlyph
        {
            Text            = text,
            FontSize        = 16,
            Foreground      = Brushes.Black,
            HorizontalSpace = 120,
            VerticalSpace   = 32,
            Opacity         = 0.25,
            Rotate          = -20
        };
    }

    private sealed class WatermarkPendingReplacePanel : Panel
    {
        private readonly Border _target;
        private readonly int    _replaceCount;
        private bool            _queued;

        public WatermarkPendingReplacePanel(Border target, int replaceCount)
        {
            _target       = target;
            _replaceCount = replaceCount;
            Children.Add(_target);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            if (_queued)
            {
                return;
            }

            _queued = true;
            Dispatcher.UIThread.Post(() =>
            {
                for (var i = 0; i < _replaceCount; i++)
                {
                    Watermark.SetGlyph(_target, CreateTextWatermarkGlyph($"AtomUI {i}"));
                }
            });
        }
    }

    private sealed class WatermarkArrangedReplacePanel : Panel
    {
        private readonly Border _target;
        private readonly int    _replaceCount;
        private bool            _replaced;

        public WatermarkArrangedReplacePanel(int replaceCount)
        {
            _target       = CreateWatermarkTarget();
            _replaceCount = replaceCount;
            Children.Add(_target);
            Watermark.SetGlyph(_target, CreateTextWatermarkGlyph("AtomUI 0"));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _target.Measure(availableSize);
            return _target.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _target.Arrange(new Rect(finalSize));
            if (!_replaced && _target.IsArrangeValid)
            {
                _replaced = true;
                for (var i = 0; i < _replaceCount; i++)
                {
                    Watermark.SetGlyph(_target, CreateTextWatermarkGlyph($"AtomUI {i + 1}"));
                }
            }

            return finalSize;
        }
    }
}
