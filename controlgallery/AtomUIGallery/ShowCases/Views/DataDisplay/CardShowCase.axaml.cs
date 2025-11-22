using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class CardShowCase : ReactiveUserControl<CardViewModel>
{
    public CardShowCase()
    {
        this.WhenActivated(disposables =>
        {
            var application = Application.Current;
            if (application != null)
            {
                application.ActualThemeVariantChanged += HandleActualThemeVariantChanged;
                disposables.Add(Disposable.Create(() => application.ActualThemeVariantChanged -= HandleActualThemeVariantChanged ));
            }
        });
        InitializeComponent();
    }

    private void HandleActualThemeVariantChanged(object? sender, EventArgs e)
    {
        ConfigureBorderlessBgFrame();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureBorderlessBgFrame();
    }

    private void ConfigureBorderlessBgFrame()
    {
        var application = Application.Current;
        if (application != null)
        {
            if (DataContext is CardViewModel cardViewModel)
            {
                if (application.IsDarkThemeMode())
                {
                    cardViewModel.BorderlessFrameBg = new SolidColorBrush(Color.FromRgb(48, 48, 48));
                }
                else
                {
                    cardViewModel.BorderlessFrameBg = new SolidColorBrush(Color.FromRgb(240, 242, 245));
                }
            }
        }
    }
}