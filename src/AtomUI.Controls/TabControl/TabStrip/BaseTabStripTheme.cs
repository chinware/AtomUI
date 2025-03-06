using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class BaseTabStripTheme : BaseControlTheme
{
    public const string FramePart = "PART_Frame";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string TabsContainerPart = "PART_TabsContainer";
    public const string AlignWrapperPart = "PART_AlignWrapper";

    public BaseTabStripTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<BaseTabStrip>((strip, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart
            };
            frame.RegisterInNameScope(scope);
            NotifyBuildControlTemplate(strip, scope, frame);
            return frame;
        });
    }

    protected virtual void NotifyBuildControlTemplate(BaseTabStrip baseTabStrip, INameScope scope, Border container)
    {
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorderSecondary);

        // 设置 items presenter 是否居中
        // 分为上、右、下、左
        {
            // 上
            var topStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.TopPC));
            topStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            topStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);

            // tabs 是否居中
            var tabAlignCenterStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(BaseTabStrip.TabAlignmentCenterProperty, true));
            {
                var tabsContainerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
                tabsContainerStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                tabAlignCenterStyle.Add(tabsContainerStyle);
            }
            topStyle.Add(tabAlignCenterStyle);

            commonStyle.Add(topStyle);
        }

        {
            // 右
            var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.RightPC));

            rightStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            rightStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);

            // tabs 是否居中
            var tabAlignCenterStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(BaseTabStrip.TabAlignmentCenterProperty, true));
            {
                var tabsContainerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
                tabsContainerStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
                tabAlignCenterStyle.Add(tabsContainerStyle);
            }
            rightStyle.Add(tabAlignCenterStyle);

            commonStyle.Add(rightStyle);
        }
        {
            // 下
            var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.BottomPC));
            bottomStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            bottomStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);

            // tabs 是否居中
            var tabAlignCenterStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(BaseTabStrip.TabAlignmentCenterProperty, true));
            {
                var tabsContainerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
                tabsContainerStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                tabAlignCenterStyle.Add(tabsContainerStyle);
            }
            bottomStyle.Add(tabAlignCenterStyle);

            commonStyle.Add(bottomStyle);
        }
        {
            // 左
            var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabStrip.LeftPC));

            leftStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            leftStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);

            // tabs 是否居中
            var tabAlignCenterStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(BaseTabStrip.TabAlignmentCenterProperty, true));
            {
                var tabsContainerStyle = new Style(selector => selector.Nesting().Template().Name(TabsContainerPart));
                tabsContainerStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
                tabAlignCenterStyle.Add(tabsContainerStyle);
            }
            leftStyle.Add(tabAlignCenterStyle);

            commonStyle.Add(leftStyle);
        }

        Add(commonStyle);
    }
}