using AtomUI.Data;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using AnimationUtils = AtomUI.Utils.AnimationUtils;

namespace AtomUI.Controls;

using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

[PseudoClasses(TopLevelPC)]
public class MenuItem : AvaloniaMenuItem
{
    public const string TopLevelPC = ":toplevel";

    #region 公共属性定义

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        Menu.SizeTypeProperty.AddOwner<MenuItem>();

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    #endregion
    
    private ContentPresenter? _topLevelContentPresenter;
    private ContentControl? _togglePresenter;

    internal static PlatformKeyGestureConverter KeyGestureConverter = new();

    static MenuItem()
    {
        AffectsRender<MenuItem>(BackgroundProperty);
    }
    
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Cursor              = new Cursor(StandardCursorType.Hand);
        HorizontalAlignment = HorizontalAlignment.Stretch;
        HandleTemplateApplied(e.NameScope);
    }

    private void SetupTransitions()
    {
        if (_topLevelContentPresenter is not null && _topLevelContentPresenter.Transitions is null)
        {
            var transitions = new Transitions();
            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
            _topLevelContentPresenter.Transitions = transitions;
        }
    }

    private void HandleTemplateApplied(INameScope scope)
    {
        if (IsTopLevel)
        {
            _topLevelContentPresenter = scope.Find<ContentPresenter>(TopLevelMenuItemTheme.HeaderPresenterPart);
        }
        else
        {
            _togglePresenter = scope.Find<ContentControl>(MenuItemTheme.TogglePresenterPart);
        }

        HandleToggleTypeChanged();
        SetupTransitions();
        UpdatePseudoClasses();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == ParentProperty)
        {
            UpdatePseudoClasses();
        }
        else if (e.Property == IconProperty)
        {
            if (Icon is not null && Icon is IconPkg.Icon icon)
            {
                TokenResourceBinder.CreateTokenBinding(icon, WidthProperty, MenuTokenResourceKey.ItemIconSize);
                TokenResourceBinder.CreateTokenBinding(icon, HeightProperty, MenuTokenResourceKey.ItemIconSize);
                TokenResourceBinder.CreateTokenBinding(icon, IconPkg.Icon.NormalFilledBrushProperty,
                    MenuTokenResourceKey.ItemColor);
            }
        }
        else if (e.Property == ToggleTypeProperty)
        {
            HandleToggleTypeChanged();
        }
    }

    private void HandleToggleTypeChanged()
    {
        if (IsTopLevel || _togglePresenter is null)
        {
            return;
        }

        if (ToggleType == MenuItemToggleType.None)
        {
            if (_togglePresenter.Presenter is not null)
            {
                _togglePresenter.Presenter.IsVisible = false;
            }
        }
        else if (ToggleType == MenuItemToggleType.CheckBox)
        {
            var checkbox = new CheckBox();
            BindUtils.RelayBind(this, IsCheckedProperty, checkbox, ToggleButton.IsCheckedProperty);
            _togglePresenter.Content   = checkbox;
            _togglePresenter.IsVisible = true;
        }
        else if (ToggleType == MenuItemToggleType.Radio)
        {
            var radioButton = new RadioButton();
            BindUtils.RelayBind(this, IsCheckedProperty, radioButton, ToggleButton.IsCheckedProperty);
            BindUtils.RelayBind(this, GroupNameProperty, radioButton, Avalonia.Controls.RadioButton.GroupNameProperty);
            _togglePresenter.Content   = radioButton;
            _togglePresenter.IsVisible = true;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(TopLevelPC, IsTopLevel);
    }
}