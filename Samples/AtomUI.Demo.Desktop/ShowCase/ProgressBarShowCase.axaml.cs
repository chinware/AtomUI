using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class ProgressBarShowCase : UserControl
{
   public LinearGradientBrush TwoStopsGradientStrokeColor
   {
      get;
      set;
   }
   
   public LinearGradientBrush ThreeStopsGradientStrokeColor
   {
      get;
      set;
   }

   public List<IBrush> StepsChunkBrushes
   {
      get;
      set;
   }
   
   public ProgressBarShowCase()
   {
      InitializeComponent();
      DataContext = this;

      TwoStopsGradientStrokeColor = new LinearGradientBrush()
      {
         GradientStops = 
         {
            new GradientStop(Color.Parse("#108ee9"), 0),
            new GradientStop(Color.Parse("#87d068"), 1)
         }
      };
      ThreeStopsGradientStrokeColor = new LinearGradientBrush()
      {
         GradientStops =
         {
            new GradientStop(Color.Parse("#87d068"), 0),
            new GradientStop(Color.Parse("#ffe58f"), 0.5),
            new GradientStop(Color.Parse("#ffccc7"), 1)
         }
      };
      StepsChunkBrushes = new List<IBrush>()
      {
         new SolidColorBrush(Colors.Green),
         new SolidColorBrush(Colors.Green),
         new SolidColorBrush(Colors.Red)
      };
   }
   
}