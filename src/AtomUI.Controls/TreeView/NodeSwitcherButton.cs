﻿using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Media;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

/// <summary>
/// 如果 checked icon 和 unchecked icon 都设置的时候，就直接切换
/// 否则就直接改变 render transform
/// </summary>
internal class NodeSwitcherButton : ToggleIconButton
{
    #region 公共属性

    public static readonly StyledProperty<Icon?> LoadingIconProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, Icon?>(nameof(LoadingIcon));

    public static readonly StyledProperty<Icon?> LeafIconProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, Icon?>(nameof(LeafIcon));

    public static readonly StyledProperty<bool> IsLeafProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, bool>(nameof(IsLeaf));

    public Icon? LoadingIcon
    {
        get => GetValue(LoadingIconProperty);
        set => SetValue(LoadingIconProperty, value);
    }

    public Icon? LeafIcon
    {
        get => GetValue(LeafIconProperty);
        set => SetValue(LeafIconProperty, value);
    }

    public bool IsLeaf
    {
        get => GetValue(IsLeafProperty);
        set => SetValue(IsLeafProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsIconVisibleProperty
        = AvaloniaProperty.Register<NodeSwitcherButton, bool>(nameof(IsIconVisible), true);

    internal bool IsIconVisible
    {
        get => GetValue(IsIconVisibleProperty);
        set => SetValue(IsIconVisibleProperty, value);
    }

    #endregion

    private readonly BorderRenderHelper _borderRenderHelper;

    static NodeSwitcherButton()
    {
        AffectsMeasure<NodeSwitcherButton>(LoadingIconProperty, LeafIconProperty);
        AffectsRender<NodeSwitcherButton>(BackgroundProperty);
    }

    public NodeSwitcherButton()
    {
        _borderRenderHelper = new BorderRenderHelper();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        LoadingIcon ??= AntDesignIconPackage.LoadingOutlined();
        ConfigureFixedSizeIcon(LoadingIcon);
        LoadingIcon.LoadingAnimation = IconAnimation.Spin;

        LeafIcon ??= AntDesignIconPackage.FileOutlined();

        ConfigureFixedSizeIcon(LeafIcon);
        base.OnApplyTemplate(e);
        ApplyIconToContent();
        Transitions ??= new Transitions
        {
            AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty)
        };
    }

    private void ConfigureFixedSizeIcon(Icon icon)
    {
        icon.SetCurrentValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
        icon.SetCurrentValue(VerticalAlignmentProperty, VerticalAlignment.Center);
        UIStructureUtils.SetTemplateParent(icon, this);
        TokenResourceBinder.CreateGlobalResourceBinding(icon, WidthProperty, GlobalTokenResourceKey.IconSize);
        TokenResourceBinder.CreateGlobalResourceBinding(icon, HeightProperty, GlobalTokenResourceKey.IconSize);
        BindUtils.RelayBind(this, IsIconVisibleProperty, icon, IsVisibleProperty);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (VisualRoot is not null)
        {
            if (change.Property == IsLeafProperty)
            {
                ApplyIconToContent();
            }
        }

        if (change.Property == LoadingIconProperty ||
            change.Property == LeafIconProperty)
        {
            if (change.NewValue is Icon newIcon)
            {
                ConfigureFixedSizeIcon(newIcon);
                ApplyIconToContent();
                RenderTransform = null;
            }
        }

        if (change.Property == CheckedIconProperty ||
            change.Property == UnCheckedIconProperty)
        {
            RenderTransform = null;
            ApplyIconToContent();
        }
    }

    internal override void ApplyIconToContent()
    {
        if (!IsLeaf)
        {
            if (IsChecked.HasValue)
            {
                if (CheckedIcon is not null && UnCheckedIcon is not null)
                {
                    // 直接切换模式
                    if (IsChecked.Value)
                    {
                        Content = CheckedIcon;
                    }
                    else
                    {
                        Content = UnCheckedIcon;
                    }
                }
                else if (UnCheckedIcon is not null)
                {
                    // 通过 render transform 进行设置
                    Content = UnCheckedIcon;
                    if (IsChecked.Value)
                    {
                        RenderTransform = new RotateTransform(90);
                    }
                    else
                    {
                        RenderTransform = null;
                    }
                }
            }
            else
            {
                Content = LoadingIcon;
            }
        }
        else
        {
            RenderTransform = null;
            Content         = LeafIcon;
        }
    }

    public override void Render(DrawingContext context)
    {
        if (IsIconVisible && !IsLeaf)
        {
            _borderRenderHelper.Render(context,
                Bounds.Size,
                new Thickness(),
                CornerRadius,
                BackgroundSizing.InnerBorderEdge,
                Background,
                null,
                default);
        }
    }
}