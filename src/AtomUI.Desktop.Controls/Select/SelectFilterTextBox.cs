using Avalonia;
using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

internal class SelectFilterTextBox : TextBox
{
    private bool _isCaretLockedToStart;
    private bool _isResettingCaret;

    internal bool IsCaretLockedToStart
    {
        get => _isCaretLockedToStart;
        set
        {
            if (_isCaretLockedToStart == value)
            {
                return;
            }

            _isCaretLockedToStart = value;
            if (value)
            {
                ResetCaretToStart();
            }
        }
    }

    internal void ResetCaretToStart()
    {
        if (_isResettingCaret)
        {
            return;
        }

        _isResettingCaret = true;
        try
        {
            SelectionStart = 0;
            SelectionEnd   = 0;
            CaretIndex     = 0;
        }
        finally
        {
            _isResettingCaret = false;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (IsCaretLockedToStart &&
            !_isResettingCaret &&
            (change.Property == CaretIndexProperty ||
             change.Property == SelectionStartProperty ||
             change.Property == SelectionEndProperty))
        {
            ResetCaretToStart();
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (IsCaretLockedToStart)
        {
            if (Focusable && !IsFocused)
            {
                Focus(NavigationMethod.Pointer);
            }
            ResetCaretToStart();
            e.Handled = false;
            return;
        }

        base.OnPointerPressed(e);
        e.Handled = false;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (IsCaretLockedToStart)
        {
            ResetCaretToStart();
            e.Handled = false;
            return;
        }

        base.OnPointerReleased(e);
        e.Handled = false;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (IsCaretLockedToStart)
        {
            ResetCaretToStart();
            return;
        }

        base.OnKeyDown(e);
    }
}
