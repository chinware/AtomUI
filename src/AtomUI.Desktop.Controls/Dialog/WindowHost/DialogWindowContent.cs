using System.Collections.Specialized;
using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls;

internal class DialogWindowContent : TemplatedControl, IMotionAwareControl
{
    public static readonly StyledProperty<object?> DialogContentProperty =
        AvaloniaProperty.Register<DialogWindowContent, object?>(nameof(DialogContent));

    public static readonly StyledProperty<IDataTemplate?> DialogContentTemplateProperty =
        AvaloniaProperty.Register<DialogWindowContent, IDataTemplate?>(nameof(DialogContentTemplate));

    public static readonly StyledProperty<DialogStandardButtons> StandardButtonsProperty =
        DialogButtonBox.StandardButtonsProperty.AddOwner<DialogWindowContent>();

    public static readonly StyledProperty<DialogStandardButton> DefaultStandardButtonProperty =
        DialogButtonBox.DefaultStandardButtonProperty.AddOwner<DialogWindowContent>();

    public static readonly StyledProperty<DialogStandardButton> EscapeStandardButtonProperty =
        DialogButtonBox.EscapeStandardButtonProperty.AddOwner<DialogWindowContent>();

    public static readonly StyledProperty<bool> IsFooterVisibleProperty =
        Dialog.IsFooterVisibleProperty.AddOwner<DialogWindowContent>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DialogWindowContent>();

    public static readonly StyledProperty<bool> IsLoadingProperty =
        Dialog.IsLoadingProperty.AddOwner<DialogWindowContent>();

    public static readonly StyledProperty<bool> IsConfirmLoadingProperty =
        Dialog.IsConfirmLoadingProperty.AddOwner<DialogWindowContent>();

    public static readonly DirectProperty<DialogWindowContent, bool> IsEffectiveFooterVisibleProperty =
        AvaloniaProperty.RegisterDirect<DialogWindowContent, bool>(
            nameof(IsEffectiveFooterVisible),
            o => o.IsEffectiveFooterVisible,
            (o, v) => o.IsEffectiveFooterVisible = v);

    public object? DialogContent
    {
        get => GetValue(DialogContentProperty);
        set => SetValue(DialogContentProperty, value);
    }

    public IDataTemplate? DialogContentTemplate
    {
        get => GetValue(DialogContentTemplateProperty);
        set => SetValue(DialogContentTemplateProperty, value);
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

    private bool _isEffectiveFooterVisible;

    public bool IsEffectiveFooterVisible
    {
        get => _isEffectiveFooterVisible;
        set => SetAndRaise(IsEffectiveFooterVisibleProperty, ref _isEffectiveFooterVisible, value);
    }

    public AvaloniaList<DialogButton> CustomButtons { get; } = new();

    public event EventHandler<DialogButtonClickedEventArgs>? ButtonBoxClicked;
    public event EventHandler<DialogBoxButtonSyncEventArgs>? ButtonsSynchronized;

    private DialogButtonBox? _buttonBox;

    public DialogWindowContent()
    {
        this.RegisterTokenResourceScope(DialogToken.ScopeProvider);
        CustomButtons.CollectionChanged += HandleCustomButtonsChanged;
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

        _buttonBox = e.NameScope.Find<DialogButtonBox>("PART_ButtonBox");
        if (_buttonBox != null)
        {
            _buttonBox.CustomButtons.AddRange(CustomButtons);
            _buttonBox.Clicked             += HandleButtonBoxClicked;
            _buttonBox.ButtonsSynchronized += HandleButtonsSynchronized;
        }
        ConfigureEffectiveFooterVisible();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StandardButtonsProperty ||
            change.Property == IsLoadingProperty ||
            change.Property == IsFooterVisibleProperty)
        {
            ConfigureEffectiveFooterVisible();
        }
    }

    private void HandleCustomButtonsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_buttonBox != null)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    _buttonBox.CustomButtons.AddRange(e.NewItems!.OfType<DialogButton>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _buttonBox.CustomButtons.RemoveAll(e.OldItems!.OfType<DialogButton>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException();
            }
        }

        ConfigureEffectiveFooterVisible();
    }

    private void ConfigureEffectiveFooterVisible()
    {
        SetCurrentValue(IsEffectiveFooterVisibleProperty,
            IsFooterVisible && !IsLoading && (StandardButtons.Count > 0 || CustomButtons.Count > 0));
    }

    private void HandleButtonBoxClicked(object? sender, DialogButtonClickedEventArgs args)
    {
        ButtonBoxClicked?.Invoke(this, args);
    }

    private void HandleButtonsSynchronized(object? sender, DialogBoxButtonSyncEventArgs args)
    {
        ButtonsSynchronized?.Invoke(this, args);
    }
}
