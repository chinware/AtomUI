using System.IO;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuImplementationContractTests
{
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
