using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;

namespace AtomUIGallery.ShowCases.Mentions;

public partial class MentionsShowCase : GalleryReactiveUserControl<MentionsViewModel>
{
    public const string LanguageId = nameof(MentionsShowCase);

    public MentionsShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is MentionsViewModel viewModel)
            {
                InitBasicMentionOptions(viewModel);
                viewModel.MentionTriggers          = ["@", "#"];
                viewModel.MentionOptionAsyncLoader = new MentionOptionsAsyncLoader();

                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, BasicMentions,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, DisabledMentions,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, ReadonlyMentions,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, PlacementMentions,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, ErrorMentions,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, WarningMentions,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, AutoSizeMentions,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, ClearableMentions1,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(MentionsViewModel.BasicMentionOptions),
                                           vm => vm.BasicMentionOptions, ClearableMentions2,
                                           AtomUI.Desktop.Controls.Mentions.OptionsSourceProperty)
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

    private void HandleCandidateTriggered(object? sender, MentionCandidateTriggeredEventArgs e)
    {
        if (sender is AtomUIMentions mentions)
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
