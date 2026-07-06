using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Rate;

public partial class RateShowCase : GalleryReactiveUserControl<RateViewModel>
{
    public const string LanguageId = nameof(RateShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public RateShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
        this.WhenActivated(disposables =>
        {
            if (DataContext is RateViewModel viewModel)
            {
                ConfigureLocalizedTooltips(viewModel);

                var themeManager = Application.Current?.GetThemeManager();
                if (themeManager != null)
                {
                    EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => ConfigureLocalizedTooltips(viewModel);
                    themeManager.LanguageVariantChanged += handler;
                    Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                        .DisposeWith(disposables);
                }

                Disposable.Create(() =>
                {
                    viewModel.Tooltips = null;
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
            ApiScenario         => new RateApiDataGrid(),
            DesignTokenScenario => new RateDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Rate scenario: {scenario}")
        };
    }

    private void ConfigureLocalizedTooltips(RateViewModel viewModel)
    {
        viewModel.Tooltips = new List<string>
        {
            RateShowCaseLanguage.Get(RateShowCaseLangResourceKind.P2TooltipTerrible, "terrible"),
            RateShowCaseLanguage.Get(RateShowCaseLangResourceKind.P2TooltipBad, "bad"),
            RateShowCaseLanguage.Get(RateShowCaseLangResourceKind.P2TooltipNormal, "normal"),
            RateShowCaseLanguage.Get(RateShowCaseLangResourceKind.P2TooltipGood, "good"),
            RateShowCaseLanguage.Get(RateShowCaseLangResourceKind.P2TooltipWonderful, "wonderful"),
        };
        SyncActiveTooltip(viewModel, 0);
        viewModel.RefreshLocalizedState();
    }

    private void HandleValueChanged(object? sender, RateValueChangedEventArgs e)
    {
        if (DataContext is RateViewModel viewModel)
        {
            SyncActiveTooltip(viewModel, e.NewValue);
        }
    }

    private static void SyncActiveTooltip(RateViewModel viewModel, double value)
    {
        var index = (int)Math.Round(value, MidpointRounding.AwayFromZero) - 1;
        if (viewModel.Tooltips?.Count > 0 && index >= 0 && index < viewModel.Tooltips.Count)
        {
            viewModel.ActiveTooltip = viewModel.Tooltips[index];
        }
        else
        {
            viewModel.ActiveTooltip = null;
        }
    }
}

internal static class RateShowCaseLanguage
{
    public static string Get(RateShowCaseLangResourceKind resourceKind, string fallback)
    {
        if (Application.Current is null)
        {
            return fallback;
        }

        return LanguageResourceBinder.GetLangResource(resourceKind) ?? fallback;
    }
}
