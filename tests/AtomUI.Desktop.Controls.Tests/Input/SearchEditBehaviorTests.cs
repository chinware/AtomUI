using System.Reflection;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Input;

public class SearchEditBehaviorTests
{
    static SearchEditBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Search_On_Enter_Is_Enabled_By_Default()
    {
        var searchEdit = new SearchEdit();

        searchEdit.IsSearchOnEnterEnabled.ShouldBeTrue();
    }

    [Fact]
    public void Enter_Raises_SearchRequested_With_Query_And_Trigger()
    {
        var searchEdit = new SearchEdit { Text = "AtomUI" };
        SearchRequestedEventArgs? receivedArgs = null;
        searchEdit.SearchRequested += (_, args) => receivedArgs = args;

        var keyEventArgs = RaiseKeyUp(searchEdit, Key.Enter);

        receivedArgs.ShouldNotBeNull();
        receivedArgs.Query.ShouldBe("AtomUI");
        receivedArgs.Trigger.ShouldBe(SearchTriggerSource.EnterKey);
        keyEventArgs.Handled.ShouldBeTrue();
    }

    [Fact]
    public void Enter_Does_Not_Request_Search_When_Enter_Search_Is_Disabled()
    {
        var searchEdit = new SearchEdit { IsSearchOnEnterEnabled = false };
        var requestCount = 0;
        searchEdit.SearchRequested += (_, _) => requestCount++;

        var keyEventArgs = RaiseKeyUp(searchEdit, Key.Enter);

        requestCount.ShouldBe(0);
        keyEventArgs.Handled.ShouldBeFalse();
    }

    [Fact]
    public void Handled_Enter_Does_Not_Request_Search()
    {
        var searchEdit = new SearchEdit();
        var requestCount = 0;
        searchEdit.SearchRequested += (_, _) => requestCount++;

        var keyEventArgs = RaiseKeyUp(searchEdit, Key.Enter, isHandled: true);

        requestCount.ShouldBe(0);
        keyEventArgs.Handled.ShouldBeTrue();
    }

    [Fact]
    public void Non_Enter_Key_Does_Not_Request_Search()
    {
        var searchEdit = new SearchEdit();
        var requestCount = 0;
        searchEdit.SearchRequested += (_, _) => requestCount++;

        var keyEventArgs = RaiseKeyUp(searchEdit, Key.A);

        requestCount.ShouldBe(0);
        keyEventArgs.Handled.ShouldBeFalse();
    }

    [Fact]
    public void Search_Button_Raises_SearchRequested_With_Button_Trigger()
    {
        var searchEdit = new SearchEdit { Text = "Button query" };
        SearchRequestedEventArgs? receivedArgs = null;
        searchEdit.SearchRequested += (_, args) => receivedArgs = args;

        ShowInWindow(searchEdit, () =>
        {
            var searchButton = searchEdit.GetVisualDescendants()
                                         .OfType<Button>()
                                         .Single(button => button.Name == "PART_RightAddOn");

            searchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, searchButton));
        });

        receivedArgs.ShouldNotBeNull();
        receivedArgs.Query.ShouldBe("Button query");
        receivedArgs.Trigger.ShouldBe(SearchTriggerSource.Button);
    }

    [Fact]
    public void Searching_State_Suppresses_Search_Requests()
    {
        var searchEdit = new SearchEdit
        {
            IsSearching = true,
            Text        = "Ignored query"
        };
        var requestCount = 0;
        searchEdit.SearchRequested += (_, _) => requestCount++;

        var keyEventArgs = RaiseKeyUp(searchEdit, Key.Enter);

        ShowInWindow(searchEdit, () =>
        {
            var searchButton = searchEdit.GetVisualDescendants()
                                         .OfType<Button>()
                                         .Single(button => button.Name == "PART_RightAddOn");

            searchButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, searchButton));
        });

        requestCount.ShouldBe(0);
        keyEventArgs.Handled.ShouldBeTrue();
    }

    [Fact]
    public void AutoCompleteSearchEdit_Forwards_Search_Properties_And_Request()
    {
        var autoComplete = new AutoCompleteSearchEdit
        {
            Width                  = 240,
            Value                  = "AutoComplete query",
            IsSearching            = true,
            IsSearchOnEnterEnabled = false
        };
        SearchRequestedEventArgs? receivedArgs = null;
        autoComplete.SearchRequested += (_, args) => receivedArgs = args;

        ShowInWindow(autoComplete, () =>
        {
            var searchEdit = autoComplete.GetVisualDescendants()
                                             .OfType<SearchEdit>()
                                             .Single();

            searchEdit.IsSearching.ShouldBeTrue();
            searchEdit.IsSearchOnEnterEnabled.ShouldBeFalse();

            autoComplete.IsSearching = false;
            autoComplete.IsSearchOnEnterEnabled = true;
            Dispatcher.UIThread.RunJobs();

            searchEdit.IsSearching.ShouldBeFalse();
            searchEdit.IsSearchOnEnterEnabled.ShouldBeTrue();

            RaiseKeyUp(searchEdit, Key.Enter);

            receivedArgs.ShouldNotBeNull();
            receivedArgs.Query.ShouldBe("AutoComplete query");
            receivedArgs.Trigger.ShouldBe(SearchTriggerSource.EnterKey);
            receivedArgs.Source.ShouldBeSameAs(searchEdit);
        });
    }

    [Fact]
    public void Legacy_Search_Api_Is_Removed()
    {
        const BindingFlags publicStatic = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        typeof(SearchEdit).GetProperty("HandleEnterAsSearch").ShouldBeNull();
        typeof(SearchEdit).GetField("HandleEnterAsSearchProperty", publicStatic).ShouldBeNull();
        typeof(SearchEdit).GetProperty("IsOperating").ShouldBeNull();
        typeof(SearchEdit).GetField("IsOperatingProperty", publicStatic).ShouldBeNull();
        typeof(SearchEdit).GetEvent("SearchButtonClick").ShouldBeNull();
        typeof(SearchEdit).GetField("SearchButtonClickEvent", publicStatic).ShouldBeNull();

        typeof(AutoCompleteSearchEdit).GetProperty("IsOperating").ShouldBeNull();
        typeof(AutoCompleteSearchEdit).GetField("IsOperatingProperty", publicStatic).ShouldBeNull();
        typeof(AutoCompleteSearchEdit).GetEvent("SearchButtonClick").ShouldBeNull();
    }

    private static KeyEventArgs RaiseKeyUp(InputElement target, Key key, bool isHandled = false)
    {
        var eventArgs = new KeyEventArgs
        {
            RoutedEvent  = InputElement.KeyUpEvent,
            Source       = target,
            Key          = key,
            PhysicalKey  = key == Key.Enter ? PhysicalKey.Enter : PhysicalKey.None,
            KeyModifiers = KeyModifiers.None,
            Handled      = isHandled
        };

        target.RaiseEvent(eventArgs);
        return eventArgs;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 160,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
