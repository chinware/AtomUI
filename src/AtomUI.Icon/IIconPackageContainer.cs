namespace AtomUI.Icon;

public interface IIconPackageContainer
{
    public IIconPackageContainer Register(IIconPackageProvider iconPackageProvider);
    public IIconPackageContainer Register<TIconProvider>() where TIconProvider : IIconPackageProvider, new();

    public TIconPackageProvider? GetIconProvider<TIconPackageProvider>(string? id)
        where TIconPackageProvider : IIconPackageProvider, new();

    public IIconPackageProvider? GetIconProvider(string? id);
}