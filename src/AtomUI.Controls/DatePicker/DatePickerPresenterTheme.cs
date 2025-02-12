using AtomUI.Controls.DatePickerLang;
using AtomUI.Controls.Localization;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;
using PickerCalendar = AtomUI.Controls.CalendarView.Calendar;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class DatePickerPresenterTheme : BaseControlTheme
{
    public const string MainLayoutPart = "PART_MainLayout";
    public const string NowButtonPart = "PART_NowButton";
    public const string TodayButtonPart = "PART_TodayButton";
    public const string ConfirmButtonPart = "PART_ConfirmButton";
    public const string ButtonsLayoutPart = "PART_ButtonsLayout";
    public const string ButtonsFramePart = "PART_ButtonsFrame";
    public const string CalendarViewPart = "PART_CalendarView";
    public const string TimeViewPart = "PART_TimeView";
    
    public DatePickerPresenterTheme() : this(typeof(DatePickerPresenter))
    {
    }

    public DatePickerPresenterTheme(Type targetType) : base(targetType)
    {
    }
    
    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<DatePickerPresenter>((presenter, scope) =>
        {
            var mainLayout = new DockPanel()
            {
                Name = MainLayoutPart,
                LastChildFill = true
            };

            var calendarView = BuildCalendarView(presenter, scope);

            var buttonsContainerFrame = new Border()
            {
                Name = ButtonsFramePart
            };
            CreateTemplateParentBinding(buttonsContainerFrame, Border.BorderThicknessProperty, DatePickerPresenter.BorderThicknessProperty);
            var buttonsPanel          = BuildButtons(presenter, scope);
            CreateTemplateParentBinding(buttonsContainerFrame, Border.IsVisibleProperty, DatePickerPresenter.ButtonsPanelVisibleProperty);
            buttonsContainerFrame.Child = buttonsPanel;
            
            DockPanel.SetDock(buttonsContainerFrame, Dock.Bottom);
            mainLayout.Children.Add(buttonsContainerFrame);
            mainLayout.Children.Add(calendarView);
            return mainLayout;
        });
    }

    protected virtual Control BuildCalendarView(DatePickerPresenter presenter, INameScope scope)
    {
        var calendarLayout = new StackPanel()
        {
            Orientation = Orientation.Horizontal
        };
        var calendarView = new PickerCalendar()
        {
            Name = CalendarViewPart,
        };
        CreateTemplateParentBinding(calendarView, PickerCalendar.SelectedDateProperty, DatePickerPresenter.SelectedDateTimeProperty);
        CreateTemplateParentBinding(calendarView, PickerCalendar.IsMotionEnabledProperty, DatePickerPresenter.IsMotionEnabledProperty);
        calendarView.RegisterInNameScope(scope);
        calendarLayout.Children.Add(calendarView);

        var timeView = new TimeView()
        {
            Name   = TimeViewPart,
            VerticalAlignment = VerticalAlignment.Top,
        };
        CreateTemplateParentBinding(timeView, TimeView.ClockIdentifierProperty, DatePickerPresenter.ClockIdentifierProperty);
        CreateTemplateParentBinding(timeView, TimeView.IsVisibleProperty, DatePickerPresenter.IsShowTimeProperty);
        timeView.RegisterInNameScope(scope);
        calendarLayout.Children.Add(timeView);
        
        return calendarLayout;
    }

    protected virtual Panel BuildButtons(DatePickerPresenter presenter, INameScope scope)
    {
        var buttonsPanel = new Panel()
        {
            Name = ButtonsLayoutPart
        };
        
        var nowButton = new Button
        {
            ButtonType          = ButtonType.Link,
            Name                = NowButtonPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            SizeType            = SizeType.Small
        };
        LanguageResourceBinder.CreateBinding(nowButton, Button.TextProperty, DatePickerLangResourceKey.Now);
        nowButton.RegisterInNameScope(scope);
        buttonsPanel.Children.Add(nowButton);
        
        var todayButton = new Button
        {
            ButtonType          = ButtonType.Link,
            Name                = TodayButtonPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            SizeType            = SizeType.Small
        };
        LanguageResourceBinder.CreateBinding(todayButton, Button.TextProperty, DatePickerLangResourceKey.Today);
        todayButton.RegisterInNameScope(scope);
        buttonsPanel.Children.Add(todayButton);

        var confirmButton = new Button
        {
            Name                = ConfirmButtonPart,
            ButtonType          = ButtonType.Primary,
            SizeType            = SizeType.Small,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        LanguageResourceBinder.CreateBinding(confirmButton, Button.TextProperty, CommonLangResourceKey.OkText);
        confirmButton.RegisterInNameScope(scope);
        buttonsPanel.Children.Add(confirmButton);
        
        return buttonsPanel;
    }
    
    protected override void BuildStyles()
    {
        var buttonsFrameStyle = new Style(selector => selector.Nesting().Template().Name(ButtonsFramePart));
        buttonsFrameStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorBorderSecondary);
        Add(buttonsFrameStyle);

        var buttonsPanelStyle = new Style(selector => selector.Nesting().Template().Name(ButtonsLayoutPart));
        buttonsPanelStyle.Add(Panel.MarginProperty, DatePickerTokenKey.ButtonsPanelMargin);
        Add(buttonsPanelStyle);

        var timeViewStyle = new Style(selector => selector.Nesting().Template().Name(TimeViewPart));
        timeViewStyle.Add(TimePicker.PaddingProperty, DatePickerTokenKey.PanelContentPadding);
        Add(timeViewStyle);
    }
}