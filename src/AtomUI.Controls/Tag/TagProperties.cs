using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class Tag
{
   // 组件的 Token 绑定属性
   
   private double _paddingXXSToken;
   private static readonly DirectProperty<Tag, double> PaddingXXSTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, double>(nameof(_paddingXXSToken),
         (o) => o._paddingXXSToken,
         (o, v) => o._paddingXXSToken = v);
   
   private IBrush? _colorTextLightSolidToken;
   private static readonly DirectProperty<Tag, IBrush?> ColorTextLightSolidTokenProperty
      = AvaloniaProperty.RegisterDirect<Tag, IBrush?>(nameof(_colorTextLightSolidToken),
         (o) => o._colorTextLightSolidToken,
         (o, v) => o._colorTextLightSolidToken = v);
   
   // 组件的 Token 绑定属性结束
}