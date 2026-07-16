using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;

namespace AtomUI.Toolkits.GalleryBase.Controls.DesignTokens
{
    public enum GalleryShowCaseHeaderTokenKind
    {
        HeaderMargin,
        HeaderSpacing,
        MetadataCornerRadius,
        MetadataItemSpacing,
        MetadataLabelWidth,
        MetadataLineHeight,
        MetadataLineSpacing,
        MetadataMinHeight,
        MetadataPadding,
        MetadataPairSpacing,
        MetadataValueFontFamily,
        MetadataValueWidth,
        SubtitleFontSize,
        SummarySpacing,
        TagItemSpacing,
        TagLineSpacing,
        TagsMargin,
        TitleFontSize,
        TitleFontWeight
    }

    public enum GalleryStickyTabsHostTokenKind
    {
        StickyBackground,
        StickyBorderBrush,
        StickyContentPadding
    }

    public enum GalleryWindowTitleBarTokenKind
    {
        MenuFontWeight,
        MenuMargin
    }

    public enum ShowCaseItemTokenKind
    {
        BadgePreviewMargin,
        CardCornerRadius,
        CardPadding,
        CardShadow,
        DeferredPlaceholderCornerRadius,
        DeferredPlaceholderHeight,
        DescriptionMargin,
        PreviewMargin,
        TitleFontWeight
    }

    public enum ShowCasePanelTokenKind
    {
        ColumnGap,
        ContentMargin,
        MaxColumns,
        MinItemWidth,
        RowGap
    }

    public class GalleryShowCaseHeaderTokenResourceExtension : TokenResourceExtension<GalleryShowCaseHeaderTokenKind>
    {
        public GalleryShowCaseHeaderTokenResourceExtension()
        {
        }

        public GalleryShowCaseHeaderTokenResourceExtension(GalleryShowCaseHeaderTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class GalleryShowCaseHeaderTokenSharedTokenResourceExtension : ControlSharedTokenResourceExtension
    {
        public GalleryShowCaseHeaderTokenSharedTokenResourceExtension(SharedTokenKind kind) : base("AtomUI", "GalleryShowCaseHeader", kind)
        {
        }
    }

    public class GalleryStickyTabsHostTokenResourceExtension : TokenResourceExtension<GalleryStickyTabsHostTokenKind>
    {
        public GalleryStickyTabsHostTokenResourceExtension()
        {
        }

        public GalleryStickyTabsHostTokenResourceExtension(GalleryStickyTabsHostTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class GalleryStickyTabsHostTokenSharedTokenResourceExtension : ControlSharedTokenResourceExtension
    {
        public GalleryStickyTabsHostTokenSharedTokenResourceExtension(SharedTokenKind kind) : base("AtomUI", "GalleryStickyTabsHost", kind)
        {
        }
    }

    public class GalleryWindowTitleBarTokenResourceExtension : TokenResourceExtension<GalleryWindowTitleBarTokenKind>
    {
        public GalleryWindowTitleBarTokenResourceExtension()
        {
        }

        public GalleryWindowTitleBarTokenResourceExtension(GalleryWindowTitleBarTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class GalleryWindowTitleBarTokenSharedTokenResourceExtension : ControlSharedTokenResourceExtension
    {
        public GalleryWindowTitleBarTokenSharedTokenResourceExtension(SharedTokenKind kind) : base("AtomUI", "GalleryWindowTitleBar", kind)
        {
        }
    }

    public class ShowCaseItemTokenResourceExtension : TokenResourceExtension<ShowCaseItemTokenKind>
    {
        public ShowCaseItemTokenResourceExtension()
        {
        }

        public ShowCaseItemTokenResourceExtension(ShowCaseItemTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ShowCaseItemTokenSharedTokenResourceExtension : ControlSharedTokenResourceExtension
    {
        public ShowCaseItemTokenSharedTokenResourceExtension(SharedTokenKind kind) : base("AtomUI", "ShowCaseItem", kind)
        {
        }
    }

    public class ShowCasePanelTokenResourceExtension : TokenResourceExtension<ShowCasePanelTokenKind>
    {
        public ShowCasePanelTokenResourceExtension()
        {
        }

        public ShowCasePanelTokenResourceExtension(ShowCasePanelTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ShowCasePanelTokenSharedTokenResourceExtension : ControlSharedTokenResourceExtension
    {
        public ShowCasePanelTokenSharedTokenResourceExtension(SharedTokenKind kind) : base("AtomUI", "ShowCasePanel", kind)
        {
        }
    }
}