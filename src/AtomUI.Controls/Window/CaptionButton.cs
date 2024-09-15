using Avalonia;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;

public class CaptionButton : AvaloniaButton
{
    public static readonly StreamGeometry WindowCloseIconGlyph = StreamGeometry.Parse(
        "M13.46,12L19,17.54V19H17.54L12,13.46L6.46,19H5V17.54L10.54,12L5,6.46V5H6.46L12,10.54L17.54,5H19V6.46L13.46,12Z");

    public static readonly StreamGeometry WindowMaximizeGlyph = StreamGeometry.Parse("M4,4H20V20H4V4M6,8V18H18V8H6Z");
    public static readonly StreamGeometry WindowMinimizeGlyph = StreamGeometry.Parse("M20,14H4V10H20");

    public static readonly StreamGeometry WindowRestoreGlyph =
        StreamGeometry.Parse("M4,8H8V4H20V16H16V20H4V8M16,8V14H18V6H10V8H16M6,12V18H14V12H6Z");

    public static readonly StreamGeometry WindowExpandGlyph = StreamGeometry.Parse(
        "M10,21V19H6.41L10.91,14.5L9.5,13.09L5,17.59V14H3V21H10M14.5,10.91L19,6.41V10H21V3H14V5H17.59L13.09,9.5L14.5,10.91Z");

    public static readonly StreamGeometry WindowCollapseGlyph = StreamGeometry.Parse(
        "M19.5,3.09L15,7.59V4H13V11H20V9H16.41L20.91,4.5L19.5,3.09M4,13V15H7.59L3.09,19.5L4.5,20.91L9,16.41V20H11V13H4Z");

    protected override Size MeasureCore(Size availableSize)
    {
        // 创建按钮内容即可
        if (IsVisible)
        {
            SetupProperties();
        }

        return base.MeasureCore(availableSize);
    }

    private void SetupProperties()
    {
        // _controlTokenBinder.AddStyleBinding(BackgroundProperty, "HoverBackgroundColor");
        // _controlTokenBinder.AddStyleBinding(BorderBrushProperty, "PressedBackgroundColor");
        // _controlTokenBinder.AddStyleBinding(ForegroundProperty, "ForegroundColor");

        CornerRadius      = new CornerRadius(6);
        Margin            = new Thickness(0, 4);
        Padding           = new Thickness(4);
        Width             = 28;
        Height            = 28;
        Cursor            = new Cursor(StandardCursorType.Hand);
        VerticalAlignment = VerticalAlignment.Stretch;
    }
}