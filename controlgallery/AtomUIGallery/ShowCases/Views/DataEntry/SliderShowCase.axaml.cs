using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class SliderShowCase : ReactiveUserControl<SliderViewModel>
{
    public SliderShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is SliderViewModel viewModel)
            {
                var marks = new List<SliderMark>();
                marks.Add(new SliderMark("0°C", 0));
                marks.Add(new SliderMark("26°C", 26));
                marks.Add(new SliderMark("37°C", 37));
                marks.Add(new SliderMark("100°C", 100)
                {
                    LabelFontWeight = FontWeight.Bold,
                    LabelBrush      = new SolidColorBrush(Colors.Red)
                });
                viewModel.SliderMarks = marks;
                
                this.OneWayBind(ViewModel, vm => vm.SliderMarks, v => v.Slider1.Marks)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SliderMarks, v => v.Slider2.Marks)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SliderMarks, v => v.Slider3.Marks)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SliderMarks, v => v.Slider4.Marks)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SliderMarks, v => v.Slider5.Marks)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SliderMarks, v => v.Slider6.Marks)
                    .DisposeWith(disposables);
                this.OneWayBind(ViewModel, vm => vm.SliderMarks, v => v.Slider7.Marks)
                    .DisposeWith(disposables);
                
                Disposable.Create(() =>
                {
                    viewModel.SliderMarks = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
}