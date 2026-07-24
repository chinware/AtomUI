using AtomUI.Native;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class DialogWindow : Window
{
    private readonly WindowsCsdSizingHook? _windowsSizingHook;
    private DialogWindowCloseState _closeState;
    private bool _isNativeUserResizeInProgress;

    internal event EventHandler? CloseRequested;
    internal event EventHandler? NativeUserResizeCompleted;

    internal bool IsNativeUserResizeInProgress =>
        _windowsSizingHook?.IsUserResizeInProgress ?? _isNativeUserResizeInProgress;

    protected override Type StyleKeyOverride { get; } = typeof(Window);

    internal DialogWindow()
    {
        if (OperatingSystem.IsWindows())
        {
            _windowsSizingHook = new WindowsCsdSizingHook(this, () => IsCsdEnabled);
            _windowsSizingHook.UserResizeCompleted += HandleWindowsUserResizeCompleted;
        }
    }

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

    protected override void OnClosed(EventArgs e)
    {
        ReleaseWindowsSizingHook();
        CancelNativeUserResize();
        base.OnClosed(e);
    }

    internal void CloseFromPresenter()
    {
        _closeState = DialogWindowCloseState.Closing;
        Close();
    }

    internal void ReleasePresenterHooks()
    {
        ReleaseWindowsSizingHook();
        CancelNativeUserResize();
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
        if (IsVisible && IsNativeUserResizeInProgress)
        {
            return ClientSize;
        }

        Width  = requestedSize.Width;
        Height = requestedSize.Height;
        if (!IsVisible)
        {
            if (ClientSize != requestedSize)
            {
                ClientSize = requestedSize;
            }

            return requestedSize;
        }

        if (requestedSize != ClientSize)
        {
            ClientSize = requestedSize;
        }

        return requestedSize;
    }

    internal void BeginNativeUserResize()
    {
        if (_windowsSizingHook is not null)
        {
            _windowsSizingHook.BeginUserResize();
            return;
        }

        _isNativeUserResizeInProgress = true;
    }

    internal void CompleteNativeUserResize()
    {
        if (_windowsSizingHook is not null)
        {
            _windowsSizingHook.CompleteUserResize();
            return;
        }

        if (!_isNativeUserResizeInProgress)
        {
            return;
        }

        _isNativeUserResizeInProgress = false;
        NativeUserResizeCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void CancelNativeUserResize()
    {
        _windowsSizingHook?.CancelUserResize();
        _isNativeUserResizeInProgress = false;
    }

    private void ReleaseWindowsSizingHook()
    {
        if (_windowsSizingHook is null)
        {
            return;
        }

        _windowsSizingHook.UserResizeCompleted -= HandleWindowsUserResizeCompleted;
        _windowsSizingHook.Dispose();
    }

    private void HandleWindowsUserResizeCompleted(object? sender, EventArgs e)
    {
        NativeUserResizeCompleted?.Invoke(this, EventArgs.Empty);
    }

    private enum DialogWindowCloseState
    {
        Open,
        Closing
    }
}
