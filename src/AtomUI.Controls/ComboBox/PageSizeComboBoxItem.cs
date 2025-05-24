using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

internal class PageSizeComboBoxItem : ComboBoxItem,
                                      IResourceBindingManager
{
    public int PageSize { get; set; }
    
    private CompositeDisposable? _resourceBindingsDisposable;
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;
    
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, typeof(ComboBoxItem)));
        base.OnAttachedToLogicalTree(e);
    }
}