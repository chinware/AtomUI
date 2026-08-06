using System.Reflection;
using AtomUI.Desktop.Controls.Localization;
using AtomUI.Localization;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Localization;

public class DataGridCatalogCoverageTests
{
    static DataGridCatalogCoverageTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void DataGrid_Catalog_Preserves_All_Translations()
    {
        var expected = new[]
        {
            new ExpectedEntry(DataGridLangResourceKind.SelectAllFilterItems, 1, "Select all items", "选择所有", "選擇所有"),
            new ExpectedEntry(DataGridLangResourceKind.AscendTooltip, 2, "Click to sort ascending", "点击升序", "點擊升序"),
            new ExpectedEntry(DataGridLangResourceKind.DescendTooltip, 3, "Click to sort descending", "点击降序", "點擊降序"),
            new ExpectedEntry(DataGridLangResourceKind.CancelTooltip, 4, "Click to cancel sorting", "取消排序", "取消排序"),
            new ExpectedEntry(DataGridLangResourceKind.DeleteConfirmText, 5, "Sure to delete?", "确认删除？", "確認刪除？"),
            new ExpectedEntry(DataGridLangResourceKind.CancelConfirmText, 6, "Sure to cancel?", "确认取消？", "確認取消？"),
            new ExpectedEntry(DataGridLangResourceKind.Operating, 7, "Operation in progress, please wait.", "正在操作中，请稍后", "正在操作中，請稍後")
        };
        var resourceKindType = typeof(DataGridLangResourceKind);

        resourceKindType.GetCustomAttribute<LanguageCatalogAttribute>()
                        .ShouldNotBeNull()
                        .ContractVersion.ShouldBe(1);
        Enum.GetValues<DataGridLangResourceKind>()
            .ShouldBe(expected.Select(static entry => entry.Kind), ignoreOrder: true);
        foreach (var entry in expected)
        {
            Convert.ToInt32(entry.Kind).ShouldBe(entry.Id);
        }

        typeof(DataGridLangResourceExtension).IsSealed.ShouldBeTrue();
        typeof(DataGridLangResourceExtension).BaseType.ShouldBe(
            typeof(LanguageResourceExtension<DataGridLangResourceKind>));

        var application = Application.Current.ShouldNotBeNull();
        var languageManager = application.GetLanguageManager().ShouldNotBeNull();
        var localizer = application.GetLocalizer().ShouldNotBeNull();
        AssertLanguage(LanguageTags.EnUS, expected.Select(static entry => (entry.Kind, entry.En)), languageManager, localizer);
        AssertLanguage(LanguageTags.ZhCN, expected.Select(static entry => (entry.Kind, entry.ZhCn)), languageManager, localizer);
        AssertLanguage(LanguageTags.ZhTW, expected.Select(static entry => (entry.Kind, entry.ZhTw)), languageManager, localizer);

        resourceKindType.Assembly.GetType("AtomUI.Desktop.Controls.DataGridLocalization.en_US").ShouldBeNull();
        resourceKindType.Assembly.GetType("AtomUI.Desktop.Controls.DataGridLocalization.zh_CN").ShouldBeNull();
        resourceKindType.Assembly.GetType("AtomUI.Desktop.Controls.DataGridLocalization.zh_TW").ShouldBeNull();
    }

    private static void AssertLanguage(
        LanguageTag language,
        IEnumerable<(DataGridLangResourceKind Kind, string Text)> expected,
        ILanguageManager languageManager,
        ILocalizer localizer)
    {
        languageManager.ChangeLanguage(language);
        foreach (var (kind, text) in expected)
        {
            localizer.Get(kind).ShouldBe(text);
        }
    }

    private readonly record struct ExpectedEntry(
        DataGridLangResourceKind Kind,
        int Id,
        string En,
        string ZhCn,
        string ZhTw);
}
