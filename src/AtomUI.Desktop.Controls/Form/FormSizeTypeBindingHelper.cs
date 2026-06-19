using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal static class FormSizeTypeBindingHelper
{
    public static IDisposable RelaySizeType(
        AvaloniaObject source,
        AvaloniaProperty<CustomizableSizeType> sourceProperty,
        Control target)
    {
        if (target is ICustomizableSizeTypeAware)
        {
            return BindUtils.RelayBind(source, sourceProperty, target, sourceProperty);
        }

        if (target is ISizeTypeAware)
        {
            return BindUtils.RelayBind(
                source,
                sourceProperty,
                target,
                SizeTypeControlProperty.SizeTypeProperty,
                ToLegacySizeType);
        }

        return Disposable.Empty;
    }

    private static SizeType ToLegacySizeType(CustomizableSizeType sizeType)
    {
        return sizeType switch
        {
            CustomizableSizeType.Large => SizeType.Large,
            CustomizableSizeType.Small => SizeType.Small,
            _                           => SizeType.Middle
        };
    }
}
