using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

public class TextBlock : AvaloniaTextBlock,
                         ITokenResourceConsumer
{
    #region 内部属性定义

    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    
    #endregion
    
    private CompositeDisposable? _tokenBindingsDisposable;
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, LineHeightProperty, SharedTokenKey.FontHeight));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}