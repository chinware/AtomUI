using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class ListBoxTheme : BaseControlTheme
{
    public const string FrameDecoratorPart = "PART_FrameDecorator";
    public const string ScrollViewerPart = "PART_ScrollViewer";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public ListBoxTheme() : this(typeof(ListBox))
    {
    }

    protected ListBoxTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<ListBox>((listBox, scope) =>
        {
            var frameBorder = new Border
            {
                Name = FrameDecoratorPart
            };
            CreateTemplateParentBinding(frameBorder, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(frameBorder, Border.BorderBrushProperty, TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(frameBorder, Border.BorderThicknessProperty,
                TemplatedControl.BorderThicknessProperty);
            CreateTemplateParentBinding(frameBorder, Border.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);

            var itemsPresenter = BuildItemsPresenter(listBox, scope);
            var scrollViewer   = BuildScrollViewer(listBox, scope);

            scrollViewer.Content = itemsPresenter;
            frameBorder.Child    = scrollViewer;

            return frameBorder;
        });
    }

    private ScrollViewer BuildScrollViewer(ListBox listBox, INameScope scope)
    {
        var scrollViewer = new ScrollViewer
        {
            Name = ScrollViewerPart
        };
        BindUtils.RelayBind(listBox, ScrollViewer.AllowAutoHideProperty, scrollViewer,
            ScrollViewer.AllowAutoHideProperty);
        BindUtils.RelayBind(listBox, ScrollViewer.HorizontalScrollBarVisibilityProperty, scrollViewer,
            ScrollViewer.HorizontalScrollBarVisibilityProperty);
        BindUtils.RelayBind(listBox, ScrollViewer.VerticalScrollBarVisibilityProperty, scrollViewer,
            ScrollViewer.VerticalScrollBarVisibilityProperty);
        BindUtils.RelayBind(listBox, ScrollViewer.IsScrollChainingEnabledProperty, scrollViewer,
            ScrollViewer.IsScrollChainingEnabledProperty);
        BindUtils.RelayBind(listBox, ScrollViewer.IsDeferredScrollingEnabledProperty, scrollViewer,
            ScrollViewer.IsDeferredScrollingEnabledProperty);
        BindUtils.RelayBind(listBox, ScrollViewer.IsScrollInertiaEnabledProperty, scrollViewer,
            ScrollViewer.IsScrollInertiaEnabledProperty);

        return scrollViewer;
    }

    private ItemsPresenter BuildItemsPresenter(ListBox listBox, INameScope scope)
    {
        var itemsPresenter = new ItemsPresenter
        {
            Name = ItemsPresenterPart
        };
        itemsPresenter.RegisterInNameScope(scope);
        CreateTemplateParentBinding(itemsPresenter, ItemsPresenter.ItemsPanelProperty, ItemsControl.ItemsPanelProperty);
        return itemsPresenter;
    }

    protected override void BuildStyles()
    {
        BuildFixedStyle();
        BuildCommonStyle();
        BuildSizeTypeStyle();
    }

    private void BuildSizeTypeStyle()
    {
        var largeStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ListBox.SizeTypeProperty, SizeType.Large));
        largeStyle.Add(Border.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);

        Add(largeStyle);

        var middleStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ListBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(Border.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);

        Add(middleStyle);

        var smallStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ListBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(Border.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        Add(smallStyle);
    }

    private void BuildFixedStyle()
    {
        this.Add(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled);
        this.Add(ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Auto);
        this.Add(ScrollViewer.IsScrollChainingEnabledProperty, true);
    }

    private void BuildCommonStyle()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FrameDecoratorPart));
        frameStyle.Add(Decorator.PaddingProperty, ListBoxTokenResourceKey.ContentPadding);
        frameStyle.Add(Border.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
        commonStyle.Add(frameStyle);
        Add(commonStyle);
    }
}