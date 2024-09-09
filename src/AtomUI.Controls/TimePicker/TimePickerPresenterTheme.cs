using AtomUI.Theme;
using AtomUI.Theme.Data;
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
internal class TimePickerPresenterTheme : BaseControlTheme
{
    public const string MainContainerPart = "PART_MainContainer";
    public const string PickerContainerPart = "PART_PickerContainer";
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

    public TimePickerPresenterTheme()
        : base(typeof(TimePickerPresenter))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TimePickerPresenter>((presenter, scope) =>
        {
            var mainContainer = new Grid
            {
                Name = MainContainerPart,
                RowDefinitions = new RowDefinitions
                {
                    new(GridLength.Auto),
                    new(GridLength.Auto),
                    new(GridLength.Star)
                }
            };
            BuildHeader(mainContainer, scope);
            BuildHosts(mainContainer, scope);
            return mainContainer;
        });
    }

    private void BuildHeader(Grid mainContainer, INameScope scope)
    {
        var headerText = new TextBlock
        {
            Name = HeaderTextPart
        };
        headerText.RegisterInNameScope(scope);
        CreateTemplateParentBinding(headerText, Visual.IsVisibleProperty, TimePickerPresenter.IsShowHeaderProperty);
        mainContainer.Children.Add(headerText);
        Grid.SetRow(headerText, 0);
        var separator = new Rectangle
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        CreateTemplateParentBinding(separator, Layoutable.HeightProperty, TimePickerPresenter.SpacerThicknessProperty);
        CreateTemplateParentBinding(separator, Visual.IsVisibleProperty, TimePickerPresenter.IsShowHeaderProperty);
        TokenResourceBinder.CreateGlobalTokenBinding(separator, Shape.FillProperty,
            GlobalTokenResourceKey.ColorBorder);
        mainContainer.Children.Add(separator);
        Grid.SetRow(separator, 1);
    }

    private void BuildHosts(Grid mainContainer, INameScope scope)
    {
        var pickerContainer = new Grid
        {
            Name = PickerContainerPart,
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
            Name = HourHostPart
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
            TokenResourceBinder.CreateTokenBinding(hourSelector, DateTimePickerPanel.ItemHeightProperty,
                TimePickerTokenResourceKey.ItemHeight);
            scrollViewer.Content = hourSelector;
            hourHost.Children.Add(scrollViewer);
        }

        var firstSpacer = new Rectangle
        {
            Name                = FirstSpacerPart,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        firstSpacer.RegisterInNameScope(scope);
        CreateTemplateParentBinding(firstSpacer, Layoutable.WidthProperty, TimePickerPresenter.SpacerThicknessProperty);
        TokenResourceBinder.CreateGlobalTokenBinding(firstSpacer, Shape.FillProperty,
            GlobalTokenResourceKey.ColorBorder);
        Grid.SetColumn(firstSpacer, 1);
        pickerContainer.Children.Add(firstSpacer);

        var minuteHost = new Panel
        {
            Name = MinuteHostPart
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
            TokenResourceBinder.CreateTokenBinding(minuteSelector, DateTimePickerPanel.ItemHeightProperty,
                TimePickerTokenResourceKey.ItemHeight);
            scrollViewer.Content = minuteSelector;
            minuteHost.Children.Add(scrollViewer);
        }

        var secondSpacer = new Rectangle
        {
            Name                = SecondSpacerPart,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        CreateTemplateParentBinding(secondSpacer, Layoutable.WidthProperty,
            TimePickerPresenter.SpacerThicknessProperty);
        secondSpacer.RegisterInNameScope(scope);
        TokenResourceBinder.CreateGlobalTokenBinding(secondSpacer, Shape.FillProperty,
            GlobalTokenResourceKey.ColorBorder);
        Grid.SetColumn(secondSpacer, 3);
        pickerContainer.Children.Add(secondSpacer);

        var secondHost = new Panel
        {
            Name = SecondHostPart
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
            TokenResourceBinder.CreateTokenBinding(secondSelector, DateTimePickerPanel.ItemHeightProperty,
                TimePickerTokenResourceKey.ItemHeight);
            scrollViewer.Content = secondSelector;
            secondHost.Children.Add(scrollViewer);
        }

        var thirdSpacer = new Rectangle
        {
            Name                = ThirdSpacerPart,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        CreateTemplateParentBinding(thirdSpacer, Layoutable.WidthProperty, TimePickerPresenter.SpacerThicknessProperty);
        thirdSpacer.RegisterInNameScope(scope);
        Grid.SetColumn(thirdSpacer, 5);
        pickerContainer.Children.Add(thirdSpacer);
        TokenResourceBinder.CreateGlobalTokenBinding(thirdSpacer, Shape.FillProperty,
            GlobalTokenResourceKey.ColorBorder);

        var periodHost = new Panel
        {
            Name = PeriodHostPart
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
            TokenResourceBinder.CreateTokenBinding(periodSelector, DateTimePickerPanel.ItemHeightProperty,
                TimePickerTokenResourceKey.ItemHeight);
            scrollViewer.Content = periodSelector;
            periodHost.Children.Add(scrollViewer);
        }

        mainContainer.Children.Add(pickerContainer);
        mainContainer.RegisterInNameScope(scope);
        Grid.SetRow(pickerContainer, 2);
    }

    protected override void BuildStyles()
    {
        var commonStyle     = new Style(selector => selector.Nesting());
        var headerTextStyle = new Style(selector => selector.Nesting().Template().Name(HeaderTextPart));
        headerTextStyle.Add(Layoutable.HeightProperty, TimePickerTokenResourceKey.ItemHeight);
        headerTextStyle.Add(Layoutable.HorizontalAlignmentProperty, HorizontalAlignment.Center);
        headerTextStyle.Add(Layoutable.VerticalAlignmentProperty, VerticalAlignment.Center);
        headerTextStyle.Add(TextBlock.FontWeightProperty, FontWeight.SemiBold);

        commonStyle.Add(headerTextStyle);
        Add(commonStyle);
    }
}