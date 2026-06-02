using System.Reflection;

namespace AtomUI;

public static class AtomUIRuntimeInfo
{
    public static string GetAvaloniaVersion() => GetAssemblyMetadata("AvaloniaVersion");
    public static string GetAtomUIVersion() => GetAssemblyMetadata("AtomUIVersion");
    
    private static string GetAssemblyMetadata(string key)
    {
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var attribute in assembly.GetCustomAttributes<AssemblyMetadataAttribute>())
        {
            if (attribute.Key == key)
            {
                return attribute.Value ?? "Unknown";
            }
        }

        return "Unknown";
    }
}
