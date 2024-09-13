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
    public const string ConfirmButtonPart = "PART_ConfirmButton";
    public const string ButtonsContainerPart = "PART_ButtonsContainer";
    public const string ButtonsContainerFramePart = "PART_ButtonsContainerFrame";
    public const string CalendarViewPart = "PART_CalendarView";
    
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
                Name = ButtonsContainerFramePart
            };
            CreateTemplateParentBinding(buttonsContainerFrame, Border.BorderThicknessProperty, DatePickerPresenter.BorderThicknessProperty);
            var buttonsPanel          = BuildButtons(presenter, scope);
            buttonsContainerFrame.Child = buttonsPanel;
            
            DockPanel.SetDock(buttonsContainerFrame, Dock.Bottom);
            mainLayout.Children.Add(buttonsContainerFrame);
            mainLayout.Children.Add(calendarView);
            return mainLayout;
        });
    }

    protected virtual Control BuildCalendarView(DatePickerPresenter presenter, INameScope scope)
    {
        return new PickerCalendar()
        {
            Name = CalendarViewPart
        };
    }

    protected virtual Panel BuildButtons(DatePickerPresenter presenter, INameScope scope)
    {
        var buttonsPanel = new Panel()
        {
            Name = ButtonsContainerPart
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
        var buttonsFrameStyle = new Style(selector => selector.Nesting().Template().Name(ButtonsContainerFramePart));
        buttonsFrameStyle.Add(Border.BorderBrushProperty, GlobalTokenResourceKey.ColorBorderSecondary);
        Add(buttonsFrameStyle);

        var buttonsPanelStyle = new Style(selector => selector.Nesting().Template().Name(ButtonsContainerPart));
        buttonsPanelStyle.Add(Panel.MarginProperty, DatePickerTokenResourceKey.ButtonsPanelMargin);
        Add(buttonsPanelStyle);
    }
}