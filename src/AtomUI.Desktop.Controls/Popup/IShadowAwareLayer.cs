using Avalonia.Media;

namespace AtomUI.Controls;

internal interface IShadowAwareLayer
{ 
    BoxShadows MaskShadows { get; set; }
    void NotifyOpenMotionAboutToStart();
    void NotifyOpenMotionCompleted();
    
    void NotifyCloseMotionAboutToStart();
    void NotifyCloseMotionCompleted();
    
    void RunOpenMotion(Action? aboutToStart = null, Action? completedAction = null);
    void RunCloseMotion(Action? aboutToStart = null, Action? completedAction = null);
}