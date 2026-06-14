using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.RadioButton;

public partial class RadioButtonShowCase : GalleryReactiveUserControl<RadioButtonViewModel>
{
    public const string LanguageId = nameof(RadioButtonShowCase);

    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public RadioButtonShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            if (DataContext is RadioButtonViewModel viewModel)
            {
                ConfigureRadioOptions(viewModel);

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
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new RadioButtonApiDataGrid(),
            DesignTokenScenario => new RadioButtonDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown RadioButton scenario: {scenario}")
        };
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
