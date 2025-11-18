using System.Diagnostics;
using AtomUI.Controls;

namespace AtomUI.Icons.AntDesign;

public class AntDesignIconInfoProvider : IconInfoProvider
{
    public AntDesignIconInfoProvider()
    {
    }
    
    public AntDesignIconInfoProvider(string kind)
        : base(kind)
    {
    }
    
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var iconInfo = AntDesignIconPackage.Current.GetIconInfo(Kind);
        Debug.Assert(iconInfo != null);
        return iconInfo;
    }
}