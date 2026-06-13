using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;

namespace AtomUIGallery.Controls.DesignTokens
{
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

    public class GalleryStickyTabsHostTokenResourceExtension : TokenResourceExtension<GalleryStickyTabsHostTokenKind>
    {
        public GalleryStickyTabsHostTokenResourceExtension()
        {
        }

        public GalleryStickyTabsHostTokenResourceExtension(GalleryStickyTabsHostTokenKind kind) : base(kind)
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

    public class ShowCaseItemTokenResourceExtension : TokenResourceExtension<ShowCaseItemTokenKind>
    {
        public ShowCaseItemTokenResourceExtension()
        {
        }

        public ShowCaseItemTokenResourceExtension(ShowCaseItemTokenKind kind) : base(kind)
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
}