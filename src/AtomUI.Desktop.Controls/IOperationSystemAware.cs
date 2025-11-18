using Avalonia;

namespace AtomUI.Desktop.Controls;

internal interface IOperationSystemAware
{
    OsType OsType { get; }
    void SetOsType(OsType osType);
    
    Version OsVersion { get; }
    void SetOsVersion(Version version);
}

internal abstract class OperationSystemAwareControlProperty : AvaloniaObject
{
    public const string OsTypePropertyName = "OsType";
    public const string OsVersionPropertyName = "OsVersion";
    public static readonly StyledProperty<OsType> OsTypeProperty =
        AvaloniaProperty.Register<OperationSystemAwareControlProperty, OsType>(OsTypePropertyName, OsType.Unknown);
    public static readonly StyledProperty<Version> OsVersionProperty =
        AvaloniaProperty.Register<OperationSystemAwareControlProperty, Version>(OsVersionPropertyName);
}

internal static class OperationSystemAwareExtensions
{
    public static void ConfigureOsType(this IOperationSystemAware operationSystemAware)
    {
        if (OperatingSystem.IsWindows())
        {
            operationSystemAware.SetOsType(OsType.Windows);
        }
        else if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            operationSystemAware.SetOsType(OsType.macOS);
        }
        else if (OperatingSystem.IsLinux())
        {
            operationSystemAware.SetOsType(OsType.Linux);
        }
        else
        {
            operationSystemAware.SetOsType(OsType.Unknown);
        }
        operationSystemAware.SetOsVersion(Environment.OSVersion.Version);
    }
}