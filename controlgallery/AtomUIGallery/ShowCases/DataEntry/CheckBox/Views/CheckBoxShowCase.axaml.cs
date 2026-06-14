using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.CheckBox;

public partial class CheckBoxShowCase : GalleryReactiveUserControl<CheckBoxViewModel>
{
    public const string LanguageId = nameof(CheckBoxShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public CheckBoxShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            if (DataContext is CheckBoxViewModel viewModel)
            {
                RefreshLocalizedContent(viewModel);
                
                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshLocalizedContent(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                        .DisposeWith(disposables);
                }
                
                Disposable.Create(() =>
                {
                    viewModel.CheckBoxOptions        = null;
                    viewModel.DefaultCheckBoxOptions = null;
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
            ApiScenario         => new CheckBoxApiDataGrid(),
            DesignTokenScenario => new CheckBoxDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown CheckBox scenario: {scenario}")
        };
    }

    private void RefreshLocalizedContent(CheckBoxViewModel viewModel)
    {
        viewModel.RefreshLocalizedTexts();
        ConfigureCheckBoxOptions(viewModel);
    }
    
    private void ConfigureCheckBoxOptions(CheckBoxViewModel viewModel)
    {
        var apple = new CheckBoxOption()
        {
            Content = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentApple, "Apple")
        };
        var pear = new CheckBoxOption()
        {
            Content = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentPear, "Pear")
        };
        viewModel.CheckBoxOptions = new List<CheckBoxOption>
        {
            apple,
            pear,
            new ()
            {
                Content   = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentOrange, "Orange"),
                IsEnabled = false
            },
        };
        viewModel.DefaultCheckBoxOptions = new List<CheckBoxOption>
        {
            pear,
        };
    }
}

internal static class CheckBoxShowCaseLanguage
{
    public static string Get(CheckBoxShowCaseLangResourceKind resourceKind, string fallback)
    {
        if (Application.Current is null)
        {
            return fallback;
        }

        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
