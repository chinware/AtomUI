using Avalonia;

namespace AtomUI.Controls;

public interface ISizeTypeAware
{
    public SizeType SizeType { get; set; }
}

public interface ICustomizableSizeTypeAware
{
    public CustomizableSizeType SizeType { get; set; }
}

public abstract class SizeTypeControlProperty
{
    public const string SizeTypePropertyName = "SizeType";
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        AvaloniaProperty.Register<StyledElement, SizeType>(SizeTypePropertyName, SizeType.Middle);
}

public abstract class CustomizableSizeTypeControlProperty
{
    public const string SizeTypePropertyName = "SizeType";
    public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
        AvaloniaProperty.Register<StyledElement, CustomizableSizeType>(SizeTypePropertyName, CustomizableSizeType.Middle);
}
