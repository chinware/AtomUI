using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;

namespace AtomUI.Controls.Commons;

using AvaloniaScrollViewer = Avalonia.Controls.ScrollViewer;

public abstract class AbstractScrollViewer : AvaloniaScrollViewer, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractScrollViewer>();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion
    
    #region 内部属性定义

    internal static readonly StyledProperty<double> ScrollBarsSeparatorOpacityProperty =
        AvaloniaProperty.Register<AbstractScrollViewer, double>(nameof(ScrollBarsSeparatorOpacity));
    
    internal static readonly StyledProperty<double> ScrollBarOpacityProperty =
        AvaloniaProperty.Register<AbstractScrollViewer, double>(nameof(ScrollBarOpacity));
    
    internal double ScrollBarsSeparatorOpacity
    {
        get => GetValue(ScrollBarsSeparatorOpacityProperty);
        set => SetValue(ScrollBarsSeparatorOpacityProperty, value);
    }
    
    internal double ScrollBarOpacity
    {
        get => GetValue(ScrollBarOpacityProperty);
        set => SetValue(ScrollBarOpacityProperty, value);
    }

    #endregion
    
    private bool _scrollBarDragging = false;
    private bool _isPointerInside = false;
    private IDisposable? _pointerMoveSubscription;

    static AbstractScrollViewer()
    {
        Thumb.DragStartedEvent.AddClassHandler<AbstractScrollViewer>((view, evt) =>
        {
            view.HandleScrollBarDragStarted();
        });
        Thumb.DragCompletedEvent.AddClassHandler<AbstractScrollViewer>((view, evt) =>
        {
            view.HandleScrollBarDragCompleted();
        });
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == AllowAutoHideProperty)
        {
            if (AllowAutoHide)
            {
                ConfigureInputManager();
            }
            else
            {
                _pointerMoveSubscription?.Dispose();
                _pointerMoveSubscription = null;
            }
        }
    }

    private void HandleScrollBarDragStarted()
    {
        _scrollBarDragging = true;
    }
    
    private void HandleScrollBarDragCompleted()
    {
        _scrollBarDragging = false;
        if (AllowAutoHide && !_isPointerInside)
        {
            ScrollBarOpacity = 0.0;
        }
    }
    
    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.Dispatcher.Post(this.EnableTransitions);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureInputManager();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _pointerMoveSubscription?.Dispose();
        _pointerMoveSubscription = null;
    }

    private void ConfigureInputManager()
    {
        if (AllowAutoHide)
        {
            var inputManager = AvaloniaLocator.Current.GetService(typeof(IInputManager)) as IInputManager;
            Debug.Assert(inputManager != null);
            _pointerMoveSubscription = inputManager.Process.Subscribe(ListenForMouseEvent);
        }
    }
    
    private void ListenForMouseEvent(RawInputEventArgs e)
    {
        if (e is RawPointerEventArgs mouseEventArgs)
        {
            if (mouseEventArgs.Root.GetRootElement() != TopLevel.GetTopLevel(this) || Classes.Contains(StdPseudoClass.Disabled))
            {
                return;
            }
            
            if (mouseEventArgs.Type == RawPointerEventType.Move)
            {
                if (IsMousePointIn(mouseEventArgs))
                {
                    if (!_isPointerInside)
                    {
                        _isPointerInside = true;
                        ScrollBarOpacity = 1.0;
                    }
                }
                else
                {
                    if (_isPointerInside)
                    {
                        _isPointerInside = false;
                        if (!_scrollBarDragging)
                        {
                            ScrollBarOpacity = 0.0;
                        }
                    }
                }
            }
        }
    }
    
    private bool IsMousePointIn(RawPointerEventArgs args)
    {
        var scaling          = TopLevelUtils.GetDesktopScaling(this);
        var pointRoot        = args.Root.GetRootElement() as Control;
        var localPoint       = pointRoot?.PointToScreen(args.Position) ?? default;
        var offset           = this.PointToScreen(new Point(0, 0));
        var constraintBounds = new Rect(new Point(offset.X, offset.Y), Bounds.Size * scaling);
        if (constraintBounds.Contains(new Point(localPoint.X, localPoint.Y)))
        {
            return true;
        }
        return false;
    }
}