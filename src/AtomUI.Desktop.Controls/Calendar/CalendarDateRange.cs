using System;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// 表示一个首尾包含的有效日期范围，仅服务 <see cref="Calendar.ValidRange"/>。
/// </summary>
public sealed class CalendarDateRange
{
    public CalendarDateRange(DateTime start, DateTime end)
    {
        var s = start.Date;
        var e = end.Date;
        if (e < s)
        {
            throw new ArgumentOutOfRangeException(nameof(end), "range end must not be earlier than start");
        }

        Start = s;
        End   = e;
    }

    /// <summary>
    /// 范围起始日期，规范化到 <see cref="DateTime.Date"/>。
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// 范围结束日期，规范化到 <see cref="DateTime.Date"/>。
    /// </summary>
    public DateTime End { get; }
}
