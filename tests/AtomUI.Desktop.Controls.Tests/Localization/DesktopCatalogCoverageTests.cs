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
            new ExpectedEntry<CalendarControlLangResourceKind>(CalendarControlLangResourceKind.Month, 1, "Month", "月", "月"),
            new ExpectedEntry<CalendarControlLangResourceKind>(CalendarControlLangResourceKind.Year, 2, "Year", "年", "年"),
            new ExpectedEntry<CalendarControlLangResourceKind>(CalendarControlLangResourceKind.YearSuffix, 3, "", "年", "年"),
            new ExpectedEntry<CalendarControlLangResourceKind>(CalendarControlLangResourceKind.Week, 4, "Week", "周", "週"));
    }

    [Fact]
    public void DatePicker_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.DatePickerLang",
            new ExpectedEntry<DatePickerLangResourceKind>(DatePickerLangResourceKind.Today, 1, "Today", "今天", "今天"),
            new ExpectedEntry<DatePickerLangResourceKind>(DatePickerLangResourceKind.Now, 2, "Now", "现在", "現在"));
    }

    [Fact]
    public void Dialog_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.DialogLang",
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Ok, 1, "OK", "确定", "確定"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Open, 2, "Open", "打开", "打開"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Save, 3, "Save", "保存", "保存"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Cancel, 4, "Cancel", "取消", "取消"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Close, 5, "Close", "关闭", "關閉"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Discard, 6, "Discard", "丢弃", "丟棄"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Apply, 7, "Apply", "应用", "應用"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Reset, 8, "Reset", "重置", "重置"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Reload, 9, "Reload", "重新加载", "重新加載"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.RestoreDefaults, 10, "Restore Defaults", "恢复默认值", "恢復默認值"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Help, 11, "Help", "帮助", "幫助"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.SaveAll, 12, "Save All", "全部保存", "全部保存"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Yes, 13, "Yes", "是", "是"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.YesToAll, 14, "Yes to All", "全部是", "全部是"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.No, 15, "No", "否", "否"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.NoToAll, 16, "No to All", "全部否", "全部否"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Abort, 17, "Abort", "中止", "中止"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Retry, 18, "Retry", "重试", "重試"),
            new ExpectedEntry<DialogLangResourceKind>(DialogLangResourceKind.Ignore, 19, "Ignore", "忽略", "忽略"));
    }

    [Fact]
    public void ImagePreviewer_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.ImagePreviewerLang",
            new ExpectedEntry<ImagePreviewerLangResourceKind>(ImagePreviewerLangResourceKind.ImageLoadFailed, 1, "Image load failed", "图片加载失败", "圖片載入失敗"),
            new ExpectedEntry<ImagePreviewerLangResourceKind>(ImagePreviewerLangResourceKind.Preview, 2, "Preview", "预览", "預覽"));
    }

    [Fact]
    public void Pagination_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.PaginationLang",
            new ExpectedEntry<PaginationLangResourceKind>(PaginationLangResourceKind.JumpToText, 1, "Go to", "跳至", "跳至"),
            new ExpectedEntry<PaginationLangResourceKind>(PaginationLangResourceKind.PageText, 2, "Page", "页", "頁"),
            new ExpectedEntry<PaginationLangResourceKind>(PaginationLangResourceKind.TotalInfoFormat, 3, "Total ${Total} items", "共 ${Total} 项", "共 ${Total} 項"));
    }

    [Fact]
    public void QRCode_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.QRCodeLang",
            new ExpectedEntry<QRCodeLangResourceKind>(QRCodeLangResourceKind.Refresh, 1, "Refresh", "点击刷新", "點擊刷新"),
            new ExpectedEntry<QRCodeLangResourceKind>(QRCodeLangResourceKind.Expired, 2, "QR code expired", "二维码过期", "二維碼過期"),
            new ExpectedEntry<QRCodeLangResourceKind>(QRCodeLangResourceKind.Scanned, 3, "Scanned", "已扫描", "已掃描"));
    }

    [Fact]
    public void TimePicker_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.TimePickerLang",
            new ExpectedEntry<TimePickerLangResourceKind>(TimePickerLangResourceKind.AMText, 1, "AM", "上午", "上午"),
            new ExpectedEntry<TimePickerLangResourceKind>(TimePickerLangResourceKind.PMText, 2, "PM", "下午", "下午"),
            new ExpectedEntry<TimePickerLangResourceKind>(TimePickerLangResourceKind.Now, 3, "Now", "现在", "現在"));
    }

    [Fact]
    public void Tour_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.TourLang",
            new ExpectedEntry<TourLangResourceKind>(TourLangResourceKind.Previous, 1, "Previous", "上一步", "上一步"),
            new ExpectedEntry<TourLangResourceKind>(TourLangResourceKind.Next, 2, "Next", "下一步", "下一步"),
            new ExpectedEntry<TourLangResourceKind>(TourLangResourceKind.Finish, 3, "Finish", "结束导览", "結束導覽"));
    }

    [Fact]
    public void Transfer_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.TransferLang",
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.Item, 1, "item", "项", "項"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.Items, 2, "items", "项", "項"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.SelectAll, 3, "select all data", "全选所有", "全選所有"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.DeSelectAll, 4, "deselect all data", "取消全选", "取消全選"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.RemoveCurrentPage, 5, "remove current page", "删除当页", "刪除當頁"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.RemoveAll, 6, "remove all data", "删除所有", "刪除所有"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.InvertSelectCurrentPage, 7, "invert current page", "反选当页", "反選當頁"),
            new ExpectedEntry<TransferLangResourceKind>(TransferLangResourceKind.SelectCurrentPage, 8, "select current page", "选择当页", "選擇當頁"));
    }

    [Fact]
    public void Upload_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.UploadLang",
            new ExpectedEntry<UploadLangResourceKind>(UploadLangResourceKind.Uploading, 1, "Uploading...", "上传中...", "上傳中..."),
            new ExpectedEntry<UploadLangResourceKind>(UploadLangResourceKind.Pending, 2, "Pending...", "等待调度...", "等待調度..."),
            new ExpectedEntry<UploadLangResourceKind>(UploadLangResourceKind.DragUploadHead, 3, "Click or drag file to this area to upload", "点击或拖动文件到此区域进行上传", "點擊或拖動文件到此區域進行上傳"));
    }

    [Fact]
    public void ColorPicker_Catalog_Preserves_All_Translations()
    {
        AssertCatalog(
            "AtomUI.Desktop.Controls.ColorPickerLang",
            new ExpectedEntry<ColorPickerLangResourceKind>(ColorPickerLangResourceKind.EmptyColorText, 1, "Transparent", "无色", "無色"));
    }

    private static void AssertCatalog<TResourceKind>(
        string legacyProviderNamespace,
        params ExpectedEntry<TResourceKind>[] expected)
        where TResourceKind : struct, Enum
    {
        var resourceKindType = typeof(TResourceKind);
        var catalog = resourceKindType.GetCustomAttribute<LanguageCatalogAttribute>().ShouldNotBeNull();
        catalog.ContractVersion.ShouldBe(1);
        Enum.GetValues<TResourceKind>().ShouldBe(expected.Select(static entry => entry.Kind), ignoreOrder: true);
        expected.Select(static entry => entry.Id).ShouldAllBe(static id => id > 0);
        expected.Select(static entry => entry.Id).Distinct().Count().ShouldBe(expected.Length);
        foreach (var entry in expected)
        {
            Convert.ToInt32(entry.Kind).ShouldBe(entry.Id);
        }

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
        int Id,
        string En,
        string ZhCn,
        string ZhTw)
        where TResourceKind : struct, Enum;
}
