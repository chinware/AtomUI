using System.Reflection;

namespace AtomUI;

public static class AtomUIRuntimeInfo
{
    public static string GetAvaloniaVersion() => GetAssemblyMetadata("AvaloniaVersion");
    public static string GetAtomUIVersion() => GetAssemblyMetadata("AtomUIVersion");
    
    private static string GetAssemblyMetadata(string key)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var attribute = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                                .FirstOrDefault(attr => attr.Key == key);
        
        return attribute?.Value ?? "Unknown";
    }
}