using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class GalleryInstallerAssetTests
{
    [Fact]
    public void AppImage_Config_References_Existing_Installer_Assets()
    {
        var desktopProjectPath = GetRepoDirectory("controlgallery/AtomUIGallery.Desktop");
        var configPath         = Path.Combine(desktopProjectPath, "configs/InstallerConfig.appimage.xml");
        var config             = XDocument.Load(configPath);

        var appIcon     = ReadRequiredElement(config, "AppIcon");
        var appIconPath = Path.Combine(desktopProjectPath, appIcon);
        Directory.Exists(appIconPath).ShouldBeTrue($"Missing AppImage AppIcon directory: {appIconPath}");

        var installerApplicationIcon = ReadRequiredElement(config, "InstallerApplicationIcon");
        var installerIconPath        = Path.Combine(desktopProjectPath, installerApplicationIcon);
        File.Exists(installerIconPath).ShouldBeTrue($"Missing AppImage InstallerApplicationIcon file: {installerIconPath}");
    }

    private static string ReadRequiredElement(XDocument document, string elementName)
    {
        var value = document.Root?.Elements(elementName).SingleOrDefault()?.Value;
        value.ShouldNotBeNullOrWhiteSpace();
        return value;
    }

    private static string GetRepoDirectory(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"Could not find repository directory: {relativePath}");
    }
}
