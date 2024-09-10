using AtomUI.Controls.Localization;
using AtomUI.Controls.TimePickerLang;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimePickerFlyoutPresenterTheme : ArrowDecoratedBoxTheme
{
    public const string ContentLayoutPart = "PART_ContentLayout";
    public const string NowButtonPart = "PART_NowButton";
    public const string ConfirmButtonPart = "PART_ConfirmButton";
    public const string ButtonsContainerPart = "PART_ButtonsContainer";

    public TimePickerFlyoutPresenterTheme()
        : base(typeof(TimePickerFlyoutPresenter))
    {
    }

    protected override Control BuildContent(INameScope scope)
    {
        var contentLayout = new DockPanel
        {
            Name = ContentLayoutPart
        };

        var contentPresenter = new TimePickerPresenter
        {
            Name         = ContentPresenterPart,
            IsShowHeader = false
        };

        CreateTemplateParentBinding(contentPresenter, TimePickerPresenter.MinuteIncrementProperty,
            TimePickerFlyoutPresenter.MinuteIncrementProperty);
        CreateTemplateParentBinding(contentPresenter, TimePickerPresenter.SecondIncrementProperty,
            TimePickerFlyoutPresenter.SecondIncrementProperty);
        CreateTemplateParentBinding(contentPresenter, TimePickerPresenter.ClockIdentifierProperty,
            TimePickerFlyoutPresenter.ClockIdentifierProperty);
        CreateTemplateParentBinding(contentPresenter, TimePickerPresenter.TimeProperty,
            TimePickerFlyoutPresenter.TimeProperty);
        contentPresenter.RegisterInNameScope(scope);

        var buttons = new Panel
        {
            Name = ButtonsContainerPart
        };
        DockPanel.SetDock(buttons, Dock.Bottom);
        var nowButton = new Button
        {
            ButtonType          = ButtonType.Link,
            Name                = NowButtonPart,
            HorizontalAlignment = HorizontalAlignment.Left,
            SizeType            = SizeType.Small
        };
        LanguageResourceBinder.CreateBinding(nowButton, Button.TextProperty, TimePickerLangResourceKey.Now);
        nowButton.RegisterInNameScope(scope);
        buttons.Children.Add(nowButton);

        var confirmButton = new Button
        {
            Name                = ConfirmButtonPart,
            ButtonType          = ButtonType.Primary,
            SizeType            = SizeType.Small,
            HorizontalAlignment = HorizontalAlignment.Right
        };
        LanguageResourceBinder.CreateBinding(confirmButton, Button.TextProperty, CommonLangResourceKey.OkText);
        confirmButton.RegisterInNameScope(scope);
        buttons.Children.Add(confirmButton);

        contentLayout.Children.Add(buttons);
        contentLayout.Children.Add(contentPresenter);

        return contentLayout;
    }

    protected override void BuildStyles()
    {
        base.BuildStyles();
        var buttonsContainerStyle = new Style(selector => selector.Nesting().Template().Name(ButtonsContainerPart));
        buttonsContainerStyle.Add(Layoutable.MarginProperty, TimePickerTokenResourceKey.ButtonsMargin);
        Add(buttonsContainerStyle);

        var contentPresenterStyle = new Style(selector => selector.Nesting().Template().Name(ContentPresenterPart));
        contentPresenterStyle.Add(Layoutable.MaxWidthProperty, TimePickerTokenResourceKey.PickerPopupWidth);
        contentPresenterStyle.Add(Layoutable.HeightProperty, TimePickerTokenResourceKey.PickerPopupHeight);
        Add(contentPresenterStyle);
    }
}