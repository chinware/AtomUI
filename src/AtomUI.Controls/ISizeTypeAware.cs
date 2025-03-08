using Avalonia;

namespace AtomUI.Controls;

public interface ISizeTypeAware
{
    public SizeType SizeType { get; set; }
}

public abstract class SizeTypeAwareControlProperty : AvaloniaObject
{
    public const string SizeTypePropertyName = "SizeType";
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<SizeTypeAwareControlProperty, SizeType>(SizeTypePropertyName, SizeType.Middle);
}
