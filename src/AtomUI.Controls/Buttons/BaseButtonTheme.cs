using AtomUI.Controls.Internal;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
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
            var loadingIcon = AntDesignIconPackage.LoadingOutlined();
            loadingIcon.Name = LoadingIconPart;

            loadingIcon.RegisterInNameScope(scope);

            CreateTemplateParentBinding(loadingIcon, Layoutable.WidthProperty, Button.IconSizeProperty);
            CreateTemplateParentBinding(loadingIcon, Layoutable.HeightProperty, Button.IconSizeProperty);
            CreateTemplateParentBinding(loadingIcon, Layoutable.MarginProperty, Button.IconMarginProperty);

            var iconPresenter = new ContentPresenter
            {
                Name = ButtonIconPart
            };
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, Button.IconProperty);

            var labelText = new SingleLineText
            {
                Name              = LabelPart,
                VerticalAlignment = VerticalAlignment.Center
            };

            CreateTemplateParentBinding(labelText, SingleLineText.SizeTypeProperty, Button.SizeTypeProperty);
            CreateTemplateParentBinding(labelText, SingleLineText.TextProperty, Button.TextProperty);
            labelText.RegisterInNameScope(scope);

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
            mainInfoLayout.Children.Add(labelText);

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
        this.Add(Button.DefaultShadowProperty, ButtonTokenKey.DefaultShadow);
        this.Add(Button.PrimaryShadowProperty, ButtonTokenKey.PrimaryShadow);
        this.Add(Button.DangerShadowProperty, ButtonTokenKey.DangerShadow);

        BuildSizeStyle();
        BuildIconSizeStyle();
        BuildLoadingStyle();
    }

    private void BuildSizeStyle()
    {
        var largeSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
        largeSizeStyle.Add(Button.ControlHeightTokenProperty, SharedTokenKey.ControlHeightLG);
        largeSizeStyle.Add(TemplatedControl.FontSizeProperty, ButtonTokenKey.ContentFontSizeLG);
        {
            var notCircleTypeStyle = new Style(selector => selector.Nesting().Not(nest =>
                nest.Nesting().PropertyEquals(Button.ButtonShapeProperty, ButtonShape.Circle)));
            notCircleTypeStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenKey.PaddingLG);
            largeSizeStyle.Add(notCircleTypeStyle);
        }
        largeSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusLG);
        {
            var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
            iconOnlyStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenKey.IconOnyPaddingLG);
            largeSizeStyle.Add(iconOnlyStyle);
        }
        {
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraContentPart));
            extraContentStyle.Add(Layoutable.MarginProperty, ButtonTokenKey.ExtraContentMarginLG);
            largeSizeStyle.Add(extraContentStyle);
        }
        Add(largeSizeStyle);

        var middleSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
        middleSizeStyle.Add(Button.ControlHeightTokenProperty, SharedTokenKey.ControlHeight);
        middleSizeStyle.Add(TemplatedControl.FontSizeProperty, ButtonTokenKey.ContentFontSize);
        middleSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadius);
        {
            var notCircleTypeStyle = new Style(selector => selector.Nesting().Not(nest =>
                nest.Nesting().PropertyEquals(Button.ButtonShapeProperty, ButtonShape.Circle)));
            notCircleTypeStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenKey.Padding);
            middleSizeStyle.Add(notCircleTypeStyle);
        }
        {
            var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
            iconOnlyStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenKey.IconOnyPadding);
            middleSizeStyle.Add(iconOnlyStyle);
        }
        {
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraContentPart));
            extraContentStyle.Add(Layoutable.MarginProperty, ButtonTokenKey.ExtraContentMargin);
            middleSizeStyle.Add(extraContentStyle);
        }
        Add(middleSizeStyle);

        var smallSizeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
        smallSizeStyle.Add(Button.ControlHeightTokenProperty, SharedTokenKey.ControlHeightSM);
        smallSizeStyle.Add(TemplatedControl.FontSizeProperty, ButtonTokenKey.ContentFontSizeSM);
        smallSizeStyle.Add(TemplatedControl.CornerRadiusProperty, SharedTokenKey.BorderRadiusSM);
        {
            var notCircleTypeStyle = new Style(selector => selector.Nesting().Not(nest =>
                nest.Nesting().PropertyEquals(Button.ButtonShapeProperty, ButtonShape.Circle)));
            notCircleTypeStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenKey.PaddingSM);
            smallSizeStyle.Add(notCircleTypeStyle);
        }
        {
            var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
            iconOnlyStyle.Add(TemplatedControl.PaddingProperty, ButtonTokenKey.IconOnyPaddingSM);
            smallSizeStyle.Add(iconOnlyStyle);
        }
        {
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraContentPart));
            extraContentStyle.Add(Layoutable.MarginProperty, ButtonTokenKey.ExtraContentMarginSM);
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
            largeSizeStyle.Add(Button.IconSizeProperty, SharedTokenKey.IconSizeLG);
            Add(largeSizeStyle);

            var middleSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
            middleSizeStyle.Add(Button.IconSizeProperty, SharedTokenKey.IconSize);
            Add(middleSizeStyle);

            var smallSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
            smallSizeStyle.Add(Button.IconSizeProperty, SharedTokenKey.IconSizeSM);
            Add(smallSizeStyle);
        }

        // icon only
        var iconOnlyStyle = new Style(selector => selector.Nesting().Class(Button.IconOnlyPC));
        iconOnlyStyle.Add(Button.IconMarginProperty, new Thickness());
        {
            var largeSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Large));
            largeSizeStyle.Add(Button.IconSizeProperty, ButtonTokenKey.OnlyIconSizeLG);
            iconOnlyStyle.Add(largeSizeStyle);

            var middleSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Middle));
            middleSizeStyle.Add(Button.IconSizeProperty, ButtonTokenKey.OnlyIconSize);
            iconOnlyStyle.Add(middleSizeStyle);

            var smallSizeStyle = new Style(selector =>
                selector.Nesting().PropertyEquals(Button.SizeTypeProperty, SizeType.Small));
            smallSizeStyle.Add(Button.IconSizeProperty, ButtonTokenKey.OnlyIconSizeSM);
            iconOnlyStyle.Add(smallSizeStyle);
        }
        Add(iconOnlyStyle);

        var notIconOnyStyle = new Style(selector => selector.Nesting().Not(x => x.Nesting().Class(Button.IconOnlyPC)));
        notIconOnyStyle.Add(Button.IconMarginProperty, ButtonTokenKey.IconMargin);
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
            loadingIconStyle.Add(Icon.LoadingAnimationProperty, IconAnimation.Spin);
            loadingStyle.Add(loadingIconStyle);
        }
        loadingStyle.Add(Visual.OpacityProperty, SharedTokenKey.OpacityLoading);
        Add(loadingStyle);
    }
}