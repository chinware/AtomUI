using AtomUI.Media;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class DataGridToken : AbstractControlDesignToken
{
    public const string ID = "DataGrid";
    
    public DataGridToken()
        : base(ID)
    {
    }
    
    /// <summary>
    /// 表头背景
    /// Background of table header
    /// </summary>
    public Color HeaderBg { get; set; }
    
    /// <summary>
    /// 表头文字颜色
    /// Color of table header text
    /// </summary>
    public Color HeaderColor { get; set; }
    
    /// <summary>
    /// 表头排序激活态背景色
    /// Background color of table header when sorted
    /// </summary>
    public Color HeaderSortActiveBg { get; set; }
    
    /// <summary>
    /// 表头排序激活态悬浮背景色
    /// Background color of table header when sorted and hovered
    /// </summary>
    public Color HeaderSortHoverBg { get; set; }
    
    /// <summary>
    /// 表格排序列背景色
    /// Background color of table sorted column
    /// </summary>
    public Color BodySortBg { get; set; }
    
    /// <summary>
    /// 表格行悬浮背景色
    /// Background color of table hovered row
    /// </summary>
    public Color RowHoverBg { get; set; }
    
    /// <summary>
    /// 表格行选中背景色
    /// Background color of table selected row
    /// </summary>
    public Color RowSelectedBg { get; set; }
    
    /// <summary>
    /// 表格行选中悬浮背景色
    /// Background color of table selected row when hovered
    /// </summary>
    public Color RowSelectedHoverBg { get; set; }
    
    /// <summary>
    /// 表格行展开背景色
    /// Background color of table expanded row
    /// </summary>
    public Color RowExpandedBg { get; set; }
    
    /// <summary>
    /// 单元格内间距（默认大尺寸）
    /// Padding of table cell (large size by default)
    /// </summary>
    public Thickness CellPadding { get; set; }
    
    /// <summary>
    /// 单元格内间距（中等尺寸）
    /// Padding of table cell (middle size)
    /// </summary>
    public Thickness CellPaddingMD { get; set; }
    
    /// <summary>
    /// 单元格内间距（小尺寸）
    /// Padding of table cell (small size)
    /// </summary>
    public Thickness CellPaddingSM { get; set; }
    
    /// <summary>
    /// 表格边框/分割线颜色
    /// Border color of table
    /// </summary>
    public Color BorderColor { get; set; }
    
    /// <summary>
    /// 表头圆角
    /// Border radius of table header
    /// </summary>
    public CornerRadius HeaderBorderRadius { get; set; }
    
    /// <summary>
    /// 表格底部背景色
    /// Background of footer
    /// </summary>
    public Color FooterBg { get; set; }
    
    /// <summary>
    /// 表格底部文字颜色
    /// Color of footer text
    /// </summary>
    public Color FooterColor { get; set; }
    
    /// <summary>
    /// 单元格文字大小（默认大尺寸）
    /// Font size of table cell (large size by default)
    /// </summary>
    public double CellFontSize { get; set; }
    
    /// <summary>
    /// 单元格文字大小（中等尺寸）
    /// Font size of table cell (middle size)
    /// </summary>
    public double CellFontSizeMD { get; set; }
    
    /// <summary>
    /// 单元格文字大小（小尺寸）
    /// Font size of table cell (small size)
    /// </summary>
    public double CellFontSizeSM { get; set; }
    
    /// <summary>
    /// 表头分割线颜色
    /// Split border color of table header
    /// </summary>
    public Color HeaderSplitColor { get; set; }
    
    /// <summary>
    /// 固定表头排序激活态背景色
    /// Background color of fixed table header when sorted
    /// </summary>
    public Color FixedHeaderSortActiveBg { get; set; }
    
    /// <summary>
    /// 表头过滤按钮悬浮背景色
    /// Background color of table header filter button when hovered
    /// </summary>
    public Color HeaderFilterHoverBg { get; set; }
    
    /// <summary>
    /// 过滤下拉菜单选项背景
    /// Background of filter dropdown menu item
    /// </summary>
    public Color FilterDropdownMenuBg { get; set; }
    
    /// <summary>
    /// 过滤下拉菜单颜色
    /// Color of filter dropdown
    /// </summary>
    public Color FilterDropdownBg { get; set; }
    
    /// <summary>
    /// 展开按钮背景色
    /// Background of expand button
    /// </summary>
    public Color ExpandIconBg { get; set; }
    
    /// <summary>
    /// 选择列宽度
    /// Width of selection column
    /// </summary>
    public double SelectionColumnWidth { get; set; }
    
    /// <summary>
    /// Sticky 模式下滚动条背景色
    /// Background of sticky scrollbar
    /// </summary>
    public Color StickyScrollBarBg { get; set; }
    
    /// <summary>
    /// Sticky 模式下滚动条圆角
    /// Border radius of sticky scrollbar
    /// </summary>
    public CornerRadius StickyScrollBarBorderRadius { get; set; }

    #region 内部 Token

    internal Thickness ExpandIconMargin { get; set; }
    internal double ExpandIconHalfInner { get; set; }
    internal double ExpandIconSize { get; set; }
    internal double ExpandIconScale { get; set; }
    internal double SortIconSize { get; set; }
    internal Color HeaderIconColor { get; set; }
    internal Color HeaderIconHoverColor { get; set; }
    internal Thickness SortIndicatorLayoutMargin { get; set; }
    
    internal Thickness FilterIndicatorPadding { get; set; }

    #endregion

    #region 别名
    public double TableFontSize { get; set; }
    public Color TableBg { get; set; }
    public CornerRadius TableRadius { get; set; }
    public Thickness TablePadding { get; set; }
    public Thickness TablePaddingMiddle { get; set; }
    public Thickness TablePaddingSmall { get; set; }
    public Color TableBorderColor { get; set; }
    public Color TableHeaderTextColor { get; set; }
    public Color TableHeaderBg { get; set; }
    public Color TableFooterTextColor { get; set; }
    public Color TableFooterBg { get; set; }
    public Color TableHeaderCellSplitColor { get; set; }
    public Color TableHeaderSortBg { get; set; }
    public Color TableHeaderSortHoverBg { get; set; }
    public Color TableBodySortBg { get; set; }
    public Color TableFixedHeaderSortActiveBg { get; set; }
    public Color TableHeaderFilterActiveBg { get; set; }
    public Color TableFilterDropdownBg { get; set; }
    public double TableFilterDropdownHeight { get; set; }
    public Color TableRowHoverBg { get; set; }
    public Color TableSelectedRowBg { get; set; }
    public Color TableSelectedRowHoverBg { get; set; }
    public double TableFontSizeMiddle { get; set; }
    public double TableFontSizeSmall { get; set; }
    public double TableSelectionColumnWidth { get; set; }
    public Color TableExpandIconBg { get; set; }
    public double TableExpandColumnWidth { get; set; }
    public Color TableExpandedRowBg { get; set; }
    public double TableFilterDropdownWidth { get; set; }
    public double TableFilterDropdownSearchWidth { get; set; }
    public int ZIndexTableFixed { get; set; }
    public int ZIndexTableSticky { get; set; }
    
    // Virtual Scroll Bar
    public double TableScrollThumbSize { get; set; }
    public Color TableScrollThumbBg { get; set; }
    public Color TableScrollThumbBgHover { get; set; }
    public Color TableScrollBg { get; set; }
    
    #endregion
    
    protected override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var colorFillSecondarySolid = ColorUtils.OnBackground(SharedToken.ColorFillSecondary, SharedToken.ColorBgContainer);
        var colorFillContentSolid = ColorUtils.OnBackground(SharedToken.ColorFillContent, SharedToken.ColorBgContainer);
        var colorFillAlterSolid = ColorUtils.OnBackground(SharedToken.ColorFillAlter, SharedToken.ColorBgContainer);

        var baseColorAction      = SharedToken.ColorIcon;
        var baseColorActionHover = SharedToken.ColorIconHover;
        
        var expandIconHalfInner = SharedToken.ControlInteractiveSize / 2 - SharedToken.LineWidth;
        var expandIconSize      = expandIconHalfInner * 2 + SharedToken.LineWidth * 3;

        HeaderBg                    = colorFillAlterSolid;
        HeaderColor                 = SharedToken.ColorTextHeading;
        HeaderSortActiveBg          = colorFillSecondarySolid;
        HeaderSortHoverBg           = colorFillContentSolid;
        BodySortBg                  = colorFillAlterSolid;
        RowHoverBg                  = colorFillAlterSolid;
        RowSelectedBg               = SharedToken.ControlItemBgActive;
        RowSelectedHoverBg          = SharedToken.ControlItemBgActiveHover;
        RowExpandedBg               = SharedToken.ColorFillAlter;
        CellPadding                 = new Thickness(SharedToken.Padding);
        CellPaddingMD               = new Thickness(SharedToken.PaddingSM);
        CellPaddingSM               = new Thickness(SharedToken.PaddingXS);
        BorderColor                 = SharedToken.ColorBorderSecondary;
        HeaderBorderRadius          = SharedToken.BorderRadiusLG;
        FooterBg                    = colorFillAlterSolid;
        FooterColor                 = SharedToken.ColorTextHeading;
        CellFontSize                = SharedToken.FontSize;
        CellFontSizeMD              = SharedToken.FontSize;
        CellFontSizeSM              = SharedToken.FontSize;
        HeaderSplitColor            = SharedToken.ColorBorderSecondary;
        FixedHeaderSortActiveBg     = colorFillSecondarySolid;
        HeaderFilterHoverBg         = SharedToken.ColorFillContent;
        FilterDropdownMenuBg        = SharedToken.ColorBgContainer;
        FilterDropdownBg            = SharedToken.ColorBgContainer;
        ExpandIconBg                = SharedToken.ColorBgContainer;
        SelectionColumnWidth        = SharedToken.ControlHeight;
        StickyScrollBarBg           = SharedToken.ColorTextPlaceholder;
        StickyScrollBarBorderRadius = new CornerRadius(100);
        ExpandIconMargin            = new Thickness(0, 
            (SharedToken.FontSize * SharedToken.FontHeight - SharedToken.LineWidth * 3) / 2 -
            Math.Ceiling((SharedToken.FontSizeSM * 1.4 - SharedToken.LineWidth * 3) / 2), 
            0, 
            0);
        HeaderIconColor = ColorUtils.FromRgbF(
            baseColorAction.GetAlphaF() * SharedToken.OpacityLoading,
            baseColorAction.GetRedF(),
            baseColorAction.GetGreenF(),
            baseColorAction.GetBlueF());
        HeaderIconHoverColor = ColorUtils.FromRgbF(
            baseColorActionHover.GetAlphaF() * SharedToken.OpacityLoading,
            baseColorAction.GetRedF(),
            baseColorAction.GetGreenF(),
            baseColorAction.GetBlueF());
        ExpandIconHalfInner       = expandIconHalfInner;
        ExpandIconSize            = expandIconSize;
        ExpandIconScale           = SharedToken.ControlInteractiveSize / expandIconSize;
        SortIconSize              = SharedToken.FontHeight / 2.5;
        SortIndicatorLayoutMargin = new Thickness(SharedToken.MarginXS, 0, 0, 0);
        FilterIndicatorPadding    = new Thickness(SharedToken.PaddingXXS, SharedToken.PaddingXS);
        
        // 别名控件初始化
        TableFontSize = CellFontSize;
        TableBg       = SharedToken.ColorBgContainer;
        TableRadius   = HeaderBorderRadius;
        
        TablePadding                 = CellPadding;
        TablePaddingMiddle           = CellPaddingMD;
        TablePaddingSmall            = CellPaddingSM;
        TableBorderColor             = BorderColor;
        TableHeaderTextColor         = HeaderColor;
        TableHeaderBg                = HeaderBg;
        TableFooterTextColor         = FooterColor;
        TableFooterBg                = FooterBg;
        TableHeaderCellSplitColor    = HeaderSplitColor;
        TableHeaderSortBg            = HeaderSortActiveBg;
        TableHeaderSortHoverBg       = HeaderSortHoverBg;
        TableBodySortBg              = BodySortBg;
        TableFixedHeaderSortActiveBg = FixedHeaderSortActiveBg;
        TableHeaderFilterActiveBg    = HeaderFilterHoverBg;
        TableFilterDropdownBg        = FilterDropdownBg;
        TableRowHoverBg              = RowHoverBg;
        TableSelectedRowBg           = RowSelectedBg;
        TableSelectedRowHoverBg      = RowSelectedHoverBg;

        ZIndexTableFixed  = 2;
        ZIndexTableSticky = ZIndexTableFixed + 1;

        TableFontSizeMiddle       = CellFontSizeMD;
        TableFontSizeSmall        = CellFontSizeSM;
        TableSelectionColumnWidth = SelectionColumnWidth;
        TableExpandIconBg         = ExpandIconBg;
        TableExpandColumnWidth    = SharedToken.ControlInteractiveSize + SharedToken.Padding * 2;
        TableExpandedRowBg        = RowExpandedBg;
        
        // Dropdown
        TableFilterDropdownWidth       = 120;
        TableFilterDropdownHeight      = 264;
        TableFilterDropdownSearchWidth = 140;
        
        // Virtual Scroll Bar
        TableScrollThumbSize    = 8;
        TableScrollThumbBg      = StickyScrollBarBg;
        TableScrollThumbBgHover = SharedToken.ColorTextHeading;
        TableScrollBg           = SharedToken.ColorSplit;
    }
}