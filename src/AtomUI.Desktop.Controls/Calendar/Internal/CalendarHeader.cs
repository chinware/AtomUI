using System.Globalization;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>
/// Calendar 专用默认 Header。展示 Year Select、条件 Month Select 与 Month/Year 模式切换，
/// 只报告用户操作，不拥有 Value/Mode/选择状态。
/// </summary>
[TemplatePart(YearSelectPart, typeof(ComboBox))]
[TemplatePart(MonthSelectPart, typeof(ComboBox))]
[TemplatePart(ModeSwitchPart, typeof(Segmented))]
internal sealed class CalendarHeader : TemplatedControl
{
    internal const string YearSelectPart = "PART_YearSelect";
    internal const string MonthSelectPart = "PART_MonthSelect";
    internal const string ModeSwitchPart = "PART_ModeSwitch";

    public static readonly StyledProperty<DateTime> ValueProperty =
        AvaloniaProperty.Register<CalendarHeader, DateTime>(nameof(Value));

    public static readonly StyledProperty<CalendarMode> ModeProperty =
        AvaloniaProperty.Register<CalendarHeader, CalendarMode>(nameof(Mode));

    public static readonly StyledProperty<CalendarDateRange?> ValidRangeProperty =
        AvaloniaProperty.Register<CalendarHeader, CalendarDateRange?>(nameof(ValidRange));

    public static readonly StyledProperty<CultureInfo?> CultureProperty =
        AvaloniaProperty.Register<CalendarHeader, CultureInfo?>(nameof(Culture));

    public static readonly StyledProperty<bool> FullscreenProperty =
        AvaloniaProperty.Register<CalendarHeader, bool>(nameof(Fullscreen), true);

    public DateTime Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public CalendarMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public CalendarDateRange? ValidRange
    {
        get => GetValue(ValidRangeProperty);
        set => SetValue(ValidRangeProperty, value);
    }

    public CultureInfo? Culture
    {
        get => GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
    }

    public bool Fullscreen
    {
        get => GetValue(FullscreenProperty);
        set => SetValue(FullscreenProperty, value);
    }

    /// <summary>用户在 Header 选择了年份。参数是完整目标日期（保留月/日，收敛到合法月）。</summary>
    public event EventHandler<DateTime>? YearSelected;

    /// <summary>用户在 Header 选择了月份。参数是完整目标日期。</summary>
    public event EventHandler<DateTime>? MonthSelected;

    /// <summary>用户切换了 Month/Year 模式。</summary>
    public event EventHandler<CalendarMode>? ModeSwitched;

    private AtomUI.Desktop.Controls.ComboBox? _yearSelect;
    private AtomUI.Desktop.Controls.ComboBox? _monthSelect;
    private Segmented? _modeSwitch;
    private bool _suppress;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        Detach();

        _yearSelect = e.NameScope.Find<AtomUI.Desktop.Controls.ComboBox>(YearSelectPart);
        _monthSelect = e.NameScope.Find<AtomUI.Desktop.Controls.ComboBox>(MonthSelectPart);
        _modeSwitch = e.NameScope.Find<Segmented>(ModeSwitchPart);

        if (_yearSelect is not null)
        {
            _yearSelect.SelectionChanged += OnYearChanged;
        }

        if (_monthSelect is not null)
        {
            _monthSelect.SelectionChanged += OnMonthChanged;
        }

        if (_modeSwitch is not null)
        {
            _modeSwitch.SelectionChanged += OnModeChanged;
        }

        UpdateControlSizes();
        Rebuild();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FullscreenProperty)
        {
            UpdateControlSizes();
        }
        else if (change.Property == ValueProperty ||
                 change.Property == ModeProperty ||
                 change.Property == ValidRangeProperty ||
                 change.Property == CultureProperty)
        {
            Rebuild();
        }
    }

    private void Detach()
    {
        if (_yearSelect is not null)
        {
            _yearSelect.SelectionChanged -= OnYearChanged;
        }

        if (_monthSelect is not null)
        {
            _monthSelect.SelectionChanged -= OnMonthChanged;
        }

        if (_modeSwitch is not null)
        {
            _modeSwitch.SelectionChanged -= OnModeChanged;
        }
    }

    private void Rebuild()
    {
        if (_yearSelect is null || _monthSelect is null)
        {
            return;
        }

        _suppress = true;

        var culture = Culture ?? CultureInfo.CurrentCulture;
        var value = Value.Date;
        var yearSuffix = LanguageResourceBinder.GetLangResource(CalendarControlLangResourceKind.YearSuffix) ?? string.Empty;

        var years = CalendarHeaderOptions.BuildYearOptions(
            value.Year, ValidRange?.Start.Year, ValidRange?.End.Year);
        _yearSelect.ItemsSource = years.Select(y => new CalendarHeaderItem(y, y.ToString(CultureInfo.InvariantCulture) + yearSuffix)).ToList();
        _yearSelect.SelectedItem = ((System.Collections.Generic.IEnumerable<CalendarHeaderItem>)_yearSelect.ItemsSource)
            .FirstOrDefault(i => i.Key == value.Year);

        // Month Select 仅 Month 模式显示
        _monthSelect.IsVisible = Mode == CalendarMode.Month;
        if (_monthSelect.IsVisible)
        {
            var months = CalendarHeaderOptions.BuildMonthOptions(value.Year, ValidRange?.Start, ValidRange?.End);
            var monthNames = culture.DateTimeFormat.AbbreviatedMonthNames;
            _monthSelect.ItemsSource = months.Select(m => new CalendarHeaderItem(m, monthNames[m - 1])).ToList();
            _monthSelect.SelectedItem = ((System.Collections.Generic.IEnumerable<CalendarHeaderItem>)_monthSelect.ItemsSource)
                .FirstOrDefault(i => i.Key == value.Month);
        }

        if (_modeSwitch is not null)
        {
            if (_modeSwitch.Items.Count >= 2)
            {
                var monthLabel = LanguageResourceBinder.GetLangResource(CalendarControlLangResourceKind.Month)
                                 ?? CalendarControlLangResourceKind.Month.ToString();
                var yearLabel = LanguageResourceBinder.GetLangResource(CalendarControlLangResourceKind.Year)
                                 ?? CalendarControlLangResourceKind.Year.ToString();

                if (_modeSwitch.Items[0] is SegmentedItem monthItem)
                {
                    monthItem.Content = monthLabel;
                }

                if (_modeSwitch.Items[1] is SegmentedItem yearItem)
                {
                    yearItem.Content = yearLabel;
                }
            }

            _modeSwitch.SelectedIndex = Mode == CalendarMode.Year ? 1 : 0;
        }

        UpdateControlSizes();
        _suppress = false;
    }

    private void UpdateControlSizes()
    {
        var sizeType = Fullscreen ? CustomizableSizeType.Middle : CustomizableSizeType.Small;
        if (_yearSelect is not null)
        {
            _yearSelect.SizeType = sizeType;
        }

        if (_monthSelect is not null)
        {
            _monthSelect.SizeType = sizeType;
        }

        if (_modeSwitch is not null)
        {
            _modeSwitch.SizeType = sizeType;
        }
    }

    private void OnYearChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppress || _yearSelect?.SelectedItem is not CalendarHeaderItem item)
        {
            return;
        }

        var value = Value.Date;
        var year = item.Key;
        var month = CalendarHeaderOptions.ClampMonthToYear(value.Month, year, ValidRange?.Start, ValidRange?.End);
        var day = Math.Min(value.Day, DateTime.DaysInMonth(year, month));
        YearSelected?.Invoke(this, new DateTime(year, month, day));
    }

    private void OnMonthChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppress || _monthSelect?.SelectedItem is not CalendarHeaderItem item)
        {
            return;
        }

        var value = Value.Date;
        var month = item.Key;
        var day = Math.Min(value.Day, DateTime.DaysInMonth(value.Year, month));
        MonthSelected?.Invoke(this, new DateTime(value.Year, month, day));
    }

    private void OnModeChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppress || _modeSwitch is null)
        {
            return;
        }

        var mode = _modeSwitch.SelectedIndex == 1 ? CalendarMode.Year : CalendarMode.Month;
        if (mode != Mode)
        {
            ModeSwitched?.Invoke(this, mode);
        }
    }
}

/// <summary>Header 下拉项：业务键 + 显示文本。</summary>
internal sealed record CalendarHeaderItem(int Key, string Display)
{
    public override string ToString() => Display;
}
