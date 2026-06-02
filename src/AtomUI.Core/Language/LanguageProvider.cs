using System.Collections.Concurrent;
using System.Reflection;
using AtomUI.Theme;
using Avalonia.Controls;
using Avalonia.Logging;

namespace AtomUI.Theme.Language;

public abstract class LanguageProvider : ILanguageProvider
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, FieldInfo>> s_languageFieldsByType = new();

    public LanguageCode LangCode { get; }

    public string LangId { get; }

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

    public void BuildResourceDictionary(IResourceDictionary dictionary)
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
            var logger = Logger.TryGet(LogEventLevel.Error, AtomUILogArea.Theme);
            logger?.Log(this, $"Build Resource for Language {resourceKindType.FullName} error.");
            throw;
        }
    }

    protected abstract Type GetResourceKindType();

    private static IReadOnlyDictionary<string, FieldInfo> GetLanguageFieldMap(Type type)
    {
        return s_languageFieldsByType.GetOrAdd(type, static languageType =>
        {
            var fields = languageType.GetFields(BindingFlags.Public |
                                                BindingFlags.Static |
                                                BindingFlags.FlattenHierarchy);
            var fieldMap = new Dictionary<string, FieldInfo>(fields.Length);
            foreach (var field in fields)
            {
                fieldMap[field.Name] = field;
            }

            return fieldMap;
        });
    }
}
