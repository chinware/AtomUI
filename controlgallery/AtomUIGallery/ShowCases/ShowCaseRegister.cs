using ReactiveUI;

namespace AtomUIGallery.ShowCases;

public sealed class ShowCaseViewModule : IViewModule
{
    public void RegisterViews(DefaultViewLocator locator)
    {
        // ShowCase View <-> ViewModel mappings will be registered here as they are added.
        // Example:
        // locator.Map<SomeViewModel, SomeShowCase>(() => new SomeShowCase());
    }
}
