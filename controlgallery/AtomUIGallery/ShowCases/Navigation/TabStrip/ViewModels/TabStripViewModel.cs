using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.TabStrip;

public class TabStripViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "TabStrip";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private Dock _positionTabStripPlacement = Dock.Top;

    public Dock PositionTabStripPlacement
    {
        get => _positionTabStripPlacement;
        set => this.RaiseAndSetIfChanged(ref _positionTabStripPlacement, value);
    }

    private Dock _positionCardTabStripPlacement = Dock.Top;

    public Dock PositionCardTabStripPlacement
    {
        get => _positionCardTabStripPlacement;
        set => this.RaiseAndSetIfChanged(ref _positionCardTabStripPlacement, value);
    }

    private SizeType _sizeTypeTabStrip = SizeType.Middle;
    private ObservableCollection<TabStripApiRow>? _apiRows;
    private ObservableCollection<TabStripDesignTokenRow>? _designTokenRows;

    public SizeType SizeTypeTabStrip
    {
        get => _sizeTypeTabStrip;
        set => this.RaiseAndSetIfChanged(ref _sizeTypeTabStrip, value);
    }

    public AvaloniaList<TabItemData> TabStripItemDataSource { get; set; } = new();

    public ObservableCollection<TabStripApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<TabStripDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public TabStripViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void HandleTabStripPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PositionTabStripPlacement = args.Index switch
        {
            0 => Dock.Top,
            1 => Dock.Bottom,
            2 => Dock.Left,
            _ => Dock.Right
        };
    }

    public void HandleCardTabStripPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PositionCardTabStripPlacement = args.Index switch
        {
            0 => Dock.Top,
            1 => Dock.Bottom,
            2 => Dock.Left,
            _ => Dock.Right
        };
    }

    public void HandleTabStripSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        SizeTypeTabStrip = args.Index switch
        {
            0 => SizeType.Small,
            1 => SizeType.Middle,
            _ => SizeType.Large
        };
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new TabStripApiRow("SelectedIndex", Lang(TabStripShowCaseLangResourceKind.ApiPropertySelectedIndex), "int", "cyan", "0"),
            new TabStripApiRow("SelectedItem", Lang(TabStripShowCaseLangResourceKind.ApiPropertySelectedItem), "object?", "cyan", "null"),
            new TabStripApiRow("ItemsSource", Lang(TabStripShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "blue", "null"),
            new TabStripApiRow("ItemTemplate", Lang(TabStripShowCaseLangResourceKind.ApiPropertyItemTemplate), "IDataTemplate?", "blue", "null"),
            new TabStripApiRow("TabStripPlacement", Lang(TabStripShowCaseLangResourceKind.ApiPropertyTabStripPlacement), "Dock", "blue", "Top"),
            new TabStripApiRow("TabAlignmentCenter", Lang(TabStripShowCaseLangResourceKind.ApiPropertyTabAlignmentCenter), "bool", "purple", "false"),
            new TabStripApiRow("SizeType", Lang(TabStripShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new TabStripApiRow("IsTabClosable", Lang(TabStripShowCaseLangResourceKind.ApiPropertyIsTabClosable), "bool", "purple", "false"),
            new TabStripApiRow("IsTabAutoHideCloseButton", Lang(TabStripShowCaseLangResourceKind.ApiPropertyIsTabAutoHideCloseButton), "bool", "purple", "false"),
            new TabStripApiRow("HeaderStartExtraContent", Lang(TabStripShowCaseLangResourceKind.ApiPropertyHeaderStartExtraContent), "object?", "cyan", "null"),
            new TabStripApiRow("HeaderEndExtraContent", Lang(TabStripShowCaseLangResourceKind.ApiPropertyHeaderEndExtraContent), "object?", "cyan", "null"),
            new TabStripApiRow("IsShowAddTabButton", Lang(TabStripShowCaseLangResourceKind.ApiPropertyIsShowAddTabButton), "bool", "purple", "false"),
            new TabStripApiRow("AddTabRequest", Lang(TabStripShowCaseLangResourceKind.ApiPropertyAddTabRequest), "event", "default", "null")
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
            new TabStripDesignTokenRow("CardBg", Lang(TabStripShowCaseLangResourceKind.TokenNameCardBg), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("CardSize", Lang(TabStripShowCaseLangResourceKind.TokenNameCardSize), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("CardPadding", Lang(TabStripShowCaseLangResourceKind.TokenNameCardPadding), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("TitleFontSize", Lang(TabStripShowCaseLangResourceKind.TokenNameTitleFontSize), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("InkBarColor", Lang(TabStripShowCaseLangResourceKind.TokenNameInkBarColor), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("HorizontalItemGutter", Lang(TabStripShowCaseLangResourceKind.TokenNameHorizontalItemGutter), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("HorizontalItemPadding", Lang(TabStripShowCaseLangResourceKind.TokenNameHorizontalItemPadding), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("VerticalItemPadding", Lang(TabStripShowCaseLangResourceKind.TokenNameVerticalItemPadding), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("ItemColor", Lang(TabStripShowCaseLangResourceKind.TokenNameItemColor), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("ItemSelectedColor", Lang(TabStripShowCaseLangResourceKind.TokenNameItemSelectedColor), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("CardGutter", Lang(TabStripShowCaseLangResourceKind.TokenNameCardGutter), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("AddTabButtonMarginHorizontal", Lang(TabStripShowCaseLangResourceKind.TokenNameAddTabButtonMarginHorizontal), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("CloseIconMargin", Lang(TabStripShowCaseLangResourceKind.TokenNameCloseIconMargin), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabStripDesignTokenRow("TabAndContentGutter", Lang(TabStripShowCaseLangResourceKind.TokenNameTabAndContentGutter), Lang(TabStripShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabStripShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TabStripShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TabStripShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TabStripShowCaseLangResourceKind.ApiPropertySelectedIndex              => en_US.ApiPropertySelectedIndex,
            TabStripShowCaseLangResourceKind.ApiPropertySelectedItem               => en_US.ApiPropertySelectedItem,
            TabStripShowCaseLangResourceKind.ApiPropertyItemsSource                => en_US.ApiPropertyItemsSource,
            TabStripShowCaseLangResourceKind.ApiPropertyItemTemplate               => en_US.ApiPropertyItemTemplate,
            TabStripShowCaseLangResourceKind.ApiPropertyTabStripPlacement          => en_US.ApiPropertyTabStripPlacement,
            TabStripShowCaseLangResourceKind.ApiPropertyTabAlignmentCenter         => en_US.ApiPropertyTabAlignmentCenter,
            TabStripShowCaseLangResourceKind.ApiPropertySizeType                   => en_US.ApiPropertySizeType,
            TabStripShowCaseLangResourceKind.ApiPropertyIsTabClosable              => en_US.ApiPropertyIsTabClosable,
            TabStripShowCaseLangResourceKind.ApiPropertyIsTabAutoHideCloseButton   => en_US.ApiPropertyIsTabAutoHideCloseButton,
            TabStripShowCaseLangResourceKind.ApiPropertyHeaderStartExtraContent    => en_US.ApiPropertyHeaderStartExtraContent,
            TabStripShowCaseLangResourceKind.ApiPropertyHeaderEndExtraContent      => en_US.ApiPropertyHeaderEndExtraContent,
            TabStripShowCaseLangResourceKind.ApiPropertyIsShowAddTabButton         => en_US.ApiPropertyIsShowAddTabButton,
            TabStripShowCaseLangResourceKind.ApiPropertyAddTabRequest              => en_US.ApiPropertyAddTabRequest,
            TabStripShowCaseLangResourceKind.TokenNameCardBg                       => en_US.TokenNameCardBg,
            TabStripShowCaseLangResourceKind.TokenNameCardSize                     => en_US.TokenNameCardSize,
            TabStripShowCaseLangResourceKind.TokenNameCardPadding                  => en_US.TokenNameCardPadding,
            TabStripShowCaseLangResourceKind.TokenNameTitleFontSize                => en_US.TokenNameTitleFontSize,
            TabStripShowCaseLangResourceKind.TokenNameInkBarColor                  => en_US.TokenNameInkBarColor,
            TabStripShowCaseLangResourceKind.TokenNameHorizontalItemGutter         => en_US.TokenNameHorizontalItemGutter,
            TabStripShowCaseLangResourceKind.TokenNameHorizontalItemPadding        => en_US.TokenNameHorizontalItemPadding,
            TabStripShowCaseLangResourceKind.TokenNameVerticalItemPadding          => en_US.TokenNameVerticalItemPadding,
            TabStripShowCaseLangResourceKind.TokenNameItemColor                    => en_US.TokenNameItemColor,
            TabStripShowCaseLangResourceKind.TokenNameItemSelectedColor            => en_US.TokenNameItemSelectedColor,
            TabStripShowCaseLangResourceKind.TokenNameCardGutter                   => en_US.TokenNameCardGutter,
            TabStripShowCaseLangResourceKind.TokenNameAddTabButtonMarginHorizontal => en_US.TokenNameAddTabButtonMarginHorizontal,
            TabStripShowCaseLangResourceKind.TokenNameCloseIconMargin              => en_US.TokenNameCloseIconMargin,
            TabStripShowCaseLangResourceKind.TokenNameTabAndContentGutter          => en_US.TokenNameTabAndContentGutter,
            TabStripShowCaseLangResourceKind.TokenScopeComponent                   => en_US.TokenScopeComponent,
            TabStripShowCaseLangResourceKind.TokenStatusStable                     => en_US.TokenStatusStable,
            _                                                                      => kind.ToString()
        };
    }
}

public sealed record TabStripApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TabStripDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
