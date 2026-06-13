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
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public CheckBoxShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;

        this.WhenActivated(disposables =>
        {
            if (DataContext is CheckBoxViewModel viewModel)
            {
                RefreshLocalizedContent(viewModel);
                
                GalleryBindingUtils.OneWay(viewModel, nameof(CheckBoxViewModel.CheckBoxOptions),
                                           vm => vm.CheckBoxOptions, BasicCheckBoxGroup,
                                           AtomUI.Controls.Commons.AbstractCheckBoxGroup.ItemsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(CheckBoxViewModel.DefaultCheckBoxOptions),
                                           vm => vm.DefaultCheckBoxOptions, BasicCheckBoxGroup,
                                           AtomUI.Controls.Commons.AbstractCheckBoxGroup.CheckedItemsProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(CheckStatusBtn, viewModel.CheckStatusCommand).DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(EnableStatusBtn, viewModel.EnableStatusCommand).DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(ControlledCheckbox, viewModel.CheckBoxCommand).DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(CheckAllCheckbox, viewModel.CheckedAllStatusCommand)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(AppleCheckBox, viewModel.CheckedItemStatusCommand1)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(PearCheckBox, viewModel.CheckedItemStatusCommand2)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.BindCommand(OrangeCheckBox, viewModel.CheckedItemStatusCommand3)
                                   .DisposeWith(disposables);

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
        EnsureSelectedScenarioContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearLazyScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ExamplesContent.DataContext = DataContext;
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            content.DataContext = DataContext;
        }
        EnsureSelectedScenarioContent();
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not TabStripItem tabStripItem ||
            tabStripItem.Tag is not string scenario)
        {
            return;
        }

        var content = ResolveScenarioContent(scenario);
        if (!ReferenceEquals(ScenarioContentHost.Content, content))
        {
            ScenarioContentHost.Content = content;
        }
    }

    private void ClearLazyScenarioContent()
    {
        if (ScenarioContentHost.Content is not null &&
            !ReferenceEquals(ScenarioContentHost.Content, ExamplesContent))
        {
            ScenarioContentHost.Content = null;
        }
        _lazyScenarioContentCache.Clear();
    }

    private Control ResolveScenarioContent(string scenario)
    {
        if (scenario == ExamplesScenario)
        {
            ExamplesContent.DataContext = DataContext;
            return ExamplesContent;
        }

        if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _lazyScenarioContentCache.Add(scenario, content);
        }

        return content;
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
