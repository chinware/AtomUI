using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.CaseNavigationLang;

[LanguageProvider(LanguageCode.zh_CN, CaseNavigation.LanguageId)]
internal class zh_CN : LanguageProvider
{
    public const string General = "通用";
    public const string General_AboutUs = "关于我们";
    public const string General_Palette = "调色板";
    public const string General_Icons = "Icons 图标";
    public const string General_Button = "Button 按钮";
    public const string General_SplitButton = "SplitButton 分割按钮";
    public const string General_Separator = "Separator 分割线";
    public const string Layout = "布局";
    public const string Layout_FlexPanel = "FlexPanel 弹性布局";
    public const string Layout_Splitter = "Splitter 分割面板";
    public const string Navigation = "导航";
    public const string Navigation_Breadcrumb = "Breadcrumb 面包屑";
    public const string Navigation_ButtonSpinner = "ButtonSpinner 按钮微调器";
    public const string Navigation_ComboBox = "ComboBox 下拉框";
    public const string Navigation_DropdownButton = "DropdownButton 下拉按钮";
    public const string DataEntry = "数据录入";
    public const string DataEntry_CheckBox = "CheckBox 多选框";
    public const string DataEntry_ToggleSwitch = "ToggleSwitch 开关";
    public const string DataEntry_RadioButton = "RadioButton 单选框";
    public const string DataEntry_Rate = "Rate 评分";
    public const string DataEntry_Slider = "Slider 滑动输入条";
    public const string DataDisplay = "数据展示";
    public const string DataDisplay_Avatar = "Avatar 头像";
    public const string DataDisplay_Calendar = "Calendar 日历";
    public const string DataDisplay_Card = "Card 卡片";
    public const string DataDisplay_Carousel = "Carousel 走马灯";
    public const string DataDisplay_Collapse = "Collapse 折叠面板";
    public const string DataDisplay_Empty = "Empty 空状态";
    public const string DataDisplay_Expander = "Expander 折叠面板";
    public const string DataDisplay_Tag = "Tag 标签";
    public const string DataDisplay_Segmented = "Segmented 分段控制器";
    public const string Feedback = "反馈";
    public const string Feedback_Alert = "Alert 警告提示";
    public const string Feedback_Message = "Message 全局提示";
    public const string Feedback_Notification = "Notification 通知提醒框";
    public const string Feedback_PopupConfirm = "PopupConfirm 气泡确认框";
    public const string Feedback_Skeleton = "Skeleton 骨架屏";
    public const string Feedback_Spin = "Spin 加载中";

    protected override Type GetResourceKindType() => typeof(CaseNavigationLangResourceKind);
}
