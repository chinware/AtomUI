using System.Reflection;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

public abstract class AbstractDesignToken : IDesignToken
{
    private readonly IDictionary<string, object?> _tokenAccessCache;
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

    public AbstractDesignToken()
    {
        _tokenAccessCache = new Dictionary<string, object?>();
    }

    internal virtual void LoadConfig(IDictionary<string, string> tokenConfigInfo)
    {
        try
        {
            var type = GetType();
            var tokenProperties =
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            var baseTokenType = typeof(AbstractDesignToken);
            foreach (var property in tokenProperties)
            {
                if (baseTokenType.IsAssignableFrom(property.PropertyType))
                {
                    // 如果当前的属性是 Token 类型，证明是组合属性，跳过
                    continue;
                }

                var tokenName = property.Name;
                if (tokenConfigInfo.ContainsKey(tokenName))
                {
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
        var baseTokenType          = typeof(AbstractDesignToken);
        var tokenResourceNamespace = GetTokenResourceCatalog();
        foreach (var property in tokenProperties)
        {
            if (baseTokenType.IsAssignableFrom(property.PropertyType))
            {
                // 如果当前的属性是 Token 类型，证明是组合属性，跳过
                continue;
            }

            var tokenName  = property.Name;
            var tokenValue = property.GetValue(this);
            if ((property.PropertyType == typeof(Color) || property.PropertyType == typeof(Color?)) && 
                tokenValue is not null)
            {
                tokenValue = new SolidColorBrush((Color)tokenValue);
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
            type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
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
            type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
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
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        var baseTokenType = typeof(AbstractDesignToken);
        var cloned        = (AbstractDesignToken)Activator.CreateInstance(type)!;

        foreach (var property in tokenProperties)
        {
            if (baseTokenType.IsAssignableFrom(property.PropertyType))
            {
                var subToken = (AbstractDesignToken)property.GetValue(this)!;
                property.SetValue(cloned, subToken.Clone());
            }
            else
            {
                property.SetValue(cloned, property.GetValue(this));
            }
        }

        return cloned;
    }
}