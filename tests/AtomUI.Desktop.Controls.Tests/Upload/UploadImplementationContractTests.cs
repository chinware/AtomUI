using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadImplementationContractTests
{
    [Fact]
    public void Legacy_Trigger_Content_And_Task_Info_Artifacts_Are_Removed()
    {
        File.Exists(GetRepoFile("src/AtomUI.Desktop.Controls/Upload/UploadTriggerContent.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("src/AtomUI.Desktop.Controls/Upload/UploadTaskInfo.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("src/AtomUI.Desktop.Controls/Upload/Themes/UploadTriggerContentTheme.axaml")).ShouldBeFalse();
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
