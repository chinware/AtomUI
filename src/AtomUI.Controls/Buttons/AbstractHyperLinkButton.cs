using AtomUI.Animations;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUI.Controls.Commons;

using AvaloniaButton = Avalonia.Controls.Button;
using ButtonSizeType = SizeType;

[PseudoClasses(ButtonPseudoClass.Visited,
    ButtonPseudoClass.IconOnly,
    ButtonPseudoClass.Loading,
    ButtonPseudoClass.IsDanger)]
public abstract class AbstractHyperLinkButton : AvaloniaButton,
                                                ISizeTypeAware,
                                                IMotionAwareControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<bool> IsDangerProperty =
        AvaloniaProperty.Register<AbstractHyperLinkButton, bool>(nameof(IsDanger));
    
    public static readonly StyledProperty<bool> IsGhostProperty =
        AvaloniaProperty.Register<AbstractHyperLinkButton, bool>(nameof(IsGhost));
    
    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<AbstractHyperLinkButton, bool>(nameof(IsLoading));
    
    public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<AbstractHyperLinkButton>();

    public static readonly StyledProperty<PathIcon?> IconProperty = 
        AvaloniaProperty.Register<AbstractHyperLinkButton, PathIcon?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty = 
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractHyperLinkButton>();

    public static readonly StyledProperty<bool> IsVisitedProperty =
        AvaloniaProperty.Register<AbstractHyperLinkButton, bool>(
            nameof(IsVisited),
            defaultValue: false);

    public static readonly StyledProperty<Uri?> NavigateUriProperty =
        AvaloniaProperty.Register<AbstractHyperLinkButton, Uri?>(
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
    
    public PathIcon? Icon
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

    static AbstractHyperLinkButton()
    {
        AffectsMeasure<AbstractHyperLinkButton>(SizeTypeProperty,
            IconProperty);
        AffectsRender<AbstractHyperLinkButton>(IsDangerProperty,
            IsGhostProperty);
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

    protected override void OnInitialized()
    {
        base.OnInitialized();
        this.DisableTransitions();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Dispatcher.UIThread.Post(this.EnableTransitions);
    }
}