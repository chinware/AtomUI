using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class SelectToken : AbstractControlDesignToken
{
    public const string ID = "Select";
    
    /// <summary>
    /// 多选标签背景色
    /// Background color of multiple tag
    /// </summary>
    public Color MultipleItemBg { get; set; }
    
    /// <summary>
    /// 多选标签边框色
    /// Border color of multiple tag
    /// </summary>
    public Color MultipleItemBorderColor { get; set; }
    
    /// <summary>
    /// 多选标签高度
    /// Height of multiple tag
    /// </summary>
    public double MultipleItemHeight { get; set; }
    
    /// <summary>
    /// 小号多选标签高度
    /// Height of multiple tag with small size
    /// </summary>
    public double MultipleItemHeightSM { get; set; }
    
    /// <summary>
    /// 大号多选标签高度
    /// Height of multiple tag with large size
    /// </summary>
    public double MultipleItemHeightLG { get; set; }
    
    /// <summary>
    /// 多选框禁用背景
    /// Background color of multiple selector when disabled
    /// </summary>
    public Color MultipleSelectorBgDisabled { get; set; }
    
    /// <summary>
    /// 多选标签禁用文本颜色
    /// Text color of multiple tag when disabled
    /// </summary>
    public Color MultipleItemColorDisabled { get; set; }
    
    /// <summary>
    /// 多选标签禁用边框色
    /// Border color of multiple tag when disabled
    /// </summary>
    public Color MultipleItemBorderColorDisabled { get; set; }
    
    /// <summary>
    /// 选项选中时文本颜色
    /// Text color when option is selected
    /// </summary>
    public Color OptionSelectedColor { get; set; }
    
    /// <summary>
    /// 选项选中时文本字重
    /// Font weight when option is selected
    /// </summary>
    public FontWeight OptionSelectedFontWeight { get; set; }
    
    /// <summary>
    /// 选项选中时背景色
    /// Font weight when option is selected
    /// </summary>
    public Color OptionSelectedBg { get; set; }
    
    /// <summary>
    /// 选项激活态时背景色
    /// Background color when option is active
    /// </summary>
    public Color OptionActiveBg { get; set; }
    
    /// <summary>
    /// 选项内间距
    /// Padding of option
    /// </summary>
    public Thickness OptionPadding { get; set; }
    
    /// <summary>
    /// 选项字体大小
    /// Font size of option
    /// </summary>
    public double OptionFontSize { get; set; }
    
    /// <summary>
    /// 选项高度
    /// Height of option
    /// </summary>
    public double OptionHeight { get; set; }
    
    /// <summary>
    /// 选框背景色
    /// Background color of selector
    /// </summary>
    public Color SelectorBg { get; set; }
    
    /// <summary>
    /// 清空按钮背景色
    /// Background color of clear button
    /// </summary>
    public Color ClearBg { get; set; }
    
    /// <summary>
    /// 单选大号回填项高度
    /// Height of single selected item with large size
    /// </summary>
    public double SingleItemHeightLG {  get; set; }

    /// <summary>
    /// 箭头的行末内边距
    /// Inline end padding of arrow
    /// </summary>
    public Thickness ShowArrowPaddingInlineEnd {  get; set; }

    /// <summary>
    /// 悬浮态边框色
    /// Hover border color
    /// </summary>
    public Color HoverBorderColor { get; set; }
    
    /// <summary>
    /// 激活态边框色
    /// Active border color
    /// </summary>
    public Color ActiveBorderColor { get; set; }

    /// <summary>
    /// 激活态 outline 颜色
    /// Active outline color
    /// </summary>
    public Color ActiveOutlineColor { get; set; }
    
    public Thickness SelectAffixPadding { get; set; }
    
    public Thickness FixedItemMargin { get; set; }
    
    /// <summary>
    /// 菜单的圆角
    /// </summary>
    public CornerRadius PopupBorderRadius { get; set; }
    
    /// <summary>
    /// 菜单 Popup 阴影
    /// </summary>
    public BoxShadows PopupBoxShadows { get; set; }
    
    /// <summary>
    /// 菜单内容边距
    /// </summary>
    public Thickness PopupContentPadding { get; set; }
    
    /// <summary>
    /// 顶层弹出菜单，距离顶层菜单项的边距
    /// </summary>
    public double PopupMarginToAnchor { get; set; }
    
    public SelectToken()
        : base(ID)
    {
    }

    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        
        // Item height default use `controlHeight - 2 * paddingXXS`,
        // but some case `paddingXXS=0`.
        // Let's fallback it.
        double dblPaddingXXS      = SharedToken.UniformlyPaddingXXS * 2;
        double dblLineWidth       = SharedToken.LineWidth * 2;
        double multipleItemHeight = Math.Min(SharedToken.ControlHeight - dblPaddingXXS, SharedToken.ControlHeight - dblLineWidth);
        double multipleItemHeightSM = Math.Min(SharedToken.ControlHeightSM - dblPaddingXXS, SharedToken.ControlHeightSM - dblLineWidth);
        double multipleItemHeightLG = Math.Min(SharedToken.ControlHeightLG - dblPaddingXXS, SharedToken.ControlHeightLG - dblLineWidth);
        FixedItemMargin = new Thickness(Math.Floor(SharedToken.UniformlyPaddingXXS / 2));

        OptionSelectedColor      = SharedToken.ColorText;
        OptionSelectedFontWeight = SharedToken.FontWeightStrong;
        OptionSelectedBg         = SharedToken.ControlItemBgActive;
        OptionActiveBg           = SharedToken.ControlItemBgHover;
        OptionPadding            = new Thickness(SharedToken.ControlPaddingHorizontal, (SharedToken.ControlHeight - SharedToken.FontHeight) / 2);
        OptionFontSize =  SharedToken.FontSize;
        OptionHeight = SharedToken.ControlHeight;
        SelectorBg = SharedToken.ColorBgContainer;
        ClearBg = SharedToken.ColorBgContainer;
        SingleItemHeightLG = SharedToken.ControlHeightLG;
        MultipleItemBg = SharedToken.ColorFillSecondary;
        MultipleItemBorderColor = Colors.Transparent;
        MultipleItemHeight = multipleItemHeight;
        MultipleItemHeightSM = multipleItemHeightSM;
        MultipleItemHeightLG = multipleItemHeightLG;
        MultipleSelectorBgDisabled = SharedToken.ColorBgContainerDisabled;
        MultipleItemColorDisabled = SharedToken.ColorTextDisabled;
        MultipleItemBorderColorDisabled = Colors.Transparent;
        ShowArrowPaddingInlineEnd = new Thickness(0, 0, Math.Ceiling(SharedToken.FontSize * 1.25d), 0);
        HoverBorderColor = SharedToken.ColorPrimaryHover;
        ActiveBorderColor = SharedToken.ColorPrimary;
        ActiveOutlineColor = SharedToken.ColorControlOutline;
        SelectAffixPadding = SharedToken.PaddingXXS;
        
        PopupBorderRadius   = SharedToken.BorderRadiusLG;
        PopupContentPadding = new Thickness(SharedToken.UniformlyPaddingXXS, PopupBorderRadius.TopLeft / 2);
        PopupBoxShadows     = SharedToken.BoxShadowsSecondary;
        PopupMarginToAnchor = SharedToken.UniformlyMarginXXS;
    }
}