using AtomUI.Demo.Desktop.ViewModels;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class CheckBoxShowCase : UserControl
{
   public CheckBoxShowCase()
   {
      DataContext = new CheckBoxShowCaseModel();
      InitializeComponent();
   }
}