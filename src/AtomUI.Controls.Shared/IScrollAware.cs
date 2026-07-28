using Avalonia;

namespace AtomUI.Controls;

public interface IScrollAwareControl
{
    bool IsScrollEnabled { get; set; }
}

public abstract class ScrollAwareControlProperty
{
    public const string IsScrollEnabledPropertyName = "IsScrollEnabled";

    public static readonly StyledProperty<bool> IsScrollEnabledProperty =
        AvaloniaProperty.Register<StyledElement, bool>(
            IsScrollEnabledPropertyName,
            defaultValue: true,
            inherits: true);
}
