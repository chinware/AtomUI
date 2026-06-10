using System.Diagnostics.CodeAnalysis;
using AtomUI.Controls;

namespace AtomUI.Icons.AntDesign;

public partial class AntDesignIconProvider : IconProvider<AntDesignIconKind>
{
    public AntDesignIconProvider()
    {
    }

    public AntDesignIconProvider(AntDesignIconKind kind)
        : base(kind)
    {
    }

    protected override Icon GetIcon(AntDesignIconKind kind)
    {
        try
        {
            return CreateIcon(kind);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Create icon {kind} failed", ex);
        }
    }

    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    protected override Type GetTypeForKind(AntDesignIconKind kind)
    {
        return GetIconType(kind);
    }
}
