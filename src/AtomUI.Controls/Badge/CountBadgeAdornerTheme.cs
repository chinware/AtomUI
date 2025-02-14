﻿using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.Badge;

[ControlThemeProvider]
internal class CountBadgeAdornerTheme : BaseControlTheme
{
    internal const string IndicatorMotionActorPart = "PART_IndicatorMotionActor";
    internal const string RootLayoutPart = "PART_RootLayout";
    internal const string BadgeIndicatorPart = "PART_BadgeIndicator";
    internal const string BadgeTextPart = "PART_BadgeText";
    
    public CountBadgeAdornerTheme()
        : base(typeof(CountBadgeAdorner))
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<CountBadgeAdorner>((adorner, scope) =>
        {
            var indicatorMotionActor = new MotionActorControl()
            {
                Name               = IndicatorMotionActorPart,
                ClipToBounds       = false,
                UseRenderTransform = true
            };
            var layout = new Panel()
            {
                Name = RootLayoutPart,
            };
            indicatorMotionActor.Child = layout;
            indicatorMotionActor.RegisterInNameScope(scope);
            layout.RegisterInNameScope(scope);
            BuildBadgeIndicator(adorner, layout, scope);
            BuildBadgeText(layout, scope);
            return indicatorMotionActor;
        });
    }

    private void BuildBadgeIndicator(CountBadgeAdorner adorner, Panel layout, INameScope scope)
    {
        var indicator = new Border()
        {
            Name = BadgeIndicatorPart
        };

        CreateTemplateParentBinding(indicator, Border.BoxShadowProperty, CountBadgeAdorner.BoxShadowProperty);
        CreateTemplateParentBinding(indicator, Border.BackgroundProperty, CountBadgeAdorner.BadgeColorProperty);
        
        layout.Children.Add(indicator);
        indicator.RegisterInNameScope(scope);
    }

    private void BuildBadgeText(Panel layout, INameScope scope)
    {
        var badgeText = new SingleLineText()
        {
            Name = BadgeTextPart,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        CreateTemplateParentBinding(badgeText, SingleLineText.TextProperty, CountBadgeAdorner.CountTextProperty);
        layout.Children.Add(badgeText);
        badgeText.RegisterInNameScope(scope);
    }
    
    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());
        commonStyle.Add(CountBadgeAdorner.ClipToBoundsProperty, false);
        var inAdornerStyle = new Style(selector => selector.Nesting().PropertyEquals(CountBadgeAdorner.IsAdornerModeProperty, true));
        var layoutStyle = new Style(selector => selector.Nesting().Template().Name(RootLayoutPart));
        layoutStyle.Add(Panel.HorizontalAlignmentProperty, HorizontalAlignment.Right);
        layoutStyle.Add(Panel.VerticalAlignmentProperty, VerticalAlignment.Top);
        inAdornerStyle.Add(layoutStyle);
        commonStyle.Add(inAdornerStyle);
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(BadgeIndicatorPart));
            indicatorStyle.Add(Border.HeightProperty, BadgeTokenKey.IndicatorHeight);
            indicatorStyle.Add(Border.MinWidthProperty, BadgeTokenKey.IndicatorHeight);
            indicatorStyle.Add(Border.CornerRadiusProperty, BadgeTokenKey.CountBadgeCornerRadius);
            commonStyle.Add(indicatorStyle);
        
            var badgeTextStyle = new Style(selector => selector.Nesting().Template().Name(BadgeTextPart));
            badgeTextStyle.Add(SingleLineText.ForegroundProperty, BadgeTokenKey.BadgeTextColor);
            badgeTextStyle.Add(SingleLineText.FontSizeProperty, BadgeTokenKey.TextFontSize);
            badgeTextStyle.Add(SingleLineText.PaddingProperty, BadgeTokenKey.CountBadgeTextPadding);
            commonStyle.Add(badgeTextStyle);
        }
        
        var smallSizeStyle = new Style(selector => selector.Nesting().PropertyEquals(CountBadgeAdorner.SizeProperty, CountBadgeSize.Small));
        {
            var indicatorStyle = new Style(selector => selector.Nesting().Template().Name(BadgeIndicatorPart));
            indicatorStyle.Add(Border.HeightProperty, BadgeTokenKey.IndicatorHeightSM);
            indicatorStyle.Add(Border.MinWidthProperty, BadgeTokenKey.IndicatorHeightSM);
            indicatorStyle.Add(Border.CornerRadiusProperty, BadgeTokenKey.CountBadgeCornerRadiusSM);
            smallSizeStyle.Add(indicatorStyle);
        
            var badgeTextStyle = new Style(selector => selector.Nesting().Template().Name(BadgeTextPart));
            badgeTextStyle.Add(TextBlock.FontSizeProperty, BadgeTokenKey.TextFontSizeSM);
            smallSizeStyle.Add(badgeTextStyle);
        }
        commonStyle.Add(smallSizeStyle);

        Add(commonStyle);
    }
}