using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace AtomUI.Generator.LinkedRegistration;

internal static class LinkedCSharpUsageCandidateProvider
{
    internal static IncrementalValuesProvider<LinkedCSharpUsageCandidate> Create(
        IncrementalGeneratorInitializationContext context)
    {
        return context.SyntaxProvider.CreateSyntaxProvider(
                static (node, _) => IsCandidate(node),
                static (syntaxContext, cancellationToken) => Transform(
                    syntaxContext,
                    cancellationToken))
            .SelectMany(static (candidates, _) => candidates);
    }

    private static bool IsCandidate(SyntaxNode node)
    {
        return node is ObjectCreationExpressionSyntax or
               ImplicitObjectCreationExpressionSyntax or
               TypeOfExpressionSyntax or
               InvocationExpressionSyntax or
               TypeDeclarationSyntax;
    }

    private static ImmutableArray<LinkedCSharpUsageCandidate> Transform(
        GeneratorSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var builder = ImmutableArray.CreateBuilder<LinkedCSharpUsageCandidate>();
        switch (context.Node)
        {
            case ObjectCreationExpressionSyntax objectCreation
                when context.SemanticModel.GetOperation(objectCreation, cancellationToken) is
                    IObjectCreationOperation operation:
                AddType(builder, operation.Type, objectCreation.GetLocation());
                AddCall(builder, operation.Constructor, objectCreation.GetLocation());
                break;
            case ImplicitObjectCreationExpressionSyntax implicitCreation
                when context.SemanticModel.GetOperation(implicitCreation, cancellationToken) is
                    IObjectCreationOperation operation:
                AddType(builder, operation.Type, implicitCreation.GetLocation());
                AddCall(builder, operation.Constructor, implicitCreation.GetLocation());
                break;
            case TypeOfExpressionSyntax typeOfExpression
                when context.SemanticModel.GetOperation(typeOfExpression, cancellationToken) is
                    ITypeOfOperation operation:
                AddType(builder, operation.TypeOperand, typeOfExpression.GetLocation());
                break;
            case InvocationExpressionSyntax invocation
                when context.SemanticModel.GetOperation(invocation, cancellationToken) is
                    IInvocationOperation operation:
                CollectInvocation(
                    builder,
                    operation,
                    context.SemanticModel,
                    cancellationToken);
                break;
            case InvocationExpressionSyntax invocation
                when context.SemanticModel.GetOperation(invocation, cancellationToken) is
                    IDynamicInvocationOperation:
                builder.Add(LinkedCSharpUsageCandidate.Create(
                    LinkedCSharpUsageCandidateKind.Dynamic,
                    "dynamic invocation",
                    invocation.GetLocation()));
                break;
            case TypeDeclarationSyntax declaration
                when context.SemanticModel.GetDeclaredSymbol(declaration, cancellationToken) is
                    INamedTypeSymbol type:
                AddType(builder, type.BaseType, declaration.Identifier.GetLocation());
                break;
        }
        return builder.ToImmutable();
    }

    private static void CollectInvocation(
        ImmutableArray<LinkedCSharpUsageCandidate>.Builder builder,
        IInvocationOperation invocation,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var target = invocation.TargetMethod;
        AddCall(builder, target, invocation.Syntax.GetLocation());
        AddControlResultType(builder, target, invocation.Syntax.GetLocation());
        target = target.OriginalDefinition;
        if (!IsDynamicCreationMethod(target))
        {
            return;
        }

        if (invocation.TargetMethod.IsGenericMethod &&
            invocation.TargetMethod.TypeArguments.Length != 0 &&
            invocation.TargetMethod.TypeArguments.All(static type => !ContainsTypeParameter(type)))
        {
            foreach (var typeArgument in invocation.TargetMethod.TypeArguments)
            {
                AddType(builder, typeArgument, invocation.Syntax.GetLocation());
            }
            return;
        }

        var resolvedType = ResolveRuntimeTypeValue(
            invocation.Arguments.FirstOrDefault()?.Value,
            semanticModel,
            new HashSet<ISymbol>(SymbolEqualityComparer.Default),
            depth: 0,
            cancellationToken);
        if (resolvedType is not null && !ContainsTypeParameter(resolvedType))
        {
            AddType(builder, resolvedType, invocation.Syntax.GetLocation());
            return;
        }

        builder.Add(LinkedCSharpUsageCandidate.Create(
            LinkedCSharpUsageCandidateKind.Dynamic,
            LinkedRegistrationSymbolName.GetMethodMetadataName(target),
            invocation.Syntax.GetLocation()));
    }

    private static void AddControlResultType(
        ImmutableArray<LinkedCSharpUsageCandidate>.Builder builder,
        IMethodSymbol method,
        Location location)
    {
        var type = method.OriginalDefinition.ReturnType;
        if (type is INamedTypeSymbol named && IsControlType(named))
        {
            builder.Add(LinkedCSharpUsageCandidate.Create(
                LinkedCSharpUsageCandidateKind.Type,
                LinkedRegistrationSymbolName.GetTypeMetadataName(named.OriginalDefinition),
                location));
        }
    }

    private static ITypeSymbol? ResolveRuntimeTypeValue(
        IOperation? operation,
        SemanticModel semanticModel,
        ISet<ISymbol> visitedSymbols,
        int depth,
        CancellationToken cancellationToken)
    {
        if (depth >= 8)
        {
            return null;
        }
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }
        if (operation is ITypeOfOperation typeOfOperation)
        {
            return typeOfOperation.TypeOperand;
        }
        if (operation is not ILocalReferenceOperation localReference ||
            !visitedSymbols.Add(localReference.Local))
        {
            return null;
        }

        foreach (var syntaxReference in localReference.Local.DeclaringSyntaxReferences)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var syntax = syntaxReference.GetSyntax(cancellationToken);
            if (!ReferenceEquals(syntax.SyntaxTree, semanticModel.SyntaxTree) ||
                semanticModel.GetOperation(syntax, cancellationToken) is not
                    IVariableDeclaratorOperation declarator ||
                declarator.Initializer is null)
            {
                continue;
            }
            var resolved = ResolveRuntimeTypeValue(
                declarator.Initializer.Value,
                semanticModel,
                visitedSymbols,
                depth + 1,
                cancellationToken);
            if (resolved is not null)
            {
                return resolved;
            }
        }
        return null;
    }

    private static void AddType(
        ImmutableArray<LinkedCSharpUsageCandidate>.Builder builder,
        ITypeSymbol? type,
        Location location)
    {
        switch (type)
        {
            case IArrayTypeSymbol array:
                AddType(builder, array.ElementType, location);
                break;
            case IPointerTypeSymbol pointer:
                AddType(builder, pointer.PointedAtType, location);
                break;
            case INamedTypeSymbol named:
                builder.Add(LinkedCSharpUsageCandidate.Create(
                    LinkedCSharpUsageCandidateKind.Type,
                    LinkedRegistrationSymbolName.GetTypeMetadataName(named.OriginalDefinition),
                    location));
                foreach (var typeArgument in named.TypeArguments)
                {
                    AddType(builder, typeArgument, location);
                }
                break;
        }
    }

    private static void AddCall(
        ImmutableArray<LinkedCSharpUsageCandidate>.Builder builder,
        IMethodSymbol? method,
        Location location)
    {
        if (method is null)
        {
            return;
        }
        builder.Add(LinkedCSharpUsageCandidate.Create(
            LinkedCSharpUsageCandidateKind.Call,
            LinkedRegistrationSymbolName.GetMethodMetadataName(method.OriginalDefinition),
            location,
            LinkedRegistrationSymbolName.GetTypeMetadataName(
                method.ContainingType.OriginalDefinition),
            method.ContainingType.TypeKind == TypeKind.Interface,
            method.ContainingType.TypeKind == TypeKind.Delegate ||
            method.MethodKind == MethodKind.DelegateInvoke));
    }

    private static bool IsDynamicCreationMethod(IMethodSymbol method)
    {
        var containingType = LinkedRegistrationSymbolName.GetTypeMetadataName(
            method.ContainingType.OriginalDefinition);
        return (containingType == "System.Activator" && method.Name == "CreateInstance") ||
               (containingType == "System.Runtime.CompilerServices.RuntimeHelpers" &&
                method.Name == "GetUninitializedObject") ||
               (containingType == "System.Runtime.Serialization.FormatterServices" &&
                method.Name == "GetUninitializedObject");
    }

    private static bool IsControlType(INamedTypeSymbol type)
    {
        for (var current = type; current is not null; current = current.BaseType)
        {
            if (LinkedRegistrationSymbolName.GetTypeMetadataName(current.OriginalDefinition) ==
                "Avalonia.Controls.Control")
            {
                return true;
            }
        }
        return false;
    }

    private static bool ContainsTypeParameter(ITypeSymbol type)
    {
        return type switch
        {
            ITypeParameterSymbol => true,
            IArrayTypeSymbol array => ContainsTypeParameter(array.ElementType),
            IPointerTypeSymbol pointer => ContainsTypeParameter(pointer.PointedAtType),
            INamedTypeSymbol named => named.TypeArguments.Any(ContainsTypeParameter) ||
                                     (named.ContainingType is not null &&
                                      ContainsTypeParameter(named.ContainingType)),
            _ => false
        };
    }
}

internal enum LinkedCSharpUsageCandidateKind
{
    Type,
    Call,
    Dynamic
}

internal sealed class LinkedCSharpUsageCandidate
{
    private LinkedCSharpUsageCandidate(
        LinkedCSharpUsageCandidateKind kind,
        string identity,
        string containingTypeIdentity,
        string source,
        int spanStart,
        int spanLength,
        int line,
        int column,
        bool isInterfaceDispatch,
        bool isOpenDispatch)
    {
        Kind = kind;
        Identity = identity;
        ContainingTypeIdentity = containingTypeIdentity;
        Source = source;
        SpanStart = spanStart;
        SpanLength = spanLength;
        Line = line;
        Column = column;
        IsInterfaceDispatch = isInterfaceDispatch;
        IsOpenDispatch = isOpenDispatch;
    }

    internal LinkedCSharpUsageCandidateKind Kind { get; }
    internal string Identity { get; }
    internal string ContainingTypeIdentity { get; }
    internal string Source { get; }
    internal int SpanStart { get; }
    internal int SpanLength { get; }
    internal int Line { get; }
    internal int Column { get; }
    internal bool IsInterfaceDispatch { get; }
    internal bool IsOpenDispatch { get; }

    internal static LinkedCSharpUsageCandidate Create(
        LinkedCSharpUsageCandidateKind kind,
        string identity,
        Location location,
        string containingTypeIdentity = "",
        bool isInterfaceDispatch = false,
        bool isOpenDispatch = false)
    {
        var lineSpan = location.GetLineSpan();
        var source = lineSpan.Path.Length != 0
            ? lineSpan.Path
            : location.SourceTree?.FilePath ?? string.Empty;
        return new LinkedCSharpUsageCandidate(
            kind,
            identity,
            containingTypeIdentity,
            source,
            location.SourceSpan.Start,
            location.SourceSpan.Length,
            lineSpan.StartLinePosition.Line + 1,
            lineSpan.StartLinePosition.Character + 1,
            isInterfaceDispatch,
            isOpenDispatch);
    }

    internal Location CreateLocation()
    {
        var start = new LinePosition(Math.Max(0, Line - 1), Math.Max(0, Column - 1));
        return Location.Create(
            Source.Length == 0 ? "<Unknown>" : Source,
            new Microsoft.CodeAnalysis.Text.TextSpan(SpanStart, SpanLength),
            new LinePositionSpan(start, start));
    }
}
