using AtomUI.Data;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaMenu = Avalonia.Controls.Menu;

public class Menu : AvaloniaMenu,
                    ISizeTypeAware
{
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (ItemContainerTheme is null)
        {
            TokenResourceBinder.CreateGlobalResourceBinding(this, ItemContainerThemeProperty, TopLevelMenuItemTheme.ID);
        }
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, menuItem, MenuItem.SizeTypeProperty);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<Menu, SizeType>(nameof(SizeType), SizeType.Middle);

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    #endregion
}