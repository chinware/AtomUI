using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal interface IShadowMaskInfoProvider
{
    CornerRadius GetMaskCornerRadius();
    Rect GetMaskBounds();
    IBrush? GetMaskBackground();
}

internal interface IArrowAwareShadowMaskInfoProvider : IShadowMaskInfoProvider
{
    bool IsShowArrow();
    ArrowPosition GetArrowPosition();
    Rect GetArrowIndicatorBounds();
    Rect GetArrowIndicatorLayoutBounds();
    void SetArrowOpacity(double opacity);
    ArrowDecoratedBox GetArrowDecoratedBox();
}