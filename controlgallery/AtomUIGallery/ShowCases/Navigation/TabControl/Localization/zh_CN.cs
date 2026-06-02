using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.TabControl;

[LanguageProvider(LanguageCode.zh_CN, TabControlShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string TabControlBasicTitle = "基础用法";
    public const string TabControlBasicDescription = "默认激活第一个标签页。";
    public const string TabControlItemsSourceTitle = "通过 ItemSource 生成标签项";
    public const string TabControlItemsSourceDescription = "基于数据源和项目模板添加 TabItem。";
    public const string TabControlDisabledTitle = "禁用标签";
    public const string TabControlDisabledDescription = "禁用某个标签页。";
    public const string TabControlCenteredTitle = "居中显示";
    public const string TabControlCenteredDescription = "标签页居中显示。";
    public const string TabControlIconTitle = "带图标";
    public const string TabControlIconDescription = "带图标的标签页。";
    public const string TabControlSlideTitle = "滑动";
    public const string TabControlSlideDescription = "为了容纳更多标签，标签可以左右滑动（或上下滑动）。";
    public const string TabControlCardTypeTitle = "卡片式标签";
    public const string TabControlCardTypeDescription = "另一种标签页类型，不支持垂直模式。";
    public const string TabControlClosableTitle = "可关闭标签";
    public const string TabControlClosableDescription = "支持可关闭的标签页设置。";
    public const string TabControlPositionTitle = "位置";
    public const string TabControlPositionDescription = "标签位置可设为 left、right、top 或 bottom，在移动端会自动切换为 top。";
    public const string TabControlCardShapePositionTitle = "卡片形态位置";
    public const string TabControlCardShapePositionDescription = "标签位置可设为 left、right、top 或 bottom，在移动端会自动切换为 top。";
    public const string TabControlSizeTitle = "尺寸";
    public const string TabControlSizeDescription = "大尺寸标签通常用于页头，小尺寸可用于模态框。";
    public const string TabControlAddCloseTitle = "新增和关闭标签";
    public const string TabControlAddCloseDescription = "隐藏默认加号图标，并为自定义触发器绑定事件。";
    public const string TabControlExtraContentTitle = "额外内容";
    public const string TabControlExtraContentDescription = "可以在标签页右侧、左侧或两侧添加额外操作。";
    public const string TabStripBasicTitle = "基础用法";
    public const string TabStripBasicDescription = "默认激活第一个标签项。";
    public const string TabStripItemsSourceTitle = "通过 ItemSource 生成 TabStripItem";
    public const string TabStripItemsSourceDescription = "基于数据源和项目模板添加 TabStripItem。";
    public const string TabStripDisabledTitle = "禁用标签";
    public const string TabStripDisabledDescription = "禁用某个标签项。";
    public const string TabStripCenteredTitle = "居中显示";
    public const string TabStripCenteredDescription = "标签项居中显示。";
    public const string TabStripIconTitle = "带图标";
    public const string TabStripIconDescription = "带图标的标签项。";
    public const string TabStripSlideTitle = "滑动";
    public const string TabStripSlideDescription = "为了容纳更多标签，标签可以左右滑动（或上下滑动）。";
    public const string TabStripCardTypeTitle = "卡片式标签";
    public const string TabStripCardTypeDescription = "另一种标签类型，不支持垂直模式。";
    public const string TabStripClosableTitle = "可关闭标签";
    public const string TabStripClosableDescription = "支持可关闭的标签设置。";
    public const string TabStripPositionTitle = "位置";
    public const string TabStripPositionDescription = "标签位置可设为 left、right、top 或 bottom，在移动端会自动切换为 top。";
    public const string TabStripCardShapePositionTitle = "卡片形态位置";
    public const string TabStripCardShapePositionDescription = "标签位置可设为 left、right、top 或 bottom，在移动端会自动切换为 top。";
    public const string TabStripSizeTitle = "尺寸";
    public const string TabStripSizeDescription = "大尺寸标签通常用于页头，小尺寸可用于模态框。";
    public const string TabStripAddCloseTitle = "新增和关闭标签";
    public const string TabStripAddCloseDescription = "隐藏默认加号图标，并为自定义触发器绑定事件。";

    protected override Type GetResourceKindType() => typeof(TabControlShowCaseLangResourceKind);
}
