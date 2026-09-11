using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator.Localization;

internal sealed class ApplicationLanguageHostInfo
{
    private const string AvaloniaApplicationMetadataName = "Avalonia.Application";

    private ApplicationLanguageHostInfo(
        string metadataName,
        string @namespace,
        string typeName,
        string accessibility,
        bool isAbstract,
        bool isGeneric,
        bool isTopLevel,
        bool isPartial,
        Location location)
    {
        MetadataName = metadataName;
        Namespace = @namespace;
        TypeName = typeName;
        AccessibilityModifier = accessibility;
        IsAbstract = isAbstract;
        IsGeneric = isGeneric;
        IsTopLevel = isTopLevel;
        IsPartial = isPartial;
        Location = location;
    }

    internal string MetadataName { get; }

    internal string Namespace { get; }

    internal string TypeName { get; }

    internal string AccessibilityModifier { get; }

    internal bool IsAbstract { get; }

    internal bool IsGeneric { get; }

    internal bool IsTopLevel { get; }

    internal bool IsPartial { get; }

    internal Location Location { get; }

    internal static ApplicationLanguageHostInfo? TryCreate(
        GeneratorSyntaxContext context,
        CancellationToken cancellationToken)
    {
        if (context.Node is not ClassDeclarationSyntax declaration ||
            context.SemanticModel.GetDeclaredSymbol(declaration, cancellationToken) is not INamedTypeSymbol symbol ||
            !InheritsFromAvaloniaApplication(symbol))
        {
            return null;
        }

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : symbol.ContainingNamespace.ToDisplayString();
        return new ApplicationLanguageHostInfo(
            GetMetadataName(symbol),
            namespaceName,
            ToIdentifier(symbol.Name),
            GetAccessibility(symbol.DeclaredAccessibility),
            symbol.IsAbstract,
            symbol.IsGenericType,
            symbol.ContainingType is null,
            HasPartialDeclarations(symbol, cancellationToken),
            declaration.Identifier.GetLocation());
    }

    private static bool InheritsFromAvaloniaApplication(INamedTypeSymbol symbol)
    {
        for (var current = symbol.BaseType; current is not null; current = current.BaseType)
        {
            if (GetMetadataName(current) == AvaloniaApplicationMetadataName)
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasPartialDeclarations(
        INamedTypeSymbol symbol,
        CancellationToken cancellationToken)
    {
        return symbol.DeclaringSyntaxReferences
                     .Select(reference => reference.GetSyntax(cancellationToken))
                     .OfType<ClassDeclarationSyntax>()
                     .All(static declaration => declaration.Modifiers.Any(SyntaxKind.PartialKeyword));
    }

    private static string GetMetadataName(INamedTypeSymbol symbol)
    {
        var typeNames = new Stack<string>();
        for (var current = symbol; current is not null; current = current.ContainingType)
        {
            typeNames.Push(current.MetadataName);
        }

        var typeName = string.Join("+", typeNames);
        return symbol.ContainingNamespace.IsGlobalNamespace
            ? typeName
            : $"{symbol.ContainingNamespace.ToDisplayString()}.{typeName}";
    }

    private static string GetAccessibility(Microsoft.CodeAnalysis.Accessibility accessibility)
    {
        return accessibility switch
        {
            Microsoft.CodeAnalysis.Accessibility.Public => "public",
            Microsoft.CodeAnalysis.Accessibility.Internal => "internal",
            _ => string.Empty
        };
    }

    private static string ToIdentifier(string value)
    {
        return SyntaxFacts.GetKeywordKind(value) == SyntaxKind.None ? value : "@" + value;
    }
}
