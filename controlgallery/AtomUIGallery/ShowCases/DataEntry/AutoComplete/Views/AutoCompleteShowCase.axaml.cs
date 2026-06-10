using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.AutoComplete;

public partial class AutoCompleteShowCase : GalleryReactiveUserControl<AutoCompleteViewModel>
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

                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, BasicAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.CustomLabelOptionsAsyncLoader),
                                           vm => vm.CustomLabelOptionsAsyncLoader, CustomizedAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.SearchEditOptionsAsyncLoader),
                                           vm => vm.SearchEditOptionsAsyncLoader, SearchAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, TextAreaAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.FilterCaseOptions),
                                           vm => vm.FilterCaseOptions, FilterAutoComplete,
                                           AbstractAutoComplete.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.CityOptions),
                                           vm => vm.CityOptions, CityAutoComplete,
                                           AbstractAutoComplete.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, ErrorAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, WarningAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, OutlineAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, FilledAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, BorderlessAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, UnderlinedAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, UnClearableAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(AutoCompleteViewModel.BasicOptionsAsyncLoader),
                                           vm => vm.BasicOptionsAsyncLoader, ClearableAutoComplete,
                                           AbstractAutoComplete.OptionsAsyncLoaderProperty)
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
