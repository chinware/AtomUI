using AtomUI.Controls;

namespace AtomUI.Icons.AntDesign;

public partial class AntDesignIconPackage : IconPackage<AntDesignIconKind>
{
    public static AntDesignIconPackage Current { get; }

    static AntDesignIconPackage()
    {
        Current = new AntDesignIconPackage();
    }
    
    public AntDesignIconPackage()
        : base("AntDesign")
    {
        SetupIconPool();
    }

    private partial void SetupIconPool();
}