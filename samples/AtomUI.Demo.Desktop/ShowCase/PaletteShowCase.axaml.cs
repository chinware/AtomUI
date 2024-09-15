using AtomUI.Demo.Desktop.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class PaletteShowCase : UserControl
{
    public PaletteShowCase()
    {
        InitializeComponent();
    }

    protected override async void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        var vm = new PaletteDemoViewModel();
        await Dispatcher.UIThread.InvokeAsync(() => { vm.InitializeResources(); });
        DataContext = vm;
    }
}