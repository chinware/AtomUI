using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;

namespace AtomUI.Controls;

using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

[PseudoClasses(TopLevelPC)]
public class MenuItem : AvaloniaMenuItem
{
    public const string TopLevelPC = ":toplevel";

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<MenuItem>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsMotionEnabledProperty
        = MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<MenuItem>();

    internal bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    #endregion

    internal static PlatformKeyGestureConverter KeyGestureConverter = new();

    static MenuItem()
    {
        AffectsRender<MenuItem>(BackgroundProperty);
        AffectsMeasure<MenuItem>(IconProperty);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == ParentProperty)
        {
            UpdatePseudoClasses();
        }

        if (e.Property == IconProperty)
        {
            if (e.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }

            if (e.NewValue is Icon newIcon)
            {
                LogicalChildren.Remove(newIcon);
                newIcon.SetTemplatedParent(this);
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(TopLevelPC, IsTopLevel);
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, menuItem, SizeTypeProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, IsMotionEnabledProperty);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdatePseudoClasses();
    }
}