using AtomUI.Controls.Utils;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
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
    public const string FramePart = "PART_Frame";
    public const string LabelPart = "PART_Label";
    public const string MainInfoLayoutPart = "PART_MainInfoLayout";
    public const string RootLayoutPart = "PART_RootLayout";
    public const string LoadingIconPart = "PART_LoadingIcon";
    public const string ButtonIconPart = "PART_ButtonIcon";
    public const string RightExtraContentPart = "PART_RightExtraContent";
    public const string RightExtraLayoutPart = "PART_RightExtraLayout";

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
            CreateTemplateParentBinding(loadingIcon, Icon.NormalFilledBrushProperty, Button.IconNormalColorProperty);
            CreateTemplateParentBinding(loadingIcon, Icon.ActiveFilledBrushProperty, Button.IconHoverColorProperty);
            CreateTemplateParentBinding(loadingIcon, Icon.SelectedFilledBrushProperty, Button.IconPressedColorProperty);
            CreateTemplateParentBinding(loadingIcon, Icon.DisabledFilledBrushProperty, Button.IconDisabledColorProperty);
            
            var iconPresenter = new ContentPresenter
            {
                Name = ButtonIconPart
            };
            CreateTemplateParentBinding(iconPresenter, ContentPresenter.ContentProperty, Button.IconProperty);

            var labelText = new TextBlock()
            {
                Name              = LabelPart,
                VerticalAlignment = VerticalAlignment.Center
            };
            
            CreateTemplateParentBinding(labelText, TextBlock.TextProperty, Button.TextProperty);
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

            var extraContentLayout = new StackPanel()
            {
                Name        = RightExtraLayoutPart,
                Orientation = Orientation.Horizontal,
            };
            CreateTemplateParentBinding(extraContentLayout, StackPanel.IsVisibleProperty, Button.ExtraContainerVisibleProperty);
            BuildExtraContainer(extraContentLayout, scope);
            DockPanel.SetDock(extraContentLayout, Dock.Right);

            var rootLayout = new DockPanel
            {
                Name          = RootLayoutPart,
                LastChildFill = true
            };

            rootLayout.Children.Add(extraContentLayout);
            rootLayout.Children.Add(mainInfoLayout);

            var frame = new Border()
            {
                Name = FramePart
            };
            frame.RegisterInNameScope(scope);
            CreateTemplateParentBinding(frame, Decorator.PaddingProperty, TemplatedControl.PaddingProperty);
            CreateTemplateParentBinding(frame, Border.BorderThicknessProperty,
                Button.EffectiveBorderThicknessProperty);
            CreateTemplateParentBinding(frame, Border.BorderBrushProperty,
                TemplatedControl.BorderBrushProperty);
            CreateTemplateParentBinding(frame, Border.BackgroundSizingProperty,
                TemplatedControl.BackgroundSizingProperty);
            CreateTemplateParentBinding(frame, Border.CornerRadiusProperty,
                TemplatedControl.CornerRadiusProperty);

            frame.Child = rootLayout;

            return frame;
        });
    }

    protected virtual void BuildExtraContainer(StackPanel containerLayout, INameScope scope)
    {
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

        containerLayout.Children.Add(extraContentPresenter);
    }

    protected override void BuildStyles()
    {
        this.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        this.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Top);
        this.Add(InputElement.CursorProperty, new SetterValueFactory<Cursor>(() => new Cursor(StandardCursorType.Hand)));
        this.Add(Button.DefaultShadowProperty, ButtonTokenKey.DefaultShadow);
        this.Add(Button.PrimaryShadowProperty, ButtonTokenKey.PrimaryShadow);
        this.Add(Button.DangerShadowProperty, ButtonTokenKey.DangerShadow);

        var extraLayoutStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraLayoutPart));
        extraLayoutStyle.Add(StackPanel.SpacingProperty, ButtonTokenKey.ExtraContentItemSpacing);
        Add(extraLayoutStyle);

        BuildSizeStyle();
        BuildIconStyle();
        BuildLoadingStyle();
        BuildShapeStyle();
        
        // 动画设置
        var isMotionEnabledStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.IsMotionEnabledProperty, true));
        var frameStyle = new Style(selector => selector.Nesting().Template().Name(FramePart));
        frameStyle.Add(IconButton.TransitionsProperty, new SetterValueFactory<Transitions>(() => new Transitions()
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(TemplatedControl.BackgroundProperty)
        }));
        isMotionEnabledStyle.Add(frameStyle);
        Add(isMotionEnabledStyle);
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
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraLayoutPart));
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
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraLayoutPart));
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
            var extraContentStyle = new Style(selector => selector.Nesting().Template().Name(RightExtraLayoutPart));
            extraContentStyle.Add(Layoutable.MarginProperty, ButtonTokenKey.ExtraContentMarginSM);
            smallSizeStyle.Add(extraContentStyle);
        }
        Add(smallSizeStyle);
    }

    private void BuildIconStyle()
    {
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ButtonIconPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Normal);
            Add(iconStyle);
        }
        var hoverStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.PointerOver));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ButtonIconPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Active);
            hoverStyle.Add(iconStyle);
        }
        Add(hoverStyle);
        var pressedStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Pressed));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ButtonIconPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Selected);
            pressedStyle.Add(iconStyle);
        }
        Add(pressedStyle);
        var disabledStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        {
            var iconStyle = new Style(selector => selector.Nesting().Template().Name(ButtonIconPart).Descendant().OfType<Icon>());
            iconStyle.Add(Icon.IconModeProperty, IconMode.Disabled);
            disabledStyle.Add(iconStyle);
        }
        Add(disabledStyle);
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

    private void BuildShapeStyle()
    {
        var circleStyle = new Style(selector => selector.Nesting().PropertyEquals(Button.ButtonShapeProperty, ButtonShape.Circle));
        circleStyle.Add(Button.PaddingProperty, ButtonTokenKey.CirclePadding);
        Add(circleStyle);
    }
}