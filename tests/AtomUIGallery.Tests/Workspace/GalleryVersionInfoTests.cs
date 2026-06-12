using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using AtomUIGallery.Workspace.Views;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class GalleryVersionInfoTests
{
    [Fact]
    public void Gallery_AtomUI_Version_Metadata_Comes_From_Version_Props()
    {
        var expectedVersion = ReadAtomUIVersionFromProps();
        var assembly        = typeof(WorkspaceWindow).Assembly;

        var metadata = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                               .SingleOrDefault(attribute => attribute.Key == "AtomUIVersion");

        metadata.ShouldNotBeNull();
        metadata.Value.ShouldBe(expectedVersion);
    }

    [Fact]
    public void Gallery_Display_Version_Uses_AtomUI_Version_Metadata()
    {
        var expectedVersion = $"v{ReadAtomUIVersionFromProps()}";
        var versionInfoType = typeof(WorkspaceWindow).Assembly.GetType("AtomUIGallery.GalleryVersionInfo");

        versionInfoType.ShouldNotBeNull();
        var displayVersion = versionInfoType.GetProperty("DisplayVersion", BindingFlags.Public | BindingFlags.Static)
                                            ?.GetValue(null) as string;

        displayVersion.ShouldBe(expectedVersion);
    }

    private static string ReadAtomUIVersionFromProps()
    {
        var path     = GetRepoFile("build/Version.props");
        var document = XDocument.Load(path);
        var version  = document.Descendants("AtomUIVersion").SingleOrDefault()?.Value;

        version.ShouldNotBeNullOrWhiteSpace();
        return version;
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

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
