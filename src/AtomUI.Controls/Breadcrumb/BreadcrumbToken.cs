using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class BreadcrumbToken : AbstractControlDesignToken
{
    public const string ID = "Breadcrumb";
    
    public BreadcrumbToken()
        : base(ID)
    {
    }
    
    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        Console.WriteLine("new CalculateFromAlias in BreadcrumbToken");
    }
}