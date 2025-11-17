using System.Reflection;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Theme.TokenSystem;

public abstract class AbstractDesignToken : IDesignToken
{
    private readonly Dictionary<string, object?> _tokenAccessCache = new Dictionary<string, object?>();
    private static readonly Dictionary<Type, ITokenValueConverter> _valueConverters;

    static AbstractDesignToken()
    {
        _valueConverters = new Dictionary<Type, ITokenValueConverter>();
        var allTypes = Assembly.GetExecutingAssembly().GetTypes();
        var controlTokenTypes = allTypes.Where(type =>
            type.IsDefined(typeof(TokenValueConverterAttribute)) &&
            typeof(ITokenValueConverter).IsAssignableFrom(type));
        var valueConverters = controlTokenTypes.Select(type => (ITokenValueConverter)Activator.CreateInstance(type)!);
        foreach (var valueConverter in valueConverters)
        {
            _valueConverters.Add(valueConverter.TargetType(), valueConverter);
        }
    }
    
    internal virtual void LoadConfig(IDictionary<string, string> tokenConfigInfo)
    {
        try
        {
            if (tokenConfigInfo.Count > 0)
            {
                var type = GetType();
                var tokenPropertyMap =
                    type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                        .ToDictionary(p => p.Name);
                foreach (var tokenInfo in tokenConfigInfo)
                {
                    var tokenName = tokenInfo.Key;
                    if (!tokenPropertyMap.ContainsKey(tokenInfo.Key))
                    {
                        var logger = Logger.TryGet(LogEventLevel.Warning, AtomUILogArea.Theme);
                        logger?.Log(this, $"Token property: '{tokenInfo.Key}' found in token {type.Name}.'");
                        continue;
                    }
                    var property     = tokenPropertyMap[tokenName];
                    var propertyType = property.PropertyType;
                    if (_valueConverters.ContainsKey(propertyType))
                    {
                        property.SetValue(this, _valueConverters[propertyType].Convert(tokenConfigInfo[tokenName]));
                    }
                    else
                    {
                        try
                        {
                            property.SetValue(tokenName, tokenConfigInfo[tokenName]);
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
        var type = GetType();
        // internal 这里也考虑进去，还是具体的 Token 自己处理？
        var tokenProperties = type.GetProperties(BindingFlags.Public |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.Instance |
                                                 BindingFlags.FlattenHierarchy);
        var tokenResourceNamespace = GetTokenResourceCatalog();
        foreach (var property in tokenProperties)
        {
            var tokenName  = property.Name;
            var tokenValue = property.GetValue(this);
            if ((property.PropertyType == typeof(Color) || property.PropertyType == typeof(Color?)) && 
                tokenValue is not null)
            {
                tokenValue = new ImmutableSolidColorBrush((Color)tokenValue);
            }

            dictionary[new TokenResourceKey(tokenName, tokenResourceNamespace)] = tokenValue;
        }
    }

    private string? GetTokenResourceCatalog()
    {
        var tokenType                   = GetType();
        var globalDesignTokenAttributes = tokenType.GetCustomAttributes<GlobalDesignTokenAttribute>();
        if (globalDesignTokenAttributes.Any())
        {
            return null;
        }

        var controlDesignTokenAttribute = tokenType.GetCustomAttribute<ControlDesignTokenAttribute>();
        if (controlDesignTokenAttribute is not null)
        {
            return controlDesignTokenAttribute.ResourceCatalog;
        }

        throw new TokenResourceRegisterException(
            $"The current Token: {tokenType.FullName} lacks the token type annotation");
    }

    public virtual object? GetTokenValue(string name)
    {
        if (_tokenAccessCache.ContainsKey(name))
        {
            return _tokenAccessCache[name];
        }

        var type = GetType();
        var tokenProperty =
            type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (tokenProperty is null)
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
        var type = GetType();
        var tokenProperty =
            type.GetProperty(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (tokenProperty is null)
        {
            return;
        }

        _tokenAccessCache.Remove(name);
        tokenProperty.SetValue(this, value);
    }

    public virtual AbstractDesignToken Clone()
    {
        var type = GetType();
        var tokenProperties =
            type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        var cloned        = (AbstractDesignToken)Activator.CreateInstance(type)!;

        foreach (var property in tokenProperties)
        {
            property.SetValue(cloned, property.GetValue(this));
        }

        return cloned;
    }
}