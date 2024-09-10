using Avalonia;

namespace AtomUI.Icon;

public static class IconPackageExtensions
{
    public static AppBuilder UseIconPackage<T>(this AppBuilder builder, bool isDefault = false)
        where T : IconPackage, new()
    {
        return builder.UseIconPackage<T>(isDefault, null, null);
    }

    public static AppBuilder UseIconPackage<T>(this AppBuilder builder, bool isDefault, ColorInfo? colorInfo,
                                               TwoToneColorInfo? twoToneColorInfo)
        where T : IconPackage, new()
    {
        builder.AfterSetup(builder =>
        {
            var package = new T();
            if (colorInfo.HasValue)
            {
                package.DefaultColorInfo = colorInfo.Value;
            }

            if (twoToneColorInfo.HasValue)
            {
                package.DefaultTwoToneColorInfo = twoToneColorInfo.Value;
            }

            IconManager.Current.Register(package);
            if (isDefault)
            {
                IconManager.Current.DefaultPackage = package.Id;
            }
        });
        return builder;
    }
}