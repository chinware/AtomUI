using System.Globalization;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>
/// 一次有效 Cell 激活的内部事件参数。
/// </summary>
internal sealed class CalendarCellSelectedEventArgs : EventArgs
{
    public CalendarCellSelectedEventArgs(DateTime value, CalendarViewCellKind kind)
    {
        Value = value;
        Kind = kind;
    }

    public DateTime Value { get; }
    public CalendarViewCellKind Kind { get; }
}

/// <summary>
/// Calendar 的内部纯面板：按输入状态构建 Cell Model 网格，只向 Calendar 报告用户意图，
/// 不持有第二份 SelectedValue 或公开 Mode。
/// </summary>
[Avalonia.Controls.Metadata.PseudoClasses(
    CalendarRootPseudoClass.Fullscreen,
    CalendarRootPseudoClass.Mini,
    CalendarCellPseudoClass.Date,
    CalendarCellPseudoClass.Month,
    CalendarRootPseudoClass.ShowWeek)]
internal sealed class CalendarView : TemplatedControl
{
    public static readonly StyledProperty<DateTime> ValueProperty =
        AvaloniaProperty.Register<CalendarView, DateTime>(nameof(Value));

    public static readonly StyledProperty<DateTime> TodayProperty =
        AvaloniaProperty.Register<CalendarView, DateTime>(nameof(Today));

    public static readonly StyledProperty<CalendarViewMode> ViewModeProperty =
        AvaloniaProperty.Register<CalendarView, CalendarViewMode>(nameof(ViewMode));

    public static readonly StyledProperty<bool> ShowWeekProperty =
        AvaloniaProperty.Register<CalendarView, bool>(nameof(ShowWeek));

    public static readonly StyledProperty<bool> FullscreenProperty =
        AvaloniaProperty.Register<CalendarView, bool>(nameof(Fullscreen), true);

    public static readonly StyledProperty<CalendarDateRange?> ValidRangeProperty =
        AvaloniaProperty.Register<CalendarView, CalendarDateRange?>(nameof(ValidRange));

    public static readonly StyledProperty<Func<DateTime, bool>?> DisabledDateProperty =
        AvaloniaProperty.Register<CalendarView, Func<DateTime, bool>?>(nameof(DisabledDate));

    public static readonly StyledProperty<CultureInfo?> CultureProperty =
        AvaloniaProperty.Register<CalendarView, CultureInfo?>(nameof(Culture));

    public static readonly StyledProperty<IDataTemplate?> CellTemplateProperty =
        AvaloniaProperty.Register<CalendarView, IDataTemplate?>(nameof(CellTemplate));

    public static readonly StyledProperty<IDataTemplate?> FullCellTemplateProperty =
        AvaloniaProperty.Register<CalendarView, IDataTemplate?>(nameof(FullCellTemplate));

    public static readonly StyledProperty<double> MiniContentHeightProperty =
        AvaloniaProperty.Register<CalendarView, double>(nameof(MiniContentHeight), double.NaN);

    public static readonly StyledProperty<double> FullCellMinHeightProperty =
        AvaloniaProperty.Register<CalendarView, double>(nameof(FullCellMinHeight), double.NaN);

    public DateTime Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public DateTime Today
    {
        get => GetValue(TodayProperty);
        set => SetValue(TodayProperty, value);
    }

    public CalendarViewMode ViewMode
    {
        get => GetValue(ViewModeProperty);
        set => SetValue(ViewModeProperty, value);
    }

    public bool ShowWeek
    {
        get => GetValue(ShowWeekProperty);
        set => SetValue(ShowWeekProperty, value);
    }

    public bool Fullscreen
    {
        get => GetValue(FullscreenProperty);
        set => SetValue(FullscreenProperty, value);
    }

    public CalendarDateRange? ValidRange
    {
        get => GetValue(ValidRangeProperty);
        set => SetValue(ValidRangeProperty, value);
    }

    public Func<DateTime, bool>? DisabledDate
    {
        get => GetValue(DisabledDateProperty);
        set => SetValue(DisabledDateProperty, value);
    }

    public CultureInfo? Culture
    {
        get => GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
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

    public double MiniContentHeight
    {
        get => GetValue(MiniContentHeightProperty);
        set => SetValue(MiniContentHeightProperty, value);
    }

    public double FullCellMinHeight
    {
        get => GetValue(FullCellMinHeightProperty);
        set => SetValue(FullCellMinHeightProperty, value);
    }

    public CalendarView()
    {
        Focusable = true;
    }

    /// <summary>面板内当前聚焦的日期（roving focus 锚点）。仅面板内部状态，不代表选中。</summary>
    public DateTime FocusedValue { get; private set; }

    /// <summary>当前网格的不可变 Cell Model 列表。</summary>
    private IReadOnlyList<CalendarViewCellModel> _cellModels = Array.Empty<CalendarViewCellModel>();

    /// <summary>用户有效激活某个可选 Cell 时触发。</summary>
    public event EventHandler<CalendarCellSelectedEventArgs>? CellSelected;

    public IReadOnlyList<CalendarViewCellModel> CellModels => _cellModels;

    private ICalendarPresentationAdapter _presentationAdapter = DefaultCalendarPresentationAdapter.Instance;

    internal ICalendarPresentationAdapter PresentationAdapter
    {
        get => _presentationAdapter;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (ReferenceEquals(_presentationAdapter, value))
            {
                return;
            }

            ReleaseContainers();
            _cellPool.Clear();
            _presentationAdapter = value;
            RebuildCells();
            RealizeContainers();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ViewModeProperty || change.Property == ShowWeekProperty)
        {
            RebuildCells();
            RealizeContainers();
        }
        else if (change.Property == ValueProperty)
        {
            SyncValue(change.OldValue is DateTime oldValue ? oldValue.Date : Value.Date);
        }
        else if (change.Property == TodayProperty ||
                 change.Property == ValidRangeProperty ||
                 change.Property == DisabledDateProperty)
        {
            RefreshCellModels();
        }
        else if (change.Property == CultureProperty)
        {
            var oldCulture = change.OldValue as CultureInfo ?? CultureInfo.CurrentCulture;
            var newCulture = Culture ?? CultureInfo.CurrentCulture;
            var dateGridTopologyChanged = ViewMode == CalendarViewMode.Date &&
                                           oldCulture.DateTimeFormat.FirstDayOfWeek !=
                                           newCulture.DateTimeFormat.FirstDayOfWeek;
            if (dateGridTopologyChanged)
            {
                RebuildCells();
                RealizeContainers();
            }
            else
            {
                RefreshCellModels();
                BuildWeekHeader(newCulture, ViewMode == CalendarViewMode.Date);
            }
        }
        else if (change.Property == CellTemplateProperty || change.Property == FullCellTemplateProperty)
        {
            RealizeContainers();
        }
        else if (change.Property == FullscreenProperty)
        {
            foreach (var cell in _cellPool)
            {
                if (cell.Model is not null)
                {
                    cell.SetFullscreen(Fullscreen);
                }
            }
        }
        else if (change.Property == FullCellMinHeightProperty)
        {
            foreach (var cell in _cellPool)
            {
                cell.FullCellMinHeight = FullCellMinHeight;
            }
        }

        if (change.Property == FullscreenProperty ||
            change.Property == ViewModeProperty ||
            change.Property == ShowWeekProperty)
        {
            UpdateViewPseudoClasses();
        }
    }

    private void UpdateViewPseudoClasses()
    {
        PseudoClasses.Set(CalendarRootPseudoClass.Fullscreen, Fullscreen);
        PseudoClasses.Set(CalendarRootPseudoClass.Mini, !Fullscreen);
        PseudoClasses.Set(CalendarCellPseudoClass.Date, ViewMode == CalendarViewMode.Date);
        PseudoClasses.Set(CalendarCellPseudoClass.Month, ViewMode == CalendarViewMode.Month);
        PseudoClasses.Set(CalendarRootPseudoClass.ShowWeek, ShowWeek);
    }

    /// <summary>按当前输入重建 Cell Model 网格。仅在失效时调用，不在 Measure/Arrange 热路径执行。</summary>
    private void RebuildCells()
    {
        var culture = Culture ?? CultureInfo.CurrentCulture;
        var value = Value.Date;
        var today = Today == default ? DateTime.Today : Today.Date;
        var effectiveRange = PresentationAdapter.GetEffectiveRange(ValidRange);
        var start = effectiveRange.BuilderStart;
        var end = effectiveRange.BuilderEnd;

        if (ViewMode == CalendarViewMode.Month)
        {
            _cellModels = CalendarViewCellBuilder.BuildMonthCells(value, today, culture, start, end, DisabledDate);
            FocusedValue = value;
            return;
        }

        var dateCells = CalendarViewCellBuilder.BuildDateCells(
            value, today, culture.DateTimeFormat.FirstDayOfWeek, start, end, DisabledDate);

        if (ShowWeek)
        {
            var weekCells = CalendarViewCellBuilder.BuildWeekNumberCells(
                dateCells, culture, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
            var combined = new List<CalendarViewCellModel>(dateCells.Count + weekCells.Count);
            combined.AddRange(dateCells);
            combined.AddRange(weekCells);
            _cellModels = combined;
        }
        else
        {
            _cellModels = dateCells;
        }

        FocusedValue = value;
    }

    /// <summary>
    /// 同一日期网格内的 Value 变化只改变选中模型，不重新计算日期拓扑或重排容器。
    /// </summary>
    private void SyncValue(DateTime oldValue)
    {
        var value = Value.Date;
        if (ViewMode == CalendarViewMode.Date &&
            oldValue.Year == value.Year &&
            oldValue.Month == value.Month &&
            TrySyncDateSelection(value, oldValue))
        {
            FocusedValue = value;
            ResetCellFocus();
            SyncRealizedCells();
            return;
        }

        if (ViewMode == CalendarViewMode.Month &&
            oldValue.Year == value.Year &&
            _cellModels.Count == 12 &&
            _cellModels.Any(model =>
                model.Kind == CalendarViewCellKind.Month &&
                model.IsSelected &&
                model.Value.Year == oldValue.Year))
        {
            RebuildCells();
            ResetCellFocus();
            SyncRealizedCells();
            return;
        }

        RebuildCells();
        RealizeContainers();
    }

    private bool TrySyncDateSelection(DateTime value, DateTime oldValue)
    {
        if (_cellModels.Count is not (CalendarViewCellBuilder.DateGridCellCount or 48) ||
            _cellModels.Any(model => model.Kind is not CalendarViewCellKind.Date and not CalendarViewCellKind.Week))
        {
            return false;
        }

        if (!_cellModels.Any(model =>
                model.Kind == CalendarViewCellKind.Date &&
                model.IsSelected &&
                model.Value.Year == oldValue.Year &&
                model.Value.Month == oldValue.Month))
        {
            return false;
        }

        List<CalendarViewCellModel>? updatedModels = null;
        for (var index = 0; index < _cellModels.Count; index++)
        {
            var model = _cellModels[index];
            if (model.Kind == CalendarViewCellKind.Week)
            {
                continue;
            }

            var isSelected = model.Value.Date == value;
            if (model.IsSelected == isSelected)
            {
                continue;
            }

            updatedModels ??= new List<CalendarViewCellModel>(_cellModels);
            updatedModels[index] = model with { IsSelected = isSelected };
        }

        if (updatedModels is not null)
        {
            _cellModels = updatedModels;
        }

        return true;
    }

    private void RefreshCellModels()
    {
        RebuildCells();
        ResetCellFocus();
        SyncRealizedCells();
    }

    private void ResetCellFocus()
    {
        foreach (var cell in _cellPool)
        {
            cell.SetFocused(false);
        }
    }

    private void SyncRealizedCells()
    {
        foreach (var cell in _cellPool)
        {
            if (cell.Model is not { } currentModel ||
                FindEquivalentModel(currentModel) is not { } nextModel ||
                Equals(currentModel, nextModel))
            {
                continue;
            }

            cell.Bind(this, nextModel);
        }
    }

    private CalendarViewCellModel? FindEquivalentModel(CalendarViewCellModel currentModel)
    {
        foreach (var model in _cellModels)
        {
            var matches = model.Kind == currentModel.Kind &&
                          (model.Kind == CalendarViewCellKind.Month
                              ? model.Value.Year == currentModel.Value.Year && model.Value.Month == currentModel.Value.Month
                              : model.Value.Date == currentModel.Value.Date);
            if (matches)
            {
                return model;
            }
        }

        return null;
    }

    /// <summary>由 Cell 容器在有效激活时调用，转发用户意图给 Calendar。</summary>
    internal void ReportCellActivated(CalendarViewCellModel model)
    {
        if (model.IsDisabled)
        {
            return;
        }

        CellSelected?.Invoke(this, new CalendarCellSelectedEventArgs(model.Value, model.Kind));
    }

    #region Focus navigation

    protected override Avalonia.Automation.Peers.AutomationPeer OnCreateAutomationPeer() =>
        new CalendarViewAutomationPeer(this);

    protected override void OnKeyDown(Avalonia.Input.KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.Key)
        {
            case Avalonia.Input.Key.Left:
                e.Handled = MoveFocus(FocusDirection.Left);
                break;
            case Avalonia.Input.Key.Right:
                e.Handled = MoveFocus(FocusDirection.Right);
                break;
            case Avalonia.Input.Key.Up:
                e.Handled = MoveFocus(FocusDirection.Up);
                break;
            case Avalonia.Input.Key.Down:
                e.Handled = MoveFocus(FocusDirection.Down);
                break;
            case Avalonia.Input.Key.Enter:
            case Avalonia.Input.Key.Space:
                ActivateFocused();
                e.Handled = true;
                break;
        }
    }

    internal enum FocusDirection { Left, Right, Up, Down }

    /// <summary>
    /// 计算方向键移动后的目标 focus 值。Date 模式左右±1 天、上下±1 周；Month 模式左右±1 月、上下±3 月。
    /// 只在当前已生成网格范围内移动，跳过不可聚焦（禁用/周序号）Cell；无合法目标时返回原值。
    /// 纯计算，不改状态、不触发事件。
    /// </summary>
    internal DateTime ComputeFocusTarget(DateTime current, FocusDirection direction)
    {
        current = current.Date;
        var step = ViewMode == CalendarViewMode.Month
            ? MonthStep(direction)
            : DateStep(direction);

        if (step == 0)
        {
            return current;
        }

        var candidate = current;
        while (true)
        {
            if (!TryAdvance(candidate, step, out candidate))
            {
                return current;
            }

            if (!TryGetMatchingCell(candidate, out var model))
            {
                return current;
            }

            if (model.IsFocusable)
            {
                return candidate.Date;
            }
        }
    }

    /// <summary>移动 roving focus（只改 FocusedValue 与伪类，不选择、不触发公开事件）。返回是否移动成功。</summary>
    internal bool MoveFocus(FocusDirection direction)
    {
        var target = ComputeFocusTarget(FocusedValue, direction);
        if (target == FocusedValue.Date)
        {
            return false;
        }

        SetFocusedValue(target);
        return true;
    }

    /// <summary>激活当前 focused cell，进入与 Pointer 相同的选择流程。</summary>
    internal void ActivateFocused()
    {
        if (TryGetMatchingCell(FocusedValue, out var model) && model.IsFocusable)
        {
            ReportCellActivated(model);
        }
    }

    private void SetFocusedValue(DateTime value, bool moveInputFocus = true)
    {
        FocusedValue = value.Date;
        CalendarViewCell? focusTarget = null;
        foreach (var cell in _cellPool)
        {
            var isFocused = cell.Model is { } m && m.Kind != CalendarViewCellKind.Week && m.Value.Date == FocusedValue;
            cell.SetFocused(isFocused);
            if (isFocused)
            {
                focusTarget = cell;
            }
        }

        if (moveInputFocus)
        {
            focusTarget?.Focus();
        }
    }

    /// <summary>进入面板时优先聚焦选中且可用的 Cell，否则第一个可用 Cell。</summary>
    protected override void OnGotFocus(Avalonia.Input.FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        if (!ReferenceEquals(e.Source, this))
        {
            return;
        }

        foreach (var model in _cellModels)
        {
            if (model.Kind != CalendarViewCellKind.Week && model.Value.Date == FocusedValue && model.IsFocusable)
            {
                SetFocusedValue(FocusedValue);
                return;
            }
        }

        foreach (var model in _cellModels)
        {
            if (model.Kind != CalendarViewCellKind.Week && model.IsFocusable)
            {
                SetFocusedValue(model.Value);
                return;
            }
        }
    }

    private static int DateStep(FocusDirection d) => d switch
    {
        FocusDirection.Left => -1,
        FocusDirection.Right => 1,
        FocusDirection.Up => -7,
        FocusDirection.Down => 7,
        _ => 0
    };

    private static int MonthStep(FocusDirection d) => d switch
    {
        FocusDirection.Left => -1,
        FocusDirection.Right => 1,
        FocusDirection.Up => -3,
        FocusDirection.Down => 3,
        _ => 0
    };

    private bool TryAdvance(DateTime value, int step, out DateTime result)
    {
        try
        {
            result = ViewMode == CalendarViewMode.Month
                ? value.AddMonths(step)
                : value.AddDays(step);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = value;
            return false;
        }
    }

    /// <summary>目标值是否落在当前网格内且对应一个可聚焦 Cell。</summary>
    private bool IsFocusable(DateTime value)
    {
        return TryGetMatchingCell(value, out var model) && model.IsFocusable;
    }

    private bool TryGetMatchingCell(DateTime value, out CalendarViewCellModel model)
    {
        foreach (var item in _cellModels)
        {
            if (item.Kind == CalendarViewCellKind.Week)
            {
                continue;
            }

            var match = ViewMode == CalendarViewMode.Month
                ? item.Value.Year == value.Year && item.Value.Month == value.Month
                : item.Value.Date == value.Date;

            if (match)
            {
                model = item;
                return true;
            }
        }

        model = default!;
        return false;
    }

    #endregion

    #region Template parts and container generation

    internal const string WeekHeaderPart = "PART_WeekHeader";
    internal const string CellHostPart = "PART_CellHost";

    private Panel? _weekHeader;
    private Grid? _cellHost;

    /// <summary>有界容器池：复用 CalendarViewCell 实例，避免 Mode/Value 变化无限增加容器。</summary>
    private readonly List<CalendarViewCell> _cellPool = new();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ReleaseContainers();
        _weekHeader = e.NameScope.Find<Panel>(WeekHeaderPart);
        _cellHost = e.NameScope.Find<Grid>(CellHostPart);
        RebuildCells();
        RealizeContainers();
        UpdateViewPseudoClasses();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_cellHost is not null && _cellHost.Children.Count == 0)
        {
            RebuildCells();
            RealizeContainers();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ReleaseContainers();
    }

    /// <summary>按当前 Cell Model 生成/复用容器并填入 CellHost，同时刷新周标题。</summary>
    private void RealizeContainers()
    {
        if (_cellHost is null)
        {
            return;
        }

        var culture = Culture ?? CultureInfo.CurrentCulture;
        var isDate = ViewMode == CalendarViewMode.Date;
        var columns = isDate ? (ShowWeek ? 8 : 7) : 3;
        var requiredCellCount = isDate
            ? (ShowWeek ? 48 : 42)
            : 12;

        ConfigureGrid(_cellHost, columns, isDate ? 6 : 4);
        ConfigureWeekHeader(columns);
        BuildWeekHeader(culture, isDate);
        TrimRealizedCells(requiredCellCount);

        if (isDate)
        {
            RealizeDateGrid(columns);
        }
        else
        {
            RealizeMonthGrid();
        }
    }

    private void RealizeDateGrid(int columns)
    {
        // _cellModels: 42 date cells (row-major) + optional 6 week cells appended.
        var hasWeek = ShowWeek && _cellModels.Count == 48;
        var poolIndex = 0;

        for (var row = 0; row < 6; row++)
        {
            var col = 0;
            if (hasWeek)
            {
                var weekModel = _cellModels[42 + row];
                var cellIndex = poolIndex++;
                PlaceCell(GetPooledCell(cellIndex), weekModel, row, col, cellIndex);
                col++;
            }

            for (var d = 0; d < 7; d++)
            {
                var dateModel = _cellModels[row * 7 + d];
                var cellIndex = poolIndex++;
                PlaceCell(GetPooledCell(cellIndex), dateModel, row, col, cellIndex);
                col++;
            }
        }
    }

    private void RealizeMonthGrid()
    {
        for (var i = 0; i < _cellModels.Count; i++)
        {
            var row = i / 3;
            var col = i % 3;
            PlaceCell(GetPooledCell(i), _cellModels[i], row, col, i);
        }
    }

    private void PlaceCell(CalendarViewCell cell, CalendarViewCellModel model, int row, int col, int index)
    {
        if (cell.FullCellMinHeight != FullCellMinHeight)
        {
            cell.FullCellMinHeight = FullCellMinHeight;
        }

        var cellTemplate = CellTemplate;
        if (!ReferenceEquals(cell.CellTemplate, cellTemplate))
        {
            cell.CellTemplate = cellTemplate;
        }

        var fullCellTemplate = FullCellTemplate;
        if (!ReferenceEquals(cell.FullCellTemplate, fullCellTemplate))
        {
            cell.FullCellTemplate = fullCellTemplate;
        }

        cell.Bind(this, model);
        if (Grid.GetRow(cell) != row)
        {
            Grid.SetRow(cell, row);
        }

        if (Grid.GetColumn(cell) != col)
        {
            Grid.SetColumn(cell, col);
        }

        if (_cellHost!.Children.Count == index)
        {
            _cellHost.Children.Add(cell);
        }
        else if (_cellHost.Children.Count < index || !ReferenceEquals(_cellHost.Children[index], cell))
        {
            // Fallback for unexpected external child mutation; normal realization stays O(n).
            if (!_cellHost.Children.Contains(cell))
            {
                _cellHost.Children.Add(cell);
            }
        }
    }

    private CalendarViewCell GetPooledCell(int index)
    {
        while (_cellPool.Count <= index)
        {
            _cellPool.Add(PresentationAdapter.CreateCell());
        }

        return _cellPool[index];
    }

    private void TrimRealizedCells(int requiredCount)
    {
        if (_cellHost is null)
        {
            return;
        }

        for (var i = _cellHost.Children.Count - 1; i >= requiredCount; i--)
        {
            if (_cellHost.Children[i] is CalendarViewCell cell)
            {
                cell.Unbind();
            }

            _cellHost.Children.RemoveAt(i);
        }
    }

    private void ReleaseContainers()
    {
        _cellHost?.Children.Clear();
        _weekHeader?.Children.Clear();
        foreach (var cell in _cellPool)
        {
            cell.Unbind();
        }
    }

    internal IEnumerable<CalendarViewCell> GetRealizedCells()
    {
        foreach (var cell in _cellPool)
        {
            if (cell.Model is not null)
            {
                yield return cell;
            }
        }
    }

    internal void RefreshPresentation()
    {
        foreach (var cell in _cellPool)
        {
            if (cell.Model is { } model)
            {
                cell.Bind(this, model);
            }
        }
    }

    private void BuildWeekHeader(CultureInfo culture, bool isDate)
    {
        if (_weekHeader is null)
        {
            return;
        }

        if (_weekHeader.IsVisible != isDate)
        {
            _weekHeader.IsVisible = isDate;
        }

        if (!isDate)
        {
            return;
        }

        var first = culture.DateTimeFormat.FirstDayOfWeek;
        var names = culture.DateTimeFormat.AbbreviatedDayNames;
        var requiredCount = ShowWeek ? 8 : 7;
        SyncWeekHeaderChildren(requiredCount);

        var index = 0;
        if (ShowWeek)
        {
            var weekLabel = LanguageResourceBinder.GetLangResource(CalendarControlLangResourceKind.Week)
                            ?? CalendarControlLangResourceKind.Week.ToString();
            UpdateWeekHeaderText(index++, weekLabel, 0);
        }

        var columnOffset = ShowWeek ? 1 : 0;
        for (var i = 0; i < 7; i++)
        {
            var day = (DayOfWeek)(((int)first + i) % 7);
            UpdateWeekHeaderText(index++, names[(int)day], i + columnOffset);
        }
    }

    private void SyncWeekHeaderChildren(int requiredCount)
    {
        while (_weekHeader!.Children.Count > requiredCount)
        {
            _weekHeader.Children.RemoveAt(_weekHeader.Children.Count - 1);
        }

        while (_weekHeader.Children.Count < requiredCount)
        {
            _weekHeader.Children.Add(CreateWeekHeaderText());
        }
    }

    private void UpdateWeekHeaderText(int index, string text, int column)
    {
        var label = GetWeekHeaderText(index);
        if (label.Text != text)
        {
            label.Text = text;
        }

        if (Grid.GetColumn(label) != column)
        {
            Grid.SetColumn(label, column);
        }
    }

    private TextBlock GetWeekHeaderText(int index)
    {
        if (_weekHeader!.Children[index] is TextBlock label)
        {
            return label;
        }

        label = CreateWeekHeaderText();
        _weekHeader.Children.RemoveAt(index);
        _weekHeader.Children.Insert(index, label);
        return label;
    }

    private static TextBlock CreateWeekHeaderText()
    {
        return new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            TextAlignment = TextAlignment.Center
        };
    }

    private void ConfigureWeekHeader(int columns)
    {
        if (_weekHeader is not Grid headerGrid)
        {
            return;
        }

        if (headerGrid.ColumnDefinitions.Count == columns)
        {
            return;
        }

        headerGrid.ColumnDefinitions.Clear();
        for (var column = 0; column < columns; column++)
        {
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }
    }

    private static void ConfigureGrid(Grid grid, int columns, int rows)
    {
        if (grid.ColumnDefinitions.Count == columns &&
            grid.RowDefinitions.Count == rows)
        {
            return;
        }

        grid.ColumnDefinitions.Clear();
        grid.RowDefinitions.Clear();
        for (var c = 0; c < columns; c++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        for (var r = 0; r < rows; r++)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        }
    }

    #endregion
}
