using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Toolkits.GalleryBase.Controls;

public class GalleryShowCaseHeader : TemplatedControl
{
    public const string LanguageId = "GalleryShowCaseHeader";

    #region 公共属性定义

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(Title));

    public static readonly StyledProperty<string?> CategoryProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(Category));

    public static readonly StyledProperty<string?> CategoryTagColorProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(CategoryTagColor), "blue");

    public static readonly StyledProperty<string?> StatusProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(Status));

    public static readonly StyledProperty<string?> StatusTagColorProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(StatusTagColor), "success");

    public static readonly StyledProperty<string?> IntroducedVersionProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(IntroducedVersion));

    public static readonly StyledProperty<string?> IntroducedVersionTagColorProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(IntroducedVersionTagColor), "blue");

    public static readonly StyledProperty<bool> IsIntroducedVersionTagBorderedProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsIntroducedVersionTagBordered), false);

    public static readonly StyledProperty<string?> SubtitleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(Subtitle));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(Description));

    public static readonly StyledProperty<string?> NamespaceProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(Namespace));

    public static readonly StyledProperty<string?> PackageProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(Package));

    public static readonly StyledProperty<string?> BaseClassProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, string?>(nameof(BaseClass));

    public static readonly StyledProperty<double> MetadataLabelWidthProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, double>(nameof(MetadataLabelWidth), double.NaN);

    public static readonly StyledProperty<double> MetadataValueWidthProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, double>(nameof(MetadataValueWidth), double.NaN);

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Category
    {
        get => GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }

    public string? CategoryTagColor
    {
        get => GetValue(CategoryTagColorProperty);
        set => SetValue(CategoryTagColorProperty, value);
    }

    public string? Status
    {
        get => GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public string? StatusTagColor
    {
        get => GetValue(StatusTagColorProperty);
        set => SetValue(StatusTagColorProperty, value);
    }

    public string? IntroducedVersion
    {
        get => GetValue(IntroducedVersionProperty);
        set => SetValue(IntroducedVersionProperty, value);
    }

    public string? IntroducedVersionTagColor
    {
        get => GetValue(IntroducedVersionTagColorProperty);
        set => SetValue(IntroducedVersionTagColorProperty, value);
    }

    public bool IsIntroducedVersionTagBordered
    {
        get => GetValue(IsIntroducedVersionTagBorderedProperty);
        set => SetValue(IsIntroducedVersionTagBorderedProperty, value);
    }

    public string? Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string? Namespace
    {
        get => GetValue(NamespaceProperty);
        set => SetValue(NamespaceProperty, value);
    }

    public string? Package
    {
        get => GetValue(PackageProperty);
        set => SetValue(PackageProperty, value);
    }

    public string? BaseClass
    {
        get => GetValue(BaseClassProperty);
        set => SetValue(BaseClassProperty, value);
    }

    public double MetadataLabelWidth
    {
        get => GetValue(MetadataLabelWidthProperty);
        set => SetValue(MetadataLabelWidthProperty, value);
    }

    public double MetadataValueWidth
    {
        get => GetValue(MetadataValueWidthProperty);
        set => SetValue(MetadataValueWidthProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsCategoryVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsCategoryVisible), false);

    internal static readonly StyledProperty<bool> IsStatusVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsStatusVisible), false);

    internal static readonly StyledProperty<bool> IsIntroducedVersionVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsIntroducedVersionVisible), false);

    internal static readonly StyledProperty<bool> IsTagsVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsTagsVisible), false);

    internal static readonly StyledProperty<bool> IsSubtitleVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsSubtitleVisible), false);

    internal static readonly StyledProperty<bool> IsDescriptionVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsDescriptionVisible), false);

    internal static readonly StyledProperty<bool> IsNamespaceVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsNamespaceVisible), false);

    internal static readonly StyledProperty<bool> IsPackageVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsPackageVisible), false);

    internal static readonly StyledProperty<bool> IsBaseClassVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsBaseClassVisible), false);

    internal static readonly StyledProperty<bool> IsMetadataVisibleProperty =
        AvaloniaProperty.Register<GalleryShowCaseHeader, bool>(nameof(IsMetadataVisible), false);

    internal bool IsCategoryVisible
    {
        get => GetValue(IsCategoryVisibleProperty);
        set => SetValue(IsCategoryVisibleProperty, value);
    }

    internal bool IsStatusVisible
    {
        get => GetValue(IsStatusVisibleProperty);
        set => SetValue(IsStatusVisibleProperty, value);
    }

    internal bool IsIntroducedVersionVisible
    {
        get => GetValue(IsIntroducedVersionVisibleProperty);
        set => SetValue(IsIntroducedVersionVisibleProperty, value);
    }

    internal bool IsTagsVisible
    {
        get => GetValue(IsTagsVisibleProperty);
        set => SetValue(IsTagsVisibleProperty, value);
    }

    internal bool IsSubtitleVisible
    {
        get => GetValue(IsSubtitleVisibleProperty);
        set => SetValue(IsSubtitleVisibleProperty, value);
    }

    internal bool IsDescriptionVisible
    {
        get => GetValue(IsDescriptionVisibleProperty);
        set => SetValue(IsDescriptionVisibleProperty, value);
    }

    internal bool IsNamespaceVisible
    {
        get => GetValue(IsNamespaceVisibleProperty);
        set => SetValue(IsNamespaceVisibleProperty, value);
    }

    internal bool IsPackageVisible
    {
        get => GetValue(IsPackageVisibleProperty);
        set => SetValue(IsPackageVisibleProperty, value);
    }

    internal bool IsBaseClassVisible
    {
        get => GetValue(IsBaseClassVisibleProperty);
        set => SetValue(IsBaseClassVisibleProperty, value);
    }

    internal bool IsMetadataVisible
    {
        get => GetValue(IsMetadataVisibleProperty);
        set => SetValue(IsMetadataVisibleProperty, value);
    }

    #endregion

    static GalleryShowCaseHeader()
    {
        AffectsMeasure<GalleryShowCaseHeader>(
            TitleProperty,
            CategoryProperty,
            StatusProperty,
            IntroducedVersionProperty,
            SubtitleProperty,
            DescriptionProperty,
            NamespaceProperty,
            PackageProperty,
            BaseClassProperty,
            MetadataLabelWidthProperty,
            MetadataValueWidthProperty);
    }

    public GalleryShowCaseHeader()
    {
        this.RegisterTokenResourceScope(GalleryShowCaseHeaderToken.ScopeProvider);
        UpdateVisibilityProperties();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CategoryProperty ||
            change.Property == StatusProperty ||
            change.Property == IntroducedVersionProperty ||
            change.Property == SubtitleProperty ||
            change.Property == DescriptionProperty ||
            change.Property == NamespaceProperty ||
            change.Property == PackageProperty ||
            change.Property == BaseClassProperty)
        {
            UpdateVisibilityProperties();
        }
    }

    private void UpdateVisibilityProperties()
    {
        IsCategoryVisible          = HasText(Category);
        IsStatusVisible            = HasText(Status);
        IsIntroducedVersionVisible = HasText(IntroducedVersion);
        IsTagsVisible              = IsCategoryVisible || IsStatusVisible || IsIntroducedVersionVisible;

        IsSubtitleVisible    = HasText(Subtitle);
        IsDescriptionVisible = HasText(Description);

        IsNamespaceVisible = HasText(Namespace);
        IsPackageVisible   = HasText(Package);
        IsBaseClassVisible = HasText(BaseClass);
        IsMetadataVisible  = IsNamespaceVisible || IsPackageVisible || IsBaseClassVisible;
    }

    private static bool HasText(string? value)
    {
        return !string.IsNullOrWhiteSpace(value);
    }
}
