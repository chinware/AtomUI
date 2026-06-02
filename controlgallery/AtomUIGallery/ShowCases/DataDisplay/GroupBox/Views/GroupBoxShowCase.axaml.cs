using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.GroupBox;

public partial class GroupBoxShowCase : ReactiveUserControl<GroupBoxViewModel>
{
    public const string LanguageId = nameof(GroupBoxShowCase);

    public GroupBoxShowCase()
    {
        this.WhenActivated(disposables => { });
        InitializeComponent();
    }
}
