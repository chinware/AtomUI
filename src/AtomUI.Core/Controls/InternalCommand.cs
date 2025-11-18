using System.Windows.Input;

namespace AtomUI.Controls;

internal class InternalCommand : ICommand
{
    private readonly Func<object?, bool>? _canExecute;
    private readonly Action<object?>      _execute;

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(_canExecute);
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }

    public InternalCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute    = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged;
}

internal class InvalidCommand : InternalCommand
{
    public static InvalidCommand Default { get; } = new();

    private InvalidCommand() : base(_ => { })
    {

    }
} 