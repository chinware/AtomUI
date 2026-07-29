using System.Collections;
using System.Collections.ObjectModel;
using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Tag;

public class TagViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Tag";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private bool _isCheckableTagChecked;

    public bool IsCheckableTagChecked
    {
        get => _isCheckableTagChecked;
        set => this.RaiseAndSetIfChanged(ref _isCheckableTagChecked, value);
    }

    private string? _singleCheckedTag;

    public string? SingleCheckedTag
    {
        get => _singleCheckedTag;
        set => this.RaiseAndSetIfChanged(ref _singleCheckedTag, value);
    }

    private IList? _multipleCheckedTags;

    public IList? MultipleCheckedTags
    {
        get => _multipleCheckedTags;
        set => this.RaiseAndSetIfChanged(ref _multipleCheckedTags, value);
    }

    public ObservableCollection<object> CheckableTagOptions { get; } =
        new ObservableCollection<object> { "Movies", "Books", "Music", "Sports" };

    public TagViewModel(IScreen screen)
    {
        HostScreen = screen;
        IsCheckableTagChecked = true;
        SingleCheckedTag = "Books";
        MultipleCheckedTags = new ObservableCollection<object> { "Movies", "Music" };
    }
}
