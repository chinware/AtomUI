using AtomUI.Theme;
using AtomUI.Utils;

namespace AtomUI.Controls.DataGridLocalization;

[LanguageProvider(LanguageCode.en_US, DataGridToken.ID)]
internal class en_US : AbstractLanguageProvider
{
    public const string SelectAllFilterItems = "Select all items";
    public const string AscendTooltip = "Click to sort ascending";
    public const string DescendTooltip = "Click to sort descending";
    public const string CancelTooltip = "Click to cancel sorting";
    public const string DeleteConfirmText = "Sure to delete?";
    public const string CancelConfirmText = "Sure to cancel?";
}