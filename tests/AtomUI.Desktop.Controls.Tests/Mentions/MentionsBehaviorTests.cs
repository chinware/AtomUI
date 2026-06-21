using System.Collections.ObjectModel;
using System.Reflection;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AtomUIMentions = AtomUI.Desktop.Controls.Mentions;

namespace AtomUI.Desktop.Controls.Tests.Mention;

public class MentionsBehaviorTests
{
    static MentionsBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void IsDropDownOpen_Opens_And_Closes_Popup_With_Single_Event_Pair()
    {
        var mentions = new AtomUIMentions
        {
            Width           = 240,
            IsMotionEnabled = false,
            OptionsSource =
            [
                new MentionOption { Header = "afc163", Value = "afc163" },
                new MentionOption { Header = "zombieJ", Value = "zombieJ" }
            ]
        };
        var openedCount = 0;
        var closedCount = 0;
        mentions.DropDownOpened += (_, _) => openedCount++;
        mentions.DropDownClosed += (_, _) => closedCount++;
        var window = CreateWindow(mentions);

        try
        {
            var popup = FindPopup(mentions);

            mentions.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            mentions.IsDropDownOpen.ShouldBeTrue();
            popup.IsOpen.ShouldBeTrue();
            openedCount.ShouldBe(1);

            mentions.IsDropDownOpen = false;
            Dispatcher.UIThread.RunJobs();

            mentions.IsDropDownOpen.ShouldBeFalse();
            popup.IsOpen.ShouldBeFalse();
            closedCount.ShouldBe(1);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void OptionsSource_Replace_Before_First_Open_Does_Not_Require_View()
    {
        var options = new ObservableCollection<IMentionOption>
        {
            new MentionOption { Header = "alpha", Value = "alpha" },
            new MentionOption { Header = "beta", Value = "beta" }
        };
        var mentions = new AtomUIMentions
        {
            Width         = 240,
            OptionsSource = options
        };

        Should.NotThrow(() =>
        {
            options[1] = new MentionOption { Header = "gamma", Value = "gamma" };
        });

        var window = CreateWindow(mentions);

        try
        {
            mentions.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var candidateList = FindCandidateList(window);
            candidateList.Items.Cast<IMentionOption>()
                         .Select(option => option.Header?.ToString())
                         .ShouldBe(["alpha", "gamma"]);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Committing_Candidate_Without_Current_Trigger_Does_Not_Insert_Stale_Mention()
    {
        var option = new MentionOption { Header = "afc163", Value = "afc163" };
        var mentions = new AtomUIMentions
        {
            Width         = 240,
            Value         = "plain text",
            OptionsSource = [option]
        };
        var window = CreateWindow(mentions);

        try
        {
            mentions.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var candidateList = FindCandidateList(window);
            candidateList.SelectedItem = option;
            candidateList.RaiseEvent(new RoutedEventArgs(CandidateList.CommitEvent));
            Dispatcher.UIThread.RunJobs();

            mentions.Value.ShouldBe("plain text");
            mentions.IsDropDownOpen.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    private static AvaloniaWindow CreateWindow(Control content)
    {
        var overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 420,
            Height = 320
        };
        overlayPanel.Children.Add(content);

        var visualLayerManager = new VisualLayerManager
        {
            Child = overlayPanel
        };
        EnablePopupOverlayLayer(visualLayerManager);

        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 320,
            Content = visualLayerManager
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static Popup FindPopup(Visual root)
    {
        return root.GetVisualDescendants()
                   .OfType<Popup>()
                   .Single();
    }

    private static CandidateList FindCandidateList(Visual root)
    {
        for (var i = 0; i < 20; i++)
        {
            Dispatcher.UIThread.RunJobs();
            var candidateList = root.GetVisualDescendants()
                                    .OfType<CandidateList>()
                                    .SingleOrDefault();
            if (candidateList != null)
            {
                return candidateList;
            }
        }

        throw new InvalidOperationException("Expected Mentions popup candidate list to be realized.");
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
