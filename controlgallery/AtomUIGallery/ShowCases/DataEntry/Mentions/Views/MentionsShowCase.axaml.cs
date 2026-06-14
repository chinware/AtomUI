using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Mentions;

public partial class MentionsShowCase : GalleryReactiveUserControl<MentionsViewModel>
{
    public const string LanguageId = nameof(MentionsShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public MentionsShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            if (DataContext is MentionsViewModel viewModel)
            {
                InitBasicMentionOptions(viewModel);
                viewModel.MentionTriggers          = ["@", "#"];
                viewModel.MentionOptionAsyncLoader = new MentionOptionsAsyncLoader();

                Disposable.Create(() =>
                {
                    viewModel.BasicMentionOptions      = null;
                    viewModel.MentionTriggers          = null;
                    viewModel.MentionOptionAsyncLoader = null;
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
            ApiScenario         => new MentionsApiDataGrid(),
            DesignTokenScenario => new MentionsDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Mentions scenario: {scenario}")
        };
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
