﻿using Avalonia;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class GlobalToken
{
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
    public CornerRadius BorderRadiusXS { get; set; } = new CornerRadius(2);

    /// <summary>
    /// SM号圆角
    /// SM号圆角，用于组件小尺寸下的圆角，如 Button、Input、Select 等输入类控件在 small size 下的圆角
    /// </summary>
    public CornerRadius BorderRadiusSM { get; set; } = new CornerRadius(4);

    /// <summary>
    /// LG号圆角
    /// LG号圆角，用于组件中的一些大圆角，如 Card、Modal 等一些组件样式。
    /// </summary>
    public CornerRadius BorderRadiusLG { get; set; } = new CornerRadius(8);

    /// <summary>
    /// 外部圆角
    /// </summary>
    public CornerRadius BorderRadiusOuter { get; set; } = new CornerRadius(4);

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
}