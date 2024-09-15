﻿using AtomUI.Theme.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

[ControlDesignToken]
public class ArrowDecoratedBoxToken : AbstractControlDesignToken
{
    public const string ID = "ArrowDecoratedBox";

    /// <summary>
    /// 箭头三角形大小
    /// </summary>
    public double ArrowSize { get; set; }

    /// <summary>
    /// 默认的内边距
    /// </summary>
    public Thickness Padding { get; set; }

    public ArrowDecoratedBoxToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ArrowSize = _globalToken.SeedToken.SizePopupArrow / 1.3;
        Padding   = new Thickness(_globalToken.PaddingXS);
    }
}