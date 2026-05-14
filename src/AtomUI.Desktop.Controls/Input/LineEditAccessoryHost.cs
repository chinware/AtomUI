using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System.Reactive.Disposables;

namespace AtomUI.Desktop.Controls;

internal class LineEditAccessoryHost : StackPanel
{
    public static readonly StyledProperty<bool> IsClearButtonSlotEnabledProperty =
        AvaloniaProperty.Register<LineEditAccessoryHost, bool>(nameof(IsClearButtonSlotEnabled), true);

    public static readonly StyledProperty<bool> IsRevealButtonSlotEnabledProperty =
        AvaloniaProperty.Register<LineEditAccessoryHost, bool>(nameof(IsRevealButtonSlotEnabled), true);

    public static readonly StyledProperty<bool> IsFormFeedbackSlotEnabledProperty =
        AvaloniaProperty.Register<LineEditAccessoryHost, bool>(nameof(IsFormFeedbackSlotEnabled), true);

    public static readonly StyledProperty<bool> IsInnerRightContentSlotEnabledProperty =
        AvaloniaProperty.Register<LineEditAccessoryHost, bool>(nameof(IsInnerRightContentSlotEnabled), true);

    public static readonly StyledProperty<bool> IsCountIndicatorSlotEnabledProperty =
        AvaloniaProperty.Register<LineEditAccessoryHost, bool>(nameof(IsCountIndicatorSlotEnabled), true);

    public bool IsClearButtonSlotEnabled
    {
        get => GetValue(IsClearButtonSlotEnabledProperty);
        set => SetValue(IsClearButtonSlotEnabledProperty, value);
    }

    public bool IsRevealButtonSlotEnabled
    {
        get => GetValue(IsRevealButtonSlotEnabledProperty);
        set => SetValue(IsRevealButtonSlotEnabledProperty, value);
    }

    public bool IsFormFeedbackSlotEnabled
    {
        get => GetValue(IsFormFeedbackSlotEnabledProperty);
        set => SetValue(IsFormFeedbackSlotEnabledProperty, value);
    }

    public bool IsInnerRightContentSlotEnabled
    {
        get => GetValue(IsInnerRightContentSlotEnabledProperty);
        set => SetValue(IsInnerRightContentSlotEnabledProperty, value);
    }

    public bool IsCountIndicatorSlotEnabled
    {
        get => GetValue(IsCountIndicatorSlotEnabledProperty);
        set => SetValue(IsCountIndicatorSlotEnabledProperty, value);
    }

    private LineEdit? _owner;
    private CompositeDisposable? _ownerSubscriptions;
    private InputClearIconButton? _clearButton;
    private RevealButton? _revealButton;
    private IDisposable? _revealButtonSubscription;
    private ContentPresenter? _formFeedbackPresenter;
    private ContentPresenter? _innerRightContentPresenter;
    private TextBlock? _countTextIndicator;
    private IDisposable? _countTextForegroundBinding;
    private bool _isAttachingOwner;
    private bool _isUpdateQueued;
    private int _updateVersion;

    public LineEditAccessoryHost()
    {
        Orientation = Orientation.Horizontal;
        IsVisible   = false;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsClearButtonSlotEnabledProperty ||
            change.Property == IsRevealButtonSlotEnabledProperty ||
            change.Property == IsFormFeedbackSlotEnabledProperty ||
            change.Property == IsInnerRightContentSlotEnabledProperty ||
            change.Property == IsCountIndicatorSlotEnabledProperty)
        {
            QueueUpdateAccessoryState();
        }
    }

    internal void AttachOwner(LineEdit owner)
    {
        if (ReferenceEquals(_owner, owner))
        {
            UpdateAccessoryState();
            return;
        }

        DetachOwner();
        _owner = owner;
        _isAttachingOwner = true;
        _ownerSubscriptions = new CompositeDisposable
        {
            owner.GetObservable(TextBox.IsEffectiveShowClearButtonProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextBox.ClearIconProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextBox.IsMotionEnabledProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextBox.IsEnableRevealButtonProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(Avalonia.Controls.TextBox.RevealPasswordProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextBox.FormFeedbackProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextBox.IsFormFeedbackVisibleProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(Avalonia.Controls.TextBox.InnerRightContentProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(LineEdit.InnerRightContentTemplateProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextBox.IsShowCountProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextBox.CountTextProperty).Subscribe(_ => HandleOwnerPropertyChanged())
        };
        _isAttachingOwner = false;
        _updateVersion++;
        UpdateAccessoryState();
    }

    internal void DetachOwner()
    {
        _updateVersion++;
        _isUpdateQueued = false;
        _ownerSubscriptions?.Dispose();
        _ownerSubscriptions = null;

        DestroyClearButton();
        DestroyRevealButton();
        DestroyFormFeedbackPresenter();
        DestroyInnerRightContentPresenter();
        DestroyCountTextIndicator();

        Children.Clear();
        _owner    = null;
        IsVisible = false;
    }

    private void HandleOwnerPropertyChanged()
    {
        if (!_isAttachingOwner)
        {
            QueueUpdateAccessoryState();
        }
    }

    private void QueueUpdateAccessoryState()
    {
        if (_owner == null || _isUpdateQueued)
        {
            return;
        }

        _isUpdateQueued = true;
        var version = _updateVersion;
        Dispatcher.UIThread.Post(() =>
        {
            _isUpdateQueued = false;
            if (_owner != null && version == _updateVersion)
            {
                UpdateAccessoryState();
            }
        }, DispatcherPriority.Render);
    }

    private void UpdateAccessoryState()
    {
        if (_owner == null)
        {
            return;
        }

        var visualsChanged = false;
        visualsChanged |= UpdateClearButton();
        visualsChanged |= UpdateRevealButton();
        visualsChanged |= UpdateFormFeedbackPresenter();
        visualsChanged |= UpdateInnerRightContentPresenter();
        visualsChanged |= UpdateCountTextIndicator();
        visualsChanged |= EnsureChildOrder();
        IsVisible = Children.Count > 0;
        if (visualsChanged)
        {
            NotifyDecoratedBoxContentChanged();
        }
    }

    private bool UpdateClearButton()
    {
        if (_owner == null || !IsClearButtonSlotEnabled || !_owner.IsEffectiveShowClearButton)
        {
            return DestroyClearButton();
        }

        var visualsChanged = false;
        if (_clearButton == null)
        {
            _clearButton = new InputClearIconButton
            {
                Name              = "PART_ClearButton",
                VerticalAlignment = VerticalAlignment.Center
            };
            _clearButton.Click += HandleClearButtonClicked;
            visualsChanged = true;
        }

        if (!Equals(_clearButton.Icon, _owner.ClearIcon))
        {
            visualsChanged = true;
        }
        _clearButton.SetCurrentValue(AbstractIconButton.IconProperty, _owner.ClearIcon);
        _clearButton.SetCurrentValue(AbstractIconButton.IsMotionEnabledProperty, _owner.IsMotionEnabled);
        return visualsChanged;
    }

    private bool DestroyClearButton()
    {
        if (_clearButton == null)
        {
            return false;
        }

        _clearButton.Click -= HandleClearButtonClicked;
        _clearButton.ClearValue(AbstractIconButton.IconProperty);
        _clearButton.ClearValue(AbstractIconButton.IsMotionEnabledProperty);
        Children.Remove(_clearButton);
        _clearButton = null;
        return true;
    }

    private void HandleClearButtonClicked(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        _owner?.NotifyAccessoryClearButtonClicked();
    }

    private bool UpdateRevealButton()
    {
        if (_owner == null || !IsRevealButtonSlotEnabled || !_owner.IsEnableRevealButton)
        {
            return DestroyRevealButton();
        }

        var visualsChanged = false;
        if (_revealButton == null)
        {
            _revealButton = new RevealButton
            {
                Name              = "PART_RevealButton",
                VerticalAlignment = VerticalAlignment.Center
            };
            _revealButton.SetCurrentValue(ToggleButton.IsCheckedProperty, _owner.RevealPassword);
            _revealButtonSubscription = _revealButton.GetObservable(ToggleButton.IsCheckedProperty)
                                                     .Subscribe(HandleRevealButtonCheckedChanged);
            visualsChanged = true;
        }

        _revealButton.SetCurrentValue(ToggleButton.IsCheckedProperty, _owner.RevealPassword);
        _revealButton.SetCurrentValue(AbstractIconButton.IsMotionEnabledProperty, _owner.IsMotionEnabled);
        return visualsChanged;
    }

    private bool DestroyRevealButton()
    {
        if (_revealButton == null)
        {
            return false;
        }

        _revealButtonSubscription?.Dispose();
        _revealButtonSubscription = null;
        _revealButton.ClearValue(ToggleButton.IsCheckedProperty);
        _revealButton.ClearValue(AbstractIconButton.IsMotionEnabledProperty);
        Children.Remove(_revealButton);
        _revealButton = null;
        return true;
    }

    private void HandleRevealButtonCheckedChanged(bool? isChecked)
    {
        if (_owner == null)
        {
            return;
        }

        var revealPassword = isChecked == true;
        if (_owner.RevealPassword != revealPassword)
        {
            _owner.SetCurrentValue(Avalonia.Controls.TextBox.RevealPasswordProperty, revealPassword);
        }
    }

    private bool UpdateFormFeedbackPresenter()
    {
        if (_owner == null ||
            !IsFormFeedbackSlotEnabled ||
            !_owner.IsFormFeedbackVisible ||
            _owner.FormFeedback == null)
        {
            return DestroyFormFeedbackPresenter();
        }

        var visualsChanged = false;
        if (_formFeedbackPresenter == null)
        {
            _formFeedbackPresenter = new ContentPresenter
            {
                Name                     = "FormFeedBack",
                VerticalAlignment        = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment      = HorizontalAlignment.Right
            };
            visualsChanged = true;
        }

        if (!ReferenceEquals(_formFeedbackPresenter.Content, _owner.FormFeedback))
        {
            visualsChanged = true;
        }
        _formFeedbackPresenter.SetCurrentValue(ContentPresenter.ContentProperty, _owner.FormFeedback);
        return visualsChanged;
    }

    private bool DestroyFormFeedbackPresenter()
    {
        if (_formFeedbackPresenter == null)
        {
            return false;
        }

        _formFeedbackPresenter.ClearValue(ContentPresenter.ContentProperty);
        Children.Remove(_formFeedbackPresenter);
        _formFeedbackPresenter = null;
        return true;
    }

    private bool UpdateInnerRightContentPresenter()
    {
        if (_owner == null ||
            !IsInnerRightContentSlotEnabled ||
            _owner.InnerRightContent == null)
        {
            return DestroyInnerRightContentPresenter();
        }

        var visualsChanged = false;
        if (_innerRightContentPresenter == null)
        {
            _innerRightContentPresenter = new ContentPresenter
            {
                Name                     = "InnerRightContentPresenter",
                VerticalAlignment        = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment      = HorizontalAlignment.Right,
                Focusable                = false
            };
            visualsChanged = true;
        }

        if (!ReferenceEquals(_innerRightContentPresenter.Content, _owner.InnerRightContent) ||
            !Equals(_innerRightContentPresenter.ContentTemplate, _owner.InnerRightContentTemplate))
        {
            visualsChanged = true;
        }
        _innerRightContentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, _owner.InnerRightContent);
        _innerRightContentPresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty,
            _owner.InnerRightContentTemplate);
        return visualsChanged;
    }

    private bool DestroyInnerRightContentPresenter()
    {
        if (_innerRightContentPresenter == null)
        {
            return false;
        }

        _innerRightContentPresenter.ClearValue(ContentPresenter.ContentProperty);
        _innerRightContentPresenter.ClearValue(ContentPresenter.ContentTemplateProperty);
        Children.Remove(_innerRightContentPresenter);
        _innerRightContentPresenter = null;
        return true;
    }

    private bool UpdateCountTextIndicator()
    {
        if (_owner == null || !IsCountIndicatorSlotEnabled || !_owner.IsShowCount)
        {
            return DestroyCountTextIndicator();
        }

        var visualsChanged = false;
        if (_countTextIndicator == null)
        {
            _countTextIndicator = new TextBlock
            {
                Name              = "TextCountIndicator",
                VerticalAlignment = VerticalAlignment.Center
            };
            _countTextForegroundBinding = TokenResourceBinder.CreateTokenBinding(
                _countTextIndicator,
                TextBlock.ForegroundProperty,
                SharedTokenKind.ColorTextPlaceholder);
            visualsChanged = true;
        }

        _countTextIndicator.SetCurrentValue(TextBlock.TextProperty, _owner.CountText);
        return visualsChanged;
    }

    private bool DestroyCountTextIndicator()
    {
        if (_countTextIndicator == null)
        {
            return false;
        }

        _countTextForegroundBinding?.Dispose();
        _countTextForegroundBinding = null;
        _countTextIndicator.ClearValue(TextBlock.TextProperty);
        _countTextIndicator.ClearValue(TextBlock.ForegroundProperty);
        Children.Remove(_countTextIndicator);
        _countTextIndicator = null;
        return true;
    }

    private bool EnsureChildOrder()
    {
        var index          = 0;
        var visualsChanged = false;
        visualsChanged |= PlaceChild(_clearButton, ref index);
        visualsChanged |= PlaceChild(_revealButton, ref index);
        visualsChanged |= PlaceChild(_formFeedbackPresenter, ref index);
        visualsChanged |= PlaceChild(_innerRightContentPresenter, ref index);
        visualsChanged |= PlaceChild(_countTextIndicator, ref index);
        return visualsChanged;
    }

    private bool PlaceChild(Control? child, ref int index)
    {
        if (child == null)
        {
            return false;
        }

        var currentIndex = Children.IndexOf(child);
        if (currentIndex == index)
        {
            index++;
            return false;
        }

        if (currentIndex >= 0)
        {
            Children.RemoveAt(currentIndex);
        }

        Children.Insert(index, child);
        index++;
        return true;
    }

    private void NotifyDecoratedBoxContentChanged()
    {
        this.FindAncestorOfType<AddOnDecoratedBox>()?.NotifyContentRightAddOnVisualsChanged();
    }
}
