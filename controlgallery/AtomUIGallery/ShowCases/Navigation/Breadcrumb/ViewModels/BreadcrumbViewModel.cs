using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Breadcrumb;

public class BreadcrumbViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Breadcrumb";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private List<BreadcrumbItemData>? _breadcrumbItems = [];
    private ObservableCollection<BreadcrumbApiRow>? _apiRows;
    private ObservableCollection<BreadcrumbDesignTokenRow>? _designTokenRows;

    public List<BreadcrumbItemData>? BreadcrumbItems
    {
        get => _breadcrumbItems;
        set => this.RaiseAndSetIfChanged(ref _breadcrumbItems, value);
    }

    public ObservableCollection<BreadcrumbApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<BreadcrumbDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public BreadcrumbViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
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
            new BreadcrumbApiRow("Breadcrumb.Separator", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertySeparator), "object?", "cyan", "\"/\""),
            new BreadcrumbApiRow("Breadcrumb.SeparatorTemplate", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertySeparatorTemplate), "IDataTemplate?", "cyan", "null"),
            new BreadcrumbApiRow("Breadcrumb.IsMotionEnabled", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token"),
            new BreadcrumbApiRow("Breadcrumb.NavigateRequest", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertyNavigateRequest), "event", "default", "null"),
            new BreadcrumbApiRow("BreadcrumbItem.Icon", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertyIcon), "PathIcon?", "cyan", "null"),
            new BreadcrumbApiRow("BreadcrumbItem.NavigateContext", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertyNavigateContext), "object?", "cyan", "null"),
            new BreadcrumbApiRow("BreadcrumbItem.NavigateUri", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertyNavigateUri), "Uri?", "cyan", "null"),
            new BreadcrumbApiRow("BreadcrumbItem.Separator", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertyItemSeparator), "object?", "cyan", "\"/\""),
            new BreadcrumbApiRow("BreadcrumbItem.SeparatorTemplate", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertyItemSeparatorTemplate), "IDataTemplate?", "cyan", "null"),
            new BreadcrumbApiRow("BreadcrumbItemData.Content", Lang(BreadcrumbShowCaseLangResourceKind.ApiPropertyItemDataContent), "object?", "cyan", "null")
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
            new BreadcrumbDesignTokenRow("IconSize", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameIconSize), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BreadcrumbDesignTokenRow("ItemColor", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameItemColor), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BreadcrumbDesignTokenRow("LastItemColor", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameLastItemColor), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BreadcrumbDesignTokenRow("LinkColor", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameLinkColor), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BreadcrumbDesignTokenRow("LinkHoverColor", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameLinkHoverColor), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BreadcrumbDesignTokenRow("LinkHoverBgColor", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameLinkHoverBgColor), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BreadcrumbDesignTokenRow("BreadcrumbItemContentPadding", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameBreadcrumbItemContentPadding), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BreadcrumbDesignTokenRow("SeparatorColor", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameSeparatorColor), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success"),
            new BreadcrumbDesignTokenRow("SeparatorMargin", Lang(BreadcrumbShowCaseLangResourceKind.TokenNameSeparatorMargin), Lang(BreadcrumbShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(BreadcrumbShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(BreadcrumbShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(BreadcrumbShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            BreadcrumbShowCaseLangResourceKind.ApiPropertySeparator                    => en_US.ApiPropertySeparator,
            BreadcrumbShowCaseLangResourceKind.ApiPropertySeparatorTemplate            => en_US.ApiPropertySeparatorTemplate,
            BreadcrumbShowCaseLangResourceKind.ApiPropertyIsMotionEnabled              => en_US.ApiPropertyIsMotionEnabled,
            BreadcrumbShowCaseLangResourceKind.ApiPropertyNavigateRequest              => en_US.ApiPropertyNavigateRequest,
            BreadcrumbShowCaseLangResourceKind.ApiPropertyIcon                         => en_US.ApiPropertyIcon,
            BreadcrumbShowCaseLangResourceKind.ApiPropertyNavigateContext              => en_US.ApiPropertyNavigateContext,
            BreadcrumbShowCaseLangResourceKind.ApiPropertyNavigateUri                  => en_US.ApiPropertyNavigateUri,
            BreadcrumbShowCaseLangResourceKind.ApiPropertyItemSeparator                => en_US.ApiPropertyItemSeparator,
            BreadcrumbShowCaseLangResourceKind.ApiPropertyItemSeparatorTemplate        => en_US.ApiPropertyItemSeparatorTemplate,
            BreadcrumbShowCaseLangResourceKind.ApiPropertyItemDataContent              => en_US.ApiPropertyItemDataContent,
            BreadcrumbShowCaseLangResourceKind.TokenNameIconSize                       => en_US.TokenNameIconSize,
            BreadcrumbShowCaseLangResourceKind.TokenNameItemColor                      => en_US.TokenNameItemColor,
            BreadcrumbShowCaseLangResourceKind.TokenNameLastItemColor                  => en_US.TokenNameLastItemColor,
            BreadcrumbShowCaseLangResourceKind.TokenNameLinkColor                      => en_US.TokenNameLinkColor,
            BreadcrumbShowCaseLangResourceKind.TokenNameLinkHoverColor                 => en_US.TokenNameLinkHoverColor,
            BreadcrumbShowCaseLangResourceKind.TokenNameLinkHoverBgColor               => en_US.TokenNameLinkHoverBgColor,
            BreadcrumbShowCaseLangResourceKind.TokenNameBreadcrumbItemContentPadding   => en_US.TokenNameBreadcrumbItemContentPadding,
            BreadcrumbShowCaseLangResourceKind.TokenNameSeparatorColor                 => en_US.TokenNameSeparatorColor,
            BreadcrumbShowCaseLangResourceKind.TokenNameSeparatorMargin                => en_US.TokenNameSeparatorMargin,
            BreadcrumbShowCaseLangResourceKind.TokenScopeComponent                     => en_US.TokenScopeComponent,
            BreadcrumbShowCaseLangResourceKind.TokenStatusStable                       => en_US.TokenStatusStable,
            _                                                                         => kind.ToString()
        };
    }
}

public sealed record BreadcrumbApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record BreadcrumbDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
