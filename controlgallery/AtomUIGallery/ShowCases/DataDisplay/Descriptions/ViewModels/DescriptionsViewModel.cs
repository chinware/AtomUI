using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Descriptions;

public class DescriptionsViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Descriptions";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<DescriptionsApiRow>? _apiRows;
    private ObservableCollection<DescriptionsDesignTokenRow>? _designTokenRows;
    private SizeType _descriptionsSizeType;

    public ObservableCollection<DescriptionsApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<DescriptionsDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public SizeType DescriptionsSizeType
    {
        get => _descriptionsSizeType;
        set => this.RaiseAndSetIfChanged(ref _descriptionsSizeType, value);
    }

    public DescriptionsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new DescriptionsApiRow("IsBordered", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyIsBordered), "bool", "green", "false"),
            new DescriptionsApiRow("IsShowColon", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyIsShowColon), "bool", "green", "true"),
            new DescriptionsApiRow("ColumnInfo", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyColumnInfo), "DescriptionsMediaBreakInfo", "blue", "3"),
            new DescriptionsApiRow("Header", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyHeader), "object?", "cyan", "null"),
            new DescriptionsApiRow("HeaderTemplate", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyHeaderTemplate), "IDataTemplate?", "cyan", "null"),
            new DescriptionsApiRow("Extra", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyExtra), "object?", "cyan", "null"),
            new DescriptionsApiRow("ExtraTemplate", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyExtraTemplate), "IDataTemplate?", "cyan", "null"),
            new DescriptionsApiRow("Layout", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyLayout), "Orientation", "blue", "Horizontal"),
            new DescriptionsApiRow("SizeType", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Large"),
            new DescriptionsApiRow("ItemsSource", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "cyan", "null"),
            new DescriptionsApiRow("Items", Lang(DescriptionsShowCaseLangResourceKind.ApiPropertyItems), "DescriptionItems", "cyan", "Empty")
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
            new DescriptionsDesignTokenRow("LabelBg", Lang(DescriptionsShowCaseLangResourceKind.TokenNameLabelBg), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("LabelColor", Lang(DescriptionsShowCaseLangResourceKind.TokenNameLabelColor), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("TitleColor", Lang(DescriptionsShowCaseLangResourceKind.TokenNameTitleColor), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("HeaderMargin", Lang(DescriptionsShowCaseLangResourceKind.TokenNameHeaderMargin), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("ItemPaddingLG", Lang(DescriptionsShowCaseLangResourceKind.TokenNameItemPaddingLG), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("ItemPadding", Lang(DescriptionsShowCaseLangResourceKind.TokenNameItemPadding), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("ItemPaddingSM", Lang(DescriptionsShowCaseLangResourceKind.TokenNameItemPaddingSM), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("ColonMargin", Lang(DescriptionsShowCaseLangResourceKind.TokenNameColonMargin), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("ContentColor", Lang(DescriptionsShowCaseLangResourceKind.TokenNameContentColor), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DescriptionsDesignTokenRow("ExtraColor", Lang(DescriptionsShowCaseLangResourceKind.TokenNameExtraColor), Lang(DescriptionsShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DescriptionsShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(DescriptionsShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(DescriptionsShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            DescriptionsShowCaseLangResourceKind.ApiPropertyIsBordered       => en_US.ApiPropertyIsBordered,
            DescriptionsShowCaseLangResourceKind.ApiPropertyIsShowColon      => en_US.ApiPropertyIsShowColon,
            DescriptionsShowCaseLangResourceKind.ApiPropertyColumnInfo       => en_US.ApiPropertyColumnInfo,
            DescriptionsShowCaseLangResourceKind.ApiPropertyHeader           => en_US.ApiPropertyHeader,
            DescriptionsShowCaseLangResourceKind.ApiPropertyHeaderTemplate   => en_US.ApiPropertyHeaderTemplate,
            DescriptionsShowCaseLangResourceKind.ApiPropertyExtra            => en_US.ApiPropertyExtra,
            DescriptionsShowCaseLangResourceKind.ApiPropertyExtraTemplate    => en_US.ApiPropertyExtraTemplate,
            DescriptionsShowCaseLangResourceKind.ApiPropertyLayout           => en_US.ApiPropertyLayout,
            DescriptionsShowCaseLangResourceKind.ApiPropertySizeType         => en_US.ApiPropertySizeType,
            DescriptionsShowCaseLangResourceKind.ApiPropertyItemsSource      => en_US.ApiPropertyItemsSource,
            DescriptionsShowCaseLangResourceKind.ApiPropertyItems            => en_US.ApiPropertyItems,
            DescriptionsShowCaseLangResourceKind.TokenNameLabelBg            => en_US.TokenNameLabelBg,
            DescriptionsShowCaseLangResourceKind.TokenNameLabelColor         => en_US.TokenNameLabelColor,
            DescriptionsShowCaseLangResourceKind.TokenNameTitleColor         => en_US.TokenNameTitleColor,
            DescriptionsShowCaseLangResourceKind.TokenNameHeaderMargin       => en_US.TokenNameHeaderMargin,
            DescriptionsShowCaseLangResourceKind.TokenNameItemPaddingLG      => en_US.TokenNameItemPaddingLG,
            DescriptionsShowCaseLangResourceKind.TokenNameItemPadding        => en_US.TokenNameItemPadding,
            DescriptionsShowCaseLangResourceKind.TokenNameItemPaddingSM      => en_US.TokenNameItemPaddingSM,
            DescriptionsShowCaseLangResourceKind.TokenNameColonMargin        => en_US.TokenNameColonMargin,
            DescriptionsShowCaseLangResourceKind.TokenNameContentColor       => en_US.TokenNameContentColor,
            DescriptionsShowCaseLangResourceKind.TokenNameExtraColor         => en_US.TokenNameExtraColor,
            DescriptionsShowCaseLangResourceKind.TokenScopeComponent         => en_US.TokenScopeComponent,
            DescriptionsShowCaseLangResourceKind.TokenStatusStable           => en_US.TokenStatusStable,
            _                                                                => kind.ToString()
        };
    }
}

public sealed record DescriptionsApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record DescriptionsDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
