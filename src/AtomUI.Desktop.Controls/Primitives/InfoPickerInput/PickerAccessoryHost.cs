using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls.Primitives;

internal class PickerAccessoryHost : StackPanel
{
    private InfoPickerInput? _owner;
    private InputClearIconButton? _clearButton;
    private IconPresenter? _infoIconPresenter;
    private ContentPresenter? _formFeedbackPresenter;
    private ContentPresenter? _contentRightAddOnPresenter;
    private CompositeDisposable? _ownerSubscriptions;
    private IDisposable? _feedbackStatusSubscription;
    private CompositeDisposable? _infoIconBindings;
    private bool _isAttachingOwner;

    public PickerAccessoryHost()
    {
        Orientation = Avalonia.Layout.Orientation.Horizontal;
    }

    public void AttachOwner(InfoPickerInput owner)
    {
        if (ReferenceEquals(_owner, owner))
        {
            return;
        }

        DetachOwner();

        _owner = owner;
        _isAttachingOwner = true;
        _ownerSubscriptions = new CompositeDisposable
        {
            owner.GetObservable(InfoPickerInput.IsClearButtonVisibleProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(InfoPickerInput.InfoIconProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(InfoPickerInput.FormFeedbackProperty).Subscribe(_ =>
            {
                ConfigureFeedbackStatusSubscription();
                HandleAccessoryStateChanged();
            }),
            owner.GetObservable(InfoPickerInput.ContentRightAddOnProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(InfoPickerInput.ContentRightAddOnTemplateProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(InfoPickerInput.IsMotionEnabledProperty).Subscribe(_ => HandleClearButtonStateChanged())
        };
        ConfigureFeedbackStatusSubscription();
        _isAttachingOwner = false;
        UpdateAccessoryState();
    }

    public void DetachOwner()
    {
        _ownerSubscriptions?.Dispose();
        _ownerSubscriptions = null;

        _feedbackStatusSubscription?.Dispose();
        _feedbackStatusSubscription = null;

        ClearClearButton();
        ClearInfoIconPresenter();
        ClearFormFeedbackPresenter();
        ClearContentRightAddOnPresenter();
        Children.Clear();
        _owner = null;
        _isAttachingOwner = false;
    }

    private void ConfigureFeedbackStatusSubscription()
    {
        _feedbackStatusSubscription?.Dispose();
        _feedbackStatusSubscription = null;
        if (_owner?.FormFeedback is FormValidateFeedback feedback)
        {
            _feedbackStatusSubscription = feedback.GetObservable(FormValidateFeedback.ValidateStatusProperty)
                                                  .Subscribe(_ => HandleAccessoryStateChanged());
        }
    }

    private void HandleAccessoryStateChanged()
    {
        if (!_isAttachingOwner)
        {
            UpdateAccessoryState();
        }
    }

    private void HandleClearButtonStateChanged()
    {
        if (!_isAttachingOwner)
        {
            UpdateClearButtonState();
        }
    }

    private void UpdateAccessoryState()
    {
        if (_owner?.IsClearButtonVisible == true)
        {
            ClearInfoIconPresenter();
            ClearFormFeedbackPresenter();
            UpdateClearButton();
        }
        else
        {
            ClearClearButton();
            UpdateInfoIconPresenter();
            UpdateFormFeedbackPresenter();
        }

        UpdateContentRightAddOnPresenter();
        EnsureChildOrder();
    }

    private void UpdateClearButton()
    {
        if (_owner == null)
        {
            return;
        }

        if (_clearButton == null)
        {
            _clearButton = new InputClearIconButton();
            _clearButton.Click += HandleClearButtonClick;
        }
        UpdateClearButtonState();
    }

    private void UpdateClearButtonState()
    {
        if (_owner == null || _clearButton == null)
        {
            return;
        }

        _clearButton.SetCurrentValue(AbstractIconButton.IsMotionEnabledProperty, _owner.IsMotionEnabled);
    }

    private void ClearClearButton()
    {
        if (_clearButton == null)
        {
            return;
        }

        _clearButton.Click -= HandleClearButtonClick;
        Children.Remove(_clearButton);
        _clearButton = null;
        NotifyDecoratedBoxContentChanged();
    }

    private void UpdateInfoIconPresenter()
    {
        if (_owner == null)
        {
            return;
        }

        if (_owner.InfoIcon == null)
        {
            ClearInfoIconPresenter();
            return;
        }

        if (_infoIconPresenter == null)
        {
            _infoIconPresenter = new IconPresenter();
            _infoIconBindings = new CompositeDisposable
            {
                TokenResourceBinder.CreateTokenBinding(
                    _infoIconPresenter,
                    WidthProperty,
                    SharedTokenKind.IconSize),
                TokenResourceBinder.CreateTokenBinding(
                    _infoIconPresenter,
                    HeightProperty,
                    SharedTokenKind.IconSize),
                TokenResourceBinder.CreateTokenBinding(
                    _infoIconPresenter,
                    IconPresenter.IconBrushProperty,
                    SharedTokenKind.ColorTextQuaternary)
            };
        }

        _infoIconPresenter.SetCurrentValue(IconPresenter.IconProperty, _owner.InfoIcon);
    }

    private void ClearInfoIconPresenter()
    {
        if (_infoIconPresenter == null)
        {
            return;
        }

        _infoIconBindings?.Dispose();
        _infoIconBindings = null;
        _infoIconPresenter.ClearValue(IconPresenter.IconProperty);
        Children.Remove(_infoIconPresenter);
        _infoIconPresenter = null;
        NotifyDecoratedBoxContentChanged();
    }

    private void UpdateFormFeedbackPresenter()
    {
        if (_owner == null)
        {
            return;
        }

        if (!IsFormFeedbackVisible())
        {
            ClearFormFeedbackPresenter();
            return;
        }

        _formFeedbackPresenter ??= new ContentPresenter
        {
            Name                     = "FormFeedBack",
            VerticalAlignment        = Avalonia.Layout.VerticalAlignment.Stretch,
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center,
            HorizontalAlignment      = Avalonia.Layout.HorizontalAlignment.Right
        };
        _formFeedbackPresenter.SetCurrentValue(ContentPresenter.ContentProperty, _owner.FormFeedback);
    }

    private void ClearFormFeedbackPresenter()
    {
        if (_formFeedbackPresenter == null)
        {
            return;
        }

        _formFeedbackPresenter.ClearValue(ContentPresenter.ContentProperty);
        Children.Remove(_formFeedbackPresenter);
        _formFeedbackPresenter = null;
        NotifyDecoratedBoxContentChanged();
    }

    private void UpdateContentRightAddOnPresenter()
    {
        if (_owner == null)
        {
            return;
        }

        if (_owner.ContentRightAddOn == null)
        {
            ClearContentRightAddOnPresenter();
            return;
        }

        _contentRightAddOnPresenter ??= new ContentPresenter
        {
            Name = "PART_ContentRightAddOnPresenter"
        };
        _contentRightAddOnPresenter.SetCurrentValue(ContentPresenter.ContentProperty, _owner.ContentRightAddOn);
        _contentRightAddOnPresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty,
            _owner.ContentRightAddOnTemplate);
    }

    private void ClearContentRightAddOnPresenter()
    {
        if (_contentRightAddOnPresenter == null)
        {
            return;
        }

        _contentRightAddOnPresenter.ClearValue(ContentPresenter.ContentProperty);
        _contentRightAddOnPresenter.ClearValue(ContentPresenter.ContentTemplateProperty);
        Children.Remove(_contentRightAddOnPresenter);
        _contentRightAddOnPresenter = null;
        NotifyDecoratedBoxContentChanged();
    }

    private bool IsFormFeedbackVisible()
    {
        return _owner?.FormFeedback is { ValidateStatus: not FormValidateStatus.Default };
    }

    private void HandleClearButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        _owner?.NotifyAccessoryClearButtonClicked();
    }

    private void EnsureChildOrder()
    {
        var changed = false;
        var index   = 0;
        changed |= EnsureChildAt(_clearButton, ref index);
        changed |= EnsureChildAt(_infoIconPresenter, ref index);
        changed |= EnsureChildAt(_formFeedbackPresenter, ref index);
        changed |= EnsureChildAt(_contentRightAddOnPresenter, ref index);

        if (changed)
        {
            NotifyDecoratedBoxContentChanged();
        }
    }

    private bool EnsureChildAt(Control? child, ref int index)
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
