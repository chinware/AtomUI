using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;


[PseudoClasses(StdPseudoClass.Normal, StdPseudoClass.Minimized, StdPseudoClass.Maximized, StdPseudoClass.Fullscreen)]
public class CaptionButtonGroup : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsFullScreenEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsFullScreenEnabled));

    public static readonly StyledProperty<bool> IsMaximizeEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsMaximizeEnabled), defaultValue: true);

    public static readonly StyledProperty<bool> IsMinimizeEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsMinimizeEnabled), defaultValue: true);

    public static readonly StyledProperty<bool> IsPinEnabledProperty =
        AvaloniaProperty.Register<Window, bool>(nameof(IsPinEnabled));
    
    public static readonly StyledProperty<bool> IsWindowActiveProperty = 
        TitleBar.IsWindowActiveProperty.AddOwner<CaptionButtonGroup>();

    public bool IsFullScreenEnabled
    {
        get => GetValue(IsFullScreenEnabledProperty);
        set => SetValue(IsFullScreenEnabledProperty, value);
    }

    public bool IsMaximizeEnabled
    {
        get => GetValue(IsMaximizeEnabledProperty);
        set => SetValue(IsMaximizeEnabledProperty, value);
    }

    public bool IsMinimizeEnabled
    {
        get => GetValue(IsMinimizeEnabledProperty);
        set => SetValue(IsMinimizeEnabledProperty, value);
    }

    public bool IsPinEnabled
    {
        get => GetValue(IsPinEnabledProperty);
        set => SetValue(IsPinEnabledProperty, value);
    }
    
    public bool IsWindowActive
    {
        get => GetValue(IsWindowActiveProperty);
        set => SetValue(IsWindowActiveProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<CaptionButtonGroup>();
    
    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    protected Window? HostWindow { get; private set; }
    private Button? _restoreButton;
    private IDisposable? _disposables;

    public virtual void Attach(Window hostWindow)
    {
        if (_disposables != null)
            return;
        HostWindow = hostWindow;
        _disposables = new CompositeDisposable(Array.Empty<IDisposable>())
        {
            HostWindow.GetObservable(Window.CanResizeProperty).Subscribe(
                x=>
                {
                    if (_restoreButton == null)
                    {
                        return;
                    }
                    _restoreButton.IsEnabled = x;
                }),
            HostWindow.GetObservable(Window.WindowStateProperty)
                .Subscribe(x =>
                {
                    PseudoClasses.Set(StdPseudoClass.Minimized, x == WindowState.Minimized);
                    PseudoClasses.Set(StdPseudoClass.Normal, x == WindowState.Normal);
                    PseudoClasses.Set(StdPseudoClass.Maximized, x == WindowState.Maximized);
                    PseudoClasses.Set(StdPseudoClass.Fullscreen, x == WindowState.FullScreen);
                })
        };
    }

    public virtual void Detach()
    {
        if (_disposables == null)
        {
            return;
        }
        _disposables.Dispose();
        _disposables = null;
        HostWindow   = null;
    }

    protected virtual void NotifyClose()
    {
        HostWindow?.Close();
    }

    protected virtual void NotifyRestore()
    {
        if (HostWindow == null)
            return;
        HostWindow.WindowState = HostWindow.WindowState == WindowState.Maximized
            ? WindowState.Normal
            : WindowState.Maximized;
    }

    protected virtual void NotifyMinimize()
    {
        if (HostWindow == null)
        {
            return;
        }
        HostWindow.WindowState = WindowState.Minimized;
    }

    protected virtual void NotifyToggleFullScreen()
    {
        if (HostWindow == null)
            return;
        HostWindow.WindowState = HostWindow.WindowState == WindowState.FullScreen
            ? WindowState.Normal
            : WindowState.FullScreen;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        // Button button1 = e.NameScope.Find<Button>("PART_CloseButton");
        // if (button1 != null)
        //     button1.Click += (EventHandler<RoutedEventArgs>)((_, args) =>
        //     {
        //         this.OnClose();
        //         args.Handled = true;
        //     });
        // Button button2 = e.NameScope.Find<Button>("PART_RestoreButton");
        // if (button2 != null)
        // {
        //     button2.Click += (EventHandler<RoutedEventArgs>)((_, args) =>
        //     {
        //         this.OnRestore();
        //         args.Handled = true;
        //     });
        //     Button button3    = button2;
        //     Window hostWindow = this.HostWindow;
        //     int    num        = hostWindow != null ? (hostWindow.CanResize ? 1 : 0) : 1;
        //     button3.IsEnabled   = num != 0;
        //     this._restoreButton = button2;
        // }
        //
        // Button button4 = e.NameScope.Find<Button>("PART_MinimizeButton");
        // if (button4 != null)
        //     button4.Click += (EventHandler<RoutedEventArgs>)((_, args) =>
        //     {
        //         this.OnMinimize();
        //         args.Handled = true;
        //     });
        // Button button5 = e.NameScope.Find<Button>("PART_FullScreenButton");
        // if (button5 == null)
        //     return;
        // button5.Click += (EventHandler<RoutedEventArgs>)((_, args) =>
        // {
        //     this.OnToggleFullScreen();
        //     args.Handled = true;
        // });
    }
}