using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Controls.Commons;

internal abstract class AbstractCheckBoxItemsControl : SelectingItemsControl
{
    public static readonly StyledProperty<double> ItemSpacingProperty = 
        AbstractCheckBoxGroup.ItemSpacingProperty.AddOwner<AbstractCheckBoxItemsControl>();
    
    public static readonly StyledProperty<double> LineSpacingProperty = 
        AbstractCheckBoxGroup.LineSpacingProperty.AddOwner<AbstractCheckBoxItemsControl>();
    
    public static readonly StyledProperty<Orientation> OrientationProperty = 
        StackPanel.OrientationProperty.AddOwner<AbstractCheckBoxItemsControl>();

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractCheckBoxItemsControl>();
    
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

    static AbstractCheckBoxItemsControl()
    {
        SelectionModeProperty.OverrideDefaultValue<AbstractCheckBoxItemsControl>(SelectionMode.Multiple | SelectionMode.Toggle);
        AbstractCheckBox.IsCheckedChangedEvent.AddClassHandler<AbstractCheckBoxItemsControl>((group, args) => group.HandleCheckBoxCheckedChanged(args));
    }
    
    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<AbstractCheckBox>(item, out recycleKey);
    }
    
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is AbstractCheckBox checkbox)
        {
            if (item != null && item is not Visual)
            {
                {
                    if (ItemTemplate != null)
                    {
                        checkbox.SetCurrentValue(AbstractCheckBox.ContentProperty, item);
                    }
                    else
                    {
                        if (item is ICheckBoxOption checkBoxOption)
                        {
                            checkbox.SetCurrentValue(AbstractCheckBox.ContentProperty, checkBoxOption.Content);
                        }
                    }
                }

                {
                    if (item is ICheckBoxOption checkBoxOption)
                    {
                        checkbox.SetCurrentValue(AbstractCheckBox.IsEnabledProperty, checkBoxOption.IsEnabled);
                        var isChecked = checkBoxOption.IsChecked || ContainsSelectedItem(item);
                        checkbox.SetCurrentValue(AbstractCheckBox.IsCheckedProperty, isChecked);
                        if (isChecked && SelectedItems?.Contains(checkBoxOption) != true)
                        {
                            SelectedItems?.Add(checkBoxOption);
                        }
                    }
                }
                if (ItemTemplate != null)
                {
                    checkbox[!AbstractCheckBox.ContentTemplateProperty] = this[!ItemTemplateProperty];
                }
            }
            checkbox[!IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
            
            PrepareRadioButton(checkbox, item, index);
        }  
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type AbstractCheckBox.");
        }
    }
    
    protected virtual void PrepareRadioButton(AbstractCheckBox checkbox, object? item, int index)
    {
    }
    
    private void HandleCheckBoxCheckedChanged(RoutedEventArgs args)
    {
        if (args.Source is AbstractCheckBox checkBox)
        {
            var index = IndexFromContainer(checkBox);
            if (index != -1)
            {
                if (checkBox.IsChecked == true)
                {
                    Selection.Select(index);
                }
                else
                {
                    Selection.Deselect(index);
                }
            }
            args.Handled = true;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedItemsProperty)
        {
            UpdateCheckBoxCheckedStates();
        }
    }

    private void UpdateCheckBoxCheckedStates()
    {
        foreach (var item in Items)
        {
            if (item == null)
            {
                continue;
            }

            if (ContainerFromItem(item) is AbstractCheckBox checkBox)
            {
                checkBox.SetCurrentValue(AbstractCheckBox.IsCheckedProperty, ContainsSelectedItem(item));
            }
        }
    }

    private bool ContainsSelectedItem(object item)
    {
        return SelectedItems?.Contains(item) == true;
    }
    
    internal IList? CheckedItems
    {
        get => SelectedItems;
        set => SelectedItems = value;
    }
}
