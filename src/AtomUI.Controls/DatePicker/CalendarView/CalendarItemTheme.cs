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
using Avalonia.Data;
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

    protected void BuildHeader(CalendarItem calendarItem, DockPanel layout, INameScope scope)
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
        NotifyConfigureHeaderLayout(calendarItem, headerLayout);
        NotifyBuildHeaderItems(calendarItem, headerLayout, scope);

        DockPanel.SetDock(headerFrame, Dock.Top);
        headerFrame.Child = headerLayout;
        layout.Children.Add(headerFrame);
    }

    protected virtual void NotifyConfigureHeaderLayout(CalendarItem calendarItem, UniformGrid headerLayout)
    {
        headerLayout.Columns = 1;
    }

    protected virtual void NotifyBuildHeaderItems(CalendarItem calendarItem, UniformGrid headerLayout, INameScope scope)
    {
        BuildHeaderItem(calendarItem,
            headerLayout,
            PreviousButtonPart,
            PreviousMonthButtonPart,
            HeaderButtonPart,
            NextButtonPart,
            NextMonthButtonPart,
            scope);
    }

    protected void BuildHeaderItem(CalendarItem calendarItem, 
                                   UniformGrid layout,
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

        var previousButton = BuildPreviousButton(calendarItem, previousButtonName);
        previousButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousButton, 0);
        headerLayout.Children.Add(previousButton);

        var previousMonthButton = BuildPreviousMonthButton(calendarItem, previousMonthButtonName);
        CreateTemplateParentBinding(previousMonthButton, Visual.IsVisibleProperty,
            CalendarItem.IsMonthViewModeProperty);
        previousMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(previousMonthButton, 1);
        headerLayout.Children.Add(previousMonthButton);

        var headerButton = new HeadTextButton
        {
            Name = headerButtonName
        };
        CreateTemplateParentBinding(headerButton, HeadTextButton.IsMotionEnabledProperty,
            CalendarItem.IsMotionEnabledProperty);
        Grid.SetColumn(headerButton, 2);
        headerButton.RegisterInNameScope(scope);
        headerLayout.Children.Add(headerButton);

        var nextMonthButton = BuildNextMonthButton(calendarItem, nextMonthButtonName);
        CreateTemplateParentBinding(nextMonthButton, Visual.IsVisibleProperty, CalendarItem.IsMonthViewModeProperty);
        nextMonthButton.IsVisible = false;
        nextMonthButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextMonthButton, 3);
        headerLayout.Children.Add(nextMonthButton);

        var nextButton = BuildNextButton(calendarItem, nextButtonName);
        nextButton.RegisterInNameScope(scope);
        Grid.SetColumn(nextButton, 4);
        headerLayout.Children.Add(nextButton);

        layout.Children.Add(headerLayout);
    }

    protected virtual IconButton BuildPreviousButton(CalendarItem calendarItem, string name)
    {
        var previousButtonIcon = AntDesignIconPackage.DoubleLeftOutlined();
        var previousButton = new IconButton
        {
            Name = name,
            Icon = previousButtonIcon
        };
        
        return previousButton;
    }

    protected virtual IconButton BuildPreviousMonthButton(CalendarItem calendarItem, string name)
    {
        var previousMonthButtonIcon = AntDesignIconPackage.LeftOutlined();
      
        var previousMonthButton = new IconButton
        {
            Name = name,
            Icon = previousMonthButtonIcon
        };
        
        return previousMonthButton;
    }

    protected virtual IconButton BuildNextButton(CalendarItem calendarItem, string name)
    {
        var nextButtonIcon = AntDesignIconPackage.DoubleRightOutlined();
        
        var nextButton = new IconButton
        {
            Name = name,
            Icon = nextButtonIcon
        };
        
        return nextButton;
    }

    protected virtual IconButton BuildNextMonthButton(CalendarItem calendarItem, string name)
    {
        var nextMonthButtonIcon = AntDesignIconPackage.RightOutlined();
      
        var nextMonthButton = new IconButton
        {
            Name = name,
            Icon = nextMonthButtonIcon
        };
        
        return nextMonthButton;
    }

    private void BuildContentView(CalendarItem calendarItem, DockPanel layout, INameScope scope)
    {
        var monthViewLayout = new UniformGrid
        {
            Name      = MonthViewLayoutPart,
            Columns   = 1,
            IsVisible = false
        };
        monthViewLayout.RegisterInNameScope(scope);

        NotifyConfigureMonthViewLayout(calendarItem, monthViewLayout, scope);
        NotifyBuildMonthViews(calendarItem, monthViewLayout, scope);

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

    protected virtual void NotifyConfigureMonthViewLayout(CalendarItem calendarItem, UniformGrid monthViewLayout,
                                                          INameScope scope)
    {
        monthViewLayout.Columns = 1;
    }

    protected virtual void NotifyBuildMonthViews(CalendarItem calendarItem, UniformGrid monthViewLayout,
                                                 INameScope scope)
    {
        var monthView = BuildMonthViewItem(MonthViewPart);
        // 不会造成内存泄漏
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
        BuildButtonsStyles(commonStyle);
        Add(commonStyle);
    }
    
    private void BuildButtonsStyles(Style commonStyle)
    {
        var buttonsStyle = new Style(selector => Selectors.Or(selector.Nesting().Template().Name(PreviousButtonPart),
            selector.Nesting().Template().Name(PreviousMonthButtonPart),
            selector.Nesting().Template().Name(NextButtonPart),
            selector.Nesting().Template().Name(NextMonthButtonPart)));
        buttonsStyle.Add(IconButton.IconHeightProperty, SharedTokenKey.IconSizeSM);
        buttonsStyle.Add(IconButton.IconWidthProperty, SharedTokenKey.IconSizeSM);
        buttonsStyle.Add(IconButton.NormalIconBrushProperty, SharedTokenKey.ColorTextDescription);
        buttonsStyle.Add(IconButton.ActiveIconBrushProperty, SharedTokenKey.ColorText);
        buttonsStyle.Add(IconButton.SelectedIconBrushProperty, SharedTokenKey.ColorText);
        commonStyle.Add(buttonsStyle);
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