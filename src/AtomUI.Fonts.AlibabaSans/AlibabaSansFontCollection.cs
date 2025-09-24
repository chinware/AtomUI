using Avalonia.Media.Fonts;

namespace AtomUI.Fonts.AlibabaSans;

public class AlibabaSansFontCollection : EmbeddedFontCollection
{
    public AlibabaSansFontCollection() : base(
        new Uri("fonts:AlibabaSans", UriKind.Absolute), 
        new Uri("avares://AtomUI.Fonts.AlibabaSans/Assets", UriKind.Absolute))
    {
    }
}