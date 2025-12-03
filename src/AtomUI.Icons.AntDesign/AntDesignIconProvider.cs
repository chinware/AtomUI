using System.Reflection;
using AtomUI.Controls;

namespace AtomUI.Icons.AntDesign;

public class AntDesignIconProvider : IconProvider<AntDesignIconKind>
{
    public AntDesignIconProvider()
    {}
    
    public AntDesignIconProvider(AntDesignIconKind kind)
        : base(kind)
    {
    }
    
    protected override Type GetTypeForKind(AntDesignIconKind kind)
    {
        var typeName = $"AtomUI.Icons.AntDesign.{kind.ToString()}";
        
        var type = Type.GetType(typeName) 
                   ?? Assembly.GetExecutingAssembly().GetType(typeName);
        if (type == null)
        {
            throw new InvalidOperationException($"Type {typeName} does not exist");
        }
        return type;
    }
}