using Microsoft.CodeAnalysis;

namespace AtomUI.Generator;

internal static class GeneratorSymbolDisplay
{
    internal static readonly SymbolDisplayFormat FullyQualifiedType =
        SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
            SymbolDisplayFormat.FullyQualifiedFormat.MiscellaneousOptions &
            ~SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
}

internal class ControlTokenInfo
{
    public string ControlNamespace { get; set; }
    public string ControlName { get; set; }
    public string? ControlId { get; set; }
    public string? ResourceCatalog { get; set; }
    public string TokenKindType => $"{ControlName}Kind";
    public HashSet<TokenName> Tokens { get; }
    public HashSet<SchemaTokenInfo> SchemaTokens { get; }
    public List<Diagnostic> Diagnostics { get; }
    public bool IsValid => Diagnostics.Count == 0 && !string.IsNullOrWhiteSpace(ControlId);

    public ControlTokenInfo(string ns, string controlName, HashSet<TokenName> tokens)
    {
        ControlNamespace = ns;
        ControlName = controlName;
        Tokens      = tokens;
        SchemaTokens = new HashSet<SchemaTokenInfo>();
        Diagnostics = new List<Diagnostic>();
    }

    public ControlTokenInfo()
        : this(string.Empty, string.Empty, new HashSet<TokenName>())
    {
    }

    public void AddToken(TokenName tokenName)
    {
        Tokens.Add(tokenName);
    }

    public string GetFullyQualifiedTypeName()
    {
        return string.IsNullOrWhiteSpace(ControlNamespace)
            ? ControlName
            : $"{ControlNamespace}.{ControlName}";
    }
}

internal sealed class GlobalTokenGenerationInfo
{
    public HashSet<TokenName> Tokens { get; } = new HashSet<TokenName>();
    public HashSet<SchemaTokenInfo> SchemaTokens { get; } = new HashSet<SchemaTokenInfo>();
}

internal class TokenInfo
{
    public HashSet<TokenName> Tokens { get; private set; }
    public HashSet<SchemaTokenInfo> SchemaTokens { get; private set; }
    public List<ControlTokenInfo> ControlTokenInfos { get; private set; }

    public TokenInfo()
    {
        Tokens            = new HashSet<TokenName>();
        SchemaTokens      = new HashSet<SchemaTokenInfo>();
        ControlTokenInfos = new List<ControlTokenInfo>();
    }
}

internal enum SchemaTokenStage
{
    Seed,
    Map,
    Alias,
    Control
}

internal sealed class SchemaTokenInfo : IEquatable<SchemaTokenInfo>
{
    internal SchemaTokenInfo(
        string name,
        string valueType,
        string declaringType,
        SchemaTokenStage stage)
    {
        Name = name;
        ValueType = valueType;
        DeclaringType = declaringType;
        Stage = stage;
    }

    public string Name { get; }
    public string ValueType { get; }
    public string DeclaringType { get; }
    public SchemaTokenStage Stage { get; }

    public bool Equals(SchemaTokenInfo? other)
    {
        return other is not null &&
               Name == other.Name &&
               ValueType == other.ValueType &&
               DeclaringType == other.DeclaringType &&
               Stage == other.Stage;
    }

    public override bool Equals(object? obj) => Equals(obj as SchemaTokenInfo);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = StringComparer.Ordinal.GetHashCode(Name);
            hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(ValueType);
            hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(DeclaringType);
            return (hash * 397) ^ (int)Stage;
        }
    }
}

internal record TokenName
{
    public string Name { get; }
    public string ResourceCatalog { get; }

    public TokenName(string name, string catalog)
    {
        Name            = name;
        ResourceCatalog = catalog;
    }
}
