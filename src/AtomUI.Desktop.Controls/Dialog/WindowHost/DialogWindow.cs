using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

internal sealed class DialogWindow : Window
{
    private const uint WmGetMinMaxInfo = 0x0024;
    private const uint WmEnterSizeMove  = 0x0231;
    private const uint WmExitSizeMove   = 0x0232;

    private DialogWindowCloseState _closeState;

    internal event EventHandler? CloseRequested;
    internal event EventHandler? NativeUserResizeCompleted;

    internal bool IsNativeUserResizeInProgress { get; private set; }

    protected override Type StyleKeyOverride { get; } = typeof(Window);

    internal DialogWindow()
    {
        if (OperatingSystem.IsWindows())
        {
            Win32Properties.AddWndProcHookCallback(this, HandleWindowsWndProc);
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
        ReleaseWindowsWndProcHook();
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
        ReleaseWindowsWndProcHook();
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
        IsNativeUserResizeInProgress = true;
    }

    internal void CompleteNativeUserResize()
    {
        if (!IsNativeUserResizeInProgress)
        {
            return;
        }

        IsNativeUserResizeInProgress = false;
        NativeUserResizeCompleted?.Invoke(this, EventArgs.Empty);
    }

    internal static (int Width, int Height) CalculateWindowsCsdTrackSize(
        Size clientSize,
        Size frameClientDelta,
        double scaling)
    {
        var effectiveScaling = double.IsFinite(scaling) && scaling > 0
            ? scaling
            : 1;
        return (
            ToTrackLength(clientSize.Width, frameClientDelta.Width, effectiveScaling),
            ToTrackLength(clientSize.Height, frameClientDelta.Height, effectiveScaling));
    }

    private IntPtr HandleWindowsWndProc(
        IntPtr hWnd,
        uint msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        switch (msg)
        {
            case WmEnterSizeMove:
                BeginNativeUserResize();
                break;
            case WmExitSizeMove:
                ScheduleNativeUserResizeCompleted();
                break;
            case WmGetMinMaxInfo:
                if (TryApplyWindowsCsdMinMaxInfo(lParam))
                {
                    handled = true;
                }
                break;
        }

        return IntPtr.Zero;
    }

    private bool TryApplyWindowsCsdMinMaxInfo(IntPtr lParam)
    {
        if (lParam == IntPtr.Zero ||
            !IsVisible ||
            !IsCsdEnabled ||
            !IsExtendedIntoWindowDecorations ||
            CanMaximize ||
            !HasWindowsTrackConstraint())
        {
            return false;
        }

        var minMaxInfo = Marshal.PtrToStructure<MinMaxInfo>(lParam);
        var frameClientDelta = ResolveFrameClientDelta();
        var minTrack = CalculateWindowsCsdTrackSize(
            new Size(MinWidth, MinHeight),
            frameClientDelta,
            RenderScaling);

        if (MinWidth > 0)
        {
            minMaxInfo.MinTrackSize.X = minTrack.Width;
        }

        if (MinHeight > 0)
        {
            minMaxInfo.MinTrackSize.Y = minTrack.Height;
        }

        if (double.IsFinite(MaxWidth) && MaxWidth > 0)
        {
            var maxTrack = CalculateWindowsCsdTrackSize(
                new Size(MaxWidth, MaxHeight),
                frameClientDelta,
                RenderScaling);
            minMaxInfo.MaxTrackSize.X = Math.Max(minMaxInfo.MinTrackSize.X, maxTrack.Width);
        }

        if (double.IsFinite(MaxHeight) && MaxHeight > 0)
        {
            var maxTrack = CalculateWindowsCsdTrackSize(
                new Size(MaxWidth, MaxHeight),
                frameClientDelta,
                RenderScaling);
            minMaxInfo.MaxTrackSize.Y = Math.Max(minMaxInfo.MinTrackSize.Y, maxTrack.Height);
        }

        Marshal.StructureToPtr(minMaxInfo, lParam, false);
        return true;
    }

    private bool HasWindowsTrackConstraint()
    {
        return MinWidth > 0 ||
               MinHeight > 0 ||
               (double.IsFinite(MaxWidth) && MaxWidth > 0) ||
               (double.IsFinite(MaxHeight) && MaxHeight > 0);
    }

    private Size ResolveFrameClientDelta()
    {
        if (FrameSize is not { } frameSize)
        {
            return default;
        }

        return new Size(
            Math.Max(0, frameSize.Width - ClientSize.Width),
            Math.Max(0, frameSize.Height - ClientSize.Height));
    }

    private void CancelNativeUserResize()
    {
        IsNativeUserResizeInProgress = false;
    }

    private void ScheduleNativeUserResizeCompleted()
    {
        Dispatcher.UIThread.Post(
            CompleteNativeUserResize,
            DispatcherPriority.Background);
    }

    private void ReleaseWindowsWndProcHook()
    {
        if (OperatingSystem.IsWindows())
        {
            Win32Properties.RemoveWndProcHookCallback(this, HandleWindowsWndProc);
        }
    }

    private static int ToTrackLength(double clientLength, double frameClientDelta, double scaling)
    {
        var client = double.IsFinite(clientLength) ? Math.Max(0, clientLength) : 0;
        var frame = double.IsFinite(frameClientDelta) ? Math.Max(0, frameClientDelta) : 0;
        return Math.Max(1, (int)Math.Ceiling((client + frame) * scaling));
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Win32Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MinMaxInfo
    {
        public Win32Point Reserved;
        public Win32Point MaxSize;
        public Win32Point MaxPosition;
        public Win32Point MinTrackSize;
        public Win32Point MaxTrackSize;
    }

    private enum DialogWindowCloseState
    {
        Open,
        Closing
    }
}
