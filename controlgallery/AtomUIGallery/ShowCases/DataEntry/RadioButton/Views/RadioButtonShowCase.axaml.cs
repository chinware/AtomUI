using AtomUIGallery.Localization;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using Avalonia;

namespace AtomUIGallery.ShowCases.RadioButton;

public partial class RadioButtonShowCase : GalleryReactiveUserControl<RadioButtonViewModel>
{
    public const string LanguageId = nameof(RadioButtonShowCase);

    public RadioButtonShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            if (DataContext is RadioButtonViewModel viewModel)
            {
                ConfigureRadioOptions(viewModel);

                var languageManager = Application.Current?.GetLanguageManager();
                if (languageManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => ConfigureRadioOptions(viewModel);
                    languageManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => languageManager.LanguageVariantChanged -= handler)
                              .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    viewModel.RadioOptions = null;
                    viewModel.ClearTwoWayRadioOptions();
                }).DisposeWith(disposables);
            }
        });
    }

    private static void ConfigureRadioOptions(RadioButtonViewModel viewModel)
    {
        viewModel.RadioOptions = new List<RadioButtonOption>
        {
            new () { Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentOptionA, "Option A") },
            new () { Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentOptionB, "Option B"), IsChecked = true },
            new () { Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentOptionC, "Option C") },
            new () { Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentOptionD, "Option D"), IsEnabled = false },
        };
        viewModel.ConfigureTwoWayRadioOptions(
            new RadioButtonOption
            {
                Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentHangzhou, "Hangzhou")
            },
            new RadioButtonOption
            {
                Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentShanghai, "Shanghai")
            },
            new RadioButtonOption
            {
                Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentBeijing, "Beijing")
            },
            new RadioButtonOption
            {
                Content = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentChengdu, "Chengdu")
            });
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
