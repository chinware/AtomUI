using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class ComboBoxAccessoryHost : StackPanel
{
    private ComboBox? _owner;
    private ContentPresenter? _contentRightAddOnPresenter;
    private ContentPresenter? _formFeedbackPresenter;
    private ComboBoxHandle? _comboBoxHandle;
    private CompositeDisposable? _ownerSubscriptions;
    private bool _isAttachingOwner;

    public ComboBoxAccessoryHost()
    {
        Orientation = Avalonia.Layout.Orientation.Horizontal;
    }

    public void AttachOwner(ComboBox owner)
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
            owner.GetObservable(ComboBox.ContentRightAddOnProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(ComboBox.ContentRightAddOnTemplateProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(ComboBox.FormFeedbackProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(ComboBox.IsFormFeedbackVisibleProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(InputElement.IsEnabledProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(ComboBox.IsMotionEnabledProperty).Subscribe(_ => HandleHandleStateChanged())
        };

        _isAttachingOwner = false;
        UpdateAccessoryState();
    }

    public void DetachOwner()
    {
        _ownerSubscriptions?.Dispose();
        _ownerSubscriptions = null;

        ClearContentRightAddOnPresenter();
        ClearFormFeedbackPresenter();
        ClearComboBoxHandle();
        Children.Clear();
        _owner = null;
        _isAttachingOwner = false;
    }

    private void HandleAccessoryStateChanged()
    {
        if (!_isAttachingOwner)
        {
            UpdateAccessoryState();
        }
    }

    private void HandleHandleStateChanged()
    {
        if (!_isAttachingOwner)
        {
            UpdateHandleState();
        }
    }

    private void UpdateAccessoryState()
    {
        UpdateContentRightAddOnPresenter();
        UpdateFormFeedbackPresenter();
        UpdateComboBoxHandle();
        EnsureChildOrder();
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

    private void UpdateFormFeedbackPresenter()
    {
        if (_owner == null)
        {
            return;
        }

        if (!_owner.IsFormFeedbackVisible || _owner.FormFeedback == null)
        {
            ClearFormFeedbackPresenter();
            return;
        }

        _formFeedbackPresenter ??= new ContentPresenter
        {
            Name                     = "PART_FormFeedBack",
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

    private void UpdateComboBoxHandle()
    {
        if (_comboBoxHandle == null)
        {
            _comboBoxHandle = new ComboBoxHandle();
            _comboBoxHandle.HandleClick += HandleComboBoxHandleClick;
        }
        UpdateHandleState();
    }

    private void ClearComboBoxHandle()
    {
        if (_comboBoxHandle == null)
        {
            return;
        }

        _comboBoxHandle.HandleClick -= HandleComboBoxHandleClick;
        Children.Remove(_comboBoxHandle);
        _comboBoxHandle = null;
        NotifyDecoratedBoxContentChanged();
    }

    private void UpdateHandleState()
    {
        if (_owner == null || _comboBoxHandle == null)
        {
            return;
        }

        _comboBoxHandle.SetCurrentValue(InputElement.IsEnabledProperty, _owner.IsEnabled);
        _comboBoxHandle.SetCurrentValue(ComboBoxHandle.IsMotionEnabledProperty, _owner.IsMotionEnabled);
    }

    private void HandleComboBoxHandleClick(object? sender, EventArgs e)
    {
        _owner?.NotifyAccessoryHandleClicked();
    }

    private void EnsureChildOrder()
    {
        var changed = false;
        var index   = 0;
        changed |= EnsureChildAt(_contentRightAddOnPresenter, ref index);
        changed |= EnsureChildAt(_formFeedbackPresenter, ref index);
        changed |= EnsureChildAt(_comboBoxHandle, ref index);

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
