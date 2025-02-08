using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
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
        : this(typeof(CalendarItem))
    {
    }
    
    public CalendarItemTheme(Type targetType) : base(targetType)
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

    protected void BuildHeader(DockPanel layout, INameScope scope)
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
        NotifyConfigureHeaderLayout(headerLayout);
        NotifyBuildHeaderItems(headerLayout, scope);

        DockPanel.SetDock(headerFrame, Dock.Top);
        headerFrame.Child = headerLayout;
        layout.Children.Add(headerFrame);
    }

    protected virtual void NotifyConfigureHeaderLayout(UniformGrid headerLayout)
    {
        headerLayout.Columns = 1;
    }

    protected virtual void NotifyBuildHeaderItems(UniformGrid headerLayout, INameScope scope)
    {
        BuildHeaderItem(headerLayout, 
            PreviousButtonPart,
            PreviousMonthButtonPart,
            HeaderButtonPart,
            NextButtonPart,
            NextMonthButtonPart,
            scope);
    }

    protected void BuildHeaderItem(UniformGrid layout,
                                   string previousButtonName,
                                   string previousMonthButtonName,
                                   string headerButtonName,
                                   string nextButtonName,
                                   string nextMonthButtonName,
                                   INameScope scope)
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

        var previousButton = BuildPreviousButton(previousButtonName);
        previousButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousButton, 0);
        headerLayout.Children.Add(previousButton);

        var previousMonthButton = BuildPreviousMonthButton(previousMonthButtonName);
        CreateTemplateParentBinding(previousMonthButton, Visual.IsVisibleProperty,
            CalendarItem.IsMonthViewModeProperty);
        previousMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousMonthButton, 1);
        headerLayout.Children.Add(previousMonthButton);

        var headerButton = new HeadTextButton
        {
            Name = headerButtonName
        };
        Grid.SetColumn(headerButton, 2);
        headerButton.RegisterInNameScope(scope);
        headerLayout.Children.Add(headerButton);

        var nextMonthButton = BuildNextMonthButton(nextMonthButtonName);
        CreateTemplateParentBinding(nextMonthButton, Visual.IsVisibleProperty, CalendarItem.IsMonthViewModeProperty);
        nextMonthButton.IsVisible = false;
        nextMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextMonthButton, 3);
        headerLayout.Children.Add(nextMonthButton);

        var nextButton = BuildNextButton(nextButtonName);
        nextButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextButton, 4);
        headerLayout.Children.Add(nextButton);

        layout.Children.Add(headerLayout);
    }

    protected virtual IconButton BuildPreviousButton(string name)
    {
        var previousButtonIcon = AntDesignIconPackage.DoubleLeftOutlined();
        TokenResourceBinder.CreateSharedTokenBinding(previousButtonIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextDescription);
        TokenResourceBinder.CreateSharedTokenBinding(previousButtonIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText);
        TokenResourceBinder.CreateSharedTokenBinding(previousButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText);

        var previousButton = new IconButton
        {
            Name = name,
            Icon = previousButtonIcon
        };

        TokenResourceBinder.CreateSharedTokenBinding(previousButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSizeSM);
        TokenResourceBinder.CreateSharedTokenBinding(previousButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSizeSM);
        return previousButton;
    }

    protected virtual IconButton BuildPreviousMonthButton(string name)
    {
        var previousMonthButtonIcon = AntDesignIconPackage.LeftOutlined();
        TokenResourceBinder.CreateSharedTokenBinding(previousMonthButtonIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextDescription);
        TokenResourceBinder.CreateSharedTokenBinding(previousMonthButtonIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText);
        TokenResourceBinder.CreateSharedTokenBinding(previousMonthButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText);

        var previousMonthButton = new IconButton
        {
            Name = name,
            Icon = previousMonthButtonIcon
        };

        TokenResourceBinder.CreateSharedTokenBinding(previousMonthButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSizeSM);
        TokenResourceBinder.CreateSharedTokenBinding(previousMonthButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSizeSM);
        return previousMonthButton;
    }

    protected virtual IconButton BuildNextButton(string name)
    {
        var nextButtonIcon = AntDesignIconPackage.DoubleRightOutlined();
        TokenResourceBinder.CreateSharedTokenBinding(nextButtonIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextDescription);
        TokenResourceBinder.CreateSharedTokenBinding(nextButtonIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText);
        TokenResourceBinder.CreateSharedTokenBinding(nextButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText);

        var nextButton = new IconButton
        {
            Name = name,
            Icon = nextButtonIcon
        };
        TokenResourceBinder.CreateSharedTokenBinding(nextButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSizeSM);
        TokenResourceBinder.CreateSharedTokenBinding(nextButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSizeSM);
        return nextButton;
    }

    protected virtual IconButton BuildNextMonthButton(string name)
    {
        var nextMonthButtonIcon = AntDesignIconPackage.RightOutlined();
        TokenResourceBinder.CreateSharedTokenBinding(nextMonthButtonIcon, Icon.NormalFilledBrushProperty,
            SharedTokenKey.ColorTextDescription);
        TokenResourceBinder.CreateSharedTokenBinding(nextMonthButtonIcon, Icon.ActiveFilledBrushProperty,
            SharedTokenKey.ColorText);
        TokenResourceBinder.CreateSharedTokenBinding(nextMonthButtonIcon, Icon.SelectedFilledBrushProperty,
            SharedTokenKey.ColorText);
        var nextMonthButton = new IconButton
        {
            Name = name,
            Icon = nextMonthButtonIcon
        };
        TokenResourceBinder.CreateSharedTokenBinding(nextMonthButton, IconButton.IconWidthProperty,
            SharedTokenKey.IconSizeSM);
        TokenResourceBinder.CreateSharedTokenBinding(nextMonthButton, IconButton.IconHeightProperty,
            SharedTokenKey.IconSizeSM);
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
        monthViewLayout.RegisterInNameScope(scope);

        NotifyConfigureMonthViewLayout(monthViewLayout, scope);
        NotifyBuildMonthViews(monthViewLayout, scope);
        
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
    
    protected virtual void NotifyConfigureMonthViewLayout(UniformGrid monthViewLayout, INameScope scope)
    {
        monthViewLayout.Columns = 1;
    }

    protected virtual void NotifyBuildMonthViews(UniformGrid monthViewLayout, INameScope scope)
    {
        var monthView = BuildMonthViewItem(MonthViewPart);
        BindUtils.RelayBind(monthViewLayout, Visual.IsVisibleProperty, monthView, Visual.IsVisibleProperty);
        monthView.RegisterInNameScope(scope);
        monthViewLayout.Children.Add(monthView);
    }
    
    protected Grid BuildMonthViewItem(string name)
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

        commonStyle.Add(CalendarItem.MinHeightProperty, DatePickerTokenKey.ItemPanelMinHeight);
        commonStyle.Add(CalendarItem.MinWidthProperty, DatePickerTokenKey.ItemPanelMinWidth);
        
        var headerFrameStyle = new Style(selector => selector.Nesting().Template().Name(HeaderFramePart));
        headerFrameStyle.Add(Border.MarginProperty, DatePickerTokenKey.HeaderMargin);
        headerFrameStyle.Add(Border.PaddingProperty, DatePickerTokenKey.HeaderPadding);
        headerFrameStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorBorderSecondary);

        commonStyle.Add(headerFrameStyle);

        Add(commonStyle);
    }
}