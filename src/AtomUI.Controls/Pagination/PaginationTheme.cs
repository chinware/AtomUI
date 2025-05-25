using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class PaginationTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string NavPart = "PART_Nav";
    public const string SizeChangerPresenterPart = "PART_SizeChangerPresenter";
    public const string QuickJumperBarPresenterPart = "PART_QuickJumperBarPresenter";
    public const string TotalInfoPresenterPart = "PART_TotalInfoPresenter";
    
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
            var showTotalPresenter = new ContentPresenter()
            {
                Name = TotalInfoPresenterPart
            };
            CreateTemplateParentBinding(showTotalPresenter, ContentPresenter.IsVisibleProperty, Pagination.ShowTotalInfoProperty);
            CreateTemplateParentBinding(showTotalPresenter, ContentPresenter.ContentProperty,
                Pagination.TotalInfoTextProperty,
                BindingMode.Default,
                new FuncValueConverter<object?, object?>(
                    o =>
                    {
                        if (o is string str)
                        {
                            return new TextBlock()
                            {
                                Text              = str,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                        }
                        return o;
                    }));
            rootLayout.Children.Add(showTotalPresenter);
            
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
                Name        = SizeChangerPresenterPart
            };
            CreateTemplateParentBinding(showSizeChangerPresenter, ContentPresenter.IsVisibleProperty, Pagination.ShowSizeChangerProperty);
            CreateTemplateParentBinding(showSizeChangerPresenter, ContentPresenter.ContentProperty, Pagination.SizeChangerProperty);
            rootLayout.Children.Add(showSizeChangerPresenter);
            
            var quickJumperBarPresenter = new ContentPresenter()
            {
                Name        = QuickJumperBarPresenterPart
            };
            CreateTemplateParentBinding(quickJumperBarPresenter, ContentPresenter.IsVisibleProperty, Pagination.ShowQuickJumperProperty);
            CreateTemplateParentBinding(quickJumperBarPresenter, ContentPresenter.ContentProperty, Pagination.QuickJumperBarProperty);
            rootLayout.Children.Add(quickJumperBarPresenter);
            
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
        
        
        var largeSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(PaginationNavItem.SizeTypeProperty, SizeType.Large));
        {
            var layoutStyle = new Style(selector => selector.Nesting().Template().Name(RootLayoutPart));
            layoutStyle.Add(StackPanel.SpacingProperty, PaginationTokenKey.PaginationLayoutSpacing);
            largeSizeStyle.Add(layoutStyle);
        }
        
        commonStyle.Add(largeSizeStyle);

        var middleSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(PaginationNavItem.SizeTypeProperty, SizeType.Middle));
        {
            var layoutStyle = new Style(selector => selector.Nesting().Template().Name(RootLayoutPart));
            layoutStyle.Add(StackPanel.SpacingProperty, PaginationTokenKey.PaginationLayoutSpacing);
            middleSizeStyle.Add(layoutStyle);
        }
        commonStyle.Add(middleSizeStyle);

        var smallSizeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(PaginationNavItem.SizeTypeProperty, SizeType.Small));
        {
            var layoutStyle = new Style(selector => selector.Nesting().Template().Name(RootLayoutPart));
            layoutStyle.Add(StackPanel.SpacingProperty, PaginationTokenKey.PaginationLayoutMiniSpacing);
            smallSizeStyle.Add(layoutStyle);
        }
        commonStyle.Add(smallSizeStyle);
        
        Add(commonStyle);
    }
}