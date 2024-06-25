using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

public partial class ToolTip
{
   private static StyledProperty<ThemeVariant?> RequestedThemeVariantProperty;

   // 组件的 Token 绑定属性
   private IBrush? _defaultBackground;

   private static readonly DirectProperty<ToolTip, IBrush?> DefaultBgTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, IBrush?>(nameof(_defaultBackground),
                                                          (o) => o._defaultBackground,
                                                          (o, v) => o._defaultBackground = v);

   private double _sizePopupArrow;

   private static readonly DirectProperty<ToolTip, double> SizePopupArrowTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, double>(nameof(_sizePopupArrow),
                                                         (o) => o._sizePopupArrow,
                                                         (o, v) => o._sizePopupArrow = v);

   // 组件的 Token 绑定属性
}