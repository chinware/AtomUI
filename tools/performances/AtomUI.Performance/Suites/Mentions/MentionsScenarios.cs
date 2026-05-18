using AtomUI.Controls;
using AtomUI.Controls.Utils;
using AtomUI.Desktop.Controls;
using AtomUI.Desktop.Controls.DataLoad;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateMentionsScenarios()
    {
        return
        [
            new PerfScenario("Mentions.Default.Closed", _ => new Mentions
            {
                Width = 300
            }),
            new PerfScenario("Mentions.OptionsSource.Closed", _ => new Mentions
            {
                Width         = 300,
                OptionsSource = CreateMentionOptions()
            }),
            new PerfScenario("Mentions.AllowClear.Closed", _ => new Mentions
            {
                Width         = 300,
                IsAllowClear  = true,
                OptionsSource = CreateMentionOptions()
            }),
            new PerfScenario("Mentions.AutoSize.Closed", _ => new Mentions
            {
                Width         = 300,
                IsAutoSize    = true,
                OptionsSource = CreateMentionOptions()
            }),
            new PerfScenario("Mentions.AsyncLoader.Closed", _ => new Mentions
            {
                Width              = 300,
                OptionsAsyncLoader = new ImmediateMentionOptionsAsyncLoader()
            }),
            new PerfScenario("Mentions.ContainsFilter.Closed", _ => new Mentions
            {
                Width         = 300,
                Filter        = ValueFilterFactory.BuildFilter(ValueFilterMode.Contains),
                OptionsSource = CreateMentionOptions()
            }),
            new PerfScenario("Mentions.PopupMaterialized", _ => MaterializeMentionsPopupAfterLoaded(new Mentions
            {
                Width         = 300,
                Value         = "@a",
                OptionsSource = CreateMentionOptions()
            })),
            new PerfScenario("Mentions.GalleryShape", _ => CreateMentionsGalleryShape())
        ];
    }

    private static List<IMentionOption> CreateMentionOptions()
    {
        return
        [
            new MentionOption { Header = "afc163", Value = "afc163" },
            new MentionOption { Header = "zombieJ", Value = "zombieJ" },
            new MentionOption { Header = "yesmeck", Value = "yesmeck" },
            new MentionOption { Header = "AlexSmith42", Value = "AlexSmith42" },
            new MentionOption { Header = "JordanLee86", Value = "JordanLee86" },
            new MentionOption { Header = "TaylorBrown13", Value = "TaylorBrown13" }
        ];
    }

    private static Control CreateMentionsGalleryShape()
    {
        var options = CreateMentionOptions();
        var panel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 10
        };

        panel.Children.Add(new Mentions
        {
            Width         = 300,
            DefaultValue  = "@afc163",
            OptionsSource = options
        });

        panel.Children.Add(new Mentions { Width = 300, StyleVariant = InputControlStyleVariant.Outlined });
        panel.Children.Add(new Mentions { Width = 300, StyleVariant = InputControlStyleVariant.Filled });
        panel.Children.Add(new Mentions { Width = 300, StyleVariant = InputControlStyleVariant.Borderless });
        panel.Children.Add(new Mentions { Width = 300, StyleVariant = InputControlStyleVariant.Underlined });

        panel.Children.Add(new Mentions
        {
            Width              = 300,
            OptionsAsyncLoader = new ImmediateMentionOptionsAsyncLoader()
        });

        panel.Children.Add(new Mentions
        {
            Width         = 300,
            TriggerPrefix = ["@", "#"]
        });

        panel.Children.Add(new Mentions
        {
            Width         = 300,
            IsEnabled     = false,
            OptionsSource = options
        });
        panel.Children.Add(new Mentions
        {
            Width         = 300,
            IsReadOnly    = true,
            OptionsSource = options
        });
        panel.Children.Add(new Mentions
        {
            Width         = 300,
            Placement     = MentionsPlacementMode.Top,
            OptionsSource = options
        });
        panel.Children.Add(new Mentions
        {
            Width         = 300,
            Status        = InputControlStatus.Error,
            DefaultValue  = "@afc163",
            OptionsSource = options
        });
        panel.Children.Add(new Mentions
        {
            Width         = 300,
            Status        = InputControlStatus.Warning,
            DefaultValue  = "@afc163",
            OptionsSource = options
        });
        panel.Children.Add(new Mentions
        {
            Width         = 300,
            IsAutoSize    = true,
            OptionsSource = options
        });
        panel.Children.Add(new Mentions
        {
            Width         = 300,
            IsAllowClear  = true,
            OptionsSource = options
        });
        panel.Children.Add(new Mentions
        {
            Width         = 300,
            IsAllowClear  = true,
            Lines         = 3,
            OptionsSource = options
        });

        return panel;
    }

    private static T MaterializeMentionsPopupAfterLoaded<T>(T mentions)
        where T : Mentions
    {
        void HandleLoaded(object? sender, RoutedEventArgs args)
        {
            mentions.Loaded -= HandleLoaded;
            MaterializeMentionsPopupContentForTest(mentions);
        }

        mentions.Loaded += HandleLoaded;
        return mentions;
    }

    private static void MaterializeMentionsPopupContentForTest(Mentions mentions)
    {
        mentions.GetType()
                .GetMethod("EnsurePopupContent", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.Invoke(mentions, null);
    }

    private sealed class ImmediateMentionOptionsAsyncLoader : IMentionOptionsAsyncLoader
    {
        public Task<MentionOptionsLoadResult> LoadAsync(string? context, CancellationToken token)
        {
            return Task.FromResult(new MentionOptionsLoadResult
            {
                StatusCode = RpcStatusCode.Success,
                Data       = CreateMentionOptions()
            });
        }
    }
}
