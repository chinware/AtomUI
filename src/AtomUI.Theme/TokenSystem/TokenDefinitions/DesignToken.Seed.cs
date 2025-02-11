using Avalonia;
using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class DesignToken
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
    public double FontSize { get; set; } = 14;

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
    /// 
    /// 用于将组件的视觉效果变为线框化，如果需要使用 V4 的效果，需要开启配置项
    /// </summary>
    public bool Wireframe { get; set; } = false;

    /// <summary>
    /// 动画风格
    /// 
    /// 用于配置动画效果，为 `false` 时则关闭动画
    /// </summary>
    public bool EnableMotion { get; set; } = true;
    
    /// <summary>
    /// 是否开启波浪动画
    ///
    /// 按钮，单选按钮等点击后的波浪动画效果
    /// </summary>
    public bool EnableWaveAnimation { get; set; } = true;
}