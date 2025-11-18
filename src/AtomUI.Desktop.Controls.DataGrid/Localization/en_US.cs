using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.DataGridLocalization;

[LanguageProvider(LanguageCode.en_US, DataGridToken.ID)]
internal class en_US : LanguageProvider
{
    public const string SelectAllFilterItems = "Select all items";
    public const string AscendTooltip = "Click to sort ascending";
    public const string DescendTooltip = "Click to sort descending";
    public const string CancelTooltip = "Click to cancel sorting";
    public const string DeleteConfirmText = "Sure to delete?";
    public const string CancelConfirmText = "Sure to cancel?";
    public const string Operating = "Operation in progress, please wait.";
}