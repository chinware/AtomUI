using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SimplePaginationTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayoutPart";
    public const string PreviousNavItemPart = "PART_PreviousNavItem";
    public const string NextNavItemPart = "PART_NextNavItem";
    public const string InfoIndicatorPart = "PART_InfoIndicator";
    public const string QuickJumperPart = "PART_QuickJumper";

    public SimplePaginationTheme()
        : base(typeof(SimplePagination))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<SimplePagination>((simplePagination, scope) =>
        {
            var mainLayout = new StackPanel()
            {
                Name = RootLayoutPart,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
            };

            var previousNavItem = new PaginationNavItem()
            {
                Name = PreviousNavItemPart,
                PaginationItemType = PaginationItemType.Previous,
                Content  = AntDesignIconPackage.LeftOutlined()
            };
            CreateTemplateParentBinding(previousNavItem, PaginationNavItem.SizeTypeProperty, SimplePagination.SizeTypeProperty);
            CreateTemplateParentBinding(previousNavItem, PaginationNavItem.IsEnabledProperty, SimplePagination.IsEnabledProperty);
            previousNavItem.RegisterInNameScope(scope);
            mainLayout.Children.Add(previousNavItem);

            var quickJumperPart = new LineEdit()
            {
                Name = QuickJumperPart
            };
            CreateTemplateParentBinding(quickJumperPart,
                LineEdit.IsVisibleProperty, 
                SimplePagination.IsReadOnlyProperty,
                BindingMode.Default,
                BoolConverters.Not);
            CreateTemplateParentBinding(quickJumperPart, LineEdit.SizeTypeProperty, SimplePagination.SizeTypeProperty);
            CreateTemplateParentBinding(quickJumperPart, LineEdit.IsEnabledProperty, SimplePagination.IsEnabledProperty);
            quickJumperPart.RegisterInNameScope(scope);
            mainLayout.Children.Add(quickJumperPart);
            
            var infoIndicator = new TextBlock()
            {
                Name = InfoIndicatorPart,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            infoIndicator.RegisterInNameScope(scope);
            CreateTemplateParentBinding(infoIndicator, TextBlock.IsEnabledProperty, SimplePagination.IsEnabledProperty);
            mainLayout.Children.Add(infoIndicator);
            
            var nextNavItem = new PaginationNavItem()
            {
                Name               = NextNavItemPart,
                PaginationItemType = PaginationItemType.Next,
                Content            = AntDesignIconPackage.RightOutlined()
            };
            CreateTemplateParentBinding(nextNavItem, PaginationNavItem.IsEnabledProperty, SimplePagination.IsEnabledProperty);
            CreateTemplateParentBinding(nextNavItem, PaginationNavItem.SizeTypeProperty, SimplePagination.SizeTypeProperty);
            nextNavItem.RegisterInNameScope(scope);
            mainLayout.Children.Add(nextNavItem);
            return mainLayout;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        
        {
            var alignStyle = new Style(selector => selector.Nesting().PropertyEquals(SimplePagination.AlignProperty, PaginationAlign.Start));
            alignStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            commonStyle.Add(alignStyle);
        }
        {
            var alignStyle = new Style(selector => selector.Nesting().PropertyEquals(SimplePagination.AlignProperty, PaginationAlign.Center));
            alignStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            commonStyle.Add(alignStyle);
        }
        {
            var alignStyle = new Style(selector => selector.Nesting().PropertyEquals(SimplePagination.AlignProperty, PaginationAlign.End));
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
        
        var quickJumperStyle = new Style(selector => selector.Nesting().Template().Name(QuickJumperPart));
        quickJumperStyle.Add(LineEdit.MinWidthProperty, PaginationTokenKey.PaginationQuickJumperInputWidth);
        commonStyle.Add(quickJumperStyle);
        
        Add(commonStyle);
    }
}