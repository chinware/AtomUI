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
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(
            TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, typeof(ComboBoxItem)));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}