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

   private double _toolTipArrowSize;

   private static readonly DirectProperty<ToolTip, double> ToolTipArrowSizeTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, double>(nameof(_toolTipArrowSize),
                                                         (o) => o._toolTipArrowSize,
                                                         (o, v) => o._toolTipArrowSize = v);
   
   private double _marginXXS;

   private static readonly DirectProperty<ToolTip, double> MarginXXSTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, double>(nameof(_marginXXS),
                                                         (o) => o._marginXXS,
                                                         (o, v) => o._marginXXS = v);

   // 组件的 Token 绑定属性
}