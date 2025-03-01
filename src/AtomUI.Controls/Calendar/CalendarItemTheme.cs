﻿using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CalendarItemTheme : BaseControlTheme
{
    public const string ItemFramePart = "PART_ItemFrame";
    public const string ItemRootLayoutPart = "PART_ItemRootLayout";
    public const string MonthViewPart = "PART_MonthView";
    public const string YearViewPart = "PART_YearView";
    public const string HeaderLayoutPart = "PART_HeaderLayout";

    public const string PreviousButtonPart = "PART_PreviousButton";
    public const string PreviousMonthButtonPart = "PART_PreviousMonthButton";
    public const string HeaderButtonPart = "PART_HeaderButton";
    public const string NextMonthButtonPart = "PART_NextMonthButton";
    public const string NextButtonPart = "PART_NextButton";

    public CalendarItemTheme()
        : this(typeof(CalendarItem))
    {
    }

    protected CalendarItemTheme(Type targetType) : base(targetType)
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<CalendarItem>((calendarItem, scope) =>
        {
            var frame = new Border
            {
                Name = ItemFramePart
            };

            var rootLayout = new DockPanel
            {
                Name          = ItemRootLayoutPart,
                LastChildFill = true
            };
            BuildHeader(calendarItem, rootLayout, scope);
            BuildContentView(calendarItem, rootLayout, scope);

            BuildDayTitleTemplate(calendarItem);
            frame.Child = rootLayout;

            return frame;
        });
    }

    private void BuildDayTitleTemplate(CalendarItem calendarItem)
    {
        calendarItem.DayTitleTemplate = new DayTitleTemplate();
    }

    protected virtual void BuildHeader(CalendarItem calendarItem, DockPanel layout, INameScope scope)
    {
        var headerLayout = new Grid
        {
            Name = HeaderLayoutPart,
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Auto),
                new(GridLength.Auto),
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Auto)
            }
        };

        var previousButton = BuildPreviousButton(calendarItem);
        previousButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousButton, 0);
        headerLayout.Children.Add(previousButton);

        var previousMonthButton = BuildPreviousMonthButton(calendarItem);
        previousMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousMonthButton, 1);
        headerLayout.Children.Add(previousMonthButton);

        var headerButton = new HeadTextButton
        {
            Name = HeaderButtonPart
        };
        Grid.SetColumn(headerButton, 2);
        headerButton.RegisterInNameScope(scope);
        headerLayout.Children.Add(headerButton);

        var nextMonthButton = BuildNextMonthButton(calendarItem);
        Grid.SetColumn(nextMonthButton, 3);
        nextMonthButton.RegisterInNameScope(scope);
        headerLayout.Children.Add(nextMonthButton);

        var nextButton = BuildNextButton(calendarItem);

        Grid.SetColumn(nextButton, 4);
        nextButton.RegisterInNameScope(scope);
        headerLayout.Children.Add(nextButton);

        DockPanel.SetDock(headerLayout, Dock.Top);
        layout.Children.Add(headerLayout);
    }

    protected virtual IconButton BuildPreviousButton(CalendarItem calendarItem)
    {
        var previousButtonIcon = AntDesignIconPackage.DoubleLeftOutlined();
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousButtonIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextDescription));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousButtonIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText));

        var previousButton = new IconButton
        {
            Name = PreviousButtonPart,
            Icon = previousButtonIcon
        };

        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSizeSM));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSizeSM));
        return previousButton;
    }

    protected virtual IconButton BuildPreviousMonthButton(CalendarItem calendarItem)
    {
        var previousMonthButtonIcon = AntDesignIconPackage.LeftOutlined();
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousMonthButtonIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextDescription));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousMonthButtonIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousMonthButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText));

        var previousMonthButton = new IconButton
        {
            Name = PreviousMonthButtonPart,
            Icon = previousMonthButtonIcon
        };

        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousMonthButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSizeSM));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(previousMonthButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSizeSM));
        return previousMonthButton;
    }

    protected virtual IconButton BuildNextButton(CalendarItem calendarItem)
    {
        var nextButtonIcon = AntDesignIconPackage.DoubleRightOutlined();
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextButtonIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextDescription));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextButtonIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText));

        var nextButton = new IconButton
        {
            Name = NextButtonPart,
            Icon = nextButtonIcon
        };
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSizeSM));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSizeSM));
        return nextButton;
    }

    protected virtual IconButton BuildNextMonthButton(CalendarItem calendarItem)
    {
        var nextMonthButtonIcon = AntDesignIconPackage.RightOutlined();
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextMonthButtonIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextDescription));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextMonthButtonIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextMonthButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText));
        var nextMonthButton = new IconButton
        {
            Name = NextMonthButtonPart,
            Icon = nextMonthButtonIcon
        };
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextMonthButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSizeSM));
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(nextMonthButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSizeSM));
        return nextMonthButton;
    }

    private void BuildContentView(CalendarItem calendarItem, DockPanel layout, INameScope scope)
    {
        var dayTitleRowDef = new RowDefinition();
        calendarItem.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(dayTitleRowDef, RowDefinition.HeightProperty,
            CalendarTokenKey.DayTitleHeight));

        var monthView = new Grid
        {
            Name                = MonthViewPart,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsVisible           = false,
            RowDefinitions = new RowDefinitions
            {
                dayTitleRowDef,
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star)
            },
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star)
            }
        };
        monthView.RegisterInNameScope(scope);
        layout.Children.Add(monthView);

        var yearView = new Grid
        {
            Name      = YearViewPart,
            IsVisible = false,
            RowDefinitions = new RowDefinitions
            {
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star)
            },
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star),
                new(GridLength.Star)
            }
        };
        yearView.RegisterInNameScope(scope);
        layout.Children.Add(yearView);
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        var headerLayoutStyle = new Style(selector => selector.Nesting().Template().Name(HeaderLayoutPart));
        headerLayoutStyle.Add(Layoutable.MarginProperty, CalendarTokenKey.HeaderMargin);

        commonStyle.Add(headerLayoutStyle);

        Add(commonStyle);
    }
}

internal class DayTitleTemplate : ITemplate<Control>
{
    public Control Build()
    {
        var textBlock = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center
        };
        
        // TODO 需要观察是否有内存泄漏
        textBlock.Bind(TextBlock.TextProperty, new Binding());
        return textBlock;
    }

    object ITemplate.Build()
    {
        return Build();
    }
}