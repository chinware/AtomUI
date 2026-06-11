using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.CaseNavigationLang;

[LanguageProvider(LanguageCode.zh_TW, CaseNavigation.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string General = "通用";
    public const string General_AboutUs = "關於我們";
    public const string General_Palette = "調色板";
    public const string General_Icons = "Icons 圖標";
    public const string General_Button = "Button 按鈕";
    public const string General_FloatButton = "FloatButton 懸浮按鈕";
    public const string General_SplitButton = "SplitButton 分割按鈕";
    public const string General_Separator = "Separator 分割線";
    public const string General_CustomizeTheme = "CustomizeTheme 主題";

    public const string Layout = "佈局";
    public const string Layout_FlexPanel = "FlexPanel 彈性佈局";
    public const string Layout_Grid = "Grid 柵格佈局";
    public const string Layout_Space = "Space 間距";
    public const string Layout_Splitter = "Splitter 分隔面板";

    public const string Navigation = "導航";
    public const string Navigation_Breadcrumb = "Breadcrumb 麵包屑";
    public const string Navigation_ButtonSpinner = "ButtonSpinner 選項按鈕";
    public const string Navigation_ComboBox = "ComboBox 組合框";
    public const string Navigation_DropdownButton = "DropdownButton 下拉菜單";
    public const string Navigation_Menu = "Menu 菜單";
    public const string Navigation_Pagination = "Pagination 分頁";
    public const string Navigation_Steps = "Steps 步驟條";
    public const string Navigation_TabControl = "TabControl 標籤頁";
    public const string Navigation_TabStrip = "TabStrip 標籤欄";

    public const string DataEntry = "數據錄入";
    public const string DataEntry_AutoComplete = "AutoComplete 自動完成";
    public const string DataEntry_Cascader = "Cascader 級聯選擇";
    public const string DataEntry_CheckBox = "CheckBox 多選框";
    public const string DataEntry_ColorPicker = "ColorPicker 顏色選擇器";
    public const string DataEntry_DatePicker = "DatePicker 日期選擇器";
    public const string DataEntry_TimePicker = "TimePicker 時間選擇器";
    public const string DataEntry_Form = "Form 表單";
    public const string DataEntry_LineEdit = "LineEdit 輸入框";
    public const string DataEntry_Mentions = "Mentions 提及";
    public const string DataEntry_NumberUpDown = "NumberUpDown 數字輸入框";
    public const string DataEntry_RadioButton = "RadioButton 單選框";
    public const string DataEntry_Rate = "Rate 評分";
    public const string DataEntry_Select = "Select 選擇器";
    public const string DataEntry_Slider = "Slider 滑動輸入條";
    public const string DataEntry_ToggleSwitch = "ToggleSwitch 開關";
    public const string DataEntry_TreeSelect = "TreeSelect 樹選擇";
    public const string DataEntry_Transfer = "Transfer 穿梭框";
    public const string DataEntry_Upload = "Upload 上傳";

    public const string DataDisplay = "數據展示";
    public const string DataDisplay_Avatar = "Avatar 頭像";
    public const string DataDisplay_Badge = "Badge 徽標數";
    public const string DataDisplay_Calendar = "Calendar 日曆";
    public const string DataDisplay_Card = "Card 卡片";
    public const string DataDisplay_Carousel = "Carousel 走馬燈";
    public const string DataDisplay_Collapse = "Collapse 折疊面板";
    public const string DataDisplay_Descriptions = "Descriptions 描述列表";
    public const string DataDisplay_DataGrid = "DataGrid 數據表格";
    public const string DataDisplay_Expander = "Expander 展開面板";
    public const string DataDisplay_Empty = "Empty 空狀態";
    public const string DataDisplay_ImagePreviewer = "ImagePreviewer 圖片預覽";
    public const string DataDisplay_GroupBox = "GroupBox 分組盒";
    public const string DataDisplay_InfoFlyout = "InfoFlyout 信息提示";
    public const string DataDisplay_List = "List 列表";
    public const string DataDisplay_QRCode = "QRCode 二維碼";
    public const string DataDisplay_Segmented = "Segmented 分段控制器";
    public const string DataDisplay_Statistic = "Statistic 統計數值";
    public const string DataDisplay_Tag = "Tag 標籤";
    public const string DataDisplay_Timeline = "Timeline 時間軸";
    public const string DataDisplay_TreeView = "TreeView 樹形控件";
    public const string DataDisplay_Tooltip = "Tooltip 文字提示";
    public const string DataDisplay_Tour = "Tour 漫遊式引導";

    public const string Feedback = "反饋";
    public const string Feedback_Alert = "Alert 警告提示";
    public const string Feedback_Drawer = "Drawer 抽屜";
    public const string Feedback_Message = "Message 全局提示";
    public const string Feedback_Modal = "Modal 對話框";
    public const string Feedback_Notification = "Notification 通知提醒框";
    public const string Feedback_PopupConfirm = "PopupConfirm 氣泡確定框";
    public const string Feedback_ProgressBar = "ProgressBar 進度條";
    public const string Feedback_Result = "Result 結果";
    public const string Feedback_Skeleton = "Skeleton 骨架屏";
    public const string Feedback_Spin = "Spin 加載提示";
    public const string Feedback_Watermark = "Watermark 水印";

    protected override Type GetResourceKindType() => typeof(CaseNavigationLangResourceKind);
}

