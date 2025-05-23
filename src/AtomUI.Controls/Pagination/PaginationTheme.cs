using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PaginationTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string NavPart = "PART_Nav";
    public const string ShowSizeChangerPresenterPart = "PART_ShowSizeChangerPresenter";
    
    public PaginationTheme()
        : base(typeof(Pagination))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        var controlTemplate = new FuncControlTemplate<Pagination>((pagination, scope) =>
        {
            var rootLayout = new StackPanel
            {
                Name        = RootLayoutPart,
                Orientation = Orientation.Horizontal
            };
            var nav = new PaginationNav
            {
                Name = NavPart
            };
            nav.RegisterInNameScope(scope);
            CreateTemplateParentBinding(nav, PaginationNav.IsMotionEnabledProperty, Pagination.IsMotionEnabledProperty);
            CreateTemplateParentBinding(nav, PaginationNav.SizeTypeProperty, Pagination.SizeTypeProperty);
            rootLayout.Children.Add(nav);

            var showSizeChangerPresenter = new ContentPresenter()
            {
                Name        = ShowSizeChangerPresenterPart
            };
            CreateTemplateParentBinding(showSizeChangerPresenter, ContentPresenter.IsVisibleProperty, Pagination.ShowSizeChangerProperty);
            CreateTemplateParentBinding(showSizeChangerPresenter, ContentPresenter.ContentProperty, Pagination.SizeChangerProperty);
            rootLayout.Children.Add(showSizeChangerPresenter);
            return rootLayout;
        });
        return controlTemplate;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        {
            var alignStyle = new Style(selector => selector.Nesting().PropertyEquals(Pagination.AlignProperty, PaginationAlign.Start));
            alignStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            commonStyle.Add(alignStyle);
        }
        {
            var alignStyle = new Style(selector => selector.Nesting().PropertyEquals(Pagination.AlignProperty, PaginationAlign.Center));
            alignStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            commonStyle.Add(alignStyle);
        }
        {
            var alignStyle = new Style(selector => selector.Nesting().PropertyEquals(Pagination.AlignProperty, PaginationAlign.End));
            alignStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            commonStyle.Add(alignStyle);
        }
        Add(commonStyle);
    }
}