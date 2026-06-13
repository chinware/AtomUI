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
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public MentionsShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;

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
