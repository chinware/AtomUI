using AtomUI.Data;
using AtomUI.Desktop.Controls.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>
/// CalendarView 网格中的内部数据容器。应用一个不可变 Cell Model，
/// 管理伪类、Pointer 与激活报告。选中、禁用、命中测试语义由本容器保留，模板只替换内容。
/// </summary>
[PseudoClasses(
    CalendarRootPseudoClass.Fullscreen,
    CalendarRootPseudoClass.Mini,
    CalendarCellPseudoClass.Date,
    CalendarCellPseudoClass.Month,
    CalendarCellPseudoClass.Week,
    CalendarCellPseudoClass.Today,
    CalendarCellPseudoClass.Selected,
    CalendarCellPseudoClass.Outside,
    CalendarCellPseudoClass.Disabled,
    CalendarCellPseudoClass.Focused)]
internal sealed class CalendarViewCell : TemplatedControl
{
    public static readonly StyledProperty<string> DisplayTextProperty =
        AvaloniaProperty.Register<CalendarViewCell, string>(nameof(DisplayText), string.Empty);

    public static readonly StyledProperty<CalendarCellContext?> ContextProperty =
        AvaloniaProperty.Register<CalendarViewCell, CalendarCellContext?>(nameof(Context));

    public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.Register<CalendarViewCell, IDataTemplate?>(nameof(CellTemplate));

    public static readonly StyledProperty<IDataTemplate?> FullCellTemplateProperty =
        AvaloniaProperty.Register<CalendarViewCell, IDataTemplate?>(nameof(FullCellTemplate));

    /// <summary>Cell 主显示文本（日两位数 / 短月名 / 周序号）。</summary>
    public string DisplayText
    {
        get => GetValue(DisplayTextProperty);
        set => SetValue(DisplayTextProperty, value);
    }

    /// <summary>供 CellTemplate/FullCellTemplate 使用的强类型上下文（Week cell 为 null）。</summary>
    public CalendarCellContext? Context
    {
        get => GetValue(ContextProperty);
        set => SetValue(ContextProperty, value);
    }

    public IDataTemplate? CellTemplate
    {
        get => GetValue(CellTemplateProperty);
        set => SetValue(CellTemplateProperty, value);
    }

    public IDataTemplate? FullCellTemplate
    {
        get => GetValue(FullCellTemplateProperty);
        set => SetValue(FullCellTemplateProperty, value);
    }

    private CalendarViewCellModel? _model;
    private CalendarView? _owner;
    private ContentControl? _itemContent;
    private TextBlock? _valueText;
    private bool _fullscreen;

    public CalendarViewCell()
    {
        Focusable = true;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _itemContent = e.NameScope.Find<ContentControl>("PART_ItemContent");
        _valueText = e.NameScope.Find<TextBlock>("PART_Value");
        UpdateTemplatePresentation();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CellTemplateProperty ||
            change.Property == FullCellTemplateProperty ||
            change.Property == ContextProperty)
        {
            UpdateTemplatePresentation();
        }
    }

    /// <summary>绑定容器到面板 owner 与一个不可变 Cell Model，并刷新伪类与内容。</summary>
    public void Bind(CalendarView owner, CalendarViewCellModel model)
    {
        _owner = owner;
        _model = model;
        SetFullscreen(owner.Fullscreen);
        Focusable = model.IsFocusable;
        DisplayText = model.DisplayText;
        Context = model.Kind == CalendarViewCellKind.Week
            ? null
            : new CalendarCellContext(
                Value: model.Value,
                Today: owner.Today == default ? DateTime.Today : owner.Today.Date,
                CellType: model.Kind == CalendarViewCellKind.Month ? CalendarCellType.Month : CalendarCellType.Date,
                DisplayValue: model.DisplayText,
                IsToday: model.IsToday,
                IsInView: model.IsInView,
                IsSelected: model.IsSelected,
                IsDisabled: model.IsDisabled);
        UpdatePseudoClasses();
        UpdateTemplatePresentation();
    }

    public CalendarViewCellModel? Model => _model;

    internal CalendarView? OwnerView => _owner;

    internal string AutomationName
    {
        get
        {
            if (_model is not { } model)
            {
                return string.Empty;
            }

            var culture = _owner?.Culture ?? System.Globalization.CultureInfo.CurrentCulture;
            return model.Kind switch
            {
                CalendarViewCellKind.Date => model.Value.ToString("D", culture),
                CalendarViewCellKind.Month => model.Value.ToString("Y", culture),
                _ => $"{LanguageResourceBinder.GetLangResource(CalendarControlLangResourceKind.Week) ?? CalendarControlLangResourceKind.Week.ToString()} {model.DisplayText}"
            };
        }
    }

    /// <summary>释放池化容器持有的 owner、model、模板与上下文引用。</summary>
    internal void Unbind()
    {
        _owner = null;
        _model = null;
        Focusable = false;
        DisplayText = string.Empty;
        Context = null;
        CellTemplate = null;
        FullCellTemplate = null;
        SetFullscreen(false);
        UpdatePseudoClasses();
        UpdateTemplatePresentation();
    }

    private void UpdateTemplatePresentation()
    {
        var model = _model;
        var isWeek = model is { Kind: CalendarViewCellKind.Week };
        var useFullTemplate = !isWeek && FullCellTemplate is not null;
        var useCellTemplate = !isWeek && FullCellTemplate is null && CellTemplate is not null;

        if (_itemContent is not null)
        {
            _itemContent.ContentTemplate = useFullTemplate
                ? FullCellTemplate
                : useCellTemplate
                    ? CellTemplate
                    : null;
            _itemContent.IsVisible = useFullTemplate || useCellTemplate;
            Grid.SetRow(_itemContent, useFullTemplate ? 0 : 1);
            Grid.SetRowSpan(_itemContent, useFullTemplate ? 2 : 1);
        }

        if (_valueText is not null)
        {
            _valueText.IsVisible = isWeek || !useFullTemplate;
            Grid.SetRow(_valueText, 0);
            Grid.SetRowSpan(_valueText, !_fullscreen && !useCellTemplate && !useFullTemplate ? 2 : 1);
        }
    }

    internal void SetFullscreen(bool fullscreen)
    {
        _fullscreen = fullscreen;
        PseudoClasses.Set(CalendarRootPseudoClass.Fullscreen, fullscreen);
        PseudoClasses.Set(CalendarRootPseudoClass.Mini, !fullscreen);
        UpdateTemplatePresentation();
    }

    private void UpdatePseudoClasses()
    {
        var m = _model;
        var isDate = m is { Kind: CalendarViewCellKind.Date };
        var isMonth = m is { Kind: CalendarViewCellKind.Month };
        var isWeek = m is { Kind: CalendarViewCellKind.Week };

        PseudoClasses.Set(CalendarCellPseudoClass.Date, isDate);
        PseudoClasses.Set(CalendarCellPseudoClass.Month, isMonth);
        PseudoClasses.Set(CalendarCellPseudoClass.Week, isWeek);
        PseudoClasses.Set(CalendarCellPseudoClass.Today, m?.IsToday ?? false);
        PseudoClasses.Set(CalendarCellPseudoClass.Selected, m?.IsSelected ?? false);
        PseudoClasses.Set(CalendarCellPseudoClass.Outside, m is { IsInView: false, Kind: CalendarViewCellKind.Date });
        PseudoClasses.Set(CalendarCellPseudoClass.Disabled, m?.IsDisabled ?? false);
        PseudoClasses.Set(CalendarCellPseudoClass.Focused, false);
    }

    public void SetFocused(bool focused)
    {
        PseudoClasses.Set(CalendarCellPseudoClass.Focused, focused);
    }

    protected override Avalonia.Automation.Peers.AutomationPeer OnCreateAutomationPeer() =>
        new CalendarViewCellAutomationPeer(this);

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (e.InitialPressMouseButton != MouseButton.Left)
        {
            return;
        }

        Activate();
    }

    /// <summary>激活本 Cell：把用户意图报告给 owner；禁用 Cell 由 owner 忽略。</summary>
    public void Activate()
    {
        if (_model is { } model && _owner is { } owner)
        {
            owner.ReportCellActivated(model);
        }
    }
}
