using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Slider;

public partial class SliderShowCase : GalleryReactiveUserControl<SliderViewModel>
{
    public const string LanguageId = nameof(SliderShowCase);

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

                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider1, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider2, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider3, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider4, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider5, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider6, AtomUI.Desktop.Controls.Slider.MarksProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(SliderViewModel.SliderMarks), vm => vm.SliderMarks,
                                           Slider7, AtomUI.Desktop.Controls.Slider.MarksProperty)
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
