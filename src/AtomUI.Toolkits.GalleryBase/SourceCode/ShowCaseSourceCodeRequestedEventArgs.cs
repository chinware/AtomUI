using Avalonia.Interactivity;

namespace AtomUI.Toolkits.GalleryBase.SourceCode;

public sealed class ShowCaseSourceCodeRequestedEventArgs : RoutedEventArgs
{
    public ShowCaseSourceCodeRequestedEventArgs(RoutedEvent routedEvent,
                                                ShowCaseCodeSnippetKey key,
                                                string? title)
        : base(routedEvent)
    {
        Key   = key;
        Title = title;
    }

    public ShowCaseCodeSnippetKey Key { get; }

    public string? Title { get; }
}
