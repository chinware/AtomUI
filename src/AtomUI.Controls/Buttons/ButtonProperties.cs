using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class Button
{
   // Control token 值绑定属性
   // 影响大小的参数，一般不变化，变化了通过触发重新计算大小

   private double _controlHeight;
   private static readonly DirectProperty<Button, double> ControlHeightTokenProperty =
      AvaloniaProperty.RegisterDirect<Button, double>(
         nameof(_controlHeight),
         o => o._controlHeight,
         (o, v) => o._controlHeight = v);
   
   private double _paddingXXS;
   private static readonly DirectProperty<Button, double> PaddingXXSTokenProperty =
      AvaloniaProperty.RegisterDirect<Button, double>(
         nameof(_paddingXXS),
         o => o._paddingXXS,
         (o, v) => o._paddingXXS = v);
   
   private BoxShadow _defaultShadow;
   private static readonly DirectProperty<Button, BoxShadow> DefaultShadowTokenProperty =
      AvaloniaProperty.RegisterDirect<Button, BoxShadow>(
         nameof(_defaultShadow),
         o => o._defaultShadow,
         (o, v) => o._defaultShadow = v);
   
   private BoxShadow _primaryShadow;
   private static readonly DirectProperty<Button, BoxShadow> PrimaryShadowTokenProperty =
      AvaloniaProperty.RegisterDirect<Button, BoxShadow>(
         nameof(_primaryShadow),
         o => o._primaryShadow,
         (o, v) => o._primaryShadow = v);
   
   private BoxShadow _dangerShadow;
   private static readonly DirectProperty<Button, BoxShadow> DangerShadowTokenProperty =
      AvaloniaProperty.RegisterDirect<Button, BoxShadow>(
         nameof(_dangerShadow),
         o => o._dangerShadow,
         (o, v) => o._dangerShadow = v);
}