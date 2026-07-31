using Avalonia.Input;
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
