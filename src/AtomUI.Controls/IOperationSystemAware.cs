using Avalonia;

namespace AtomUI.Controls;

public interface IOperationSystemAware
{
    OperationSystemType OperationSystemType { get; }
}

public abstract class OperationSystemAwareControlProperty : AvaloniaObject
{
    public const string OperationSystemPropertyName = "OperationSystemType";
    public static readonly StyledProperty<OperationSystemType> OperationSystemTypeProperty =
        AvaloniaProperty.Register<OperationSystemAwareControlProperty, OperationSystemType>(OperationSystemPropertyName, OperationSystemType.Unknown);
}
