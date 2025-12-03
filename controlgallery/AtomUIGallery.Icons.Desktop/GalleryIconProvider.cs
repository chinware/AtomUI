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
    
    protected override Type GetTypeForKind(DesktopIconKind kind)
    {
        var typeName =  $"AtomUIGallery.Icons.Desktop.{kind.ToString()}";
        
        var type = Type.GetType(typeName) 
                   ?? Assembly.GetExecutingAssembly().GetType(typeName);
        if (type == null)
        {
            throw new InvalidOperationException($"Type {typeName} does not exist");
        }
        return type;
    }
}