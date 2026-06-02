using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Tag;

public partial class TagShowCase : ReactiveUserControl<TagViewModel>
{
    public const string LanguageId = nameof(TagShowCase);

    public TagShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
