using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AtomUI.Generator;

internal static class ControlTokenCalculationValidator
{
    private const string MethodName = "CalculateTokenValues";

    internal static IReadOnlyList<Diagnostic> Validate(
        IReadOnlyList<ControlTokenLayerInfo> layers,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        var diagnostics = new List<Diagnostic>();
        for (var layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var layer = layers[layerIndex];
            var method = FindDeclaredCalculationMethod(layer.Symbol);
            if (method is null)
            {
                continue;
            }

            if (!method.IsOverride)
            {
                diagnostics.Add(CreateDiagnostic(
                    GetMethodLocation(method, cancellationToken),
                    layer.Symbol,
                    "must be an override"));
                continue;
            }

            var syntax = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(cancellationToken);
            if (syntax is not MethodDeclarationSyntax methodSyntax)
            {
                continue;
            }

            // The first marked layer may override the root's intentionally empty implementation.
            if (layerIndex == 0)
            {
                continue;
            }

            var reason = ValidateLaterOverride(method, methodSyntax, compilation, cancellationToken);
            if (reason is not null)
            {
                diagnostics.Add(CreateDiagnostic(methodSyntax.Identifier.GetLocation(), layer.Symbol, reason));
            }
        }

        return diagnostics;
    }

    private static IMethodSymbol? FindDeclaredCalculationMethod(INamedTypeSymbol layer)
    {
        return layer.GetMembers(MethodName)
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault(static method =>
                        !method.IsStatic &&
                        method.Arity == 0 &&
                        method.ReturnsVoid &&
                        method.Parameters.Length == 1 &&
                        method.Parameters[0].RefKind == RefKind.None &&
                        method.Parameters[0].Type.SpecialType == SpecialType.System_Boolean);
    }

    private static string? ValidateLaterOverride(
        IMethodSymbol method,
        MethodDeclarationSyntax methodSyntax,
        Compilation compilation,
        CancellationToken cancellationToken)
    {
        if (methodSyntax.Body is null)
        {
            return "expression body is not allowed; use a block body with the base call first";
        }

        var semanticModel = compilation.GetSemanticModel(methodSyntax.SyntaxTree);
        var overriddenMethod = method.OverriddenMethod;
        var baseCalls = methodSyntax.Body.DescendantNodes()
                                    .OfType<InvocationExpressionSyntax>()
                                    .Where(IsSyntacticBaseCall)
                                    .Where(call => IsDirectOverriddenMethodCall(
                                        call,
                                        overriddenMethod,
                                        semanticModel,
                                        cancellationToken))
                                    .ToArray();
        if (baseCalls.Length == 0)
        {
            return "missing base.CalculateTokenValues call";
        }
        if (baseCalls.Any(static call => call.Ancestors().Any(static ancestor =>
                ancestor is LocalFunctionStatementSyntax or AnonymousFunctionExpressionSyntax)))
        {
            return "nested local function or lambda cannot contain the base call";
        }
        if (baseCalls.Length != 1)
        {
            return "duplicate base.CalculateTokenValues calls; call it exactly once";
        }

        var baseCall = baseCalls[0];
        if (methodSyntax.Body.Statements.FirstOrDefault() is not ExpressionStatementSyntax firstStatement ||
            firstStatement.Expression.Span != baseCall.Span)
        {
            if (baseCall.Ancestors().Any(static ancestor =>
                    ancestor is IfStatementSyntax or SwitchStatementSyntax or
                        ForStatementSyntax or ForEachStatementSyntax or WhileStatementSyntax or
                        DoStatementSyntax or TryStatementSyntax or CatchClauseSyntax))
            {
                return "conditional or nested base call is not allowed";
            }

            return "base.CalculateTokenValues call is not first";
        }

        var target = semanticModel.GetSymbolInfo(baseCall, cancellationToken).Symbol as IMethodSymbol;
        if (overriddenMethod is null ||
            target is null ||
            !SymbolEqualityComparer.Default.Equals(
                target.OriginalDefinition,
                overriddenMethod.OriginalDefinition))
        {
            return "base call must target the inherited CalculateTokenValues implementation";
        }

        if (baseCall.ArgumentList.Arguments.Count != 1 ||
            baseCall.ArgumentList.Arguments[0].Expression is not IdentifierNameSyntax argument ||
            !SymbolEqualityComparer.Default.Equals(
                semanticModel.GetSymbolInfo(argument, cancellationToken).Symbol,
                method.Parameters[0]))
        {
            return "wrong argument; forward the current bool parameter unchanged";
        }

        return null;
    }

    private static bool IsDirectOverriddenMethodCall(
        InvocationExpressionSyntax call,
        IMethodSymbol? overriddenMethod,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        var target = semanticModel.GetSymbolInfo(call, cancellationToken).Symbol as IMethodSymbol;
        return overriddenMethod is not null &&
               target is not null &&
               SymbolEqualityComparer.Default.Equals(
                   target.OriginalDefinition,
                   overriddenMethod.OriginalDefinition);
    }

    private static bool IsSyntacticBaseCall(InvocationExpressionSyntax invocation)
    {
        return invocation.Expression is MemberAccessExpressionSyntax
        {
            Expression: BaseExpressionSyntax,
            Name.Identifier.ValueText: MethodName
        };
    }

    private static Location? GetMethodLocation(
        IMethodSymbol method,
        CancellationToken cancellationToken)
    {
        var syntax = method.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(cancellationToken);
        return syntax is MethodDeclarationSyntax declaration
            ? declaration.Identifier.GetLocation()
            : method.Locations.FirstOrDefault(static location => location.IsInSource);
    }

    private static Diagnostic CreateDiagnostic(
        Location? location,
        INamedTypeSymbol layer,
        string reason)
    {
        return Diagnostic.Create(
            AtomUIDiagnosticDescriptors.ControlTokenInvalidCalculationChain,
            location,
            layer.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
            reason);
    }
}
