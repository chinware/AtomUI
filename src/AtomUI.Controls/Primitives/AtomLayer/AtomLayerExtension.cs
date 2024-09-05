using Avalonia;

namespace AtomUI.Controls.Primitives;

public static class AtomLayerExtension
{
    public static AtomLayer? GetLayer(this Visual? visual)
    {
        return visual == null ? null : AtomLayer.GetLayer(visual);
    }
}