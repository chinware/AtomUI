using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.Upload;

[LanguageProvider(LanguageCode.zh_CN, UploadShowCase.LanguageId)]
internal partial class zh_CN
{
    public const string UploadByClickingTitle = "点击上传";
    public const string UploadByClickingDescription = "经典模式。点击上传按钮时弹出文件选择对话框。";
    public const string ScenarioBasic = "基础";
    public const string ScenarioPictures = "图片";
    public const string ScenarioConstraints = "限制";
    public const string AvatarTitle = "头像";
    public const string AvatarDescription = "点击上传用户头像，并通过 beforeUpload 校验图片大小和格式。";
    public const string DefaultFilesTitle = "默认文件";
    public const string DefaultFilesDescription = "通过绑定 Files 提供初始的可观察上传文件列表。";
    public const string PicturesWallTitle = "图片墙";
    public const string PicturesWallDescription = "用户上传图片后，缩略图会显示在 picture-card 列表中，触发器始终作为独立 append 入口呈现。";
    public const string PictureCircleTypeTitle = "圆形图片卡片";
    public const string PictureCircleTypeDescription = "picture-card 的另一种展示形式。";
    public const string DragAndDropTitle = "拖拽上传";
    public const string DragAndDropDescription = "可以将文件拖拽到指定区域上传，也可以通过选择文件上传。";
    public const string PicturesWithListStyleTitle = "列表样式图片";
    public const string PicturesWithListStyleDescription = "如果上传文件是图片，可以显示缩略图。";
    public const string MaxCountTitle = "最大数量";
    public const string MaxCountDescription = "使用 maxCount 限制文件数量。当 maxCount 为 1 时会替换当前文件。";
    public const string FileAndDirectoryTitle = "文件与目录";
    public const string FileAndDirectoryDescription = "组合两个 UploadTrigger，分别触发文件选择和目录选择。";
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
    public const string PageSubtitle = "选择、校验、预览并上传文件，支持列表和图片展示方式。";
    public const string ScrollableListTitle = "可滚动列表";
    public const string ScrollableListDescription = "UploadList 自己管理滚动区域，长文件列表不需要外层 ScrollViewer。";
    public const string SuccessAutoRemoveTitle = "成功后自动移除";
    public const string SuccessAutoRemoveDescription = "通过 SuccessAutoRemoveDelay 在上传成功后延迟移除文件。";
    public const string PageDescription = "Upload 支持组合式文件和目录触发器、拖拽区域、可观察 Files 状态、图片列表、最大数量限制、成功清理和文件类型过滤。";
    public const string ComponentCategory = "数据录入";
    public const string ComponentStatusStable = "稳定";
    public const string ScenarioExamples = "示例";

}
