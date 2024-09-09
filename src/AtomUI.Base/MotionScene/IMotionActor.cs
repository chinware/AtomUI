using Avalonia.Controls;

namespace AtomUI.MotionScene;

internal interface IMotionActor
{
    public bool CompletedStatus { get; }

    public Control MotionTarget { get; set; }
    public IMotion Motion { get; }
    public bool DispatchInSceneLayer { get; set; }
    public event EventHandler? PreStart;
    public event EventHandler? Started;
    public event EventHandler? Completed;
}