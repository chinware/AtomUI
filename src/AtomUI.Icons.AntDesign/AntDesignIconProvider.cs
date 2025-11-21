using System.Diagnostics;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Icons.AntDesign;

namespace AtomUI.Icons.AntDesign;

public class AntDesignIconProvider : IconProvider<AntDesignIconKind>
{
    public AntDesignIconProvider()
    {}
    
    public AntDesignIconProvider(AntDesignIconKind kind)
        : base(kind)
    {
    }
    
    protected override Icon GetIcon(AntDesignIconKind kind)
    {
        try
        {
            var fullTypeName = $"AtomUI.Icons.AntDesign.{kind.ToString()}";
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