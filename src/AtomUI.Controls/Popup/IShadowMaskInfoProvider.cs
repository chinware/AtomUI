using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal interface IShadowMaskInfoProvider
{
    CornerRadius GetMaskCornerRadius();
    Rect GetMaskBounds();
    IBrush? GetMaskBackground();
}

internal interface IArrowAwareShadowMaskInfoProvider : IShadowMaskInfoProvider
{
    bool IsShowArrow { get; }
    ArrowPosition ArrowPosition { get; }
    Rect ArrowIndicatorBounds { get; }
}