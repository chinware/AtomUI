using Avalonia;

namespace AtomUI.Controls;

internal interface IShadowMaskInfoProvider
{
    CornerRadius GetMaskCornerRadius();
    Rect GetMaskBounds();
}

internal interface IArrowAwareShadowMaskInfoProvider : IShadowMaskInfoProvider
{
    bool IsShowArrow { get; }
    ArrowPosition ArrowPosition { get; }
    Rect ArrowIndicatorBounds { get; }
}