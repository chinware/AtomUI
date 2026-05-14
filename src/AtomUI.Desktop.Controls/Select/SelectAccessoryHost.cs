using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class SelectAccessoryHost : StackPanel
{
    private AbstractSelect? _owner;
    private AddOnDecoratedBox? _addOnDecoratedBox;
    private SelectMaxCountIndicator? _maxCountIndicator;
    private ContentPresenter? _contentRightAddOnPresenter;
    private SelectHandle? _selectHandle;
    private CompositeDisposable? _ownerSubscriptions;
    private CompositeDisposable? _decoratedBoxSubscriptions;
    private bool _isAttachingOwner;

    public SelectAccessoryHost()
    {
        Orientation = Avalonia.Layout.Orientation.Horizontal;
    }

    public void AttachOwner(AbstractSelect owner, AddOnDecoratedBox? addOnDecoratedBox)
    {
        if (ReferenceEquals(_owner, owner) &&
            ReferenceEquals(_addOnDecoratedBox, addOnDecoratedBox))
        {
            return;
        }

        DetachOwner();

        _owner             = owner;
        _addOnDecoratedBox = addOnDecoratedBox;
        _isAttachingOwner  = true;
        _ownerSubscriptions = new CompositeDisposable
        {
            owner.GetObservable(AbstractSelect.IsShowMaxCountIndicatorProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(AbstractSelect.MaxCountProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(AbstractSelect.SelectedCountProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(AbstractSelect.ContentRightAddOnProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(AbstractSelect.ContentRightAddOnTemplateProperty).Subscribe(_ => HandleAccessoryStateChanged()),
            owner.GetObservable(AbstractSelect.FormFeedbackProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(AbstractSelect.SuffixLoadingIconProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(AbstractSelect.SuffixIconProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(AbstractSelect.IsFilterEnabledProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(InputElement.IsEnabledProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(AbstractSelect.IsMotionEnabledProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(AbstractSelect.IsLoadingProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(AbstractSelect.IsAllowClearProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(AbstractSelect.IsSelectionEmptyProperty).Subscribe(_ => HandleHandleStateChanged()),
            owner.GetObservable(AbstractSelect.IsDropDownOpenProperty).Subscribe(_ => HandleHandleStateChanged())
        };

        if (owner is Select select)
        {
            _ownerSubscriptions.Add(select.GetObservable(Select.IsEffectiveFilterEnabledProperty)
                                          .Subscribe(_ => HandleHandleStateChanged()));
        }

        if (addOnDecoratedBox != null)
        {
            _decoratedBoxSubscriptions = new CompositeDisposable
            {
                addOnDecoratedBox.GetObservable(AddOnDecoratedBox.IsInnerBoxHoverProperty)
                                 .Subscribe(_ => HandleHandleInputStateChanged()),
                addOnDecoratedBox.GetObservable(AddOnDecoratedBox.IsInnerBoxPressedProperty)
                                 .Subscribe(_ => HandleHandleInputStateChanged())
            };
        }

        _isAttachingOwner = false;
        UpdateAccessoryState();
    }

    public void DetachOwner()
    {
        _ownerSubscriptions?.Dispose();
        _ownerSubscriptions = null;

        _decoratedBoxSubscriptions?.Dispose();
        _decoratedBoxSubscriptions = null;

        ClearMaxCountIndicator();
        ClearContentRightAddOnPresenter();
        ClearSelectHandle();
        Children.Clear();

        _owner             = null;
        _addOnDecoratedBox = null;
        _isAttachingOwner  = false;
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

    private void HandleHandleInputStateChanged()
    {
        if (!_isAttachingOwner)
        {
            UpdateHandleInputState();
        }
    }

    private void UpdateAccessoryState()
    {
        UpdateMaxCountIndicator();
        UpdateContentRightAddOnPresenter();
        UpdateSelectHandle();
        EnsureChildOrder();
    }

    private void UpdateMaxCountIndicator()
    {
        if (_owner == null)
        {
            return;
        }

        if (!_owner.IsShowMaxCountIndicator)
        {
            ClearMaxCountIndicator();
            return;
        }

        _maxCountIndicator ??= new SelectMaxCountIndicator();
        _maxCountIndicator.SetCurrentValue(SelectMaxCountIndicator.MaxCountProperty, _owner.MaxCount);
        _maxCountIndicator.SetCurrentValue(SelectMaxCountIndicator.SelectedCountProperty, _owner.SelectedCount);
    }

    private void ClearMaxCountIndicator()
    {
        if (_maxCountIndicator == null)
        {
            return;
        }

        Children.Remove(_maxCountIndicator);
        _maxCountIndicator = null;
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

    private void UpdateSelectHandle()
    {
        _selectHandle ??= new SelectHandle();
        UpdateHandleState();
    }

    private void ClearSelectHandle()
    {
        if (_selectHandle == null)
        {
            return;
        }

        _selectHandle.ClearValue(SelectHandle.FormFeedbackProperty);
        _selectHandle.ClearValue(SelectHandle.LoadingIconProperty);
        _selectHandle.ClearValue(SelectHandle.OpenIndicatorProperty);
        Children.Remove(_selectHandle);
        _selectHandle = null;
        NotifyDecoratedBoxContentChanged();
    }

    private void UpdateHandleState()
    {
        if (_owner == null || _selectHandle == null)
        {
            return;
        }

        _selectHandle.SetCurrentValue(SelectHandle.FormFeedbackProperty, _owner.FormFeedback);
        _selectHandle.SetCurrentValue(SelectHandle.LoadingIconProperty, _owner.SuffixLoadingIcon);
        _selectHandle.SetCurrentValue(SelectHandle.OpenIndicatorProperty, _owner.SuffixIcon);
        _selectHandle.SetCurrentValue(SelectHandle.IsFilterEnabledProperty, GetEffectiveFilterEnabled());
        _selectHandle.SetCurrentValue(InputElement.IsEnabledProperty, _owner.IsEnabled);
        _selectHandle.SetCurrentValue(SelectHandle.IsMotionEnabledProperty, _owner.IsMotionEnabled);
        _selectHandle.SetCurrentValue(SelectHandle.IsLoadingProperty, _owner.IsLoading);
        _selectHandle.SetCurrentValue(SelectHandle.IsAllowClearProperty, _owner.IsAllowClear);
        _selectHandle.SetCurrentValue(SelectHandle.IsSelectionEmptyProperty, _owner.IsSelectionEmpty);
        _selectHandle.SetCurrentValue(SelectHandle.IsDropDownOpenProperty, _owner.IsDropDownOpen);
        UpdateHandleInputState();
    }

    private void UpdateHandleInputState()
    {
        if (_selectHandle == null)
        {
            return;
        }

        _selectHandle.SetCurrentValue(SelectHandle.IsInputHoverProperty,
            _addOnDecoratedBox?.IsInnerBoxHover ?? false);
        _selectHandle.SetCurrentValue(SelectHandle.IsInputPressedProperty,
            _addOnDecoratedBox?.IsInnerBoxPressed ?? false);
    }

    private bool GetEffectiveFilterEnabled()
    {
        return _owner is Select select ? select.IsEffectiveFilterEnabled : _owner?.IsFilterEnabled == true;
    }

    private void EnsureChildOrder()
    {
        var changed = false;
        var index   = 0;
        changed |= EnsureChildAt(_maxCountIndicator, ref index);
        changed |= EnsureChildAt(_contentRightAddOnPresenter, ref index);
        changed |= EnsureChildAt(_selectHandle, ref index);

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
