using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class ProgressBar
{
   // 获取 Token 值属性开始
   protected static readonly DirectProperty<ProgressBar, IBrush?> ColorTextLabelTokenProperty =
      AvaloniaProperty.RegisterDirect<ProgressBar, IBrush?>(nameof(ColorTextLabel),
         o => o.ColorTextLabel,
         (o, v) => o.ColorTextLabel = v);

   protected IBrush? _colorTextLabel;
   protected IBrush? ColorTextLabel
   {
      get => _colorTextLabel;
      set => SetAndRaise(ColorTextLabelTokenProperty, ref _colorTextLabel, value);
   }
   
   protected static readonly DirectProperty<ProgressBar, IBrush?> ColorTextLightSolidTokenProperty =
      AvaloniaProperty.RegisterDirect<ProgressBar, IBrush?>(nameof(ColorTextLightSolid),
                                                            o => o.ColorTextLightSolid,
                                                            (o, v) => o.ColorTextLightSolid = v);

   protected IBrush? _colorTextLightSolid;
   protected IBrush? ColorTextLightSolid
   {
      get => _colorTextLightSolid;
      set => SetAndRaise(ColorTextLightSolidTokenProperty, ref _colorTextLightSolid, value);
   }

   protected IBrush? GrooveBrush
   {
      get => GetValue(GrooveBrushProperty);
      set => SetValue(GrooveBrushProperty, value);
   }
   // 获取 Token 值属性结束
}