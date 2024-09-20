#nullable enable
using Avalonia.Media;
using Avalonia.Controls;
using AtomUI.Theme.Palette;
using AtomUI.Theme.Styling;
using Avalonia;

namespace AtomUI.Theme.Styling
{
    public partial class MergedGlobalToken
    {
        /// <summary>
        /// 品牌主色
        /// 品牌色是体现产品特性和传播理念最直观的视觉元素之一。在你完成品牌主色的选取之后，我们会自动帮你生成一套完整的色板，并赋予它们有效的设计语义
        /// </summary>
        public Color ColorPrimary { get; set; }
        /// <summary>
        /// 成功色
        /// 用于表示操作成功的 Token 序列，如 Result、Progress 等组件会使用该组梯度变量。
        /// </summary>
        public Color ColorSuccess { get; set; }
        /// <summary>
        /// 警戒色
        /// 用于表示操作警告的 Token 序列，如 Notification、 Alert等警告类组件或 Input 输入类等组件会使用该组梯度变量。
        /// </summary>
        public Color ColorWarning { get; set; }
        /// <summary>
        /// 错误色
        /// 用于表示操作失败的 Token 序列，如失败按钮、错误状态提示（Result）组件等。
        /// </summary>
        public Color ColorError { get; set; }
        /// <summary>
        /// 信息色
        /// 用于表示操作信息的 Token 序列，如 Alert 、Tag、 Progress 等组件都有用到该组梯度变量。
        /// </summary>
        public Color ColorInfo { get; set; }
        /// <summary>
        /// 基础文本色
        /// 用于派生文本色梯度的基础变量，v5 中我们添加了一层文本色的派生算法可以产出梯度明确的文本色的梯度变量。但请不要在代码中直接使用该 Seed Token ！
        /// </summary>
        public Color? ColorTextBase { get; set; }
        /// <summary>
        /// 基础背景色
        /// 用于派生背景色梯度的基础变量，v5 中我们添加了一层背景色的派生算法可以产出梯度明确的背景色的梯度变量。但请不要在代码中直接使用该 Seed Token ！
        /// </summary>
        public Color? ColorBgBase { get; set; }
        /// <summary>
        /// 超链接颜色
        /// 控制超链接的颜色。
        /// </summary>
        public Color? ColorLink { get; set; }
        /// <summary>
        /// 透明色
        /// </summary>
        public Color ColorTransparent { get; set; }
        //  ----------   Font   ---------- //
        /// <summary>
        /// 字体
        /// Ant Design 的字体家族中优先使用系统默认的界面字体，同时提供了一套利于屏显的备用字体库，来维护在不同平台以及浏览器的显示下，字体始终保持良好的易读性和可读性，体现了友好、稳定和专业的特性。
        /// </summary>
        public IList<string> FontFamily { get; set; } = Array.Empty<string>();
        /// <summary>
        /// 代码字体
        /// 代码字体，用于 Typography 内的 code、pre 和 kbd 类型的元素
        /// </summary>
        public IList<string> FontFamilyCode { get; set; } = Array.Empty<string>();
        /// <summary>
        /// 默认字号
        /// 设计系统中使用最广泛的字体大小，文本梯度也将基于该字号进行派生。
        /// </summary>
        public int FontSize { get; set; } = 14;
        //  ----------   Line   ---------- //
        /// <summary>
        /// 基础线宽
        /// 用于控制组件边框、分割线等的宽度
        /// </summary>
        public double LineWidth { get; set; } = 1;
        /// <summary>
        /// 线条样式
        /// 用于控制组件边框、分割线等的样式，默认是实线
        /// </summary>
        public LineStyle LineType { get; set; } = LineStyle.Solid;
        //  ----------   BorderRadius   ---------- //
        /// <summary>
        /// 基础圆角
        /// 基础组件的圆角大小，例如按钮、输入框、卡片等
        /// </summary>
        public CornerRadius BorderRadius { get; set; }
        /// <summary>
        /// 尺寸变化单位
        /// 用于控制组件尺寸的变化单位，在 Ant Design 中我们的基础单位为 4 ，便于更加细致地控制尺寸梯度
        /// </summary>
        public double SizeUnit { get; set; } = 4;
        /// <summary>
        /// 尺寸步长
        /// 用于控制组件尺寸的基础步长，尺寸步长结合尺寸变化单位，就可以派生各种尺寸梯度。通过调整步长即可得到不同的布局模式，例如 V5 紧凑模式下的尺寸步长为 2
        /// </summary>
        public double SizeStep { get; set; } = 4;
        /// <summary>
        /// 基础高度
        /// Ant Design 中按钮和输入框等基础控件的高度
        /// </summary>
        public double SizePopupArrow { get; set; } = 16;
        /// <summary>
        /// 基础高度
        /// Ant Design 中按钮和输入框等基础控件的高度
        /// </summary>
        public double ControlHeight { get; set; } = 32;
        //  ----------   zIndex   ---------- //
        /// <summary>
        /// 浮层基础 zIndex
        /// 所有组件的基础 Z 轴值，用于一些悬浮类的组件的可以基于该值 Z 轴控制层级，例如 BackTop、 Affix 等
        /// </summary>
        public int ZIndexBase { get; set; } = 0;
        /// <summary>
        /// 浮层基础 zIndex
        /// 浮层类组件的基础 Z 轴值，用于一些悬浮类的组件的可以基于该值 Z 轴控制层级，例如 FloatButton、 Affix、Modal 等
        /// </summary>
        public int ZIndexPopupBase { get; set; } = 1000;
        //  ----------   Opacity   ---------- //
        /// <summary>
        /// 图片不透明度
        /// </summary>
        public double OpacityImage { get; set; } = 1.0;
        //  ----------   motion   ---------- //
        // TODO: 缺一个懂 motion 的人来收敛 Motion 相关的 Token
        /// <summary>
        /// 动画时长变化单位
        /// 用于控制动画时长的变化单位
        /// </summary>
        public int MotionUnit { get; set; } = 100;
        /// <summary>
        /// 动画基础时长。
        /// </summary>
        public int MotionBase { get; set; } = 0;
        //  ----------   Style   ---------- //
        /// <summary>
        /// 线框风格
        /// 用于将组件的视觉效果变为线框化，如果需要使用 V4 的效果，需要开启配置项
        /// </summary>
        public bool Wireframe { get; set; } = false;
        /// <summary>
        /// 动画风格
        /// 用于配置动画效果，为 `false` 时则关闭动画
        /// </summary>
        public bool Motion { get; set; } = true;
        // ----------   Text   ---------- //
        /// <summary>
        /// 一级文本色
        /// 最深的文本色。为了符合W3C标准，默认的文本颜色使用了该色，同时这个颜色也是最深的中性色。
        /// </summary>
        public Color ColorText { get; set; }
        /// <summary>
        /// 二级文本色
        /// 作为第二梯度的文本色，一般用在不那么需要强化文本颜色的场景，例如 Label 文本、Menu 的文本选中态等场景。
        /// </summary>
        public Color ColorTextSecondary { get; set; }
        /// <summary>
        /// 三级文本色
        /// 第三级文本色一般用于描述性文本，例如表单的中的补充说明文本、列表的描述性文本等场景。
        /// </summary>
        public Color ColorTextTertiary { get; set; }
        /// <summary>
        /// 四级文本色
        /// 第四级文本色是最浅的文本色，例如表单的输入提示文本、禁用色文本等。
        /// </summary>
        public Color ColorTextQuaternary { get; set; }
        // ----------   Border   ---------- //
        /// <summary>
        /// 一级边框色
        /// 默认使用的边框颜色, 用于分割不同的元素，例如：表单的分割线、卡片的分割线等。
        /// </summary>
        public Color ColorBorder { get; set; }
        /// <summary>
        /// 二级边框色
        /// 比默认使用的边框色要浅一级，此颜色和 colorSplit 的颜色一致。使用的是实色。
        /// </summary>
        public Color ColorBorderSecondary { get; set; }
        // ----------   Fill   ---------- //
        /// <summary>
        /// 一级填充色
        /// 最深的填充色，用于拉开与二、三级填充色的区分度，目前只用在 Slider 的 hover 效果。
        /// </summary>
        public Color ColorFill { get; set; }
        /// <summary>
        /// 二级填充色
        /// 二级填充色可以较为明显地勾勒出元素形体，如 Rate、Skeleton 等。也可以作为三级填充色的 Hover 状态，如 Table 等。
        /// </summary>
        public Color ColorFillSecondary { get; set; }
        /// <summary>
        /// 三级填充色
        /// 三级填充色用于勾勒出元素形体的场景，如 Slider、Segmented 等。如无强调需求的情况下，建议使用三级填色作为默认填色。
        /// </summary>
        public Color ColorFillTertiary { get; set; }
        /// <summary>
        /// 四级填充色
        /// 最弱一级的填充色，适用于不易引起注意的色块，例如斑马纹、区分边界的色块等。
        /// </summary>
        public Color ColorFillQuaternary { get; set; }
        // ----------   Surface   ---------- //
        /// <summary>
        /// 布局背景色
        /// 该色用于页面整体布局的背景色，只有需要在页面中处于 B1 的视觉层级时才会使用该 token，其他用法都是错误的
        /// </summary>
        public Color ColorBgLayout { get; set; }
        /// <summary>
        /// 组件容器背景色
        /// 组件的容器背景色，例如：默认按钮、输入框等。务必不要将其与 `colorBgElevated` 混淆。
        /// </summary>
        public Color ColorBgContainer { get; set; }
        /// <summary>
        /// 浮层容器背景色
        /// 浮层容器背景色，在暗色模式下该 token 的色值会比 `colorBgContainer` 要亮一些。例如：模态框、弹出框、菜单等。
        /// </summary>
        public Color ColorBgElevated { get; set; }
        /// <summary>
        /// 引起注意的背景色
        /// 该色用于引起用户强烈关注注意的背景色，目前只用在 Tooltip 的背景色上。
        /// </summary>
        public Color ColorBgSpotlight { get; set; }
        /// <summary>
        /// 毛玻璃容器背景色
        /// 控制毛玻璃容器的背景色，通常为透明色。
        /// </summary>
        public Color ColorBgBlur { get; set; }
        /// <summary>
        /// 主色浅色背景色
        /// 主色浅色背景颜色，一般用于视觉层级较弱的选中状态。
        /// 1 号色
        /// </summary>
        public Color ColorPrimaryBg { get; set; }
        /// <summary>
        /// 主色浅色背景悬浮态
        /// 与主色浅色背景颜色相对应的悬浮态颜色。
        /// 2 号色
        /// </summary>
        public Color ColorPrimaryBgHover { get; set; }
        /// <summary>
        /// 主色描边色
        /// 主色梯度下的描边用色，用在 Slider 等组件的描边上。
        /// 3 号色
        /// </summary>
        public Color ColorPrimaryBorder { get; set; }
        /// <summary>
        /// 主色描边色悬浮态
        /// 主色梯度下的描边用色的悬浮态，Slider 、Button 等组件的描边 Hover 时会使用。
        /// 4 号色
        /// </summary>
        public Color ColorPrimaryBorderHover { get; set; }
        /// <summary>
        /// 主色悬浮态
        /// 主色梯度下的悬浮态。
        /// 5 号色
        /// </summary>
        public Color ColorPrimaryHover { get; set; }
        /// <summary>
        /// 主色激活态
        /// 主色梯度下的深色激活态。
        /// 7 号色
        /// </summary>
        public Color ColorPrimaryActive { get; set; }
        /// <summary>
        /// 主色文本悬浮态
        /// 主色梯度下的文本悬浮态。
        /// 8 号色
        /// </summary>
        public Color ColorPrimaryTextHover { get; set; }
        /// <summary>
        /// 主色文本
        /// 主色梯度下的文本颜色。
        /// 9 号色
        /// </summary>
        public Color ColorPrimaryText { get; set; }
        /// <summary>
        /// 主色文本激活态
        /// 主色梯度下的文本激活态。
        /// 10 号色
        /// </summary>
        public Color ColorPrimaryTextActive { get; set; }
        /// <summary>
        /// 成功色的浅色背景颜色
        /// 成功色的浅色背景颜色，用于 Tag 和 Alert 的成功态背景色
        /// 1 号色
        /// </summary>
        public Color ColorSuccessBg { get; set; }
        /// <summary>
        /// 成功色的浅色背景色悬浮态
        /// 成功色浅色背景颜色，一般用于视觉层级较弱的选中状态，不过 antd 目前没有使用到该 token
        /// 2 号色
        /// </summary>
        public Color ColorSuccessBgHover { get; set; }
        /// <summary>
        /// 成功色的描边色
        /// 成功色的描边色，用于 Tag 和 Alert 的成功态描边色
        /// 3 号色
        /// </summary>
        public Color ColorSuccessBorder { get; set; }
        /// <summary>
        /// 成功色的描边色悬浮态
        /// 4 号色
        /// </summary>
        public Color ColorSuccessBorderHover { get; set; }
        /// <summary>
        /// 成功色的深色悬浮态
        /// 5 号色
        /// </summary>
        public Color ColorSuccessHover { get; set; }
        /// <summary>
        /// 成功色的深色激活态
        /// 7 号色
        /// </summary>
        public Color ColorSuccessActive { get; set; }
        /// <summary>
        /// 成功色的文本悬浮态
        /// 8 号色
        /// </summary>
        public Color ColorSuccessTextHover { get; set; }
        /// <summary>
        /// 成功色的文本默认态
        /// 9 号色
        /// </summary>
        public Color ColorSuccessText { get; set; }
        /// <summary>
        /// 成功色的文本激活态
        /// 10 号色
        /// </summary>
        public Color ColorSuccessTextActive { get; set; }
        /// <summary>
        /// 警戒色的浅色背景颜色
        /// 1 号色
        /// </summary>
        public Color ColorWarningBg { get; set; }
        /// <summary>
        /// 警戒色的浅色背景色悬浮态
        /// 2 号色
        /// </summary>
        public Color ColorWarningBgHover { get; set; }
        /// <summary>
        /// 警戒色的描边色
        /// 3 号色
        /// </summary>
        public Color ColorWarningBorder { get; set; }
        /// <summary>
        /// 警戒色的描边色悬浮态
        /// 4 号色
        /// </summary>
        public Color ColorWarningBorderHover { get; set; }
        /// <summary>
        /// 警戒色的深色悬浮态
        /// 5 号色
        /// </summary>
        public Color ColorWarningHover { get; set; }
        /// <summary>
        /// 警戒色的深色激活态
        /// 7 号色
        /// </summary>
        public Color ColorWarningActive { get; set; }
        /// <summary>
        /// 警戒色的文本悬浮态
        /// 8 号色
        /// </summary>
        public Color ColorWarningTextHover { get; set; }
        /// <summary>
        /// 警戒色的文本默认态
        /// 9 号色
        /// </summary>
        public Color ColorWarningText { get; set; }
        /// <summary>
        /// 警戒色的文本激活态
        /// 10 号色
        /// </summary>
        public Color ColorWarningTextActive { get; set; }
        /// <summary>
        /// 错误色的浅色背景颜色
        /// 1 号色
        /// </summary>
        public Color ColorErrorBg { get; set; }
        /// <summary>
        /// 错误色的浅色背景色悬浮态
        /// 2 号色
        /// </summary>
        public Color ColorErrorBgHover { get; set; }
        /// <summary>
        /// 错误色的浅色背景色激活态
        /// 3 号色
        /// </summary>
        public Color ColorErrorBgActive { get; set; }
        /// <summary>
        /// 错误色的描边色
        /// 3 号色
        /// </summary>
        public Color ColorErrorBorder { get; set; }
        /// <summary>
        /// 错误色的描边色悬浮态
        /// 4 号色
        /// </summary>
        public Color ColorErrorBorderHover { get; set; }
        /// <summary>
        /// 错误色的深色悬浮态
        /// 5 号色
        /// </summary>
        public Color ColorErrorHover { get; set; }
        /// <summary>
        /// 错误色的深色激活态
        /// 7 号色
        /// </summary>
        public Color ColorErrorActive { get; set; }
        /// <summary>
        /// 错误色的文本悬浮态
        /// 8 号色
        /// </summary>
        public Color ColorErrorTextHover { get; set; }
        /// <summary>
        /// 错误色的文本默认态
        /// 9 号色
        /// </summary>
        public Color ColorErrorText { get; set; }
        /// <summary>
        /// 错误色的文本激活态
        /// 10 号色
        /// </summary>
        public Color ColorErrorTextActive { get; set; }
        /// <summary>
        /// 信息色的浅色背景颜色
        /// 1 号色
        /// </summary>
        public Color ColorInfoBg { get; set; }
        /// <summary>
        /// 信息色的浅色背景色悬浮态
        /// 2 号色
        /// </summary>
        public Color ColorInfoBgHover { get; set; }
        /// <summary>
        /// 信息色的描边色
        /// 3 号色
        /// </summary>
        public Color ColorInfoBorder { get; set; }
        /// <summary>
        /// 信息色的描边色悬浮态
        /// 4 号色
        /// </summary>
        public Color ColorInfoBorderHover { get; set; }
        /// <summary>
        /// 信息色的深色悬浮态
        /// 5 号色
        /// </summary>
        public Color ColorInfoHover { get; set; }
        /// <summary>
        /// 信息色的深色激活态
        /// 7 号色
        /// </summary>
        public Color ColorInfoActive { get; set; }
        /// <summary>
        /// 信息色的文本悬浮态
        /// 8 号色
        /// </summary>
        public Color ColorInfoTextHover { get; set; }
        /// <summary>
        /// 信息色的文本默认态
        /// 9 号色
        /// </summary>
        public Color ColorInfoText { get; set; }
        /// <summary>
        /// 信息色的文本激活态
        /// 10 号色
        /// </summary>
        public Color ColorInfoTextActive { get; set; }
        /// <summary>
        /// 超链接悬浮颜色
        /// 控制超链接悬浮时的颜色。
        /// </summary>
        public Color ColorLinkHover { get; set; }
        /// <summary>
        /// 超链接激活颜色
        /// 控制超链接被点击时的颜色。
        /// </summary>
        public Color ColorLinkActive { get; set; }
        /// <summary>
        /// 纯白色
        /// 不随主题变化的纯白色
        /// </summary>
        public Color ColorWhite { get; set; } = Color.FromRgb(255, 255, 255);
        /// <summary>
        /// 浮层的背景蒙层颜色
        /// 浮层的背景蒙层颜色，用于遮罩浮层下面的内容，Modal、Drawer 等组件的蒙层使用的是该 token
        /// </summary>
        public Color ColorBgMask { get; set; }
        /// <summary>
        /// 纯黑色
        /// 不随主题变化的纯黑色
        /// </summary>
        public Color ColorBlack { get; set; } = Color.FromRgb(0, 0, 0);
        /// <summary>
        /// 选择背景色
        /// </summary>
        public Color SelectionBackground { get; set; }
        /// <summary>
        /// 选择前景色
        /// </summary>
        public Color SelectionForeground { get; set; }
        /// <summary>
        /// 线宽
        /// 描边类组件的默认线宽，如 Button、Input、Select 等输入类控件。
        /// </summary>
        public double LineWidthBold { get; set; } = 1;
        /// <summary>
        /// 边框的宽细
        /// </summary>
        public Thickness BorderThickness { get; set; }
        /// <summary>
        /// XS号圆角
        /// XS号圆角，用于组件中的一些小圆角，如 Segmented 、Arrow 等一些内部圆角的组件样式中。
        /// </summary>
        public CornerRadius BorderRadiusXS { get; set; }
        /// <summary>
        /// SM号圆角
        /// SM号圆角，用于组件小尺寸下的圆角，如 Button、Input、Select 等输入类控件在 small size 下的圆角
        /// </summary>
        public CornerRadius BorderRadiusSM { get; set; }
        /// <summary>
        /// LG号圆角
        /// LG号圆角，用于组件中的一些大圆角，如 Card、Modal 等一些组件样式。
        /// </summary>
        public CornerRadius BorderRadiusLG { get; set; }
        /// <summary>
        /// 外部圆角
        /// </summary>
        public CornerRadius BorderRadiusOuter { get; set; }
        // Motion
        /// <summary>
        /// 动效播放速度，快速。用于小型元素动画交互
        /// </summary>
        public TimeSpan MotionDurationFast { get; set; }
        /// <summary>
        /// 动效播放速度，中速。用于中型元素动画交互
        /// </summary>
        public TimeSpan MotionDurationMid { get; set; }
        /// <summary>
        /// 动效播放速度，慢速。用于大型元素如面板动画交互
        /// </summary>
        public TimeSpan MotionDurationSlow { get; set; }
        /// <summary>
        /// 动效播放速度，最慢速。用于大型元素如面板动画交互
        /// </summary>
        public TimeSpan MotionDurationVerySlow { get; set; }
        /// <summary>
        /// XXL
        /// </summary>
        public double SizeXXL { get; set; } = 48;
        /// <summary>
        /// XL
        /// </summary>
        public double SizeXL { get; set; } = 32;
        /// <summary>
        /// LG
        /// </summary>
        public double SizeLG { get; set; } = 24;
        /// <summary>
        /// MD
        /// </summary>
        public double SizeMD { get; set; } = 20;
        /// <summary>
        /// Same as size by default, but could be larger in compact mode
        /// </summary>
        public double SizeMS { get; set; }
        /// <summary>
        /// 默认
        /// 默认尺寸
        /// </summary>
        public double Size { get; set; } = 16;
        /// <summary>
        /// SM
        /// </summary>
        public double SizeSM { get; set; } = 12;
        /// <summary>
        /// XS
        /// </summary>
        public double SizeXS { get; set; } = 8;
        /// <summary>
        /// XXS
        /// </summary>
        public double SizeXXS { get; set; } = 4;
        /// <summary>
        /// 更小的组件高度
        /// </summary>
        public double ControlHeightXS { get; set; }
        /// <summary>
        /// 较小的组件高度
        /// </summary>
        public double ControlHeightSM { get; set; }
        /// <summary>
        /// 较高的组件高度
        /// </summary>
        public double ControlHeightLG { get; set; }
        // Font Size
        /// <summary>
        /// 小号字体大小
        /// </summary>
        public double FontSizeSM { get; set; }
        /// <summary>
        /// 大号字体大小
        /// </summary>
        public double FontSizeLG { get; set; }
        /// <summary>
        /// 超大号字体大小
        /// </summary>
        public double FontSizeXL { get; set; }
        /// <summary>
        /// 一级标题字号
        /// H1 标签所使用的字号
        /// </summary>
        public double FontSizeHeading1 { get; set; } = 38;
        /// <summary>
        /// 二级标题字号
        /// h2 标签所使用的字号
        /// </summary>
        public double FontSizeHeading2 { get; set; } = 30;
        /// <summary>
        /// 三级标题字号
        /// h3 标签使用的字号
        /// </summary>
        public double FontSizeHeading3 { get; set; } = 24;
        /// <summary>
        /// 四级标题字号
        /// h4 标签使用的字号
        /// </summary>
        public double FontSizeHeading4 { get; set; } = 20;
        /// <summary>
        /// 五级标题字号
        /// h5 标签使用的字号
        /// </summary>
        public double FontSizeHeading5 { get; set; } = 16;
        // LineHeight
        /// <summary>
        /// 文本行高
        /// </summary>
        public double LineHeight { get; set; }
        /// <summary>
        /// 大型文本行高
        /// </summary>
        public double LineHeightLG { get; set; }
        /// <summary>
        /// 小型文本行高
        /// </summary>
        public double LineHeightSM { get; set; }
        // TextHeight
        /// <summary>
        /// Round of fontSize * lineHeight
        /// </summary>
        internal double FontHeight { get; set; }
        /// <summary>
        /// Round of fontSizeSM * lineHeightSM
        /// </summary>
        internal double FontHeightSM { get; set; }
        /// <summary>
        /// Round of fontSizeLG * lineHeightLG
        /// </summary>
        internal double FontHeightLG { get; set; }
        /// <summary>
        /// 一级标题行高
        /// H1 标签所使用的行高
        /// </summary>
        public double LineHeightHeading1 { get; set; }
        /// <summary>
        /// 二级标题行高
        /// h2 标签所使用的行高
        /// </summary>
        public double LineHeightHeading2 { get; set; }
        /// <summary>
        /// 三级标题行高
        /// h3 标签所使用的行高
        /// </summary>
        public double LineHeightHeading3 { get; set; }
        /// <summary>
        /// 四级标题行高
        /// h4 标签所使用的行高
        /// </summary>
        public double LineHeightHeading4 { get; set; }
        /// <summary>
        /// 五级标题行高
        /// h5 标签所使用的行高
        /// </summary>
        public double LineHeightHeading5 { get; set; }
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
        public double IconSizeXS { get; set; }
        /// <summary>
        /// 内联 PathIcon 大小定义
        /// </summary>
        public double IconSizeSM { get; set; }
        /// <summary>
        /// 内联 PathIcon 大小定义, 正常的尺寸
        /// </summary>
        public double IconSize { get; set; }
        /// <summary>
        /// 内联 PathIcon 大小定义, 最大的尺寸
        /// </summary>
        public double IconSizeLG { get; set; }
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
        public Color ControlItemBgHover { get; set; }
        /// <summary>
        /// 控制组件项在激活状态下的背景颜色
        /// </summary>
        public Color ControlItemBgActive { get; set; }
        /// <summary>
        /// 控制组件项在鼠标悬浮且激活状态下的背景颜色
        /// </summary>
        public Color ControlItemBgActiveHover { get; set; }
        /// <summary>
        /// 控制组件的交互大小
        /// </summary>
        public double ControlInteractiveSize { get; set; }
        /// <summary>
        /// 控制组件项在禁用状态下的激活背景颜色
        /// </summary>
        public Color ControlItemBgActiveDisabled { get; set; }
        /// <summary>
        /// 线条宽度(聚焦态)
        /// </summary>
        public double LineWidthFocus { get; set; }
        /// <summary>
        /// 波浪动画的波动范围
        /// </summary>
        public double WaveAnimationRange { get; set; }
        /// <summary>
        /// 波浪动画的初始透明度
        /// </summary>
        public double WaveStartOpacity { get; set; }
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
        /// 内容水平内间距（XS）
        /// 控制内容元素水平内间距，适用于小屏幕设备。
        /// </summary>
        public double PaddingContentHorizontalXS { get; set; }
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
}