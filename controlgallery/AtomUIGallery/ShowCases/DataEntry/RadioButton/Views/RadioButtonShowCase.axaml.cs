using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using Avalonia;
using AtomUIGallery.Localization;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.RadioButton;

public partial class RadioButtonShowCase : ReactiveUserControl<RadioButtonViewModel>
{
    public const string LanguageId = nameof(RadioButtonShowCase);

    public RadioButtonShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is RadioButtonViewModel viewModel)
            {
                ConfigureRadioOptions(viewModel);

                this.OneWayBind(ViewModel, vm => vm.RadioOptions, v => v.RadioButtonGroup.ItemsSource)
                    .DisposeWith(disposables);

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => ConfigureRadioOptions(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                        .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    viewModel.RadioOptions = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }

    private void ConfigureRadioOptions(RadioButtonViewModel viewModel)
    {
        viewModel.RadioOptions = new List<RadioButtonOption>
        {
            new () { Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentOptionA, "Option A") },
            new () { Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentOptionB, "Option B"), IsChecked = true },
            new () { Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentOptionC, "Option C") },
            new () { Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentOptionD, "Option D"), IsEnabled = false },
        };
    }
}

internal static class RadioButtonShowCaseLanguage
{
    public static string Get(RadioButtonShowCaseLangResourceKind resourceKind, string fallback)
    {
        if (Application.Current is null)
        {
            return fallback;
        }

        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
