using AtomUI.Native;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;
using Avalonia.Platform;

namespace AtomUI.Controls;

internal class WindowBuddyLayer : WindowBase, IDisposable
{
    protected readonly IManagedPopupPositionerPopup? ManagedPopupPositionerPopup;

    static WindowBuddyLayer()
    {
        BackgroundProperty.OverrideDefaultValue<WindowBuddyLayer>(Brushes.Red);
    }
    
    public WindowBuddyLayer()
        : this(PlatformManager.CreateWindow(), null)
    {
    }
    
    public WindowBuddyLayer(IWindowImpl impl, IAvaloniaDependencyResolver? dependencyResolver)
        : base(impl, dependencyResolver)
    {
        IsHitTestVisible = false;
        this.SetWindowIgnoreMouseEvents(true);

        Topmost   = false;
        Focusable = false;
        Opacity   = 0.5;
        impl.SetSystemDecorations(SystemDecorations.None);
    }

    public void Dispose()
    {
        PlatformImpl?.Dispose();
    }

    // 这个地方我们可以需要定制
    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(500, 500);
    }
    
    public void MoveAndResize(Point point, Size size)
    {
        Width  = size.Width;
        Height = size.Height;
        ManagedPopupPositionerPopup?.MoveAndResize(new Point(Math.Round(point.X), Math.Floor(point.Y + 0.5)), size);
    }

}