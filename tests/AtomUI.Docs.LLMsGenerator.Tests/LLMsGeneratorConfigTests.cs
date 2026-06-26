using AtomUI.Docs.LLMsGenerator.Config;
using Shouldly;
using Xunit;

namespace AtomUI.Docs.LLMsGenerator.Tests;

public class LLMsGeneratorConfigTests
{
    [Fact]
    public void ReaderLoadsProjectAndControlSetSettings()
    {
        var directory = Directory.CreateTempSubdirectory("atomui-llms-config-");
        var configPath = Path.Combine(directory.FullName, "llms.config.json");

        File.WriteAllText(configPath, """
        {
          "schemaVersion": 1,
          "projectId": "AtomUI.Desktop",
          "displayName": "AtomUI Desktop Controls",
          "defaultLanguage": "cn",
          "languages": ["cn"],
          "outputRoot": "docs/AI/llms",
          "visibility": {
            "default": "public",
            "included": ["public"],
            "excluded": ["internal", "private"]
          },
          "controlSets": [
            {
              "id": "desktop",
              "platform": "desktop",
              "docsRoot": "docs/controls/desktop",
              "galleryRoot": "controlgallery/AtomUIGallery/ShowCases",
              "sourceRoots": [
                "src/AtomUI.Desktop.Controls",
                "src/AtomUI.Desktop.Controls.DataGrid"
              ],
              "categoryOrder": [
                "general",
                "layout"
              ]
            }
          ]
        }
        """);

        var config = LLMsGeneratorConfigReader.Read(configPath);

        config.SchemaVersion.ShouldBe(1);
        config.ProjectId.ShouldBe("AtomUI.Desktop");
        config.DisplayName.ShouldBe("AtomUI Desktop Controls");
        config.DefaultLanguage.ShouldBe("cn");
        config.Languages.ShouldBe(["cn"]);
        config.OutputRoot.ShouldBe("docs/AI/llms");
        config.Visibility.Default.ShouldBe("public");
        config.Visibility.Included.ShouldBe(["public"]);
        config.Visibility.Excluded.ShouldBe(["internal", "private"]);
        config.ControlSets.Count.ShouldBe(1);
        config.ControlSets[0].Id.ShouldBe("desktop");
        config.ControlSets[0].SourceRoots.ShouldBe([
            "src/AtomUI.Desktop.Controls",
            "src/AtomUI.Desktop.Controls.DataGrid"
        ]);
        config.ControlSets[0].CategoryOrder.ShouldBe(["general", "layout"]);
    }
}
