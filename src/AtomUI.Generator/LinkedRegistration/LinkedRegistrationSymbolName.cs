using Microsoft.CodeAnalysis;

namespace AtomUI.Generator.LinkedRegistration;

internal static class LinkedRegistrationSymbolName
{
    internal static string GetMethodMetadataName(IMethodSymbol method)
    {
        return GetTypeMetadataName(method.ContainingType.OriginalDefinition) + "." + method.Name;
    }

    internal static string GetTypeMetadataName(INamedTypeSymbol type)
    {
        var names = new Stack<string>();
        for (var current = type; current is not null; current = current.ContainingType)
        {
            names.Push(current.MetadataName);
        }
        var typeName = string.Join("+", names);
        return type.ContainingNamespace is { IsGlobalNamespace: false } namespaceSymbol
            ? namespaceSymbol.ToDisplayString() + "." + typeName
            : typeName;
    }
}
