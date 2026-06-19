using ReactiveUI;

namespace AtomUIGallery.ShowCases;

public sealed class ShowCaseViewModule : IViewModule
{
    public void RegisterViews(DefaultViewLocator locator)
    {
        AtomUIGalleryModule.RegisterViews(locator);
    }
}
