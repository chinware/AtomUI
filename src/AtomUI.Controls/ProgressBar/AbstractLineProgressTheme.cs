using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class AbstractLineProgressTheme : AbstractProgressBarTheme
{
    public AbstractLineProgressTheme(Type targetType) : base(targetType)
    {
    }

    protected override void NotifyBuildControlTemplate(AbstractProgressBar bar, INameScope scope, Canvas container)
    {
        base.NotifyBuildControlTemplate(bar, scope, container);
        CreateCompletedIcons(bar, scope, container);
    }

    private void CreateCompletedIcons(AbstractProgressBar progressBar, INameScope scope, Canvas container)
    {
        var exceptionCompletedIcon = AntDesignIconPackage.CloseCircleFilled();
        exceptionCompletedIcon.Name                = ExceptionCompletedIconPart;
        exceptionCompletedIcon.HorizontalAlignment = HorizontalAlignment.Left;

        exceptionCompletedIcon.RegisterInNameScope(scope);

        var successCompletedIcon = AntDesignIconPackage.CheckCircleFilled();
        successCompletedIcon.Name                = SuccessCompletedIconPart;
        successCompletedIcon.HorizontalAlignment = HorizontalAlignment.Left;

        successCompletedIcon.RegisterInNameScope(scope);

        container.Children.Add(exceptionCompletedIcon);
        container.Children.Add(successCompletedIcon);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(AbstractLineProgress.LineProgressPaddingProperty, ProgressBarTokenKey.LineProgressPadding);
        commonStyle.Add(AbstractLineProgress.LineExtraInfoMarginProperty, ProgressBarTokenKey.LineExtraInfoMargin);
        Add(commonStyle);
        
        // 完成图标样式
        var exceptionCompletedIconStyle = new Style(selector => selector.Nesting().Template().Name(ExceptionCompletedIconPart));
        exceptionCompletedIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorError);
        exceptionCompletedIconStyle.Add(Icon.DisabledFilledBrushProperty, SharedTokenKey.ControlItemBgActiveDisabled);
        Add(exceptionCompletedIconStyle);
        
        var successCompletedIconStyle = new Style(selector => selector.Nesting().Template().Name(SuccessCompletedIconPart));
        successCompletedIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorSuccess);
        successCompletedIconStyle.Add(Icon.DisabledFilledBrushProperty, SharedTokenKey.ControlItemBgActiveDisabled);
        Add(successCompletedIconStyle);
        
        BuildSizeTypeStyle();
    }

    private void BuildSizeTypeStyle()
    {
        var largeSizeTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Large));
        largeSizeTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty, ProgressBarTokenKey.LineInfoIconSize);
        // icon
        {
            var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            completedIconsStyle.Add(Layoutable.WidthProperty, ProgressBarTokenKey.LineInfoIconSize);
            completedIconsStyle.Add(Layoutable.HeightProperty, ProgressBarTokenKey.LineInfoIconSize);
            largeSizeTypeStyle.Add(completedIconsStyle);
        }
        largeSizeTypeStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSize);

        Add(largeSizeTypeStyle);

        var middleTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Middle));
        middleTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty, ProgressBarTokenKey.LineInfoIconSize);
        // icon
        {
            var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            completedIconsStyle.Add(Layoutable.WidthProperty, ProgressBarTokenKey.LineInfoIconSizeSM);
            completedIconsStyle.Add(Layoutable.HeightProperty, ProgressBarTokenKey.LineInfoIconSizeSM);
            middleTypeStyle.Add(completedIconsStyle);
        }

        middleTypeStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSizeSM);
        Add(middleTypeStyle);

        var smallTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Small));
        smallTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty, ProgressBarTokenKey.LineInfoIconSizeSM);
        // icon
        {
            var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            completedIconsStyle.Add(Layoutable.WidthProperty, ProgressBarTokenKey.LineInfoIconSizeSM);
            completedIconsStyle.Add(Layoutable.HeightProperty, ProgressBarTokenKey.LineInfoIconSizeSM);
            smallTypeStyle.Add(completedIconsStyle);
        }
        smallTypeStyle.Add(TemplatedControl.FontSizeProperty, SharedTokenKey.FontSizeSM);
        Add(smallTypeStyle);
    }
}