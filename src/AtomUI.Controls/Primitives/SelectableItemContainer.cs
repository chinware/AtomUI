using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls.Primitives;

internal class SelectableItemContainer  : ContentPresenter, ISelectable
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<SelectableItemContainer>();
    
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    
    static SelectableItemContainer()
    {
        SelectableMixin.Attach<SelectableItemContainer>(IsSelectedProperty);
        PressedMixin.Attach<SelectableItemContainer>();
        FocusableProperty.OverrideDefaultValue(typeof(SelectableItemContainer), true);
    }
}