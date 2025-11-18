using AtomUI.Controls;

namespace AtomUI.Icons.AntDesign;

public class AntDesignIconProvider : IconProvider
{
    public AntDesignIconProvider()
    {}
    
    public AntDesignIconProvider(string kind)
        : base(kind)
    {
    }
    
    protected override Icon GetIcon(string kind)
    {
        var icon = AntDesignIconPackage.Current.BuildIcon(Kind) ?? new Icon();
        return icon;
    }
}