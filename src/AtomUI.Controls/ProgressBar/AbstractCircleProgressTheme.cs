using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class AbstractCircleProgressTheme : AbstractProgressBarTheme
{
    public AbstractCircleProgressTheme(Type targetType) : base(targetType)
    {
    }

    protected override void NotifyBuildControlTemplate(AbstractProgressBar progressBar, INameScope scope, Canvas container)
    {
        base.NotifyBuildControlTemplate(progressBar, scope, container);
        CreateCompletedIcons(progressBar, scope, container);
    }

    private void CreateCompletedIcons(AbstractProgressBar progressBar, INameScope scope, Canvas container)
    {
        var exceptionCompletedIcon = AntDesignIconPackage.CloseOutlined();
        exceptionCompletedIcon.Name                = ExceptionCompletedIconPart;
        exceptionCompletedIcon.HorizontalAlignment = HorizontalAlignment.Center;
        exceptionCompletedIcon.VerticalAlignment   = VerticalAlignment.Center;
        
        exceptionCompletedIcon.RegisterInNameScope(scope);

        var successCompletedIcon = AntDesignIconPackage.CheckOutlined();
        successCompletedIcon.Name                = SuccessCompletedIconPart;
        successCompletedIcon.HorizontalAlignment = HorizontalAlignment.Center;
        successCompletedIcon.VerticalAlignment   = VerticalAlignment.Center;
        
        successCompletedIcon.RegisterInNameScope(scope);

        container.Children.Add(exceptionCompletedIcon);
        container.Children.Add(successCompletedIcon);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        
        // 完成图标样式
        var exceptionCompletedIconStyle = new Style(selector => selector.Nesting().Template().Name(ExceptionCompletedIconPart));
        exceptionCompletedIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorError);
        exceptionCompletedIconStyle.Add(Icon.DisabledFilledBrushProperty, SharedTokenKey.ControlItemBgActiveDisabled);
        Add(exceptionCompletedIconStyle);
        
        var successCompletedIconStyle = new Style(selector => selector.Nesting().Template().Name(SuccessCompletedIconPart));
        successCompletedIconStyle.Add(Icon.NormalFilledBrushProperty, SharedTokenKey.ColorSuccess);
        successCompletedIconStyle.Add(Icon.DisabledFilledBrushProperty, SharedTokenKey.ControlItemBgActiveDisabled);
        Add(successCompletedIconStyle);

        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(AbstractCircleProgress.CircleMinimumTextFontSizeProperty,
            ProgressBarTokenKey.CircleMinimumTextFontSize);
        commonStyle.Add(AbstractCircleProgress.CircleMinimumIconSizeProperty,
            ProgressBarTokenKey.CircleMinimumIconSize);
        {
            var labelStyle = new Style(selector => selector.Nesting().Template().OfType<Label>());
            labelStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            labelStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
            commonStyle.Add(labelStyle);
        }
        var labelVisibleStyle = new Style(selector => selector.Nesting()
                                                              .PropertyEquals(
                                                                  AbstractProgressBar.PercentLabelVisibleProperty,
                                                                  true));
        {
            var labelStyle = new Style(selector => selector.Nesting().Template().OfType<Label>());
            labelStyle.Add(Visual.IsVisibleProperty, true);
            labelVisibleStyle.Add(labelStyle);
        }
        commonStyle.Add(labelVisibleStyle);

        var labelInVisibleStyle = new Style(selector => selector.Nesting()
                                                                .PropertyEquals(
                                                                    AbstractProgressBar.PercentLabelVisibleProperty,
                                                                    false));
        {
            var labelStyle = new Style(selector => selector.Nesting().Template().OfType<Label>());
            labelStyle.Add(Visual.IsVisibleProperty, false);
            labelInVisibleStyle.Add(labelStyle);
        }
        commonStyle.Add(labelInVisibleStyle);

        Add(commonStyle);
    }
}