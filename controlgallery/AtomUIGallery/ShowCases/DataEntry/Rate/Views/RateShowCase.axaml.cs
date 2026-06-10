using System;
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

namespace AtomUIGallery.ShowCases.Rate;

public partial class RateShowCase : GalleryReactiveUserControl<RateViewModel>
{
    public const string LanguageId = nameof(RateShowCase);

    public RateShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is RateViewModel viewModel)
            {
                ConfigureLocalizedTooltips(viewModel);

                GalleryBindingUtils.OneWay(viewModel, nameof(RateViewModel.Tooltips), vm => vm.Tooltips,
                                           ToolTipRate, AtomUI.Controls.Commons.AbstractRate.ToolTipsProperty)
                                   .DisposeWith(disposables);

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
        InitializeComponent();
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
        SyncActiveTooltip(viewModel, ToolTipRate.Value);
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
