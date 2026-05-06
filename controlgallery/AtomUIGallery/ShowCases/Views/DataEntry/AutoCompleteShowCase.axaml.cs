using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class AutoCompleteShowCase : ReactiveUserControl<AutoCompleteViewModel>
{
    public AutoCompleteShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is AutoCompleteViewModel viewModel)
            {
                viewModel.BasicOptionsAsyncLoader       = new BasicOptionsAsyncLoader();
                viewModel.CustomLabelOptionsAsyncLoader = new CustomLabelOptionsAsyncLoader();
                viewModel.SearchEditOptionsAsyncLoader  = new SearchEditOptionsAsyncLoader();
                InitFilterCaseOptions(viewModel);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.BasicAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.CustomLabelOptionsAsyncLoader,
                        v => v.CustomizedAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.SearchEditOptionsAsyncLoader,
                        v => v.SearchAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.TextAreaAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.FilterCaseOptions,
                        v => v.FilterAutoComplete.OptionsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.ErrorAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.WarningAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.OutlineAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.FilledAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.BorderlessAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.UnderlinedAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.UnClearableAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                this.OneWayBind(viewModel, vm => vm.BasicOptionsAsyncLoader,
                        v => v.ClearableAutoComplete.OptionsAsyncLoader)
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    viewModel.BasicOptionsAsyncLoader       = null;
                    viewModel.CustomLabelOptionsAsyncLoader = null;
                    viewModel.SearchEditOptionsAsyncLoader  = null;
                    viewModel.FilterCaseOptions             = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }

    private void InitFilterCaseOptions(AutoCompleteViewModel vm)
    {
        vm.FilterCaseOptions =
        [
            new AutoCompleteOption()
            {
                Header  = "Burns Bay Road",
                Content = "Burns Bay Road"
            },
            new AutoCompleteOption()
            {
                Header  = "Downing Street",
                Content = "Downing Street"
            },
            new AutoCompleteOption()
            {
                Header  = "Wall Street",
                Content = "Wall Street"
            }
        ];
    }
}
