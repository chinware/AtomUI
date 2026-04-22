using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal interface IShadowAwareLayer
{ 
    BoxShadows MaskShadows { get; set; }
    void NotifyOpenMotionAboutToStart();
    void NotifyOpenMotionCompleted();
    
    void NotifyCloseMotionAboutToStart();
    void NotifyCloseMotionCompleted();
    
    Task RunOpenMotionAsync(Action? aboutToStart = null);
    Task RunCloseMotionAsync(Action? aboutToStart = null);
}