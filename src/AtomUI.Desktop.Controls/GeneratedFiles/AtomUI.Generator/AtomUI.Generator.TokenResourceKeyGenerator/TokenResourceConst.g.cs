using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls.DesignTokens
{
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

    public enum PopupHostTokenKind
    {
        BorderRadius,
        BoxShadows,
        MarginToAnchor
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

    public class PopupHostTokenResourceExtension : TokenResourceExtension<PopupHostTokenKind>
    {
        public PopupHostTokenResourceExtension()
        {
        }

        public PopupHostTokenResourceExtension(PopupHostTokenKind kind) : base(kind)
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