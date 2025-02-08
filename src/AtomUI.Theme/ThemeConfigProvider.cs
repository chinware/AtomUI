using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Theme;

public class ThemeConfigProvider : Control, IThemeConfigProvider
{
    #region 公共属性定义
    public static readonly StyledProperty<Control?> ContentProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, Control?>(nameof(Content));
    
    [Content]
    public Control? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public DesignToken SharedToken => _tokenResourceLayer.SharedToken;
    
    public Dictionary<string, IControlDesignToken> ControlTokens => _tokenResourceLayer.ControlTokens;
    
    public IList<string> Algorithms => _tokenResourceLayer.Algorithms;
    
    public bool IsDarkMode => _tokenResourceLayer.IsDarkMode;
    
    public IList<TokenSetter> SharedTokenSetters => _sharedTokenSetters ??= new();
    
    #endregion

    #region 内部属性定义

    private List<TokenSetter> _sharedTokenSetters;

    #endregion
    
    private TokenResourceLayer _tokenResourceLayer;
    private bool _needCalculateTokenResources = true;

    static ThemeConfigProvider()
    {
        AffectsMeasure<ThemeConfigProvider>(ContentProperty);
        ContentProperty.Changed.AddClassHandler<ThemeConfigProvider>((x, e) => x.ContentChanged(e));
    }

    public ThemeConfigProvider()
    {
        _tokenResourceLayer = new TokenResourceLayer();
        _sharedTokenSetters = new List<TokenSetter>();
    }
    
    private void ContentChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldChild = (Control?)e.OldValue;
        var newChild = (Control?)e.NewValue;

        if (oldChild != null)
        {
            ((ISetLogicalParent)oldChild).SetParent(null);
            LogicalChildren.Clear();
            VisualChildren.Remove(oldChild);
        }

        if (newChild != null)
        {
            ((ISetLogicalParent)newChild).SetParent(this);
            VisualChildren.Add(newChild);
            LogicalChildren.Add(newChild);
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (_needCalculateTokenResources)
        {
            _tokenResourceLayer.Calculate();
            _tokenResourceLayer.MountTokenResources(this);
            _needCalculateTokenResources = false;
        }
    }
}