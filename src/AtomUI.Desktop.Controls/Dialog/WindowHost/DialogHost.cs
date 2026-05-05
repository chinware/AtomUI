using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Themes;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Desktop.Controls;

internal class DialogHost : Window,
                            IDialogHost,
                            IStyleHost,
                            IMotionAwareControl
{
    #region 公共属性定义

    public static readonly StyledProperty<Transform?> TransformProperty =
        AvaloniaProperty.Register<DialogHost, Transform?>(nameof(Transform));
    
    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        DialogButtonBox.StandardButtonsProperty.AddOwner<DialogHost>();
    
    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        DialogButtonBox.DefaultStandardButtonProperty.AddOwner<DialogHost>();
    
    public static readonly StyledProperty<DialogStandardButton> EscapeStandardButtonProperty =
        DialogButtonBox.EscapeStandardButtonProperty.AddOwner<DialogHost>();
    
    public static readonly StyledProperty<bool> IsFooterVisibleProperty =
        Dialog.IsFooterVisibleProperty.AddOwner<DialogHost>();
    
    public static readonly StyledProperty<bool> IsModalProperty =
        AvaloniaProperty.Register<DialogHost, bool>(nameof(IsModal));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DialogHost>();
        
    public static readonly StyledProperty<bool> IsLoadingProperty = Dialog.IsLoadingProperty.AddOwner<DialogHost>();
    public static readonly StyledProperty<bool> IsConfirmLoadingProperty = Dialog.IsConfirmLoadingProperty.AddOwner<DialogHost>();
    
    public Transform? Transform
    {
        get => GetValue(TransformProperty);
        set => SetValue(TransformProperty, value);
    }
    
    public DialogStandardButtons StandardButtons
    {
        get => GetValue(StandardButtonsProperty);
        set => SetValue(StandardButtonsProperty, value);
    }
    
    public DialogStandardButton DefaultStandardButton
    {
        get => GetValue(DefaultStandardButtonProperty);
        set => SetValue(DefaultStandardButtonProperty, value);
    }
    
    public DialogStandardButton EscapeStandardButton
    {
        get => GetValue(EscapeStandardButtonProperty);
        set => SetValue(EscapeStandardButtonProperty, value);
    }
    
    public bool IsFooterVisible
    {
        get => GetValue(IsFooterVisibleProperty);
        set => SetValue(IsFooterVisibleProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public bool IsConfirmLoading
    {
        get => GetValue(IsConfirmLoadingProperty);
        set => SetValue(IsConfirmLoadingProperty, value);
    }
    
    public bool IsModal
    {
        get => GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    IStyleHost? IStyleHost.StylingParent => Parent;
    
    public TopLevel ParentTopLevel { get; }
    
    public AvaloniaList<DialogButton> CustomButtons { get; } = new ();
    
    #endregion
    
    #region 内部属性定义
    
    public static readonly DirectProperty<DialogHost, bool> IsEffectiveFooterVisibleProperty =
        AvaloniaProperty.RegisterDirect<DialogHost, bool>(
            nameof(IsEffectiveFooterVisible),
            o => o.IsEffectiveFooterVisible,
            (o, v) => o.IsEffectiveFooterVisible = v);
    
    private bool _isEffectiveFooterVisible;

    public bool IsEffectiveFooterVisible
    {
        get => _isEffectiveFooterVisible;
        set => SetAndRaise(IsEffectiveFooterVisibleProperty, ref _isEffectiveFooterVisible, value);
    }
    
    #endregion
    
    protected override Type StyleKeyOverride { get; } = typeof(DialogHost);
    
    private Size _dialogSize;
    private bool _needsUpdate;
    private Dialog _dialog;
    private PixelPoint _latestDialogPosition;
    private DialogButtonBox? _buttonBox;
    private CompositeDisposable? _confirmLoadingBindings;
    
    public DialogHost(TopLevel parent, Dialog dialog)
    {
        ParentTopLevel                  =  parent;
        _dialog                         =  dialog;
        CustomButtons.CollectionChanged += new(HandleCustomButtonsChanged);
    }
    
    public void SetChild(Control? control) => Content = control;

    protected override void ArrangeCore(Rect finalRect)
    {
        if (_dialogSize != finalRect.Size)
        {
            _dialogSize  = finalRect.Size;
            var  screens       = Screens.All;
            var  primaryScreen = screens.FirstOrDefault(s => s.IsPrimary);
            Rect workingArea   = default;
            if (primaryScreen != null)
            {
                workingArea = new Rect(primaryScreen.WorkingArea.X, primaryScreen.WorkingArea.Y, primaryScreen.WorkingArea.Width, primaryScreen.WorkingArea.Height);
            }
            _dialog.NotifyDialogHostMeasured(_dialogSize, workingArea);
            _needsUpdate = true;
        }
        
        base.ArrangeCore(finalRect);
    }
    
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (!e.IsProgrammatic || CloseByClickCloseCaptionButton)
        {
            e.Cancel = true;
            Dispatcher.Post(() =>
            {
                CloseByClickCloseCaptionButton = false;
                _dialog.NotifyDialogHostCloseRequest();
            });
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        if (_buttonBox != null)
        {
            _buttonBox.CustomButtons.Clear();
            _buttonBox.Clicked             -= HandleButtonBoxClicked;
            _buttonBox.ButtonsSynchronized -= HandleButtonsSynchronized;
        }
        _buttonBox = e.NameScope.Find<DialogButtonBox>(DialogThemeConstants.ButtonBoxPart);
        if (_buttonBox != null)
        {
            _buttonBox.CustomButtons.AddRange(CustomButtons);
            _buttonBox.Clicked             += HandleButtonBoxClicked;
            _buttonBox.ButtonsSynchronized += HandleButtonsSynchronized;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StandardButtonsProperty ||
            change.Property == IsLoadingProperty)
        {
            ConfigureEffectiveFooterVisible();
        }
    }

    private void ConfigureEffectiveFooterVisible()
    {
        if (IsFooterVisible)
        {
            if (!IsLoading)
            {
                SetCurrentValue(IsEffectiveFooterVisibleProperty, StandardButtons.Count > 0 || CustomButtons.Count > 0);
            }
            else
            {
                SetCurrentValue(IsEffectiveFooterVisibleProperty, false);
            }
        }
        else
        {
            SetCurrentValue(IsEffectiveFooterVisibleProperty, false);
        }
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_buttonBox != null)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newItems = e.NewItems!.OfType<DialogButton>();
                    _buttonBox.CustomButtons.AddRange(newItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldItems = e.OldItems!.OfType<DialogButton>();
                    _buttonBox.CustomButtons.RemoveAll(oldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException();
            }
        }

        ConfigureEffectiveFooterVisible();
    }

    private void HandleButtonBoxClicked(object? sender, DialogButtonClickedEventArgs args)
    {
        _dialog.NotifyDialogButtonBoxClicked(args.SourceButton);
    }

    private void HandleButtonsSynchronized(object? sender, DialogBoxButtonSyncEventArgs args)
    {
        _dialog.NotifyDialogButtonSynchronized(args.Buttons);
        _confirmLoadingBindings?.Dispose();
        _confirmLoadingBindings = new CompositeDisposable(args.Buttons.Count);
        foreach (var button in args.Buttons)
        {
            if (button.Role == DialogButtonRole.AcceptRole ||
                button.Role == DialogButtonRole.YesRole ||
                button.Role == DialogButtonRole.ApplyRole)
            {
                _confirmLoadingBindings.Add(BindUtils.RelayBind(this, IsConfirmLoadingProperty, button, Button.IsLoadingProperty));
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        return new Size(Math.Max(size.Width, MinWidth), Math.Max(size.Height, MinHeight));
    }

    public void Close(Action? callback = null)
    {
        base.Close();
        callback?.Invoke();
    }
}