using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public class LineEdit : TextBox,
                        IInputControlStatusAware,
                        IInputControlStyleVariantAware
{
    #region 公共属性定义

    public static readonly StyledProperty<InputControlStyleVariant> StyleVariantProperty =
        InputControlStyleVariantProperty.StyleVariantProperty.AddOwner<LineEdit>();

    public static readonly StyledProperty<InputControlStatus> StatusProperty =
        InputControlStatusProperty.StatusProperty.AddOwner<LineEdit>();
    
    public static readonly StyledProperty<object?> LeftAddOnProperty =
        AddOnDecoratedBox.LeftAddOnProperty.AddOwner<LineEdit>();
    
    public static readonly StyledProperty<IDataTemplate?> LeftAddOnTemplateProperty =
        AddOnDecoratedBox.LeftAddOnTemplateProperty.AddOwner<LineEdit>();

    public static readonly StyledProperty<object?> RightAddOnProperty =
        AddOnDecoratedBox.RightAddOnProperty.AddOwner<LineEdit>();
    
    public static readonly StyledProperty<IDataTemplate?> RightAddOnTemplateProperty =
        AddOnDecoratedBox.RightAddOnTemplateProperty.AddOwner<LineEdit>();
    
    public static readonly StyledProperty<IDataTemplate?> InnerLeftContentTemplateProperty =
        AvaloniaProperty.Register<LineEdit, IDataTemplate?>(nameof(InnerLeftContentTemplate));
    
    public static readonly StyledProperty<IDataTemplate?> InnerRightContentTemplateProperty =
        AvaloniaProperty.Register<LineEdit, IDataTemplate?>(nameof(InnerRightContentTemplate));
    
    public InputControlStyleVariant StyleVariant
    {
        get => GetValue(StyleVariantProperty);
        set => SetValue(StyleVariantProperty, value);
    }

    public InputControlStatus Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }
    
    [DependsOn(nameof(LeftAddOnTemplate))]
    public object? LeftAddOn
    {
        get => GetValue(LeftAddOnProperty);
        set => SetValue(LeftAddOnProperty, value);
    }
    
    public IDataTemplate? LeftAddOnTemplate
    {
        get => GetValue(LeftAddOnTemplateProperty);
        set => SetValue(LeftAddOnTemplateProperty, value);
    }

    [DependsOn(nameof(RightAddOnTemplate))]
    public object? RightAddOn
    {
        get => GetValue(RightAddOnProperty);
        set => SetValue(RightAddOnProperty, value);
    }
    
    public IDataTemplate? RightAddOnTemplate
    {
        get => GetValue(RightAddOnTemplateProperty);
        set => SetValue(RightAddOnTemplateProperty, value);
    }

    public IDataTemplate? InnerLeftContentTemplate
    {
        get => GetValue(InnerLeftContentTemplateProperty);
        set => SetValue(InnerLeftContentTemplateProperty, value);
    }
    
    public IDataTemplate? InnerRightContentTemplate
    {
        get => GetValue(InnerRightContentTemplateProperty);
        set => SetValue(InnerRightContentTemplateProperty, value);
    }
    #endregion

    private AddOnDecoratedBox? _addOnDecoratedBox;
    private LineEditAccessoryHost? _accessoryHost;
    private IDisposable? _accessoryHostSpacingBinding;
    private CompositeDisposable? _contentRightAddOnBindings;
    private bool _isTemplateProvidedAccessoryHost;
    private bool _isUsingLegacyAccessoryTemplate;
    
    public LineEdit()
    {
        this.RegisterTokenResourceScope(LineEditToken.ScopeProvider);
    }
    
    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Error, Status == InputControlStatus.Error);
        PseudoClasses.Set(StdPseudoClass.Warning, Status == InputControlStatus.Warning);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Outline, StyleVariant == InputControlStyleVariant.Outlined);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Filled, StyleVariant == InputControlStyleVariant.Filled);
        PseudoClasses.Set(AddOnDecoratedBoxPseudoClass.Borderless, StyleVariant == InputControlStyleVariant.Borderless);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == StatusProperty ||
            change.Property == LeftAddOnProperty)
        {
            UpdatePseudoClasses();
        }

        if (IsAccessoryStateProperty(change.Property))
        {
            ConfigureOwnerDrivenAccessoryHost();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = null;
        ClearOwnerDrivenAccessoryHost();
        if (_accessoryHost != null)
        {
            _accessoryHost.DetachOwner();
            _accessoryHost = null;
        }

        _addOnDecoratedBox = e.NameScope.Find<AddOnDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);
        _accessoryHost    = e.NameScope.Find<LineEditAccessoryHost>("PART_RightAccessoryHost");
        if (_accessoryHost != null)
        {
            _isTemplateProvidedAccessoryHost = true;
            _isUsingLegacyAccessoryTemplate  = false;
            _accessoryHost.AttachOwner(this);
        }
        else
        {
            _isTemplateProvidedAccessoryHost = false;
            _isUsingLegacyAccessoryTemplate  = SetupContentRightAddOnBindings(e);
            if (!_isUsingLegacyAccessoryTemplate)
            {
                ConfigureOwnerDrivenAccessoryHost();
            }
        }
    }

    private bool SetupContentRightAddOnBindings(TemplateAppliedEventArgs e)
    {
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = null;
        var bindings    = new CompositeDisposable();
        var hasBindings = false;

        if (e.NameScope.Find<InputClearIconButton>("PART_ClearButton") is { } clearButton)
        {
            hasBindings = true;
            bindings.Add(clearButton.Bind(AbstractIconButton.IconProperty,
                new Binding(nameof(ClearIcon)) { Source = this }));
            bindings.Add(clearButton.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsEffectiveShowClearButton)) { Source = this }));
            bindings.Add(clearButton.Bind(AbstractIconButton.IsMotionEnabledProperty,
                new Binding(nameof(IsMotionEnabled)) { Source = this }));
        }

        if (e.NameScope.Find<RevealButton>("PART_RevealButton") is { } revealButton)
        {
            hasBindings = true;
            bindings.Add(revealButton.Bind(ToggleButton.IsCheckedProperty,
                new Binding(nameof(RevealPassword)) { Source = this, Mode = BindingMode.TwoWay }));
            bindings.Add(revealButton.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsEnableRevealButton)) { Source = this }));
        }

        if (e.NameScope.Find<ContentPresenter>("FormFeedBack") is { } formFeedback)
        {
            hasBindings = true;
            bindings.Add(formFeedback.Bind(ContentPresenter.ContentProperty,
                new Binding(nameof(FormFeedback)) { Source = this }));
            bindings.Add(formFeedback.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsFormFeedbackVisible)) { Source = this }));
        }

        if (e.NameScope.Find<ContentPresenter>("InnerRightContentPresenter") is { } innerRightContent)
        {
            hasBindings = true;
            bindings.Add(innerRightContent.Bind(ContentPresenter.ContentProperty,
                new Binding(nameof(InnerRightContent)) { Source = this }));
            bindings.Add(innerRightContent.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(InnerRightContent)) { Source = this, Converter = ObjectConverters.IsNotNull }));
        }

        if (e.NameScope.Find<TextBlock>("TextCountIndicator") is { } textCountIndicator)
        {
            hasBindings = true;
            bindings.Add(textCountIndicator.Bind(Avalonia.Controls.TextBlock.TextProperty,
                new Binding(nameof(CountText)) { Source = this }));
            bindings.Add(textCountIndicator.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsShowCount)) { Source = this }));
        }

        if (hasBindings)
        {
            _contentRightAddOnBindings = bindings;
        }
        else
        {
            bindings.Dispose();
        }

        return hasBindings;
    }

    internal void NotifyAccessoryClearButtonClicked()
    {
        NotifyClearButtonClicked();
    }

    private static bool IsAccessoryStateProperty(AvaloniaProperty property)
    {
        return property == IsEffectiveShowClearButtonProperty ||
               property == ClearIconProperty ||
               property == IsMotionEnabledProperty ||
               property == IsEnableRevealButtonProperty ||
               property == RevealPasswordProperty ||
               property == FormFeedbackProperty ||
               property == IsFormFeedbackVisibleProperty ||
               property == InnerRightContentProperty ||
               property == InnerRightContentTemplateProperty ||
               property == IsShowCountProperty ||
               property == CountTextProperty;
    }

    private void ConfigureOwnerDrivenAccessoryHost()
    {
        if (_addOnDecoratedBox == null ||
            _isTemplateProvidedAccessoryHost ||
            _isUsingLegacyAccessoryTemplate)
        {
            return;
        }

        if (!NeedsRightAccessoryHost())
        {
            ClearOwnerDrivenAccessoryHost();
            return;
        }

        if (_accessoryHost == null)
        {
            _accessoryHost = CreateAccessoryHost();
            _accessoryHostSpacingBinding = TokenResourceBinder.CreateTokenBinding(
                _accessoryHost,
                StackPanel.SpacingProperty,
                SharedTokenKind.UniformlyPaddingXXS);
            _accessoryHost.AttachOwner(this);
        }

        if (!ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _accessoryHost))
        {
            _addOnDecoratedBox.SetCurrentValue(AddOnDecoratedBox.ContentRightAddOnProperty, _accessoryHost);
        }
    }

    private void ClearOwnerDrivenAccessoryHost()
    {
        if (_addOnDecoratedBox != null &&
            _accessoryHost != null &&
            ReferenceEquals(_addOnDecoratedBox.ContentRightAddOn, _accessoryHost))
        {
            _addOnDecoratedBox.ClearValue(AddOnDecoratedBox.ContentRightAddOnProperty);
        }

        _accessoryHostSpacingBinding?.Dispose();
        _accessoryHostSpacingBinding = null;

        if (!_isTemplateProvidedAccessoryHost && _accessoryHost != null)
        {
            _accessoryHost.DetachOwner();
            _accessoryHost = null;
        }
    }

    private protected virtual LineEditAccessoryHost CreateAccessoryHost()
    {
        return new LineEditAccessoryHost();
    }

    private protected virtual bool IsFormFeedbackAccessoryEnabled => true;

    private protected virtual bool IsCountIndicatorAccessoryEnabled => true;

    private bool NeedsRightAccessoryHost()
    {
        return IsEffectiveShowClearButton ||
               IsEnableRevealButton ||
               (IsFormFeedbackAccessoryEnabled && IsFormFeedbackVisible && FormFeedback != null) ||
               InnerRightContent != null ||
               (IsCountIndicatorAccessoryEnabled && IsShowCount);
    }
    
    protected override double GetBorderThicknessForCompactSpace()
    {
        if (!IsUsedInCompactSpace)
        {
            return 0.0;
        }

        if (_addOnDecoratedBox == null || _addOnDecoratedBox.StyleVariant != InputControlStyleVariant.Outlined)
        {
            return 0.0;
        }

        // 都一样宽
        return _addOnDecoratedBox.InnerBoxBorderThickness.Left;
    }
    
    protected override void NotifyValidateStatus(FormValidateStatus status)
    {
        if (status == FormValidateStatus.Error)
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Error);
        }
        else if (status == FormValidateStatus.Warning)
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Warning);
        }
        else
        {
            SetCurrentValue(StatusProperty, InputControlStatus.Default);
        }
    }
}
