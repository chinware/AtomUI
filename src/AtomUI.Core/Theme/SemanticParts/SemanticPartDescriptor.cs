using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Theme.Schema;

public sealed class SemanticPartDescriptor
{
    public SemanticPartDescriptor(
        string name,
        string path,
        string? selectorClass,
        Type contractType,
        SemanticPartCardinality cardinality,
        SemanticPartCustomization customization,
        ControlThemeSemanticPartDescriptor? theme,
        bool crossVisualRoot,
        string? since,
        bool runtimeCreated,
        string? selectorRoute = null,
        Type? styleType = null,
        bool crossNestedOwners = false)
    {
        ValidatePartName(name, nameof(name));
        ValidatePartPath(path, nameof(path));
        ArgumentNullException.ThrowIfNull(contractType);
        if (cardinality is not (SemanticPartCardinality.Single or
            SemanticPartCardinality.Optional or
            SemanticPartCardinality.Multiple))
        {
            throw new ArgumentOutOfRangeException(
                nameof(cardinality),
                cardinality,
                "Unsupported Semantic Part cardinality.");
        }
        if (customization is not (SemanticPartCustomization.Root or
            SemanticPartCustomization.Selector or
            SemanticPartCustomization.SelectorAndTheme))
        {
            throw new ArgumentOutOfRangeException(
                nameof(customization),
                customization,
                "Unsupported Semantic Part customization mode.");
        }
        if (!typeof(StyledElement).IsAssignableFrom(contractType))
        {
            throw new ArgumentException(
                $"Semantic Part ContractType '{contractType.FullName}' must derive from Avalonia.StyledElement.",
                nameof(contractType));
        }

        var isRoot = string.Equals(name, "root", StringComparison.Ordinal);
        if (isRoot)
        {
            if (!string.Equals(path, "root", StringComparison.Ordinal) ||
                selectorClass is not null ||
                cardinality != SemanticPartCardinality.Single ||
                customization != SemanticPartCustomization.Root ||
                theme is not null ||
                crossVisualRoot ||
                runtimeCreated ||
                selectorRoute is not null ||
                styleType is not null)
            {
                throw new ArgumentException(
                    "The root Semantic Part must use path 'root', Single cardinality, Root customization, and no selector, theme, cross-root, or runtime-created metadata.",
                    nameof(name));
            }
        }
        else
        {
            if (string.Equals(path, "root", StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "Only the implicit root Semantic Part can use path 'root'.",
                    nameof(path));
            }
            ValidateSelectorClass(selectorClass, nameof(selectorClass));
            selectorRoute = NormalizeSelectorRoute(
                selectorRoute,
                selectorClass!,
                runtimeCreated,
                nameof(selectorRoute));
            if (customization == SemanticPartCustomization.Root)
            {
                throw new ArgumentException(
                    "Only the implicit root Semantic Part can use Root customization.",
                    nameof(customization));
            }
        }

        if (customization == SemanticPartCustomization.SelectorAndTheme)
        {
            if (theme is null)
            {
                throw new ArgumentException(
                    "SelectorAndTheme Semantic Parts require strongly typed theme metadata.",
                    nameof(theme));
            }
            if (!typeof(Control).IsAssignableFrom(contractType))
            {
                throw new ArgumentException(
                    "SelectorAndTheme Semantic Parts must target an Avalonia Control.",
                    nameof(contractType));
            }
        }
        else if (theme is not null)
        {
            throw new ArgumentException(
                "Semantic Part theme metadata is only valid with SelectorAndTheme customization.",
                nameof(theme));
        }

        Name = name;
        Path = path;
        SelectorClass = selectorClass;
        SelectorRoute = selectorRoute;
        ContractType = contractType;
        Cardinality = cardinality;
        Customization = customization;
        Theme = theme;
        CrossVisualRoot = crossVisualRoot;
        Since = string.IsNullOrWhiteSpace(since) ? null : since;
        RuntimeCreated = runtimeCreated;
        CrossNestedOwners = crossNestedOwners;
        StyleType = styleType;
    }

    public string Name { get; }
    public string Path { get; }
    public string? SelectorClass { get; }
    public string? SelectorRoute { get; }
    public Type ContractType { get; }
    public SemanticPartCardinality Cardinality { get; }
    public SemanticPartCustomization Customization { get; }
    public ControlThemeSemanticPartDescriptor? Theme { get; }
    public bool CrossVisualRoot { get; }
    public string? Since { get; }
    public bool RuntimeCreated { get; }
    public bool CrossNestedOwners { get; }
    public Type? StyleType { get; }

    private static void ValidatePartName(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        if (value.Split('.').Any(static segment => !IsCamelCaseSegment(segment)))
        {
            throw new ArgumentException(
                $"'{value}' is not a valid dot-separated camelCase Semantic Part name.",
                parameterName);
        }
    }

    private static void ValidatePartPath(string value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        if (value.Split('.').Any(static segment => !IsCamelCaseSegment(segment)))
        {
            throw new ArgumentException(
                $"'{value}' is not a valid Semantic Part path.",
                parameterName);
        }
    }

    private static bool IsCamelCaseSegment(string value)
    {
        if (value.Length == 0 || value[0] is < 'a' or > 'z')
        {
            return false;
        }

        return value.Skip(1).All(static character =>
            character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9');
    }

    private static void ValidateSelectorClass(string? value, string parameterName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, parameterName);
        const string prefix = "semantic-";
        if (!value!.StartsWith(prefix, StringComparison.Ordinal) || value.Length == prefix.Length)
        {
            throw new ArgumentException(
                $"'{value}' is not a valid Semantic Part selector class.",
                parameterName);
        }
        if (string.Equals(value, "semantic-root", StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "'semantic-root' is reserved because the root Semantic Part is implicit.",
                parameterName);
        }

        var previousWasHyphen = false;
        for (var index = prefix.Length; index < value.Length; index++)
        {
            var character = value[index];
            if (character == '-')
            {
                if (previousWasHyphen || index == value.Length - 1)
                {
                    throw new ArgumentException(
                        $"'{value}' is not a valid Semantic Part selector class.",
                        parameterName);
                }
                previousWasHyphen = true;
                continue;
            }

            if (character is not (>= 'a' and <= 'z') and not (>= '0' and <= '9'))
            {
                throw new ArgumentException(
                    $"'{value}' is not a valid Semantic Part selector class.",
                    parameterName);
            }
            previousWasHyphen = false;
        }
    }

    private static string NormalizeSelectorRoute(
        string? value,
        string selectorClass,
        bool runtimeCreated,
        string parameterName)
    {
        if (value is null)
        {
            if (runtimeCreated)
            {
                throw new ArgumentException(
                    "Runtime-created Semantic Parts must declare an explicit SelectorRoute.",
                    parameterName);
            }

            return $"/template/ .{selectorClass}";
        }

        if (!IsValidSelectorRoute(value, selectorClass))
        {
            throw new ArgumentException(
                $"'{value}' is not a valid owner-relative Semantic Part selector route for '{selectorClass}'.",
                parameterName);
        }

        return value;
    }

    private static bool IsValidSelectorRoute(string value, string selectorClass)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        var tokens = value.Split(' ');
        if (tokens.Length < 2)
        {
            return false;
        }

        // 可选的首段自锚点：owner 节点自身携带的过滤类（方向限定等场景）。
        var index = 0;
        if (tokens[0].StartsWith(".", StringComparison.Ordinal))
        {
            if (!IsValidRouteClassToken(tokens[0]))
            {
                return false;
            }

            index = 1;
        }

        if ((tokens.Length - index) < 2 || (tokens.Length - index) % 2 != 0)
        {
            return false;
        }

        for (; index < tokens.Length; index += 2)
        {
            if (tokens[index] is not ("/template/" or ">" or ">>"))
            {
                return false;
            }

            if (!IsValidRouteClassToken(tokens[index + 1]))
            {
                return false;
            }
        }

        return string.Equals(tokens[^1], $".{selectorClass}", StringComparison.Ordinal);
    }

    private static bool IsValidRouteClassToken(string token)
    {
        if (token.Length < 2 || token[0] != '.')
        {
            return false;
        }

        try
        {
            ValidateSelectorClass(token.Substring(1), nameof(token));
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }
}
