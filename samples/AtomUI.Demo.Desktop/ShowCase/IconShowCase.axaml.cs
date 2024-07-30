using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using AtomUI.Demo.Desktop.ViewModels;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class IconShowCase : UserControl
{
   public IconShowCase()
   {
      InitializeComponent();
   }

   protected override async  void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      PaletteDemoViewModel vm = new PaletteDemoViewModel();
      await Dispatcher.UIThread.InvokeAsync(() =>
      {
         vm.InitializeResources();
      });
      DataContext = vm;
   }
}