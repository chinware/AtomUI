using AtomUI.Generated.AtomUIDesktopControls;
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
internal class CalendarViewCell : TemplatedControl
{
    public static readonly StyledProperty<string> DisplayTextProperty =
        AvaloniaProperty.Register<CalendarViewCell, string>(nameof(DisplayText), string.Empty);

    public static readonly StyledProperty<CalendarCellContext?> ContextProperty =
        AvaloniaProperty.Register<CalendarViewCell, CalendarCellContext?>(nameof(Context));

    public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.Register<CalendarViewCell, IDataTemplate?>(nameof(CellTemplate));

    public static readonly StyledProperty<IDataTemplate?> FullCellTemplateProperty =
        AvaloniaProperty.Register<CalendarViewCell, IDataTemplate?>(nameof(FullCellTemplate));

    public static readonly StyledProperty<double> FullCellMinHeightProperty =
        AvaloniaProperty.Register<CalendarViewCell, double>(nameof(FullCellMinHeight), double.NaN);

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

    public double FullCellMinHeight
    {
        get => GetValue(FullCellMinHeightProperty);
        set => SetValue(FullCellMinHeightProperty, value);
    }

    private CalendarViewCellModel? _model;
    private CalendarView? _owner;
    private ContentControl? _itemContent;
    private TextBlock? _valueText;
    private bool _fullscreen;
    private bool _fullscreenStateApplied;

    public CalendarViewCell()
    {
        Focusable = true;
        Classes.Add(CalendarSemanticParts.ItemClass);
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
            change.Property == FullCellTemplateProperty)
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
        if (IsEnabled == model.IsDisabled)
        {
            SetCurrentValue(IsEnabledProperty, !model.IsDisabled);
        }

        if (Focusable != model.IsFocusable)
        {
            Focusable = model.IsFocusable;
        }

        if (DisplayText != model.DisplayText)
        {
            DisplayText = model.DisplayText;
        }

        if (model.Kind == CalendarViewCellKind.Week || !HasCustomTemplate)
        {
            if (Context is not null)
            {
                Context = null;
            }
        }
        else
        {
            Context = owner.PresentationAdapter.CreateCellContext(owner, model);
        }
        owner.PresentationAdapter.ApplyCellPresentation(this, owner, model);
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

            return _owner?.PresentationAdapter.GetAutomationName(_owner, model) ?? string.Empty;
        }
    }

    /// <summary>释放池化容器持有的 owner、model、模板与上下文引用。</summary>
    internal void Unbind()
    {
        _owner?.PresentationAdapter.ClearCellPresentation(this);
        _owner = null;
        _model = null;
        SetCurrentValue(IsEnabledProperty, true);
        Focusable = false;
        DisplayText = string.Empty;
        Context = null;
        CellTemplate = null;
        FullCellTemplate = null;
        FullCellMinHeight = double.NaN;
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
            var contentTemplate = useFullTemplate
                ? FullCellTemplate
                : useCellTemplate
                    ? CellTemplate
                    : null;
            var hasTemplate = contentTemplate is not null;
            var content = hasTemplate ? Context : null;

            if (!ReferenceEquals(_itemContent.Content, content))
            {
                _itemContent.Content = content;
            }

            if (!ReferenceEquals(_itemContent.ContentTemplate, contentTemplate))
            {
                _itemContent.ContentTemplate = contentTemplate;
            }

            // Fullscreen 单元格即使没有自定义模板也保留可见的内容区域（与 antd full 单元格
            // 始终渲染 date-content 的契约一致），itemContent 语义 Part 才能有可标注的几何区域。
            var itemContentVisible = hasTemplate || (_fullscreen && !isWeek);
            if (_itemContent.IsVisible != itemContentVisible)
            {
                _itemContent.IsVisible = itemContentVisible;
            }

            var itemContentRow = useFullTemplate ? 0 : 1;
            if (Grid.GetRow(_itemContent) != itemContentRow)
            {
                Grid.SetRow(_itemContent, itemContentRow);
            }

            var itemContentRowSpan = useFullTemplate ? 2 : 1;
            if (Grid.GetRowSpan(_itemContent) != itemContentRowSpan)
            {
                Grid.SetRowSpan(_itemContent, itemContentRowSpan);
            }
        }

        if (_valueText is not null)
        {
            var valueVisible = isWeek || !useFullTemplate;
            if (_valueText.IsVisible != valueVisible)
            {
                _valueText.IsVisible = valueVisible;
            }

            if (Grid.GetRow(_valueText) != 0)
            {
                Grid.SetRow(_valueText, 0);
            }

            var valueRowSpan = !_fullscreen &&
                               !HasDefaultSecondaryContent &&
                               !useCellTemplate &&
                               !useFullTemplate
                ? 2
                : 1;
            if (Grid.GetRowSpan(_valueText) != valueRowSpan)
            {
                Grid.SetRowSpan(_valueText, valueRowSpan);
            }
        }
    }

    internal void SetFullscreen(bool fullscreen)
    {
        if (_fullscreenStateApplied && _fullscreen == fullscreen)
        {
            return;
        }

        _fullscreen = fullscreen;
        _fullscreenStateApplied = true;
        PseudoClasses.Set(CalendarRootPseudoClass.Fullscreen, fullscreen);
        PseudoClasses.Set(CalendarRootPseudoClass.Mini, !fullscreen);
        UpdateTemplatePresentation();
    }

    private bool HasCustomTemplate => CellTemplate is not null || FullCellTemplate is not null;

    protected virtual bool HasDefaultSecondaryContent => false;

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
