using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace AtomUI.Controls.Utils;

internal static class MotionInvoker
{
    public static void Invoke(MotionActorControl target,
                              MotionConfig motionConfig,
                              Action? aboutToStart = null,
                              Action? completedAction = null)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            using var originRestore = new RenderTransformOriginRestore(target);
            target.RenderTransformOrigin = motionConfig.RenderTransformOrigin;
            if (aboutToStart != null)
            {
                aboutToStart();
            }

            foreach (var animation in motionConfig.Animations)
            {
                await animation.RunAsync(target);
            }

            if (completedAction != null)
            {
                completedAction();
            }
        });
    }
}

internal class RenderTransformOriginRestore : IDisposable
{
    RelativePoint _origin;
    Control _target;

    public RenderTransformOriginRestore(Control target)
    {
        _target = target;
        _origin = target.RenderTransformOrigin;
    }

    public void Dispose()
    {
        _target.RenderTransformOrigin = _origin;
    }
}