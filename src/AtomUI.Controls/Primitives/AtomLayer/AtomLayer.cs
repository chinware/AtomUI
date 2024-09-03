using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.VisualTree;

// ReSharper disable SuggestBaseTypeForParameter

namespace AtomUI.Controls.Primitives
{
    public class AtomLayer : Panel
    {
        public static Control? GetTarget(Visual element)
        {
            return element.GetValue(TargetProperty);
        }
        public static void SetTarget(Visual element, Control? value)
        {
            element.SetValue(TargetProperty, value);
        }
        public static readonly AttachedProperty<Control?> TargetProperty = AvaloniaProperty
            .RegisterAttached<AtomLayer, Visual, Control?>("Target");


        
        public static AtomLayer? GetLayer(Visual visual)
        {
            var host = visual.FindAncestorOfType<ScrollContentPresenter>(true)?.Content as Control
                       ?? TopLevel.GetTopLevel(visual);
            
            if (host == null)
            {
                return null;
            }
            
            var layer = host.GetVisualChildren().FirstOrDefault(c => c is AtomLayer) as AtomLayer;
            layer ??= TryInject(host);
            return layer;
        }

        private static AtomLayer TryInject(Control control)
        {
            var layer = new AtomLayer();
            
            if (control.IsLoaded == false)
            {
                control.Loaded += (sender, args) =>
                {
                    InjectCore(control, layer);
                };
            }
            else
            {
                InjectCore(control, layer);
            }

            return layer;
        }

        private static void InjectCore(Control control, AtomLayer layer)
        {
            if (control.GetVisualChildren() is not IList<Visual> visualChildren)
            {
                return;
            }
            if (visualChildren.Any(c => c is AtomLayer))
            {
                return;
            }

            layer.HorizontalAlignment = HorizontalAlignment.Stretch;
            layer.VerticalAlignment   = VerticalAlignment.Stretch;
            layer.InheritanceParent   = control;

            visualChildren.Add(layer);
            control.InvalidateMeasure();
        }
        
        
        
        private AtomLayer()
        {
            
        }

        public void AddAdorner(Visual target, Control adorner)
        {
            if (Children.Contains(adorner))
            {
                return;
            }
            
            SetTarget(target, adorner);
            Children.Add(adorner);
            
            target.AttachedToVisualTree   -= OnTargetOnAttachedToVisualTree;
            target.DetachedFromVisualTree -= OnTargetOnDetachedFromVisualTree;
            target.AttachedToVisualTree   += OnTargetOnAttachedToVisualTree;
            target.DetachedFromVisualTree += OnTargetOnDetachedFromVisualTree;
        }
        
        private void OnTargetOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
        {
            if (Children.Contains(this))
            {
                return;
            }

            Children.Add(this);
        }
        
        private void OnTargetOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
        {
            Children.Remove(this);
        }

        public Control? GetAdorner<T>(Visual target) where T : Control, IAtomAdorner
        {
            var adorner = this.Children.OfType<T>().FirstOrDefault(a => a.Target == target);
            return adorner;
        }
    }
}
