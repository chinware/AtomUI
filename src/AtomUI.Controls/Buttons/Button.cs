using System.Diagnostics;
using AtomUI.Animations;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Themes;
using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;
using ButtonSizeType = SizeType;

public enum ButtonType
{
    Default,
    Dashed,
    Primary,
    Link,
    Text
}

public enum ButtonShape
{
    Default,
    Circle,
    Round
}

[PseudoClasses(ButtonPseudoClass.IconOnly,
    ButtonPseudoClass.Loading,
    ButtonPseudoClass.IsDanger,
    ButtonPseudoClass.DefaultType,
    ButtonPseudoClass.DashedType,
    ButtonPseudoClass.PrimaryType,
    ButtonPseudoClass.LinkType,
    ButtonPseudoClass.TextType)]
public class Button : AvaloniaButton,
                      ISizeTypeAware,
                      IWaveSpiritAwareControl,
                      IControlSharedTokenResourcesHost
{
    #region 公共属性定义

    public static readonly StyledProperty<ButtonType> ButtonTypeProperty =
        AvaloniaProperty.Register<Button, ButtonType>(nameof(ButtonType));

    public static readonly StyledProperty<ButtonShape> ShapeProperty =
        AvaloniaProperty.Register<Button, ButtonShape>(nameof(Shape));

    public static readonly StyledProperty<bool> IsDangerProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsDanger));

    public static readonly StyledProperty<bool> IsGhostProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsGhost));

    public static readonly StyledProperty<bool> IsLoadingProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsLoading));

    public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
        SizeTypeAwareControlProperty.SizeTypeProperty.AddOwner<Button>();

    public static readonly StyledProperty<Icon?> IconProperty =
        AvaloniaProperty.Register<Button, Icon?>(nameof(Icon));

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Button>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Button>();
    
    public ButtonType ButtonType
    {
        get => GetValue(ButtonTypeProperty);
        set => SetValue(ButtonTypeProperty, value);
    }

    public ButtonShape Shape
    {
        get => GetValue(ShapeProperty);
        set => SetValue(ShapeProperty, value);
    }

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

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsIconVisibleProperty =
        AvaloniaProperty.Register<Button, bool>(nameof(IsIconVisible), true);

    internal static readonly StyledProperty<object?> RightExtraContentProperty =
        AvaloniaProperty.Register<Button, object?>(nameof(RightExtraContent));

    internal static readonly StyledProperty<IDataTemplate?> RightExtraContentTemplateProperty =
        AvaloniaProperty.Register<ContentControl, IDataTemplate?>(nameof(RightExtraContentTemplate));

    internal static readonly StyledProperty<Thickness> EffectiveBorderThicknessProperty =
        AvaloniaProperty.Register<Button, Thickness>(
            nameof(EffectiveBorderThickness));
    
    internal static readonly StyledProperty<WaveSpiritType> WaveSpiritTypeProperty =
        WaveSpiritAwareControlProperty.WaveSpiritTypeProperty.AddOwner<Button>();

    internal bool IsIconVisible
    {
        get => GetValue(IsIconVisibleProperty);
        set => SetValue(IsIconVisibleProperty, value);
    }

    internal object? RightExtraContent
    {
        get => GetValue(RightExtraContentProperty);
        set => SetValue(RightExtraContentProperty, value);
    }

    internal IDataTemplate? RightExtraContentTemplate
    {
        get => GetValue(RightExtraContentTemplateProperty);
        set => SetValue(RightExtraContentTemplateProperty, value);
    }

    internal Thickness EffectiveBorderThickness
    {
        get => GetValue(EffectiveBorderThicknessProperty);
        set => SetValue(EffectiveBorderThicknessProperty, value);
    }
    
    internal WaveSpiritType WaveSpiritType
    {
        get => GetValue(WaveSpiritTypeProperty);
        set => SetValue(WaveSpiritTypeProperty, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ButtonToken.ID;
    
    #endregion
    
    protected bool ThemeConfigured;
    private IDisposable? _borderThicknessDisposable;
    private WaveSpiritDecorator? _waveSpiritDecorator;

    static Button()
    {
        AffectsMeasure<Button>(SizeTypeProperty,
            ShapeProperty,
            IconProperty);
        AffectsRender<Button>(ButtonTypeProperty,
            IsDangerProperty,
            IsGhostProperty);
    }

    public Button()
    {
        this.RegisterResources();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size         = base.MeasureOverride(availableSize);
        var targetWidth  = size.Width;
        var targetHeight = size.Height;

        targetWidth = Math.Max(targetWidth, targetHeight);

        if (Shape == ButtonShape.Circle)
        {
            targetWidth  = targetHeight;
            CornerRadius = new CornerRadius(targetHeight);
        }
        else if (Shape == ButtonShape.Round)
        {
            CornerRadius = new CornerRadius(targetHeight);
            targetWidth  = Math.Max(targetWidth, targetHeight + targetHeight / 2);
        }

        return new Size(targetWidth, targetHeight);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsPressedProperty)
        {
            if (!IsLoading &&
                IsWaveSpiritEnabled &&
                (e.OldValue as bool? == true) &&
                (ButtonType == ButtonType.Primary || ButtonType == ButtonType.Default || ButtonType == ButtonType.Dashed))
            {
                Debug.Assert(_waveSpiritDecorator != null);
                
                IBrush? waveBrush = null;
                if (IsDanger)
                {
                    if (ButtonType == ButtonType.Primary && !IsGhost)
                    {
                        waveBrush = Background;
                    }
                    else
                    {
                        waveBrush = Foreground;
                    }
                }

                if (waveBrush != null)
                {
                    _waveSpiritDecorator.WaveBrush = waveBrush;
                }
     
                Dispatcher.UIThread.Post(() =>
                {
                    _waveSpiritDecorator?.Play();
                });
            }
        }

        if (this.IsAttachedToVisualTree())
        {
            if (e.Property == ButtonTypeProperty)
            {
                ConfigureWaveSpiritType();
            }
        }

        if (e.Property == ContentProperty ||
            e.Property == IsLoadingProperty)
        {
            UpdatePseudoClasses();
        }
        else if (e.Property == BorderBrushProperty ||
                 e.Property == ButtonTypeProperty ||
                 e.Property == IsEnabledProperty)
        {
            SetupEffectiveBorderThickness();
        }

        if (IsLoaded)
        {
            if (e.Property == IsMotionEnabledProperty ||
                e.Property == IsWaveSpiritEnabledProperty)
            {
                ConfigureTransitions(true);
            }
        }
        if (e.Property == ButtonTypeProperty)
        {
            ConfigureControlThemeBindings(true);
        }
    }

    private void ConfigureWaveSpiritType()
    {
        WaveSpiritType waveType = default;
        if (Shape == ButtonShape.Default)
        {
            waveType = WaveSpiritType.RoundRectWave;
        }
        else if (Shape == ButtonShape.Round)
        {
            waveType = WaveSpiritType.PillWave;
        }
        else if (Shape == ButtonShape.Circle)
        {
            waveType = WaveSpiritType.CircleWave;
        }

        WaveSpiritType = waveType;
    }

    protected virtual void ConfigureControlThemeBindings(bool force)
    {
        if (!ThemeConfigured || force)
        {
            string? resourceKey = null;
            if (ButtonType == ButtonType.Default)
            {
                resourceKey = DefaultButtonTheme.ID;
            }
            else if (ButtonType == ButtonType.Primary)
            {
                resourceKey = PrimaryButtonTheme.ID;
            }
            else if (ButtonType == ButtonType.Dashed)
            {
                resourceKey = DashedButtonTheme.ID;
            }
            else if (ButtonType == ButtonType.Text)
            {
                resourceKey = TextButtonTheme.ID;
            }
            else if (ButtonType == ButtonType.Link)
            {
                resourceKey = LinkButtonTheme.ID;
            }

            resourceKey ??= DefaultButtonTheme.ID;

            if (Application.Current != null)
            {
                if (Application.Current.TryFindResource(resourceKey, out var resource))
                {
                    if (resource is ControlTheme theme)
                    {
                        Theme = theme;
                    }
                }
            }
         
            ThemeConfigured = true;
        }
    }

    private void ConfigureTransitions(bool force)
    {
        if (IsMotionEnabled)
        {
            if (force || Transitions == null)
            {
                var transitions = new Transitions();
                transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
                if (ButtonType == ButtonType.Primary)
                {
                    if (IsGhost)
                    {
                        transitions.Add(
                            TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
                        transitions.Add(
                            TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                    }
                }
                else if (ButtonType == ButtonType.Default || ButtonType == ButtonType.Dashed)
                {
                    transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
                    transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                }
                else if (ButtonType == ButtonType.Link)
                {
                    transitions.Add(TransitionUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
                }
        
                Transitions = transitions;
            }
        }
        else
        {
            Transitions = null;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _waveSpiritDecorator = e.NameScope.Find<WaveSpiritDecorator>(ButtonThemeConstants.WaveSpiritPart);
        UpdatePseudoClasses();
        ConfigureControlThemeBindings(false);
        ConfigureWaveSpiritType();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureTransitions(false);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        Transitions = null;
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
        SetupEffectiveBorderThickness();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
    }

    private void SetupEffectiveBorderThickness()
    {
        if (ButtonType == ButtonType.Default ||
            ButtonType == ButtonType.Dashed)
        {
            EffectiveBorderThickness = BorderThickness;
        }
        else if (ButtonType == ButtonType.Primary)
        {
            if (IsGhost || !IsEnabled)
            {
                EffectiveBorderThickness = BorderThickness;
            }
            else
            {
                EffectiveBorderThickness = new Thickness(0);
            }
        }
        else
        {
            EffectiveBorderThickness = new Thickness(0);
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(ButtonPseudoClass.IconOnly, Icon is not null && Content is null);
        PseudoClasses.Set(ButtonPseudoClass.Loading, IsLoading);
        PseudoClasses.Set(ButtonPseudoClass.DefaultType, ButtonType == ButtonType.Default);
        PseudoClasses.Set(ButtonPseudoClass.DashedType, ButtonType == ButtonType.Dashed);
        PseudoClasses.Set(ButtonPseudoClass.PrimaryType, ButtonType == ButtonType.Primary);
        PseudoClasses.Set(ButtonPseudoClass.LinkType, ButtonType == ButtonType.Link);
        PseudoClasses.Set(ButtonPseudoClass.TextType, ButtonType == ButtonType.Text);
        PseudoClasses.Set(ButtonPseudoClass.IsDanger, IsDanger);
    }
}