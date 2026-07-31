using System.Windows.Input;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// 自定义 Header 模板（<see cref="Calendar.HeaderTemplate"/>）的数据上下文。
/// 不暴露 Calendar 实例；模板只能通过命令提交用户意图。
/// </summary>
public sealed class CalendarHeaderContext
{
    public CalendarHeaderContext(DateTime value, CalendarMode mode, ICommand changeValueCommand, ICommand changeModeCommand)
    {
        Value = value;
        Mode = mode;
        ChangeValueCommand = changeValueCommand;
        ChangeModeCommand = changeModeCommand;
    }

    /// <summary>当前 Value。</summary>
    public DateTime Value { get; }

    /// <summary>当前 Mode。</summary>
    public CalendarMode Mode { get; }

    /// <summary>提交 <see cref="DateTime"/>，来源为 <see cref="CalendarSelectSource.Customize"/>。参数非 <see cref="DateTime"/> 时 CanExecute 为 false。</summary>
    public ICommand ChangeValueCommand { get; }

    /// <summary>提交 <see cref="CalendarMode"/>。参数非合法枚举时 CanExecute 为 false。</summary>
    public ICommand ChangeModeCommand { get; }
}
