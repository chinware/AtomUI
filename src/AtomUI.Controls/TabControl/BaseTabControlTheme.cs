using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class BaseTabControlTheme : BaseControlTheme
{
    public const string FrameDecoratorPart = "PART_FrameDecorator";
    public const string ItemsPresenterPart = "PART_ItemsPresenter";
    public const string MainLayoutContainerPart = "PART_MainLayoutContainer";
    public const string SelectedContentHostPart = "PART_SelectedContentHost";
    public const string TabsContainerPart = "PART_TabsContainer";
    public const string AlignWrapperPart = "PART_AlignWrapper";

    public BaseTabControlTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<BaseTabControl>((baseTabControl, scope) =>
        {
            var frameDecorator = new Border
            {
                Name = FrameDecoratorPart
            };
            frameDecorator.RegisterInNameScope(scope);
            var layoutContainer = new DockPanel
            {
                Name = MainLayoutContainerPart
            };

            NotifyBuildTabStripTemplate(baseTabControl, scope, layoutContainer);
            NotifyBuildContentPresenter(baseTabControl, scope, layoutContainer);
            frameDecorator.Child = layoutContainer;
            return frameDecorator;
        });
    }

    protected virtual void NotifyBuildTabStripTemplate(BaseTabControl baseTabControl, INameScope scope,
        DockPanel container)
    {
    }

    protected virtual void NotifyBuildContentPresenter(BaseTabControl baseTabControl, INameScope scope,
        DockPanel container)
    {
        var contentPresenter = new ContentPresenter
        {
            Name = SelectedContentHostPart
        };
        contentPresenter.RegisterInNameScope(scope);
        CreateTemplateParentBinding(contentPresenter, Layoutable.MarginProperty, TemplatedControl.PaddingProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
            Avalonia.Controls.TabControl.SelectedContentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
            Avalonia.Controls.TabControl.SelectedContentTemplateProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.HorizontalContentAlignmentProperty,
            Avalonia.Controls.TabControl.HorizontalContentAlignmentProperty);
        CreateTemplateParentBinding(contentPresenter, ContentPresenter.VerticalContentAlignmentProperty,
            Avalonia.Controls.TabControl.VerticalContentAlignmentProperty);
        container.Children.Add(contentPresenter);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.BorderBrushProperty, GlobalTokenResourceKey.ColorBorderSecondary);

        // 设置 items presenter 是否居中
        // 分为上、右、下、左
        {
            // 上
            var topStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.TopPC));
            topStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            topStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);

            // tabs 是否居中
            var tabAlignCenterStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(BaseTabControl.TabAlignmentCenterProperty, true));
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
            var rightStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.RightPC));

            rightStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            rightStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);

            // tabs 是否居中
            var tabAlignCenterStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(BaseTabControl.TabAlignmentCenterProperty, true));
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
            var bottomStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.BottomPC));
            bottomStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            bottomStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);

            // tabs 是否居中
            var tabAlignCenterStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(BaseTabControl.TabAlignmentCenterProperty, true));
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
            var leftStyle = new Style(selector => selector.Nesting().Class(BaseTabControl.LeftPC));


            leftStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            leftStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Stretch);

            // tabs 是否居中
            var tabAlignCenterStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(BaseTabControl.TabAlignmentCenterProperty, true));
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