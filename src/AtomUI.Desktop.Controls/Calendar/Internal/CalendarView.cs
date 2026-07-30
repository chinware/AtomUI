using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>
/// 一次有效 Cell 激活的内部事件参数。
/// </summary>
internal sealed class CalendarCellSelectedEventArgs : EventArgs
{
    public CalendarCellSelectedEventArgs(DateTime value, CalendarViewCellKind kind)
    {
        Value = value;
        Kind  = kind;
    }

    public DateTime Value { get; }
    public CalendarViewCellKind Kind { get; }
}

/// <summary>
/// Calendar 的内部纯面板：按输入状态构建 Cell Model 网格，只向 Calendar 报告用户意图，
/// 不持有第二份 SelectedValue 或公开 Mode（spec §7.4）。
/// </summary>
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

    /// <summary>面板内当前聚焦的日期（roving focus 锚点）。仅面板内部状态，不代表选中。</summary>
    public DateTime FocusedValue { get; private set; }

    /// <summary>当前网格的不可变 Cell Model 列表。</summary>
    private IReadOnlyList<CalendarViewCellModel> _cellModels = Array.Empty<CalendarViewCellModel>();

    /// <summary>Cell 模型失效需要重建的属性集合（spec §14.2）。</summary>
    private static readonly HashSet<AvaloniaProperty> RebuildTriggers = new()
    {
        ValueProperty, TodayProperty, ViewModeProperty, ShowWeekProperty,
        ValidRangeProperty, DisabledDateProperty, CultureProperty
    };

    /// <summary>用户有效激活某个可选 Cell 时触发。</summary>
    public event EventHandler<CalendarCellSelectedEventArgs>? CellSelected;

    public IReadOnlyList<CalendarViewCellModel> CellModels => _cellModels;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (RebuildTriggers.Contains(change.Property))
        {
            RebuildCells();
        }
    }

    /// <summary>按当前输入重建 Cell Model 网格。仅在失效时调用，不在 Measure/Arrange 热路径执行。</summary>
    private void RebuildCells()
    {
        var culture = Culture ?? CultureInfo.CurrentCulture;
        var value   = Value.Date;
        var today   = Today == default ? DateTime.Today : Today.Date;
        var start   = ValidRange?.Start;
        var end     = ValidRange?.End;

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

    /// <summary>由 Cell 容器在有效激活时调用，转发用户意图给 Calendar。</summary>
    internal void ReportCellActivated(CalendarViewCellModel model)
    {
        if (model.IsDisabled || model.Kind == CalendarViewCellKind.Week)
        {
            return;
        }

        CellSelected?.Invoke(this, new CalendarCellSelectedEventArgs(model.Value, model.Kind));
    }
}
