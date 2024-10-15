﻿using AtomUI.IconPkg;
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
        TokenResourceBinder.CreateGlobalTokenBinding(exceptionCompletedIcon, Icon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorError);
        TokenResourceBinder.CreateGlobalTokenBinding(exceptionCompletedIcon, Icon.DisabledFilledBrushProperty,
            GlobalTokenResourceKey.ControlItemBgActiveDisabled);
        
        var successCompletedIcon = AntDesignIconPackage.CheckCircleFilled();
        successCompletedIcon.Name                = SuccessCompletedIconPart;
        successCompletedIcon.HorizontalAlignment = HorizontalAlignment.Left;
        
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
        commonStyle.Add(AbstractLineProgress.LineProgressPaddingProperty,
            ProgressBarTokenResourceKey.LineProgressPadding);
        commonStyle.Add(AbstractLineProgress.LineExtraInfoMarginProperty,
            ProgressBarTokenResourceKey.LineExtraInfoMargin);
        Add(commonStyle);
        BuildSizeTypeStyle();
    }

    private void BuildSizeTypeStyle()
    {
        var largeSizeTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Large));
        largeSizeTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty,
            ProgressBarTokenResourceKey.LineInfoIconSize);
        // icon
        {
            var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            completedIconsStyle.Add(Layoutable.WidthProperty, ProgressBarTokenResourceKey.LineInfoIconSize);
            completedIconsStyle.Add(Layoutable.HeightProperty, ProgressBarTokenResourceKey.LineInfoIconSize);
            largeSizeTypeStyle.Add(completedIconsStyle);
        }
        largeSizeTypeStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSize);

        Add(largeSizeTypeStyle);

        var middleTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Middle));
        middleTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty,
            ProgressBarTokenResourceKey.LineInfoIconSize);
        // icon
        {
            var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            completedIconsStyle.Add(Layoutable.WidthProperty, ProgressBarTokenResourceKey.LineInfoIconSizeSM);
            completedIconsStyle.Add(Layoutable.HeightProperty, ProgressBarTokenResourceKey.LineInfoIconSizeSM);
            middleTypeStyle.Add(completedIconsStyle);
        }

        middleTypeStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSizeSM);
        Add(middleTypeStyle);

        var smallTypeStyle = new Style(selector =>
            selector.Nesting().PropertyEquals(AbstractProgressBar.EffectiveSizeTypeProperty, SizeType.Small));
        smallTypeStyle.Add(AbstractLineProgress.LineInfoIconSizeProperty,
            ProgressBarTokenResourceKey.LineInfoIconSizeSM);
        // icon
        {
            var completedIconsStyle = new Style(selector => selector.Nesting().Template().OfType<Icon>());
            completedIconsStyle.Add(Layoutable.WidthProperty, ProgressBarTokenResourceKey.LineInfoIconSizeSM);
            completedIconsStyle.Add(Layoutable.HeightProperty, ProgressBarTokenResourceKey.LineInfoIconSizeSM);
            smallTypeStyle.Add(completedIconsStyle);
        }
        smallTypeStyle.Add(TemplatedControl.FontSizeProperty, GlobalTokenResourceKey.FontSizeSM);
        Add(smallTypeStyle);
    }
}