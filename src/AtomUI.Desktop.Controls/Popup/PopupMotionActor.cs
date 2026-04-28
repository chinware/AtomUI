using AtomUI.Controls.Primitives;
using Avalonia.LogicalTree;

namespace AtomUI.Desktop.Controls;

internal class PopupMotionActor : MotionActor
{
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        var popup = this.FindLogicalAncestorOfType<Popup>();
        if (popup != null)
        {
            popup.NotifyMotionActorReady(this);
        }
    }
}