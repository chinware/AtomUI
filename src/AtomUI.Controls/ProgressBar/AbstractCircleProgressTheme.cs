using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Data;
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

    protected override void NotifyBuildControlTemplate(AbstractProgressBar bar, INameScope scope, Canvas container)
    {
        base.NotifyBuildControlTemplate(bar, scope, container);
        CreateCompletedIcons(scope, container);
    }

    private void CreateCompletedIcons(INameScope scope, Canvas container)
    {
        var exceptionCompletedIcon = AntDesignIconPackage.CloseOutlined();
        exceptionCompletedIcon.Name                = ExceptionCompletedIconPart;
        exceptionCompletedIcon.HorizontalAlignment = HorizontalAlignment.Center;
        exceptionCompletedIcon.VerticalAlignment   = VerticalAlignment.Center;
        
        exceptionCompletedIcon.RegisterInNameScope(scope);
        TokenResourceBinder.CreateGlobalTokenBinding(exceptionCompletedIcon, Icon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorError);
        TokenResourceBinder.CreateGlobalTokenBinding(exceptionCompletedIcon, Icon.DisabledFilledBrushProperty,
            GlobalTokenResourceKey.ControlItemBgActiveDisabled);

        var successCompletedIcon = AntDesignIconPackage.CheckOutlined();
        successCompletedIcon.Name                = SuccessCompletedIconPart;
        successCompletedIcon.HorizontalAlignment = HorizontalAlignment.Center;
        successCompletedIcon.VerticalAlignment   = VerticalAlignment.Center;
        
        successCompletedIcon.RegisterInNameScope(scope);
        TokenResourceBinder.CreateGlobalTokenBinding(successCompletedIcon, Icon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorSuccess);
        TokenResourceBinder.CreateGlobalTokenBinding(successCompletedIcon, Icon.DisabledFilledBrushProperty,
            GlobalTokenResourceKey.ControlItemBgActiveDisabled);

        container.Children.Add(exceptionCompletedIcon);
        container.Children.Add(successCompletedIcon);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();

        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(AbstractCircleProgress.CircleMinimumTextFontSizeProperty,
            ProgressBarTokenResourceKey.CircleMinimumTextFontSize);
        commonStyle.Add(AbstractCircleProgress.CircleMinimumIconSizeProperty,
            ProgressBarTokenResourceKey.CircleMinimumIconSize);
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