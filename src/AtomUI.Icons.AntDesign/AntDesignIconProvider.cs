using System.Diagnostics;
using AtomUI.Controls;

namespace AtomUI.Icons.AntDesign;

public class AntDesignIconProvider : IconProvider<AntDesignIconKind>
{
    public AntDesignIconProvider()
    {}
    
    public AntDesignIconProvider(AntDesignIconKind kind)
        : base(kind)
    {
    }
    
    protected override Icon GetIcon(AntDesignIconKind kind)
    {
        var icon = AntDesignIconPackage.Current.BuildIcon(Kind);
        Debug.Assert(icon != null);
        return icon;
    }
}