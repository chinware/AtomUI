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
        locator.Map<SplitterViewModel, SplitterShowCase>(() => new SplitterShowCase());
        locator.Map<BreadcrumbViewModel, BreadcrumbShowCase>(() => new BreadcrumbShowCase());
        locator.Map<MessageViewModel, MessageShowCase>(() => new MessageShowCase());
        locator.Map<NotificationViewModel, NotificationShowCase>(() => new NotificationShowCase());
        locator.Map<PopupConfirmViewModel, PopupConfirmShowCase>(() => new PopupConfirmShowCase());
    }
}
