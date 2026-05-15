using AtomUI.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace AtomUI.Controls.Commons;

using AvaloniaCheckBox = Avalonia.Controls.CheckBox;

public abstract class AbstractCheckBox : AvaloniaCheckBox, 
                                         IWaveSpiritAwareControl,
                                         IFormItemAware
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractCheckBox>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<AbstractCheckBox>();
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    #endregion

    private DockPanel? _contentLayout;
    private ContentPresenter? _contentPresenter;
    private CheckBoxIndicator? _indicator;

    static AbstractCheckBox()
    {
        IsCheckedChangedEvent.AddClassHandler<AbstractCheckBox>((checkbox, args) => checkbox.HandleCheckedChanged());
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        DetachContentPresenter();
        _indicator = null;
        base.OnApplyTemplate(e);

        _contentLayout = e.NameScope.Find<DockPanel>("PART_ContentLayout");
        _indicator     = e.NameScope.Find<CheckBoxIndicator>("Indicator");
        SyncIndicator();
        UpdateContentPresenter();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ContentControl.ContentProperty ||
            change.Property == ContentControl.ContentTemplateProperty)
        {
            UpdateContentPresenter();
        }
        if (change.Property == IsCheckedProperty ||
            change.Property == IsEnabledProperty ||
            change.Property == IsMotionEnabledProperty ||
            change.Property == IsWaveSpiritEnabledProperty)
        {
            SyncIndicator();
        }
    }

    private void SyncIndicator()
    {
        if (_indicator == null)
        {
            return;
        }

        _indicator.SetCurrentValue(CheckBoxIndicator.StateProperty, GetIndicatorState(IsChecked));
        _indicator.SetCurrentValue(CheckBoxIndicator.IsEnabledProperty, IsEnabled);
        _indicator.SetCurrentValue(CheckBoxIndicator.IsMotionEnabledProperty, IsMotionEnabled);
        _indicator.SetCurrentValue(CheckBoxIndicator.IsWaveSpiritEnabledProperty, IsWaveSpiritEnabled);
    }

    private static CheckBoxIndicatorState GetIndicatorState(bool? isChecked)
    {
        return isChecked switch
        {
            true => CheckBoxIndicatorState.Checked,
            null => CheckBoxIndicatorState.Indeterminate,
            _ => CheckBoxIndicatorState.Unchecked
        };
    }

    private void UpdateContentPresenter()
    {
        if (_contentLayout == null)
        {
            return;
        }

        if (!ShouldShowContentPresenter())
        {
            DetachContentPresenter();
            return;
        }

        if (_contentPresenter == null)
        {
            _contentPresenter = new ContentPresenter
            {
                Name              = "ContentPresenter",
                VerticalAlignment = VerticalAlignment.Center
            };
            _contentPresenter.SetTemplatedParent(this);
            _contentLayout.Children.Add(_contentPresenter);
        }

        _contentPresenter.SetCurrentValue(ContentPresenter.ContentProperty, Content);
        _contentPresenter.SetCurrentValue(ContentPresenter.ContentTemplateProperty, ContentTemplate);
    }

    private bool ShouldShowContentPresenter()
    {
        return Content != null || ContentTemplate != null;
    }

    private void DetachContentPresenter()
    {
        if (_contentPresenter == null)
        {
            return;
        }

        _contentPresenter.ClearValue(ContentPresenter.ContentProperty);
        _contentPresenter.ClearValue(ContentPresenter.ContentTemplateProperty);

        if (_contentPresenter.GetVisualParent() is Panel parent)
        {
            parent.Children.Remove(_contentPresenter);
        }
        else
        {
            _contentLayout?.Children.Remove(_contentPresenter);
        }

        _contentPresenter.SetTemplatedParent(null);
        _contentPresenter = null;
    }
    
    #region 实现 FormItem 接口
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue((bool?)value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    
    private void HandleCheckedChanged()
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(bool? value)
    {
        SetCurrentValue(IsCheckedProperty, value);
    }

    protected virtual bool? NotifyGetFormValue()
    {
        return IsChecked;
    }

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(IsCheckedProperty, null);
    }
    
    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }
    #endregion
}
