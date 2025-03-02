﻿using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public enum SearchEditButtonStyle
{
    Default,
    Primary
}

public class SearchEdit : LineEdit
{
    #region 公共属性定义

    public static readonly StyledProperty<SearchEditButtonStyle> SearchButtonStyleProperty =
        AvaloniaProperty.Register<SearchEdit, SearchEditButtonStyle>(nameof(SearchButtonStyle));

    public static readonly StyledProperty<string> SearchButtonTextProperty =
        AvaloniaProperty.Register<SearchEdit, string>(nameof(SearchButtonText));

    public SearchEditButtonStyle SearchButtonStyle
    {
        get => GetValue(SearchButtonStyleProperty);
        set => SetValue(SearchButtonStyleProperty, value);
    }

    public object? SearchButtonText
    {
        get => GetValue(SearchButtonTextProperty);
        set => SetValue(SearchButtonTextProperty, value);
    }

    #endregion
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }
}