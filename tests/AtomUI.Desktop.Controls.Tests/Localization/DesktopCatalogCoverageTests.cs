using System.Reflection;
using AtomUI.Desktop.Controls.Localization;
using AtomUI.Localization;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Localization;

public class DesktopCatalogCoverageTests
{
    static DesktopCatalogCoverageTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Calendar_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.CalendarControlLang",
            new ExpectedEntry<CalendarControlLangResourceKind>(CalendarControlLangResourceKind.Month, "Month", "月", "月"),
            new ExpectedEntry<CalendarControlLangResourceKind>(CalendarControlLangResourceKind.Year, "Year", "年", "年"),
            new ExpectedEntry<CalendarControlLangResourceKind>(CalendarControlLangResourceKind.YearSuffix, "", "年", "年"),
            new ExpectedEntry<CalendarControlLangResourceKind>(CalendarControlLangResourceKind.Week, "Week", "周", "週"));
    }

    [Fact]
    public void DatePicker_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.DatePickerLang",
            new ExpectedEntry<DatePickerLangResourceKind>(DatePickerLangResourceKind.Today, "Today", "今天", "今天"),
            new ExpectedEntry<DatePickerLangResourceKind>(DatePickerLangResourceKind.Now, "Now", "现在", "現在"));
    }

    [Fact]
    public void Dialog_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.DialogLang",
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Ok, "OK", "确定", "確定"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Open, "Open", "打开", "打開"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Save, "Save", "保存", "保存"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Cancel, "Cancel", "取消", "取消"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Close, "Close", "关闭", "關閉"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Discard, "Discard", "丢弃", "丟棄"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Apply, "Apply", "应用", "應用"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Reset, "Reset", "重置", "重置"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Reload, "Reload", "重新加载", "重新加載"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.RestoreDefaults, "Restore Defaults", "恢复默认值", "恢復默認值"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Help, "Help", "帮助", "幫助"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.SaveAll, "Save All", "全部保存", "全部保存"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Yes, "Yes", "是", "是"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.YesToAll, "Yes to All", "全部是", "全部是"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.No, "No", "否", "否"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.NoToAll, "No to All", "全部否", "全部否"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Abort, "Abort", "中止", "中止"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Retry, "Retry", "重试", "重試"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Ignore, "Ignore", "忽略", "忽略"));
    }

    [Fact]
    public void ImagePreviewer_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.ImagePreviewerLang",
            new ExpectedEntry<ImagePreviewerLangResourceKind>(ImagePreviewerLangResourceKind.ImageLoadFailed, "Image load failed", "图片加载失败", "圖片載入失敗"),
            new ExpectedEntry<ImagePreviewerLangResourceKind>(ImagePreviewerLangResourceKind.Preview, "Preview", "预览", "預覽"));
    }

    [Fact]
    public void Pagination_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.PaginationLang",
            new ExpectedEntry<PaginationLangResourceKind>(PaginationLangResourceKind.JumpToText, "Go to", "跳至", "跳至"),
            new ExpectedEntry<PaginationLangResourceKind>(PaginationLangResourceKind.PageText, "Page", "页", "頁"),
            new ExpectedEntry<PaginationLangResourceKind>(PaginationLangResourceKind.TotalInfoFormat, "Total ${Total} items", "共 ${Total} 项", "共 ${Total} 項"));
    }

    [Fact]
    public void QRCode_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.QRCodeLang",
            new ExpectedEntry<QRCodeLangResourceKind>(QRCodeLangResourceKind.Refresh, "Refresh", "点击刷新", "點擊刷新"),
            new ExpectedEntry<QRCodeLangResourceKind>(QRCodeLangResourceKind.Expired, "QR code expired", "二维码过期", "二維碼過期"),
            new ExpectedEntry<QRCodeLangResourceKind>(QRCodeLangResourceKind.Scanned, "Scanned", "已扫描", "已掃描"));
    }

    [Fact]
    public void TimePicker_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.TimePickerLang",
            new ExpectedEntry<TimePickerLangResourceKind>(TimePickerLangResourceKind.AMText, "AM", "上午", "上午"),
            new ExpectedEntry<TimePickerLangResourceKind>(TimePickerLangResourceKind.PMText, "PM", "下午", "下午"),
            new ExpectedEntry<TimePickerLangResourceKind>(TimePickerLangResourceKind.Now, "Now", "现在", "現在"));
    }

    [Fact]
    public void Tour_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.TourLang",
            new ExpectedEntry<TourLangResourceKind>(TourLangResourceKind.Previous, "Previous", "上一步", "上一步"),
            new ExpectedEntry<TourLangResourceKind>(TourLangResourceKind.Next, "Next", "下一步", "下一步"),
            new ExpectedEntry<TourLangResourceKind>(TourLangResourceKind.Finish, "Finish", "结束导览", "結束導覽"));
    }

    [Fact]
    public void Transfer_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.TransferLang",
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.Item, "item", "项", "項"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.Items, "items", "项", "項"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.SelectAll, "select all data", "全选所有", "全選所有"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.DeSelectAll, "deselect all data", "取消全选", "取消全選"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.RemoveCurrentPage, "remove current page", "删除当页", "刪除當頁"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.RemoveAll, "remove all data", "删除所有", "刪除所有"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.InvertSelectCurrentPage, "invert current page", "反选当页", "反選當頁"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.SelectCurrentPage, "select current page", "选择当页", "選擇當頁"));
    }

    [Fact]
    public void Upload_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.UploadLang",
            new ExpectedEntry<UploadLangResourceKind>(UploadLangResourceKind.Uploading, "Uploading...", "上传中...", "上傳中..."),
            new ExpectedEntry<UploadLangResourceKind>(UploadLangResourceKind.Pending, "Pending...", "等待调度...", "等待調度..."),
            new ExpectedEntry<UploadLangResourceKind>(UploadLangResourceKind.DragUploadHead, "Click or drag file to this area to upload", "点击或拖动文件到此区域进行上传", "點擊或拖動文件到此區域進行上傳"));
    }

    [Fact]
    public void ColorPicker_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.ColorPickerLang",
            new ExpectedEntry<ColorPickerLangResourceKind>(ColorPickerLangResourceKind.EmptyColorText, "Transparent", "无色", "無色"));
    }

    private static void AssertCatalog<TResourceKind>(
        string legacyProviderNamespace,
        params ExpectedEntry<TResourceKind>[] expected)
        where TResourceKind : struct, Enum
    {
        var resourceKindType = typeof(TResourceKind);
        var catalog = resourceKindType.GetCustomAttribute<LanguageCatalogAttribute>().ShouldNotBeNull();
        catalog.ContractVersion.ShouldBe(2);
        Enum.GetNames<TResourceKind>().ShouldBe(
            expected.Select(static entry => entry.Kind.ToString()));

        var extensionName = resourceKindType.Name.EndsWith("Kind", StringComparison.Ordinal)
            ? resourceKindType.Name[..^"Kind".Length] + "Extension"
            : resourceKindType.Name + "Extension";
        var extensionType = resourceKindType.Assembly.GetType(
            $"{resourceKindType.Namespace}.{extensionName}").ShouldNotBeNull();
        extensionType.IsSealed.ShouldBeTrue();
        extensionType.BaseType.ShouldBe(typeof(LanguageResourceExtension<TResourceKind>));

        var application = Application.Current.ShouldNotBeNull();
        var languageManager = application.GetLanguageManager().ShouldNotBeNull();
        var localizer = application.GetLocalizer().ShouldNotBeNull();
        AssertLanguage(LanguageTags.EnUS, expected.Select(static entry => (entry.Kind, entry.En)), languageManager, localizer);
        AssertLanguage(LanguageTags.ZhCN, expected.Select(static entry => (entry.Kind, entry.ZhCn)), languageManager, localizer);
        AssertLanguage(LanguageTags.ZhTW, expected.Select(static entry => (entry.Kind, entry.ZhTw)), languageManager, localizer);

        resourceKindType.Assembly.GetType($"{legacyProviderNamespace}.en_US").ShouldBeNull();
        resourceKindType.Assembly.GetType($"{legacyProviderNamespace}.zh_CN").ShouldBeNull();
        resourceKindType.Assembly.GetType($"{legacyProviderNamespace}.zh_TW").ShouldBeNull();
    }

    private static void AssertLanguage<TResourceKind>(
        LanguageTag language,
        IEnumerable<(TResourceKind Kind, string Text)> expected,
        ILanguageManager languageManager,
        ILocalizer localizer)
        where TResourceKind : struct, Enum
    {
        languageManager.ChangeLanguage(language);
        foreach (var (kind, text) in expected)
        {
            localizer.Get(kind).ShouldBe(text);
        }
    }

    private readonly record struct ExpectedEntry<TResourceKind>(
        TResourceKind Kind,
        string En,
        string ZhCn,
        string ZhTw)
        where TResourceKind : struct, Enum;
}
