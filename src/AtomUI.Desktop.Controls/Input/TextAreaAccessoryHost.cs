using AtomUI.Controls;
using AtomUI.Controls.Commons;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.VisualTree;
using System.Reactive.Disposables;

namespace AtomUI.Desktop.Controls;

internal class TextAreaAccessoryHost : StackPanel
{
    private TextArea? _owner;
    private CompositeDisposable? _ownerSubscriptions;
    private InputClearIconButton? _clearButton;
    private ContentPresenter? _formFeedbackPresenter;
    private ContentPresenter? _innerRightContentPresenter;
    private bool _isAttachingOwner;

    public TextAreaAccessoryHost()
    {
        Orientation = Orientation.Vertical;
        IsVisible   = false;
    }

    internal void AttachOwner(TextArea owner)
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
            owner.GetObservable(TextArea.IsEffectiveShowClearButtonProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextArea.ClearIconProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextArea.IsMotionEnabledProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextArea.FormFeedbackProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextArea.IsFormFeedbackVisibleProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextBox.InnerRightContentProperty).Subscribe(_ => HandleOwnerPropertyChanged()),
            owner.GetObservable(TextArea.InnerRightContentTemplateProperty).Subscribe(_ => HandleOwnerPropertyChanged())
        };
        _isAttachingOwner = false;
        UpdateAccessoryState();
    }

    internal void DetachOwner()
    {
        _ownerSubscriptions?.Dispose();
        _ownerSubscriptions = null;

        DestroyClearButton();
        DestroyFormFeedbackPresenter();
        DestroyInnerRightContentPresenter();

        Children.Clear();
        _owner    = null;
        IsVisible = false;
    }

    private void HandleOwnerPropertyChanged()
    {
        if (!_isAttachingOwner)
        {
            UpdateAccessoryState();
        }
    }

    private void UpdateAccessoryState()
    {
        if (_owner == null)
        {
            return;
        }

        var visualsChanged = false;
        visualsChanged |= UpdateClearButton();
        visualsChanged |= UpdateFormFeedbackPresenter();
        visualsChanged |= UpdateInnerRightContentPresenter();
        visualsChanged |= EnsureChildOrder();
        IsVisible = Children.Count > 0;
        if (visualsChanged)
        {
            NotifyDecoratedBoxContentChanged();
        }
    }

    private bool UpdateClearButton()
    {
        if (_owner == null || !_owner.IsEffectiveShowClearButton)
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

    private bool UpdateFormFeedbackPresenter()
    {
        if (_owner == null ||
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
                Name                     = "PART_FormFeedBack",
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
        if (_owner == null || _owner.InnerRightContent == null)
        {
            return DestroyInnerRightContentPresenter();
        }

        var visualsChanged = false;
        if (_innerRightContentPresenter == null)
        {
            _innerRightContentPresenter = new ContentPresenter
            {
                Name                     = "PART_InnerRightContentPresenter",
                VerticalAlignment        = VerticalAlignment.Center,
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

    private bool EnsureChildOrder()
    {
        var index          = 0;
        var visualsChanged = false;
        visualsChanged |= PlaceChild(_clearButton, ref index);
        visualsChanged |= PlaceChild(_formFeedbackPresenter, ref index);
        visualsChanged |= PlaceChild(_innerRightContentPresenter, ref index);
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
