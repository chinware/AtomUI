using System;
using System.Globalization;
using System.Windows.Input;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Internal.Calendar;
using AtomUI.Theme;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;

namespace AtomUI.Desktop.Controls;

[PseudoClasses(
    CalendarRootPseudoClass.Fullscreen,
    CalendarRootPseudoClass.Mini,
    CalendarRootPseudoClass.Month,
    CalendarRootPseudoClass.Year,
    CalendarRootPseudoClass.ShowWeek)]
[TemplatePart(CalendarViewPart, typeof(CalendarViewControl))]

/// <summary>
/// 按 Ant Design 6 语义组织的桌面日历控件。Calendar 是唯一业务状态 owner，
/// 管理 <see cref="Value"/>、<see cref="Mode"/> 与三个公开事件。
/// </summary>
public class Calendar : TemplatedControl
{
    #region Avalonia Properties

    public static readonly StyledProperty<DateTime> ValueProperty =
        AvaloniaProperty.Register<Calendar, DateTime>(nameof(Value));

    public static readonly StyledProperty<CalendarMode> ModeProperty =
        AvaloniaProperty.Register<Calendar, CalendarMode>(nameof(Mode), CalendarMode.Month);

    public static readonly StyledProperty<bool> FullscreenProperty =
        AvaloniaProperty.Register<Calendar, bool>(nameof(Fullscreen), true);

    public static readonly StyledProperty<bool> ShowWeekProperty =
        AvaloniaProperty.Register<Calendar, bool>(nameof(ShowWeek));

    public static readonly StyledProperty<CalendarDateRange?> ValidRangeProperty =
        AvaloniaProperty.Register<Calendar, CalendarDateRange?>(nameof(ValidRange));

    public static readonly StyledProperty<Func<DateTime, bool>?> DisabledDateProperty =
        AvaloniaProperty.Register<Calendar, Func<DateTime, bool>?>(nameof(DisabledDate));

    public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.Register<Calendar, IDataTemplate?>(nameof(CellTemplate));

    public static readonly StyledProperty<IDataTemplate?> FullCellTemplateProperty =
        AvaloniaProperty.Register<Calendar, IDataTemplate?>(nameof(FullCellTemplate));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<Calendar, IDataTemplate?>(nameof(HeaderTemplate));

    /// <summary>当前选中日期，同时作为默认面板锚点。默认为实例创建时的 <see cref="DateTime.Today"/>。</summary>
    public DateTime Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>公开显示模式。默认 <see cref="CalendarMode.Month"/>。</summary>
    public CalendarMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    /// <summary>是否使用完整数据展示布局。默认 <c>true</c>。</summary>
    public bool Fullscreen
    {
        get => GetValue(FullscreenProperty);
        set => SetValue(FullscreenProperty, value);
    }

    /// <summary>是否显示周序号列。默认 <c>false</c>。</summary>
    public bool ShowWeek
    {
        get => GetValue(ShowWeekProperty);
        set => SetValue(ShowWeekProperty, value);
    }

    /// <summary>有效日期范围（首尾包含）。默认 <c>null</c>。</summary>
    public CalendarDateRange? ValidRange
    {
        get => GetValue(ValidRangeProperty);
        set => SetValue(ValidRangeProperty, value);
    }

    /// <summary>业务禁用规则。默认 <c>null</c>。</summary>
    public Func<DateTime, bool>? DisabledDate
    {
        get => GetValue(DisabledDateProperty);
        set => SetValue(DisabledDateProperty, value);
    }

    /// <summary>对应 Ant Design <c>cellRender</c>，替换默认 Cell 的业务内容区域。</summary>
    public IDataTemplate? CellTemplate
    {
        get => GetValue(CellTemplateProperty);
        set => SetValue(CellTemplateProperty, value);
    }

    /// <summary>对应 Ant Design <c>fullCellRender</c>，替换 Cell 的完整 inner 内容。优先于 <see cref="CellTemplate"/>。</summary>
    public IDataTemplate? FullCellTemplate
    {
        get => GetValue(FullCellTemplateProperty);
        set => SetValue(FullCellTemplateProperty, value);
    }

    /// <summary>自定义 Header 模板。为 <c>null</c> 时使用默认 Header。</summary>
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    #endregion

    #region Public Events

    /// <summary>选中日期实际变化时触发。程序直接设置 <see cref="Value"/> 不触发。</summary>
    public event EventHandler<CalendarValueChangedEventArgs>? ValueChanged;

    /// <summary>每次有效用户选择触发。</summary>
    public event EventHandler<CalendarSelectedEventArgs>? Selected;

    /// <summary>面板（自然月/自然年）变化或用户切换 Mode 时触发。</summary>
    public event EventHandler<CalendarPanelChangedEventArgs>? PanelChanged;

    #endregion

    /// <summary>true 期间通过属性系统写入的 Value/Mode 来自内部提交，不重复触发用户事件。</summary>
    private bool _isCommitting;

    public Calendar()
    {
        SetCurrentValue(ValueProperty, DateTime.Today);
    }

    /// <summary>公开 <see cref="Mode"/> 到内部面板模式的映射（spec §4.2）。</summary>
    internal CalendarViewMode ViewMode =>
        Mode == CalendarMode.Year ? CalendarViewMode.Month : CalendarViewMode.Date;

    internal const string CalendarViewPart = "PART_CalendarView";
    internal const string DefaultHeaderPart = "PART_DefaultHeader";
    internal const string CustomHeaderPart = "PART_CustomHeader";

    private CalendarViewControl? _calendarView;
    private CalendarHeader? _defaultHeader;
    private ContentControl? _customHeader;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_calendarView is not null)
        {
            _calendarView.CellSelected -= OnCellSelected;
        }

        _calendarView = e.NameScope.Find<CalendarViewControl>(CalendarViewPart);
        if (_calendarView is not null)
        {
            _calendarView.Today = DateTime.Today;
            _calendarView.CellSelected += OnCellSelected;
        }

        if (_defaultHeader is not null)
        {
            _defaultHeader.YearSelected  -= OnHeaderYearSelected;
            _defaultHeader.MonthSelected -= OnHeaderMonthSelected;
            _defaultHeader.ModeSwitched  -= OnHeaderModeSwitched;
        }

        _defaultHeader = e.NameScope.Find<CalendarHeader>(DefaultHeaderPart);
        if (_defaultHeader is not null)
        {
            _defaultHeader.IsVisible      = HeaderTemplate is null;
            _defaultHeader.YearSelected  += OnHeaderYearSelected;
            _defaultHeader.MonthSelected += OnHeaderMonthSelected;
            _defaultHeader.ModeSwitched  += OnHeaderModeSwitched;
        }

        _customHeader = e.NameScope.Find<ContentControl>(CustomHeaderPart);

        SyncViewMode();
        RefreshCustomHeaderContent();
        ApplyCulture();
        UpdateRootPseudoClasses();
    }

    /// <summary>
    /// 为自定义 HeaderTemplate 提供强类型 <see cref="CalendarHeaderContext"/> 作为 DataContext，
    /// 使模板能通过命令提交 Value/Mode（spec §5.4）。Value/Mode 变化时重建 context。
    /// </summary>
    private void RefreshCustomHeaderContent()
    {
        if (_customHeader is not null && HeaderTemplate is not null)
        {
            _customHeader.Content = BuildHeaderContext();
        }
    }

    /// <summary>
    /// 命令式把公开 Mode 投影到内部面板的 ViewMode。
    /// 不用 XAML binding，因为 <see cref="ViewMode"/> 是无变更通知的计算属性，绑定不会随 Mode 变化重新求值。
    /// </summary>
    private void SyncViewMode()
    {
        if (_calendarView is not null)
        {
            _calendarView.ViewMode = ViewMode;
        }
    }

    private ILanguageManager? _subscribedLanguageManager;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AttachLanguageListener();
        ApplyCulture();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        DetachLanguageListener();
    }

    private void AttachLanguageListener()
    {
        if (_subscribedLanguageManager is not null)
        {
            return;
        }

        var languageManager = Application.Current?.GetLanguageManager();
        if (languageManager is null)
        {
            return;
        }

        languageManager.LanguageVariantChanged += OnLanguageVariantChanged;
        _subscribedLanguageManager = languageManager;
    }

    private void DetachLanguageListener()
    {
        if (_subscribedLanguageManager is null)
        {
            return;
        }

        _subscribedLanguageManager.LanguageVariantChanged -= OnLanguageVariantChanged;
        _subscribedLanguageManager = null;
    }

    private void OnLanguageVariantChanged(object? sender, LanguageVariantChangedEventArgs e) => ApplyCulture();

    /// <summary>解析当前语言的 Culture 并推给 CalendarView / 默认 Header，触发它们重建（spec §12）。</summary>
    private void ApplyCulture()
    {
        var culture = Application.Current?.GetLanguageVariant()?.ToCultureInfo() ?? CultureInfo.CurrentCulture;
        if (_calendarView is not null)
        {
            _calendarView.Culture = culture;
        }

        if (_defaultHeader is not null)
        {
            _defaultHeader.Culture = culture;
        }
    }

    private void OnHeaderYearSelected(object? sender, DateTime target) =>
        CommitUserSelection(target, CalendarSelectSource.Year);

    private void OnHeaderMonthSelected(object? sender, DateTime target) =>
        CommitUserSelection(target, CalendarSelectSource.Month);

    private void OnHeaderModeSwitched(object? sender, CalendarMode mode) =>
        CommitModeChange(mode);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ModeProperty)
        {
            SyncViewMode();
            RefreshCustomHeaderContent();
            UpdateRootPseudoClasses();
        }
        else if (change.Property == ValueProperty)
        {
            RefreshCustomHeaderContent();
        }
        else if (change.Property == FullscreenProperty ||
                 change.Property == ShowWeekProperty)
        {
            UpdateRootPseudoClasses();
        }
        else if (change.Property == HeaderTemplateProperty)
        {
            if (_defaultHeader is not null)
            {
                _defaultHeader.IsVisible = HeaderTemplate is null;
            }

            RefreshCustomHeaderContent();
        }
    }

    private void UpdateRootPseudoClasses()
    {
        PseudoClasses.Set(CalendarRootPseudoClass.Fullscreen, Fullscreen);
        PseudoClasses.Set(CalendarRootPseudoClass.Mini, !Fullscreen);
        PseudoClasses.Set(CalendarRootPseudoClass.Month, Mode == CalendarMode.Month);
        PseudoClasses.Set(CalendarRootPseudoClass.Year, Mode == CalendarMode.Year);
        PseudoClasses.Set(CalendarRootPseudoClass.ShowWeek, ShowWeek);
    }

    private void OnCellSelected(object? sender, CalendarCellSelectedEventArgs e)
    {
        var source = e.Kind == CalendarViewCellKind.Month
            ? CalendarSelectSource.Month
            : CalendarSelectSource.Date;
        CommitUserSelection(e.Value, source);
    }

    /// <summary>
    /// 处理一次有效用户选择：写入 Value 并按固定顺序 PanelChanged -> ValueChanged -> Selected 触发事件（spec §6.1）。
    /// </summary>
    internal void CommitUserSelection(DateTime target, CalendarSelectSource source)
    {
        target = target.Date;
        var oldValue = Value.Date;
        var panelChanged = Mode == CalendarMode.Year
            ? oldValue.Year != target.Year
            : oldValue.Year != target.Year || oldValue.Month != target.Month;

        _isCommitting = true;
        SetCurrentValue(ValueProperty, target);
        _isCommitting = false;

        if (panelChanged)
        {
            PanelChanged?.Invoke(this, new CalendarPanelChangedEventArgs(target, Mode));
        }

        if (target != oldValue)
        {
            ValueChanged?.Invoke(this, new CalendarValueChangedEventArgs(oldValue, target));
        }

        Selected?.Invoke(this, new CalendarSelectedEventArgs(target, source));
    }

    /// <summary>
    /// 处理一次用户 Mode 切换：写入 Mode 并触发一次 <see cref="PanelChanged"/>（spec §6.2）。
    /// 不触发 ValueChanged / Selected。
    /// </summary>
    internal void CommitModeChange(CalendarMode mode)
    {
        if (mode == Mode)
        {
            return;
        }

        _isCommitting = true;
        SetCurrentValue(ModeProperty, mode);
        _isCommitting = false;

        PanelChanged?.Invoke(this, new CalendarPanelChangedEventArgs(Value.Date, mode));
    }

    /// <summary>
    /// 为自定义 Header 模板构建强类型上下文（spec §5.4）。命令的 CanExecute 校验参数类型；
    /// ChangeValueCommand 以 <see cref="CalendarSelectSource.Customize"/> 提交，且不自动应用
    /// ValidRange 或 DisabledDate（约束由自定义 Header 负责）。
    /// </summary>
    internal CalendarHeaderContext BuildHeaderContext()
    {
        var changeValue = new CalendarRelayCommand(
            execute: p => CommitUserSelection((DateTime)p!, CalendarSelectSource.Customize),
            canExecute: p => p is DateTime);

        var changeMode = new CalendarRelayCommand(
            execute: p => CommitModeChange((CalendarMode)p!),
            canExecute: p => p is CalendarMode mode && (mode == CalendarMode.Month || mode == CalendarMode.Year));

        return new CalendarHeaderContext(Value.Date, Mode, changeValue, changeMode);
    }
}
