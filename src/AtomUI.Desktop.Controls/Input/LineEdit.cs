using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Theme;
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
    private CompositeDisposable? _contentRightAddOnBindings;
    
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
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
        _addOnDecoratedBox = e.NameScope.Find<AddOnDecoratedBox>(AddOnDecoratedBox.AddOnDecoratedBoxPart);
        SetupContentRightAddOnBindings(e);
    }

    private void SetupContentRightAddOnBindings(TemplateAppliedEventArgs e)
    {
        _contentRightAddOnBindings?.Dispose();
        _contentRightAddOnBindings = new CompositeDisposable();

        if (e.NameScope.Find<InputClearIconButton>("PART_ClearButton") is { } clearButton)
        {
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, ClearIconProperty, clearButton,
                AbstractIconButton.IconProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsEffectiveShowClearButtonProperty, clearButton,
                Visual.IsVisibleProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, clearButton,
                AbstractIconButton.IsMotionEnabledProperty));
        }

        if (e.NameScope.Find<RevealButton>("PART_RevealButton") is { } revealButton)
        {
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, RevealPasswordProperty, revealButton,
                ToggleButton.IsCheckedProperty, BindingMode.TwoWay));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsEnableRevealButtonProperty, revealButton,
                Visual.IsVisibleProperty));
        }

        if (e.NameScope.Find<ContentPresenter>("FormFeedBack") is { } formFeedback)
        {
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, FormFeedbackProperty, formFeedback,
                ContentPresenter.ContentProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsFormFeedbackVisibleProperty, formFeedback,
                Visual.IsVisibleProperty));
        }

        if (e.NameScope.Find<ContentPresenter>("InnerRightContentPresenter") is { } innerRightContent)
        {
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, InnerRightContentProperty, innerRightContent,
                ContentPresenter.ContentProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, InnerRightContentProperty, innerRightContent,
                Visual.IsVisibleProperty, value => value is not null));
        }

        if (e.NameScope.Find<TextBlock>("TextCountIndicator") is { } textCountIndicator)
        {
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, CountTextProperty, textCountIndicator,
                Avalonia.Controls.TextBlock.TextProperty));
            _contentRightAddOnBindings.Add(BindUtils.RelayBind(this, IsShowCountProperty, textCountIndicator,
                Visual.IsVisibleProperty));
        }
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
