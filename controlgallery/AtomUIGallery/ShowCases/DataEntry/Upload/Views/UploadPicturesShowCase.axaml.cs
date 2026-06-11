
namespace AtomUIGallery.ShowCases.Upload;

public partial class UploadPicturesShowCase : UploadScenarioShowCase
{
    public UploadPicturesShowCase()
    {
        InitializeComponent();
        this.WhenActivated(disposables =>
        {
            disposables.Add(AttachUpload(PicturesWallUpload));
            disposables.Add(AttachUpload(PicturesCircleWallUpload));
            disposables.Add(AttachUpload(DragAndDropUpload));
            disposables.Add(AttachUpload(PictureListUpload));
        });
    }
}
