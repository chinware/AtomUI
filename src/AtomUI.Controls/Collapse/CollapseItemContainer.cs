using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

internal class CollapseItemContainer : ContentPresenter, ISelectable
{
    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<CollapseItemContainer>();
    
    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    
    static CollapseItemContainer()
    {
        SelectableMixin.Attach<CollapseItemContainer>(IsSelectedProperty);
        PressedMixin.Attach<CollapseItemContainer>();
        FocusableProperty.OverrideDefaultValue(typeof(CollapseItemContainer), true);
    }
}