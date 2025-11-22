using System.Diagnostics;
using System.Reflection;
using AtomUI.Controls;

namespace AtomUIGallery.Icons.Desktop;

public class GalleryDesktopIconProvider : IconProvider<DesktopIconKind>
{
    public GalleryDesktopIconProvider()
    {
    }

    public GalleryDesktopIconProvider(DesktopIconKind kind)
        : base(kind)
    {
    }

    protected override Icon GetIcon(DesktopIconKind kind)
    {
        try
        {
            var fullTypeName = $"AtomUIGallery.Icons.Desktop.{kind.ToString()}";
            var type = Type.GetType(fullTypeName)
                       ?? Assembly.GetExecutingAssembly().GetType(fullTypeName);

            if (type == null)
            {
                throw new InvalidOperationException($"{fullTypeName} not exist");
            }

            var icon = (Icon?)Activator.CreateInstance(type);
            Debug.Assert(icon != null);
            return icon;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"create icon {kind.ToString()} failed", ex);
        }
    }
}