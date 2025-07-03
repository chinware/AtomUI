using AtomUI.Animations;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;
using ButtonSizeType = SizeType;

[PseudoClasses(ButtonPseudoClass.Visited,
    ButtonPseudoClass.IconOnly,
    ButtonPseudoClass.Loading,
    ButtonPseudoClass.IsDanger)]
public class HyperLinkButton : AvaloniaButton,
                               ISizeTypeAware,
                               IControlSharedTokenResourcesHost,
                               IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsDangerProperty =
        AvaloniaProperty.Register<HyperLinkButton, bool>(nameof(IsDanger));
    
    public static readonly StyledProperty<bool> IsGhostProperty =
        AvaloniaProperty.Register<HyperLinkButton, bool>(nameof(IsGhost));
    
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<HyperLinkButton, bool>(nameof(IsLoading));
    
    public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<HyperLinkButton>();

    public static readonly StyledProperty<Icon?> IconProperty = 
        AvaloniaProperty.Register<HyperLinkButton, Icon?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<HyperLinkButton>();

    public static readonly StyledProperty<bool> IsVisitedProperty =
        AvaloniaProperty.Register<HyperLinkButton, bool>(
            nameof(IsVisited),
            defaultValue: false);

    public static readonly StyledProperty<Uri?> NavigateUriProperty =
        AvaloniaProperty.Register<HyperLinkButton, Uri?>(
            nameof(NavigateUri),
            defaultValue: null);
    
    public bool IsDanger
    {
        get => GetValue(IsDangerProperty);
        set => SetValue(IsDangerProperty, value);
    }
    
    public bool IsGhost
    {
        get => GetValue(IsGhostProperty);
        set => SetValue(IsGhostProperty, value);
    }
    
    public bool IsLoading
    {
        get => GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }
    
    public ButtonSizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    public Icon? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsVisited
    {
        get => GetValue(IsVisitedProperty);
        set => SetValue(IsVisitedProperty, value);
    }

    public Uri? NavigateUri
    {
        get => GetValue(NavigateUriProperty);
        set => SetValue(NavigateUriProperty, value);
    }

    #endregion
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;

    static HyperLinkButton()
    {
        AffectsMeasure<HyperLinkButton>(SizeTypeProperty,
            IconProperty);
        AffectsRender<HyperLinkButton>(IsDangerProperty,
            IsGhostProperty);
    }

    public HyperLinkButton()
    {
        this.RegisterResources();
        this.BindMotionProperties();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsVisitedProperty ||
            change.Property == ContentProperty ||
            change.Property == IsLoadingProperty)
        {
            UpdatePseudoClasses();
        }
        if (this.IsAttachedToVisualTree())
        { 
            if (change.Property == IsMotionEnabledProperty)
            {
                ConfigureTransitions();
            }
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ButtonPseudoClass.Visited, IsVisited);
    }

    protected override void OnClick()
    {
        base.OnClick();

        Uri? uri = NavigateUri;
        if (uri is not null)
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                bool success = await TopLevel.GetTopLevel(this)!.Launcher.LaunchUriAsync(uri);
                if (success)
                {
                    SetCurrentValue(IsVisitedProperty, true);
                }
            });
        }
    }
    
    private void ConfigureTransitions()
    {
        if (IsMotionEnabled)
        {
            if (Transitions is null)
            {
                var transitions = new Transitions
                {
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
                    TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
                };

                Transitions = transitions;
            }
        }
        else
        {
            Transitions?.Clear();
            Transitions = null;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureTransitions();
    }
}