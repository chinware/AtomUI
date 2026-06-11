namespace AtomUI.Generator.Language;

internal class LanguageInfo
{
    /// <summary>
    /// 生成的结果类枚举和标记扩展存放的命名空间
    /// </summary>
    public string? TargetNamespace { get; internal set; }
    public string Namespace { get; internal set; } = string.Empty;
    public string LanguageId { get; internal set; } = string.Empty;
    public string LanguageCode { get; internal set; } = string.Empty;
    public string ClassName { get; internal set; } = string.Empty;
    public string Accessibility { get; internal set; } = "internal";
    public bool IsPartial { get; internal set; }
    public bool HasParameterlessConstructor { get; internal set; }

    public Dictionary<string, string> Items { get; internal set; }

    public bool ShouldGenerateExplicitConstructor => IsPartial && !HasParameterlessConstructor;

    public string ProviderTypeFullName =>
        string.IsNullOrEmpty(Namespace) ? ClassName : $"{Namespace}.{ClassName}";

    public string ResourceKindTypeFullName =>
        $"{TargetNamespace ?? Namespace}.{LanguageId}LangResourceKind";

    public LanguageInfo()
    {
        Items = new Dictionary<string, string>();
    }
}
