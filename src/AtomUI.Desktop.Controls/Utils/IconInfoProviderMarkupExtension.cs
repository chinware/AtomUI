using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using Avalonia.Markup.Xaml;

namespace AtomUI.Desktop.Controls.Utils;

public class IconInfoProvider : MarkupExtension
{
    public string Kind { get; set; }

    public IconInfoProvider()
    {
        Kind = string.Empty;
    }

    public IconInfoProvider(string kind)
    {
        Kind = kind;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return AntDesignIconPackage.Current.GetIconInfo(Kind) ?? new IconInfo();
    }
}