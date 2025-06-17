using AtomUI.Animations;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

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

    private Border? _itemDecorator;
    private ContentPresenter? _headerPresenterPart;
    
    static MenuItem()
    {
        AffectsRender<MenuItem>(BackgroundProperty);
        AffectsMeasure<MenuItem>(IconProperty);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ParentProperty)
        {
            UpdatePseudoClasses();
        }
        
        if (change.Property == IconProperty)
        {
            if (change.OldValue is Icon oldIcon)
            {
                oldIcon.SetTemplatedParent(null);
            }

            if (change.NewValue is Icon newIcon)
            {
                LogicalChildren.Remove(newIcon);
                newIcon.SetTemplatedParent(this);
            }
        }

        if (change.Property == IsMotionEnabledProperty)
        {
            ConfigureTransitions();
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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemDecorator       = e.NameScope.Find<Border>(MenuItemThemeConstants.ItemDecoratorPart);
        _headerPresenterPart = e.NameScope.Find<ContentPresenter>(TopLevelMenuItemThemeConstants.HeaderPresenterPart);
        UpdatePseudoClasses();
        ConfigureTransitions();
    }
    
    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (_itemDecorator != null)
            {
                _itemDecorator.Transitions = new Transitions()
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(Border.BackgroundProperty)
                };
            }

            if (IsTopLevel)
            {
                if (_headerPresenterPart != null)
                {
                    _headerPresenterPart.Transitions = new Transitions()
                    {
                        TransitionUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.BackgroundProperty),
                        TransitionUtils.CreateTransition<SolidColorBrushTransition>(ContentPresenter.ForegroundProperty)
                    };
                }
            }
        }
        else
        {
            if (_itemDecorator != null)
            {
                _itemDecorator.Transitions?.Clear();
                _itemDecorator.Transitions = null;
            }

            if (_headerPresenterPart != null)
            {
                _headerPresenterPart.Transitions?.Clear();
                _headerPresenterPart.Transitions = null;
            }
        }
    }
}