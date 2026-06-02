using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DropdownButton;

[LanguageProvider(LanguageCode.zh_CN, DropdownButtonShowCase.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "最基础的下拉菜单。";
    public const string ButtonTypesTitle = "按钮类型";
    public const string ButtonTypesDescription = "支持统一的按钮类型。";
    public const string ArrowTitle = "箭头";
    public const string ArrowDescription = "可以显示箭头。";
    public const string PlacementTitle = "弹出位置";
    public const string PlacementDescription = "支持 6 种弹出位置。";

    protected override Type GetResourceKindType() => typeof(DropdownButtonShowCaseLangResourceKind);
}
