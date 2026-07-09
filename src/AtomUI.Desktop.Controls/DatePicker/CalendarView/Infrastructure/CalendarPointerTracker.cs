using Avalonia.Input;
using Avalonia.Input.Raw;

namespace AtomUI.Desktop.Controls.CalendarView.Infrastructure;

internal sealed class CalendarPointerTracker : IDisposable
{
    private readonly CalendarItem _owner;
    private IDisposable? _subscription;

    public CalendarPointerTracker(CalendarItem owner)
    {
        _owner = owner;
    }

    public void Attach(IInputManager? inputManager)
    {
        Detach();
        _subscription = inputManager?.Process.Subscribe(HandleRawInput);
    }

    public void Detach()
    {
        _subscription?.Dispose();
        _subscription = null;
    }

    public void Dispose()
    {
        Detach();
    }

    private void HandleRawInput(RawInputEventArgs args)
    {
        if (args is RawPointerEventArgs pointerEventArgs)
        {
            _owner.UpdatePointerMonthViewState(pointerEventArgs.Position);
        }
    }
}
