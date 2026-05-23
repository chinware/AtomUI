using AtomUI;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateAdornerLayerScenarios()
    {
        return
        [
            new PerfScenario("AdornerLayer.ScopeAware.LayerLookup16", _ => CreateLayerLookupScenario(16)),
            new PerfScenario("AdornerLayer.ScopeAware.AdornerAttach8", _ => CreateAdornerAttachScenario(8)),
            new PerfScenario("AdornerLayer.ScopeAware.DrawerOpen", _ => CreateDrawerOpenScenario())
        ];
    }

    private static Control CreateLayerLookupScenario(int lookupCount)
    {
        return new VisualLayerManager
        {
            Child = new ScopeAwareLayerLookupHost(lookupCount)
            {
                Width  = 360,
                Height = 120
            }
        };
    }

    private static Control CreateAdornerAttachScenario(int targetCount)
    {
        return new VisualLayerManager
        {
            Child = new ScopeAwareAdornerAttachHost(targetCount)
            {
                Width  = 360,
                Height = 32 * targetCount
            }
        };
    }

    private static Control CreateDrawerOpenScenario()
    {
        return new VisualLayerManager
        {
            Child = new ScopeAwareDrawerOpenHost
            {
                Width  = 640,
                Height = 480
            }
        };
    }

    private sealed class ScopeAwareLayerLookupHost : Panel
    {
        private readonly int _lookupCount;

        public ScopeAwareLayerLookupHost(int lookupCount)
        {
            _lookupCount = lookupCount;
            Children.Add(new Border
            {
                Width      = 320,
                Height     = 80,
                Background = Brushes.Transparent
            });
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            for (var i = 0; i < _lookupCount; i++)
            {
                ScopeAwareAdornerLayer.GetLayer(this);
            }
        }
    }

    private sealed class ScopeAwareAdornerAttachHost : Panel
    {
        private readonly List<Border> _targets;

        public ScopeAwareAdornerAttachHost(int targetCount)
        {
            _targets = new List<Border>(targetCount);
            for (var i = 0; i < targetCount; i++)
            {
                var target = new Border
                {
                    Width      = 280,
                    Height     = 24,
                    Background = Brushes.Transparent
                };
                _targets.Add(target);
                Children.Add(target);
            }
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            foreach (var target in _targets)
            {
                ScopeAwareAdornerLayer.SetAdorner(target, new Border
                {
                    Width      = 16,
                    Height     = 16,
                    Background = Brushes.Transparent
                });
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }

            return new Size(320, Math.Max(1, _targets.Count) * 32);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                Children[i].Arrange(new Rect(0, i * 32, 280, 24));
            }

            return finalSize;
        }
    }

    private sealed class ScopeAwareDrawerOpenHost : Panel
    {
        private readonly Drawer _drawer;

        public ScopeAwareDrawerOpenHost()
        {
            _drawer = new Drawer
            {
                Title      = "Scoped Drawer",
                DialogSize = new Dimension(50, DimensionUnitType.Percentage),
                Content    = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Spacing     = 5,
                    Children =
                    {
                        new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." },
                        new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." },
                        new AtomUI.Desktop.Controls.TextBlock { Text = "Some contents..." }
                    }
                }
            };
            _drawer.SetValue(Drawer.IsMotionEnabledProperty, false, Avalonia.Data.BindingPriority.Animation);
            Children.Add(_drawer);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _drawer.OpenOn = this;
            _drawer.IsOpen = true;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _drawer.Measure(availableSize);
            return new Size(640, 480);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _drawer.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}
