using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Upload;

[LanguageProvider(LanguageCode.en_US, UploadShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string UploadByClickingTitle = "Upload by clicking";
    public const string UploadByClickingDescription = "Classic mode. File selection dialog pops up when upload button is clicked.";
    public const string ScenarioBasic = "Basic";
    public const string ScenarioPictures = "Pictures";
    public const string ScenarioConstraints = "Constraints";
    public const string AvatarTitle = "Avatar";
    public const string AvatarDescription = "Click to upload user's avatar, and validate size and format of picture with beforeUpload.";
    public const string DefaultFilesTitle = "Default Files";
    public const string DefaultFilesDescription = "Use defaultFileList for uploaded files when page init.";
    public const string PicturesWallTitle = "Pictures Wall";
    public const string PicturesWallDescription = "After users upload picture, the thumbnail will be shown in list. The upload button will disappear when count meets limitation.";
    public const string PictureCircleTypeTitle = "Pictures with picture-circle type";
    public const string PictureCircleTypeDescription = "Alternative display for picture-card.";
    public const string DragAndDropTitle = "Drag and Drop";
    public const string DragAndDropDescription = "You can drag files to a specific area, to upload. Alternatively, you can also upload by selecting.";
    public const string PicturesWithListStyleTitle = "Pictures with list style";
    public const string PicturesWithListStyleDescription = "If uploaded file is a picture, the thumbnail can be shown.";
    public const string MaxCountTitle = "Max Count";
    public const string MaxCountDescription = "Limit files with maxCount. Will replace current one when maxCount is 1.";
    public const string UploadDirectoryTitle = "Upload directory";
    public const string UploadDirectoryDescription = "You can select and upload a whole directory.";
    public const string UploadPngOnlyTitle = "Upload png file only";
    public const string UploadPngOnlyDescription = "beforeUpload only prevent upload behavior when return false or reject promise, the prevented file would still show in file list. Here is the example you can keep prevented files out of list by return UPLOAD.LIST_IGNORE.";
    public const string P2ContentClickToUpload = "Click to Upload";
    public const string P2TextUpload = "Upload";
    public const string P2ContentUploadMaxN1 = "Upload (Max: 1)";
    public const string P2ContentUploadMaxN3 = "Upload (Max: 3)";
    public const string P2ContentUploadDirectory = "Upload Directory";
    public const string P2ContentUploadPngOnly = "Upload PNG only";
    public const string P2ErrorServer500 = "Server Error 500";
    public const string P2ErrorUpload = "Upload error!";
    public const string P2CancelJpgPngOnly = "You can only upload JPG/PNG file!";
    public const string P2CancelImageSize = "Image must be smaller than 2MB!";
    public const string P2CancelPngOnly = "You can only upload PNG file!";
    public const string P2UploadSuccessFormat = "{0} uploaded successfully!";

    protected override Type GetResourceKindType() => typeof(UploadShowCaseLangResourceKind);
}
