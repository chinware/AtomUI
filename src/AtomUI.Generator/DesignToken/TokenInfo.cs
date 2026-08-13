using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

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
    public string TokenNamespace { get; set; }
    public string TokenName { get; set; }
    public string? ControlName { get; set; }
    public HashSet<TokenName> Tokens { get; }
    public HashSet<SchemaTokenInfo> SchemaTokens { get; }
    public List<Diagnostic> Diagnostics { get; }
    public Location? DeclarationLocation { get; set; }
    public bool IsValid => Diagnostics.Count == 0 && !string.IsNullOrWhiteSpace(ControlName);

    public ControlTokenInfo(string ns, string tokenName, HashSet<TokenName> tokens)
    {
        TokenNamespace = ns;
        TokenName = tokenName;
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

    public string GetFullyQualifiedTokenTypeName()
    {
        return string.IsNullOrWhiteSpace(TokenNamespace)
            ? TokenName
            : $"{TokenNamespace}.{TokenName}";
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
    public HashSet<string> AvailableGlobalTokenNames { get; }
    public HashSet<SchemaTokenInfo> SchemaTokens { get; private set; }
    public List<ControlThemeInfo> ControlThemeInfos { get; private set; }

    public TokenInfo()
    {
        Tokens            = new HashSet<TokenName>();
        AvailableGlobalTokenNames = new HashSet<string>(StringComparer.Ordinal);
        SchemaTokens      = new HashSet<SchemaTokenInfo>();
        ControlThemeInfos = new List<ControlThemeInfo>();
    }
}

internal sealed class ThemeCompilationInfo
{
    internal ThemeCompilationInfo(
        Compilation compilation,
        string assemblyName,
        string packageId,
        string? projectDirectory,
        string controlCatalog,
        IReadOnlyList<string> globalTokenNames,
        AnalyzerConfigOptionsProvider optionsProvider,
        string registrationEntries)
    {
        Compilation = compilation;
        AssemblyName = assemblyName;
        PackageId = packageId;
        ProjectDirectory = projectDirectory;
        ControlCatalog = controlCatalog;
        GlobalTokenNames = globalTokenNames;
        OptionsProvider = optionsProvider;
        RegistrationEntries = registrationEntries;
    }

    internal Compilation Compilation { get; }
    internal string AssemblyName { get; }
    internal string PackageId { get; }
    internal string? ProjectDirectory { get; }
    internal string ControlCatalog { get; }
    internal IReadOnlyList<string> GlobalTokenNames { get; }
    internal AnalyzerConfigOptionsProvider OptionsProvider { get; }
    internal string RegistrationEntries { get; }
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
