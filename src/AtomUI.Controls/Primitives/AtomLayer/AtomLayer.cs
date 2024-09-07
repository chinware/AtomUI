using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Rect = Avalonia.Rect;

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
        
        #endregion


        
        public Visual? Host
        {
            get => GetValue(HostProperty);
            set => SetValue(HostProperty, value);
        }
        public static readonly StyledProperty<Visual?> HostProperty = AvaloniaProperty
            .Register<AtomLayer, Visual?>(nameof(Host));

        public Vector HostOffset
        {
            get => GetValue(HostOffsetProperty);
            set => SetValue(HostOffsetProperty, value);
        }
        public static readonly StyledProperty<Vector> HostOffsetProperty = AvaloniaProperty
            .Register<AtomLayer, Vector>(nameof(HostOffset));

        private readonly IList<WeakReference<Control>> _detachedAdorners = new List<WeakReference<Control>>();
        
        static AtomLayer()
        {
            HostOffsetProperty.Changed.AddClassHandler<AtomLayer>((layer, args) =>
            {
                layer.Measure();
            });
        }
        
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
            UpdateLocation(target, adorner);
            Arrange();
        }

        private void Measure()
        {
            Measure(new Size());
            Arrange();
        }

        private void Arrange()
        {
            Arrange(new Rect(new Point(HostOffset.X, -HostOffset.Y), new Size()));
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
            
            // Child element's bounds will be updated before it's ancestors do.
            Dispatcher.UIThread.Post(() =>
            {
                foreach (var adorner in GetAdorners(target))
                {
                    UpdateLocation(target, adorner);
                }    
            }, DispatcherPriority.Send);
        }

        private void UpdateLocation(Visual target, Control adorner)
        {
            if (this.Host is Control { IsLoaded: false })
            {
                this.Host.PropertyChanged -= ParentHostOnPropertyChanged;
                this.Host.PropertyChanged += ParentHostOnPropertyChanged;
            }
            else
            {
                UpdateLocationCore(target, adorner);
            }

            return;

            void ParentHostOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
            {
                this.Host.PropertyChanged -= ParentHostOnPropertyChanged;
                UpdateLocationCore(target, adorner);
            }
        }

        private void UpdateLocationCore(Visual target, Control adorner)
        {
            var matrix = target.TransformToVisual(this)!;
            var x      = matrix.Value.M31;
            var y      = matrix.Value.M32;
            
            SetLeft(adorner, x);
            SetTop(adorner, y);
            adorner.Width  = target.Bounds.Width;
            adorner.Height = target.Bounds.Height;
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
