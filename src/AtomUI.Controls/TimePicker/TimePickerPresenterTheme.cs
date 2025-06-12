using AtomUI.Controls.DatePickerLang;
using AtomUI.Controls.Localization;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimePickerPresenterTheme : BaseControlTheme
{
    public const string MainLayoutPart = "PART_MainLayout";
    public const string NowButtonPart = "PART_NowButton";
    public const string ConfirmButtonPart = "PART_ConfirmButton";
    public const string ButtonsLayoutPart = "PART_ButtonsLayout";
    public const string ButtonsFramePart = "PART_ButtonsFrame";
    public const string TimeViewPart = "PART_TimeView";

    public TimePickerPresenterTheme()
        : base(typeof(TimePickerPresenter))
    {
    }

    protected override IControlTemplate BuildControlTemplate()
    {
        return new FuncControlTemplate<TimePickerPresenter>((presenter, scope) =>
        {
            var mainLayout = new DockPanel()
            {
                Name          = MainLayoutPart,
                LastChildFill = true
            };

            var calendarView = BuildTimeView(presenter, scope);
            
            var buttonsContainerFrame = new Border()
            {
                Name = ButtonsFramePart
            };
            CreateTemplateParentBinding(buttonsContainerFrame, Border.BorderThicknessProperty, TimePickerPresenter.BorderThicknessProperty);
            var buttonsPanel          = BuildButtons(presenter, scope);
            CreateTemplateParentBinding(buttonsContainerFrame, Border.IsVisibleProperty, TimePickerPresenter.ButtonsPanelVisibleProperty);
            buttonsContainerFrame.Child = buttonsPanel;
            
            DockPanel.SetDock(buttonsContainerFrame, Dock.Bottom);
            mainLayout.Children.Add(buttonsContainerFrame);
            mainLayout.Children.Add(calendarView);
            return mainLayout;
        });
    }
    
      protected virtual Control BuildTimeView(TimePickerPresenter presenter, INameScope scope)
    {
        var timeView = new TimeView()
        {
            Name = TimeViewPart,
            IsShowHeader = false
        };
        CreateTemplateParentBinding(timeView, TimeView.IsMotionEnabledProperty, TimePickerPresenter.IsMotionEnabledProperty);
        CreateTemplateParentBinding(timeView, TimeView.MinuteIncrementProperty, TimePickerPresenter.MinuteIncrementProperty);
        CreateTemplateParentBinding(timeView, TimeView.SecondIncrementProperty, TimePickerPresenter.SecondIncrementProperty);
        CreateTemplateParentBinding(timeView, TimeView.SelectedTimeProperty, TimePickerPresenter.SelectedTimeProperty);
        CreateTemplateParentBinding(timeView, TimeView.ClockIdentifierProperty, TimePickerPresenter.ClockIdentifierProperty);
        timeView.RegisterInNameScope(scope);
        return timeView;
    }

    protected virtual Panel BuildButtons(TimePickerPresenter presenter, INameScope scope)
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
        LanguageResourceBinder.CreateBinding(nowButton, Button.ContentProperty, DatePickerLangResourceKey.Now);
        nowButton.RegisterInNameScope(scope);
        buttonsPanel.Children.Add(nowButton);

        var confirmButton = new Button
        {
            Name                = ConfirmButtonPart,
            ButtonType          = ButtonType.Primary,
            SizeType            = SizeType.Small,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        LanguageResourceBinder.CreateBinding(confirmButton, Button.ContentProperty, CommonLangResourceKey.OkText);
        confirmButton.RegisterInNameScope(scope);
        buttonsPanel.Children.Add(confirmButton);
        
        return buttonsPanel;
    }
    
    protected override void BuildStyles()
    {
        var commonStyle       = new Style(selector => selector.Nesting());
        
        var buttonsFrameStyle = new Style(selector => selector.Nesting().Template().Name(ButtonsFramePart));
        buttonsFrameStyle.Add(Border.BorderBrushProperty, SharedTokenKey.ColorBorderSecondary);
        commonStyle.Add(buttonsFrameStyle);

        var buttonsPanelStyle = new Style(selector => selector.Nesting().Template().Name(ButtonsLayoutPart));
        buttonsPanelStyle.Add(Panel.MarginProperty, DatePickerTokenKey.ButtonsPanelMargin);
        commonStyle.Add(buttonsPanelStyle);
        
        Add(commonStyle);
    }
}