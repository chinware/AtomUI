using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

// ReSharper disable SuggestBaseTypeForParameter

namespace AtomUI.Controls.Primitives
{
    public class AtomLayer : Canvas
    {
        #region Static

        public static AtomLayer? GetLayer(Visual target)
        {
            return target.GetLayer();
        }

        public static Visual GetTarget(Control adorner)
        {
            return adorner.GetValue(TargetProperty);
        }
        private static void SetTarget(Control adorner, Visual value)
        {
            adorner.SetValue(TargetProperty, value);
        }
        public static readonly AttachedProperty<Visual> TargetProperty = AvaloniaProperty
            .RegisterAttached<AtomLayer, Control, Visual>("Target");

        public static Rect GetTargetRect(Control adorner)
        {
            return adorner.GetValue(TargetRectProperty);
        }
        private static void SetTargetRect(Control adorner, Rect value)
        {
            adorner.SetValue(TargetRectProperty, value);
        }
        public static readonly AttachedProperty<Rect> TargetRectProperty = AvaloniaProperty
            .RegisterAttached<AtomLayer, Control, Rect>("TargetRect");
        
        #endregion



        private readonly IList<WeakReference<Control>> _detachedAdorners = new List<WeakReference<Control>>();
        
        internal Visual? ParentHost { get; set; }
        
        internal AtomLayer() { }
        
        public T? GetAdorner<T>(Visual target) where T : Control
        {
            return this.Children.OfType<T>().FirstOrDefault(a => GetTarget(a) == target);
        }

        public IEnumerable<Control> GetAdorners(Visual target)
        {
            return this.Children.Where(v => GetTarget(v) == target).ToList();
        }

        public void AddAdorner(Visual target, Control adorner)
        {
            if (Children.Contains(adorner))
            {
                return;
            }
            
            target.PropertyChanged -= TargetOnPropertyChanged;
            target.PropertyChanged += TargetOnPropertyChanged;
            
            target.AttachedToVisualTree   -= OnTargetOnAttachedToVisualTree;
            target.DetachedFromVisualTree -= OnTargetOnDetachedFromVisualTree;
            target.AttachedToVisualTree   += OnTargetOnAttachedToVisualTree;
            target.DetachedFromVisualTree += OnTargetOnDetachedFromVisualTree;
            
            SetTarget(adorner, target);
            Children.Add(adorner);
            Locate(target, adorner);
        }

        private void TargetOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property != BoundsProperty)
            {
                return;
            }
            
            if (sender is not Visual target)
            {
                return;
            }
            
            // Child element's bounds will be updated first.
            Dispatcher.UIThread.Post(() =>
            {
                foreach (var adorner in GetAdorners(target))
                {
                    Locate(target, adorner);
                }    
            }, DispatcherPriority.Send);
        }

        private void Locate(Visual target, Control adorner)
        {
            if (this.ParentHost is Control { IsLoaded: false })
            {
                this.ParentHost.PropertyChanged -= ParentHostOnPropertyChanged;
                this.ParentHost.PropertyChanged += ParentHostOnPropertyChanged;
            }
            else
            {
                LocateCore(target, adorner);
            }

            return;

            void ParentHostOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
            {
                this.ParentHost.PropertyChanged -= ParentHostOnPropertyChanged;
                LocateCore(target, adorner);
            }
        }

        private void LocateCore(Visual target, Control adorner)
        {
            var matrix = target.TransformToVisual(this)!;
            var x      = matrix.Value.M31;
            var y      = matrix.Value.M32;
            var rect   = new Rect(x, y, target.Bounds.Width, target.Bounds.Height);
            
            SetTargetRect(adorner, rect);
            SetLeft(adorner, x);
            SetTop(adorner, y);
            adorner.Width  = target.Bounds.Width;
            adorner.Height = target.Bounds.Height;
            
            adorner.Measure(target.Bounds.Size);
            adorner.Arrange(rect);
        }

        private void OnTargetOnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
        {
            if (sender is not Visual target)
            {
                return;
            }

            var adorners = new List<Control>();
            for (var i = 0; i < _detachedAdorners.Count; i++)
            {
                var w = _detachedAdorners[i];
                if (w.TryGetTarget(out var t) && Children.Contains(t) == false && GetTarget(t) == target)
                {
                    adorners.Add(t);
                    _detachedAdorners.RemoveAt(i--);
                }
            }
            
            adorners = adorners.Where(a => Children.Contains(a) == false && GetTarget(a) == target).ToList();
            foreach (var adorner in adorners)
            {
                Children.Add(adorner);   
            }
        }
        
        private void OnTargetOnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args)
        {
            if (sender is not Visual target)
            {
                return;
            }
           
            var adorners = target.GetAdorners();
            foreach (var adorner in adorners)
            {
                Children.Remove(adorner);   
                _detachedAdorners.Add(new WeakReference<Control>(adorner));
            }
        }
    }
}
