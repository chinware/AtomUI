﻿using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace AtomUI.Controls;

using AvaloniaListBox = Avalonia.Controls.ListBox;

public class ListBox : AvaloniaListBox,
                       IAnimationAwareControl,
                       IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<ListBox, SizeType>(nameof(SizeType), SizeType.Middle);

    public static readonly StyledProperty<bool> DisabledItemHoverEffectProperty =
        AvaloniaProperty.Register<ListBox, bool>(nameof(DisabledItemHoverEffect));
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<ListBox, bool>(nameof(IsMotionEnabled));

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<ListBox, bool>(nameof(IsWaveAnimationEnabled));

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public bool DisabledItemHoverEffect
    {
        get => GetValue(DisabledItemHoverEffectProperty);
        set => SetValue(DisabledItemHoverEffectProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }
    #endregion
    
    #region 内部属性定义

    Control IAnimationAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ListBoxToken.ID;
    
    #endregion

    public ListBox()
    {
        this.RegisterResources();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }
    
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new ListBoxItem();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        return base.ArrangeOverride(finalSize.Deflate(new Thickness(BorderThickness.Left,
            BorderThickness.Top,
            BorderThickness.Right,
            BorderThickness.Bottom)));
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is ListBoxItem listBoxItem)
        {
            BindUtils.RelayBind(this, IsMotionEnabledProperty, listBoxItem, ListBoxItem.IsMotionEnabledProperty);
            BindUtils.RelayBind(this, SizeTypeProperty, listBoxItem, ListBoxItem.SizeTypeProperty);
            BindUtils.RelayBind(this, DisabledItemHoverEffectProperty, listBoxItem,
                ListBoxItem.DisabledItemHoverEffectProperty);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this));
    }
}