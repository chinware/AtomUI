using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Upload;

public class UploadViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Upload";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    public IFileUploadTransport UploadTransport { get; } = new UploadMockTransport();

    private ObservableCollection<UploadFileItem>? _defaultFiles;

    public ObservableCollection<UploadFileItem>? DefaultFiles
    {
        get => _defaultFiles;
        set => this.RaiseAndSetIfChanged(ref _defaultFiles, value);
    }

    private ObservableCollection<UploadFileItem>? _picturesWallFiles;

    public ObservableCollection<UploadFileItem>? PicturesWallFiles
    {
        get => _picturesWallFiles;
        set => this.RaiseAndSetIfChanged(ref _picturesWallFiles, value);
    }

    private ObservableCollection<UploadFileItem>? _pictureCircleFiles;

    public ObservableCollection<UploadFileItem>? PictureCircleFiles
    {
        get => _pictureCircleFiles;
        set => this.RaiseAndSetIfChanged(ref _pictureCircleFiles, value);
    }

    private ObservableCollection<UploadFileItem>? _pictureListStyleFiles;

    public ObservableCollection<UploadFileItem>? PictureListStyleFiles
    {
        get => _pictureListStyleFiles;
        set => this.RaiseAndSetIfChanged(ref _pictureListStyleFiles, value);
    }

    private ObservableCollection<UploadFileItem>? _scrollableUploadFiles;

    public ObservableCollection<UploadFileItem>? ScrollableUploadFiles
    {
        get => _scrollableUploadFiles;
        set => this.RaiseAndSetIfChanged(ref _scrollableUploadFiles, value);
    }

    public UploadViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
