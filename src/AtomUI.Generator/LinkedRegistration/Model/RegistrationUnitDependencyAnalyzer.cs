using AtomUI.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace AtomUI.Generator.LinkedRegistration.Model;

internal static class RegistrationUnitDependencyAnalyzer
{
    internal static RegistrationUnitDependencyAnalysis Analyze(
        Compilation compilation,
        IReadOnlyList<ControlThemeInfo> controls,
        string packageId,
        RegistrationUnitGranularity granularity,
        string? projectDirectory,
        AnalyzerConfigOptionsProvider optionsProvider,
        CancellationToken cancellationToken)
    {
        if (granularity != RegistrationUnitGranularity.Directory)
        {
            return RegistrationUnitDependencyAnalysis.Empty;
        }

        var analyzer = new Analyzer(
            compilation,
            controls,
            packageId,
            projectDirectory,
            optionsProvider);
        return analyzer.Analyze(cancellationToken);
    }

    private sealed class Analyzer
    {
        private readonly Compilation _compilation;
        private readonly string _packageId;
        private readonly string? _projectDirectory;
        private readonly AnalyzerConfigOptionsProvider _optionsProvider;
        private readonly HashSet<string> _knownUnitIds;
        private readonly Dictionary<INamedTypeSymbol, string?> _unitByType =
            new(SymbolEqualityComparer.Default);
        private readonly Dictionary<string, HashSet<string>> _dependencies =
            new(StringComparer.Ordinal);
        private readonly Dictionary<string, RegistrationUnitDependencyIssue> _issues =
            new(StringComparer.Ordinal);
        private readonly Dictionary<string, HashSet<string>> _visitedMembersByUnit =
            new(StringComparer.Ordinal);

        internal Analyzer(
            Compilation compilation,
            IReadOnlyList<ControlThemeInfo> controls,
            string packageId,
            string? projectDirectory,
            AnalyzerConfigOptionsProvider optionsProvider)
        {
            _compilation = compilation;
            _packageId = packageId;
            _projectDirectory = projectDirectory;
            _optionsProvider = optionsProvider;
            _knownUnitIds = new HashSet<string>(
                controls.Select(static control => control.UnitId),
                StringComparer.Ordinal);
        }

        internal RegistrationUnitDependencyAnalysis Analyze(CancellationToken cancellationToken)
        {
            foreach (var type in GetTypes(_compilation.Assembly.GlobalNamespace))
            {
                cancellationToken.ThrowIfCancellationRequested();
                CollectSymbolDependencies(type);
            }

            foreach (var tree in _compilation.SyntaxTrees.OrderBy(
                         static tree => tree.FilePath,
                         StringComparer.Ordinal))
            {
                cancellationToken.ThrowIfCancellationRequested();
                CollectSyntaxDependencies(tree, cancellationToken);
            }

            return new RegistrationUnitDependencyAnalysis(
                _dependencies.OrderBy(static pair => pair.Key, StringComparer.Ordinal)
                             .ToDictionary(
                                 static pair => pair.Key,
                                 static pair => (IReadOnlyList<string>)pair.Value
                                     .OrderBy(static dependency => dependency, StringComparer.Ordinal)
                                     .ToArray(),
                                 StringComparer.Ordinal),
                _issues.Values.OrderBy(static issue => issue.Source, StringComparer.Ordinal)
                       .ThenBy(static issue => issue.Line)
                       .ThenBy(static issue => issue.Column)
                       .ThenBy(static issue => issue.Identity, StringComparer.Ordinal)
                       .ToArray());
        }

        private void CollectSymbolDependencies(INamedTypeSymbol type)
        {
            var ownerUnitId = GetUnitId(type);
            if (ownerUnitId is not null)
            {
                AddTypeDependency(ownerUnitId, type.BaseType, GetSourceLocation(type), null);
            }

            foreach (var nested in type.GetTypeMembers())
            {
                CollectSymbolDependencies(nested);
            }
        }

        private void CollectSyntaxDependencies(
            SyntaxTree tree,
            CancellationToken cancellationToken)
        {
            var model = _compilation.GetSemanticModel(tree);
            foreach (var node in tree.GetRoot(cancellationToken).DescendantNodes())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var ownerUnitId = GetOwningUnitId(model, node.SpanStart);
                if (ownerUnitId is null)
                {
                    continue;
                }

                CollectSyntaxNodeDependencies(
                    ownerUnitId,
                    model,
                    node,
                    substitutions: null,
                    cancellationToken);
            }
        }

        private void CollectSyntaxNodeDependencies(
            string ownerUnitId,
            SemanticModel model,
            SyntaxNode node,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions,
            CancellationToken cancellationToken)
        {
            switch (node)
            {
                case ObjectCreationExpressionSyntax objectCreation
                    when model.GetOperation(objectCreation, cancellationToken) is
                        IObjectCreationOperation operation:
                    AddTypeDependency(
                        ownerUnitId,
                        operation.Type,
                        operation.Syntax.GetLocation(),
                        substitutions);
                    CollectInvokedMethodDependencies(
                        ownerUnitId,
                        operation.Constructor,
                        operation.Syntax.GetLocation(),
                        substitutions,
                        cancellationToken);
                    break;
                case ImplicitObjectCreationExpressionSyntax implicitCreation
                    when model.GetOperation(implicitCreation, cancellationToken) is
                        IObjectCreationOperation operation:
                    AddTypeDependency(
                        ownerUnitId,
                        operation.Type,
                        operation.Syntax.GetLocation(),
                        substitutions);
                    CollectInvokedMethodDependencies(
                        ownerUnitId,
                        operation.Constructor,
                        operation.Syntax.GetLocation(),
                        substitutions,
                        cancellationToken);
                    break;
                case TypeOfExpressionSyntax typeOfExpression
                    when model.GetOperation(typeOfExpression, cancellationToken) is
                        ITypeOfOperation operation:
                    AddTypeDependency(
                        ownerUnitId,
                        operation.TypeOperand,
                        operation.Syntax.GetLocation(),
                        substitutions);
                    break;
                case InvocationExpressionSyntax invocation
                    when model.GetOperation(invocation, cancellationToken) is
                        IInvocationOperation operation:
                    if (!CollectDynamicCreation(ownerUnitId, operation, substitutions))
                    {
                        CollectInvokedMethodDependencies(
                            ownerUnitId,
                            operation.TargetMethod,
                            operation.Syntax.GetLocation(),
                            substitutions,
                            cancellationToken);
                    }
                    break;
                case InvocationExpressionSyntax invocation
                    when model.GetOperation(invocation, cancellationToken) is
                        IDynamicInvocationOperation operation:
                    AddIssue(operation.Syntax.GetLocation(), "dynamic invocation");
                    break;
                case MemberAccessExpressionSyntax memberAccess:
                    CollectReferenceOperationDependencies(
                        ownerUnitId,
                        model.GetOperation(memberAccess, cancellationToken),
                        memberAccess.GetLocation(),
                        substitutions,
                        cancellationToken);
                    break;
                case ElementAccessExpressionSyntax elementAccess:
                    CollectReferenceOperationDependencies(
                        ownerUnitId,
                        model.GetOperation(elementAccess, cancellationToken),
                        elementAccess.GetLocation(),
                        substitutions,
                        cancellationToken);
                    break;
                case IdentifierNameSyntax identifier
                    when identifier.Parent is not MemberAccessExpressionSyntax &&
                         identifier.Parent is not ElementAccessExpressionSyntax:
                    CollectReferenceOperationDependencies(
                        ownerUnitId,
                        model.GetOperation(identifier, cancellationToken),
                        identifier.GetLocation(),
                        substitutions,
                        cancellationToken);
                    break;
            }
        }

        private void CollectReferenceOperationDependencies(
            string ownerUnitId,
            IOperation? operation,
            Location location,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions,
            CancellationToken cancellationToken)
        {
            switch (operation)
            {
                case IPropertyReferenceOperation propertyReference:
                    CollectInvokedMethodDependencies(
                        ownerUnitId,
                        propertyReference.Property.GetMethod,
                        location,
                        substitutions,
                        cancellationToken);
                    CollectInvokedMethodDependencies(
                        ownerUnitId,
                        propertyReference.Property.SetMethod,
                        location,
                        substitutions,
                        cancellationToken);
                    break;
                case IFieldReferenceOperation fieldReference:
                    CollectFieldInitializerDependencies(
                        ownerUnitId,
                        fieldReference.Field,
                        substitutions,
                        cancellationToken);
                    break;
                case IMethodReferenceOperation methodReference:
                    CollectInvokedMethodDependencies(
                        ownerUnitId,
                        methodReference.Method,
                        location,
                        substitutions,
                        cancellationToken);
                    break;
            }
        }

        private bool CollectDynamicCreation(
            string ownerUnitId,
            IInvocationOperation invocation,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions)
        {
            var target = invocation.TargetMethod.OriginalDefinition;
            if (!IsDynamicCreationMethod(target))
            {
                return false;
            }

            if (invocation.TargetMethod.IsGenericMethod && invocation.TargetMethod.TypeArguments.Length != 0)
            {
                foreach (var typeArgument in invocation.TargetMethod.TypeArguments)
                {
                    AddTypeDependency(
                        ownerUnitId,
                        typeArgument,
                        invocation.Syntax.GetLocation(),
                        substitutions);
                }
                return true;
            }

            var resolvedType = ResolveRuntimeTypeValue(
                invocation.Arguments.FirstOrDefault()?.Value,
                new HashSet<ISymbol>(SymbolEqualityComparer.Default),
                substitutions);
            if (resolvedType is not null)
            {
                AddTypeDependency(
                    ownerUnitId,
                    resolvedType,
                    invocation.Syntax.GetLocation(),
                    substitutions);
                return true;
            }

            AddIssue(
                invocation.Syntax.GetLocation(),
                LinkedRegistrationSymbolName.GetMethodMetadataName(target));
            return true;
        }

        private void CollectInvokedMethodDependencies(
            string ownerUnitId,
            IMethodSymbol? method,
            Location location,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions,
            CancellationToken cancellationToken)
        {
            if (method is null)
            {
                return;
            }

            var definition = method.OriginalDefinition;
            if (!SymbolEqualityComparer.Default.Equals(
                    definition.ContainingAssembly,
                    _compilation.Assembly))
            {
                return;
            }

            var methodSubstitutions = CreateMethodSubstitutions(
                method,
                definition,
                substitutions);
            if (!TryVisitMember(ownerUnitId, definition, methodSubstitutions))
            {
                return;
            }

            var syntaxReferences = definition.DeclaringSyntaxReferences;
            foreach (var syntaxReference in syntaxReferences)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var syntax = syntaxReference.GetSyntax(cancellationToken);
                var model = _compilation.GetSemanticModel(syntax.SyntaxTree);
                foreach (var node in syntax.DescendantNodesAndSelf())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CollectSyntaxNodeDependencies(
                        ownerUnitId,
                        model,
                        node,
                        methodSubstitutions,
                        cancellationToken);
                }
            }
        }

        private void CollectFieldInitializerDependencies(
            string ownerUnitId,
            IFieldSymbol field,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions,
            CancellationToken cancellationToken)
        {
            var definition = field.OriginalDefinition;
            if (!SymbolEqualityComparer.Default.Equals(
                    definition.ContainingAssembly,
                    _compilation.Assembly) ||
                !TryVisitMember(ownerUnitId, definition, substitutions))
            {
                return;
            }

            foreach (var syntaxReference in definition.DeclaringSyntaxReferences)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var syntax = syntaxReference.GetSyntax(cancellationToken);
                var model = _compilation.GetSemanticModel(syntax.SyntaxTree);
                foreach (var node in syntax.DescendantNodesAndSelf())
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CollectSyntaxNodeDependencies(
                        ownerUnitId,
                        model,
                        node,
                        substitutions,
                        cancellationToken);
                }
            }
        }

        private bool TryVisitMember(
            string ownerUnitId,
            ISymbol member,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions)
        {
            if (!_visitedMembersByUnit.TryGetValue(ownerUnitId, out var visitedMembers))
            {
                visitedMembers = new HashSet<string>(StringComparer.Ordinal);
                _visitedMembersByUnit.Add(ownerUnitId, visitedMembers);
            }
            return visitedMembers.Add(CreateMemberVisitKey(member, substitutions));
        }

        private ITypeSymbol? ResolveRuntimeTypeValue(
            IOperation? operation,
            ISet<ISymbol> visitedSymbols,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions)
        {
            while (operation is IConversionOperation conversion)
            {
                operation = conversion.Operand;
            }
            if (operation is ITypeOfOperation typeOfOperation)
            {
                return ResolveSubstitution(typeOfOperation.TypeOperand, substitutions);
            }
            if (operation is not ILocalReferenceOperation localReference ||
                !visitedSymbols.Add(localReference.Local))
            {
                return null;
            }

            foreach (var syntaxReference in localReference.Local.DeclaringSyntaxReferences)
            {
                var syntax = syntaxReference.GetSyntax();
                var semanticModel = _compilation.GetSemanticModel(syntax.SyntaxTree);
                if (semanticModel.GetOperation(syntax) is IVariableDeclaratorOperation declarator &&
                    declarator.Initializer is not null)
                {
                    var resolved = ResolveRuntimeTypeValue(
                        declarator.Initializer.Value,
                        visitedSymbols,
                        substitutions);
                    if (resolved is not null)
                    {
                        return resolved;
                    }
                }
            }
            return null;
        }

        private void AddTypeDependency(
            string ownerUnitId,
            ITypeSymbol? type,
            Location? location,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions)
        {
            type = ResolveSubstitution(type, substitutions);
            switch (type)
            {
                case null:
                    return;
                case INamedTypeSymbol named:
                    var definition = named.OriginalDefinition;
                    if (SymbolEqualityComparer.Default.Equals(
                            definition.ContainingAssembly,
                            _compilation.Assembly) &&
                        IsControlType(definition))
                    {
                        var dependencyUnitId = GetUnitId(definition);
                        if (dependencyUnitId is null)
                        {
                            AddIssue(location, GetMetadataName(definition));
                        }
                        else if (!string.Equals(
                                     ownerUnitId,
                                     dependencyUnitId,
                                     StringComparison.Ordinal))
                        {
                            if (!_dependencies.TryGetValue(ownerUnitId, out var dependencies))
                            {
                                dependencies = new HashSet<string>(StringComparer.Ordinal);
                                _dependencies.Add(ownerUnitId, dependencies);
                            }
                            dependencies.Add(dependencyUnitId);
                        }
                    }

                    return;
            }
        }

        private static IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>?
            CreateMethodSubstitutions(
                IMethodSymbol method,
                IMethodSymbol definition,
                IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions)
        {
            Dictionary<ITypeParameterSymbol, ITypeSymbol>? result = null;
            if (substitutions is not null)
            {
                result = new Dictionary<ITypeParameterSymbol, ITypeSymbol>(
                    SymbolEqualityComparer.Default);
                foreach (var pair in substitutions)
                {
                    result.Add(pair.Key, pair.Value);
                }
            }

            AddTypeParameterSubstitutions(
                definition.ContainingType.TypeParameters,
                method.ContainingType.TypeArguments,
                substitutions,
                ref result);
            AddTypeParameterSubstitutions(
                definition.TypeParameters,
                method.TypeArguments,
                substitutions,
                ref result);
            return result;
        }

        private static void AddTypeParameterSubstitutions(
            IReadOnlyList<ITypeParameterSymbol> parameters,
            IReadOnlyList<ITypeSymbol> arguments,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions,
            ref Dictionary<ITypeParameterSymbol, ITypeSymbol>? result)
        {
            for (var index = 0; index < parameters.Count && index < arguments.Count; index++)
            {
                var argument = ResolveSubstitution(arguments[index], substitutions);
                if (argument is null)
                {
                    continue;
                }
                if (SymbolEqualityComparer.Default.Equals(parameters[index], argument))
                {
                    continue;
                }

                result ??= new Dictionary<ITypeParameterSymbol, ITypeSymbol>(
                    SymbolEqualityComparer.Default);
                result[parameters[index]] = argument;
            }
        }

        private static ITypeSymbol? ResolveSubstitution(
            ITypeSymbol? type,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions)
        {
            var visited = 0;
            while (type is ITypeParameterSymbol typeParameter &&
                   substitutions is not null &&
                   substitutions.TryGetValue(typeParameter, out var substitution) &&
                   visited++ < substitutions.Count)
            {
                type = substitution;
            }
            return type;
        }

        private static string CreateMemberVisitKey(
            ISymbol member,
            IReadOnlyDictionary<ITypeParameterSymbol, ITypeSymbol>? substitutions)
        {
            var key = member.GetDocumentationCommentId() ??
                      member.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            if (substitutions is null || substitutions.Count == 0)
            {
                return key;
            }

            return key + "|" + string.Join(
                ",",
                substitutions.OrderBy(
                                 static pair => pair.Key.ToDisplayString(
                                     SymbolDisplayFormat.FullyQualifiedFormat),
                                 StringComparer.Ordinal)
                             .Select(static pair =>
                                 pair.Key.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) +
                                 "=" +
                                 pair.Value.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));
        }

        private string? GetOwningUnitId(SemanticModel model, int position)
        {
            var symbol = model.GetEnclosingSymbol(position);
            var containingType = symbol as INamedTypeSymbol ?? symbol?.ContainingType;
            return containingType is null ? null : GetUnitId(containingType);
        }

        private string? GetUnitId(INamedTypeSymbol type)
        {
            if (_unitByType.TryGetValue(type, out var cached))
            {
                return cached;
            }

            var candidates = type.DeclaringSyntaxReferences
                                 .Select(reference => reference.SyntaxTree)
                                 .Select(tree => RegistrationUnitId.Create(
                                     _packageId,
                                     RegistrationUnitGranularity.Directory,
                                     tree.FilePath,
                                     _projectDirectory,
                                     type.Name,
                                     GetExplicitUnit(tree)))
                                 .Where(_knownUnitIds.Contains)
                                 .Distinct(StringComparer.Ordinal)
                                 .OrderBy(static unitId => unitId, StringComparer.Ordinal)
                                 .ToArray();
            var unitId = candidates.Length == 1 ? candidates[0] : null;
            _unitByType.Add(type, unitId);
            if (candidates.Length > 1)
            {
                AddIssue(GetSourceLocation(type), GetMetadataName(type));
            }
            return unitId;
        }

        private string? GetExplicitUnit(SyntaxTree tree)
        {
            _optionsProvider.GetOptions(tree).TryGetValue(
                "build_metadata.Compile.AtomUIRegistrationUnit",
                out var explicitUnit);
            return explicitUnit;
        }

        private void AddIssue(Location? location, string identity)
        {
            var source = "<Unknown>";
            var line = 0;
            var column = 0;
            if (location is not null && location.IsInSource)
            {
                var span = location.GetLineSpan();
                source = NormalizeSourcePath(span.Path, _projectDirectory);
                line = span.StartLinePosition.Line + 1;
                column = span.StartLinePosition.Character + 1;
            }

            var key = string.Join(
                "\u001f",
                source,
                line.ToString(System.Globalization.CultureInfo.InvariantCulture),
                column.ToString(System.Globalization.CultureInfo.InvariantCulture),
                identity);
            if (!_issues.ContainsKey(key))
            {
                _issues.Add(
                    key,
                    new RegistrationUnitDependencyIssue(
                        identity,
                        source,
                        line,
                        column,
                        location));
            }
        }

        private Location? GetSourceLocation(ISymbol symbol)
        {
            return symbol.Locations.Where(static location => location.IsInSource)
                         .OrderBy(location => NormalizeSourcePath(
                             location.GetLineSpan().Path,
                             _projectDirectory), StringComparer.Ordinal)
                         .ThenBy(static location => location.SourceSpan.Start)
                         .ThenBy(static location => location.SourceSpan.Length)
                         .FirstOrDefault();
        }

        private static IEnumerable<INamedTypeSymbol> GetTypes(INamespaceSymbol ns)
        {
            foreach (var type in ns.GetTypeMembers().OrderBy(
                         static type => type.MetadataName,
                         StringComparer.Ordinal))
            {
                yield return type;
            }
            foreach (var child in ns.GetNamespaceMembers().OrderBy(
                         static child => child.Name,
                         StringComparer.Ordinal))
            {
                foreach (var type in GetTypes(child))
                {
                    yield return type;
                }
            }
        }

        private static bool IsDynamicCreationMethod(IMethodSymbol method)
        {
            var containingType = GetMetadataName(method.ContainingType.OriginalDefinition);
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
                if (GetMetadataName(current.OriginalDefinition) == "Avalonia.Controls.Control")
                {
                    return true;
                }
            }
            return false;
        }

        private static string GetMetadataName(INamedTypeSymbol type)
        {
            return LinkedRegistrationSymbolName.GetTypeMetadataName(type);
        }

        private static string NormalizeSourcePath(string path, string? projectDirectory)
        {
            var normalized = path.Replace('\\', '/');
            var normalizedProjectDirectory = projectDirectory?.Replace('\\', '/').TrimEnd('/');
            if (normalizedProjectDirectory is { Length: > 0 } &&
                normalized.StartsWith(
                    normalizedProjectDirectory + "/",
                    StringComparison.OrdinalIgnoreCase))
            {
                return normalized.Substring(normalizedProjectDirectory.Length + 1);
            }
            return normalized;
        }
    }
}

internal sealed class RegistrationUnitDependencyAnalysis
{
    internal static RegistrationUnitDependencyAnalysis Empty { get; } = new(
        new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal),
        Array.Empty<RegistrationUnitDependencyIssue>());

    internal RegistrationUnitDependencyAnalysis(
        IReadOnlyDictionary<string, IReadOnlyList<string>> dependencies,
        IReadOnlyList<RegistrationUnitDependencyIssue> issues)
    {
        Dependencies = dependencies;
        Issues = issues;
    }

    internal IReadOnlyDictionary<string, IReadOnlyList<string>> Dependencies { get; }
    internal IReadOnlyList<RegistrationUnitDependencyIssue> Issues { get; }

    internal IReadOnlyList<string> GetDependencies(string unitId)
    {
        return Dependencies.TryGetValue(unitId, out var dependencies)
            ? dependencies
            : Array.Empty<string>();
    }
}

internal sealed class RegistrationUnitDependencyIssue
{
    internal RegistrationUnitDependencyIssue(
        string identity,
        string source,
        int line,
        int column,
        Location? location)
    {
        Identity = identity;
        Source = source;
        Line = line;
        Column = column;
        Location = location;
    }

    internal string Identity { get; }
    internal string Source { get; }
    internal int Line { get; }
    internal int Column { get; }
    internal Location? Location { get; }
}
