using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public class ColorNeutralMapDesignToken : AbstractDesignToken
{
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
}

[GlobalDesignToken]
public class ColorPrimaryMapDesignToken : AbstractDesignToken
{
   /// <summary>
   /// 品牌主色
   /// 品牌色是体现产品特性和传播理念最直观的视觉元素之一，用于产品的主色调、主按钮、主图标、主文本等
   /// 6 号色
   /// </summary>
   public Color ColorPrimary { get; set; }

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
}

[GlobalDesignToken]
public class ColorSuccessMapDesignToken : AbstractDesignToken
{
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
   /// 成功色
   /// 默认的成功色，如 Result、Progress 等组件中都有使用该颜色
   /// 6 号色
   /// </summary>
   public Color ColorSuccess { get; set; }

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
}

[GlobalDesignToken]
public class ColorWarningMapDesignToken : AbstractDesignToken
{
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
   /// 警戒色
   /// 最常用的警戒色，例如 Notification、 Alert等警告类组件或 Input 输入类等组件会使用该颜色
   /// 6 号色
   /// </summary>
   public Color ColorWarning { get; set; }

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
}

[GlobalDesignToken]
public class ColorInfoMapDesignToken : AbstractDesignToken
{
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
   /// 信息色
   /// 6 号色
   /// </summary>
   public Color ColorInfo { get; set; }

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
}

[GlobalDesignToken]
public class ColorErrorMapDesignToken : AbstractDesignToken
{
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
   /// 错误色
   /// 6 号色
   /// </summary>
   public Color ColorError { get; set; }

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
}

[GlobalDesignToken]
public class ColorLinkMapDesignToken : AbstractDesignToken
{
   /// <summary>
   /// 超链接颜色
   /// 控制超链接的颜色。
   /// </summary>
   public Color ColorLink { get; set; }
   
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
}

[GlobalDesignToken]
public class ColorMapDesignToken : AbstractDesignToken
{
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

   public ColorNeutralMapDesignToken ColorNeutralToken { get; set; }
   public ColorPrimaryMapDesignToken ColorPrimaryToken { get; set; }
   public ColorSuccessMapDesignToken ColorSuccessToken { get; set; }
   public ColorWarningMapDesignToken ColorWarningToken { get; set; }
   public ColorErrorMapDesignToken ColorErrorToken { get; set; }
   public ColorInfoMapDesignToken ColorInfoToken { get; set; }
   public ColorLinkMapDesignToken ColorLinkToken { get; set; }
   
   public ColorMapDesignToken()
   {
      ColorNeutralToken = new ColorNeutralMapDesignToken();
      ColorPrimaryToken = new ColorPrimaryMapDesignToken();
      ColorSuccessToken = new ColorSuccessMapDesignToken();
      ColorWarningToken = new ColorWarningMapDesignToken();
      ColorErrorToken = new ColorErrorMapDesignToken();
      ColorInfoToken = new ColorInfoMapDesignToken();
      ColorLinkToken = new ColorLinkMapDesignToken();
   }

   public override void BuildResourceDictionary(IResourceDictionary dictionary)
   {
      ColorNeutralToken.BuildResourceDictionary(dictionary);
      ColorPrimaryToken.BuildResourceDictionary(dictionary);
      ColorSuccessToken.BuildResourceDictionary(dictionary);
      ColorWarningToken.BuildResourceDictionary(dictionary);
      ColorErrorToken.BuildResourceDictionary(dictionary);
      ColorInfoToken.BuildResourceDictionary(dictionary);
      ColorLinkToken.BuildResourceDictionary(dictionary);
   }

   internal override void LoadConfig(IDictionary<string, string> tokenConfigInfo)
   {
      ColorNeutralToken.LoadConfig(tokenConfigInfo);
      ColorPrimaryToken.LoadConfig(tokenConfigInfo);
      ColorSuccessToken.LoadConfig(tokenConfigInfo);
      ColorWarningToken.LoadConfig(tokenConfigInfo);
      ColorErrorToken.LoadConfig(tokenConfigInfo);
      ColorInfoToken.LoadConfig(tokenConfigInfo);
      ColorLinkToken.LoadConfig(tokenConfigInfo);
   }
}