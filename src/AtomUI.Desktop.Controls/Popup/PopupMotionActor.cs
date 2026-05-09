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
            // 先把透明度预置为 0，避免 popup host Show() 之后、HandlePopupOpened
            // 再把 Opacity 改为 0 之间出现一帧的全不透明闪烁。
            if (popup.IsMotionEnabled && popup.OpenMotion is not null)
            {
                Opacity = 0.0d;
            }
            popup.NotifyMotionActorReady(this);
        }
    }
}
