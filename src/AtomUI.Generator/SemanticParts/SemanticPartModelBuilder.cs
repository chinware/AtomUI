using AtomUI.Generator.Diagnostics;
using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

internal static class SemanticPartModelBuilder
{
    private const string AvaloniaControlType = "Avalonia.Controls.Control";
    private const string AvaloniaStyledElementType = "Avalonia.StyledElement";
    private const string AvaloniaControlThemeType = "global::Avalonia.Styling.ControlTheme";

    internal static IReadOnlyList<SemanticControlDeclaration> Build(
        Compilation compilation,
        IEnumerable<SemanticControlDeclaration> declarations,
        IReadOnlyList<ThemeAssetInfo> assets,
        Action<Diagnostic> reportDiagnostic)
    {
        var result = new List<SemanticControlDeclaration>();
        var sourceControls = ControlThemeModelBuilder
                             .GetPublicControls(compilation.Assembly.GlobalNamespace)
                             .ToArray();
        var mergedDeclarations = declarations
                                 .GroupBy(static declaration => declaration.ControlType, SymbolEqualityComparer.Default)
                                 .Select(static group => SemanticControlDeclaration.Merge(group));
        foreach (var declaration in mergedDeclarations.OrderBy(
                     static item => item.ControlType.ToDisplayString(GeneratorSymbolDisplay.FullyQualifiedType),
                     StringComparer.Ordinal))
        {
            if (!ValidateControl(compilation, declaration, reportDiagnostic))
            {
                continue;
            }

            var validParts = new List<SemanticPartDeclaration>();
            foreach (var part in declaration.Parts)
            {
                if (ValidatePart(
                        compilation,
                        sourceControls,
                        assets,
                        declaration,
                        part,
                        reportDiagnostic,
                        out var validatedPart))
                {
                    validParts.Add(validatedPart);
                }
            }

            if (!ValidateUniqueParts(declaration, validParts, reportDiagnostic) ||
                validParts.Count != declaration.Parts.Count)
            {
                continue;
            }

            if (!ValidateTemplates(
                    compilation,
                    sourceControls,
                    declaration,
                    validParts,
                    assets,
                    reportDiagnostic))
            {
                continue;
            }

            result.Add(declaration.WithParts(validParts));
        }

        return result;
    }

    private static bool ValidateControl(
        Compilation compilation,
        SemanticControlDeclaration declaration,
        Action<Diagnostic> reportDiagnostic)
    {
        var controlBase = compilation.GetTypeByMetadataName(AvaloniaControlType);
        if (declaration.ControlType.DeclaredAccessibility == Accessibility.Public &&
            declaration.ControlType.Arity == 0 &&
            controlBase is not null &&
            IsAssignableTo(declaration.ControlType, controlBase))
        {
            return true;
        }

        reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.SemanticPartInvalidDeclaration,
            declaration.Location,
            "<control>",
            declaration.ControlType.ToDisplayString(),
            "the target must be a public, non-generic Avalonia Control"));
        return false;
    }

    private static bool ValidatePart(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        IReadOnlyList<ThemeAssetInfo> assets,
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        Action<Diagnostic> reportDiagnostic,
        out SemanticPartDeclaration validatedPart)
    {
        validatedPart = part;
        var valid = true;
        if (!IsCamelCaseSegment(part.Name) || string.Equals(part.Name, "root", StringComparison.Ordinal))
        {
            ReportInvalidDeclaration(control, part, "name must be camelCase and cannot be root", reportDiagnostic);
            valid = false;
        }
        if (!IsPartPath(part.Path))
        {
            ReportInvalidDeclaration(control, part, "path must contain camelCase segments", reportDiagnostic);
            valid = false;
        }
        else if (string.Equals(part.Path, "root", StringComparison.Ordinal))
        {
            ReportInvalidDeclaration(control, part, "path cannot be root", reportDiagnostic);
            valid = false;
        }
        if (!IsSemanticSelectorClass(part.SelectorClass))
        {
            ReportInvalidDeclaration(
                control,
                part,
                "SelectorClass must use the semantic-* kebab-case namespace",
                reportDiagnostic);
            valid = false;
        }
        if (part.Cardinality is < 0 or > 2)
        {
            ReportInvalidDeclaration(control, part, "Cardinality is outside the supported range", reportDiagnostic);
            valid = false;
        }
        if (part.Customization is < 1 or > 2)
        {
            ReportInvalidDeclaration(
                control,
                part,
                "only Selector or SelectorAndTheme customization is valid for declared Parts",
                reportDiagnostic);
            valid = false;
        }

        var styledElement = compilation.GetTypeByMetadataName(AvaloniaStyledElementType);
        if (part.ContractType is not INamedTypeSymbol contractType ||
            contractType.DeclaredAccessibility != Accessibility.Public ||
            styledElement is null ||
            !IsAssignableTo(contractType, styledElement))
        {
            reportDiagnostic(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.SemanticPartInvalidContractType,
                part.Location,
                part.Name,
                control.ControlType.ToDisplayString()));
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(part.Since))
        {
            reportDiagnostic(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.SemanticPartMissingSince,
                part.Location,
                part.Name,
                control.ControlType.ToDisplayString()));
        }

        if (!ValidateThemeContract(
                compilation,
                sourceControls,
                assets,
                control,
                part,
                reportDiagnostic,
                out var themeTargetType))
        {
            valid = false;
        }

        validatedPart = part.WithThemeTargetType(themeTargetType);
        return valid;
    }

    private static bool ValidateThemeContract(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        IReadOnlyList<ThemeAssetInfo> assets,
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        Action<Diagnostic> reportDiagnostic,
        out ITypeSymbol? themeTargetType)
    {
        themeTargetType = null;
        if (part.Customization != 2)
        {
            if (string.IsNullOrWhiteSpace(part.ThemePropertyName))
            {
                return true;
            }

            ReportInvalidTheme(
                control,
                part,
                "ThemePropertyName is only valid with SelectorAndTheme",
                reportDiagnostic);
            return false;
        }

        var controlBase = compilation.GetTypeByMetadataName(AvaloniaControlType);
        if (part.ContractType is not INamedTypeSymbol contractType ||
            controlBase is null ||
            !IsAssignableTo(contractType, controlBase))
        {
            ReportInvalidTheme(
                control,
                part,
                "ContractType must be a public Control",
                reportDiagnostic);
            return false;
        }
        if (string.IsNullOrWhiteSpace(part.ThemePropertyName))
        {
            ReportInvalidTheme(
                control,
                part,
                "ThemePropertyName is required",
                reportDiagnostic);
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
                $"public property '{part.ThemePropertyName}' must have type Avalonia.Styling.ControlTheme and a public getter and setter",
                reportDiagnostic);
            return false;
        }

        return ValidateThemeAssetTargets(
            compilation,
            sourceControls,
            assets,
            control,
            part,
            contractType,
            reportDiagnostic,
            out themeTargetType);
    }

    private static bool ValidateThemeAssetTargets(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        IReadOnlyList<ThemeAssetInfo> assets,
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        INamedTypeSymbol contractType,
        Action<Diagnostic> reportDiagnostic,
        out ITypeSymbol? themeTargetType)
    {
        themeTargetType = null;
        INamedTypeSymbol? sharedTarget = null;
        var valid = true;
        foreach (var asset in assets
                     .Where(asset => IsSemanticThemeAssetForControl(
                         compilation,
                         sourceControls,
                         control.ControlType,
                         part.ThemePropertyName!,
                         asset))
                     .OrderBy(static asset => asset.AssetPath, StringComparer.Ordinal))
        {
            var target = ResolveSemanticThemeTarget(compilation, sourceControls, asset);
            if (target is null || !IsAssignableTo(target, contractType))
            {
                ReportInvalidTheme(
                    control,
                    part,
                    $"theme asset '{asset.AssetPath}' TargetType must be assignable to '{contractType.ToDisplayString()}'",
                    reportDiagnostic);
                valid = false;
                continue;
            }

            if (sharedTarget is not null &&
                !SymbolEqualityComparer.Default.Equals(sharedTarget, target))
            {
                ReportInvalidTheme(
                    control,
                    part,
                    $"theme assets must use one TargetType; '{asset.AssetPath}' targets '{target.ToDisplayString()}' instead of '{sharedTarget.ToDisplayString()}'",
                    reportDiagnostic);
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

    private static bool IsSemanticThemeAssetForControl(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
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
                compilation,
                sourceControls,
                asset.ControlCandidate).Any())
        {
            return false;
        }

        var owners = sourceControls.Where(control => HasControlThemeProperty(control, propertyName)).ToArray();
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

    private static INamedTypeSymbol? ResolveSemanticThemeTarget(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ThemeAssetInfo asset)
    {
        foreach (var targetType in asset.TargetTypes)
        {
            if (ResolveTargetType(compilation, sourceControls, targetType) is { } target)
            {
                return target;
            }
        }
        return null;
    }

    private static bool ValidateUniqueParts(
        SemanticControlDeclaration control,
        IReadOnlyList<SemanticPartDeclaration> parts,
        Action<Diagnostic> reportDiagnostic)
    {
        var valid = true;
        valid &= ValidateUnique(control, parts, static part => part.Name, "name", reportDiagnostic);
        valid &= ValidateUnique(control, parts, static part => part.Path, "path", reportDiagnostic);
        valid &= ValidateUnique(
            control,
            parts,
            static part => part.SelectorClass ?? string.Empty,
            "selector class",
            reportDiagnostic);
        return valid;
    }

    private static bool ValidateUnique(
        SemanticControlDeclaration control,
        IReadOnlyList<SemanticPartDeclaration> parts,
        Func<SemanticPartDeclaration, string> keySelector,
        string kind,
        Action<Diagnostic> reportDiagnostic)
    {
        var valid = true;
        foreach (var group in parts.GroupBy(keySelector, StringComparer.Ordinal).Where(static group => group.Count() > 1))
        {
            foreach (var duplicate in group.Skip(1))
            {
                reportDiagnostic(Diagnostic.Create(
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

    private static bool ValidateTemplates(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        SemanticControlDeclaration control,
        IReadOnlyList<SemanticPartDeclaration> parts,
        IReadOnlyList<ThemeAssetInfo> assets,
        Action<Diagnostic> reportDiagnostic)
    {
        var staticParts = parts.Where(static part => !part.RuntimeCreated).ToArray();
        if (staticParts.Length == 0)
        {
            return true;
        }

        var templates = ResolveTemplates(compilation, sourceControls, control.ControlType, assets);

        if (templates.Count == 0)
        {
            reportDiagnostic(Diagnostic.Create(
                AtomUIDiagnosticDescriptors.SemanticPartMissingControlTemplate,
                control.Location,
                control.ControlType.ToDisplayString()));
            return false;
        }

        var valid = true;
        for (var templateIndex = 0; templateIndex < templates.Count; templateIndex++)
        {
            var entry = templates[templateIndex];
            var templateName = $"{entry.Asset.AssetPath}#{entry.Template.Index}";
            var declaredClasses = new HashSet<string>(
                staticParts.Select(static part => part.SelectorClass!),
                StringComparer.Ordinal);
            foreach (var nodeMarkers in entry.Template.Markers
                         .Where(marker => declaredClasses.Contains(marker.SelectorClass))
                         .GroupBy(static marker => marker.NodeId))
            {
                var selectorClasses = nodeMarkers.Select(static marker => marker.SelectorClass)
                                                 .Distinct(StringComparer.Ordinal)
                                                 .OrderBy(static selectorClass => selectorClass, StringComparer.Ordinal)
                                                 .ToArray();
                if (selectorClasses.Length <= 1)
                {
                    continue;
                }

                reportDiagnostic(Diagnostic.Create(
                    AtomUIDiagnosticDescriptors.SemanticPartTemplateNodeConflict,
                    entry.Asset.CreateLocation(),
                    control.ControlType.ToDisplayString(),
                    templateName,
                    nodeMarkers.First().TypeName,
                    string.Join(", ", selectorClasses)));
                valid = false;
            }
            foreach (var part in staticParts)
            {
                var markers = entry.Template.Markers
                                   .Where(marker => string.Equals(
                                       marker.SelectorClass,
                                       part.SelectorClass,
                                       StringComparison.Ordinal))
                                   .ToArray();
                if (!MatchesCardinality(part.Cardinality, markers.Length))
                {
                    reportDiagnostic(Diagnostic.Create(
                        AtomUIDiagnosticDescriptors.SemanticPartTemplateCardinalityMismatch,
                        entry.Asset.CreateLocation(),
                        control.ControlType.ToDisplayString(),
                        templateName,
                        markers.Length,
                        part.Name,
                        DescribeCardinality(part.Cardinality)));
                    valid = false;
                }

                if (part.ContractType is not INamedTypeSymbol contractType)
                {
                    continue;
                }
                foreach (var marker in markers)
                {
                    var markerType = ResolveElementType(compilation, sourceControls, marker);
                    if (markerType is null || !IsAssignableTo(markerType, contractType))
                    {
                        reportDiagnostic(Diagnostic.Create(
                            AtomUIDiagnosticDescriptors.SemanticPartTemplateContractTypeMismatch,
                            entry.Asset.CreateLocation(),
                            control.ControlType.ToDisplayString(),
                            templateName,
                            part.Name,
                            markerType?.ToDisplayString() ?? marker.TypeName,
                            contractType.ToDisplayString()));
                        valid = false;
                    }
                }
            }
        }

        return valid;
    }

    private static IReadOnlyList<(
        ThemeAssetInfo Asset,
        ThemeAssetSemanticTemplateInfo Template)> ResolveTemplates(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        INamedTypeSymbol controlType,
        IReadOnlyList<ThemeAssetInfo> assets)
    {
        var themes = assets.OrderBy(static asset => asset.AssetPath, StringComparer.Ordinal)
                           .SelectMany(static asset => asset.SemanticThemes.Select(theme => (Asset: asset, Theme: theme)))
                           .ToArray();
        var resolvedTargets = themes.ToDictionary(
            static entry => entry.Theme,
            entry => ResolveTargetType(compilation, sourceControls, entry.Theme.TargetType));
        var templates = new List<(
            ThemeAssetInfo Asset,
            ThemeAssetSemanticTemplateInfo Template)>();
        var visited = new HashSet<ThemeAssetSemanticThemeInfo>();

        foreach (var theme in themes.Where(entry =>
                     SymbolEqualityComparer.Default.Equals(resolvedTargets[entry.Theme], controlType)))
        {
            CollectTemplates(theme);
        }
        return templates;

        void CollectTemplates((ThemeAssetInfo Asset, ThemeAssetSemanticThemeInfo Theme) entry)
        {
            if (!visited.Add(entry.Theme))
            {
                return;
            }

            if (entry.Theme.Templates.Count != 0)
            {
                for (var index = 0; index < entry.Theme.Templates.Count; index++)
                {
                    templates.Add((entry.Asset, entry.Theme.Templates[index]));
                }
            }

            if (entry.Theme.OverridesBaseTemplate)
            {
                return;
            }

            if (entry.Theme.BasedOn is null ||
                ResolveTargetType(compilation, sourceControls, entry.Theme.BasedOn) is not { } baseType)
            {
                return;
            }

            foreach (var baseTheme in themes.Where(candidate =>
                         SymbolEqualityComparer.Default.Equals(resolvedTargets[candidate.Theme], baseType)))
            {
                CollectTemplates(baseTheme);
            }
        }
    }

    private static INamedTypeSymbol? ResolveTargetType(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ThemeAssetTargetTypeReference reference)
    {
        if (!TryParseTypeReference(reference.Value, out var prefix, out var typeName) ||
            !reference.Namespaces.TryGetValue(prefix, out var xmlNamespace))
        {
            return null;
        }

        return ResolveXmlType(compilation, sourceControls, xmlNamespace, typeName);
    }

    private static INamedTypeSymbol? ResolveElementType(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        ThemeAssetSemanticMarkerInfo marker)
    {
        return ResolveXmlType(compilation, sourceControls, marker.XmlNamespace, marker.TypeName);
    }

    private static INamedTypeSymbol? ResolveXmlType(
        Compilation compilation,
        IReadOnlyList<INamedTypeSymbol> sourceControls,
        string xmlNamespace,
        string typeName)
    {
        if (xmlNamespace.StartsWith("using:", StringComparison.Ordinal))
        {
            return compilation.GetTypeByMetadataName($"{xmlNamespace.Substring("using:".Length)}.{typeName}");
        }
        if (xmlNamespace.StartsWith("clr-namespace:", StringComparison.Ordinal))
        {
            var value = xmlNamespace.Substring("clr-namespace:".Length);
            var separator = value.IndexOf(';');
            var clrNamespace = separator < 0 ? value : value.Substring(0, separator);
            return compilation.GetTypeByMetadataName($"{clrNamespace}.{typeName}");
        }

        foreach (var assembly in GetAssemblies(compilation))
        {
            foreach (var attribute in assembly.GetAttributes())
            {
                if (!string.Equals(
                        attribute.AttributeClass?.ToDisplayString(),
                        "Avalonia.Metadata.XmlnsDefinitionAttribute",
                        StringComparison.Ordinal) ||
                    attribute.ConstructorArguments.Length < 2 ||
                    !string.Equals(
                        attribute.ConstructorArguments[0].Value as string,
                        xmlNamespace,
                        StringComparison.Ordinal))
                {
                    continue;
                }

                var clrNamespace = attribute.ConstructorArguments[1].Value as string;
                if (!string.IsNullOrWhiteSpace(clrNamespace) &&
                    assembly.GetTypeByMetadataName($"{clrNamespace}.{typeName}") is { } resolved)
                {
                    return resolved;
                }
            }
        }

        var matches = ControlThemeModelBuilder.FindPublicControlsByName(
            compilation,
            sourceControls,
            typeName).ToArray();
        return matches.Length == 1 ? matches[0] : null;
    }

    private static IEnumerable<IAssemblySymbol> GetAssemblies(Compilation compilation)
    {
        yield return compilation.Assembly;
        foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
        {
            yield return assembly;
        }
    }

    private static bool TryParseTypeReference(string value, out string prefix, out string typeName)
    {
        var reference = value.Trim();
        if (reference.StartsWith("{x:Type", StringComparison.Ordinal) &&
            reference.EndsWith("}", StringComparison.Ordinal))
        {
            reference = reference.Substring("{x:Type".Length, reference.Length - "{x:Type".Length - 1).Trim();
        }

        var colon = reference.IndexOf(':');
        if (colon < 0)
        {
            prefix = string.Empty;
            typeName = reference;
            return typeName.Length != 0;
        }

        prefix = reference.Substring(0, colon);
        typeName = reference.Substring(colon + 1);
        return prefix.Length != 0 && typeName.Length != 0;
    }

    private static bool MatchesCardinality(int cardinality, int count)
    {
        return cardinality switch
        {
            1 => count <= 1,
            2 => count >= 1,
            _ => count == 1
        };
    }

    private static string DescribeCardinality(int cardinality)
    {
        return cardinality switch
        {
            1 => "zero or one",
            2 => "one or more",
            _ => "exactly one"
        };
    }

    private static bool IsAssignableTo(ITypeSymbol type, ITypeSymbol contractType)
    {
        for (var current = type as INamedTypeSymbol; current is not null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, contractType))
            {
                return true;
            }
        }
        return false;
    }

    private static bool IsPartPath(string value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Split('.').All(IsCamelCaseSegment);
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

    private static void ReportInvalidDeclaration(
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        string reason,
        Action<Diagnostic> reportDiagnostic)
    {
        reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.SemanticPartInvalidDeclaration,
            part.Location,
            part.Name,
            control.ControlType.ToDisplayString(),
            reason));
    }

    private static void ReportInvalidTheme(
        SemanticControlDeclaration control,
        SemanticPartDeclaration part,
        string reason,
        Action<Diagnostic> reportDiagnostic)
    {
        reportDiagnostic(Diagnostic.Create(
            AtomUIDiagnosticDescriptors.SemanticPartInvalidThemeContract,
            part.Location,
            part.Name,
            control.ControlType.ToDisplayString(),
            reason));
    }
}
