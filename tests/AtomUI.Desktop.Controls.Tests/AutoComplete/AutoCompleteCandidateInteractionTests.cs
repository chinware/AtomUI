using System.Reflection;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.AutoComplete;

public class AutoCompleteCandidateInteractionTests
{
    static AutoCompleteCandidateInteractionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Pointer_Move_Then_Enter_Commits_The_Pointer_Candidate()
    {
        var alpha = new AutoCompleteOption { Header = "Alpha", Content = "Alpha" };
        var alpine = new AutoCompleteOption { Header = "Alpine", Content = "Alpine" };
        var autoComplete = new Desktop.Controls.AutoComplete
        {
            Width           = 240,
            IsMotionEnabled = false,
            OptionsSource   = new[] { alpha, alpine }
        };
        var window = CreateWindow(autoComplete);

        try
        {
            var textBox = autoComplete.GetVisualDescendants()
                                      .OfType<AbstractTextInput>()
                                      .Single();
            textBox.Text       = "Al";
            textBox.CaretIndex = 2;
            Dispatcher.UIThread.RunJobs();

            autoComplete.IsDropDownOpen.ShouldBeTrue();
            var candidateList   = FindCandidateList(window);
            var alphaContainer  = GetContainer(candidateList, 0);
            var alpineContainer = GetContainer(candidateList, 1);

            PressCandidateKey(candidateList, Key.Down);
            MovePointerTo(alpineContainer, window);
            Dispatcher.UIThread.RunJobs();

            alphaContainer.IsCandidateSelected.ShouldBeFalse();
            alpineContainer.IsCandidateSelected.ShouldBeTrue();
            candidateList.CandidateSelectedItem.ShouldBeSameAs(alpine);

            PressCandidateKey(candidateList, Key.Enter);
            Dispatcher.UIThread.RunJobs();

            autoComplete.SelectedOption.ShouldBeSameAs(alpine);
            autoComplete.Value.ShouldBe("Alpine");
            autoComplete.IsDropDownOpen.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    private static CandidateList FindCandidateList(Visual root)
    {
        for (var i = 0; i < 20; i++)
        {
            Dispatcher.UIThread.RunJobs();
            var candidateList = root.GetVisualDescendants()
                                    .OfType<CandidateList>()
                                    .SingleOrDefault();
            if (candidateList is not null)
            {
                return candidateList;
            }
        }

        throw new InvalidOperationException("Expected AutoComplete popup candidate list to be realized.");
    }

    private static CandidateListItem GetContainer(CandidateList candidateList, int index)
    {
        var container = candidateList.ContainerFromIndex(index) as CandidateListItem;
        container.ShouldNotBeNull();
        return container;
    }

    private static void MovePointerTo(Control target, AvaloniaWindow window)
    {
        var point = target.TranslatePoint(
            new Point(target.Bounds.Width / 2, target.Bounds.Height / 2),
            window);
        point.ShouldNotBeNull();

        window.MouseMove(point.Value);
        Dispatcher.UIThread.RunJobs();
    }

    private static void PressCandidateKey(CandidateList candidateList, Key key)
    {
        candidateList.HandleKeyDown(new KeyEventArgs
        {
            RoutedEvent  = InputElement.KeyDownEvent,
            Source       = candidateList,
            Key          = key,
            PhysicalKey  = key switch
            {
                Key.Enter => PhysicalKey.Enter,
                Key.Down  => PhysicalKey.ArrowDown,
                _         => PhysicalKey.None
            },
            KeyModifiers = KeyModifiers.None
        });
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

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);
        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
