using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Internal.Calendar;
using AtomUI.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.VisualTree;
using CalendarRangeBarPanelControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarRangeBarPanel;
using CalendarViewControl = AtomUI.Desktop.Controls.Internal.Calendar.CalendarView;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// 桌面日历控件。Calendar 是唯一业务状态 owner，
/// 管理 <see cref="Value"/>、<see cref="Mode"/> 与三个公开事件。
/// </summary>
[PseudoClasses(
    CalendarRootPseudoClass.Fullscreen,
    CalendarRootPseudoClass.Mini,
    CalendarRootPseudoClass.Month,
    CalendarRootPseudoClass.Year,
    CalendarRootPseudoClass.ShowWeek)]
[TemplatePart(CalendarViewPart, typeof(CalendarViewControl))]
[TemplatePart(RangeBarPanelPart, typeof(CalendarRangeBarPanelControl))]
public partial class Calendar : TemplatedControl
{
    #region Avalonia Properties

    public static readonly StyledProperty<DateTime> ValueProperty =
        AvaloniaProperty.Register<Calendar, DateTime>(nameof(Value), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<CalendarMode> ModeProperty =
        AvaloniaProperty.Register<Calendar, CalendarMode>(
            nameof(Mode), CalendarMode.Month, defaultBindingMode: BindingMode.TwoWay);

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

    public static readonly DirectProperty<Calendar, CalendarRangeBarCollection> RangeBarsProperty =
        AvaloniaProperty.RegisterDirect<Calendar, CalendarRangeBarCollection>(
            nameof(RangeBars),
            o => o.RangeBars,
            (o, v) => o.RangeBars = v);

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

    /// <summary>替换默认 Cell 的业务内容区域，但保留默认日期/月值和 Cell 状态。</summary>
    public IDataTemplate? CellTemplate
    {
        get => GetValue(CellTemplateProperty);
        set => SetValue(CellTemplateProperty, value);
    }

    /// <summary>替换 Cell 的完整内部内容。优先于 <see cref="CellTemplate"/>。</summary>
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

    private CalendarRangeBarCollection _rangeBars = new();

    /// <summary>连续日期范围条集合。仅 Fullscreen Month 日期网格 overlay 使用。</summary>
    public CalendarRangeBarCollection RangeBars
    {
        get => _rangeBars;
        set
        {
            value ??= new CalendarRangeBarCollection();
            if (ReferenceEquals(_rangeBars, value))
            {
                return;
            }

            _rangeBars.CollectionChanged -= OnRangeBarsCollectionChanged;
            DetachAllRangeBars();
            var oldValue = _rangeBars;
            _rangeBars = value;
            _rangeBars.CollectionChanged += OnRangeBarsCollectionChanged;
            if (this.IsAttachedToVisualTree())
            {
                AttachRangeBars(_rangeBars);
            }

            RaisePropertyChanged(RangeBarsProperty, oldValue, value);
            InvalidateRangeBars();
        }
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

    #region Internal Presentation Properties

    internal static readonly StyledProperty<double> EffectiveMiniContentHeightProperty =
        AvaloniaProperty.Register<Calendar, double>(nameof(EffectiveMiniContentHeight), double.NaN);

    internal static readonly StyledProperty<double> EffectiveFullCellMinHeightProperty =
        AvaloniaProperty.Register<Calendar, double>(nameof(EffectiveFullCellMinHeight), double.NaN);

    internal static readonly StyledProperty<double> EffectiveRangeBarTopOffsetProperty =
        AvaloniaProperty.Register<Calendar, double>(nameof(EffectiveRangeBarTopOffset), double.NaN);

    internal double EffectiveMiniContentHeight
    {
        get => GetValue(EffectiveMiniContentHeightProperty);
        set => SetValue(EffectiveMiniContentHeightProperty, value);
    }

    internal double EffectiveFullCellMinHeight
    {
        get => GetValue(EffectiveFullCellMinHeightProperty);
        set => SetValue(EffectiveFullCellMinHeightProperty, value);
    }

    internal double EffectiveRangeBarTopOffset
    {
        get => GetValue(EffectiveRangeBarTopOffsetProperty);
        set => SetValue(EffectiveRangeBarTopOffsetProperty, value);
    }

    #endregion

    public Calendar()
    {
        SetCurrentValue(ValueProperty, DateTime.Today);
        _rangeBars.CollectionChanged += OnRangeBarsCollectionChanged;
        BindPresentationMetrics();
    }

    /// <summary>公开 <see cref="Mode"/> 到内部面板模式的映射：Month 显示日期，Year 显示月份。</summary>
    internal CalendarViewMode ViewMode =>
        Mode == CalendarMode.Year ? CalendarViewMode.Month : CalendarViewMode.Date;

    internal const string CalendarViewPart = "PART_CalendarView";
    internal const string RangeBarPanelPart = "PART_RangeBarPanel";
    internal const string DefaultHeaderPart = "PART_DefaultHeader";
    internal const string CustomHeaderPart = "PART_CustomHeader";

    private CalendarViewControl? _calendarView;
    private CalendarRangeBarPanelControl? _rangeBarPanel;
    private CalendarHeader? _defaultHeader;
    private ContentControl? _customHeader;
    private ICalendarPresentationAdapter _presentationAdapter = DefaultCalendarPresentationAdapter.Instance;
    private IDisposable? _miniContentHeightBinding;
    private IDisposable? _fullCellMinHeightBinding;
    private IDisposable? _rangeBarTopOffsetBinding;
    private readonly Dictionary<CalendarRangeBar, CalendarRangeBarAttachment> _rangeBarAttachments =
        new(ReferenceEqualityComparer.Instance);

    internal CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentCulture;

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
            _calendarView.PresentationAdapter = _presentationAdapter;
            _calendarView.Today = DateTime.Today;
            _calendarView.CellSelected += OnCellSelected;
        }

        _rangeBarPanel = e.NameScope.Find<CalendarRangeBarPanelControl>(RangeBarPanelPart);
        UpdateRangeBarPanelVisibility();

        if (_defaultHeader is not null)
        {
            _defaultHeader.YearSelected -= OnHeaderYearSelected;
            _defaultHeader.MonthSelected -= OnHeaderMonthSelected;
            _defaultHeader.ModeSwitched -= OnHeaderModeSwitched;
        }

        _defaultHeader = e.NameScope.Find<CalendarHeader>(DefaultHeaderPart);
        if (_defaultHeader is not null)
        {
            _defaultHeader.PresentationAdapter = _presentationAdapter;
            _defaultHeader.IsVisible = HeaderTemplate is null;
            _defaultHeader.YearSelected += OnHeaderYearSelected;
            _defaultHeader.MonthSelected += OnHeaderMonthSelected;
            _defaultHeader.ModeSwitched += OnHeaderModeSwitched;
        }

        _customHeader = e.NameScope.Find<ContentControl>(CustomHeaderPart);

        SyncViewMode();
        RefreshCustomHeaderContent();
        ApplyCulture();
        InvalidateRangeBars();
        UpdateRootPseudoClasses();
    }

    /// <summary>
    /// 为自定义 HeaderTemplate 提供强类型 <see cref="CalendarHeaderContext"/> 作为 DataContext，
    /// 使模板能通过命令提交 Value/Mode。Value/Mode 变化时重建 context。
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
        AttachRangeBars(RangeBars);
        ApplyCulture();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        DetachAllRangeBars();
        DetachLanguageListener();
    }

    private void AttachRangeBars(IEnumerable<CalendarRangeBar> rangeBars)
    {
        foreach (var rangeBar in rangeBars)
        {
            AttachRangeBar(rangeBar);
        }
    }

    private void AttachRangeBarsFromList(IList rangeBars)
    {
        for (var i = 0; i < rangeBars.Count; i++)
        {
            if (rangeBars[i] is CalendarRangeBar rangeBar)
            {
                AttachRangeBar(rangeBar);
            }
        }
    }

    private void DetachRangeBarsFromList(IList rangeBars)
    {
        for (var i = 0; i < rangeBars.Count; i++)
        {
            if (rangeBars[i] is CalendarRangeBar rangeBar)
            {
                DetachRangeBar(rangeBar);
            }
        }
    }

    private void AttachRangeBar(CalendarRangeBar rangeBar)
    {
        if (_rangeBarAttachments.TryGetValue(rangeBar, out var attachment))
        {
            attachment.ReferenceCount++;
            return;
        }

        rangeBar.PropertyChanged += OnRangeBarPropertyChanged;
        _rangeBarAttachments.Add(rangeBar, new CalendarRangeBarAttachment(rangeBar.AttachResourceHost(this)));
    }

    private void DetachRangeBar(CalendarRangeBar rangeBar)
    {
        if (!_rangeBarAttachments.TryGetValue(rangeBar, out var attachment))
        {
            return;
        }

        attachment.ReferenceCount--;
        if (attachment.ReferenceCount > 0)
        {
            return;
        }

        rangeBar.PropertyChanged -= OnRangeBarPropertyChanged;
        attachment.Dispose();
        _rangeBarAttachments.Remove(rangeBar);
    }

    private void DetachAllRangeBars()
    {
        foreach (var (rangeBar, attachment) in _rangeBarAttachments)
        {
            rangeBar.PropertyChanged -= OnRangeBarPropertyChanged;
            attachment.Dispose();
        }

        _rangeBarAttachments.Clear();
    }

    private void ResetRangeBarAttachments()
    {
        DetachAllRangeBars();
        if (this.IsAttachedToVisualTree())
        {
            AttachRangeBars(RangeBars);
        }
    }

    private void OnRangeBarsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (this.IsAttachedToVisualTree())
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems is not null)
                    {
                        AttachRangeBarsFromList(e.NewItems);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems is not null)
                    {
                        DetachRangeBarsFromList(e.OldItems);
                    }

                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems is not null)
                    {
                        DetachRangeBarsFromList(e.OldItems);
                    }

                    if (e.NewItems is not null)
                    {
                        AttachRangeBarsFromList(e.NewItems);
                    }

                    break;
                case NotifyCollectionChangedAction.Reset:
                    ResetRangeBarAttachments();
                    break;
            }
        }

        InvalidateRangeBars();
    }

    private void OnRangeBarPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        InvalidateRangeBars();
    }

    private void InvalidateRangeBars()
    {
        UpdateRangeBarPanelVisibility();
        _rangeBarPanel?.InvalidateRangeBars();
    }

    private void UpdateRangeBarPanelVisibility()
    {
        if (_rangeBarPanel is not null)
        {
            _rangeBarPanel.IsVisible = Mode == CalendarMode.Month && Fullscreen && RangeBars.Count > 0;
        }
    }

    private void AttachLanguageListener()
    {
        if (_subscribedLanguageManager is not null)
        {
            return;
        }

        var languageManager = Application.Current is { } application
            ? global::AtomUI.ApplicationExtensions.GetLanguageManager(application)
            : null;
        if (languageManager is null)
        {
            return;
        }

        languageManager.LanguageChanged += OnLanguageChanged;
        _subscribedLanguageManager = languageManager;
    }

    private void DetachLanguageListener()
    {
        if (_subscribedLanguageManager is null)
        {
            return;
        }

        _subscribedLanguageManager.LanguageChanged -= OnLanguageChanged;
        _subscribedLanguageManager = null;
    }

    private void OnLanguageChanged(object? sender, LanguageChangedEventArgs e) => ApplyCulture();

    /// <summary>解析当前语言的 Culture 并推给 CalendarView / 默认 Header，触发它们按需同步。</summary>
    private void ApplyCulture()
    {
        var culture = Application.Current is { } application
            ? global::AtomUI.ApplicationExtensions.GetLanguageManager(application)?.Current.FormattingCulture
            : null;
        culture ??= CultureInfo.CurrentCulture;
        CurrentCulture = culture;
        if (_calendarView is not null)
        {
            _calendarView.Culture = culture;
        }

        if (_rangeBarPanel is not null)
        {
            _rangeBarPanel.Culture = culture;
        }

        if (_defaultHeader is not null)
        {
            _defaultHeader.Culture = culture;
        }

        RefreshCustomHeaderContent();
    }

    internal void SetPresentationAdapter(ICalendarPresentationAdapter presentationAdapter)
    {
        ArgumentNullException.ThrowIfNull(presentationAdapter);
        if (ReferenceEquals(_presentationAdapter, presentationAdapter))
        {
            return;
        }

        _presentationAdapter = presentationAdapter;
        BindPresentationMetrics();
        if (_calendarView is not null)
        {
            _calendarView.PresentationAdapter = presentationAdapter;
        }

        if (_defaultHeader is not null)
        {
            _defaultHeader.PresentationAdapter = presentationAdapter;
        }

        RefreshCustomHeaderContent();
    }

    private void BindPresentationMetrics()
    {
        _miniContentHeightBinding?.Dispose();
        _fullCellMinHeightBinding?.Dispose();
        _rangeBarTopOffsetBinding?.Dispose();

        var metrics = _presentationAdapter.Metrics;
        _miniContentHeightBinding = Bind(
            EffectiveMiniContentHeightProperty,
            new DynamicResourceExtension(metrics.MiniContentHeightResourceKey));
        _fullCellMinHeightBinding = Bind(
            EffectiveFullCellMinHeightProperty,
            new DynamicResourceExtension(metrics.FullCellMinHeightResourceKey));
        _rangeBarTopOffsetBinding = Bind(
            EffectiveRangeBarTopOffsetProperty,
            new DynamicResourceExtension(metrics.RangeBarTopOffsetResourceKey));
    }

    internal void RefreshPresentation()
    {
        _calendarView?.RefreshPresentation();
        _defaultHeader?.RefreshPresentation();
        RefreshCustomHeaderContent();
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
            UpdateRangeBarPanelVisibility();
            UpdateRootPseudoClasses();
        }
        else if (change.Property == ValueProperty)
        {
            var value = (DateTime)change.NewValue!;
            if (value != value.Date)
            {
                SetCurrentValue(ValueProperty, value.Date);
                return;
            }

            RefreshCustomHeaderContent();
        }
        else if (change.Property == FullscreenProperty ||
                 change.Property == ShowWeekProperty)
        {
            UpdateRangeBarPanelVisibility();
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
    /// 处理一次有效用户选择：写入 Value，并按 PanelChanged -> ValueChanged -> Selected 的顺序触发事件。
    /// </summary>
    internal void CommitUserSelection(DateTime target, CalendarSelectSource source)
    {
        target = target.Date;
        var oldValue = Value.Date;
        var panelChanged = Mode == CalendarMode.Year
            ? oldValue.Year != target.Year
            : oldValue.Year != target.Year || oldValue.Month != target.Month;

        SetCurrentValue(ValueProperty, target);

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
    /// 处理一次用户 Mode 切换：写入 Mode 并触发一次 <see cref="PanelChanged"/>。
    /// 不触发 ValueChanged / Selected。
    /// </summary>
    internal void CommitModeChange(CalendarMode mode)
    {
        if (mode == Mode)
        {
            return;
        }

        SetCurrentValue(ModeProperty, mode);

        PanelChanged?.Invoke(this, new CalendarPanelChangedEventArgs(Value.Date, mode));
    }

    /// <summary>
    /// 为自定义 Header 模板构建强类型上下文。命令的 CanExecute 校验参数类型；
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

        return _presentationAdapter.CreateHeaderContext(this, changeValue, changeMode);
    }

    private sealed class CalendarRangeBarAttachment : IDisposable
    {
        private readonly IDisposable _resourceHostAttachment;

        public int ReferenceCount { get; set; } = 1;

        public CalendarRangeBarAttachment(IDisposable resourceHostAttachment)
        {
            _resourceHostAttachment = resourceHostAttachment;
        }

        public void Dispose()
        {
            _resourceHostAttachment.Dispose();
        }
    }
}
