using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls.CalendarView;

[ControlThemeProvider]
internal class CalendarItemTheme : BaseControlTheme
{
    public const string ItemFramePart = "PART_ItemFrame";
    public const string ItemRootLayoutPart = "PART_ItemRootLayout";
    public const string MonthViewLayoutPart = "PART_MonthViewLayout";
    public const string MonthViewPart = "PART_MonthView";
    public const string YearViewPart = "PART_YearView";
    public const string HeaderLayoutPart = "PART_HeaderLayout";
    public const string HeaderFramePart = "PART_HeaderFrame";

    public const string PreviousButtonPart = "PART_PreviousButton";
    public const string PreviousMonthButtonPart = "PART_PreviousMonthButton";
    public const string HeaderButtonPart = "PART_HeaderButton";
    public const string NextMonthButtonPart = "PART_NextMonthButton";
    public const string NextButtonPart = "PART_NextButton";

    public CalendarItemTheme()
        : base(typeof(CalendarItem))
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

            CreateTemplateParentBinding(frame, Border.MinWidthProperty, CalendarItem.MinWidthProperty);
            CreateTemplateParentBinding(frame, Border.MinHeightProperty, CalendarItem.MinHeightProperty);

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

    private void BuildDayTitleTemplate(CalendarItem calendarItem)
    {
        calendarItem.DayTitleTemplate = new DayTitleTemplate();
    }

    protected virtual void BuildHeader(DockPanel layout, INameScope scope)
    {
        var headerFrame = new Border()
        {
            Name = HeaderFramePart
        };
        CreateTemplateParentBinding(headerFrame, Border.BorderThicknessProperty, CalendarItem.BorderThicknessProperty);
        
        var headerLayout = new UniformGrid
        {
            Name    = HeaderLayoutPart,
            Columns = 1
        };

        headerLayout.RegisterInNameScope(scope);
        BuildHeaderItem(headerLayout, scope);

        DockPanel.SetDock(headerFrame, Dock.Top);
        headerFrame.Child = headerLayout;
        layout.Children.Add(headerFrame);
    }

    private void BuildHeaderItem(UniformGrid layout, INameScope scope)
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

        var previousButton = BuildPreviousButton(PreviousButtonPart);
        previousButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousButton, 0);
        headerLayout.Children.Add(previousButton);

        var previousMonthButton = BuildPreviousMonthButton(PreviousMonthButtonPart);
        CreateTemplateParentBinding(previousMonthButton, Visual.IsVisibleProperty,
            CalendarItem.IsMonthViewModeProperty);
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

        var nextMonthButton = BuildNextMonthButton(NextMonthButtonPart);
        CreateTemplateParentBinding(nextMonthButton, Visual.IsVisibleProperty, CalendarItem.IsMonthViewModeProperty);
        nextMonthButton.IsVisible = false;
        nextMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextMonthButton, 3);
        headerLayout.Children.Add(nextMonthButton);

        var nextButton = BuildNextButton(NextButtonPart);
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
        var monthViewLayout = new UniformGrid
        {
            Name      = MonthViewLayoutPart,
            Columns   = 1,
            IsVisible = false
        };

        var monthView = BuildMonthViewItem(MonthViewPart);
        monthView.RegisterInNameScope(scope);
        monthViewLayout.Children.Add(monthView);
        
        BindUtils.RelayBind(monthViewLayout, Visual.IsVisibleProperty, monthView, Visual.IsVisibleProperty);

        monthViewLayout.RegisterInNameScope(scope);
        layout.Children.Add(monthViewLayout);

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
        var monthViewLayout = new Grid
        {
            Name                = name,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsVisible           = false,
            RowDefinitions = new RowDefinitions
            {
                new(GridLength.Auto),
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
        return monthViewLayout;
    }

    protected override void BuildStyles()
    {
        var commonStyle = new Style(selector => selector.Nesting());

        commonStyle.Add(CalendarItem.MinHeightProperty, DatePickerTokenResourceKey.ItemPanelMinHeight);
        commonStyle.Add(CalendarItem.MinWidthProperty, DatePickerTokenResourceKey.ItemPanelMinWidth);
        var headerFrameStyle = new Style(selector => selector.Nesting().Template().Name(HeaderFramePart));
        headerFrameStyle.Add(Border.MarginProperty, DatePickerTokenResourceKey.HeaderMargin);
        headerFrameStyle.Add(Border.PaddingProperty, DatePickerTokenResourceKey.HeaderPadding);
        headerFrameStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorBorderSecondary);

        commonStyle.Add(headerFrameStyle);

        Add(commonStyle);
    }
}