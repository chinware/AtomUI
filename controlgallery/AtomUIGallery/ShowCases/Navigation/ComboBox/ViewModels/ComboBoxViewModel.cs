using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ComboBox;

public class ComboBoxViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ComboBox";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private List<ComboBoxItemData>? _comboBoxItems;
    private ObservableCollection<ComboBoxApiRow>? _apiRows;
    private ObservableCollection<ComboBoxDesignTokenRow>? _designTokenRows;

    public List<ComboBoxItemData>? ComboBoxItems
    {
        get => _comboBoxItems;
        set => this.RaiseAndSetIfChanged(ref _comboBoxItems, value);
    }

    public ObservableCollection<ComboBoxApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ComboBoxDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ComboBoxViewModel(IScreen screen)
    {
        HostScreen    = screen;
        ComboBoxItems = CreateComboBoxItems();
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new ComboBoxApiRow("ItemsSource", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "cyan", "null"),
            new ComboBoxApiRow("SelectedItem", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertySelectedItem), "object?", "cyan", "null"),
            new ComboBoxApiRow("SelectedIndex", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertySelectedIndex), "int", "cyan", "-1"),
            new ComboBoxApiRow("PlaceholderText", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyPlaceholderText), "string?", "cyan", "null"),
            new ComboBoxApiRow("LeftAddOn", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyLeftAddOn), "object?", "cyan", "null"),
            new ComboBoxApiRow("RightAddOn", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyRightAddOn), "object?", "cyan", "null"),
            new ComboBoxApiRow("ContentLeftAddOn", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyContentLeftAddOn), "object?", "cyan", "null"),
            new ComboBoxApiRow("ContentRightAddOn", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyContentRightAddOn), "object?", "cyan", "null"),
            new ComboBoxApiRow("SizeType", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new ComboBoxApiRow("StyleVariant", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "blue", "Outlined"),
            new ComboBoxApiRow("Status", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "blue", "Default"),
            new ComboBoxApiRow("IsAllowClear", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyIsAllowClear), "bool", "purple", "false"),
            new ComboBoxApiRow("OptionFontSize", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyOptionFontSize), "double", "cyan", "token"),
            new ComboBoxApiRow("DropDownDisplayPageSize", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyDropDownDisplayPageSize), "int", "cyan", "10"),
            new ComboBoxApiRow("ShouldUseOverlayPopup", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup), "bool", "purple", "true"),
            new ComboBoxApiRow("IsMotionEnabled", Lang(ComboBoxShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token")
        ];
    }

    public void EnsureDesignTokenRows()
    {
        if (DesignTokenRows is not null)
        {
            return;
        }

        DesignTokenRows =
        [
            new ComboBoxDesignTokenRow("ControlWidth", Lang(ComboBoxShowCaseLangResourceKind.TokenNameControlWidth), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("HandleWidth", Lang(ComboBoxShowCaseLangResourceKind.TokenNameHandleWidth), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("HandleIconSize", Lang(ComboBoxShowCaseLangResourceKind.TokenNameHandleIconSize), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("HandleBg", Lang(ComboBoxShowCaseLangResourceKind.TokenNameHandleBg), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("HandleActiveBg", Lang(ComboBoxShowCaseLangResourceKind.TokenNameHandleActiveBg), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("HandleHoverColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameHandleHoverColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("HandleBorderColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameHandleBorderColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("FilledHandleBg", Lang(ComboBoxShowCaseLangResourceKind.TokenNameFilledHandleBg), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("InputFontSize", Lang(ComboBoxShowCaseLangResourceKind.TokenNameInputFontSize), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("InputFontSizeLG", Lang(ComboBoxShowCaseLangResourceKind.TokenNameInputFontSizeLG), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("InputFontSizeSM", Lang(ComboBoxShowCaseLangResourceKind.TokenNameInputFontSizeSM), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("PopupContentPadding", Lang(ComboBoxShowCaseLangResourceKind.TokenNamePopupContentPadding), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemHoverColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemHoverColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemSelectedColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemSelectedColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemDisabledColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemDisabledColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemBgColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemBgColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemHoverBgColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemHoverBgColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemSelectedBgColor", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemSelectedBgColor), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemPadding", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemPadding), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ComboBoxDesignTokenRow("ItemMargin", Lang(ComboBoxShowCaseLangResourceKind.TokenNameItemMargin), Lang(ComboBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ComboBoxShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static List<ComboBoxItemData> CreateComboBoxItems()
    {
        return
        [
            new ComboBoxItemData { Text = "床前明月光" },
            new ComboBoxItemData { Text = "疑是地上霜" },
            new ComboBoxItemData { Text = "举头望明月" },
            new ComboBoxItemData { Text = "低头思故乡" }
        ];
    }

    private static string Lang(ComboBoxShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ComboBoxShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ComboBoxShowCaseLangResourceKind.ApiPropertyItemsSource              => en_US.ApiPropertyItemsSource,
            ComboBoxShowCaseLangResourceKind.ApiPropertySelectedItem             => en_US.ApiPropertySelectedItem,
            ComboBoxShowCaseLangResourceKind.ApiPropertySelectedIndex            => en_US.ApiPropertySelectedIndex,
            ComboBoxShowCaseLangResourceKind.ApiPropertyPlaceholderText          => en_US.ApiPropertyPlaceholderText,
            ComboBoxShowCaseLangResourceKind.ApiPropertyLeftAddOn                => en_US.ApiPropertyLeftAddOn,
            ComboBoxShowCaseLangResourceKind.ApiPropertyRightAddOn               => en_US.ApiPropertyRightAddOn,
            ComboBoxShowCaseLangResourceKind.ApiPropertyContentLeftAddOn         => en_US.ApiPropertyContentLeftAddOn,
            ComboBoxShowCaseLangResourceKind.ApiPropertyContentRightAddOn        => en_US.ApiPropertyContentRightAddOn,
            ComboBoxShowCaseLangResourceKind.ApiPropertySizeType                 => en_US.ApiPropertySizeType,
            ComboBoxShowCaseLangResourceKind.ApiPropertyStyleVariant             => en_US.ApiPropertyStyleVariant,
            ComboBoxShowCaseLangResourceKind.ApiPropertyStatus                   => en_US.ApiPropertyStatus,
            ComboBoxShowCaseLangResourceKind.ApiPropertyIsAllowClear             => en_US.ApiPropertyIsAllowClear,
            ComboBoxShowCaseLangResourceKind.ApiPropertyOptionFontSize           => en_US.ApiPropertyOptionFontSize,
            ComboBoxShowCaseLangResourceKind.ApiPropertyDropDownDisplayPageSize  => en_US.ApiPropertyDropDownDisplayPageSize,
            ComboBoxShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup    => en_US.ApiPropertyShouldUseOverlayPopup,
            ComboBoxShowCaseLangResourceKind.ApiPropertyIsMotionEnabled          => en_US.ApiPropertyIsMotionEnabled,
            ComboBoxShowCaseLangResourceKind.TokenNameControlWidth               => en_US.TokenNameControlWidth,
            ComboBoxShowCaseLangResourceKind.TokenNameHandleWidth                => en_US.TokenNameHandleWidth,
            ComboBoxShowCaseLangResourceKind.TokenNameHandleIconSize             => en_US.TokenNameHandleIconSize,
            ComboBoxShowCaseLangResourceKind.TokenNameHandleBg                   => en_US.TokenNameHandleBg,
            ComboBoxShowCaseLangResourceKind.TokenNameHandleActiveBg             => en_US.TokenNameHandleActiveBg,
            ComboBoxShowCaseLangResourceKind.TokenNameHandleHoverColor           => en_US.TokenNameHandleHoverColor,
            ComboBoxShowCaseLangResourceKind.TokenNameHandleBorderColor          => en_US.TokenNameHandleBorderColor,
            ComboBoxShowCaseLangResourceKind.TokenNameFilledHandleBg             => en_US.TokenNameFilledHandleBg,
            ComboBoxShowCaseLangResourceKind.TokenNameInputFontSize              => en_US.TokenNameInputFontSize,
            ComboBoxShowCaseLangResourceKind.TokenNameInputFontSizeLG            => en_US.TokenNameInputFontSizeLG,
            ComboBoxShowCaseLangResourceKind.TokenNameInputFontSizeSM            => en_US.TokenNameInputFontSizeSM,
            ComboBoxShowCaseLangResourceKind.TokenNamePopupContentPadding        => en_US.TokenNamePopupContentPadding,
            ComboBoxShowCaseLangResourceKind.TokenNameItemColor                  => en_US.TokenNameItemColor,
            ComboBoxShowCaseLangResourceKind.TokenNameItemHoverColor             => en_US.TokenNameItemHoverColor,
            ComboBoxShowCaseLangResourceKind.TokenNameItemSelectedColor          => en_US.TokenNameItemSelectedColor,
            ComboBoxShowCaseLangResourceKind.TokenNameItemDisabledColor          => en_US.TokenNameItemDisabledColor,
            ComboBoxShowCaseLangResourceKind.TokenNameItemBgColor                => en_US.TokenNameItemBgColor,
            ComboBoxShowCaseLangResourceKind.TokenNameItemHoverBgColor           => en_US.TokenNameItemHoverBgColor,
            ComboBoxShowCaseLangResourceKind.TokenNameItemSelectedBgColor        => en_US.TokenNameItemSelectedBgColor,
            ComboBoxShowCaseLangResourceKind.TokenNameItemPadding                => en_US.TokenNameItemPadding,
            ComboBoxShowCaseLangResourceKind.TokenNameItemMargin                 => en_US.TokenNameItemMargin,
            ComboBoxShowCaseLangResourceKind.TokenScopeComponent                 => en_US.TokenScopeComponent,
            ComboBoxShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }
}

public class ComboBoxItemData
{
    public string Text { get; set; } = string.Empty;
}

public sealed record ComboBoxApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ComboBoxDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
