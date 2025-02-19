using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class SegmentedTheme : BaseControlTheme
{
    public const string FrameDecoratorPart = "PART_FrameDecorator";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";

    public SegmentedTheme() : base(typeof(Segmented))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Segmented>((segmented, scope) =>
        {
            var frameDecorator = new Border
            {
                Name         = FrameDecoratorPart,
                ClipToBounds = true
            };
            // TODO 需要观察是否会有内存泄漏
            var itemsPresenter = new ItemsPresenter
            {
                Name = ItemsPresenterPart,
                ItemsPanel = new FuncTemplate<Panel?>(() =>
                {
                    var panel = new SegmentedStackPanel();
                    BindUtils.RelayBind(segmented, Segmented.IsExpandingProperty, panel, SegmentedStackPanel.IsExpandingProperty);
                    return panel;
                })
            };
            itemsPresenter.RegisterInNameScope(scope);
            frameDecorator.Child = itemsPresenter;
            
            CreateTemplateParentBinding(frameDecorator, Border.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            CreateTemplateParentBinding(frameDecorator, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
            return frameDecorator;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.PaddingProperty, SegmentedTokenKey.TrackPadding);
        commonStyle.Add(TemplatedControl.BackgroundProperty, SegmentedTokenKey.TrackBg);
        commonStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        commonStyle.Add(Segmented.SelectedThumbBgProperty, SegmentedTokenKey.ItemSelectedBg);
        commonStyle.Add(Segmented.SelectedThumbBoxShadowsProperty, SharedTokenKey.BoxShadowsTertiary);
        var expandingStyle =
            new Style(selector => selector.Nesting().PropertyEquals(Segmented.IsExpandingProperty, true));
        expandingStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
        commonStyle.Add(expandingStyle);
        Add(commonStyle);

        BuildSizeTypeStyle();
    }

    private void BuildSizeTypeStyle()
    {
        var largeSizeTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Large));
        largeSizeTypeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
        largeSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, SharedTokenKey.BorderRadius);
        Add(largeSizeTypeStyle);

        var middleSizeTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Middle));
        middleSizeTypeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        middleSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        Add(middleSizeTypeStyle);

        var smallSizeTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Segmented.SizeTypeProperty, SizeType.Small));
        smallSizeTypeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        smallSizeTypeStyle.Add(Segmented.SelectedThumbCornerRadiusProperty, SharedTokenKey.BorderRadiusXS);
        Add(smallSizeTypeStyle);
    }
}