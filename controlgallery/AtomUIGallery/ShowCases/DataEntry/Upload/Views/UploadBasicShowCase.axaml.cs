
namespace AtomUIGallery.ShowCases.Upload;

public partial class UploadBasicShowCase : UploadScenarioShowCase
{
    public UploadBasicShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            disposables.Add(AttachUpload(BasicUpload));
            disposables.Add(AttachUpload(AvatarDemoPictureCardUpload, HandleImageUploadAboutToScheduling));
            disposables.Add(AttachUpload(AvatarDemoPictureCircleUpload, HandleImageUploadAboutToScheduling));
            disposables.Add(AttachUpload(DefaultFileList));
        });
    }
}
