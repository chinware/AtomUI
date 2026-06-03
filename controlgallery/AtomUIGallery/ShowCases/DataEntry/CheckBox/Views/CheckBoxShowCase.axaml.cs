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

namespace AtomUIGallery.ShowCases.CheckBox;

public partial class CheckBoxShowCase : ReactiveUserControl<CheckBoxViewModel>
{
    public const string LanguageId = nameof(CheckBoxShowCase);

    public CheckBoxShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is CheckBoxViewModel viewModel)
            {
                RefreshLocalizedContent(viewModel);
                
                this.OneWayBind(viewModel, vm => vm.CheckBoxOptions, v => v.BasicCheckBoxGroup.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DefaultCheckBoxOptions, v => v.BasicCheckBoxGroup.CheckedItems)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckStatusCommand, v => v.CheckStatusBtn)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.EnableStatusCommand, v => v.EnableStatusBtn)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckBoxCommand, v => v.ControlledCheckbox)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckedAllStatusCommand, v => v.CheckAllCheckbox)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckedItemStatusCommand1, v => v.AppleCheckBox)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckedItemStatusCommand2, v => v.PearCheckBox)
                    .DisposeWith(disposables);
                this.BindCommand(viewModel, vm => vm.CheckedItemStatusCommand3, v => v.OrangeCheckBox)
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
        InitializeComponent();
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
