using Avalonia.Media.Fonts;

namespace AtomUI.Fonts.AlibabaPuHuiTi;

public class AlibabaPuHuiTiFontCollection : EmbeddedFontCollection
{
    public AlibabaPuHuiTiFontCollection()
        : base(new Uri("fonts:AlibabaPuHuiTi", UriKind.Absolute),
               new Uri("avares://AtomUI.Fonts.AlibabaPuHuiTi/Assets", UriKind.Absolute))
    {
    }
}
