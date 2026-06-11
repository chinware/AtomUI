using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Theme;
using Avalonia.Controls;
using Avalonia.Logging;

namespace AtomUI.Theme.Language;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
public abstract class LanguageProvider : ILanguageProvider
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, FieldInfo>> s_languageFieldsByType = new();

    public LanguageCode LangCode { get; }

    public string LangId { get; }

    protected LanguageProvider(LanguageCode langCode, string langId)
    {
        LangCode = langCode;
        LangId   = langId;
    }

    [RequiresUnreferencedCode("Reflects over the runtime language provider type to read LanguageProviderAttribute.")]
    public LanguageProvider()
    {
        var type                      = GetType();
        var languageProviderAttribute = type.GetCustomAttribute<LanguageProviderAttribute>();
        if (languageProviderAttribute is null)
        {
            throw new LanguageMetaInfoParseException("No annotations found LanguageProviderAttribute");
        }

        LangCode        = languageProviderAttribute.LanguageCode;
        LangId          = languageProviderAttribute.LanguageId;
    }

    public virtual void BuildResourceDictionary(IResourceDictionary dictionary)
    {
        var type             = GetType();
        var resourceKindType = GetResourceKindType();
        try
        {
            var languageFields = GetLanguageFieldMap(type);
            foreach (var entry in ThemeResourceKeyCache.GetEnumEntries(resourceKindType))
            {
                if (languageFields.TryGetValue(entry.Name, out var field))
                {
                    var languageText = field.GetValue(null);
                    dictionary[entry.Value] = languageText;
                }
                else
                {
                    throw new Exception($"Language item: {entry.Name} does not exist in {type.FullName}");
                }
            }
        }
        catch (Exception)
        {
            LogBuildResourceDictionaryError(resourceKindType);
            throw;
        }
    }

    protected abstract Type GetResourceKindType();

    protected void LogBuildResourceDictionaryError(Type resourceKindType)
    {
        var logger = Logger.TryGet(LogEventLevel.Error, AtomUILogArea.Theme);
        logger?.Log(this, $"Build Resource for Language {resourceKindType.FullName} error.");
    }

    private static IReadOnlyDictionary<string, FieldInfo> GetLanguageFieldMap(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
        Type type)
    {
        if (s_languageFieldsByType.TryGetValue(type, out var cachedFields))
        {
            return cachedFields;
        }

        var fields = type.GetFields(BindingFlags.Public |
                                    BindingFlags.Static |
                                    BindingFlags.FlattenHierarchy);
        var fieldMap = new Dictionary<string, FieldInfo>(fields.Length);
        foreach (var field in fields)
        {
            fieldMap[field.Name] = field;
        }

        return s_languageFieldsByType.GetOrAdd(type, fieldMap);
    }
}
