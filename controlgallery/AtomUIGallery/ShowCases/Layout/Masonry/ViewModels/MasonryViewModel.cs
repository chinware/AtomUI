using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Masonry;

public class MasonryViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "MasonryShowCase";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<MasonryApiRow>? _apiRows;
    private ObservableCollection<MasonryDesignTokenRow>? _designTokenRows;
    private ObservableCollection<MasonryBasicItem>? _basicItems;
    private ObservableCollection<MasonryBasicItem>? _responsiveItems;
    private ObservableCollection<MasonryImageItem>? _imageItems;

    public ObservableCollection<MasonryApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<MasonryDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    /// <summary>
    /// Sample items for the basic example, mirroring the Ant Design Masonry basic demo:
    /// 15 cards of varying heights, where the 5th item (index 4) is a special card with
    /// a cover image.
    /// </summary>
    public ObservableCollection<MasonryBasicItem>? BasicItems
    {
        get => _basicItems;
        private set => this.RaiseAndSetIfChanged(ref _basicItems, value);
    }

    public ObservableCollection<MasonryBasicItem>? ResponsiveItems
    {
        get => _responsiveItems;
        private set => this.RaiseAndSetIfChanged(ref _responsiveItems, value);
    }

    public ObservableCollection<MasonryImageItem>? ImageItems
    {
        get => _imageItems;
        private set => this.RaiseAndSetIfChanged(ref _imageItems, value);
    }

    public MasonryViewModel(IScreen screen)
    {
        HostScreen = screen;
        EnsureBasicItems();
        EnsureResponsiveItems();
        EnsureImageItems();
    }

    private void EnsureBasicItems()
    {
        if (BasicItems is not null)
        {
            return;
        }

        // Heights mirror the Ant Design Masonry basic demo. The 5th item (index 4) provides
        // custom children, so its rendered height comes from the special card content.
        var heights = new double[] { 150, 50, 90, 70, 110, 150, 130, 80, 50, 90, 100, 150, 60, 50, 80 };
        var items = new ObservableCollection<MasonryBasicItem>();
        for (var i = 0; i < heights.Length; i++)
        {
            // The 5th item (index 4) is the special card with a cover image.
            if (i == 4)
            {
                items.Add(new MasonryBasicItem(
                    index: i + 1,
                    height: heights[i],
                    isSpecial: true,
                    coverSource: "https://images.unsplash.com/photo-1491961865842-98f7befd1a60?w=523&auto=format",
                    title: "I'm Special",
                    description: "Let's have a meal"));
            }
            else
            {
                items.Add(new MasonryBasicItem(index: i + 1, height: heights[i]));
            }
        }

        BasicItems = items;
    }

    private void EnsureResponsiveItems()
    {
        if (ResponsiveItems is not null)
        {
            return;
        }

        var heights = new double[] { 120, 55, 85, 160, 95, 140, 75, 110, 65, 130, 90, 145, 55, 100, 80 };
        var items = new ObservableCollection<MasonryBasicItem>();
        for (var i = 0; i < heights.Length; i++)
        {
            items.Add(new MasonryBasicItem(index: i + 1, height: heights[i]));
        }

        ResponsiveItems = items;
    }

    private void EnsureImageItems()
    {
        if (ImageItems is not null)
        {
            return;
        }

        var imageSources = new[]
        {
            "https://images.unsplash.com/photo-1510001618818-4b4e3d86bf0f?w=523&auto=format",
            "https://images.unsplash.com/photo-1507513319174-e556268bb244?w=523&auto=format",
            "https://images.unsplash.com/photo-1474181487882-5abf3f0ba6c2?w=523&auto=format",
            "https://images.unsplash.com/photo-1492778297155-7be4c83960c7?w=523&auto=format",
            "https://images.unsplash.com/photo-1508062878650-88b52897f298?w=523&auto=format",
            "https://images.unsplash.com/photo-1506158278516-d720e72406fc?w=523&auto=format",
            "https://images.unsplash.com/photo-1552203274-e3c7bd771d26?w=523&auto=format",
            "https://images.unsplash.com/photo-1528163186890-de9b86b54b51?w=523&auto=format",
            "https://images.unsplash.com/photo-1727423304224-6d2fd99b864c?w=523&auto=format",
            "https://images.unsplash.com/photo-1675090391405-432434e23595?w=523&auto=format",
            "https://images.unsplash.com/photo-1554196967-97a8602084d9?w=523&auto=format",
            "https://images.unsplash.com/photo-1491961865842-98f7befd1a60?w=523&auto=format",
            "https://images.unsplash.com/photo-1721728613411-d56d2ddda959?w=523&auto=format",
            "https://images.unsplash.com/photo-1731901245099-20ac7f85dbaa?w=523&auto=format",
            "https://images.unsplash.com/photo-1617694455303-59af55af7e58?w=523&auto=format",
            "https://images.unsplash.com/photo-1709198165282-1dab551df890?w=523&auto=format"
        };

        var items = new ObservableCollection<MasonryImageItem>();
        for (var i = 0; i < imageSources.Length; i++)
        {
            items.Add(new MasonryImageItem(i + 1, imageSources[i]));
        }

        ImageItems = items;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new MasonryApiRow("ColumnCount", Lang(MasonryShowCaseLangResourceKind.ApiPropertyColumnCount), "int", "orange", "0"),
            new MasonryApiRow("ColumnInfo", Lang(MasonryShowCaseLangResourceKind.ApiPropertyColumnInfo), "ResponsiveInt?", "purple", "null"),
            new MasonryApiRow("MinColumnWidth", Lang(MasonryShowCaseLangResourceKind.ApiPropertyMinColumnWidth), "double", "cyan", "320"),
            new MasonryApiRow("MaxColumnCount", Lang(MasonryShowCaseLangResourceKind.ApiPropertyMaxColumnCount), "int", "orange", "4"),
            new MasonryApiRow("ColumnGap", Lang(MasonryShowCaseLangResourceKind.ApiPropertyColumnGap), "double", "cyan", "16"),
            new MasonryApiRow("RowGap", Lang(MasonryShowCaseLangResourceKind.ApiPropertyRowGap), "double", "cyan", "16"),
            new MasonryApiRow("Gutter", Lang(MasonryShowCaseLangResourceKind.ApiPropertyGutter), "ResponsiveGutter?", "purple", "null"),
            new MasonryApiRow("ItemsSource", Lang(MasonryShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable", "green", "null"),
            new MasonryApiRow("ItemTemplate", Lang(MasonryShowCaseLangResourceKind.ApiPropertyItemTemplate), "IDataTemplate", "green", "null"),
            new MasonryApiRow("Masonry.Column", Lang(MasonryShowCaseLangResourceKind.ApiPropertyMasonryColumn), "int?", "purple", "null"),
            new MasonryApiRow("Masonry.Span", Lang(MasonryShowCaseLangResourceKind.ApiPropertyMasonrySpan), "MasonryItemSpan", "purple", "Auto"),
            new MasonryApiRow("LayoutChanged", Lang(MasonryShowCaseLangResourceKind.ApiEventLayoutChanged), "event", "geekblue", "-")
        ];
    }

    public void EnsureDesignTokenRows()
    {
        if (DesignTokenRows is not null)
        {
            return;
        }

        DesignTokenRows = [];
    }

    private static string Lang(MasonryShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(MasonryShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            MasonryShowCaseLangResourceKind.ApiPropertyColumnCount     => en_US.ApiPropertyColumnCount,
            MasonryShowCaseLangResourceKind.ApiPropertyColumnInfo      => en_US.ApiPropertyColumnInfo,
            MasonryShowCaseLangResourceKind.ApiPropertyMinColumnWidth  => en_US.ApiPropertyMinColumnWidth,
            MasonryShowCaseLangResourceKind.ApiPropertyMaxColumnCount  => en_US.ApiPropertyMaxColumnCount,
            MasonryShowCaseLangResourceKind.ApiPropertyColumnGap       => en_US.ApiPropertyColumnGap,
            MasonryShowCaseLangResourceKind.ApiPropertyRowGap          => en_US.ApiPropertyRowGap,
            MasonryShowCaseLangResourceKind.ApiPropertyGutter          => en_US.ApiPropertyGutter,
            MasonryShowCaseLangResourceKind.ApiPropertyItemsSource     => en_US.ApiPropertyItemsSource,
            MasonryShowCaseLangResourceKind.ApiPropertyItemTemplate    => en_US.ApiPropertyItemTemplate,
            MasonryShowCaseLangResourceKind.ApiPropertyMasonryColumn   => en_US.ApiPropertyMasonryColumn,
            MasonryShowCaseLangResourceKind.ApiPropertyMasonrySpan     => en_US.ApiPropertyMasonrySpan,
            MasonryShowCaseLangResourceKind.ApiEventLayoutChanged      => en_US.ApiEventLayoutChanged,
            MasonryShowCaseLangResourceKind.TokenNameNoComponentToken  => en_US.TokenNameNoComponentToken,
            MasonryShowCaseLangResourceKind.TokenScopeComponent        => en_US.TokenScopeComponent,
            MasonryShowCaseLangResourceKind.TokenStatusNotApplicable   => en_US.TokenStatusNotApplicable,
            _                                                            => kind.ToString()
        };
    }
}

public sealed record MasonryApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record MasonryDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);

public sealed record MasonryImageItem(int Index, string ImageSource);

/// <summary>
/// Data item for the basic Masonry example. Regular items render as a small Card with the
/// given height and a 1-based number. The special item renders a Card with a cover image.
/// </summary>
public sealed class MasonryBasicItem
{
    public int Index { get; }
    public double Height { get; }
    public bool IsSpecial { get; }
    public string? CoverSource { get; }
    public string? Title { get; }
    public string? Description { get; }

    public MasonryBasicItem(int index, double height, bool isSpecial = false,
        string? coverSource = null, string? title = null, string? description = null)
    {
        Index       = index;
        Height      = height;
        IsSpecial   = isSpecial;
        CoverSource = coverSource;
        Title       = title;
        Description = description;
    }
}
