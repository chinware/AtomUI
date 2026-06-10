using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Theme.TokenSystem;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                            DynamicallyAccessedMemberTypes.PublicProperties |
                            DynamicallyAccessedMemberTypes.NonPublicProperties)]
public abstract class AbstractDesignToken : IDesignToken
{
    private readonly Dictionary<string, object?> _tokenAccessCache = new Dictionary<string, object?>();
    private static readonly Dictionary<Type, ITokenValueConverter> _valueConverters;
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> s_tokenPropertiesByType = new();
    private static readonly ConcurrentDictionary<Type, IReadOnlyDictionary<string, PropertyInfo>> s_tokenPropertyMapsByType = new();
    private const BindingFlags TokenPropertyFlags = BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance |
                                                    BindingFlags.FlattenHierarchy;

    static AbstractDesignToken()
    {
        _valueConverters = TokenValueConverterRegistry.Create();
    }
    
    internal virtual void LoadConfig(IDictionary<string, string> tokenConfigInfo)
    {
        try
        {
            if (tokenConfigInfo.Count > 0)
            {
                var type             = GetType();
                var tokenPropertyMap = GetTokenPropertyMap(type);
                foreach (var tokenInfo in tokenConfigInfo)
                {
                    var tokenName = tokenInfo.Key;
                    if (!tokenPropertyMap.TryGetValue(tokenName, out var property))
                    {
                        var logger = Logger.TryGet(LogEventLevel.Warning, AtomUILogArea.Theme);
                        logger?.Log(this, $"Token property: '{tokenName}' found in token {type.Name}.'");
                        continue;
                    }
                    var propertyType = property.PropertyType;
                    if (_valueConverters.TryGetValue(propertyType, out var valueConverter))
                    {
                        property.SetValue(this, valueConverter.Convert(tokenInfo.Value));
                    }
                    else
                    {
                        try
                        {
                            property.SetValue(this, tokenInfo.Value);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Unable to set token property: {tokenName}, maybe value type mismatch.", ex);
                        }
                    }
                }
            }
        }
        catch (Exception exception)
        {
            throw new ThemeLoadException("Load theme design token failed.", exception);
        }
    }

    public virtual void BuildResourceDictionary(IResourceDictionary dictionary)
    {
        var type            = GetType();
        var tokenProperties = GetTokenPropertyMap(type);
            
        foreach (var entry in ThemeResourceKeyCache.GetEnumEntries(typeof(SharedTokenKind)))
        {
            if (tokenProperties.TryGetValue(entry.Name, out var property))
            {
                var tokenValue = property.GetValue(this);
                if ((property.PropertyType == typeof(Color) || property.PropertyType == typeof(Color?)) && 
                    tokenValue is not null)
                {
                    tokenValue = new ImmutableSolidColorBrush((Color)tokenValue);
                }
                dictionary[entry.Value] = tokenValue;
            }
            else
            {
                throw new Exception($"Token: {entry.Name} does not exist in {type.FullName}");
            }
        }
    }

    public virtual object? GetTokenValue(string name)
    {
        if (_tokenAccessCache.TryGetValue(name, out var cachedTokenValue))
        {
            return cachedTokenValue;
        }

        var tokenPropertyMap = GetTokenPropertyMap(GetType());
        if (!tokenPropertyMap.TryGetValue(name, out var tokenProperty))
        {
            _tokenAccessCache[name] = null;
            return null;
        }

        var tokenValue = tokenProperty.GetValue(this);
        _tokenAccessCache[name] = tokenValue;
        return tokenValue;
    }

    public virtual void SetTokenValue(string name, object value)
    {
        var tokenPropertyMap = GetTokenPropertyMap(GetType());
        if (!tokenPropertyMap.TryGetValue(name, out var tokenProperty))
        {
            return;
        }

        _tokenAccessCache.Remove(name);
        tokenProperty.SetValue(this, value);
    }

    public virtual AbstractDesignToken Clone()
    {
        var type            = GetType();
        var tokenProperties = GetTokenProperties(type);
        var cloned        = (AbstractDesignToken)Activator.CreateInstance(type)!;

        foreach (var property in tokenProperties)
        {
            property.SetValue(cloned, property.GetValue(this));
        }

        return cloned;
    }

    protected static PropertyInfo[] GetTokenProperties(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type type)
    {
        if (s_tokenPropertiesByType.TryGetValue(type, out var cachedProperties))
        {
            return cachedProperties;
        }

        var tokenProperties = type.GetProperties(TokenPropertyFlags);
        return s_tokenPropertiesByType.GetOrAdd(type, tokenProperties);
    }

    protected static IReadOnlyDictionary<string, PropertyInfo> GetTokenPropertyMap(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type type)
    {
        if (s_tokenPropertyMapsByType.TryGetValue(type, out var cachedMap))
        {
            return cachedMap;
        }

        var tokenProperties = GetTokenProperties(type);
        var propertyMap     = new Dictionary<string, PropertyInfo>(tokenProperties.Length);
        foreach (var property in tokenProperties)
        {
            propertyMap[property.Name] = property;
        }

        return s_tokenPropertyMapsByType.GetOrAdd(type, propertyMap);
    }
}
