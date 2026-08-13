using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests;

public class ThemeAssetManifestGeneratorTests
{
    [Fact]
    public void Does_Not_Generate_Theme_Manifest_Without_Theme_Assets()
    {
        var result = RunGenerator(
            CreateCompilation("namespace Demo { public sealed class PlainType { } }"),
            [],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        result.SyntaxTrees.Any(static tree =>
            tree.FilePath.EndsWith("GeneratedControlThemeAssetManifest.g.cs", StringComparison.Ordinal))
              .ShouldBeFalse();
    }

    [Fact]
    public void Uses_Configured_Control_Catalog_For_Third_Party_Assets()
    {
        var result = RunGenerator(
            CreateCompilation(TokenSource, "Acme.Controls"),
            [Asset("Themes/ButtonTheme.axaml", ResourceDictionary("ButtonTokenResource Height"))],
            out var diagnostics,
            controlCatalog: "Acme.Controls");

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("namespace AtomUI.Generated.AcmeControls;");
        source.ShouldNotContain("namespace AtomUI.Generated.Acme_Controls;");
        source.ShouldContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"Acme.Controls\", \"Button\")");
        source.ShouldNotContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"AtomUI\", \"Button\")");
    }

    [Fact]
    public void Normalizes_Absolute_Asset_Path_Relative_To_External_Project()
    {
        const string projectDirectory = "/private/tmp/Acme.Controls";
        var result = RunGenerator(
            CreateCompilation(TokenSource),
            [Asset(
                $"{projectDirectory}/Themes/ButtonTheme.axaml",
                ResourceDictionary("ButtonTokenResource Height"))],
            out var diagnostics,
            projectDirectory);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("avares://ThemeAssetManifestTests/Themes/ButtonTheme.axaml");
        source.ShouldContain(
            $"controlThemes.Add(new {GetThemeAssetResourceClassName("Themes/ButtonTheme.axaml")}());");
    }

    [Fact]
    public void Generates_Convention_Owned_Asset_Without_Ambient_Identity_Or_Wrapper()
    {
        var result = RunGenerator(
            [Asset("Button/Themes/ButtonTheme.axaml", ControlTheme("Button", "ButtonTokenResource Height"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("avares://ThemeAssetManifestTests/Button/Themes/ButtonTheme.axaml");
        source.ShouldContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"AtomUI\", \"Button\")");
        source.ShouldNotContain("ButtonThemeAsset");
        source.ShouldNotContain("ControlTokenScope");
        source.ShouldNotContain("\r");
    }

    [Fact]
    public void Generates_Multiple_Assets_For_The_Control_Owned_By_The_Themes_Directory()
    {
        var result = RunGenerator(
            [
                Asset("Button/Themes/ButtonTheme.axaml", ControlTheme("Button", "ButtonTokenResource Height")),
                Asset("Button/Themes/ButtonIconTheme.axaml", ControlTheme("Button", "ButtonTokenResource Height"))
            ],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("Button/Themes/ButtonTheme.axaml");
        source.ShouldContain("Button/Themes/ButtonIconTheme.axaml");
        source.Split("new global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor(", StringSplitOptions.None)
              .Length.ShouldBe(3);
    }

    [Fact]
    public void Excludes_Aggregate_Theme_Dictionaries_From_The_Manifest()
    {
        var result = RunGenerator(
            [
                Asset("Button/Themes/ButtonTheme.axaml", ControlTheme("Button")),
                Asset("Button/Themes/ButtonThemes.axaml", ResourceDictionary())
            ],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("Button/Themes/ButtonTheme.axaml");
        source.ShouldNotContain("Button/Themes/ButtonThemes.axaml");
        source.Split("new global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor(", StringSplitOptions.None)
              .Length.ShouldBe(2);
    }

    [Fact]
    public void Generates_Aot_Safe_Resource_Loader_For_Leaf_Dictionaries()
    {
        var result = RunGenerator(
            [Asset("Button/Themes/ButtonFrameTheme.axaml", ResourceDictionary())],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("internal static void AddResources(");
        source.ShouldContain("controlThemes.Add(new GeneratedThemeAssetResource_");
        source.ShouldNotContain("new global::Avalonia.Markup.Xaml.Styling.ResourceInclude");
    }

    [Fact]
    public void Generates_Unit_Theme_Parts_Without_Per_Asset_Manifests()
    {
        var result = RunGenerator(
            [
                Asset("Button/Themes/ButtonTheme.axaml", ResourceDictionary("ButtonTokenResource Height")),
                Asset("Alert/Themes/AlertTheme.axaml", ResourceDictionary("AlertTokenResource Height"))
            ],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");

        source.ShouldContain("namespace AtomUI.Generated.ThemeAssetManifestTests.LinkedRegistrationV1;");
        source.ShouldContain("public static partial class GeneratedRegistrationUnit_Button_");
        source.ShouldContain("public static partial class GeneratedRegistrationUnit_Alert_");
        source.ShouldContain("static partial void AddThemes(");
        source.ShouldContain("builder.AddThemeAsset(");
        source.ShouldNotContain("AssemblyMetadata(\"AtomUI.Linked.Asset.v1\"");
        source.ShouldNotContain("AssemblyMetadata(\"AtomUI.Linked.AuxiliaryAsset.v1\"");
        source.ShouldContain(GetThemeAssetResourceClassName("Button/Themes/ButtonTheme.axaml"));
        source.ShouldContain(GetThemeAssetResourceClassName("Alert/Themes/AlertTheme.axaml"));
        source.ShouldNotContain("switch (asset.AssetUri.AbsoluteUri)");
        source.ShouldNotContain("GeneratedControlThemeAssetManifest.GetDescriptors()");
    }

    [Fact]
    public void Unit_Theme_Part_Calls_Same_Package_Unit_Dependencies_Directly()
    {
        var compilation = CreateCompilation("""
            namespace Demo;
            public sealed class Button : Avalonia.Controls.Control { }
            """).AddSyntaxTrees(
                CSharpSyntaxTree.ParseText(
                    """
                    namespace Demo;
                    public sealed class Alert : Avalonia.Controls.Control { }
                    """,
                    path: "Alert/Alert.cs",
                    cancellationToken: TestContext.Current.CancellationToken));
        var result = RunGenerator(
            compilation,
            [Asset(
                "Button/Themes/ButtonTheme.axaml",
                ResourceDictionary("ButtonTokenResource Height", "AlertTokenResource Height"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");
        var dependencyFragment = global::AtomUI.Generator.LinkedRegistration.LinkedRegistrationFragmentName.ForUnit(
            "ThemeAssetManifestTests/Alert");
        source.ShouldContain($"{dependencyFragment}.Add(builder);");
    }

    [Fact]
    public void Unit_Theme_Part_Calls_Template_Control_Unit_Dependencies_Directly()
    {
        var compilation = CreateCompilation(
            """
            [assembly: Avalonia.Metadata.XmlnsDefinition("https://atomui.net", "Demo")]

            namespace Avalonia.Metadata
            {
                [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple = true)]
                public sealed class XmlnsDefinitionAttribute(string xmlNamespace, string clrNamespace) : System.Attribute
                {
                }
            }

            namespace Demo
            {
                public sealed class Alert : Avalonia.Controls.Control { }
            }
            """,
            sourcePath: "Alert/Alert.cs").AddSyntaxTrees(
                CSharpSyntaxTree.ParseText(
                    """
                    namespace Demo;
                    public sealed class MarqueeLabel : Avalonia.Controls.Control { }
                    """,
                    path: "MarqueeLabel/MarqueeLabel.cs",
                    cancellationToken: TestContext.Current.CancellationToken));
        var result = RunGenerator(
            compilation,
            [Asset(
                "Alert/Themes/AlertTheme.axaml",
                """
                <ControlTheme xmlns="https://github.com/avaloniaui"
                              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                              xmlns:atom="https://atomui.net"
                              xmlns:demo="using:Demo"
                              TargetType="{x:Type demo:Alert}">
                    <Setter Property="Template">
                        <ControlTemplate>
                            <atom:MarqueeLabel />
                        </ControlTemplate>
                    </Setter>
                </ControlTheme>
                """)],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");
        var dependencyFragment = global::AtomUI.Generator.LinkedRegistration.LinkedRegistrationFragmentName.ForUnit(
            "ThemeAssetManifestTests/MarqueeLabel");
        source.ShouldContain($"{dependencyFragment}.Add(builder);");
        var manifest = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        manifest.ShouldNotContain("ControlTokenIdentity(\"AtomUI\", \"MarqueeLabel\")");
    }

    [Fact]
    public void Unit_Theme_Part_Does_Not_Treat_Default_Avalonia_Elements_As_Package_Controls()
    {
        var compilation = CreateCompilation(
            """
            namespace Demo;
            public sealed class Alert : Avalonia.Controls.Control { }
            """,
            sourcePath: "Alert/Alert.cs").AddSyntaxTrees(
                CSharpSyntaxTree.ParseText(
                    """
                    namespace Demo;
                    public sealed class Button : Avalonia.Controls.Control { }
                    """,
                    path: "Button/Button.cs",
                    cancellationToken: TestContext.Current.CancellationToken));
        var result = RunGenerator(
            compilation,
            [Asset(
                "Alert/Themes/AlertTheme.axaml",
                """
                <ControlTheme xmlns="https://github.com/avaloniaui"
                              xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                              xmlns:demo="using:Demo"
                              TargetType="{x:Type demo:Alert}">
                    <Setter Property="Template">
                        <ControlTemplate>
                            <Button />
                        </ControlTemplate>
                    </Setter>
                </ControlTheme>
                """)],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");
        var unrelatedFragment = global::AtomUI.Generator.LinkedRegistration.LinkedRegistrationFragmentName.ForUnit(
            "ThemeAssetManifestTests/Button");
        source.ShouldNotContain($"{unrelatedFragment}.Add(builder);");
    }

    [Fact]
    public void Generates_Explicit_PackageShared_Asset_As_Package_Core_Fragment()
    {
        var result = RunGenerator(
            CreateCompilation(TokenSource),
            [Asset("Themes/SharedResources.axaml", ResourceDictionary())],
            out var diagnostics,
            packageSharedThemes: ["Themes/SharedResources.axaml"]);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");

        source.ShouldContain("public static class GeneratedPackageSharedThemeFragment_");
        source.ShouldContain("builder.AddPackageSharedThemeAsset(");
        source.ShouldNotContain("AssemblyMetadata(\"AtomUI.Linked.AuxiliaryAsset.v1\"");
        source.ShouldContain(GetThemeAssetResourceClassName("Themes/SharedResources.axaml"));
    }

    [Fact]
    public void Ownerless_Asset_Is_Unknown_And_Forces_Package_Fallback()
    {
        var result = RunGenerator(
            [Asset("Themes/UnownedResources.axaml", ResourceDictionary())],
            out var diagnostics,
            linkedPublish: true);

        diagnostics.ShouldHaveSingleItem().Id.ShouldBe("ATOMUILINK002");
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");
        source.ShouldContain("AssemblyMetadata(\"AtomUI.Linked.Usage.v1\"");
        source.ShouldContain("PackageRoot");
        source.ShouldNotContain("builder.AddPackageSharedThemeAsset(");
    }

    [Fact]
    public void Ordinary_Build_Records_Unknown_Asset_Fallback_Without_Linked_Warning()
    {
        var result = RunGenerator(
            [Asset("Themes/UnownedResources.axaml", ResourceDictionary())],
            out var diagnostics);

        diagnostics.ShouldNotContain(diagnostic => diagnostic.Id == "ATOMUILINK002");
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");
        source.ShouldContain("AssemblyMetadata(\"AtomUI.Linked.Usage.v1\"");
        source.ShouldContain("PackageRoot");
    }

    [Fact]
    public void Resource_Key_Schema_Fingerprint_Uses_Shared_Token_Kind_Value_Order()
    {
        var compilation = CreateCompilation(TokenSource + """

            namespace AtomUI.Theme.Resources
            {
                public enum SharedTokenKind
                {
                    Zeta = 0,
                    Alpha = 1
                }
            }
            """);
        var result = RunGenerator(
            compilation,
            [Asset("Button/Themes/ButtonTheme.axaml", ControlTheme("Button", "ButtonTokenResource Height"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        var expected = ComputeFingerprint("AtomUI", "Button", "AtomUI", "Button", "Zeta", "Alpha");
        source.ShouldContain(
            "0x" + expected.ToString("X16", System.Globalization.CultureInfo.InvariantCulture) + "UL");
    }

    [Fact]
    public void Generates_Aot_Safe_Resource_Loader_For_Default_Typed_Control_Themes()
    {
        var compilation = CreateCompilation("""
            namespace Demo
            {
                public sealed class Button : Avalonia.Controls.Control
                {
                }

                internal sealed class ButtonTheme : Avalonia.Styling.ControlTheme
                {
                }
            }
            """);
        var result = RunGenerator(
            compilation,
            [Asset("Button/Themes/ButtonTheme.axaml", TypedControlTheme("Button", "Demo.ButtonTheme"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("controlThemes.Add(new GeneratedThemeAssetResource_");
        source.ShouldNotContain("new global::Demo.ButtonTheme()");
    }

    [Fact]
    public void Assigns_Internal_Typed_Control_Theme_As_A_Resource_Only_Unit_Asset()
    {
        var compilation = CreateCompilation("""
            namespace Demo
            {
                public sealed class LineEdit : Avalonia.Controls.Control
                {
                }

                internal sealed class InputClearIconButton : Avalonia.Controls.Control
                {
                }

                internal sealed class InputClearIconButtonTheme : Avalonia.Styling.ControlTheme
                {
                }
            }
            """,
            sourcePath: "Input/LineEdit.cs");
        var result = RunGenerator(
            compilation,
            [Asset(
                "Input/Themes/InputClearIconButtonTheme.axaml",
                TypedControlTheme("InputClearIconButton", "Demo.InputClearIconButtonTheme"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("global::System.Array.Empty<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>()");
        source.ShouldContain("controlThemes.Add(new GeneratedThemeAssetResource_");
        source.ShouldNotContain("new global::Demo.InputClearIconButtonTheme()");
        var fragmentSource = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");
        fragmentSource.ShouldContain("builder.AddUnitThemeResource(");
        fragmentSource.ShouldNotContain("AssemblyMetadata(\"AtomUI.Linked.Usage.v1\"");
    }

    [Fact]
    public void Does_Not_Load_Typed_Semantic_Part_Theme_As_A_Default_Control_Theme()
    {
        var compilation = CreateCompilation("""
            namespace Demo
            {
                public sealed class SearchEdit : Avalonia.Controls.Control
                {
                    public Avalonia.Styling.ControlTheme? SearchButtonTheme { get; set; }
                }

                public sealed class Button : Avalonia.Controls.Control
                {
                }

                internal sealed class SearchButtonTheme : Avalonia.Styling.ControlTheme
                {
                }
            }
            """);
        var result = RunGenerator(
            compilation,
            [Asset("Input/Themes/SearchButtonTheme.axaml", TypedControlTheme("Button", "Demo.SearchButtonTheme"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("ControlThemeSemanticPartDescriptor(\"SearchButtonTheme\"");
        source.ShouldNotContain("new global::Demo.SearchButtonTheme()");
    }

    [Theory]
    [InlineData("AbstractButton")]
    [InlineData("BaseButton")]
    public void Does_Not_Load_Abstract_Or_Base_Typed_Theme_As_A_Default_Control_Theme(string controlName)
    {
        var compilation = CreateCompilation($$"""
            namespace Demo
            {
                public sealed class {{controlName}} : Avalonia.Controls.Control
                {
                }

                internal sealed class {{controlName}}Theme : Avalonia.Styling.ControlTheme
                {
                }
            }
            """);
        var result = RunGenerator(
            compilation,
            [Asset(
                $"Button/Themes/{controlName}Theme.axaml",
                TypedControlTheme(controlName, $"Demo.{controlName}Theme"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain($"Button/Themes/{controlName}Theme.axaml");
        source.ShouldNotContain($"new global::Demo.{controlName}Theme()");
    }

    [Fact]
    public void Generates_Empty_Manifest_For_Global_Only_Dictionary_Without_A_Control_Owner()
    {
        var result = RunGenerator(
            CreateCompilation(TokenSource),
            [Asset("Themes/Global.axaml", ResourceDictionary("SharedTokenResource ColorPrimary"))],
            out var diagnostics,
            packageSharedThemes: ["Themes/Global.axaml"]);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("global::System.Array.Empty<global::AtomUI.Theme.Schema.ControlThemeAssetDescriptor>()");
        source.ShouldContain("controlThemes.Add(new GeneratedThemeAssetResource_");
    }

    [Fact]
    public void Assigns_Top_Level_Internal_Target_Theme_To_An_Explicit_Existing_Unit()
    {
        var compilation = CreateCompilation(
            """
            namespace Demo
            {
                public sealed class DataGrid : Avalonia.Controls.Control
                {
                }

                internal sealed class DataGridOperationButtons : Avalonia.Controls.Control
                {
                }
            }
            """,
            sourcePath: "DataGrid.cs");
        var asset = Asset(
            "Themes/DataGridOperationButtons.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui"
                                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                                xmlns:demo="using:Demo">
                <ControlTheme x:Key="{x:Type demo:DataGridOperationButtons}"
                              TargetType="demo:DataGridOperationButtons" />
            </ResourceDictionary>
            """);

        var result = RunGenerator(
            compilation,
            [asset],
            out var diagnostics,
            linkedPublish: true,
            registrationUnits: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [asset.Path] = "DataGrid"
            });

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");
        source.ShouldContain("public static partial class GeneratedRegistrationUnit_DataGrid_");
        source.ShouldContain("builder.AddUnitThemeResource(");
        source.ShouldNotContain("PackageRoot");
    }

    [Fact]
    public void Does_Not_Load_Explicit_Abstract_Typed_Theme_Through_A_Missing_Wrapper()
    {
        var compilation = CreateCompilation(
            """
            namespace Demo
            {
                public sealed class ColorPicker : Avalonia.Controls.Control
                {
                }

                internal sealed class AbstractColorPickerSliderTrack : Avalonia.Controls.Control
                {
                }

                internal sealed class AbstractColorPickerSliderTrackTheme : Avalonia.Styling.ControlTheme
                {
                }
            }
            """,
            sourcePath: "ColorPicker.cs");
        var asset = Asset(
            "Themes/ColorSlider/AbstractColorPickerSliderTrackTheme.axaml",
            TypedControlTheme(
                "AbstractColorPickerSliderTrack",
                "Demo.AbstractColorPickerSliderTrackTheme"));

        var result = RunGenerator(
            compilation,
            [asset],
            out var diagnostics,
            linkedPublish: true,
            registrationUnits: new Dictionary<string, string>(StringComparer.Ordinal)
            {
                [asset.Path] = "ColorPicker"
            });

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetFragments.g.cs");
        source.ShouldContain("public static partial class GeneratedRegistrationUnit_ColorPicker_");
        source.ShouldNotContain("builder.AddUnitThemeResource(");
        source.ShouldNotContain(GetThemeAssetResourceClassName(asset.Path));
        source.ShouldNotContain("PackageRoot");
    }

    [Fact]
    public void Infers_Owner_From_A_Single_Control_Token_Family()
    {
        var result = RunGenerator(
            [Asset(
                "Badge/Themes/AlertAdornerTheme.axaml",
                ResourceDictionary("AlertTokenResource Height"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"AtomUI\", \"Alert\")");
        source.ShouldContain("Badge/Themes/AlertAdornerTheme.axaml");
    }

    [Fact]
    public void Allows_Explicit_Foreign_Control_Token_Dependencies()
    {
        var result = RunGenerator(
            [Asset(
                "Button/Themes/ButtonCompositeTheme.axaml",
                ControlTheme("Button", "ButtonTokenResource Height", "AlertTokenResource Height"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"AtomUI\", \"Button\")");
        source.ShouldContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"AtomUI\", \"Alert\")");
    }

    [Fact]
    public void Prefers_Referenced_Control_With_The_AtomUI_Catalog_Metadata()
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(static path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();
        var avaloniaControls = CSharpCompilation.Create(
            "Avalonia.Controls",
            [CSharpSyntaxTree.ParseText("""
                namespace Avalonia.Controls
                {
                    public class Control
                    {
                    }

                    public sealed class Button : Control
                    {
                    }
                }
                """, cancellationToken: TestContext.Current.CancellationToken)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .ToMetadataReference();
        var atomUIControls = CSharpCompilation.Create(
            "AtomUI.Controls",
            [CSharpSyntaxTree.ParseText("""
                [assembly: System.Reflection.AssemblyMetadata("AtomUIThemeControlCatalog", "AtomUI")]

                namespace AtomUI.Controls
                {
                    public sealed class Button : Avalonia.Controls.Control
                    {
                    }
                }
                """, cancellationToken: TestContext.Current.CancellationToken)],
            references.Add(avaloniaControls),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .ToMetadataReference();
        var compilation = CreateCompilation("""
            using AtomUI.Theme.DesignTokens;

            namespace Demo
            {
                public sealed class Alert : Avalonia.Controls.Control
                {
                }

                [ControlDesignToken]
                internal sealed class AlertToken : AbstractControlDesignToken
                {
                    public double Height { get; set; }
                }
            }
            """).AddReferences(atomUIControls, avaloniaControls);

        var result = RunGenerator(
            compilation,
            [Asset(
                "Alert/Themes/AlertTheme.axaml",
                ControlTheme("Alert", "AlertTokenResource Height", "ButtonTokenResource Height"))],
            out var diagnostics);

        diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(result, "GeneratedControlThemeAssetManifest.g.cs");
        source.ShouldContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"AtomUI\", \"Button\")");
        source.ShouldNotContain(
            "new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"Avalonia.Controls\", \"Button\")");
    }

    [Fact]
    public void Reports_Ambiguous_Control_Owner()
    {
        var compilation = CreateCompilation("""
            namespace First
            {
                public sealed class Rating : Avalonia.Controls.Control
                {
                }
            }

            namespace Second
            {
                public sealed class Rating : Avalonia.Controls.Control
                {
                }
            }
            """);

        RunGenerator(
            compilation,
            [Asset("Themes/RatingTheme.axaml", ControlTheme("Rating"))],
            out var diagnostics);

        diagnostics.ShouldHaveSingleItem().Id.ShouldBe("ATOMUIGEN016");
    }

    [Fact]
    public void Reports_Duplicate_Asset_Uris()
    {
        RunGenerator(
            [
                Asset("Button/Themes/ButtonTheme.axaml", ControlTheme("Button", "ButtonTokenResource Height")),
                Asset("Button/Themes/ButtonTheme.axaml", ControlTheme("Button", "ButtonTokenResource Height"))
            ],
            out var diagnostics);

        diagnostics.ShouldHaveSingleItem().Id.ShouldBe("ATOMUIGEN011");
    }

    [Fact]
    public void Reports_Semantic_Part_Target_That_Is_Not_A_Control()
    {
        var compilation = CreateCompilation("""
            namespace Demo
            {
                public sealed class SearchEdit : Avalonia.Controls.Control
                {
                    public Avalonia.Styling.ControlTheme? SearchButtonTheme { get; set; }
                }

                public sealed class NotAControl
                {
                }
            }
            """);

        RunGenerator(
            compilation,
            [Asset("Input/Themes/SearchButtonTheme.axaml", ControlTheme("NotAControl"))],
            out var diagnostics);

        diagnostics.ShouldHaveSingleItem().Id.ShouldBe("ATOMUIGEN017");
    }

    private static CSharpCompilation RunGenerator(
        IEnumerable<AdditionalText> assets,
        out ImmutableArray<Diagnostic> diagnostics,
        bool linkedPublish = false)
    {
        return RunGenerator(
            CreateCompilation(TokenSource),
            assets,
            out diagnostics,
            linkedPublish: linkedPublish);
    }

    private static CSharpCompilation RunGenerator(
        CSharpCompilation compilation,
        IEnumerable<AdditionalText> assets,
        out ImmutableArray<Diagnostic> diagnostics,
        string? projectDirectory = null,
        string? controlCatalog = null,
        IReadOnlyList<string>? packageSharedThemes = null,
        bool linkedPublish = false,
        IReadOnlyDictionary<string, string>? registrationUnits = null)
    {
        var options = new Dictionary<string, string>(StringComparer.Ordinal);
        if (linkedPublish)
        {
            options["build_property.AtomUILinkedPublish"] = "true";
        }
        if (projectDirectory is not null)
        {
            options["build_property.AtomUIThemeAssetProjectDirectory"] = projectDirectory;
        }
        if (controlCatalog is not null)
        {
            options["build_property.AtomUIThemeControlCatalog"] = controlCatalog;
        }
        if (packageSharedThemes is not null)
        {
            options["build_property.AtomUIPackageSharedThemePaths"] =
                string.Join(";", packageSharedThemes);
        }
        AnalyzerConfigOptionsProvider? optionsProvider = options.Count == 0 && registrationUnits is null
            ? null
            : new TestAnalyzerConfigOptionsProvider(options, registrationUnits);
        var driver = CSharpGeneratorDriver.Create(
            [new ThemeAssetManifestGenerator().AsSourceGenerator()],
            assets.ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            optionsProvider);
        driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var output,
            out diagnostics,
            TestContext.Current.CancellationToken);
        return (CSharpCompilation)output;
    }

    private static CSharpCompilation CreateCompilation(
        string source,
        string assemblyName = "ThemeAssetManifestTests",
        string sourcePath = "TokenSource.cs")
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(static path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();
        return CSharpCompilation.Create(
            assemblyName,
            [
                CSharpSyntaxTree.ParseText(source, path: sourcePath),
                CSharpSyntaxTree.ParseText(AtomUIStubs)
            ],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private static string GetGeneratedSource(CSharpCompilation compilation, string fileName)
    {
        return compilation.SyntaxTrees
                          .Single(tree => tree.FilePath.EndsWith(fileName, StringComparison.Ordinal))
                          .GetText(TestContext.Current.CancellationToken)
                          .ToString();
    }

    private static AdditionalText Asset(string path, string text)
    {
        return new InMemoryAdditionalText(path, text);
    }

    private static string GetThemeAssetResourceClassName(string path)
    {
        var hash = 14695981039346656037UL;
        foreach (var character in path.Replace('\\', '/'))
        {
            hash ^= character;
            hash *= 1099511628211UL;
        }
        return $"GeneratedThemeAssetResource_{hash:X16}";
    }

    private static string ControlTheme(string targetType, params string[] resources)
    {
        var setters = string.Join(
            Environment.NewLine,
            resources.Select((resource, index) =>
                $"    <Setter Property=\"Tag{index}\" Value=\"{{atom:{resource}}}\" />"));
        return $$"""
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                          xmlns:atom="https://atomui.net"
                          xmlns:demo="using:Demo"
                          TargetType="{x:Type demo:{{targetType}}}">
            {{setters}}
            </ControlTheme>
            """;
    }

    private static string TypedControlTheme(string targetType, string className)
    {
        return $$"""
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                          xmlns:demo="using:Demo"
                          x:Class="{{className}}"
                          TargetType="demo:{{targetType}}" />
            """;
    }

    private static string ResourceDictionary(params string[] resources)
    {
        var values = string.Join(
            Environment.NewLine,
            resources.Select((resource, index) =>
                $"    <x:String x:Key=\"Value{index}\">{{atom:{resource}}}</x:String>"));
        return $$"""
            <ResourceDictionary xmlns="https://github.com/avaloniaui"
                                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                                xmlns:atom="https://atomui.net">
            {{values}}
            </ResourceDictionary>
            """;
    }

    private static ulong ComputeFingerprint(params string[] values)
    {
        var hash = 14695981039346656037UL;
        foreach (var value in values)
        {
            foreach (var character in value)
            {
                hash ^= character;
                hash *= 1099511628211UL;
            }
        }
        return hash;
    }

    private const string TokenSource = """
        using AtomUI.Theme.DesignTokens;

        namespace Demo
        {
            public sealed class Button : Avalonia.Controls.Control
            {
            }

            public sealed class Alert : Avalonia.Controls.Control
            {
            }

            [ControlDesignToken]
            internal sealed class ButtonToken : AbstractControlDesignToken
            {
                public double Height { get; set; }
            }

            [ControlDesignToken]
            internal sealed class AlertToken : AbstractControlDesignToken
            {
                public double Height { get; set; }
            }
        }
        """;

    private const string AtomUIStubs = """
        namespace AtomUI.Theme.DesignTokens
        {
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public sealed class ControlDesignTokenAttribute : System.Attribute
            {
            }

            public abstract class AbstractControlDesignToken
            {
            }
        }

        namespace Avalonia.Controls
        {
            public class Control
            {
            }
        }

        namespace Avalonia.Styling
        {
            public class ControlTheme
            {
            }
        }
        """;

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly SourceText _text;

        internal InMemoryAdditionalText(string path, string text)
        {
            Path  = path;
            _text = SourceText.From(text);
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            return _text;
        }
    }

    private sealed class TestAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private static readonly AnalyzerConfigOptions s_empty =
            new TestAnalyzerConfigOptions(new Dictionary<string, string>());
        private readonly AnalyzerConfigOptions _globalOptions;
        private readonly IReadOnlyDictionary<string, string> _registrationUnits;

        internal TestAnalyzerConfigOptionsProvider(
            IReadOnlyDictionary<string, string> globalOptions,
            IReadOnlyDictionary<string, string>? registrationUnits = null)
        {
            _globalOptions = new TestAnalyzerConfigOptions(globalOptions);
            _registrationUnits = registrationUnits ?? new Dictionary<string, string>(StringComparer.Ordinal);
        }

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => s_empty;

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            return _registrationUnits.TryGetValue(textFile.Path, out var unit)
                ? new TestAnalyzerConfigOptions(new Dictionary<string, string>(StringComparer.Ordinal)
                {
                    ["build_metadata.AdditionalFiles.AtomUIRegistrationUnit"] = unit
                })
                : s_empty;
        }
    }

    private sealed class TestAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly IReadOnlyDictionary<string, string> _values;

        internal TestAnalyzerConfigOptions(IReadOnlyDictionary<string, string> values)
        {
            _values = values;
        }

        public override bool TryGetValue(string key, out string value)
        {
            return _values.TryGetValue(key, out value!);
        }
    }
}
