using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

internal sealed class SemanticPartContractValidator
{
    private const string AvaloniaControlType = "Avalonia.Controls.Control";
    private const string AvaloniaStyledElementType = "Avalonia.StyledElement";
    private const string AvaloniaControlThemeType = "global::Avalonia.Styling.ControlTheme";

    private readonly Compilation _compilation;
    private readonly IReadOnlyList<INamedTypeSymbol> _sourceControls;
    private readonly IReadOnlyList<ThemeAssetInfo> _assets;
    private readonly SemanticPartTypeResolver _typeResolver;
    private readonly Action<Diagnostic> _reportDiagnostic;

    internal SemanticPartContractValidator(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        IReadOnlyList<ThemeAssetInfo> assets,
        SemanticPartTypeResolver typeResolver,
        Action<Diagnostic> reportDiagnostic)
    {
        _compilation = compilation;
        _sourceControls = sourceControls;
        _assets = assets;
        _typeResolver = typeResolver;
        _reportDiagnostic = reportDiagnostic;
    }

    internal bool ValidateControl(SemanticControlDeclaration declaration)
    {
        var controlBase = _compilation.GetTypeByMetadataName(AvaloniaControlType);
        if (declaration.ControlType.DeclaredAccessibility == Accessibility.Public &&
            declaration.ControlType.Arity == 0 &&
            controlBase is not null &&
            SemanticPartTypeResolver.IsAssignableTo(declaration.ControlType, controlBase))
        {
            return true;
        }

        _reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.SemanticPartInvalidDeclaration,
            declaration.Location,
            "<control>",
            declaration.ControlType.ToDisplayString(),
            "the target must be a public, non-generic Avalonia Control"));
        return false;
    }

    internal bool ValidatePart(
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        out SemanticPartDeclaration validatedPart)
    {
        validatedPart = part;
        var valid = true;
        if (!IsCamelCaseSegment(part.Name) || string.Equals(part.Name, "root", StringComparison.Ordinal))
        {
            ReportInvalidDeclaration(control, part, "name must be camelCase and cannot be root");
            valid = false;
        }
        if (!IsPartPath(part.Path))
        {
            ReportInvalidDeclaration(control, part, "path must contain camelCase segments");
            valid = false;
        }
        else if (string.Equals(part.Path, "root", StringComparison.Ordinal))
        {
            ReportInvalidDeclaration(control, part, "path cannot be root");
            valid = false;
        }
        if (!IsSemanticSelectorClass(part.SelectorClass))
        {
            ReportInvalidDeclaration(
                control,
                part,
                "SelectorClass must use the semantic-* kebab-case namespace");
            valid = false;
        }
        if (part.Cardinality is < 0 or > 2)
        {
            ReportInvalidDeclaration(control, part, "Cardinality is outside the supported range");
            valid = false;
        }
        if (part.Customization is < 1 or > 2)
        {
            ReportInvalidDeclaration(
                control,
                part,
                "only Selector or SelectorAndTheme customization is valid for declared Parts");
            valid = false;
        }

        var styledElement = _compilation.GetTypeByMetadataName(AvaloniaStyledElementType);
        if (part.ContractType is not INamedTypeSymbol contractType ||
            contractType.DeclaredAccessibility != Accessibility.Public ||
            styledElement is null ||
            !SemanticPartTypeResolver.IsAssignableTo(contractType, styledElement))
        {
            _reportDiagnostic(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.SemanticPartInvalidContractType,
                part.Location,
                part.Name,
                control.ControlType.ToDisplayString()));
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(part.Since))
        {
            _reportDiagnostic(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.SemanticPartMissingSince,
                part.Location,
                part.Name,
                control.ControlType.ToDisplayString()));
        }

        if (!ValidateThemeContract(control, part, out var themeTargetType))
        {
            valid = false;
        }

        validatedPart = part.WithThemeTargetType(themeTargetType);
        return valid;
    }

    internal bool ValidateUniqueParts(
        SemanticControlDeclaration control,
        IReadOnlyList<SemanticPartDeclaration> parts)
    {
        var valid = true;
        valid &= ValidateUnique(control, parts, static part => part.Name, "name");
        valid &= ValidateUnique(control, parts, static part => part.Path, "path");
        valid &= ValidateUnique(
            control,
            parts,
            static part => part.SelectorClass ?? string.Empty,
            "selector class");
        return valid;
    }

    private bool ValidateThemeContract(
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        out ITypeSymbol? themeTargetType)
    {
        themeTargetType = null;
        if (part.Customization != 2)
        {
            if (string.IsNullOrWhiteSpace(part.ThemePropertyName))
            {
                return true;
            }

            ReportInvalidTheme(control, part, "ThemePropertyName is only valid with SelectorAndTheme");
            return false;
        }

        var controlBase = _compilation.GetTypeByMetadataName(AvaloniaControlType);
        if (part.ContractType is not INamedTypeSymbol contractType ||
            controlBase is null ||
            !SemanticPartTypeResolver.IsAssignableTo(contractType, controlBase))
        {
            ReportInvalidTheme(control, part, "ContractType must be a public Control");
            return false;
        }
        if (string.IsNullOrWhiteSpace(part.ThemePropertyName))
        {
            ReportInvalidTheme(control, part, "ThemePropertyName is required");
            return false;
        }

        var property = control.ControlType.GetMembers(part.ThemePropertyName!)
                              .OfType<IPropertySymbol>()
                              .SingleOrDefault(static candidate =>
                                  !candidate.IsStatic && candidate.DeclaredAccessibility == Accessibility.Public);
        if (property is null ||
            property.GetMethod is null ||
            property.GetMethod.DeclaredAccessibility != Accessibility.Public ||
            property.SetMethod is null ||
            property.SetMethod.DeclaredAccessibility != Accessibility.Public ||
            property.SetMethod.IsInitOnly ||
            !string.Equals(
                property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                AvaloniaControlThemeType,
                StringComparison.Ordinal))
        {
            ReportInvalidTheme(
                control,
                part,
                $"public property '{part.ThemePropertyName}' must have type Avalonia.Styling.ControlTheme and a public getter and setter");
            return false;
        }

        return ValidateThemeAssetTargets(control, part, contractType, out themeTargetType);
    }

    private bool ValidateThemeAssetTargets(
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        INamedTypeSymbol contractType,
        out ITypeSymbol? themeTargetType)
    {
        themeTargetType = null;
        INamedTypeSymbol? sharedTarget = null;
        var valid = true;
        foreach (var asset in _assets
                     .Where(asset => IsSemanticThemeAssetForControl(
                         control.ControlType,
                         part.ThemePropertyName!,
                         asset))
                     .OrderBy(static asset => asset.AssetPath, StringComparer.Ordinal))
        {
            var target = ResolveSemanticThemeTarget(asset);
            if (target is null || !SemanticPartTypeResolver.IsAssignableTo(target, contractType))
            {
                ReportInvalidTheme(
                    control,
                    part,
                    $"theme asset '{asset.AssetPath}' TargetType must be assignable to '{contractType.ToDisplayString()}'");
                valid = false;
                continue;
            }

            if (sharedTarget is not null && !SymbolEqualityComparer.Default.Equals(sharedTarget, target))
            {
                ReportInvalidTheme(
                    control,
                    part,
                    $"theme assets must use one TargetType; '{asset.AssetPath}' targets '{target.ToDisplayString()}' instead of '{sharedTarget.ToDisplayString()}'");
                valid = false;
                continue;
            }

            sharedTarget = target;
        }

        if (valid && sharedTarget is not null)
        {
            themeTargetType = sharedTarget;
        }
        return valid;
    }

    private bool IsSemanticThemeAssetForControl(
        INamedTypeSymbol controlType,
        string propertyName,
        ThemeAssetInfo asset)
    {
        if (!string.Equals(asset.FileName, propertyName, StringComparison.Ordinal))
        {
            return false;
        }

        if (asset.ControlCandidate is not null &&
            ControlThemeModelBuilder.FindPublicControlsByName(
                _compilation,
                _sourceControls,
                asset.ControlCandidate).Any())
        {
            return false;
        }

        var owners = _sourceControls.Where(control => HasControlThemeProperty(control, propertyName)).ToArray();
        return owners.Length == 1 && SymbolEqualityComparer.Default.Equals(owners[0], controlType);
    }

    private static bool HasControlThemeProperty(INamedTypeSymbol control, string propertyName)
    {
        return control.GetMembers(propertyName)
                      .OfType<IPropertySymbol>()
                      .Any(static property =>
                          !property.IsStatic &&
                          property.DeclaredAccessibility == Accessibility.Public &&
                          string.Equals(
                              property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                              AvaloniaControlThemeType,
                              StringComparison.Ordinal));
    }

    private INamedTypeSymbol? ResolveSemanticThemeTarget(ThemeAssetInfo asset)
    {
        foreach (var targetType in asset.TargetTypes)
        {
            if (_typeResolver.ResolveTargetType(targetType) is { } target)
            {
                return target;
            }
        }
        return null;
    }

    private bool ValidateUnique(
        SemanticControlDeclaration control,
        IEnumerable<SemanticPartDeclaration> parts,
        Func<SemanticPartDeclaration, string> keySelector,
        string kind)
    {
        var valid = true;
        foreach (var group in parts.GroupBy(keySelector, StringComparer.Ordinal).Where(static group => group.Count() > 1))
        {
            foreach (var duplicate in group.Skip(1))
            {
                _reportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.SemanticPartDuplicateDeclaration,
                    duplicate.Location,
                    duplicate.Name,
                    control.ControlType.ToDisplayString(),
                    kind,
                    group.Key));
            }
            valid = false;
        }
        return valid;
    }

    private static bool IsPartPath(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && value.Split('.').All(IsCamelCaseSegment);
    }

    private static bool IsCamelCaseSegment(string value)
    {
        return value.Length != 0 &&
               value[0] is >= 'a' and <= 'z' &&
               value.Skip(1).All(static character =>
                   character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9');
    }

    private static bool IsSemanticSelectorClass(string? value)
    {
        const string prefix = "semantic-";
        if (string.IsNullOrWhiteSpace(value) ||
            !value!.StartsWith(prefix, StringComparison.Ordinal) ||
            value.Length == prefix.Length ||
            string.Equals(value, "semantic-root", StringComparison.Ordinal))
        {
            return false;
        }

        var previousWasHyphen = false;
        for (var index = prefix.Length; index < value.Length; index++)
        {
            var character = value[index];
            if (character == '-')
            {
                if (previousWasHyphen || index == value.Length - 1)
                {
                    return false;
                }
                previousWasHyphen = true;
                continue;
            }
            if (character is not (>= 'a' and <= 'z') and not (>= '0' and <= '9'))
            {
                return false;
            }
            previousWasHyphen = false;
        }
        return true;
    }

    private void ReportInvalidDeclaration(
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        string reason)
    {
        _reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.SemanticPartInvalidDeclaration,
            part.Location,
            part.Name,
            control.ControlType.ToDisplayString(),
            reason));
    }

    private void ReportInvalidTheme(
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        string reason)
    {
        _reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.SemanticPartInvalidThemeContract,
            part.Location,
            part.Name,
            control.ControlType.ToDisplayString(),
            reason));
    }
}
