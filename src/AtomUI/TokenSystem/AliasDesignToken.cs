using Avalonia.Media;

namespace AtomUI.TokenSystem;

[GlobalDesignToken]
public class AliasDesignToken : MapDesignToken
{
   /// <summary>
   /// 内容区域背景色（悬停）
   /// 控制内容区域背景色在鼠标悬停时的样式。
   /// </summary>
   public Color ColorFillContentHover { get; set; }
   
   /// <summary>
   /// 替代背景色
   /// 控制元素替代背景色。
   /// </summary>
   public Color ColorFillAlter { get; set; }
   
   /// <summary>
   /// 内容区域背景色
   /// 控制内容区域的背景色。
   /// </summary>
   public Color ColorFillContent { get; set; }

   /// <summary>
   /// 容器禁用态下的背景色
   /// 控制容器在禁用状态下的背景色。
   /// </summary>
   public Color ColorBgContainerDisabled { get; set; }
   
   /// <summary>
   /// 文本悬停态背景色
   /// 控制文本在悬停状态下的背景色。
   /// </summary>
   public Color ColorBgTextHover { get; set; }

   /// <summary>
   /// 文本激活态背景色
   /// 控制文本在激活状态下的背景色。
   /// </summary>
   public Color ColorBgTextActive { get; set; }

   // Border
   /// <summary>
   /// 背景边框颜色
   /// 控制元素背景边框的颜色。
   /// </summary>
   public Color ColorBorderBg { get; set; }
   
   /// <summary>
   /// 分割线颜色
   /// 用于作为分割线的颜色，此颜色和 colorBorderSecondary 的颜色一致，但是用的是透明色。
   /// </summary>
   public Color ColorSplit { get; set; }

   // Text
   /// <summary>
   /// 占位文本颜色
   /// 控制占位文本的颜色。
   /// </summary>
   public Color ColorTextPlaceholder { get; set; }
   
   /// <summary>
   /// 禁用字体颜色
   /// 控制禁用状态下的字体颜色。
   /// </summary>
   public Color ColorTextDisabled { get; set; }
   
   /// <summary>
   /// 标题字体颜色
   /// 控制标题字体颜色。
   /// </summary>
   public Color ColorTextHeading { get; set; }

   /// <summary>
   /// 文本标签字体颜色
   /// 控制文本标签字体颜色。
   /// </summary>
   public Color ColorTextLabel { get; set; }
   
   /// <summary>
   /// 文本描述字体颜色
   /// 控制文本描述字体颜色。
   /// </summary>
   public Color ColorTextDescription { get; set; }

   /// <summary>
   /// 固定文本高亮颜色
   /// 控制带背景色的文本，例如 Primary Button 组件中的文本高亮颜色。
   /// </summary>
   public Color ColorTextLightSolid { get; set; }
   
   /// <summary>
   /// 弱操作图标颜色
   /// 控制弱操作图标的颜色，例如 allowClear 或 Alert 关闭按钮。
   /// </summary>
   public Color ColorIcon { get; set; }
   
   /// <summary>
   /// 弱操作图标悬浮态颜色
   /// 控制弱操作图标在悬浮状态下的颜色，例如 allowClear 或 Alert 关闭按钮。
   /// </summary>
   public Color ColorIconHover { get; set; }
   
   /// <summary>
   /// 高亮颜色
   /// 控制页面元素高亮时的颜色。
   /// </summary>
   public Color ColorHighlight { get; set; }
   
   /// <summary>
   /// 输入组件的 Outline 颜色
   /// 控制输入组件的外轮廓线颜色。
   /// </summary>
   public Color ColorControlOutline { get; set; }
   
   /// <summary>
   /// 警告状态下的 Outline 颜色
   /// 控制输入组件警告状态下的外轮廓线颜色。
   /// </summary>
   public Color ColorWarningOutline { get; set; }
   
   /// <summary>
   /// 错误状态下的 Outline 颜色
   /// 控制输入组件错误状态下的外轮廓线颜色。
   /// </summary>
   public Color ColorErrorOutline { get; set; }

   // Font
   /// <summary>
   /// 选择器、级联选择器等中的操作图标字体大小
   /// 控制选择器、级联选择器等中的操作图标字体大小。正常情况下与 fontSizeSM 相同。
   /// </summary>
   public double FontSizeIcon { get; set; }
   
   /// <summary>
   /// 内联 PathIcon 大小定义, 最小的尺寸
   /// </summary>
   public int IconSizeXS { get; set; }
   
   /// <summary>
   /// 内联 PathIcon 大小定义
   /// </summary>
   public int IconSizeSM { get; set; }
   
   /// <summary>
   /// 内联 PathIcon 大小定义, 正常的尺寸
   /// </summary>
   public int IconSize { get; set; }
   
   /// <summary>
   /// 内联 PathIcon 大小定义, 最大的尺寸
   /// </summary>
   public int IconSizeLG { get; set; }
   
   /// <summary>
   /// 标题类组件（如 h1、h2、h3）或选中项的字体粗细
   /// 控制标题类组件（如 h1、h2、h3）或选中项的字体粗细。
   /// </summary>
   public double FontWeightStrong { get; set; }

   // Control
   /// <summary>
   /// 输入组件的外轮廓线宽度
   /// 控制标题类组件（如 h1、h2、h3）或选中项的字体粗细。
   /// </summary>
   public double ControlOutlineWidth { get; set; }
   
   /// <summary>
   /// 控制组件项在鼠标悬浮时的背景颜色
   /// </summary>
   public Color ControlItemBgHover { get; set; } // Note. It also is a color
   
   /// <summary>
   /// 控制组件项在激活状态下的背景颜色
   /// </summary>
   public Color ControlItemBgActive { get; set; } // Note. It also is a color
   
   /// <summary>
   /// 控制组件项在鼠标悬浮且激活状态下的背景颜色
   /// </summary>
   public Color ControlItemBgActiveHover { get; set; } // Note. It also is a color
   
   /// <summary>
   /// 控制组件的交互大小
   /// </summary>
   public double ControlInteractiveSize { get; set; }

   /// <summary>
   /// 控制组件项在禁用状态下的激活背景颜色
   /// </summary>
   public Color ControlItemBgActiveDisabled { get; set; } // Note. It also is a color

   // Line
   /// <summary>
   /// 线条宽度(聚焦态)
   /// </summary>
   public double LineWidthFocus { get; set; }
   
   /// <summary>
   /// 波浪动画的波动范围
   /// </summary>
   public double WaveAnimationRange { get; set; }

   // Padding
   /// <summary>
   /// 极小内间距
   /// 控制元素的极小内间距。
   /// </summary>
   public double PaddingXXS { get; set; }
   
   /// <summary>
   /// 特小内间距
   /// 控制元素的特小内间距。
   /// </summary>
   public double PaddingXS { get; set; }
   
   /// <summary>
   /// 小内间距
   /// 控制元素的小内间距。
   /// </summary>
   public double PaddingSM { get; set; }
   
   /// <summary>
   /// 内间距
   /// 控制元素的内间距。
   /// </summary>
   public double Padding { get; set; }
   
   /// <summary>
   /// 中等内间距
   /// 控制元素的中等内间距。
   /// </summary>
   public double PaddingMD { get; set; }
   
   /// <summary>
   /// 大内间距
   /// 控制元素的大内间距。
   /// </summary>
   public double PaddingLG { get; set; }
   
   /// <summary>
   /// 特大内间距
   /// 控制元素的特大内间距。
   /// </summary>
   public double PaddingXL { get; set; }

   // Padding Content
   /// <summary>
   /// 内容水平内间距（LG）
   /// 控制内容元素水平内间距，适用于大屏幕设备。
   /// </summary>
   public double PaddingContentHorizontalLG { get; set; }
   
   /// <summary>
   /// 内容水平内间距
   /// 控制内容元素水平内间距。
   /// </summary>
   public double PaddingContentHorizontal { get; set; }
   
   /// <summary>
   /// 内容水平内间距（SM）
   /// 控制内容元素水平内间距，适用于小屏幕设备。
   /// </summary>
   public double PaddingContentHorizontalSM { get; set; }

   /// <summary>
   /// 内容垂直内间距（LG）
   /// 控制内容元素垂直内间距，适用于大屏幕设备。
   /// </summary>
   public double PaddingContentVerticalLG { get; set; }
   
   /// <summary>
   /// 内容垂直内间距
   /// 控制内容元素垂直内间距。
   /// </summary>
   public double PaddingContentVertical { get; set; }

   /// <summary>
   /// 内容垂直内间距（SM）
   /// 控制内容元素垂直内间距，适用于小屏幕设备。
   /// </summary>
   public double PaddingContentVerticalSM { get; set; }

   // Margin
   
   /// <summary>
   /// 外边距 XXS
   /// 控制元素外边距，最小尺寸。
   /// </summary>
   public double MarginXXS { get; set; }

   /// <summary>
   /// 外边距 XS
   /// 控制元素外边距，小尺寸。
   /// </summary>
   public double MarginXS { get; set; }

   /// <summary>
   /// 外边距 SM
   /// 控制元素外边距，中小尺寸。
   /// </summary>
   public double MarginSM { get; set; }
   
   /// <summary>
   /// 外边距
   /// 控制元素外边距，中等尺寸。
   /// </summary>
   public double Margin { get; set; }
   
   /// <summary>
   /// 外边距 MD
   /// 控制元素外边距，中大尺寸。
   /// </summary>
   public double MarginMD { get; set; }
   
   /// <summary>
   /// 外边距 LG
   /// 控制元素外边距，大尺寸。
   /// </summary>
   public double MarginLG { get; set; }
   
   /// <summary>
   /// 外边距 XL
   /// 控制元素外边距，超大尺寸。
   /// </summary>
   public double MarginXL { get; set; }
   
   /// <summary>
   /// 外边距 XXL
   /// 控制元素外边距，最大尺寸。
   /// </summary>
   public double MarginXXL { get; set; }

   // =============== Legacy: should be remove ===============
   /// <summary>
   /// 加载状态透明度
   /// 控制加载状态的透明度。
   /// </summary>
   public double OpacityLoading { get; set; }

   /// <summary>
   /// 一级阴影
   /// 控制元素阴影样式。
   /// </summary>
   public BoxShadows BoxShadows { get; set; }
   
   /// <summary>
   /// 二级阴影
   /// 控制元素二级阴影样式。
   /// </summary>
   public BoxShadows BoxShadowsSecondary { get; set; }
   
   /// <summary>
   /// 三级阴影
   /// 控制元素三级盒子阴影样式。
   /// </summary>
   public BoxShadows BoxShadowsTertiary { get; set; }

   /// <summary>
   /// 链接文本装饰
   /// 控制链接文本的装饰样式。
   /// </summary>
   public TextDecorationInfo? LinkDecoration { get; set; }
   
   /// <summary>
   /// 链接鼠标悬浮时文本装饰
   /// 控制链接鼠标悬浮时文本的装饰样式。
   /// </summary>
   public TextDecorationInfo? LinkHoverDecoration { get; set; }
   
   /// <summary>
   /// 链接聚焦时文本装饰
   /// 控制链接聚焦时文本的装饰样式。
   /// </summary>
   public TextDecorationInfo? LinkFocusDecoration { get; set; }

   /// <summary>
   /// 控制水平内间距
   /// 控制元素水平内间距。
   /// </summary>
   public double ControlPadding { get; set; }
   
   /// <summary>
   /// 控制中小尺寸水平内间距
   /// 控制元素中小尺寸水平内间距。
   /// </summary>
   public double ControlPaddingSM { get; set; }

   // Media queries breakpoints
   /// <summary>
   /// 屏幕宽度（像素） - 超小屏幕
   /// 控制超小屏幕的屏幕宽度。
   /// </summary>
   public int ScreenXS { get; set; }

   /// <summary>
   /// 屏幕宽度（像素） - 超小屏幕最小值
   /// 控制超小屏幕的最小宽度。
   /// </summary>
   public int ScreenXSMin { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 超小屏幕最大值
   /// 控制超小屏幕的最大宽度。
   /// </summary>
   public int ScreenXSMax { get; set; }

   /// <summary>
   /// 屏幕宽度（像素） - 小屏幕
   /// 控制小屏幕的屏幕宽度。
   /// </summary>
   public int ScreenSM { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 小屏幕最小值
   /// 控制小屏幕的最小宽度。
   /// </summary>
   public int ScreenSMMin { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 小屏幕最大值
   /// 控制小屏幕的最大宽度。
   /// </summary>
   public int ScreenSMMax { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 中等屏幕
   /// 控制中等屏幕的屏幕宽度。
   /// </summary>
   public int ScreenMD { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 中等屏幕最小值
   /// 控制中等屏幕的最小宽度。
   /// </summary>
   public int ScreenMDMin { get; set; }

   /// <summary>
   /// 屏幕宽度（像素） - 中等屏幕最大值
   /// 控制中等屏幕的最大宽度。
   /// </summary>
   public int ScreenMDMax { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 大屏幕
   /// 控制大屏幕的屏幕宽度。
   /// </summary>
   public int ScreenLG { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 大屏幕最小值
   /// 控制大屏幕的最小宽度。
   /// </summary>
   public int ScreenLGMin { get; set; }

   /// <summary>
   /// 屏幕宽度（像素） - 大屏幕最大值
   /// 控制大屏幕的最大宽度。
   /// </summary>
   public int ScreenLGMax { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 超大屏幕
   /// 控制超大屏幕的屏幕宽度。
   /// </summary>
   public int ScreenXL { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 超大屏幕最小值
   /// 控制超大屏幕的最小宽度。
   /// </summary>
   public int ScreenXLMin { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 超大屏幕最大值
   /// 控制超大屏幕的最大宽度。
   /// </summary>
   public int ScreenXLMax { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 超超大屏幕
   /// 控制超超大屏幕的屏幕宽度。
   /// </summary>
   public int ScreenXXL { get; set; }
   
   /// <summary>
   /// 屏幕宽度（像素） - 超超大屏幕最小值
   /// 控制超超大屏幕的最小宽度。
   /// </summary>
   public int ScreenXXLMin { get; set; }
}