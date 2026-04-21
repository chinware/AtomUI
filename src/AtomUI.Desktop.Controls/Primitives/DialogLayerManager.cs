using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls.Primitives;

internal class DialogLayerManager : VisualLayerManager
{
    private const int DialogZIndex = int.MaxValue - 1000;

    private static readonly AttachedProperty<DialogLayerManager?> InstanceProperty =
        AvaloniaProperty.RegisterAttached<DialogLayerManager, TopLevel, DialogLayerManager?>("Instance");

    public static DialogLayerManager? GetInstance(TopLevel topLevel) => topLevel.GetValue(InstanceProperty);

    public DialogLayer DialogLayer
    {
        get
        {
            var dialogLayer = FindLayer<DialogLayer>();
            if (dialogLayer == null)
            {
                dialogLayer = new DialogLayer();
                this.AddLayer(dialogLayer, DialogZIndex);
            }
            return dialogLayer;
        }
    }

    private TopLevel? _topLevel;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _topLevel = TopLevel.GetTopLevel(this);
        _topLevel?.SetValue(InstanceProperty, this);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_topLevel != null && _topLevel.GetValue(InstanceProperty) == this)
        {
            _topLevel.SetValue(InstanceProperty, null);
        }
        _topLevel = null;
        base.OnDetachedFromVisualTree(e);
    }
}
