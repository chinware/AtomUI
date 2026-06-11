using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Upload;

[LanguageProvider(LanguageCode.zh_CN, UploadShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string UploadByClickingTitle = "点击上传";
    public const string UploadByClickingDescription = "经典模式。点击上传按钮时弹出文件选择对话框。";
    public const string ScenarioBasic = "基础";
    public const string ScenarioPictures = "图片";
    public const string ScenarioConstraints = "限制";
    public const string AvatarTitle = "头像";
    public const string AvatarDescription = "点击上传用户头像，并通过 beforeUpload 校验图片大小和格式。";
    public const string DefaultFilesTitle = "默认文件";
    public const string DefaultFilesDescription = "页面初始化时使用 defaultFileList 设置已上传文件。";
    public const string PicturesWallTitle = "图片墙";
    public const string PicturesWallDescription = "用户上传图片后，缩略图会显示在列表中。当数量达到限制时，上传按钮会消失。";
    public const string PictureCircleTypeTitle = "圆形图片卡片";
    public const string PictureCircleTypeDescription = "picture-card 的另一种展示形式。";
    public const string DragAndDropTitle = "拖拽上传";
    public const string DragAndDropDescription = "可以将文件拖拽到指定区域上传，也可以通过选择文件上传。";
    public const string PicturesWithListStyleTitle = "列表样式图片";
    public const string PicturesWithListStyleDescription = "如果上传文件是图片，可以显示缩略图。";
    public const string MaxCountTitle = "最大数量";
    public const string MaxCountDescription = "使用 maxCount 限制文件数量。当 maxCount 为 1 时会替换当前文件。";
    public const string UploadDirectoryTitle = "上传目录";
    public const string UploadDirectoryDescription = "可以选择并上传整个目录。";
    public const string UploadPngOnlyTitle = "仅上传 PNG 文件";
    public const string UploadPngOnlyDescription = "beforeUpload 返回 false 或拒绝 promise 时只会阻止上传行为，被阻止的文件仍会显示在文件列表中。本示例通过返回 UPLOAD.LIST_IGNORE 将被阻止的文件排除在列表外。";
    public const string P2ContentClickToUpload = "点击上传";
    public const string P2TextUpload = "上传";
    public const string P2ContentUploadMaxN1 = "上传（最多：1）";
    public const string P2ContentUploadMaxN3 = "上传（最多：3）";
    public const string P2ContentUploadDirectory = "上传目录";
    public const string P2ContentUploadPngOnly = "仅上传 PNG";
    public const string P2ErrorServer500 = "服务器错误 500";
    public const string P2ErrorUpload = "上传失败！";
    public const string P2CancelJpgPngOnly = "只能上传 JPG/PNG 文件！";
    public const string P2CancelImageSize = "图片必须小于 2MB！";
    public const string P2CancelPngOnly = "只能上传 PNG 文件！";
    public const string P2UploadSuccessFormat = "{0} 上传成功！";

    protected override Type GetResourceKindType() => typeof(UploadShowCaseLangResourceKind);
}
