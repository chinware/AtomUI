using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PaginationTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string NavPart = "PART_Nav";
    
    public PaginationTheme()
        : base(typeof(Pagination))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        var controlTemplate = new FuncControlTemplate<Pagination>((pagination, scope) =>
        {
            var rootLayout = new StackPanel()
            {
                Name        = RootLayoutPart,
                Orientation = Orientation.Horizontal
            };
            var nav = new PaginationNav()
            {
                Name = NavPart
            };
            nav.RegisterInNameScope(scope);
            CreateTemplateParentBinding(nav, PaginationNav.IsMotionEnabledProperty, Pagination.IsMotionEnabledProperty);
            CreateTemplateParentBinding(nav, PaginationNav.SizeTypeProperty, Pagination.SizeTypeProperty);
            rootLayout.Children.Add(nav);
            return rootLayout;
        });
        return controlTemplate;
    }

    protected override void BuildStyles()
    {
    }
}