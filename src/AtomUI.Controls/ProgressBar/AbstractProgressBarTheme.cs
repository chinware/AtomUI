using AtomUI.IconPkg;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class AbstractProgressBarTheme : BaseControlTheme
{
    public const string PercentageLabelPart = "PART_PercentageLabel";
    public const string SuccessCompletedIconPart = "PART_SuccessCompletedIcon";
    public const string ExceptionCompletedIconPart = "PART_ExceptionCompletedIcon";
    public const string LayoutTransformControlPart = "PART_LayoutTransformControl";

    public AbstractProgressBarTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate? BuildControlTemplate()
    {
        return new FuncControlTemplate<AbstractProgressBar>((bar, scope) =>
        {
            var mainContainer = new Canvas();
            NotifyBuildControlTemplate(bar, scope, mainContainer);
            return mainContainer;
        });
    }

    protected virtual void NotifyBuildControlTemplate(AbstractProgressBar bar, INameScope scope, Canvas container)
    {
        var percentageLabel = new Label
        {
            Name                     = PercentageLabelPart,
            Padding                  = new Thickness(0),
            Margin                   = new Thickness(0),
            VerticalContentAlignment = VerticalAlignment.Center
        };
        percentageLabel.RegisterInNameScope(scope);
        CreateTemplateParentBinding(percentageLabel, InputElement.IsEnabledProperty, InputElement.IsEnabledProperty);
        var layoutTransform = new LayoutTransformControl
        {
            Name  = LayoutTransformControlPart,
            Child = percentageLabel
        };
        layoutTransform.RegisterInNameScope(scope);
        container.Children.Add(layoutTransform);
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        BuildCommonStyle();
        BuildInfoLabelStyle();
        BuildStatusStyle();
        BuildDisabledStyle();
    }

    private void BuildCommonStyle()
    {
        var enabledStyle =
            new Style(selector => selector.Nesting().PropertyEquals(InputElement.IsEnabledProperty, true));
        enabledStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextLabel);
        enabledStyle.Add(AbstractProgressBar.GrooveBrushProperty, ProgressBarTokenKey.RemainingColor);
        Add(enabledStyle);
    }

    private void BuildStatusStyle()
    {
        // 异常状态
        var exceptionStatusStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Exception));
        {
            var exceptionIconStyle = new Style(selector =>
                selector.Nesting().Template().OfType<Icon>().Name(ExceptionCompletedIconPart));
            exceptionIconStyle.Add(Visual.IsVisibleProperty, true);
            exceptionStatusStyle.Add(exceptionIconStyle);

            var successIconStyle = new Style(selector =>
                selector.Nesting().Template().OfType<Icon>().Name(SuccessCompletedIconPart));
            successIconStyle.Add(Visual.IsVisibleProperty, false);
            exceptionStatusStyle.Add(successIconStyle);

            var infoLabelStyle = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
            infoLabelStyle.Add(Visual.IsVisibleProperty, false);

            exceptionStatusStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, SharedTokenKey.ColorError);

            exceptionStatusStyle.Add(infoLabelStyle);
        }
        Add(exceptionStatusStyle);

        // 成功状态
        var successStatusStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Success));
        {
            var exceptionIconStyle = new Style(selector =>
                selector.Nesting().Template().OfType<Icon>().Name(ExceptionCompletedIconPart));
            exceptionIconStyle.Add(Visual.IsVisibleProperty, false);
            successStatusStyle.Add(exceptionIconStyle);

            var successIconStyle = new Style(selector =>
                selector.Nesting().Template().OfType<Icon>().Name(SuccessCompletedIconPart));
            successIconStyle.Add(Visual.IsVisibleProperty, true);
            successStatusStyle.Add(successIconStyle);

            var infoLabelStyle = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
            infoLabelStyle.Add(Visual.IsVisibleProperty, false);

            successStatusStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, SharedTokenKey.ColorSuccess);

            successStatusStyle.Add(infoLabelStyle);
        }
        Add(successStatusStyle);

        // 正常状态
        var normalOrActiveStatusStyle = new Style(selector => Selectors.Or(
            selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Normal),
            selector.Nesting().PropertyEquals(AbstractProgressBar.StatusProperty, ProgressStatus.Active)));
        {
            {
                var exceptionIconStyle = new Style(selector =>
                    selector.Nesting().Template().OfType<Icon>().Name(ExceptionCompletedIconPart));
                exceptionIconStyle.Add(Visual.IsVisibleProperty, false);
                normalOrActiveStatusStyle.Add(exceptionIconStyle);
            }
            {
                var successIconStyle = new Style(selector =>
                    selector.Nesting().Template().OfType<Icon>().Name(SuccessCompletedIconPart));
                successIconStyle.Add(Visual.IsVisibleProperty, false);
                normalOrActiveStatusStyle.Add(successIconStyle);
            }
            {
                var completedStyle = new Style(selector => selector.Nesting().Class(AbstractProgressBar.CompletedPC));
                completedStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty, SharedTokenKey.ColorSuccess);
                var successIconStyle = new Style(selector =>
                    selector.Nesting().Template().OfType<Icon>().Name(SuccessCompletedIconPart));
                successIconStyle.Add(Visual.IsVisibleProperty, true);
                completedStyle.Add(successIconStyle);
                normalOrActiveStatusStyle.Add(completedStyle);
            }

            normalOrActiveStatusStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty,
                ProgressBarTokenKey.DefaultColor);
        }
        Add(normalOrActiveStatusStyle);
    }

    private void BuildInfoLabelStyle()
    {
        var completedStyle = new Style(selector => selector.Nesting().Class(AbstractProgressBar.CompletedPC));
        var infoLabelStyle = new Style(selector => selector.Nesting().Template().OfType<LayoutTransformControl>());
        infoLabelStyle.Add(Visual.IsVisibleProperty, false);
        completedStyle.Add(infoLabelStyle);
        Add(completedStyle);
    }

    private void BuildDisabledStyle()
    {
        var disableStyle = new Style(selector => selector.Nesting().Class(StdPseudoClass.Disabled));
        disableStyle.Add(AbstractProgressBar.GrooveBrushProperty, SharedTokenKey.ColorBgContainerDisabled);
        disableStyle.Add(AbstractProgressBar.IndicatorBarBrushProperty,
            SharedTokenKey.ControlItemBgActiveDisabled);
        disableStyle.Add(TemplatedControl.ForegroundProperty, SharedTokenKey.ColorTextDisabled);
        var statusIconStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
        statusIconStyle.Add(Icon.IconModeProperty, IconMode.Disabled);
        disableStyle.Add(statusIconStyle);
        Add(disableStyle);
    }
}