using AtomUI.Theme.TokenSystem;
using AtomUI.Theme;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;

namespace AtomUI.Desktop.Controls.DesignTokens
{
    public enum AddOnDecoratedBoxTokenKind
    {
        ActiveBg,
        ActiveBorderColor,
        ActiveShadow,
        AddonBg,
        AddOnPadding,
        AddOnPaddingLG,
        AddOnPaddingSM,
        ContentMargin,
        ErrorActiveShadow,
        HoverBg,
        HoverBorderColor,
        LeftInnerAddOnMargin,
        Padding,
        PaddingLG,
        PaddingSM,
        RightInnerAddOnMargin,
        WarningActiveShadow
    }

    public enum AdornerLayerTokenKind
    {
        FocusVisualMargin
    }

    public enum AlertTokenKind
    {
        CloseIconSize,
        DefaultPadding,
        DescriptionLabelMargin,
        ExtraElementMargin,
        IconDefaultMargin,
        IconSize,
        IconWithDescriptionMargin,
        MessageWithDescriptionMargin,
        WithDescriptionIconSize,
        WithDescriptionPadding
    }

    public enum ArrowDecoratedBoxTokenKind
    {
        ArrowSize,
        ArrowStrokeColor,
        ArrowStrokeThickness,
        Padding
    }

    public enum AutoCompleteTokenKind
    {
        MaxPopupWidth,
        MinPopupWidth,
        OptionHeight,
        PopupContentPadding
    }

    public enum AvatarTokenKind
    {
        AvatarBg,
        AvatarColor,
        ContainerSize,
        ContainerSizeLG,
        ContainerSizeSM,
        GroupBorderColor,
        GroupOverlapping,
        GroupSpace,
        TextFontSize,
        TextFontSizeLG,
        TextFontSizeSM
    }

    public enum BadgeTokenKind
    {
        BadgeColor,
        BadgeColorHover,
        BadgeFontHeight,
        BadgeProcessingDuration,
        BadgeRibbonCornerDarkenAmount,
        BadgeRibbonCornerTransform,
        BadgeRibbonOffset,
        BadgeRibbonTextPadding,
        BadgeShadowColor,
        BadgeShadowSize,
        BadgeTextColor,
        CountBadgeCornerRadius,
        CountBadgeCornerRadiusSM,
        CountBadgeTextPadding,
        DotBadgeLabelMargin,
        DotSize,
        IndicatorHeight,
        IndicatorHeightSM,
        StatusSize,
        TextFontSize,
        TextFontSizeSM,
        TextFontWeight
    }

    public enum BorderBeamTokenKind
    {
        BeamOpacity,
        BeamSize,
        MaxVisibleStopPercent,
        MotionDuration
    }

    public enum BreadcrumbTokenKind
    {
        BreadcrumbItemContentPadding,
        IconSize,
        ItemColor,
        LastItemColor,
        LinkColor,
        LinkHoverBgColor,
        LinkHoverColor,
        SeparatorColor,
        SeparatorMargin
    }

    public enum ButtonSpinnerTokenKind
    {
        ControlWidth,
        FilledHandleBg,
        HandleActiveBg,
        HandleBg,
        HandleBorderColor,
        HandleHoverColor,
        HandleIconSize,
        HandleWidth,
        InputFontSize,
        InputFontSizeLG,
        InputFontSizeSM
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
        IconEndMargin,
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

    public enum CalendarTokenKind
    {
        CellActiveWithRangeBg,
        CellBgDisabled,
        CellHeight,
        CellHoverBg,
        CellHoverWithRangeBg,
        CellLineHeight,
        CellMargin,
        CellRangeBorderColor,
        CellWidth,
        DayTitleHeight,
        HeaderMargin,
        ItemPanelMinHeight,
        ItemPanelMinWidth,
        PanelContentPadding,
        RangeCalendarSpacing,
        TextHeight,
        WithoutTimeCellHeight
    }

    public enum CardTokenKind
    {
        ActionsBg,
        ActionsSpacing,
        BodyPadding,
        BodyPaddingLG,
        BodyPaddingSM,
        CardActionsIconSize,
        CardGridItemShadows,
        CardHeadPadding,
        CardPaddingBase,
        CardShadows,
        ExtraColor,
        HeaderBg,
        HeaderFontSize,
        HeaderFontSizeLG,
        HeaderFontSizeSM,
        HeaderHeight,
        HeaderHeightLG,
        HeaderHeightSM,
        HeaderPadding,
        HeaderPaddingLG,
        HeaderPaddingSM,
        TabsMarginBottom
    }

    public enum CarouselTokenKind
    {
        ArrowOffset,
        ArrowSize,
        IndicatorActiveWidth,
        IndicatorGap,
        IndicatorHeight,
        IndicatorWidth,
        PaginationOffset
    }

    public enum CascaderTokenKind
    {
        ControlItemWidth,
        ControlWidth,
        DropdownHeight,
        FilterHighlightColor,
        HeaderHeight,
        ItemHeaderSpacing,
        MenuPadding,
        OptionHoverBg,
        OptionPadding,
        OptionSelectedBg,
        OptionSelectedColor,
        OptionSelectedFontWeight
    }

    public enum CheckBoxTokenKind
    {
        CheckedMarkSize,
        CheckIndicatorSize,
        IndicatorTristateMarkSize,
        TextMargin
    }

    public enum CollapseTokenKind
    {
        CollapseContentPaddingLG,
        CollapseContentPaddingSM,
        CollapseHeaderPaddingLG,
        CollapseHeaderPaddingSM,
        CollapsePanelBorderRadius,
        ContentBg,
        ContentPadding,
        HeaderBg,
        HeaderPadding,
        LeftExpandButtonMargin,
        LeftExpandButtonMarginLG,
        LeftExpandButtonMarginSM,
        RightExpandButtonMargin,
        RightExpandButtonMarginLG,
        RightExpandButtonMarginSM
    }

    public enum ComboBoxTokenKind
    {
        ControlWidth,
        FilledHandleBg,
        HandleActiveBg,
        HandleBg,
        HandleBorderColor,
        HandleHoverColor,
        HandleIconSize,
        HandleWidth,
        InputFontSize,
        InputFontSizeLG,
        InputFontSizeSM,
        ItemBgColor,
        ItemColor,
        ItemDisabledColor,
        ItemHoverBgColor,
        ItemHoverColor,
        ItemMargin,
        ItemPadding,
        ItemSelectedBgColor,
        ItemSelectedColor,
        PopupContentPadding
    }

    public enum DatePickerTokenKind
    {
        ButtonsPanelMargin,
        CellActiveWithRangeBg,
        CellBgDisabled,
        CellHeight,
        CellHoverBg,
        CellHoverWithRangeBg,
        CellMargin,
        CellRangeBorderColor,
        CellWidth,
        DayTitleHeight,
        HeaderMargin,
        HeaderPadding,
        ItemPanelMinHeight,
        ItemPanelMinWidth,
        MonthViewMinWidth,
        PanelContentPadding,
        RangeCalendarSpacing,
        TextHeight,
        WithoutTimeCellHeight,
        YearMonthCellWidth
    }

    public enum DescriptionsTokenKind
    {
        ColonMargin,
        ContentColor,
        ExtraColor,
        HeaderMargin,
        ItemPadding,
        ItemPaddingLG,
        ItemPaddingSM,
        LabelBg,
        LabelColor,
        TitleColor
    }

    public enum DialogTokenKind
    {
        ButtonGroupSpacing,
        CloseBtnSize,
        ContentBg,
        ContentPadding,
        FooterBg,
        FooterMarginTop,
        FooterPadding,
        HeaderBg,
        HeaderColor,
        HeaderFontSize,
        HeaderMarginBottom,
        HeaderPadding,
        LoadingIndicatorMargin,
        LogoSize,
        MinHeight,
        MinWidth
    }

    public enum DrawerTokenKind
    {
        BoxShadowDrawerDown,
        BoxShadowDrawerLeft,
        BoxShadowDrawerRight,
        BoxShadowDrawerUp,
        CloseIconMargin,
        CloseIconPadding,
        ContentPadding,
        FooterPadding,
        HeaderMargin,
        LargeSize,
        MiddleSize,
        PushOffsetPercent,
        SmallSize
    }

    public enum EmptyTokenKind
    {
        DescriptionMargin,
        DescriptionMarginSM,
        EmptyImgHeight,
        EmptyImgHeightMD,
        EmptyImgHeightSM
    }

    public enum ExpanderTokenKind
    {
        ContentBg,
        ContentPadding,
        ContentPaddingLG,
        ContentPaddingSM,
        ExpanderBorderRadius,
        HeaderBg,
        HeaderPadding,
        HeaderPaddingLG,
        HeaderPaddingSM,
        LeftExpandButtonHMargin,
        LeftExpandButtonVMargin,
        RightExpandButtonHMargin,
        RightExpandButtonVMargin
    }

    public enum FloatButtonTokenKind
    {
        CircleBadgeOffset,
        DescriptionLineHeight,
        FloatButtonIconSize,
        FloatButtonSize,
        FloatOffsetX,
        FloatOffsetY,
        PrimaryColor,
        SquareBadgeOffset
    }

    public enum FlyoutHostTokenKind
    {
        HorizontalOffset,
        MarginToAnchor,
        OverlayHostShadow,
        PopupRootShadow,
        VerticalOffset
    }

    public enum FormTokenKind
    {
        FormItemSpacing,
        InlineItemSpacing,
        LabelColonMargin,
        LabelColor,
        LabelFontSize,
        LabelRequiredMarkColor,
        VerticalLabelMargin,
        VerticalLabelPadding
    }

    public enum GroupBoxTokenKind
    {
        ContentPadding,
        HeaderContainerMargin,
        HeaderContentPadding,
        HeaderIconMargin,
        OrientationMarginPercent,
        TextPaddingInline,
        VerticalMarginInline
    }

    public enum ImagePreviewerTokenKind
    {
        CoverImageWidth,
        DialogMinHeight,
        DialogMinWidth,
        FloatToolbarIndicatorPadding,
        FloatToolbarPadding,
        ImagePreviewSwitchSize,
        MaskBgColor,
        NavButtonBgColor,
        NavButtonBgHoverColor,
        PreviewOperationColor,
        PreviewOperationColorDisabled,
        PreviewOperationHoverColor,
        PreviewOperationSize,
        TitleBarBackgroundColor
    }

    public enum LineEditTokenKind
    {
        InputFontSize,
        InputFontSizeLG,
        InputFontSizeSM
    }

    public enum ListBoxTokenKind
    {
        ContentPadding,
        FilterHighlightColor,
        ItemBgColor,
        ItemColor,
        ItemDisabledColor,
        ItemHoverBgColor,
        ItemHoverColor,
        ItemMargin,
        ItemPadding,
        ItemPaddingLG,
        ItemPaddingSM,
        ItemSelectedBgColor,
        ItemSelectedColor,
        SelectedIndicatorMargin
    }

    public enum ListViewTokenKind
    {
        ContentPadding,
        GroupHeaderColor,
        ItemBgColor,
        ItemColor,
        ItemDisabledColor,
        ItemHoverBgColor,
        ItemHoverColor,
        ItemMargin,
        ItemPadding,
        ItemPaddingLG,
        ItemPaddingSM,
        ItemSelectedBgColor,
        ItemSelectedColor,
        PaginationMargin,
        SelectedIndicatorMargin
    }

    public enum MarqueeLabelTokenKind
    {
        CycleSpace,
        DefaultSpeed
    }

    public enum MentionsTokenKind
    {
        MinPopupWidth,
        OptionHeight,
        PopupContentPadding
    }

    public enum MenuTokenKind
    {
        ContextMenuOffsetX,
        ContextMenuOffsetY,
        DangerItemColor,
        DangerItemHoverColor,
        ItemBg,
        ItemBorderRadius,
        ItemColor,
        ItemDisabledColor,
        ItemHeight,
        ItemHoverBg,
        ItemHoverColor,
        ItemIconMarginInlineEnd,
        ItemIconSize,
        ItemMargin,
        ItemPaddingInline,
        KeyGestureColor,
        MenuPopupBgColor,
        MenuPopupContentPadding,
        MenuPopupMaxWidth,
        MenuPopupMinWidth,
        MenuTearOffHeight,
        SeparatorItemHeight,
        TopLevelItemBg,
        TopLevelItemBorderRadius,
        TopLevelItemBorderRadiusLG,
        TopLevelItemBorderRadiusSM,
        TopLevelItemColor,
        TopLevelItemFontSize,
        TopLevelItemFontSizeLG,
        TopLevelItemFontSizeSM,
        TopLevelItemHoverBg,
        TopLevelItemHoverColor,
        TopLevelItemLineHeight,
        TopLevelItemLineHeightLG,
        TopLevelItemLineHeightSM,
        TopLevelItemPadding,
        TopLevelItemPaddingLG,
        TopLevelItemPaddingSM,
        TopLevelItemPopupMarginToAnchor,
        TopLevelItemSelectedBg,
        TopLevelItemSelectedColor
    }

    public enum MessageBoxTokenKind
    {
        MinWidth,
        StyleIconSize
    }

    public enum MessageTokenKind
    {
        CardHeight,
        ContentBg,
        ContentPadding,
        MessageIconMargin,
        MessageIconSize,
        MessageTopMargin
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
        InlineCollapsedWidth,
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

    public enum NotificationTokenKind
    {
        HeaderMargin,
        NotificationBg,
        NotificationBottomMargin,
        NotificationCloseButtonPadding,
        NotificationCloseButtonSize,
        NotificationContentMargin,
        NotificationIconMargin,
        NotificationIconSize,
        NotificationMarginBottom,
        NotificationPadding,
        NotificationProgressBg,
        NotificationProgressHeight,
        NotificationProgressMargin,
        NotificationTopMargin,
        NotificationWidth
    }

    public enum NumericUpDownTokenKind
    {
        ControlWidth,
        FilledHandleBg,
        HandleActiveBg,
        HandleBg,
        HandleBorderColor,
        HandleHoverColor,
        HandleIconSize,
        HandleWidth,
        InputFontSize,
        InputFontSizeLG,
        InputFontSizeSM
    }

    public enum OptionButtonTokenKind
    {
        ButtonBackground,
        ButtonCheckedBackground,
        ButtonCheckedBgDisabled,
        ButtonCheckedColorDisabled,
        ButtonColor,
        ButtonPadding,
        ButtonSolidCheckedActiveBackground,
        ButtonSolidCheckedBackground,
        ButtonSolidCheckedColor,
        ButtonSolidCheckedHoverBackground,
        ContentFontSize,
        ContentFontSizeLG,
        ContentFontSizeSM,
        ContentLineHeight,
        ContentLineHeightLG,
        ContentLineHeightSM,
        Padding,
        PaddingLG,
        PaddingSM
    }

    public enum OtpLineEditTokenKind
    {
        CellGap,
        CellGapLG,
        CellGapSM,
        CellWidth,
        CellWidthLG,
        CellWidthSM,
        SeparatorMarginInline,
        SeparatorMarginInlineLG,
        SeparatorMarginInlineSM
    }

    public enum PaginationTokenKind
    {
        InputOutlineOffset,
        ItemActiveBg,
        ItemActiveBgDisabled,
        ItemActiveColorDisabled,
        ItemBg,
        ItemInputBg,
        ItemLinkBg,
        ItemSize,
        ItemSizeSM,
        PaginationItemPaddingInline,
        PaginationLayoutMiniSpacing,
        PaginationLayoutSpacing,
        PaginationMiniQuickJumperInputWidth,
        PaginationQuickJumperInputWidth
    }

    public enum PopupConfirmTokenKind
    {
        ButtonContainerMargin,
        ButtonSpacing,
        ContentContainerMargin,
        IconMargin,
        PopupMinHeight,
        PopupMinWidth,
        TitleMargin
    }

    public enum PopupHostTokenKind
    {
        BorderRadius,
        MarginToAnchor,
        OverlayHostShadow,
        PopupRootShadow
    }

    public enum ProgressBarTokenKind
    {
        CircleMinimumIconSize,
        CircleMinimumTextFontSize,
        CircleTextColor,
        DefaultColor,
        LineBorderRadius,
        LineExtraInfoMargin,
        LineInfoIconSize,
        LineInfoIconSizeSM,
        LineProgressPadding,
        ProgressActiveMotionDuration,
        ProgressStepMarginInlineEnd,
        ProgressStepMinWidth,
        RemainingColor
    }

    public enum QRCodeTokenKind
    {
        QRCodeMaskBackgroundColor,
        QRCodeTextColor
    }

    public enum RadioButtonTokenKind
    {
        DotColorDisabled,
        DotPadding,
        DotSize,
        RadioBgColor,
        RadioColor,
        RadioSize,
        TextMargin
    }

    public enum RateTokenKind
    {
        StarBg,
        StarColor,
        StarHoverScale,
        StarSize,
        StarSizeLG,
        StarSizeSM
    }

    public enum ResultTokenKind
    {
        ContentMargin,
        ContentPadding,
        ExtraMargin,
        FramePadding,
        HeaderFontSize,
        HeaderMargin,
        IconSize,
        ImageHeight,
        ImageWidth,
        ResultErrorIconColor,
        ResultInfoIconColor,
        ResultSuccessIconColor,
        ResultWarningIconColor,
        StatusImageMargin,
        SubHeaderFontSize
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

    public enum SegmentedTokenKind
    {
        ItemActiveBg,
        ItemColor,
        ItemHoverBg,
        ItemHoverColor,
        ItemMinHeight,
        ItemMinHeightLG,
        ItemMinHeightSM,
        ItemSelectedBg,
        ItemSelectedColor,
        SegmentedItemContentMargin,
        SegmentedItemPadding,
        SegmentedItemPaddingSM,
        TrackBg,
        TrackPadding
    }

    public enum SelectTokenKind
    {
        FixedItemMargin,
        MultiModePadding,
        MultiModePaddingLG,
        MultiModePaddingSM,
        MultipleItemBg,
        MultipleItemColorDisabled,
        MultipleItemHeight,
        MultipleItemHeightLG,
        MultipleItemHeightSM,
        MultipleSelectorBgDisabled,
        OptionActiveBg,
        OptionFontSize,
        OptionHeight,
        OptionPadding,
        OptionSelectedBg,
        OptionSelectedColor,
        OptionSelectedFontWeight,
        Padding,
        PaddingLG,
        PaddingSM,
        PopupContentPadding,
        SelectAffixPadding
    }

    public enum SeparatorTokenKind
    {
        HorizontalMarginBlock,
        HorizontalMarginBlockLG,
        HorizontalMarginBlockSM,
        HorizontalWithTextGutterMargin,
        OrientationMarginPercent,
        TextPaddingInline,
        VerticalMarginInline
    }

    public enum SkeletonTokenKind
    {
        AvatarMarginRight,
        BlockRadius,
        GradientFromColor,
        GradientToColor,
        ImageContainerMaxSize,
        ImageContainerSize,
        ImageSize,
        LoadingBackgroundEnd,
        LoadingBackgroundMiddle,
        LoadingBackgroundStart,
        LoadingMotionDuration,
        ParagraphLineHeight,
        ParagraphLineRoundCornerRadius,
        ParagraphMarginTop,
        TitleHeight
    }

    public enum SliderTokenKind
    {
        MarginPartWithMark,
        MarkBorderColor,
        MarkBorderColorActive,
        MarkBorderColorHover,
        MarkSize,
        RailBg,
        RailHoverBg,
        RailSize,
        SliderPaddingHorizontal,
        SliderPaddingVertical,
        SliderTrackSize,
        ThumbCircleBorderActiveColor,
        ThumbCircleBorderColor,
        ThumbCircleBorderColorDisabled,
        ThumbCircleBorderHoverColor,
        ThumbCircleBorderThickness,
        ThumbCircleBorderThicknessHover,
        ThumbCircleSize,
        ThumbCircleSizeHover,
        ThumbOutlineColor,
        ThumbOutlineThickness,
        ThumbSize,
        TrackBg,
        TrackBgDisabled,
        TrackHoverBg
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

    public enum SpinTokenKind
    {
        DotSize,
        DotSizeLG,
        DotSizeSM,
        IndicatorDuration,
        IndicatorSize,
        IndicatorSizeLG,
        IndicatorSizeSM
    }

    public enum SplitterTokenKind
    {
        HandleIconColor,
        HandleIconHoverColor,
        HandleIconPressedColor,
        HandleIconSize,
        HandleLineColor,
        HandleLineDragColor,
        HandleLineHoverColor,
        HandleLineThickness,
        SplitBarCollapseCrossOffset,
        SplitBarCollapseOffset,
        SplitBarCollapseOffsetNegative,
        SplitBarDraggableSize,
        SplitBarHandleSize,
        SplitBarSize,
        SplitTriggerSize
    }

    public enum SplitViewTokenKind
    {
        CompactPaneThemeLength,
        OpenPaneThemeLength,
        PaneCloseMotionDuration,
        PaneMotionEasing,
        PaneOpenMotionDuration
    }

    public enum StatisticTokenKind
    {
        ContentFontSize,
        TitleFontSize
    }

    public enum StepsTokenKind
    {
        CustomIconFontSize,
        CustomIconSize,
        DescriptionMaxWidth,
        DotCurrentSize,
        DotLineThickness,
        DotSize,
        ErrorDescriptionColor,
        ErrorDotColor,
        ErrorIconBgColor,
        ErrorIconBorderColor,
        ErrorIconColor,
        ErrorTailColor,
        ErrorTitleColor,
        FinishDescriptionColor,
        FinishDotColor,
        FinishIconBgColor,
        FinishIconBorderColor,
        FinishIconColor,
        FinishTailColor,
        FinishTitleColor,
        HorizontalDotMargin,
        HorizontalHeaderMargin,
        IconFontSize,
        IconSize,
        IconSizeSM,
        InlineDotSize,
        InlineHeaderMargin,
        InlineHeaderPadding,
        InlineItemPadding,
        InlineTailColor,
        InlineTitleColor,
        NavArrowColor,
        NavItemGutter,
        NavItemGutterSM,
        ProcessDescriptionColor,
        ProcessDotColor,
        ProcessIconBgColor,
        ProcessIconBorderColor,
        ProcessIconColor,
        ProcessTailColor,
        ProcessTitleColor,
        ProgressColor,
        ProgressFramePadding,
        ProgressFramePaddingSM,
        ProgressGrooveColor,
        StepsNavActiveColor,
        StepsProgressSize,
        VerticalDescriptionPadding,
        VerticalDotMargin,
        VerticalItemSpacing,
        VerticalLabelContentMargin,
        VerticalNavArrowMargin,
        VerticalNavArrowMarginSM,
        WaitDescriptionColor,
        WaitDotColor,
        WaitIconBgColor,
        WaitIconBorderColor,
        WaitIconColor,
        WaitTailColor,
        WaitTitleColor
    }

    public enum TabControlTokenKind
    {
        AddTabButtonMarginHorizontal,
        AddTabButtonMarginVertical,
        CardBg,
        CardGutter,
        CardPadding,
        CardPaddingLG,
        CardPaddingSM,
        CardSize,
        CloseIconMargin,
        HorizontalItemGutter,
        HorizontalItemMargin,
        HorizontalItemPadding,
        HorizontalItemPaddingLG,
        HorizontalItemPaddingSM,
        HorizontalMargin,
        InkBarColor,
        ItemColor,
        ItemHoverColor,
        ItemIconMargin,
        ItemSelectedColor,
        MenuEdgeThickness,
        MenuIndicatorPaddingHorizontal,
        MenuIndicatorPaddingVertical,
        TabAndContentGutter,
        TitleFontSize,
        TitleFontSizeLG,
        TitleFontSizeSM,
        VerticalItemGutter,
        VerticalItemPadding
    }

    public enum TagTokenKind
    {
        DefaultBg,
        DefaultColor,
        TagBorderlessBg,
        TagCloseIconSize,
        TagFontSize,
        TagIconSize,
        TagLineHeight,
        TagPadding,
        TagTextPaddingInline
    }

    public enum TextAreaTokenKind
    {
        FontSize,
        FontSizeLG,
        FontSizeSM,
        ResizeHandleSize,
        ResizeIndicatorLineColor,
        RightAddOnPadding,
        RightAddOnPaddingLG,
        RightAddOnPaddingSM
    }

    public enum TextBoxTokenKind
    {
        ActiveBorderColor,
        ActiveShadow,
        BorderColor,
        BorderRadius,
        BorderRadiusLG,
        BorderRadiusSM,
        BorderThickness,
        HoverBorderColor,
        Padding,
        PaddingLG,
        PaddingSM
    }

    public enum TimelineTokenKind
    {
        IndicatorDotBorderWidth,
        IndicatorDotSize,
        IndicatorLeftModeMargin,
        IndicatorMiddleModeMargin,
        IndicatorRightModeMargin,
        IndicatorSize,
        IndicatorTailColor,
        IndicatorTailWidth,
        ItemPaddingBottom,
        ItemPaddingBottomLG,
        LastItemContentMinHeight
    }

    public enum TimePickerTokenKind
    {
        ButtonsMargin,
        HeaderMargin,
        ItemHeight,
        ItemPadding,
        ItemWidth,
        PeriodHostWidth,
        RangePickerArrowMargin,
        RangePickerIndicatorThickness
    }

    public enum ToggleSwitchTokenKind
    {
        ExtraInfoFontSize,
        ExtraInfoFontSizeSM,
        HandleBg,
        HandleShadow,
        HandleSize,
        HandleSizeSM,
        IconSize,
        IconSizeSM,
        InnerMaxMargin,
        InnerMaxMarginSM,
        InnerMinMargin,
        InnerMinMarginSM,
        LoadingAnimationDuration,
        OffStateLoadIndicatorColor,
        SwitchColor,
        SwitchDisabledOpacity,
        TrackHeight,
        TrackHeightSM,
        TrackMinWidth,
        TrackMinWidthSM,
        TrackPadding
    }

    public enum ToolTipTokenKind
    {
        BorderRadiusOuter,
        MotionDuration,
        Padding,
        ToolTipBackground,
        ToolTipColor,
        ToolTipMaxWidth
    }

    public enum TourTokenKind
    {
        CloseBtnSize,
        HeaderColor,
        IndicatorSize,
        PopupMarginToAnchor,
        PrimaryNextBtnHoverBg,
        PrimaryPrevBtnBg,
        TourBorderRadius,
        TourViewMinHeight,
        TourViewMinWidth
    }

    public enum TransferTokenKind
    {
        DataGridSelectionHeaderMargin,
        HeaderHeight,
        HeaderPadding,
        ItemHeight,
        ItemPadding,
        ListHeight,
        ListWidth,
        ListWidthLG,
        PaginationMargin
    }

    public enum TreeFlyoutTokenKind
    {
        PopupBgColor,
        PopupContentPadding,
        PopupMaxHeight,
        PopupMaxWidth,
        PopupMinHeight,
        PopupMinWidth
    }

    public enum TreeSelectTokenKind
    {
        MinPopupWidth
    }

    public enum TreeViewTokenKind
    {
        DirectoryNodeSelectedBg,
        DirectoryNodeSelectedColor,
        DragIndicatorLineWidth,
        FilterHighlightColor,
        HeaderHeight,
        NodeHoverBg,
        NodeSelectedBg,
        TreeItemHeaderMargin,
        TreeItemHeaderPadding,
        TreeItemMargin,
        TreeNodeIconMargin,
        TreeNodeSwitcherMargin
    }

    public enum UploadTokenKind
    {
        ActionsColor,
        DragHeaderMargin,
        DragIconMargin,
        DragIconSize,
        PictureCardSize,
        PictureListItemMargin,
        PictureListPreviewerSize,
        TextListItemMargin,
        TextListNamePadding,
        TextListProgressPadding,
        UploadThumbnailSize
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
        FullscreenCaptionButtonSize,
        HeaderHorizontalSpacing,
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
        WindowsCaptionIconSize,
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

    public class AddOnDecoratedBoxTokenResourceExtension : TokenResourceExtension<AddOnDecoratedBoxTokenKind>
    {
        public AddOnDecoratedBoxTokenResourceExtension()
        {
        }

        public AddOnDecoratedBoxTokenResourceExtension(AddOnDecoratedBoxTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class AddOnDecoratedBoxTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public AddOnDecoratedBoxTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "AddOnDecoratedBox", kind)
        {
        }
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

    public sealed class AdornerLayerTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public AdornerLayerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "AdornerLayer", kind)
        {
        }
    }

    public class AlertTokenResourceExtension : TokenResourceExtension<AlertTokenKind>
    {
        public AlertTokenResourceExtension()
        {
        }

        public AlertTokenResourceExtension(AlertTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class AlertTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public AlertTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Alert", kind)
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

    public sealed class ArrowDecoratedBoxTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ArrowDecoratedBoxTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ArrowDecoratedBox", kind)
        {
        }
    }

    public class AutoCompleteTokenResourceExtension : TokenResourceExtension<AutoCompleteTokenKind>
    {
        public AutoCompleteTokenResourceExtension()
        {
        }

        public AutoCompleteTokenResourceExtension(AutoCompleteTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class AutoCompleteTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public AutoCompleteTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "AutoComplete", kind)
        {
        }
    }

    public class AvatarTokenResourceExtension : TokenResourceExtension<AvatarTokenKind>
    {
        public AvatarTokenResourceExtension()
        {
        }

        public AvatarTokenResourceExtension(AvatarTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class AvatarTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public AvatarTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Avatar", kind)
        {
        }
    }

    public class BadgeTokenResourceExtension : TokenResourceExtension<BadgeTokenKind>
    {
        public BadgeTokenResourceExtension()
        {
        }

        public BadgeTokenResourceExtension(BadgeTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class BadgeTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public BadgeTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Badge", kind)
        {
        }
    }

    public class BorderBeamTokenResourceExtension : TokenResourceExtension<BorderBeamTokenKind>
    {
        public BorderBeamTokenResourceExtension()
        {
        }

        public BorderBeamTokenResourceExtension(BorderBeamTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class BorderBeamTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public BorderBeamTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "BorderBeam", kind)
        {
        }
    }

    public class BreadcrumbTokenResourceExtension : TokenResourceExtension<BreadcrumbTokenKind>
    {
        public BreadcrumbTokenResourceExtension()
        {
        }

        public BreadcrumbTokenResourceExtension(BreadcrumbTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class BreadcrumbTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public BreadcrumbTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Breadcrumb", kind)
        {
        }
    }

    public class ButtonSpinnerTokenResourceExtension : TokenResourceExtension<ButtonSpinnerTokenKind>
    {
        public ButtonSpinnerTokenResourceExtension()
        {
        }

        public ButtonSpinnerTokenResourceExtension(ButtonSpinnerTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ButtonSpinnerTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ButtonSpinnerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ButtonSpinner", kind)
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

    public sealed class ButtonTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ButtonTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Button", kind)
        {
        }
    }

    public class CalendarTokenResourceExtension : TokenResourceExtension<CalendarTokenKind>
    {
        public CalendarTokenResourceExtension()
        {
        }

        public CalendarTokenResourceExtension(CalendarTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class CalendarTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public CalendarTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Calendar", kind)
        {
        }
    }

    public class CardTokenResourceExtension : TokenResourceExtension<CardTokenKind>
    {
        public CardTokenResourceExtension()
        {
        }

        public CardTokenResourceExtension(CardTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class CardTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public CardTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Card", kind)
        {
        }
    }

    public class CarouselTokenResourceExtension : TokenResourceExtension<CarouselTokenKind>
    {
        public CarouselTokenResourceExtension()
        {
        }

        public CarouselTokenResourceExtension(CarouselTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class CarouselTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public CarouselTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Carousel", kind)
        {
        }
    }

    public class CascaderTokenResourceExtension : TokenResourceExtension<CascaderTokenKind>
    {
        public CascaderTokenResourceExtension()
        {
        }

        public CascaderTokenResourceExtension(CascaderTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class CascaderTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public CascaderTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Cascader", kind)
        {
        }
    }

    public class CheckBoxTokenResourceExtension : TokenResourceExtension<CheckBoxTokenKind>
    {
        public CheckBoxTokenResourceExtension()
        {
        }

        public CheckBoxTokenResourceExtension(CheckBoxTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class CheckBoxTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public CheckBoxTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "CheckBox", kind)
        {
        }
    }

    public class CollapseTokenResourceExtension : TokenResourceExtension<CollapseTokenKind>
    {
        public CollapseTokenResourceExtension()
        {
        }

        public CollapseTokenResourceExtension(CollapseTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class CollapseTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public CollapseTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Collapse", kind)
        {
        }
    }

    public class ComboBoxTokenResourceExtension : TokenResourceExtension<ComboBoxTokenKind>
    {
        public ComboBoxTokenResourceExtension()
        {
        }

        public ComboBoxTokenResourceExtension(ComboBoxTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ComboBoxTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ComboBoxTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ComboBox", kind)
        {
        }
    }

    public class DatePickerTokenResourceExtension : TokenResourceExtension<DatePickerTokenKind>
    {
        public DatePickerTokenResourceExtension()
        {
        }

        public DatePickerTokenResourceExtension(DatePickerTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class DatePickerTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public DatePickerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "DatePicker", kind)
        {
        }
    }

    public class DescriptionsTokenResourceExtension : TokenResourceExtension<DescriptionsTokenKind>
    {
        public DescriptionsTokenResourceExtension()
        {
        }

        public DescriptionsTokenResourceExtension(DescriptionsTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class DescriptionsTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public DescriptionsTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Descriptions", kind)
        {
        }
    }

    public class DialogTokenResourceExtension : TokenResourceExtension<DialogTokenKind>
    {
        public DialogTokenResourceExtension()
        {
        }

        public DialogTokenResourceExtension(DialogTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class DialogTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public DialogTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Dialog", kind)
        {
        }
    }

    public class DrawerTokenResourceExtension : TokenResourceExtension<DrawerTokenKind>
    {
        public DrawerTokenResourceExtension()
        {
        }

        public DrawerTokenResourceExtension(DrawerTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class DrawerTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public DrawerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Drawer", kind)
        {
        }
    }

    public class EmptyTokenResourceExtension : TokenResourceExtension<EmptyTokenKind>
    {
        public EmptyTokenResourceExtension()
        {
        }

        public EmptyTokenResourceExtension(EmptyTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class EmptyTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public EmptyTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Empty", kind)
        {
        }
    }

    public class ExpanderTokenResourceExtension : TokenResourceExtension<ExpanderTokenKind>
    {
        public ExpanderTokenResourceExtension()
        {
        }

        public ExpanderTokenResourceExtension(ExpanderTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ExpanderTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ExpanderTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Expander", kind)
        {
        }
    }

    public class FloatButtonTokenResourceExtension : TokenResourceExtension<FloatButtonTokenKind>
    {
        public FloatButtonTokenResourceExtension()
        {
        }

        public FloatButtonTokenResourceExtension(FloatButtonTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class FloatButtonTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public FloatButtonTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "FloatButton", kind)
        {
        }
    }

    public class FlyoutHostTokenResourceExtension : TokenResourceExtension<FlyoutHostTokenKind>
    {
        public FlyoutHostTokenResourceExtension()
        {
        }

        public FlyoutHostTokenResourceExtension(FlyoutHostTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class FlyoutHostTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public FlyoutHostTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "FlyoutHost", kind)
        {
        }
    }

    public class FormTokenResourceExtension : TokenResourceExtension<FormTokenKind>
    {
        public FormTokenResourceExtension()
        {
        }

        public FormTokenResourceExtension(FormTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class FormTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public FormTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Form", kind)
        {
        }
    }

    public class GroupBoxTokenResourceExtension : TokenResourceExtension<GroupBoxTokenKind>
    {
        public GroupBoxTokenResourceExtension()
        {
        }

        public GroupBoxTokenResourceExtension(GroupBoxTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class GroupBoxTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public GroupBoxTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "GroupBox", kind)
        {
        }
    }

    public class ImagePreviewerTokenResourceExtension : TokenResourceExtension<ImagePreviewerTokenKind>
    {
        public ImagePreviewerTokenResourceExtension()
        {
        }

        public ImagePreviewerTokenResourceExtension(ImagePreviewerTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ImagePreviewerTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ImagePreviewerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ImagePreviewer", kind)
        {
        }
    }

    public class LineEditTokenResourceExtension : TokenResourceExtension<LineEditTokenKind>
    {
        public LineEditTokenResourceExtension()
        {
        }

        public LineEditTokenResourceExtension(LineEditTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class LineEditTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public LineEditTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "LineEdit", kind)
        {
        }
    }

    public class ListBoxTokenResourceExtension : TokenResourceExtension<ListBoxTokenKind>
    {
        public ListBoxTokenResourceExtension()
        {
        }

        public ListBoxTokenResourceExtension(ListBoxTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ListBoxTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ListBoxTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ListBox", kind)
        {
        }
    }

    public class ListViewTokenResourceExtension : TokenResourceExtension<ListViewTokenKind>
    {
        public ListViewTokenResourceExtension()
        {
        }

        public ListViewTokenResourceExtension(ListViewTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ListViewTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ListViewTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ListView", kind)
        {
        }
    }

    public class MarqueeLabelTokenResourceExtension : TokenResourceExtension<MarqueeLabelTokenKind>
    {
        public MarqueeLabelTokenResourceExtension()
        {
        }

        public MarqueeLabelTokenResourceExtension(MarqueeLabelTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class MarqueeLabelTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public MarqueeLabelTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "MarqueeLabel", kind)
        {
        }
    }

    public class MentionsTokenResourceExtension : TokenResourceExtension<MentionsTokenKind>
    {
        public MentionsTokenResourceExtension()
        {
        }

        public MentionsTokenResourceExtension(MentionsTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class MentionsTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public MentionsTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Mentions", kind)
        {
        }
    }

    public class MenuTokenResourceExtension : TokenResourceExtension<MenuTokenKind>
    {
        public MenuTokenResourceExtension()
        {
        }

        public MenuTokenResourceExtension(MenuTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class MenuTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public MenuTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Menu", kind)
        {
        }
    }

    public class MessageBoxTokenResourceExtension : TokenResourceExtension<MessageBoxTokenKind>
    {
        public MessageBoxTokenResourceExtension()
        {
        }

        public MessageBoxTokenResourceExtension(MessageBoxTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class MessageBoxTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public MessageBoxTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "MessageBox", kind)
        {
        }
    }

    public class MessageTokenResourceExtension : TokenResourceExtension<MessageTokenKind>
    {
        public MessageTokenResourceExtension()
        {
        }

        public MessageTokenResourceExtension(MessageTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class MessageTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public MessageTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Message", kind)
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

    public sealed class NavMenuTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public NavMenuTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "NavMenu", kind)
        {
        }
    }

    public class NotificationTokenResourceExtension : TokenResourceExtension<NotificationTokenKind>
    {
        public NotificationTokenResourceExtension()
        {
        }

        public NotificationTokenResourceExtension(NotificationTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class NotificationTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public NotificationTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Notification", kind)
        {
        }
    }

    public class NumericUpDownTokenResourceExtension : TokenResourceExtension<NumericUpDownTokenKind>
    {
        public NumericUpDownTokenResourceExtension()
        {
        }

        public NumericUpDownTokenResourceExtension(NumericUpDownTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class NumericUpDownTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public NumericUpDownTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "NumericUpDown", kind)
        {
        }
    }

    public class OptionButtonTokenResourceExtension : TokenResourceExtension<OptionButtonTokenKind>
    {
        public OptionButtonTokenResourceExtension()
        {
        }

        public OptionButtonTokenResourceExtension(OptionButtonTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class OptionButtonTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public OptionButtonTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "OptionButton", kind)
        {
        }
    }

    public class OtpLineEditTokenResourceExtension : TokenResourceExtension<OtpLineEditTokenKind>
    {
        public OtpLineEditTokenResourceExtension()
        {
        }

        public OtpLineEditTokenResourceExtension(OtpLineEditTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class OtpLineEditTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public OtpLineEditTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "OtpLineEdit", kind)
        {
        }
    }

    public class PaginationTokenResourceExtension : TokenResourceExtension<PaginationTokenKind>
    {
        public PaginationTokenResourceExtension()
        {
        }

        public PaginationTokenResourceExtension(PaginationTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class PaginationTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public PaginationTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Pagination", kind)
        {
        }
    }

    public class PopupConfirmTokenResourceExtension : TokenResourceExtension<PopupConfirmTokenKind>
    {
        public PopupConfirmTokenResourceExtension()
        {
        }

        public PopupConfirmTokenResourceExtension(PopupConfirmTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class PopupConfirmTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public PopupConfirmTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "PopupConfirm", kind)
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

    public sealed class PopupHostTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public PopupHostTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "PopupHost", kind)
        {
        }
    }

    public class ProgressBarTokenResourceExtension : TokenResourceExtension<ProgressBarTokenKind>
    {
        public ProgressBarTokenResourceExtension()
        {
        }

        public ProgressBarTokenResourceExtension(ProgressBarTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ProgressBarTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ProgressBarTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ProgressBar", kind)
        {
        }
    }

    public class QRCodeTokenResourceExtension : TokenResourceExtension<QRCodeTokenKind>
    {
        public QRCodeTokenResourceExtension()
        {
        }

        public QRCodeTokenResourceExtension(QRCodeTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class QRCodeTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public QRCodeTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "QRCode", kind)
        {
        }
    }

    public class RadioButtonTokenResourceExtension : TokenResourceExtension<RadioButtonTokenKind>
    {
        public RadioButtonTokenResourceExtension()
        {
        }

        public RadioButtonTokenResourceExtension(RadioButtonTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class RadioButtonTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public RadioButtonTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "RadioButton", kind)
        {
        }
    }

    public class RateTokenResourceExtension : TokenResourceExtension<RateTokenKind>
    {
        public RateTokenResourceExtension()
        {
        }

        public RateTokenResourceExtension(RateTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class RateTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public RateTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Rate", kind)
        {
        }
    }

    public class ResultTokenResourceExtension : TokenResourceExtension<ResultTokenKind>
    {
        public ResultTokenResourceExtension()
        {
        }

        public ResultTokenResourceExtension(ResultTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ResultTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ResultTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Result", kind)
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

    public sealed class ScrollViewerTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ScrollViewerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ScrollViewer", kind)
        {
        }
    }

    public class SegmentedTokenResourceExtension : TokenResourceExtension<SegmentedTokenKind>
    {
        public SegmentedTokenResourceExtension()
        {
        }

        public SegmentedTokenResourceExtension(SegmentedTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class SegmentedTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SegmentedTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Segmented", kind)
        {
        }
    }

    public class SelectTokenResourceExtension : TokenResourceExtension<SelectTokenKind>
    {
        public SelectTokenResourceExtension()
        {
        }

        public SelectTokenResourceExtension(SelectTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class SelectTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SelectTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Select", kind)
        {
        }
    }

    public class SeparatorTokenResourceExtension : TokenResourceExtension<SeparatorTokenKind>
    {
        public SeparatorTokenResourceExtension()
        {
        }

        public SeparatorTokenResourceExtension(SeparatorTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class SeparatorTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SeparatorTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Separator", kind)
        {
        }
    }

    public class SkeletonTokenResourceExtension : TokenResourceExtension<SkeletonTokenKind>
    {
        public SkeletonTokenResourceExtension()
        {
        }

        public SkeletonTokenResourceExtension(SkeletonTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class SkeletonTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SkeletonTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Skeleton", kind)
        {
        }
    }

    public class SliderTokenResourceExtension : TokenResourceExtension<SliderTokenKind>
    {
        public SliderTokenResourceExtension()
        {
        }

        public SliderTokenResourceExtension(SliderTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class SliderTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SliderTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Slider", kind)
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

    public sealed class SpaceTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SpaceTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Space", kind)
        {
        }
    }

    public class SpinTokenResourceExtension : TokenResourceExtension<SpinTokenKind>
    {
        public SpinTokenResourceExtension()
        {
        }

        public SpinTokenResourceExtension(SpinTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class SpinTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SpinTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Spin", kind)
        {
        }
    }

    public class SplitterTokenResourceExtension : TokenResourceExtension<SplitterTokenKind>
    {
        public SplitterTokenResourceExtension()
        {
        }

        public SplitterTokenResourceExtension(SplitterTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class SplitterTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SplitterTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Splitter", kind)
        {
        }
    }

    public class SplitViewTokenResourceExtension : TokenResourceExtension<SplitViewTokenKind>
    {
        public SplitViewTokenResourceExtension()
        {
        }

        public SplitViewTokenResourceExtension(SplitViewTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class SplitViewTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public SplitViewTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "SplitView", kind)
        {
        }
    }

    public class StatisticTokenResourceExtension : TokenResourceExtension<StatisticTokenKind>
    {
        public StatisticTokenResourceExtension()
        {
        }

        public StatisticTokenResourceExtension(StatisticTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class StatisticTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public StatisticTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Statistic", kind)
        {
        }
    }

    public class StepsTokenResourceExtension : TokenResourceExtension<StepsTokenKind>
    {
        public StepsTokenResourceExtension()
        {
        }

        public StepsTokenResourceExtension(StepsTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class StepsTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public StepsTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Steps", kind)
        {
        }
    }

    public class TabControlTokenResourceExtension : TokenResourceExtension<TabControlTokenKind>
    {
        public TabControlTokenResourceExtension()
        {
        }

        public TabControlTokenResourceExtension(TabControlTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TabControlTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TabControlTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "TabControl", kind)
        {
        }
    }

    public class TagTokenResourceExtension : TokenResourceExtension<TagTokenKind>
    {
        public TagTokenResourceExtension()
        {
        }

        public TagTokenResourceExtension(TagTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TagTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TagTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Tag", kind)
        {
        }
    }

    public class TextAreaTokenResourceExtension : TokenResourceExtension<TextAreaTokenKind>
    {
        public TextAreaTokenResourceExtension()
        {
        }

        public TextAreaTokenResourceExtension(TextAreaTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TextAreaTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TextAreaTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "TextArea", kind)
        {
        }
    }

    public class TextBoxTokenResourceExtension : TokenResourceExtension<TextBoxTokenKind>
    {
        public TextBoxTokenResourceExtension()
        {
        }

        public TextBoxTokenResourceExtension(TextBoxTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TextBoxTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TextBoxTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "TextBox", kind)
        {
        }
    }

    public class TimelineTokenResourceExtension : TokenResourceExtension<TimelineTokenKind>
    {
        public TimelineTokenResourceExtension()
        {
        }

        public TimelineTokenResourceExtension(TimelineTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TimelineTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TimelineTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Timeline", kind)
        {
        }
    }

    public class TimePickerTokenResourceExtension : TokenResourceExtension<TimePickerTokenKind>
    {
        public TimePickerTokenResourceExtension()
        {
        }

        public TimePickerTokenResourceExtension(TimePickerTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TimePickerTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TimePickerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "TimePicker", kind)
        {
        }
    }

    public class ToggleSwitchTokenResourceExtension : TokenResourceExtension<ToggleSwitchTokenKind>
    {
        public ToggleSwitchTokenResourceExtension()
        {
        }

        public ToggleSwitchTokenResourceExtension(ToggleSwitchTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class ToggleSwitchTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ToggleSwitchTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ToggleSwitch", kind)
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

    public sealed class ToolTipTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public ToolTipTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "ToolTip", kind)
        {
        }
    }

    public class TourTokenResourceExtension : TokenResourceExtension<TourTokenKind>
    {
        public TourTokenResourceExtension()
        {
        }

        public TourTokenResourceExtension(TourTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TourTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TourTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Tour", kind)
        {
        }
    }

    public class TransferTokenResourceExtension : TokenResourceExtension<TransferTokenKind>
    {
        public TransferTokenResourceExtension()
        {
        }

        public TransferTokenResourceExtension(TransferTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TransferTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TransferTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Transfer", kind)
        {
        }
    }

    public class TreeFlyoutTokenResourceExtension : TokenResourceExtension<TreeFlyoutTokenKind>
    {
        public TreeFlyoutTokenResourceExtension()
        {
        }

        public TreeFlyoutTokenResourceExtension(TreeFlyoutTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TreeFlyoutTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TreeFlyoutTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "TreeFlyout", kind)
        {
        }
    }

    public class TreeSelectTokenResourceExtension : TokenResourceExtension<TreeSelectTokenKind>
    {
        public TreeSelectTokenResourceExtension()
        {
        }

        public TreeSelectTokenResourceExtension(TreeSelectTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TreeSelectTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TreeSelectTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "TreeSelect", kind)
        {
        }
    }

    public class TreeViewTokenResourceExtension : TokenResourceExtension<TreeViewTokenKind>
    {
        public TreeViewTokenResourceExtension()
        {
        }

        public TreeViewTokenResourceExtension(TreeViewTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class TreeViewTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public TreeViewTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "TreeView", kind)
        {
        }
    }

    public class UploadTokenResourceExtension : TokenResourceExtension<UploadTokenKind>
    {
        public UploadTokenResourceExtension()
        {
        }

        public UploadTokenResourceExtension(UploadTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class UploadTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public UploadTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Upload", kind)
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

    public sealed class WindowTitleBarTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public WindowTitleBarTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "WindowTitleBar", kind)
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

    public sealed class WindowTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public WindowTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "Window", kind)
        {
        }
    }
}

namespace AtomUI.Desktop.Controls.Primitives.DesignTokens
{
    public enum IndicatorScrollViewerTokenKind
    {
        ScrollBarThickness,
        ThumbBg,
        ThumbCornerRadius,
        ThumbThickness
    }

    public enum InfoPickerInputTokenKind
    {
        RangeMarginToAnchor,
        RangePickerArrowMargin,
        RangePickerIndicatorThickness
    }

    public class IndicatorScrollViewerTokenResourceExtension : TokenResourceExtension<IndicatorScrollViewerTokenKind>
    {
        public IndicatorScrollViewerTokenResourceExtension()
        {
        }

        public IndicatorScrollViewerTokenResourceExtension(IndicatorScrollViewerTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class IndicatorScrollViewerTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public IndicatorScrollViewerTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "IndicatorScrollViewer", kind)
        {
        }
    }

    public class InfoPickerInputTokenResourceExtension : TokenResourceExtension<InfoPickerInputTokenKind>
    {
        public InfoPickerInputTokenResourceExtension()
        {
        }

        public InfoPickerInputTokenResourceExtension(InfoPickerInputTokenKind kind) : base(kind)
        {
        }
    }

    public sealed class InfoPickerInputTokenSharedTokenResourceExtension : ComponentSharedTokenResourceExtension
    {
        public InfoPickerInputTokenSharedTokenResourceExtension(SharedTokenKind kind) : base(null, "InfoPickerInput", kind)
        {
        }
    }
}