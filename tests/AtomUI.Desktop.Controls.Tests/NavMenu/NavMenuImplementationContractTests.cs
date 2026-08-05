using Avalonia.Input;
using Avalonia.Controls.Templates;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuImplementationContractTests
{
    [Fact]
    public void NavMenuNode_Exposes_Command_Configuration_Without_Becoming_Command_Source()
    {
        var node = new NavMenuNode();

        typeof(INavMenuNode).GetProperty(nameof(INavMenuNode.Command)).ShouldNotBeNull();
        typeof(INavMenuNode).GetProperty(nameof(INavMenuNode.CommandParameter)).ShouldNotBeNull();
        NavMenuNode.CommandProperty.ShouldNotBeNull();
        NavMenuNode.CommandParameterProperty.ShouldNotBeNull();
        node.Command.ShouldBeNull();
        node.CommandParameter.ShouldBeNull();
        node.ShouldNotBeAssignableTo<ICommandSource>();
    }

    [Fact]
    public void NavMenu_Entry_Composition_Public_Contract_Is_Strongly_Typed_And_Backward_Compatible()
    {
        typeof(INavMenuEntry).IsAssignableFrom(typeof(INavMenuNode)).ShouldBeTrue();

        var interfaceEntries = typeof(INavMenuNode).GetProperty(nameof(INavMenuNode.Entries));
        interfaceEntries.ShouldNotBeNull();
        interfaceEntries.PropertyType.ShouldBe(typeof(IEnumerable<INavMenuEntry>));

        var nodeEntries = typeof(NavMenuNode).GetProperty(nameof(NavMenuNode.Entries));
        nodeEntries.ShouldNotBeNull();
        nodeEntries.PropertyType.ShouldBe(typeof(IList<INavMenuEntry>));

        typeof(NavMenuGroup).GetProperty(nameof(NavMenuGroup.Header)).ShouldNotBeNull();
        typeof(NavMenuGroup).GetProperty(nameof(NavMenuGroup.HeaderTemplate)).ShouldNotBeNull();
        typeof(NavMenuGroup).GetProperty(nameof(NavMenuGroup.Entries))!
                            .PropertyType.ShouldBe(typeof(IList<INavMenuEntry>));
        typeof(INavMenuEntry).IsAssignableFrom(typeof(NavMenuDivider)).ShouldBeTrue();
    }

    [Fact]
    public void NavMenu_Root_Composition_Public_Contract_Exposes_Fixed_Slots_And_Entry_Spacing()
    {
        var menu = new AtomUI.Desktop.Controls.NavMenu();

        typeof(AtomUI.Desktop.Controls.NavMenu).GetProperty(nameof(menu.Header))!
                                                  .PropertyType.ShouldBe(typeof(object));
        typeof(AtomUI.Desktop.Controls.NavMenu).GetProperty(nameof(menu.HeaderTemplate))!
                                                  .PropertyType.ShouldBe(typeof(IDataTemplate));
        typeof(AtomUI.Desktop.Controls.NavMenu).GetProperty(nameof(menu.Footer))!
                                                  .PropertyType.ShouldBe(typeof(object));
        typeof(AtomUI.Desktop.Controls.NavMenu).GetProperty(nameof(menu.FooterTemplate))!
                                                  .PropertyType.ShouldBe(typeof(IDataTemplate));
        typeof(AtomUI.Desktop.Controls.NavMenu).GetProperty(nameof(menu.ItemSpacing))!
                                                  .PropertyType.ShouldBe(typeof(double));

        menu.Header.ShouldBeNull();
        menu.HeaderTemplate.ShouldBeNull();
        menu.Footer.ShouldBeNull();
        menu.FooterTemplate.ShouldBeNull();
        menu.ItemSpacing.ShouldBe(0d);
    }

    [Fact]
    public void Default_Path_Replay_Does_Not_Wait_On_Fixed_Timer_Delays()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs");
        const string inlineCollapsedWidthMotionDelay =
            "await Task.Delay(InlineCollapsedWidthMotionFrameInterval, cancellationTokenSource.Token);";

        source.ShouldContain(inlineCollapsedWidthMotionDelay);
        source.Replace(inlineCollapsedWidthMotionDelay, string.Empty).ShouldNotContain("Task.Delay");
        source.ShouldNotContain("TimeSpan.FromMilliseconds(50)");
        source.ShouldNotContain("GetNavMenuItemContainerAsync");
        source.ShouldContain("ExecuteLayoutPass()");
    }

    [Fact]
    public void Keyboard_Navigation_Uses_Cursors_Instead_Of_Flattened_Item_Lists()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/NavMenuInteractionHandlerBase.cs");

        source.ShouldNotContain("new List<NavMenuItem>");
        source.ShouldNotContain("CollectDirectNavigationItems");
        source.ShouldNotContain("CollectVisibleInlineItems");
        source.ShouldContain("EnumerateDirectNavigationItems");
        source.ShouldContain("EnumerateVisibleInlineNavigationItems");
        source.ShouldContain("FindAdjacentNavigationItem");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Unable to locate repository file '{relativePath}'.");
    }
}
