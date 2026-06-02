using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.AutoComplete;

public partial class AutoCompleteShowCase : ReactiveUserControl<AutoCompleteViewModel>
{
    public const string LanguageId = nameof(AutoCompleteShowCase);

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
                InitCityOptions(viewModel);

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

                this.OneWayBind(viewModel, vm => vm.CityOptions,
                        v => v.CityAutoComplete.OptionsSource)
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
                    viewModel.CityOptions                   = null;
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

    private void InitCityOptions(AutoCompleteViewModel vm)
    {
        vm.CityOptions =
        [
            new CityAutoCompleteOption { Header = "Amsterdam", Content = "Amsterdam", Country = "NL", Population = 905234 },
            new CityAutoCompleteOption { Header = "Auckland",  Content = "Auckland",  Country = "NZ", Population = 1657200 },
            new CityAutoCompleteOption { Header = "Beijing",   Content = "Beijing",   Country = "CN", Population = 21893095 },
            new CityAutoCompleteOption { Header = "Berlin",    Content = "Berlin",    Country = "DE", Population = 3677472 },
            new CityAutoCompleteOption { Header = "Boston",    Content = "Boston",    Country = "US", Population = 654776 },
            new CityAutoCompleteOption { Header = "Jakarta",   Content = "Jakarta",   Country = "ID", Population = 10770487 },
            new CityAutoCompleteOption { Header = "Lisbon",    Content = "Lisbon",    Country = "PT", Population = 545923 }
        ];
    }
}
