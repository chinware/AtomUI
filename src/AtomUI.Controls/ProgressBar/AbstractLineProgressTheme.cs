using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
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
        CreateCompletedIcons(scope, container);
    }

    private void CreateCompletedIcons(INameScope scope, Canvas container)
    {
        var exceptionCompletedIcon = AntDesignIconPackage.CloseCircleFilled();
        exceptionCompletedIcon.Name                = ExceptionCompletedIconPart;
        exceptionCompletedIcon.HorizontalAlignment = HorizontalAlignment.Left;
        
        exceptionCompletedIcon.RegisterInNameScope(scope);
        TokenResourceBinder.CreateSharedTokenBinding(exceptionCompletedIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorError);
        TokenResourceBinder.CreateSharedTokenBinding(exceptionCompletedIcon, Icon.DisabledFilledBrushProperty,
            SharedTokenKey.ControlItemBgActiveDisabled);
        
        var successCompletedIcon = AntDesignIconPackage.CheckCircleFilled();
        successCompletedIcon.Name                = SuccessCompletedIconPart;
        successCompletedIcon.HorizontalAlignment = HorizontalAlignment.Left;
        
        successCompletedIcon.RegisterInNameScope(scope);
        TokenResourceBinder.CreateSharedTokenBinding(successCompletedIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorSuccess);
        TokenResourceBinder.CreateSharedTokenBinding(successCompletedIcon, Icon.DisabledFilledBrushProperty,
            SharedTokenKey.ControlItemBgActiveDisabled);

        container.Children.Add(exceptionCompletedIcon);
        container.Children.Add(successCompletedIcon);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(AbstractLineProgress.LineProgressPaddingProperty,
            ProgressBarTokenKey.LineProgressPadding);
        commonStyle.Add(AbstractLineProgress.LineExtraInfoMarginProperty,
            ProgressBarTokenKey.LineExtraInfoMargin);
        Add(commonStyle);
        BuildSizeTypeStyle();
    }

    private void BuildSizeTypeStyle()
    {
        var largeSizeTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Large));
        largeSizeTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty,
            ProgressBarTokenKey.LineInfoIconSize);
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
        middleTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty,
            ProgressBarTokenKey.LineInfoIconSize);
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
        smallTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty,
            ProgressBarTokenKey.LineInfoIconSizeSM);
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