using Avalonia;

namespace AtomUI.Controls;

public interface IShadowMaskInfoProvider
{
    public CornerRadius GetMaskCornerRadius();
    public Rect GetMaskBounds();
}