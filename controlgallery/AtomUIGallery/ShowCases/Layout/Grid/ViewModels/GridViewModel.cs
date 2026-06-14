using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Grid;

public class GridViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "GridShowCase";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<GridApiRow>? _apiRows;
    private ObservableCollection<GridDesignTokenRow>? _designTokenRows;

    public ObservableCollection<GridApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<GridDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public GridViewModel(IScreen screen)
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
            new GridApiRow("Row.Gutter", Lang(GridShowCaseLangResourceKind.ApiPropertyRowGutter), "GridGutter", "cyan", "0"),
            new GridApiRow("Row.Justify", Lang(GridShowCaseLangResourceKind.ApiPropertyRowJustify), "RowJustify", "cyan", "Start"),
            new GridApiRow("Row.Align", Lang(GridShowCaseLangResourceKind.ApiPropertyRowAlign), "RowAlign", "cyan", "Stretch"),
            new GridApiRow("Row.IsWrapped", Lang(GridShowCaseLangResourceKind.ApiPropertyRowIsWrapped), "bool", "cyan", "true"),
            new GridApiRow("Col.Span", Lang(GridShowCaseLangResourceKind.ApiPropertyColSpan), "GridColSpanInfo", "cyan", "24"),
            new GridApiRow("Col.Offset", Lang(GridShowCaseLangResourceKind.ApiPropertyColOffset), "int", "orange", "0"),
            new GridApiRow("Col.Push", Lang(GridShowCaseLangResourceKind.ApiPropertyColPush), "int", "orange", "0"),
            new GridApiRow("Col.Pull", Lang(GridShowCaseLangResourceKind.ApiPropertyColPull), "int", "orange", "0"),
            new GridApiRow("Col.Order", Lang(GridShowCaseLangResourceKind.ApiPropertyColOrder), "int", "orange", "0"),
            new GridApiRow("Col.Xs/Sm/Md/Lg/Xl/Xxl", Lang(GridShowCaseLangResourceKind.ApiPropertyColBreakpoints), "GridColSize?", "cyan", "null"),
            new GridApiRow("ColInfo", Lang(GridShowCaseLangResourceKind.ApiPropertyColInfo), "GridColSize", "cyan", "null")
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
            new GridDesignTokenRow("N/A", Lang(GridShowCaseLangResourceKind.TokenNameNoComponentToken), Lang(GridShowCaseLangResourceKind.TokenScopeComponent), "default", Lang(GridShowCaseLangResourceKind.TokenStatusNotApplicable), "default")
        ];
    }

    private static string Lang(GridShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(GridShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            GridShowCaseLangResourceKind.ApiPropertyRowGutter       => en_US.ApiPropertyRowGutter,
            GridShowCaseLangResourceKind.ApiPropertyRowJustify      => en_US.ApiPropertyRowJustify,
            GridShowCaseLangResourceKind.ApiPropertyRowAlign        => en_US.ApiPropertyRowAlign,
            GridShowCaseLangResourceKind.ApiPropertyRowIsWrapped    => en_US.ApiPropertyRowIsWrapped,
            GridShowCaseLangResourceKind.ApiPropertyColSpan         => en_US.ApiPropertyColSpan,
            GridShowCaseLangResourceKind.ApiPropertyColOffset       => en_US.ApiPropertyColOffset,
            GridShowCaseLangResourceKind.ApiPropertyColPush         => en_US.ApiPropertyColPush,
            GridShowCaseLangResourceKind.ApiPropertyColPull         => en_US.ApiPropertyColPull,
            GridShowCaseLangResourceKind.ApiPropertyColOrder        => en_US.ApiPropertyColOrder,
            GridShowCaseLangResourceKind.ApiPropertyColBreakpoints  => en_US.ApiPropertyColBreakpoints,
            GridShowCaseLangResourceKind.ApiPropertyColInfo         => en_US.ApiPropertyColInfo,
            GridShowCaseLangResourceKind.TokenNameNoComponentToken  => en_US.TokenNameNoComponentToken,
            GridShowCaseLangResourceKind.TokenScopeComponent        => en_US.TokenScopeComponent,
            GridShowCaseLangResourceKind.TokenStatusNotApplicable   => en_US.TokenStatusNotApplicable,
            _                                                       => kind.ToString()
        };
    }
}

public sealed record GridApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record GridDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
