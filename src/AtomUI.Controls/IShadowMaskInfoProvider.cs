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
    bool IsArrowVisible();
    ArrowPosition GetArrowPosition();
    Rect GetArrowIndicatorBounds();
    Rect GetArrowIndicatorLayoutBounds();
    void SetArrowOpacity(double opacity);
    AbstractArrowDecoratedBox GetArrowDecoratedBox();
}