using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.MotionScene;

public interface IMotionActor
{
    RelativePoint RenderTransformOrigin {  get; set; }
    ITransform? MotionTransform { get; set; }
    TransformOperations? MotionTransformOperations  { get; set; }
    double Opacity { get; set; }

    void NotifyMotionPreStart();
    void NotifyMotionCompleted();
}

public abstract class MotionActorControlProperty : AvaloniaObject
{
    public const string MotionTransformPropertyName = "MotionTransform";
    public const string MotionTransformOperationsPropertyName = "MotionTransformOperations";
    public const string RenderTransformOriginPropertyName = "RenderTransformOrigin";
    
    public static readonly StyledProperty<ITransform?> MotionTransformProperty =
        AvaloniaProperty.Register<MotionActorControlProperty, ITransform?>(MotionTransformPropertyName);
    
    public static readonly StyledProperty<TransformOperations?> MotionTransformOperationsProperty =
        AvaloniaProperty.Register<MotionActorControlProperty, TransformOperations?>(nameof(MotionTransformOperationsPropertyName));
}
