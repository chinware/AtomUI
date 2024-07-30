using AtomUI.Controls;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class SliderShowCase : UserControl
{
   
   public static readonly StyledProperty<AvaloniaList<SliderMark>?> SliderMarksProperty =
      AvaloniaProperty.Register<SliderShowCase, AvaloniaList<SliderMark>?>(nameof(SliderMarks));
   
   public static readonly StyledProperty<bool> NormalDisabledProperty =
      AvaloniaProperty.Register<SliderShowCase, bool>(nameof(NormalEnabled), true);
   
   public AvaloniaList<SliderMark>? SliderMarks {
      get => GetValue(SliderMarksProperty); 
      set => SetValue(SliderMarksProperty, value);
   }
   
   public bool NormalEnabled {
      get => GetValue(NormalDisabledProperty); 
      set => SetValue(NormalDisabledProperty, value);
   }
   
   public SliderShowCase()
   {
      InitializeComponent();
      SliderMarks = new AvaloniaList<SliderMark>();
      SliderMarks.Add(new SliderMark("0°C", 0));
      SliderMarks.Add(new SliderMark("26°C", 26));
      SliderMarks.Add(new SliderMark("37°C", 37));
      SliderMarks.Add(new SliderMark("100°C", 100)
      {
         LabelFontWeight = FontWeight.Bold,
         LabelBrush = new SolidColorBrush(Colors.Red)
      });
      DataContext = this;
   }
   
}