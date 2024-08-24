using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class ButtonSpinnerShowCase : UserControl
{
   public ButtonSpinnerShowCase()
   {
      InitializeComponent();
   }
   
   public void HandleSpin(object sender, SpinEventArgs e)
   {
      var spinner = (ButtonSpinner)sender;

      if (spinner.Content is TextBlock textBlock)
      {
         int value = Array.IndexOf(_spinnerItems, textBlock.Text);
         if (e.Direction == SpinDirection.Increase) {
            value++;
         } else {
            value--;
         }

         if (value < 0) {
            value = _spinnerItems.Length - 1;
         } else if (value >= _spinnerItems.Length) {
            value = 0;
         }
         textBlock.Text = _spinnerItems[value];
      }

   }
   
   private readonly string[] _spinnerItems = new[]
   {
      "床前明月光",
      "疑是地上霜",
      "举头望明月",
      "低头思故乡"
   };
}