using AtomUI.Icon;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal abstract class BaseButtonTheme : BaseControlTheme
{
    public const string LabelPart = "PART_Label";
    public const string MainInfoLayoutPart = "PART_MainInfoLayout";
    public const string RootLayoutPart = "PART_RootLayout";
    public const string LoadingIconPart = "PART_LoadingIcon";
    public const string ButtonIconPart = "PART_ButtonIcon";
    public const string RightExtraContentPart = "PART_RightExtraContent";

    public BaseButtonTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<Button>((button, scope) =>
        {
            var loadingIcon = new PathIcon
            {
                Kind = "LoadingOutlined",
                Name = LoadingIconPart
            };

            loadingIcon.RegisterInNameScope(scope);

            CreateTemplateParentBinding(loadingIcon, Layoutable.WidthProperty, Button.IconSizeProperty);
            CreateTemplateParentBinding(loadingIcon, Layoutable.HeightProperty, Button.IconSizeProperty);
            CreateTemplateParentBinding(loadingIcon, Layoutable.MarginProperty, Button.IconMarginProperty);

            var iconPresenter = new ContentPresenter
            {
                Name = ButtonIconPart
            };
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, Button.IconProperty);

            var label = new Label
            {
                Name                       = LabelPart,
                Padding                    = new Thickness(0),
                VerticalContentAlignment   = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalAlignment          = VerticalAlignment.Center
            };
            CreateTemplateParentBinding(label, ContentControl.ContentProperty, Button.TextProperty);
            label.RegisterInNameScope(scope);

            var mainInfoLayout = new StackPanel
            {
                Name                = MainInfoLayoutPart,
                VerticalAlignment   = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation         = Orientation.Horizontal
            };

            mainInfoLayout.RegisterInNameScope(scope);
            mainInfoLayout.Children.Add(loadingIcon);
            mainInfoLayout.Children.Add(iconPresenter);
            mainInfoLayout.Children.Add(label);

            var extraContentPresenter = new ContentPresenter
            {
                Name                       = RightExtraContentPart,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment   = VerticalAlignment.Center,
                HorizontalAlignment        = HorizontalAlignment.Center,
                VerticalAlignment          = VerticalAlignment.Center
            };
            CreateTemplateParentBinding(extraContentPresenter, Visual.IsVisibleProperty,
                Button.RightExtraContentProperty,
                BindingMode.Default,
                ObjectConverters.IsNotNull);
            CreateTemplateParentBinding(extraContentPresenter, ContentPresenter.ContentProperty,
                Button.RightExtraContentProperty);

            DockPanel.SetDock(extraContentPresenter, Dock.Right);

            var rootLayout = new DockPanel
            {
                Name          = RootLayoutPart,
                LastChildFill = true
            };

            rootLayout.Children.Add(extraContentPresenter);
            rootLayout.Children.Add(mainInfoLayout);

            var frameDecorator = new Border();

            CreateTemplateParentBinding(frameDecorator, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(frameDecorator, Border.BorderThicknessProperty,
                Button.EffectiveBorderThicknessProperty);
            CreateTemplateParentBinding(frameDecorator, Border.BorderBrushProperty,
                TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(frameDecorator, Border.BackgroundProperty, TemplatedControl.BackgroundProperty);
            CreateTemplateParentBinding(frameDecorator, Border.BackgroundSizingProperty,
                TemplatedControl.BackgroundSizingProperty);
            CreateTemplateParentBinding(frameDecorator, Border.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);

            frameDecorator.Child = rootLayout;

            return frameDecorator;
        });
    }

    protected override void BuildStyles()
    {
        this.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        this.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Bottom);
        this.Add(InputElement.CursorProperty, new Cursor(StandardCursorType.Hand));
        this.Add(Button.DefaultShadowProperty, ButtonTokenResourceKey.DefaultShadow);
        this.Add(Button.PrimaryShadowProperty, ButtonTokenResourceKey.PrimaryShadow);
        this.Add(Button.DangerShadowProperty, ButtonTokenResourceKey.DangerShadow);

        BuildSizeStyle();
        BuildIconSizeStyle();
        BuildLoadingStyle();
    }

    private void BuildSizeStyle()
    {
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalTokenResourceKey.ControlHeightLG);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, ButtonTokenResourceKey.ContentFontSizeLG);
        {
            var notCircleTypeStyle = new Style(selector => selector.Nesting().Not(nest =>
                nest.Nesting().PropertyEquals(Button.ButtonShapeProperty, ButtonShape.Circle)));
            notCircleTypeStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenResourceKey.PaddingLG);
            largeSizeStyle.Add(notCircleTypeStyle);
        }
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusLG);
        {
            var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
            iconOnlyStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenResourceKey.IconOnyPaddingLG);
            largeSizeStyle.Add(iconOnlyStyle);
        }
        {
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraContentPart));
            extraContentStyle.Add(Layoutable.MarginProperty, ButtonTokenResourceKey.ExtraContentMarginLG);
            largeSizeStyle.Add(extraContentStyle);
        }
        Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalTokenResourceKey.ControlHeight);
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, ButtonTokenResourceKey.ContentFontSize);
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
        {
            var notCircleTypeStyle = new Style(selector => selector.Nesting().Not(nest =>
                nest.Nesting().PropertyEquals(Button.ButtonShapeProperty, ButtonShape.Circle)));
            notCircleTypeStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenResourceKey.Padding);
            middleSizeStyle.Add(notCircleTypeStyle);
        }
        {
            var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
            iconOnlyStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenResourceKey.IconOnyPadding);
            middleSizeStyle.Add(iconOnlyStyle);
        }
        {
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraContentPart));
            extraContentStyle.Add(Layoutable.MarginProperty, ButtonTokenResourceKey.ExtraContentMargin);
            middleSizeStyle.Add(extraContentStyle);
        }
        Add(middleSizeStyle);

        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(Button.ControlHeightTokenProperty, GlobalTokenResourceKey.ControlHeightSM);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, ButtonTokenResourceKey.ContentFontSizeSM);
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadiusSM);
        {
            var notCircleTypeStyle = new Style(selector => selector.Nesting().Not(nest =>
                nest.Nesting().PropertyEquals(Button.ButtonShapeProperty, ButtonShape.Circle)));
            notCircleTypeStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenResourceKey.PaddingSM);
            smallSizeStyle.Add(notCircleTypeStyle);
        }
        {
            var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
            iconOnlyStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenResourceKey.IconOnyPaddingSM);
            smallSizeStyle.Add(iconOnlyStyle);
        }
        {
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraContentPart));
            extraContentStyle.Add(Layoutable.MarginProperty, ButtonTokenResourceKey.ExtraContentMarginSM);
            smallSizeStyle.Add(extraContentStyle);
        }
        Add(smallSizeStyle);
    }

    private void BuildIconSizeStyle()
    {
        // text 和 icon 都存在的情况
        {
            var largeSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
            largeSizeStyle.Add(Button.IconSizeProperty, GlobalTokenResourceKey.IconSizeLG);
            Add(largeSizeStyle);

            var middleSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
            middleSizeStyle.Add(Button.IconSizeProperty, GlobalTokenResourceKey.IconSize);
            Add(middleSizeStyle);

            var smallSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
            smallSizeStyle.Add(Button.IconSizeProperty, GlobalTokenResourceKey.IconSizeSM);
            Add(smallSizeStyle);
        }

        // icon only
        var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
        iconOnlyStyle.Add(Button.IconMarginProperty, new Thickness());
        {
            var largeSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
            largeSizeStyle.Add(Button.IconSizeProperty, ButtonTokenResourceKey.OnlyIconSizeLG);
            iconOnlyStyle.Add(largeSizeStyle);

            var middleSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
            middleSizeStyle.Add(Button.IconSizeProperty, ButtonTokenResourceKey.OnlyIconSize);
            iconOnlyStyle.Add(middleSizeStyle);

            var smallSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
            smallSizeStyle.Add(Button.IconSizeProperty, ButtonTokenResourceKey.OnlyIconSizeSM);
            iconOnlyStyle.Add(smallSizeStyle);
        }
        Add(iconOnlyStyle);

        var notIconOnyStyle = new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(Button.IconOnlyPC)));
        notIconOnyStyle.Add(Button.IconMarginProperty, ButtonTokenResourceKey.IconMargin);
        Add(notIconOnyStyle);
    }

    private void BuildLoadingStyle()
    {
        // 正常状态
        {
            var buttonIconStyle = new Style(selector => selector.Nesting().Template().Name(ButtonIconPart));
            buttonIconStyle.Add(Visual.IsVisibleProperty, true);
            Add(buttonIconStyle);

            var loadingIconStyle = new Style(selector => selector.Nesting().Template().Name(LoadingIconPart));
            loadingIconStyle.Add(Visual.IsVisibleProperty, false);
            Add(loadingIconStyle);
        }
        // loading 状态
        var loadingStyle = new Style(selector => selector.Nesting().Class(Button.LoadingPC));
        {
            var buttonIconStyle = new Style(selector => selector.Nesting().Template().Name(ButtonIconPart));
            buttonIconStyle.Add(Visual.IsVisibleProperty, false);
            loadingStyle.Add(buttonIconStyle);

            var loadingIconStyle = new Style(selector => selector.Nesting().Template().Name(LoadingIconPart));
            loadingIconStyle.Add(Visual.IsVisibleProperty, true);
            loadingIconStyle.Add(PathIcon.LoadingAnimationProperty, IconAnimation.Spin);
            loadingStyle.Add(loadingIconStyle);
        }
        loadingStyle.Add(Visual.OpacityProperty, GlobalTokenResourceKey.OpacityLoading);
        Add(loadingStyle);
    }
}