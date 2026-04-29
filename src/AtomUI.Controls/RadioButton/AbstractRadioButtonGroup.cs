using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls.Commons;

public abstract class AbstractRadioButtonGroup : ItemsControl,
                                                IMotionAwareControl,
                                                IFormItemAware
{
    #region 公共属性定义
    public static readonly StyledProperty<object?> CheckedItemProperty = 
        AvaloniaProperty.Register<AbstractRadioButtonGroup, object?>(nameof (CheckedItem));
    
    public static readonly StyledProperty<double> ItemSpacingProperty = 
        AvaloniaProperty.Register<AbstractRadioButtonGroup, double>(nameof (ItemSpacing));
    
    public static readonly StyledProperty<double> LineSpacingProperty = 
        AvaloniaProperty.Register<AbstractRadioButtonGroup, double>(nameof (LineSpacing));
    
    public static readonly StyledProperty<Orientation> OrientationProperty = 
        StackPanel.OrientationProperty.AddOwner<AbstractRadioButtonGroup>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractRadioButtonGroup>();
    
    public object? CheckedItem
    {
        get => GetValue(CheckedItemProperty);
        set => SetValue(CheckedItemProperty, value);
    }
    
    public double ItemSpacing
    {
        get => GetValue(ItemSpacingProperty);
        set => SetValue(ItemSpacingProperty, value);
    }
    
    public double LineSpacing
    {
        get => GetValue(LineSpacingProperty);
        set => SetValue(LineSpacingProperty, value);
    }
    
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    #endregion

    #region 公共事件定义

    public event EventHandler<RadioButtonGroupCheckedChangedEventArgs>? CheckedChanged;

    #endregion
    
    private bool _ignoreSyncChecked;

    static AbstractRadioButtonGroup()
    {
        OrientationProperty.OverrideDefaultValue<AbstractRadioButtonGroup>(Orientation.Horizontal);
        AbstractRadioButton.IsCheckedChangedEvent.AddClassHandler<AbstractRadioButtonGroup>((group, args) => group.HandleRadioButtonCheckedChanged(args));
        CheckedItemProperty.Changed.AddClassHandler<AbstractRadioButtonGroup>((group, args) => group.HandleCheckedItemChanged(args));
    }
    
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AbstractRadioButton>(item, out recycleKey);
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is AbstractRadioButton radioButton)
        {
            if (item != null && item is not Visual)
            {
                {
                    if (ItemTemplate != null)
                    {
                        radioButton.SetCurrentValue(AbstractRadioButton.ContentProperty, item);
                    }
                    else
                    {
                        if (item is IRadioButtonOption radioButtonOption)
                        {
                            radioButton.SetCurrentValue(AbstractRadioButton.ContentProperty, radioButtonOption.Content);
                        }
                    }
                }

                {
                    if (item is IRadioButtonOption radioButtonOption)
                    {
                        radioButton.SetCurrentValue(AbstractRadioButton.IsEnabledProperty, radioButtonOption.IsEnabled);
                        radioButton.SetCurrentValue(AbstractRadioButton.IsCheckedProperty, radioButtonOption.IsChecked);
                    }
                }
                if (ItemTemplate != null)
                {
                    radioButton[!AbstractRadioButton.ContentTemplateProperty] = this[!ItemTemplateProperty];
                }
            }
            radioButton[!IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            
            PrepareRadioButton(radioButton, item, index);
        }  
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type RadioButton.");
        }
    }
    
    protected virtual void PrepareRadioButton(AbstractRadioButton radioButton, object? item, int index)
    {
    }
    
    private void HandleRadioButtonCheckedChanged(RoutedEventArgs args)
    {
        if (args.Source is AbstractRadioButton radioButton && radioButton.IsChecked == true)
        {
            _ignoreSyncChecked = true;
            if (ItemsSource != null)
            {
                SetCurrentValue(CheckedItemProperty, radioButton.DataContext);
            }
            else
            {
                SetCurrentValue(CheckedItemProperty, radioButton);
            }
            args.Handled = true;
        }
    }

    private void SyncCheckedState()
    {
        var isSourceMode = ItemsSource != null;
        foreach (var radioButton in LogicalChildren.OfType<AbstractRadioButton>())
        {
            if (isSourceMode)
            {
                if (radioButton.DataContext == CheckedItem)
                {
                    radioButton.SetCurrentValue(AbstractRadioButton.IsCheckedProperty, true);
                }
            }
            else
            {
                if (radioButton == CheckedItem)
                {
                    radioButton.SetCurrentValue(AbstractRadioButton.IsCheckedProperty, true);
                }
            }
        }
    }

    private void HandleCheckedItemChanged(AvaloniaPropertyChangedEventArgs args)
    {
        CheckedChanged?.Invoke(this, new RadioButtonGroupCheckedChangedEventArgs(args.OldValue, args.NewValue));
        _formValueChanged?.Invoke(this, EventArgs.Empty);
        if (_ignoreSyncChecked)
        {
            _ignoreSyncChecked = false;
            return;
        }
        SyncCheckedState();
    }
    
    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is AbstractRadioButton radioButton)
        {
            if (item == CheckedItem)
            {
                radioButton.SetCurrentValue(AbstractRadioButton.IsCheckedProperty, true);
            }
        }
    }
    
    #region 实现 FormItem 接口
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value);

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);

    protected virtual void NotifySetFormValue(object? value)
    {
        SetCurrentValue(CheckedItemProperty, value);
    }

    protected virtual object? NotifyGetFormValue()
    {
        return CheckedItem;
    }

    protected virtual void NotifyClearFormValue()
    {
        SetCurrentValue(CheckedItemProperty, null);
    }
    
    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }
    #endregion
}