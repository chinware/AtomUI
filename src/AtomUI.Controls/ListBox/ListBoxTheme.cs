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
    public const string FramePart = "PART_Frame";
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
                Name = FramePart
            };
            CreateTemplateParentBinding(frameBorder, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(frameBorder, Border.BorderBrushProperty, TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(frameBorder, Border.BorderThicknessProperty, TemplatedControl.BorderThicknessProperty);
            CreateTemplateParentBinding(frameBorder, Border.CornerRadiusProperty, TemplatedControl.CornerRadiusProperty);

            var itemsPresenter = BuildItemsPresenter(listBox, scope);
            var scrollViewer   = BuildScrollViewer();

            scrollViewer.Content = itemsPresenter;
            frameBorder.Child    = scrollViewer;

            return frameBorder;
        });
    }

    private ScrollViewer BuildScrollViewer()
    {
        var scrollViewer = new ScrollViewer
        {
            Name = ScrollViewerPart
        };

        CreateTemplateParentBinding(scrollViewer, ScrollViewer.AllowAutoHideProperty, ScrollViewer.AllowAutoHideProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollViewer.HorizontalScrollBarVisibilityProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.VerticalScrollBarVisibilityProperty, ScrollViewer.VerticalScrollBarVisibilityProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.IsScrollChainingEnabledProperty, ScrollViewer.IsScrollChainingEnabledProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.IsDeferredScrollingEnabledProperty, ScrollViewer.IsDeferredScrollingEnabledProperty);
        CreateTemplateParentBinding(scrollViewer, ScrollViewer.IsScrollInertiaEnabledProperty, ScrollViewer.IsScrollInertiaEnabledProperty);

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
        largeStyle.Add(Border.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);

        Add(largeStyle);

        var middleStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ListBox.SizeTypeProperty, SizeType.Middle));
        middleStyle.Add(Border.CornerRadiusProperty, SharedTokenKey.BorderRadius);

        Add(middleStyle);

        var smallStyle =
            new Style(selector => selector.Nesting().PropertyEquals(ListBox.SizeTypeProperty, SizeType.Small));
        smallStyle.Add(Border.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
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
        commonStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorBorder);
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(Decorator.PaddingProperty, ListBoxTokenKey.ContentPadding);
        frameStyle.Add(Border.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        commonStyle.Add(frameStyle);
        Add(commonStyle);
    }
}