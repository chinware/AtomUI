using System;
using System.Windows.Input;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>
/// Calendar Header Context 使用的轻量 <see cref="ICommand"/>。参数校验交给 <c>canExecute</c>，
/// 非法参数时 <see cref="CanExecute"/> 返回 false 且 <see cref="Execute"/> 不动作。
/// </summary>
internal sealed class CalendarRelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool> _canExecute;

    public CalendarRelayCommand(Action<object?> execute, Func<object?, bool> canExecute)
    {
        _execute    = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => _canExecute(parameter);

    public void Execute(object? parameter)
    {
        if (!_canExecute(parameter))
        {
            return;
        }

        _execute(parameter);
    }

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
