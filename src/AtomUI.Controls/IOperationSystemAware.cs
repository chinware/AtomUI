using Avalonia;

namespace AtomUI.Controls;

internal interface IOperationSystemAware
{
    OperationSystemType OperationSystemType { get; }
    void SetOperationSystemType(OperationSystemType operationSystemType);
}

internal abstract class OperationSystemAwareControlProperty : AvaloniaObject
{
    public const string OperationSystemPropertyName = "OperationSystemType";
    public static readonly StyledProperty<OperationSystemType> OperationSystemTypeProperty =
        AvaloniaProperty.Register<OperationSystemAwareControlProperty, OperationSystemType>(OperationSystemPropertyName, OperationSystemType.Unknown);
}

internal static class OperationSystemAwareExtensions
{
    public static void ConfigureOperationSystemType(this IOperationSystemAware operationSystemAware)
    {
        if (OperatingSystem.IsWindows())
        {
            operationSystemAware.SetOperationSystemType(OperationSystemType.Windows);
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            operationSystemAware.SetOperationSystemType(OperationSystemType.macOS);
        }
        else if (OperatingSystem.IsLinux())
        {
            operationSystemAware.SetOperationSystemType(OperationSystemType.Linux);
        }
        else
        {
            operationSystemAware.SetOperationSystemType(OperationSystemType.Unknown);
        }
    }
}