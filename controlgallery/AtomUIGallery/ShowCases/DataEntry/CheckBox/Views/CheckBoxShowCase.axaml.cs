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

public partial class CheckBoxShowCase : GalleryReactiveUserControl<CheckBoxViewModel>
{
    public const string LanguageId = nameof(CheckBoxShowCase);

    public CheckBoxShowCase()
    {
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
