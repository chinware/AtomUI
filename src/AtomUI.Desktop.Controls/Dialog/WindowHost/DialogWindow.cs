using AtomUI.Native;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class DialogWindow : Window
{
    private readonly INativeWindowSizingHook? _nativeSizingHook;
    private DialogWindowCloseState _closeState;
    private bool _isNativeUserResizeInProgress;

    internal event EventHandler? CloseRequested;
    internal event EventHandler? NativeUserResizeCompleted;

    internal bool IsNativeUserResizeInProgress =>
        _nativeSizingHook?.IsUserResizeInProgress ?? _isNativeUserResizeInProgress;

    protected override Type StyleKeyOverride { get; } = typeof(Window);

    internal DialogWindow()
    {
        _nativeSizingHook = NativeWindowSizing.TryAttachCsdSizingHook(this, () => IsCsdEnabled);
        if (_nativeSizingHook is not null)
        {
            _nativeSizingHook.UserResizeCompleted += HandleNativeUserResizeCompleted;
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
        ReleaseNativeSizingHook();
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
        ReleaseNativeSizingHook();
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
        if (_nativeSizingHook is not null)
        {
            _nativeSizingHook.BeginUserResize();
            return;
        }

        _isNativeUserResizeInProgress = true;
    }

    internal void CompleteNativeUserResize()
    {
        if (_nativeSizingHook is not null)
        {
            _nativeSizingHook.CompleteUserResize();
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
        _nativeSizingHook?.CancelUserResize();
        _isNativeUserResizeInProgress = false;
    }

    private void ReleaseNativeSizingHook()
    {
        if (_nativeSizingHook is null)
        {
            return;
        }

        _nativeSizingHook.UserResizeCompleted -= HandleNativeUserResizeCompleted;
        _nativeSizingHook.Dispose();
    }

    private void HandleNativeUserResizeCompleted(object? sender, EventArgs e)
    {
        NativeUserResizeCompleted?.Invoke(this, EventArgs.Empty);
    }

    private enum DialogWindowCloseState
    {
        Open,
        Closing
    }
}
