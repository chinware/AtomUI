using System.IO;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadImplementationContractTests
{
    [Fact]
    public void Trigger_Content_File_Select_Request_Event_Is_Registered_On_Trigger_Content()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/Upload/UploadTriggerContent.cs");

        source.ShouldContain("RoutedEvent.Register<UploadTriggerContent, RoutedEventArgs>(nameof(FileSelectRequest), RoutingStrategies.Bubble)");
        source.ShouldNotContain("RoutedEvent.Register<AbstractUploadListItem, RoutedEventArgs>(nameof(FileSelectRequest)");
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
