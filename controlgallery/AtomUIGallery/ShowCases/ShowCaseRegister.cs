using AtomUIGallery.ShowCases.ViewModels;
using AtomUIGallery.ShowCases.Views;
using ReactiveUI;

namespace AtomUIGallery.ShowCases;

public sealed class ShowCaseViewModule : IViewModule
{
    public void RegisterViews(DefaultViewLocator locator)
    {
        locator.Map<AboutUsViewModel, AboutUsPage>(() => new AboutUsPage());
        locator.Map<PaletteViewModel, PaletteShowCase>(() => new PaletteShowCase());
        locator.Map<IconViewModel, IconShowCase>(() => new IconShowCase());
        locator.Map<ButtonViewModel, ButtonShowCase>(() => new ButtonShowCase());
        locator.Map<SplitButtonViewModel, SplitButtonShowCase>(() => new SplitButtonShowCase());
        locator.Map<SeparatorViewModel, SeparatorShowCase>(() => new SeparatorShowCase());
        locator.Map<SplitterViewModel, SplitterShowCase>(() => new SplitterShowCase());
        locator.Map<BreadcrumbViewModel, BreadcrumbShowCase>(() => new BreadcrumbShowCase());
        locator.Map<ButtonSpinnerViewModel, ButtonSpinnerShowCase>(() => new ButtonSpinnerShowCase());
        locator.Map<ComboBoxViewModel, ComboBoxShowCase>(() => new ComboBoxShowCase());
        locator.Map<DropdownButtonViewModel, DropdownButtonShowCase>(() => new DropdownButtonShowCase());
        locator.Map<MessageViewModel, MessageShowCase>(() => new MessageShowCase());
        locator.Map<NotificationViewModel, NotificationShowCase>(() => new NotificationShowCase());
        locator.Map<PopupConfirmViewModel, PopupConfirmShowCase>(() => new PopupConfirmShowCase());
        locator.Map<AlertViewModel, AlertShowCase>(() => new AlertShowCase());
        locator.Map<SpinViewModel, SpinShowCase>(() => new SpinShowCase());
        locator.Map<FlexPanelViewModel, FlexPanelShowCase>(() => new FlexPanelShowCase());
        locator.Map<CarouselViewModel, CarouselShowCase>(() => new CarouselShowCase());
        locator.Map<AvatarViewModel, AvatarShowCase>(() => new AvatarShowCase());
        locator.Map<CalendarViewModel, CalendarShowCase>(() => new CalendarShowCase());
        locator.Map<SkeletonViewModel, SkeletonShowCase>(() => new SkeletonShowCase());
        locator.Map<CardViewModel, CardShowCase>(() => new CardShowCase());
        locator.Map<CollapseViewModel, CollapseShowCase>(() => new CollapseShowCase());
        locator.Map<CheckBoxViewModel, CheckBoxShowCase>(() => new CheckBoxShowCase());
        locator.Map<ToggleSwitchViewModel, ToggleSwitchShowCase>(() => new ToggleSwitchShowCase());
        locator.Map<RadioButtonViewModel, RadioButtonShowCase>(() => new RadioButtonShowCase());
        locator.Map<RateViewModel, RateShowCase>(() => new RateShowCase());
    }
}
