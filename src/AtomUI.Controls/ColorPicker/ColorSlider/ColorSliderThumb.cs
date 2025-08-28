using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Primitives;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Utilities;

namespace AtomUI.Controls;

internal class ColorSliderThumb : Thumb, IResourceBindingManager
{
    #region 内部属性定义
    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable { get; set; }

    #endregion

    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        Debug.Assert(MathUtilities.AreClose(e.NewSize.Width, e.NewSize.Height));
        CornerRadius = new CornerRadius(e.NewSize.Width / 2);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, BorderThicknessProperty,
            SharedTokenKey.BorderThickness,
            BindingPriority.Template,
            new RenderScaleAwareThicknessConfigure(this)));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
}