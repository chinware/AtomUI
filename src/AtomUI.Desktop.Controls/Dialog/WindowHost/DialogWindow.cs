using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class DialogWindow : Window
{
    private DialogWindowCloseState _closeState;

    internal event EventHandler? CloseRequested;

    protected override Type StyleKeyOverride { get; } = typeof(Window);

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (_closeState == DialogWindowCloseState.Open &&
            e.CloseReason != WindowCloseReason.OwnerWindowClosing)
        {
            e.Cancel = true;
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        base.OnClosing(e);
    }

    internal void CloseFromPresenter()
    {
        _closeState = DialogWindowCloseState.Closing;
        Close();
    }

    internal Size ApplyRequestedSize(double width, double height)
    {
        var requestedSize = new Size(
            double.IsNaN(width)
                ? Math.Clamp(ClientSize.Width, MinWidth, MaxWidth)
                : Math.Clamp(width, MinWidth, MaxWidth),
            double.IsNaN(height)
                ? Math.Clamp(ClientSize.Height, MinHeight, MaxHeight)
                : Math.Clamp(height, MinHeight, MaxHeight));
        Width  = requestedSize.Width;
        Height = requestedSize.Height;
        if (!IsVisible)
        {
            return requestedSize;
        }

        if (requestedSize != ClientSize)
        {
            ClientSize = requestedSize;
        }

        return requestedSize;
    }

    private enum DialogWindowCloseState
    {
        Open,
        Closing
    }
}
