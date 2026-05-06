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
        locator.Map<FloatButtonViewModel, FloatButtonShowCase>(() => new FloatButtonShowCase());
        locator.Map<SplitButtonViewModel, SplitButtonShowCase>(() => new SplitButtonShowCase());
        locator.Map<SeparatorViewModel, SeparatorShowCase>(() => new SeparatorShowCase());
        locator.Map<SplitterViewModel, SplitterShowCase>(() => new SplitterShowCase());
        locator.Map<GridViewModel, GridShowCase>(() => new GridShowCase());
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
        locator.Map<EmptyViewModel, EmptyShowCase>(() => new EmptyShowCase());
        locator.Map<ExpanderViewModel, ExpanderShowCase>(() => new ExpanderShowCase());
        locator.Map<CheckBoxViewModel, CheckBoxShowCase>(() => new CheckBoxShowCase());
        locator.Map<ToggleSwitchViewModel, ToggleSwitchShowCase>(() => new ToggleSwitchShowCase());
        locator.Map<RadioButtonViewModel, RadioButtonShowCase>(() => new RadioButtonShowCase());
        locator.Map<RateViewModel, RateShowCase>(() => new RateShowCase());
        locator.Map<SliderViewModel, SliderShowCase>(() => new SliderShowCase());
        locator.Map<TagViewModel, TagShowCase>(() => new TagShowCase());
        locator.Map<SegmentedViewModel, SegmentedShowCase>(() => new SegmentedShowCase());
        locator.Map<GroupBoxViewModel, GroupBoxShowCase>(() => new GroupBoxShowCase());
        locator.Map<ResultViewModel, ResultShowCase>(() => new ResultShowCase());
        locator.Map<BadgeViewModel, BadgeShowCase>(() => new BadgeShowCase());
        locator.Map<TimelineViewModel, TimelineShowCase>(() => new TimelineShowCase());
        locator.Map<QRCodeViewModel, QRCodeShowCase>(() => new QRCodeShowCase());
        locator.Map<LineEditViewModel, LineEditShowCase>(() => new LineEditShowCase());
        locator.Map<AutoCompleteViewModel, AutoCompleteShowCase>(() => new AutoCompleteShowCase());
        locator.Map<SelectViewModel, SelectShowCase>(() => new SelectShowCase());
        locator.Map<CascaderViewModel, CascaderShowCase>(() => new CascaderShowCase());
        locator.Map<TreeSelectViewModel, TreeSelectShowCase>(() => new TreeSelectShowCase());
        locator.Map<TabControlViewModel, TabControlShowCase>(() => new TabControlShowCase());
        locator.Map<InfoFlyoutViewModel, InfoFlyoutShowCase>(() => new InfoFlyoutShowCase());
        locator.Map<WatermarkViewModel, WatermarkShowCase>(() => new WatermarkShowCase());
        locator.Map<ProgressBarViewModel, ProgressBarShowCase>(() => new ProgressBarShowCase());
        locator.Map<PaginationViewModel, PaginationShowCase>(() => new PaginationShowCase());
        locator.Map<MenuViewModel, MenuShowCase>(() => new MenuShowCase());
        locator.Map<TimePickerViewModel, TimePickerShowCase>(() => new TimePickerShowCase());
        locator.Map<DatePickerViewModel, DatePickerShowCase>(() => new DatePickerShowCase());
        locator.Map<NumberUpDownViewModel, NumberUpDownShowCase>(() => new NumberUpDownShowCase());
        locator.Map<ListViewModel, ListShowCase>(() => new ListShowCase());
        locator.Map<TreeViewViewModel, TreeViewShowCase>(() => new TreeViewShowCase());
        locator.Map<TourViewModel, TourShowCase>(() => new TourShowCase());
    }
}
