﻿using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimeViewTheme : BaseControlTheme
{
    public const string RootLayoutPart = "PART_RootLayout";
    public const string MainFramePart = "PART_MainFrame";
    public const string PickerSelectorContainerPart = "PART_PickerContainer";
    public const string HeaderTextPart = "PART_HeaderText";

    public const string HourHostPart = "PART_HourHost";
    public const string MinuteHostPart = "PART_MinuteHost";
    public const string SecondHostPart = "PART_SecondHost";
    public const string PeriodHostPart = "PART_PeriodHost";

    public const string HourSelectorPart = "PART_HourSelector";
    public const string MinuteSelectorPart = "PART_MinuteSelector";
    public const string SecondSelectorPart = "PART_SecondSelector";
    public const string PeriodSelectorPart = "PART_PeriodSelector";

    public const string FirstSpacerPart = "PART_FirstSpacer";
    public const string SecondSpacerPart = "PART_SecondSpacer";
    public const string ThirdSpacerPart = "PART_ThirdSpacer";
    
     public TimeViewTheme()
        : base(typeof(TimeView))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TimeView>((timeView, scope) =>
        {
            var frame = new Border()
            {
                Name = MainFramePart,
            };
            CreateTemplateParentBinding(frame, Border.PaddingProperty, TimeView.PaddingProperty);
            CreateTemplateParentBinding(frame, Border.BackgroundProperty, TimeView.BackgroundProperty);
            var rootLayout = new Grid
            {
                Name = RootLayoutPart,
                RowDefinitions = new RowDefinitions
                {
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Star)
                }
            };
            BuildHeader(timeView, rootLayout, scope);
            BuildHosts(timeView, rootLayout, scope);
            frame.Child = rootLayout;
            return frame;
        });
    }

    private void BuildHeader(TimeView timeView, Grid rootLayout, INameScope scope)
    {
        var headerText = new TextBlock()
        {
            Name = HeaderTextPart
        };
        headerText.RegisterInNameScope(scope);
        CreateTemplateParentBinding(headerText, Visual.IsVisibleProperty, TimeView.IsShowHeaderProperty);
        rootLayout.Children.Add(headerText);
        Grid.SetRow(headerText, 0);
        var separator = new Rectangle
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        CreateTemplateParentBinding(separator, Layoutable.HeightProperty, TimeView.SpacerThicknessProperty);
        CreateTemplateParentBinding(separator, Visual.IsVisibleProperty, TimeView.IsShowHeaderProperty);
        rootLayout.Children.Add(separator);
        Grid.SetRow(separator, 1);
    }

    private void BuildHosts(TimeView timeView, Grid rootLayout, INameScope scope)
    {
        var pickerContainer = new Grid
        {
            Name = PickerSelectorContainerPart,
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Star)
            }
        };
        pickerContainer.RegisterInNameScope(scope);

        var hourHost = new Panel
        {
            Name = HourHostPart,
        };
        Grid.SetColumn(hourHost, 0);
        hourHost.RegisterInNameScope(scope);
        pickerContainer.Children.Add(hourHost);
        {
            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Hidden
            };
            var hourSelector = new DateTimePickerPanel
            {
                Name       = HourSelectorPart,
                PanelType  = DateTimePickerPanelType.Hour,
                ShouldLoop = true
            };
            hourSelector.RegisterInNameScope(scope);
            CreateTemplateParentBinding(hourSelector, DateTimePickerPanel.IsMotionEnabledProperty, TimeView.IsMotionEnabledProperty);
            scrollViewer.Content = hourSelector;
            hourHost.Children.Add(scrollViewer);
        }

        var firstSpacer = new Rectangle
        {
            Name                = FirstSpacerPart,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        firstSpacer.RegisterInNameScope(scope);
        CreateTemplateParentBinding(firstSpacer, Layoutable.WidthProperty, TimeView.SpacerThicknessProperty);
        Grid.SetColumn(firstSpacer, 1);
        pickerContainer.Children.Add(firstSpacer);

        var minuteHost = new Panel
        {
            Name  = MinuteHostPart,
        };
        Grid.SetColumn(minuteHost, 2);
        minuteHost.RegisterInNameScope(scope);
        pickerContainer.Children.Add(minuteHost);

        {
            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Hidden
            };
            var minuteSelector = new DateTimePickerPanel
            {
                Name       = MinuteSelectorPart,
                PanelType  = DateTimePickerPanelType.Minute,
                ShouldLoop = true
            };
            minuteSelector.RegisterInNameScope(scope);
            CreateTemplateParentBinding(minuteSelector, DateTimePickerPanel.IsMotionEnabledProperty, TimeView.IsMotionEnabledProperty);
            scrollViewer.Content = minuteSelector;
            minuteHost.Children.Add(scrollViewer);
        }

        var secondSpacer = new Rectangle
        {
            Name                = SecondSpacerPart,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        CreateTemplateParentBinding(secondSpacer, Layoutable.WidthProperty,
            TimeView.SpacerThicknessProperty);
        secondSpacer.RegisterInNameScope(scope);
        Grid.SetColumn(secondSpacer, 3);
        pickerContainer.Children.Add(secondSpacer);

        var secondHost = new Panel
        {
            Name  = SecondHostPart,
        };
        Grid.SetColumn(secondHost, 4);
        secondHost.RegisterInNameScope(scope);
        pickerContainer.Children.Add(secondHost);

        {
            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Hidden
            };
            var secondSelector = new DateTimePickerPanel
            {
                Name       = SecondSelectorPart,
                PanelType  = DateTimePickerPanelType.Second,
                ShouldLoop = true
            };
            secondSelector.RegisterInNameScope(scope);
            CreateTemplateParentBinding(secondSelector, DateTimePickerPanel.IsMotionEnabledProperty, TimeView.IsMotionEnabledProperty);
            scrollViewer.Content = secondSelector;
            secondHost.Children.Add(scrollViewer);
        }

        var thirdSpacer = new Rectangle
        {
            Name                = ThirdSpacerPart,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        CreateTemplateParentBinding(thirdSpacer, Layoutable.WidthProperty, TimeView.SpacerThicknessProperty);
        thirdSpacer.RegisterInNameScope(scope);
        Grid.SetColumn(thirdSpacer, 5);
        pickerContainer.Children.Add(thirdSpacer);

        var periodHost = new Panel
        {
            Name  = PeriodHostPart,
        };
        Grid.SetColumn(periodHost, 6);
        periodHost.RegisterInNameScope(scope);
        pickerContainer.Children.Add(periodHost);

        {
            var scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility   = ScrollBarVisibility.Hidden
            };
            var periodSelector = new DateTimePickerPanel
            {
                Name       = PeriodSelectorPart,
                PanelType  = DateTimePickerPanelType.TimePeriod,
                ShouldLoop = false
            };
            periodSelector.RegisterInNameScope(scope);
            CreateTemplateParentBinding(periodSelector, DateTimePickerPanel.IsMotionEnabledProperty, TimeView.IsMotionEnabledProperty);
            scrollViewer.Content = periodSelector;
            periodHost.Children.Add(scrollViewer);
        }

        rootLayout.Children.Add(pickerContainer);
        rootLayout.RegisterInNameScope(scope);
        Grid.SetRow(pickerContainer, 2);
    }

    protected override void BuildStyles()
    {
        var commonStyle     = new Style(selector => selector.Nesting());
        
        commonStyle.Add(TimeView.ItemHeightProperty, TimePickerTokenKey.ItemHeight);
        
        var headerTextStyle = new Style(selector => selector.Nesting().Template().Name(HeaderTextPart));
        headerTextStyle.Add(TextBlock.HeightProperty, TimePickerTokenKey.ItemHeight);
        headerTextStyle.Add(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        headerTextStyle.Add(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
        headerTextStyle.Add(TextBlock.FontWeightProperty, FontWeight.SemiBold);
        headerTextStyle.Add(TextBlock.MarginProperty, TimePickerTokenKey.HeaderMargin);
        
        var dateTimePickerPanelStyle = new Style(selector => selector.Nesting().Template().OfType<DateTimePickerPanel>());
        dateTimePickerPanelStyle.Add(DateTimePickerPanel.ItemHeightProperty, TimePickerTokenKey.ItemHeight);
        commonStyle.Add(dateTimePickerPanelStyle);
        
        var spacerStyle = new Style(selector => selector.Nesting().Template().OfType<Rectangle>());
        spacerStyle.Add(Shape.FillProperty, SharedTokenKey.ColorBorderSecondary);
        commonStyle.Add(spacerStyle);

        var hourHostStyle = new Style(selector => selector.Nesting().Template().Name(HourHostPart));
        hourHostStyle.Add(Panel.WidthProperty, TimePickerTokenKey.ItemWidth);
        commonStyle.Add(hourHostStyle);
        
        var minuteHostStyle = new Style(selector => selector.Nesting().Template().Name(MinuteHostPart));
        minuteHostStyle.Add(Panel.WidthProperty, TimePickerTokenKey.ItemWidth);
        commonStyle.Add(minuteHostStyle);
        
        var secondHostStyle = new Style(selector => selector.Nesting().Template().Name(SecondHostPart));
        secondHostStyle.Add(Panel.WidthProperty, TimePickerTokenKey.ItemWidth);
        commonStyle.Add(secondHostStyle);
        
        var periodHostStyle = new Style(selector => selector.Nesting().Template().Name(PeriodHostPart));
        periodHostStyle.Add(Panel.WidthProperty, TimePickerTokenKey.PeriodHostWidth);
        commonStyle.Add(periodHostStyle);

        commonStyle.Add(headerTextStyle);
        Add(commonStyle);
    }
}