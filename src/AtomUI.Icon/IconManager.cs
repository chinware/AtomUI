namespace AtomUI.Icon;

public class IconManager : IIconPackageContainer, IPackageAwareIconReader
{
    private readonly Dictionary<string, IIconPackageProvider> _iconPackageProviders = new();
    private string? _defaultPackage;

    protected IconManager()
    {
    }

    public static IconManager Current { get; } = new();

    public string? DefaultPackage
    {
        get => _defaultPackage;

        set
        {
            if (value != null)
            {
                if (!_iconPackageProviders.ContainsKey(value))
                {
                    throw new ArgumentException($"PathIcon package {value} is not registered.");
                }

                _defaultPackage = value;
            }
        }
    }

    public IIconPackageContainer Register(IIconPackageProvider iconPackageProvider)
    {
        var packageId = iconPackageProvider.Id;
        if (!_iconPackageProviders.TryAdd(packageId, iconPackageProvider))
        {
            throw new ArgumentException($"\"{packageId}\" is already registered.");
        }

        return this;
    }

    public IIconPackageContainer Register<TIconProvider>() where TIconProvider : IIconPackageProvider, new()
    {
        return Register(new TIconProvider());
    }

    public TIconPackageProvider? GetIconProvider<TIconPackageProvider>(string? id = null)
        where TIconPackageProvider : IIconPackageProvider, new()
    {
        return (TIconPackageProvider?)GetIconProvider(id);
    }

    public IIconPackageProvider? GetIconProvider(string? id = null)
    {
        if (id is null)
        {
            id = DefaultPackage;
        }

        if (id is null || !_iconPackageProviders.ContainsKey(id))
        {
            return default;
        }

        return _iconPackageProviders[id];
    }

    public IconInfo? GetIcon(string iconKind)
    {
        if (_defaultPackage != null)
        {
            return GetIcon(_defaultPackage, iconKind);
        }

        // 顺序搜索，是否增加个优先级？
        var iconPackageProviders = _iconPackageProviders.Values.OrderByDescending(provider => provider.Priority);
        foreach (var provider in iconPackageProviders)
        {
            var iconInfo = provider.GetIcon(iconKind);
            if (iconInfo != null)
            {
                return iconInfo;
            }
        }

        return null;
    }

    public IconInfo? GetIcon(string iconKind, ColorInfo colorInfo)
    {
        if (_defaultPackage != null)
        {
            return GetIcon(_defaultPackage, iconKind, colorInfo);
        }

        // 顺序搜索，是否增加个优先级？
        var iconPackageProviders = _iconPackageProviders.Values.OrderByDescending(provider => provider.Priority);
        foreach (var provider in iconPackageProviders)
        {
            var iconInfo = provider.GetIcon(iconKind, colorInfo);
            if (iconInfo != null)
            {
                return iconInfo;
            }
        }

        return null;
    }

    public IconInfo? GetIcon(string iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        if (_defaultPackage != null)
        {
            return GetIcon(_defaultPackage, iconKind, twoToneColorInfo);
        }

        var iconPackageProviders = _iconPackageProviders.Values.OrderByDescending(provider => provider.Priority);
        foreach (var provider in iconPackageProviders)
        {
            var iconInfo = provider.GetIcon(iconKind, twoToneColorInfo);
            if (iconInfo != null)
            {
                return iconInfo;
            }
        }

        return null;
    }

    public IconInfo? GetIcon(string package, string iconKind)
    {
        if (!_iconPackageProviders.ContainsKey(package))
        {
            return null;
        }

        var packageProvider = _iconPackageProviders[package];
        return packageProvider.GetIcon(iconKind);
    }

    public IconInfo? GetIcon(string package, string iconKind, ColorInfo colorInfo)
    {
        if (!_iconPackageProviders.ContainsKey(package))
        {
            return null;
        }

        var packageProvider = _iconPackageProviders[package];
        return packageProvider.GetIcon(iconKind, colorInfo);
    }

    public IconInfo? GetIcon(string package, string iconKind, TwoToneColorInfo twoToneColorInfo)
    {
        if (!_iconPackageProviders.ContainsKey(package))
        {
            return null;
        }

        var packageProvider = _iconPackageProviders[package];
        return packageProvider.GetIcon(iconKind, twoToneColorInfo);
    }
}