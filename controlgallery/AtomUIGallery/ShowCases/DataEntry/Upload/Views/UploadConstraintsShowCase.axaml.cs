using ReactiveUI;

namespace AtomUIGallery.ShowCases.Upload;

public partial class UploadConstraintsShowCase : UploadScenarioShowCase
{
    public UploadConstraintsShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            disposables.Add(AttachUpload(MaxCount1Upload));
            disposables.Add(AttachUpload(MaxCount3Upload));
            disposables.Add(AttachUpload(DirectoryUpload));
            disposables.Add(AttachUpload(PngOnlyUpload, HandlePngUploadAboutToScheduling));
        });
    }
}
