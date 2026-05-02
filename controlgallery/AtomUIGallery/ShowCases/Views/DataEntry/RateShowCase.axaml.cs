using System;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class RateShowCase : ReactiveUserControl<RateViewModel>
{
    public RateShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is RateViewModel viewModel)
            {
                viewModel.Tooltips = [
                    "terrible", "bad", "normal", "good", "wonderful"
                ];
                this.OneWayBind(viewModel, vm => vm.Tooltips, v => v.ToolTipRate.ToolTips)
                    .DisposeWith(disposables);
                Disposable.Create(() =>
                {
                    viewModel.Tooltips = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }

    private void HandleValueChanged(object? sender, RateValueChangedEventArgs e)
    {
        if (DataContext is RateViewModel viewModel)
        {
            var index = (int)Math.Round(e.NewValue, MidpointRounding.AwayFromZero) - 1;
            if (viewModel.Tooltips?.Count > 0 && index >= 0)
            {
                var tooltip = viewModel.Tooltips[index];
                viewModel.ActiveTooltip = tooltip;
            }
            else
            {
                viewModel.ActiveTooltip = null;
            }
        }
    }
}
