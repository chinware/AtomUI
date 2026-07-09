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

namespace AtomUIGallery.ShowCases.TabControl;

public class TabControlViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "TabControl";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private Dock _positionTabControlPlacement = Dock.Top;

    public Dock PositionTabControlPlacement
    {
        get => _positionTabControlPlacement;
        set => this.RaiseAndSetIfChanged(ref _positionTabControlPlacement, value);
    }

    private Dock _positionCardTabControlPlacement = Dock.Top;

    public Dock PositionCardTabControlPlacement
    {
        get => _positionCardTabControlPlacement;
        set => this.RaiseAndSetIfChanged(ref _positionCardTabControlPlacement, value);
    }

    private SizeType _sizeTypeControl = SizeType.Middle;
    private ObservableCollection<TabControlApiRow>? _apiRows;
    private ObservableCollection<TabControlDesignTokenRow>? _designTokenRows;

    public SizeType SizeTypeTabControl
    {
        get => _sizeTypeControl;
        set => this.RaiseAndSetIfChanged(ref _sizeTypeControl, value);
    }

    public AvaloniaList<TabItemData> TabItemDataSource { get; set; } = new();

    public ObservableCollection<TabControlApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<TabControlDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public TabControlViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void HandleTabControlPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PositionTabControlPlacement = args.Index switch
        {
            0 => Dock.Top,
            1 => Dock.Bottom,
            2 => Dock.Left,
            _ => Dock.Right
        };
    }

    public void HandleCardTabControlPlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PositionCardTabControlPlacement = args.Index switch
        {
            0 => Dock.Top,
            1 => Dock.Bottom,
            2 => Dock.Left,
            _ => Dock.Right
        };
    }

    public void HandleTabControlSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        SizeTypeTabControl = args.Index switch
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
            new TabControlApiRow("SelectedIndex", Lang(TabControlShowCaseLangResourceKind.ApiPropertySelectedIndex), "int", "cyan", "0"),
            new TabControlApiRow("SelectedItem", Lang(TabControlShowCaseLangResourceKind.ApiPropertySelectedItem), "object?", "cyan", "null"),
            new TabControlApiRow("ItemsSource", Lang(TabControlShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "blue", "null"),
            new TabControlApiRow("ItemTemplate", Lang(TabControlShowCaseLangResourceKind.ApiPropertyItemTemplate), "IDataTemplate?", "blue", "null"),
            new TabControlApiRow("TabStripPlacement", Lang(TabControlShowCaseLangResourceKind.ApiPropertyTabStripPlacement), "Dock", "blue", "Top"),
            new TabControlApiRow("TabAlignmentCenter", Lang(TabControlShowCaseLangResourceKind.ApiPropertyTabAlignmentCenter), "bool", "purple", "false"),
            new TabControlApiRow("IsTabReorderEnabled", Lang(TabControlShowCaseLangResourceKind.ApiPropertyIsTabReorderEnabled), "bool", "purple", "false"),
            new TabControlApiRow("TabActivationTrigger", Lang(TabControlShowCaseLangResourceKind.ApiPropertyTabActivationTrigger), "TabActivationTrigger", "blue", "PointerReleased"),
            new TabControlApiRow("SizeType", Lang(TabControlShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new TabControlApiRow("IsTabClosable", Lang(TabControlShowCaseLangResourceKind.ApiPropertyIsTabClosable), "bool", "purple", "false"),
            new TabControlApiRow("IsTabAutoHideCloseButton", Lang(TabControlShowCaseLangResourceKind.ApiPropertyIsTabAutoHideCloseButton), "bool", "purple", "false"),
            new TabControlApiRow("HeaderStartExtraContent", Lang(TabControlShowCaseLangResourceKind.ApiPropertyHeaderStartExtraContent), "object?", "cyan", "null"),
            new TabControlApiRow("HeaderEndExtraContent", Lang(TabControlShowCaseLangResourceKind.ApiPropertyHeaderEndExtraContent), "object?", "cyan", "null"),
            new TabControlApiRow("IsShowAddTabButton", Lang(TabControlShowCaseLangResourceKind.ApiPropertyIsShowAddTabButton), "bool", "purple", "false"),
            new TabControlApiRow("AddTabRequest", Lang(TabControlShowCaseLangResourceKind.ApiPropertyAddTabRequest), "event", "default", "null"),
            new TabControlApiRow("TabReordering", Lang(TabControlShowCaseLangResourceKind.ApiEventTabReordering), "event EventHandler<TabReorderingEventArgs>?", "default", "null"),
            new TabControlApiRow("TabReordered", Lang(TabControlShowCaseLangResourceKind.ApiEventTabReordered), "event EventHandler<TabReorderedEventArgs>?", "default", "null")
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
            new TabControlDesignTokenRow("CardBg", Lang(TabControlShowCaseLangResourceKind.TokenNameCardBg), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("CardSize", Lang(TabControlShowCaseLangResourceKind.TokenNameCardSize), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("CardPadding", Lang(TabControlShowCaseLangResourceKind.TokenNameCardPadding), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("TitleFontSize", Lang(TabControlShowCaseLangResourceKind.TokenNameTitleFontSize), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("InkBarColor", Lang(TabControlShowCaseLangResourceKind.TokenNameInkBarColor), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("HorizontalItemGutter", Lang(TabControlShowCaseLangResourceKind.TokenNameHorizontalItemGutter), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("HorizontalItemPadding", Lang(TabControlShowCaseLangResourceKind.TokenNameHorizontalItemPadding), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("VerticalItemPadding", Lang(TabControlShowCaseLangResourceKind.TokenNameVerticalItemPadding), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("ItemColor", Lang(TabControlShowCaseLangResourceKind.TokenNameItemColor), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("ItemSelectedColor", Lang(TabControlShowCaseLangResourceKind.TokenNameItemSelectedColor), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("CardGutter", Lang(TabControlShowCaseLangResourceKind.TokenNameCardGutter), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("AddTabButtonMarginHorizontal", Lang(TabControlShowCaseLangResourceKind.TokenNameAddTabButtonMarginHorizontal), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("CloseIconMargin", Lang(TabControlShowCaseLangResourceKind.TokenNameCloseIconMargin), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TabControlDesignTokenRow("TabAndContentGutter", Lang(TabControlShowCaseLangResourceKind.TokenNameTabAndContentGutter), Lang(TabControlShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TabControlShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TabControlShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TabControlShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TabControlShowCaseLangResourceKind.ApiPropertySelectedIndex              => en_US.ApiPropertySelectedIndex,
            TabControlShowCaseLangResourceKind.ApiPropertySelectedItem               => en_US.ApiPropertySelectedItem,
            TabControlShowCaseLangResourceKind.ApiPropertyItemsSource                => en_US.ApiPropertyItemsSource,
            TabControlShowCaseLangResourceKind.ApiPropertyItemTemplate               => en_US.ApiPropertyItemTemplate,
            TabControlShowCaseLangResourceKind.ApiPropertyTabStripPlacement          => en_US.ApiPropertyTabStripPlacement,
            TabControlShowCaseLangResourceKind.ApiPropertyTabAlignmentCenter         => en_US.ApiPropertyTabAlignmentCenter,
            TabControlShowCaseLangResourceKind.ApiPropertyIsTabReorderEnabled        => en_US.ApiPropertyIsTabReorderEnabled,
            TabControlShowCaseLangResourceKind.ApiPropertyTabActivationTrigger       => en_US.ApiPropertyTabActivationTrigger,
            TabControlShowCaseLangResourceKind.ApiPropertySizeType                   => en_US.ApiPropertySizeType,
            TabControlShowCaseLangResourceKind.ApiPropertyIsTabClosable              => en_US.ApiPropertyIsTabClosable,
            TabControlShowCaseLangResourceKind.ApiPropertyIsTabAutoHideCloseButton   => en_US.ApiPropertyIsTabAutoHideCloseButton,
            TabControlShowCaseLangResourceKind.ApiPropertyHeaderStartExtraContent    => en_US.ApiPropertyHeaderStartExtraContent,
            TabControlShowCaseLangResourceKind.ApiPropertyHeaderEndExtraContent      => en_US.ApiPropertyHeaderEndExtraContent,
            TabControlShowCaseLangResourceKind.ApiPropertyIsShowAddTabButton         => en_US.ApiPropertyIsShowAddTabButton,
            TabControlShowCaseLangResourceKind.ApiPropertyAddTabRequest              => en_US.ApiPropertyAddTabRequest,
            TabControlShowCaseLangResourceKind.ApiEventTabReordering                 => en_US.ApiEventTabReordering,
            TabControlShowCaseLangResourceKind.ApiEventTabReordered                  => en_US.ApiEventTabReordered,
            TabControlShowCaseLangResourceKind.TokenNameCardBg                       => en_US.TokenNameCardBg,
            TabControlShowCaseLangResourceKind.TokenNameCardSize                     => en_US.TokenNameCardSize,
            TabControlShowCaseLangResourceKind.TokenNameCardPadding                  => en_US.TokenNameCardPadding,
            TabControlShowCaseLangResourceKind.TokenNameTitleFontSize                => en_US.TokenNameTitleFontSize,
            TabControlShowCaseLangResourceKind.TokenNameInkBarColor                  => en_US.TokenNameInkBarColor,
            TabControlShowCaseLangResourceKind.TokenNameHorizontalItemGutter         => en_US.TokenNameHorizontalItemGutter,
            TabControlShowCaseLangResourceKind.TokenNameHorizontalItemPadding        => en_US.TokenNameHorizontalItemPadding,
            TabControlShowCaseLangResourceKind.TokenNameVerticalItemPadding          => en_US.TokenNameVerticalItemPadding,
            TabControlShowCaseLangResourceKind.TokenNameItemColor                    => en_US.TokenNameItemColor,
            TabControlShowCaseLangResourceKind.TokenNameItemSelectedColor            => en_US.TokenNameItemSelectedColor,
            TabControlShowCaseLangResourceKind.TokenNameCardGutter                   => en_US.TokenNameCardGutter,
            TabControlShowCaseLangResourceKind.TokenNameAddTabButtonMarginHorizontal => en_US.TokenNameAddTabButtonMarginHorizontal,
            TabControlShowCaseLangResourceKind.TokenNameCloseIconMargin              => en_US.TokenNameCloseIconMargin,
            TabControlShowCaseLangResourceKind.TokenNameTabAndContentGutter          => en_US.TokenNameTabAndContentGutter,
            TabControlShowCaseLangResourceKind.TokenScopeComponent                   => en_US.TokenScopeComponent,
            TabControlShowCaseLangResourceKind.TokenStatusStable                     => en_US.TokenStatusStable,
            _                                                                        => kind.ToString()
        };
    }
}

public sealed record TabControlApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TabControlDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
