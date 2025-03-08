using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class GroupBoxTheme : BaseControlTheme
{
    public const string HeaderPresenterPart = "PART_HeaderPresenter";
    public const string HeaderContainerPart = "PART_HeaderContainer";
    public const string HeaderContentPart = "PART_HeaderContentLayout";
    public const string ContentPresenterPart = "PART_ContentPresenter";
    public const string HeaderIconPresenterPart = "PART_HeaderIconPresenter";
    public const string FramePart = "PART_Frame";

    public GroupBoxTheme()
        : base(typeof(GroupBox))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<GroupBox>((groupBox, scope) =>
        {
            var frame = new Border
            {
                Name = FramePart
            };
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);
            frame.RegisterInNameScope(scope);
            var mainLayout = new DockPanel
            {
                LastChildFill = true
            };

            var headerContainer = new Panel
            {
                Name = HeaderContainerPart
            };
            headerContainer.RegisterInNameScope(scope);
            DockPanel.SetDock(headerContainer, Dock.Top);

            var headerContentContainer = new Decorator
            {
                Name              = HeaderContentPart,
                VerticalAlignment = VerticalAlignment.Center
            };

            var headerContentLayout = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            headerContentContainer.Child = headerContentLayout;
            headerContentContainer.RegisterInNameScope(scope);
            headerContainer.Children.Add(headerContentContainer);

            var headerIconContentPresenter = new ContentPresenter
            {
                Name = HeaderIconPresenterPart
            };
            CreateTemplateParentBinding(headerIconContentPresenter, Visual.IsVisibleProperty,
                GroupBox.HeaderIconProperty,
                BindingMode.Default,
                ObjectConverters.IsNotNull);
            CreateTemplateParentBinding(headerIconContentPresenter, ContentPresenter.ContentProperty,
                GroupBox.HeaderIconProperty);
            headerContentLayout.Children.Add(headerIconContentPresenter);

            var headerText = new TextBlock()
            {
                Name                = HeaderPresenterPart,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment   = VerticalAlignment.Center
            };

            CreateTemplateParentBinding(headerText, TextBlock.TextProperty, GroupBox.HeaderTitleProperty);
            CreateTemplateParentBinding(headerText, TextBlock.FontSizeProperty, GroupBox.HeaderFontSizeProperty);
            CreateTemplateParentBinding(headerText, TextBlock.ForegroundProperty,
                GroupBox.HeaderTitleColorProperty);
            CreateTemplateParentBinding(headerText, TextBlock.FontStyleProperty, GroupBox.HeaderFontStyleProperty);
            CreateTemplateParentBinding(headerText, TextBlock.FontWeightProperty,
                GroupBox.HeaderFontWeightProperty);

            headerContentLayout.Children.Add(headerText);

            mainLayout.Children.Add(headerContainer);

            var contentPresenter = new ContentPresenter
            {
                Name = ContentPresenterPart
            };
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentProperty,
                ContentControl.ContentProperty);
            CreateTemplateParentBinding(contentPresenter, ContentPresenter.ContentTemplateProperty,
                ContentControl.ContentTemplateProperty);

            mainLayout.Children.Add(contentPresenter);
            frame.Child = mainLayout;
            return frame;
        });
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(TemplatedControl.BackgroundProperty, SharedTokenKey.ColorBgContainer);
        commonStyle.Add(TemplatedControl.BorderBrushProperty, SharedTokenKey.ColorBorder);
        commonStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        commonStyle.Add(GroupBox.HeaderTitleColorProperty, SharedTokenKey.ColorText);
        commonStyle.Add(GroupBox.HeaderFontSizeProperty, SharedTokenKey.FontSize);

        var headerContainerStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContainerPart));
        headerContainerStyle.Add(Layoutable.MarginProperty, GroupBoxTokenKey.HeaderContainerMargin);
        commonStyle.Add(headerContainerStyle);

        var headerContentStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContentPart));
        headerContentStyle.Add(ContentPresenter.PaddingProperty, GroupBoxTokenKey.HeaderContentPadding);
        commonStyle.Add(headerContentStyle);

        var headerIconPresenterStyle = new Style(selector => selector.Nesting().Template().Name(HeaderIconPresenterPart));
        headerIconPresenterStyle.Add(Layoutable.MarginProperty, GroupBoxTokenKey.HeaderIconMargin);
        commonStyle.Add(headerIconPresenterStyle);

        var headerIcon = new Style(selector => selector.Nesting().Template().Name(HeaderIconPresenterPart).Descendant().OfType<Icon>());
        headerIcon.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorText);
        headerIcon.Add(Layoutable.WidthProperty, SharedTokenKey.IconSizeLG);
        headerIcon.Add(Layoutable.HeightProperty, SharedTokenKey.IconSizeLG);
        headerIcon.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        commonStyle.Add(headerIcon);

        var contentStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentStyle.Add(ContentPresenter.PaddingProperty, GroupBoxTokenKey.ContentPadding);
        commonStyle.Add(contentStyle);
        
        Add(commonStyle);

        BuildHeaderPositionStyle();
    }

    private void BuildHeaderPositionStyle()
    {
        var leftStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(GroupBox.HeaderTitlePositionProperty, GroupBoxTitlePosition.Left));
        {
            var contentStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContentPart));
            contentStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
            leftStyle.Add(contentStyle);
        }
        Add(leftStyle);

        var centerStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(GroupBox.HeaderTitlePositionProperty, GroupBoxTitlePosition.Center));
        {
            var contentStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContentPart));
            contentStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            centerStyle.Add(contentStyle);
        }
        Add(centerStyle);

        var rightStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(GroupBox.HeaderTitlePositionProperty, GroupBoxTitlePosition.Right));
        {
            var contentStyle = new Style(selector => selector.Nesting().Template().Name(HeaderContentPart));
            contentStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Right);
            rightStyle.Add(contentStyle);
        }
        Add(rightStyle);
    }
}