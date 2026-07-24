using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Native;

internal sealed class WindowsCsdSizingHook : IDisposable
{
    private const uint WmGetMinMaxInfo = 0x0024;
    private const uint WmEnterSizeMove = 0x0231;
    private const uint WmExitSizeMove  = 0x0232;

    private readonly Window _window;
    private readonly Func<bool> _isEnabled;
    private bool _isDisposed;
    private bool _isUserResizeInProgress;

    public WindowsCsdSizingHook(Window window, Func<bool> isEnabled)
    {
        _window = window ?? throw new ArgumentNullException(nameof(window));
        _isEnabled = isEnabled ?? throw new ArgumentNullException(nameof(isEnabled));
        Win32Properties.AddWndProcHookCallback(_window, HandleWndProc);
    }

    public event EventHandler? UserResizeCompleted;

    public bool IsUserResizeInProgress => _isUserResizeInProgress;

    public void BeginUserResize()
    {
        if (_isDisposed)
        {
            return;
        }

        _isUserResizeInProgress = true;
    }

    public void CompleteUserResize()
    {
        if (_isDisposed || !_isUserResizeInProgress)
        {
            return;
        }

        _isUserResizeInProgress = false;
        UserResizeCompleted?.Invoke(this, EventArgs.Empty);
    }

    public void CancelUserResize()
    {
        _isUserResizeInProgress = false;
    }

    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _isDisposed = true;
        Win32Properties.RemoveWndProcHookCallback(_window, HandleWndProc);
        _isUserResizeInProgress = false;
        UserResizeCompleted = null;
    }

    internal static (int Width, int Height) CalculateTrackSize(
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

    private IntPtr HandleWndProc(
        IntPtr hWnd,
        uint msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        switch (msg)
        {
            case WmEnterSizeMove:
                BeginUserResize();
                break;
            case WmExitSizeMove:
                ScheduleUserResizeCompleted();
                break;
            case WmGetMinMaxInfo:
                if (TryApplyMinMaxInfo(lParam))
                {
                    handled = true;
                }
                break;
        }

        return IntPtr.Zero;
    }

    private bool TryApplyMinMaxInfo(IntPtr lParam)
    {
        if (_isDisposed ||
            lParam == IntPtr.Zero ||
            !_isEnabled() ||
            !_window.IsVisible ||
            !_window.IsExtendedIntoWindowDecorations ||
            _window.CanMaximize ||
            !HasTrackConstraint())
        {
            return false;
        }

        var minMaxInfo = Marshal.PtrToStructure<MinMaxInfo>(lParam);
        var frameClientDelta = ResolveFrameClientDelta();
        var minTrack = CalculateTrackSize(
            new Size(_window.MinWidth, _window.MinHeight),
            frameClientDelta,
            _window.RenderScaling);

        if (_window.MinWidth > 0)
        {
            minMaxInfo.MinTrackSize.X = minTrack.Width;
        }

        if (_window.MinHeight > 0)
        {
            minMaxInfo.MinTrackSize.Y = minTrack.Height;
        }

        var maxTrack = CalculateTrackSize(
            new Size(_window.MaxWidth, _window.MaxHeight),
            frameClientDelta,
            _window.RenderScaling);

        if (double.IsFinite(_window.MaxWidth) && _window.MaxWidth > 0)
        {
            minMaxInfo.MaxTrackSize.X = Math.Max(minMaxInfo.MinTrackSize.X, maxTrack.Width);
        }

        if (double.IsFinite(_window.MaxHeight) && _window.MaxHeight > 0)
        {
            minMaxInfo.MaxTrackSize.Y = Math.Max(minMaxInfo.MinTrackSize.Y, maxTrack.Height);
        }

        Marshal.StructureToPtr(minMaxInfo, lParam, false);
        return true;
    }

    private bool HasTrackConstraint()
    {
        return _window.MinWidth > 0 ||
               _window.MinHeight > 0 ||
               (double.IsFinite(_window.MaxWidth) && _window.MaxWidth > 0) ||
               (double.IsFinite(_window.MaxHeight) && _window.MaxHeight > 0);
    }

    private Size ResolveFrameClientDelta()
    {
        if (_window.FrameSize is not { } frameSize)
        {
            return default;
        }

        return new Size(
            Math.Max(0, frameSize.Width - _window.ClientSize.Width),
            Math.Max(0, frameSize.Height - _window.ClientSize.Height));
    }

    private void ScheduleUserResizeCompleted()
    {
        Dispatcher.UIThread.Post(
            () =>
            {
                if (!_isDisposed)
                {
                    CompleteUserResize();
                }
            },
            DispatcherPriority.Background);
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
}
