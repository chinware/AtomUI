using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class PageSizeComboBoxItem : ComboBoxItem,
                                      ITokenResourceConsumer
{
    public int PageSize { get; set; }
    
    private CompositeDisposable? _tokenBindingsDisposable;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _tokenBindingsDisposable = new CompositeDisposable();
        this.AddTokenBindingDisposable(
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, typeof(ComboBoxItem)));
        base.OnAttachedToLogicalTree(e);
    }
}