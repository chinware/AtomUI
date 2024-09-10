using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class RangeCalendarItemTheme : BaseControlTheme
{
    public const string ItemFramePart = "PART_ItemFrame";
    public const string ItemRootLayoutPart = "PART_ItemRootLayout";
    public const string MonthViewPart = "PART_MonthView";
    public const string PrimaryMonthViewPart = "PART_PrimaryMonthView";
    public const string SecondaryMonthViewPart = "PART_SecondaryMonthView";
    public const string YearViewPart = "PART_YearView";
    public const string HeaderLayoutPart = "PART_HeaderLayout";

    public const string PrimaryPreviousButtonPart = "PART_PrimaryPreviousButton";
    public const string PrimaryPreviousMonthButtonPart = "PART_PrimaryPreviousMonthButton";
    public const string PrimaryHeaderButtonPart = "PART_PrimaryHeaderButton";
    public const string PrimaryNextMonthButtonPart = "PART_PrimaryNextMonthButton";
    public const string PrimaryNextButtonPart = "PART_PrimaryNextButton";

    public const string SecondaryPreviousButtonPart = "PART_SecondaryPreviousButton";
    public const string SecondaryPreviousMonthButtonPart = "PART_SecondaryPreviousMonthButton";
    public const string SecondaryHeaderButtonPart = "PART_SecondaryHeaderButton";
    public const string SecondaryNextMonthButtonPart = "PART_SecondaryNextMonthButton";
    public const string SecondaryNextButtonPart = "PART_SecondaryNextButton";

    public RangeCalendarItemTheme()
        : base(typeof(RangeCalendarItem))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<RangeCalendarItem>((calendarItem, scope) =>
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
            BuildHeader(rootLayout, scope);
            BuildContentView(rootLayout, scope);

            BuildDayTitleTemplate(calendarItem);
            frame.Child = rootLayout;

            return frame;
        });
    }

    private void BuildDayTitleTemplate(RangeCalendarItem calendarItem)
    {
        calendarItem.DayTitleTemplate = new DayTitleTemplate();
    }

    protected virtual void BuildHeader(DockPanel layout, INameScope scope)
    {
        var headerLayout = new UniformGrid
        {
            Name    = HeaderLayoutPart,
            Columns = 2
        };

        headerLayout.RegisterInNameScope(scope);
        BuildPrimaryHeaderItem(headerLayout, scope);
        BuildSecondaryHeaderItem(headerLayout, scope);

        DockPanel.SetDock(headerLayout, Dock.Top);
        layout.Children.Add(headerLayout);
    }

    private void BuildPrimaryHeaderItem(UniformGrid layout, INameScope scope)
    {
        var headerLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Auto),
                new(GridLength.Auto),
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Auto)
            }
        };

        var previousButton = BuildPreviousButton(PrimaryPreviousButtonPart);
        previousButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousButton, 0);
        headerLayout.Children.Add(previousButton);

        var previousMonthButton = BuildPreviousMonthButton(PrimaryPreviousMonthButtonPart);
        CreateTemplateParentBinding(previousMonthButton, Visual.IsVisibleProperty,
            RangeCalendarItem.IsMonthViewModeProperty);
        previousMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousMonthButton, 1);
        headerLayout.Children.Add(previousMonthButton);

        var primaryHeaderButton = new HeadTextButton
        {
            Name = PrimaryHeaderButtonPart
        };
        Grid.SetColumn(primaryHeaderButton, 2);
        primaryHeaderButton.RegisterInNameScope(scope);
        headerLayout.Children.Add(primaryHeaderButton);

        var nextMonthButton = BuildNextMonthButton(PrimaryNextMonthButtonPart);
        nextMonthButton.IsVisible = false;
        nextMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextMonthButton, 3);
        headerLayout.Children.Add(nextMonthButton);

        var nextButton = BuildNextButton(PrimaryNextButtonPart);
        CreateTemplateParentBinding(nextButton, Visual.IsVisibleProperty, RangeCalendarItem.IsMonthViewModeProperty,
            BindingMode.Default,
            BoolConverters.Not);
        nextButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextButton, 4);
        headerLayout.Children.Add(nextButton);

        layout.Children.Add(headerLayout);
    }

    private void BuildSecondaryHeaderItem(UniformGrid layout, INameScope scope)
    {
        var headerLayout = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions
            {
                new(GridLength.Auto),
                new(GridLength.Auto),
                new(GridLength.Star),
                new(GridLength.Auto),
                new(GridLength.Auto)
            }
        };

        CreateTemplateParentBinding(headerLayout, Visual.IsVisibleProperty, RangeCalendarItem.IsMonthViewModeProperty);

        TokenResourceBinder.CreateTokenBinding(headerLayout, Layoutable.MarginProperty,
            CalendarTokenResourceKey.RangeCalendarSpacing, BindingPriority.Template,
            v =>
            {
                if (v is double dval)
                {
                    return new Thickness(dval, 0, 0, 0);
                }

                return new Thickness();
            });

        // 原则上这两个按钮不需要，但是可能后面会用到

        var previousButton = BuildPreviousButton(SecondaryPreviousButtonPart);
        previousButton.RegisterInNameScope(scope);
        previousButton.IsVisible = false;
        Grid.SetColumn(previousButton, 0);
        headerLayout.Children.Add(previousButton);

        var previousMonthButton = BuildPreviousMonthButton(SecondaryPreviousMonthButtonPart);
        previousMonthButton.RegisterInNameScope(scope);
        previousMonthButton.IsVisible = false;
        Grid.SetColumn(previousMonthButton, 1);
        headerLayout.Children.Add(previousMonthButton);

        var secondaryHeaderButton = new HeadTextButton
        {
            Name = SecondaryHeaderButtonPart
        };
        Grid.SetColumn(secondaryHeaderButton, 2);
        secondaryHeaderButton.RegisterInNameScope(scope);
        headerLayout.Children.Add(secondaryHeaderButton);

        var nextMonthButton = BuildNextMonthButton(SecondaryNextMonthButtonPart);
        CreateTemplateParentBinding(nextMonthButton, Visual.IsVisibleProperty,
            RangeCalendarItem.IsMonthViewModeProperty);
        nextMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextMonthButton, 3);
        headerLayout.Children.Add(nextMonthButton);

        var nextButton = BuildNextButton(SecondaryNextButtonPart);
        CreateTemplateParentBinding(nextButton, Visual.IsVisibleProperty, RangeCalendarItem.IsMonthViewModeProperty);
        nextButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextButton, 4);
        headerLayout.Children.Add(nextButton);

        layout.Children.Add(headerLayout);
    }

    protected virtual IconButton BuildPreviousButton(string name)
    {
        var previousButtonIcon = new PathIcon
        {
            Kind = "DoubleLeftOutlined"
        };
        TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorTextDescription);
        TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.ActiveFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);
        TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.SelectedFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);

        var previousButton = new IconButton
        {
            Name = name,
            Icon = previousButtonIcon
        };

        TokenResourceBinder.CreateGlobalTokenBinding(previousButton, IconButton.IconWidthProperty,
            GlobalTokenResourceKey.IconSizeSM);
        TokenResourceBinder.CreateGlobalTokenBinding(previousButton, IconButton.IconHeightProperty,
            GlobalTokenResourceKey.IconSizeSM);
        return previousButton;
    }

    protected virtual IconButton BuildPreviousMonthButton(string name)
    {
        var previousMonthButtonIcon = new PathIcon
        {
            Kind = "LeftOutlined"
        };
        TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButtonIcon, PathIcon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorTextDescription);
        TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButtonIcon, PathIcon.ActiveFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);
        TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButtonIcon, PathIcon.SelectedFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);

        var previousMonthButton = new IconButton
        {
            Name = name,
            Icon = previousMonthButtonIcon
        };

        TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButton, IconButton.IconWidthProperty,
            GlobalTokenResourceKey.IconSizeSM);
        TokenResourceBinder.CreateGlobalTokenBinding(previousMonthButton, IconButton.IconHeightProperty,
            GlobalTokenResourceKey.IconSizeSM);
        return previousMonthButton;
    }

    protected virtual IconButton BuildNextButton(string name)
    {
        var nextButtonIcon = new PathIcon
        {
            Kind = "DoubleRightOutlined"
        };
        TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorTextDescription);
        TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.ActiveFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);
        TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.SelectedFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);

        var nextButton = new IconButton
        {
            Name = name,
            Icon = nextButtonIcon
        };
        TokenResourceBinder.CreateGlobalTokenBinding(nextButton, IconButton.IconWidthProperty,
            GlobalTokenResourceKey.IconSizeSM);
        TokenResourceBinder.CreateGlobalTokenBinding(nextButton, IconButton.IconHeightProperty,
            GlobalTokenResourceKey.IconSizeSM);
        return nextButton;
    }

    protected virtual IconButton BuildNextMonthButton(string name)
    {
        var nextMonthButtonIcon = new PathIcon
        {
            Kind = "RightOutlined"
        };
        TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButtonIcon, PathIcon.NormalFilledBrushProperty,
            GlobalTokenResourceKey.ColorTextDescription);
        TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButtonIcon, PathIcon.ActiveFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);
        TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButtonIcon, PathIcon.SelectedFilledBrushProperty,
            GlobalTokenResourceKey.ColorText);
        var nextMonthButton = new IconButton
        {
            Name = name,
            Icon = nextMonthButtonIcon
        };
        TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButton, IconButton.IconWidthProperty,
            GlobalTokenResourceKey.IconSizeSM);
        TokenResourceBinder.CreateGlobalTokenBinding(nextMonthButton, IconButton.IconHeightProperty,
            GlobalTokenResourceKey.IconSizeSM);
        return nextMonthButton;
    }

    private void BuildContentView(DockPanel layout, INameScope scope)
    {
        var monthView = new UniformGrid
        {
            Name      = MonthViewPart,
            Columns   = 2,
            IsVisible = false
        };

        var primaryMonthView = BuildMonthViewItem(PrimaryMonthViewPart);
        primaryMonthView.RegisterInNameScope(scope);
        monthView.Children.Add(primaryMonthView);

        var secondaryMonthView = BuildMonthViewItem(SecondaryMonthViewPart);
        secondaryMonthView.RegisterInNameScope(scope);
        monthView.Children.Add(secondaryMonthView);

        TokenResourceBinder.CreateTokenBinding(secondaryMonthView, Layoutable.MarginProperty,
            CalendarTokenResourceKey.RangeCalendarSpacing, BindingPriority.Template,
            v =>
            {
                if (v is double dval)
                {
                    return new Thickness(dval, 0, 0, 0);
                }

                return new Thickness();
            });

        BindUtils.RelayBind(monthView, Visual.IsVisibleProperty, primaryMonthView, Visual.IsVisibleProperty);
        BindUtils.RelayBind(monthView, Visual.IsVisibleProperty, secondaryMonthView, Visual.IsVisibleProperty);

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

    private Grid BuildMonthViewItem(string name)
    {
        var dayTitleRowDef = new RowDefinition();
        TokenResourceBinder.CreateTokenBinding(dayTitleRowDef, RowDefinition.HeightProperty,
            CalendarTokenResourceKey.DayTitleHeight);
        var monthView = new Grid
        {
            Name                = name,
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
        return monthView;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        var headerLayoutStyle = new Style(selector => selector.Nesting().Template().Name(HeaderLayoutPart));
        headerLayoutStyle.Add(Layoutable.MarginProperty, CalendarTokenResourceKey.HeaderMargin);

        commonStyle.Add(headerLayoutStyle);

        Add(commonStyle);
    }
}