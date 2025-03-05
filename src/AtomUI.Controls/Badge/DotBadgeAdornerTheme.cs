using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.Badge;

[ControlThemeProvider]
internal class DotBadgeAdornerTheme : BaseControlTheme
{
    internal const string IndicatorMotionActorPart = "PART_IndicatorMotionActor";
    internal const string IndicatorPart = "PART_Indicator";
    internal const string LabelPart = "PART_Label";
    internal const string RootLayoutPart = "PART_RootLayout";

    public DotBadgeAdornerTheme()
        : base(typeof(DotBadgeAdorner))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DotBadgeAdorner>((adorner, scope) =>
        {
            var layout = new DockPanel()
            {
                Name = RootLayoutPart,
                LastChildFill = true,
                ClipToBounds = false
            };
            BuildIndicator(layout, scope);
            BuildLabel(layout, scope);
            return layout;
        });
    }

    private void BuildIndicator(DockPanel layout, INameScope scope)
    {
        var indicatorMotionActor = new MotionActorControl()
        {
            Name = IndicatorMotionActorPart,
            ClipToBounds = false,
            UseRenderTransform = true
        };
        indicatorMotionActor.RegisterInNameScope(scope);
        var indicator = new DotBadgeIndicator()
        {
            Name = IndicatorPart
        };
        indicator.RegisterInNameScope(scope);
        DockPanel.SetDock(indicatorMotionActor, Dock.Left);

        CreateTemplateParentBinding(indicator, DotBadgeIndicator.BadgeDotColorProperty, DotBadgeAdorner.BadgeDotColorProperty);
        indicatorMotionActor.Child = indicator;
        layout.Children.Add(indicatorMotionActor);
    }

    private void BuildLabel(DockPanel layout, INameScope scope)
    {
        var label = new Label
        {
            Name = LabelPart
        };
        label.RegisterInNameScope(scope);
        CreateTemplateParentBinding(label, Label.ContentProperty, DotBadgeAdorner.TextProperty);
        CreateTemplateParentBinding(label, Label.IsVisibleProperty, DotBadgeAdorner.IsAdornerModeProperty,
            BindingMode.Default,
            BoolConverters.Not);
        layout.Children.Add(label);
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(DotBadgeAdorner.ClipToBoundsProperty, false);
        commonStyle.Add(DotBadgeAdorner.BadgeDotColorProperty, BadgeTokenKey.BadgeColor);
        var inAdornerStyle = new Style(selector => selector.Nesting().PropertyEquals(DotBadgeAdorner.IsAdornerModeProperty, true));
        var layoutStyle = new Style(selector => selector.Nesting().Template().Name(RootLayoutPart));
        layoutStyle.Add(DockPanel.HorizontalAlignmentProperty, HorizontalAlignment.Right);
        layoutStyle.Add(DockPanel.VerticalAlignmentProperty, VerticalAlignment.Top);
        inAdornerStyle.Add(layoutStyle);
        commonStyle.Add(inAdornerStyle);

        var labelStyle = new Style(selector => selector.Nesting().Template().Name(LabelPart));
        labelStyle.Add(Label.HorizontalAlignmentProperty, HorizontalAlignment.Left);
        labelStyle.Add(Label.VerticalAlignmentProperty, VerticalAlignment.Center);
        labelStyle.Add(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Center);
        labelStyle.Add(Label.VerticalContentAlignmentProperty, VerticalAlignment.Center);
        labelStyle.Add(Label.MarginProperty, BadgeTokenKey.DotBadgeLabelMargin);
        commonStyle.Add(labelStyle);

        var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(IndicatorMotionActorPart));

        indicatorStyle.Add(MotionActorControl.WidthProperty, BadgeTokenKey.DotSize);
        indicatorStyle.Add(MotionActorControl.HeightProperty, BadgeTokenKey.DotSize);
        commonStyle.Add(indicatorStyle);

        Add(commonStyle);

        BuildDotBadgeColorStyle();
    }

    protected void BuildDotBadgeColorStyle()
    {
        var errorStyle = new Style(selector => selector.Nesting().PropertyEquals(DotBadgeAdorner.StatusProperty, DotBadgeStatus.Error));
        errorStyle.Add(DotBadgeAdorner.BadgeDotColorProperty, SharedTokenKey.ColorError);
        Add(errorStyle);
        
        var successStyle = new Style(selector => selector.Nesting().PropertyEquals(DotBadgeAdorner.StatusProperty, DotBadgeStatus.Success));
        successStyle.Add(DotBadgeAdorner.BadgeDotColorProperty, SharedTokenKey.ColorSuccess);
        Add(successStyle);
        
        var warningStyle = new Style(selector => selector.Nesting().PropertyEquals(DotBadgeAdorner.StatusProperty, DotBadgeStatus.Warning));
        warningStyle.Add(DotBadgeAdorner.BadgeDotColorProperty, SharedTokenKey.ColorWarning);
        Add(warningStyle);
        
        var processingStyle = new Style(selector => selector.Nesting().PropertyEquals(DotBadgeAdorner.StatusProperty, DotBadgeStatus.Processing));
        processingStyle.Add(DotBadgeAdorner.BadgeDotColorProperty, SharedTokenKey.ColorInfo);
        Add(processingStyle);
        
        var defaultStyle = new Style(selector => selector.Nesting().PropertyEquals(DotBadgeAdorner.StatusProperty, DotBadgeStatus.Default));
        defaultStyle.Add(DotBadgeAdorner.BadgeDotColorProperty, SharedTokenKey.ColorTextPlaceholder);
        Add(defaultStyle);
    }
}