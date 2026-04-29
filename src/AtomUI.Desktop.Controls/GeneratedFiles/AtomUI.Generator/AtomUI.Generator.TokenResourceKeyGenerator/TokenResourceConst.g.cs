using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls.DesignTokens
{
    public enum AdornerLayerTokenKind
    {
        FocusVisualMargin
    }

    public enum ArrowDecoratedBoxTokenKind
    {
        ArrowSize,
        Padding
    }

    public enum ButtonTokenKind
    {
        BorderColorDisabled,
        CirclePadding,
        ContentFontSize,
        ContentFontSizeLG,
        ContentFontSizeSM,
        ContentLineHeight,
        ContentLineHeightLG,
        ContentLineHeightSM,
        DangerColor,
        DangerShadow,
        DefaultActiveBg,
        DefaultActiveBorderColor,
        DefaultActiveColor,
        DefaultBg,
        DefaultBorderColor,
        DefaultBorderColorDisabled,
        DefaultColor,
        DefaultGhostBorderColor,
        DefaultGhostColor,
        DefaultHoverBg,
        DefaultHoverBorderColor,
        DefaultHoverColor,
        DefaultShadow,
        ExtraContentItemSpacing,
        ExtraContentMargin,
        ExtraContentMarginLG,
        ExtraContentMarginSM,
        FontWeight,
        GhostBg,
        GroupBorderColor,
        GutterToFlyout,
        IconMargin,
        IconOnyPadding,
        IconOnyPaddingLG,
        IconOnyPaddingSM,
        IconSize,
        IconSizeLG,
        IconSizeSM,
        LinkHoverBg,
        OnlyIconSize,
        OnlyIconSizeLG,
        OnlyIconSizeSM,
        Padding,
        PaddingLG,
        PaddingSM,
        PrimaryColor,
        PrimaryShadow,
        SolidTextColor,
        TextHoverBg,
        TextTextActiveColor,
        TextTextColor,
        TextTextHoverColor
    }

    public enum NavMenuTokenKind
    {
        ActiveBarHeight,
        ActiveBarScaleX,
        CollapsedIconSize,
        CollapsedWidth,
        DangerItemActiveBg,
        DangerItemColor,
        DangerItemHoverColor,
        DangerItemSelectedBg,
        DangerItemSelectedColor,
        DarkDangerItemActiveBg,
        DarkDangerItemColor,
        DarkDangerItemHoverColor,
        DarkDangerItemSelectedBg,
        DarkDangerItemSelectedColor,
        DarkGroupTitleColor,
        DarkItemBg,
        DarkItemColor,
        DarkItemDisabledColor,
        DarkItemHoverBg,
        DarkItemHoverColor,
        DarkItemSelectedBg,
        DarkItemSelectedColor,
        DarkMenuBg,
        DarkMenuPopupBg,
        DarkSubMenuItemBg,
        GroupTitleColor,
        GroupTitleFontSize,
        GroupTitleLineHeight,
        HorizontalItemBorderRadius,
        HorizontalItemHoverBg,
        HorizontalItemHoverColor,
        HorizontalItemMargin,
        HorizontalItemSelectedBg,
        HorizontalItemSelectedColor,
        HorizontalLineHeight,
        IconMargin,
        IconSize,
        InlineItemIndentUnit,
        ItemActiveBg,
        ItemBg,
        ItemBorderRadius,
        ItemColor,
        ItemContentMargin,
        ItemContentPadding,
        ItemDisabledColor,
        ItemHeight,
        ItemHoverBg,
        ItemHoverColor,
        ItemIconSize,
        ItemMargin,
        ItemSelectedBg,
        ItemSelectedColor,
        KeyGestureColor,
        MenuArrowSize,
        MenuHorizontalHeight,
        MenuPopupBg,
        MenuPopupContentPadding,
        MenuPopupMaxHeight,
        MenuPopupMaxWidth,
        MenuPopupMinWidth,
        MenuSubMenuBg,
        SubMenuItemBg,
        SubMenuItemBorderRadius,
        TopLevelItemPopupMarginToAnchor,
        VerticalChildItemsMargin,
        VerticalItemsPanelSpacing,
        VerticalMenuContentPadding
    }

    public enum PopupHostTokenKind
    {
        BorderRadius,
        BoxShadows,
        MarginToAnchor
    }

    public enum ScrollViewerTokenKind
    {
        LiteModeThumbThickness,
        NormalModeThumbThickness,
        ScrollBarContentHPadding,
        ScrollBarContentVPadding,
        ThumbActiveBg,
        ThumbBg,
        ThumbCornerRadius,
        ThumbHoverBg
    }

    public enum SpaceTokenKind
    {
        AddonBg,
        AddOnPadding,
        AddOnPaddingLG,
        AddOnPaddingSM,
        GapLargeSize,
        GapMiddleSize,
        GapSmallSize
    }

    public enum ToolTipTokenKind
    {
        BorderRadiusOuter,
        MotionDuration,
        OverlayHostShadows,
        Padding,
        PopupRootHostShadows,
        ToolTipBackground,
        ToolTipColor,
        ToolTipMaxWidth
    }

    public enum WindowTitleBarTokenKind
    {
        ActiveBgColor,
        ActiveColor,
        ActiveHoverBgColor,
        ActivePressedBgColor,
        CaptionButtonIconSize,
        CaptionButtonPadding,
        CaptionGroupSpacing,
        CloseHoverBackgroundColor,
        ClosePressedBackgroundColor,
        ForegroundColor,
        Height,
        HoverBackgroundColor,
        InactiveBgColor,
        InactiveColor,
        InactiveHoverBgColor,
        LogoAndTitleSpacing,
        LogoSize,
        PressedBackgroundColor,
        TitleBarPadding,
        TitleFontSize,
        TitleFontWeight,
        WindowsCloseButtonHoverBgColor,
        WindowsCloseButtonHoverColor,
        WindowsCloseButtonPressedBgColor
    }

    public enum WindowTokenKind
    {
        CornerRadius,
        DefaultBackground,
        DefaultForeground,
        FrameShadows,
        FullscreenHeaderFramePadding,
        FullscreenPopoverShadows,
        SystemBarColor,
        TitleBarHeight
    }

    public class AdornerLayerTokenResourceExtension : TokenResourceExtension<AdornerLayerTokenKind>
    {
        public AdornerLayerTokenResourceExtension()
        {
        }

        public AdornerLayerTokenResourceExtension(AdornerLayerTokenKind kind) : base(kind)
        {
        }
    }

    public class ArrowDecoratedBoxTokenResourceExtension : TokenResourceExtension<ArrowDecoratedBoxTokenKind>
    {
        public ArrowDecoratedBoxTokenResourceExtension()
        {
        }

        public ArrowDecoratedBoxTokenResourceExtension(ArrowDecoratedBoxTokenKind kind) : base(kind)
        {
        }
    }

    public class ButtonTokenResourceExtension : TokenResourceExtension<ButtonTokenKind>
    {
        public ButtonTokenResourceExtension()
        {
        }

        public ButtonTokenResourceExtension(ButtonTokenKind kind) : base(kind)
        {
        }
    }

    public class NavMenuTokenResourceExtension : TokenResourceExtension<NavMenuTokenKind>
    {
        public NavMenuTokenResourceExtension()
        {
        }

        public NavMenuTokenResourceExtension(NavMenuTokenKind kind) : base(kind)
        {
        }
    }

    public class PopupHostTokenResourceExtension : TokenResourceExtension<PopupHostTokenKind>
    {
        public PopupHostTokenResourceExtension()
        {
        }

        public PopupHostTokenResourceExtension(PopupHostTokenKind kind) : base(kind)
        {
        }
    }

    public class ScrollViewerTokenResourceExtension : TokenResourceExtension<ScrollViewerTokenKind>
    {
        public ScrollViewerTokenResourceExtension()
        {
        }

        public ScrollViewerTokenResourceExtension(ScrollViewerTokenKind kind) : base(kind)
        {
        }
    }

    public class SpaceTokenResourceExtension : TokenResourceExtension<SpaceTokenKind>
    {
        public SpaceTokenResourceExtension()
        {
        }

        public SpaceTokenResourceExtension(SpaceTokenKind kind) : base(kind)
        {
        }
    }

    public class ToolTipTokenResourceExtension : TokenResourceExtension<ToolTipTokenKind>
    {
        public ToolTipTokenResourceExtension()
        {
        }

        public ToolTipTokenResourceExtension(ToolTipTokenKind kind) : base(kind)
        {
        }
    }

    public class WindowTitleBarTokenResourceExtension : TokenResourceExtension<WindowTitleBarTokenKind>
    {
        public WindowTitleBarTokenResourceExtension()
        {
        }

        public WindowTitleBarTokenResourceExtension(WindowTitleBarTokenKind kind) : base(kind)
        {
        }
    }

    public class WindowTokenResourceExtension : TokenResourceExtension<WindowTokenKind>
    {
        public WindowTokenResourceExtension()
        {
        }

        public WindowTokenResourceExtension(WindowTokenKind kind) : base(kind)
        {
        }
    }
}