using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Desktop.Controls.Themes;
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

        if (e.NameScope.Find<InputClearIconButton>(TextBoxThemeConstants.ClearButtonPart) is { } clearButton)
        {
            _contentRightAddOnBindings.Add(clearButton.Bind(AbstractIconButton.IconProperty,
                new Binding(nameof(ClearIcon)) { Source = this }));
            _contentRightAddOnBindings.Add(clearButton.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsEffectiveShowClearButton)) { Source = this }));
            _contentRightAddOnBindings.Add(clearButton.Bind(AbstractIconButton.IsMotionEnabledProperty,
                new Binding(nameof(IsMotionEnabled)) { Source = this }));
        }

        if (e.NameScope.Find<RevealButton>(TextBoxThemeConstants.RevealButtonPart) is { } revealButton)
        {
            _contentRightAddOnBindings.Add(revealButton.Bind(ToggleButton.IsCheckedProperty,
                new Binding(nameof(RevealPassword)) { Source = this, Mode = BindingMode.TwoWay }));
            _contentRightAddOnBindings.Add(revealButton.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsEnableRevealButton)) { Source = this }));
        }

        if (e.NameScope.Find<ContentPresenter>("FormFeedBack") is { } formFeedback)
        {
            _contentRightAddOnBindings.Add(formFeedback.Bind(ContentPresenter.ContentProperty,
                new Binding(nameof(FormFeedback)) { Source = this }));
            _contentRightAddOnBindings.Add(formFeedback.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(FormFeedback)) { Source = this, Converter = ObjectConverters.IsNotNull }));
        }

        if (e.NameScope.Find<ContentPresenter>("InnerRightContentPresenter") is { } innerRightContent)
        {
            _contentRightAddOnBindings.Add(innerRightContent.Bind(ContentPresenter.ContentProperty,
                new Binding(nameof(InnerRightContent)) { Source = this }));
            _contentRightAddOnBindings.Add(innerRightContent.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(InnerRightContent)) { Source = this, Converter = ObjectConverters.IsNotNull }));
        }

        if (e.NameScope.Find<TextBlock>("TextCountIndicator") is { } textCountIndicator)
        {
            _contentRightAddOnBindings.Add(textCountIndicator.Bind(Avalonia.Controls.TextBlock.TextProperty,
                new Binding(nameof(CountText)) { Source = this }));
            _contentRightAddOnBindings.Add(textCountIndicator.Bind(Visual.IsVisibleProperty,
                new Binding(nameof(IsShowCount)) { Source = this }));
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