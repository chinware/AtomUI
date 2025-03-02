using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using AnimationUtils = AtomUI.Utils.AnimationUtils;

namespace AtomUI.Controls;

using AvaloniaMenuItem = Avalonia.Controls.MenuItem;

[PseudoClasses(TopLevelPC)]
public class MenuItem : AvaloniaMenuItem,
                        ITokenResourceConsumer
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

    #region 内部属性定义

    internal static readonly DirectProperty<MenuItem, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<MenuItem, bool>(nameof(IsMotionEnabled),
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    private bool _isMotionEnabled;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    private CompositeDisposable? _tokenBindingsDisposable;
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
        HorizontalAlignment = HorizontalAlignment.Stretch;
        var scope = e.NameScope;
        if (IsTopLevel)
        {
            _topLevelContentPresenter = scope.Find<ContentPresenter>(TopLevelMenuItemTheme.HeaderPresenterPart);
        }
        else
        {
            _togglePresenter = scope.Find<ContentControl>(MenuItemTheme.TogglePresenterPart);
        }

        SetupIconBindings();
        HandleToggleTypeChanged();
        SetupTransitions();
        UpdatePseudoClasses();
    }

    private void SetupTransitions()
    {
        if (_topLevelContentPresenter is not null)
        {
            if (IsMotionEnabled)
            {
                _topLevelContentPresenter.Transitions ??= new Transitions
                {
                    AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
                };
            }
            else
            {
                _topLevelContentPresenter.Transitions = null;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == ParentProperty)
        {
            UpdatePseudoClasses();
        }
        else if (e.Property == ToggleTypeProperty)
        {
            HandleToggleTypeChanged();
        }
        else if (e.Property == IsMotionEnabledProperty)
        {
            SetupTransitions();
        }

        if (this.IsAttachedToLogicalTree())
        {
            if (e.Property == IconProperty)
            {
                SetupIconBindings();
            }
        }
    }

    private void SetupIconBindings()
    {
        if (Icon is Icon icon)
        {
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(icon, WidthProperty, MenuTokenKey.ItemIconSize));
            this.AddTokenBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(icon, HeightProperty, MenuTokenKey.ItemIconSize));
            this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(icon,
                IconPkg.Icon.NormalFilledBrushProperty,
                MenuTokenKey.ItemColor));
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

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        if (container is MenuItem menuItem)
        {
            BindUtils.RelayBind(this, SizeTypeProperty, menuItem, SizeTypeProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, menuItem, IsMotionEnabledProperty);
        }

        base.PrepareContainerForItemOverride(container, item, index);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}