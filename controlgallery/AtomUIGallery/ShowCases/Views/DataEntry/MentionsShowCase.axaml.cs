using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class MentionsShowCase : ReactiveUserControl<MentionsViewModel>
{
    public MentionsShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is MentionsViewModel viewModel)
            {
                InitBasicMentionOptions(viewModel);
                viewModel.MentionTriggers          = ["@", "#"];
                viewModel.MentionOptionAsyncLoader = new MentionOptionsAsyncLoader();
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.BasicMentions.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.DisabledMentions.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.ReadonlyMentions.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.PlacementMentions.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.ErrorMentions.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.WarningMentions.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.AutoSizeMentions.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.ClearableMentions1.OptionsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BasicMentionOptions, v=> v.ClearableMentions2.OptionsSource)
                    .DisposeWith(disposables);
                
                Disposable.Create(() =>
                {
                    viewModel.BasicMentionOptions      = null;
                    viewModel.MentionTriggers          = null;
                    viewModel.MentionOptionAsyncLoader = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
    
    private void InitBasicMentionOptions(MentionsViewModel viewModel)
    {
        viewModel.BasicMentionOptions =
        [
            new MentionOption()
            {
                Header = "afc163",
                Value = "afc163"
            },
            new MentionOption()
            {
                Header = "zombieJ",
                Value  = "zombieJ"
            },
            new MentionOption()
            {
                Header = "yesmeck",
                Value  = "yesmeck"
            }
        ];
    }

    private void HandleCandidateTriggered(object? sender, MentionCandidateTriggeredEventArgs e)
    {
        if (sender is Mentions mentions)
        {
            if (e.TriggerChar == "@")
            {
                mentions.OptionsSource =
                [
                    new MentionOption()
                    {
                        Header = "afc163",
                        Value  = "afc163"
                    },
                    new MentionOption()
                    {
                        Header = "zombieJ",
                        Value  = "zombieJ"
                    },
                    new MentionOption()
                    {
                        Header = "yesmeck",
                        Value  = "yesmeck"
                    }
                ];
            }
            else if (e.TriggerChar == "#")
            {
                mentions.OptionsSource =
                [
                    new MentionOption()
                    {
                        Header = "1.0",
                        Value  = "1.0"
                    },
                    new MentionOption()
                    {
                        Header = "2.0",
                        Value  = "2.0"
                    },
                    new MentionOption()
                    {
                        Header = "3.0",
                        Value  = "3.0"
                    }
                ];
            }
        }
    }
}