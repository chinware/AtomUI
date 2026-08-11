using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.SemanticParts;

public class SemanticPartGeneratorTests
{
    [Fact]
    public void Generates_Implicit_Root_Declared_Parts_Constants_And_Package_Registration()
    {
        var outputCompilation = RunGenerator(ButtonSource, out var diagnostics, ButtonTheme);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var manifest = GetGeneratedSource(outputCompilation, "GeneratedSemanticPartManifest.g.cs");
        manifest.ShouldContain("internal static class GeneratedSemanticPartManifest");
        manifest.ShouldContain("typeof(global::Demo.Button)");
        manifest.ShouldContain("new global::AtomUI.Theme.Schema.ControlTokenIdentity(\"AtomUI\", \"Button\")");
        manifest.ShouldContain("\"root\"");
        manifest.ShouldContain("null,");
        manifest.ShouldContain("SemanticPartCustomization.Root");
        manifest.ShouldContain("\"icon\"");
        manifest.ShouldContain("\"semantic-icon\"");
        manifest.ShouldContain("typeof(global::Avalonia.Controls.Control)");
        manifest.ShouldContain("SemanticPartCardinality.Multiple");
        manifest.ShouldContain("\"content\"");
        manifest.ShouldContain("\"semantic-content\"");
        manifest.ShouldContain("typeof(global::Avalonia.Controls.Presenters.ContentPresenter)");
        manifest.ShouldNotContain("Activator.CreateInstance");
        manifest.ShouldNotContain("Assembly.GetTypes");

        var constants = GetGeneratedSource(outputCompilation, "ButtonSemanticParts.g.cs");
        constants.ShouldContain("internal static class ButtonSemanticParts");
        constants.ShouldContain("internal const string Icon = \"icon\"");
        constants.ShouldContain("internal const string IconClass = \"semantic-icon\"");
        constants.ShouldContain("internal const string Content = \"content\"");
        constants.ShouldContain("internal const string ContentClass = \"semantic-content\"");
        constants.ShouldNotContain("RootClass");

        var registration = GetGeneratedSource(outputCompilation, "GeneratedControlPackageRegistration.g.cs");
        registration.ShouldContain("GeneratedSemanticPartManifest.GetDescriptors()");
        registration.ShouldContain("selectedSemanticControls");
        registration.ShouldContain("includeIdentity(semanticControl.Identity)");
        registration.ShouldContain("            selectedSemanticControls,");
    }

    [Fact]
    public void Generated_Output_Is_Independent_Of_Attribute_Order()
    {
        var first = GetGeneratedSource(
            RunGenerator(ButtonSource, out _, ButtonTheme),
            "GeneratedSemanticPartManifest.g.cs");
        var reordered = GetGeneratedSource(
            RunGenerator(ReorderedButtonSource, out _, ButtonTheme),
            "GeneratedSemanticPartManifest.g.cs");

        reordered.ShouldBe(first);
    }

    [Fact]
    public void Merges_Semantic_Parts_Declared_Across_Partial_Control_Declarations()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Controls.Presenters;

            namespace Demo;

            [SemanticPart(
                "icon",
                SelectorClass = "semantic-icon",
                ContractType = typeof(Control),
                Cardinality = SemanticPartCardinality.Multiple,
                Since = "6.0")]
            public partial class Button : Control
            {
            }

            [SemanticPart(
                "content",
                SelectorClass = "semantic-content",
                ContractType = typeof(ContentPresenter),
                Since = "6.0")]
            public partial class Button
            {
            }
            """;

        var output = RunGenerator(source, out var diagnostics, ButtonTheme);

        diagnostics.ShouldBeEmpty();
        output.GetDiagnostics(TestContext.Current.CancellationToken)
              .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
              .ShouldBeEmpty();
        output.SyntaxTrees.Count(tree => tree.FilePath.EndsWith(
            "ButtonSemanticParts.g.cs",
            StringComparison.Ordinal)).ShouldBe(1);
        var manifest = GetGeneratedSource(output, "GeneratedSemanticPartManifest.g.cs");
        manifest.ShouldContain("\"icon\"");
        manifest.ShouldContain("\"content\"");
    }

    [Fact]
    public void Reports_Invalid_And_Duplicate_Declarations()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;

            namespace Demo;

            [SemanticPart("Icon", SelectorClass = "icon", ContractType = typeof(Control), Since = "6.0")]
            [SemanticPart("content", SelectorClass = "semantic-content", ContractType = typeof(Control), Since = "6.0")]
            [SemanticPart("content", SelectorClass = "semantic-other", ContractType = typeof(Control), Since = "6.0")]
            public partial class Button : Control
            {
            }
            """;

        _ = RunGenerator(source, out var diagnostics, ButtonTheme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN020");
        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN021");
    }

    [Fact]
    public void Reports_Reserved_Root_Path_And_Selector_Class()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;

            namespace Demo;

            [SemanticPart(
                "icon",
                Path = "root",
                SelectorClass = "semantic-root",
                ContractType = typeof(Control),
                Since = "6.0")]
            public partial class Button : Control
            {
            }
            """;

        _ = RunGenerator(source, out var diagnostics, ButtonTheme);

        diagnostics.Count(diagnostic => diagnostic.Id == "ATOMUIGEN020").ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void Reports_Explicitly_Empty_Path()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;

            namespace Demo;

            [SemanticPart(
                "icon",
                Path = "",
                SelectorClass = "semantic-icon",
                ContractType = typeof(Control),
                Since = "6.0")]
            public partial class Button : Control
            {
            }
            """;

        _ = RunGenerator(source, out var diagnostics, ButtonTheme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN020");
    }

    [Fact]
    public void Generates_Strongly_Typed_Theme_Metadata_For_A_Public_Child_Control()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Styling;

            namespace Demo;

            [SemanticPart(
                "action",
                SelectorClass = "semantic-action",
                ContractType = typeof(Control),
                Customization = SemanticPartCustomization.SelectorAndTheme,
                ThemePropertyName = nameof(ActionTheme),
                Since = "6.0")]
            public partial class Button : Control
            {
                public ControlTheme? ActionTheme { get; set; }
            }
            """;
        var theme = new InMemoryAdditionalText(
            "Button/Themes/ButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:Button">
                <Setter Property="Template">
                    <ControlTemplate>
                        <Control Classes="semantic-action" />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);

        var output = RunGenerator(source, out var diagnostics, theme);

        diagnostics.ShouldBeEmpty();
        var manifest = GetGeneratedSource(output, "GeneratedSemanticPartManifest.g.cs");
        manifest.ShouldContain("ControlThemeSemanticPartDescriptor(\"ActionTheme\", \"global::Avalonia.Controls.Control\")");
    }

    [Fact]
    public void Uses_The_Actual_Compatible_Semantic_Theme_Asset_Target_Type()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Styling;

            namespace Demo;

            public class ActionControl : Control
            {
            }

            [SemanticPart(
                "action",
                SelectorClass = "semantic-action",
                ContractType = typeof(Control),
                Customization = SemanticPartCustomization.SelectorAndTheme,
                ThemePropertyName = nameof(ActionTheme),
                Since = "6.0")]
            public partial class Button : Control
            {
                public ControlTheme? ActionTheme { get; set; }
            }
            """;
        var ownerTheme = new InMemoryAdditionalText(
            "Button/Themes/ButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:Button">
                <Setter Property="Template">
                    <ControlTemplate>
                        <atom:ActionControl Classes="semantic-action" />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);
        var actionTheme = new InMemoryAdditionalText(
            "Button/Themes/ActionTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:ActionControl" />
            """);

        var output = RunGenerator(source, out var diagnostics, ownerTheme, actionTheme);

        diagnostics.ShouldBeEmpty();
        var manifest = GetGeneratedSource(output, "GeneratedSemanticPartManifest.g.cs");
        manifest.ShouldContain(
            "ControlThemeSemanticPartDescriptor(\"ActionTheme\", \"global::Demo.ActionControl\")");
    }

    [Fact]
    public void Reports_Incompatible_Semantic_Theme_Asset_Target_Type()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Styling;

            namespace Demo;

            public class UnrelatedElement : Avalonia.StyledElement
            {
            }

            [SemanticPart(
                "action",
                SelectorClass = "semantic-action",
                ContractType = typeof(Control),
                Customization = SemanticPartCustomization.SelectorAndTheme,
                ThemePropertyName = nameof(ActionTheme),
                Since = "6.0")]
            public partial class Button : Control
            {
                public ControlTheme? ActionTheme { get; set; }
            }
            """;
        var actionTheme = new InMemoryAdditionalText(
            "Button/Themes/ActionTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:UnrelatedElement" />
            """);

        _ = RunGenerator(source, out var diagnostics, ButtonTheme, actionTheme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN023");
    }

    [Fact]
    public void Updated_Theme_Assets_Do_Not_Retain_Stale_Resolved_Target_Types()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Styling;

            namespace Demo;

            public class ActionControl : Control
            {
            }

            [SemanticPart(
                "action",
                SelectorClass = "semantic-action",
                ContractType = typeof(Control),
                Customization = SemanticPartCustomization.SelectorAndTheme,
                ThemePropertyName = nameof(ActionTheme),
                Since = "6.0")]
            public partial class Button : Control
            {
                public ControlTheme? ActionTheme { get; set; }
            }
            """;
        var ownerTheme = new InMemoryAdditionalText(
            "Button/Themes/ButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:Button">
                <Setter Property="Template">
                    <ControlTemplate>
                        <atom:ActionControl Classes="semantic-action" />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);
        var actionTheme = new InMemoryAdditionalText(
            "Button/Themes/ActionTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:ActionControl" />
            """);
        var unrelatedTheme = new InMemoryAdditionalText(
            "Button/Themes/UnusedTheme.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui" />
            """);
        var compilation = CreateCompilation(source);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            [new TokenResourceKeyGenerator().AsSourceGenerator()],
            new AdditionalText[] { ownerTheme, actionTheme }.ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            null);

        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var firstOutput,
            out _,
            TestContext.Current.CancellationToken);
        GetGeneratedSource((CSharpCompilation)firstOutput, "GeneratedSemanticPartManifest.g.cs")
            .ShouldContain("ControlThemeSemanticPartDescriptor(\"ActionTheme\", \"global::Demo.ActionControl\")");

        driver = driver.ReplaceAdditionalText(actionTheme, unrelatedTheme)
                       .RunGeneratorsAndUpdateCompilation(
                           compilation,
                           out var secondOutput,
                           out _,
                           TestContext.Current.CancellationToken);

        GetGeneratedSource((CSharpCompilation)secondOutput, "GeneratedSemanticPartManifest.g.cs")
            .ShouldContain(
                "ControlThemeSemanticPartDescriptor(\"ActionTheme\", \"global::Avalonia.Controls.Control\")");
    }

    [Fact]
    public void Reports_Invalid_Contract_Type_And_Missing_Since()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;

            namespace Demo;

            [SemanticPart("icon", SelectorClass = "semantic-icon", ContractType = typeof(string))]
            public partial class Button : Control
            {
            }
            """;

        _ = RunGenerator(source, out var diagnostics, ButtonTheme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN022");
        diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUIGEN024" && diagnostic.Severity == DiagnosticSeverity.Warning);
    }

    [Fact]
    public void Reports_Generic_Semantic_Control()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;

            namespace Demo;

            [SemanticPart(
                "item",
                SelectorClass = "semantic-item",
                ContractType = typeof(Control),
                RuntimeCreated = true,
                Since = "6.0")]
            public partial class GenericButton<T> : Control
            {
            }
            """;

        _ = RunGenerator(source, out var diagnostics);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN020");
    }

    [Fact]
    public void Reports_Invalid_Strongly_Typed_Theme_Property()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;

            namespace Demo;

            [SemanticPart(
                "action",
                SelectorClass = "semantic-action",
                ContractType = typeof(Control),
                Customization = SemanticPartCustomization.SelectorAndTheme,
                ThemePropertyName = "ActionTheme",
                Since = "6.0")]
            public partial class Button : Control
            {
                public object? ActionTheme { get; set; }
            }
            """;

        _ = RunGenerator(source, out var diagnostics, ButtonTheme);

        var diagnostic = diagnostics.Single(diagnostic => diagnostic.Id == "ATOMUIGEN023");
        diagnostic.GetMessage().ShouldContain("public getter and setter");
    }

    [Fact]
    public void Reports_Strongly_Typed_Theme_Property_Without_A_Public_Setter()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Styling;

            namespace Demo;

            [SemanticPart(
                "action",
                SelectorClass = "semantic-action",
                ContractType = typeof(Control),
                Customization = SemanticPartCustomization.SelectorAndTheme,
                ThemePropertyName = nameof(ActionTheme),
                Since = "6.0")]
            public partial class Button : Control
            {
                public ControlTheme? ActionTheme { get; private set; }
            }
            """;

        _ = RunGenerator(source, out var diagnostics, ButtonTheme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN023");
    }

    [Fact]
    public void Reports_Strongly_Typed_Theme_Property_Without_A_Public_Getter()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Styling;

            namespace Demo;

            [SemanticPart(
                "action",
                SelectorClass = "semantic-action",
                ContractType = typeof(Control),
                Customization = SemanticPartCustomization.SelectorAndTheme,
                ThemePropertyName = nameof(ActionTheme),
                Since = "6.0")]
            public partial class Button : Control
            {
                public ControlTheme? ActionTheme { private get; set; }
            }
            """;

        _ = RunGenerator(source, out var diagnostics, ButtonTheme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN023");
    }

    [Fact]
    public void Reports_Marker_Cardinality_And_Contract_Type_Mismatches()
    {
        var missingMarkerTheme = new InMemoryAdditionalText(
            "Button/Themes/ButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:Button">
                <Setter Property="Template">
                    <ControlTemplate>
                        <Panel>
                            <Control Classes="semantic-icon" />
                        </Panel>
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);
        var incompatibleTheme = new InMemoryAdditionalText(
            "Button/Themes/Browser/ButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:Button">
                <Setter Property="Template">
                    <ControlTemplate>
                        <Control Classes="semantic-content" />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);

        _ = RunGenerator(ButtonSource, out var diagnostics, missingMarkerTheme, incompatibleTheme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN025");
        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN026");
    }

    [Fact]
    public void Validates_A_Derived_Control_Against_Its_Typed_BasedOn_Template()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Controls.Presenters;

            namespace Demo;

            public class BaseButton : Control
            {
            }

            [SemanticPart(
                "content",
                SelectorClass = "semantic-content",
                ContractType = typeof(ContentPresenter),
                Since = "6.0")]
            public partial class DerivedButton : BaseButton
            {
            }
            """;
        var baseTheme = new InMemoryAdditionalText(
            "BaseButton/Themes/BaseButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:BaseButton">
                <Setter Property="Template">
                    <ControlTemplate>
                        <ContentPresenter Classes="semantic-content" />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);
        var derivedTheme = new InMemoryAdditionalText(
            "DerivedButton/Themes/DerivedButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:DerivedButton"
                          BasedOn="{StaticResource {x:Type atom:BaseButton}}" />
            """);

        var output = RunGenerator(source, out var diagnostics, derivedTheme, baseTheme);

        diagnostics.ShouldBeEmpty();
        GetGeneratedSource(output, "GeneratedSemanticPartManifest.g.cs")
            .ShouldContain("typeof(global::Demo.DerivedButton)");
    }

    [Fact]
    public void Validates_The_Inherited_Default_Template_When_A_Derived_Theme_Adds_Only_A_Conditional_Template()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Controls.Presenters;

            namespace Demo;

            public class BaseButton : Control
            {
            }

            [SemanticPart(
                "content",
                SelectorClass = "semantic-content",
                ContractType = typeof(ContentPresenter),
                Since = "6.0")]
            public partial class DerivedButton : BaseButton
            {
            }
            """;
        var baseTheme = new InMemoryAdditionalText(
            "BaseButton/Themes/BaseButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:BaseButton">
                <Setter Property="Template">
                    <ControlTemplate>
                        <Control />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);
        var derivedTheme = new InMemoryAdditionalText(
            "DerivedButton/Themes/DerivedButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:DerivedButton"
                          BasedOn="{StaticResource {x:Type atom:BaseButton}}">
                <Style Selector="^:pointerover">
                    <Setter Property="Template">
                        <ControlTemplate>
                            <ContentPresenter Classes="semantic-content" />
                        </ControlTemplate>
                    </Setter>
                </Style>
            </ControlTheme>
            """);

        _ = RunGenerator(source, out var diagnostics, derivedTheme, baseTheme);

        diagnostics.ShouldContain(diagnostic =>
            diagnostic.Id == "ATOMUIGEN025" &&
            diagnostic.GetMessage().Contains("BaseButtonTheme.axaml#1", StringComparison.Ordinal));
    }

    [Fact]
    public void Does_Not_Validate_A_Base_Template_Replaced_By_A_Direct_Derived_Theme_Setter()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Controls.Presenters;

            namespace Demo;

            public class BaseButton : Control
            {
            }

            [SemanticPart(
                "content",
                SelectorClass = "semantic-content",
                ContractType = typeof(ContentPresenter),
                Since = "6.0")]
            public partial class DerivedButton : BaseButton
            {
            }
            """;
        var baseTheme = new InMemoryAdditionalText(
            "BaseButton/Themes/BaseButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:BaseButton">
                <Setter Property="Template">
                    <ControlTemplate>
                        <Control />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);
        var derivedTheme = new InMemoryAdditionalText(
            "DerivedButton/Themes/DerivedButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:DerivedButton"
                          BasedOn="{StaticResource {x:Type atom:BaseButton}}">
                <Setter Property="Template">
                    <ControlTemplate>
                        <ContentPresenter Classes="semantic-content" />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);

        _ = RunGenerator(source, out var diagnostics, derivedTheme, baseTheme);

        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Typed_BasedOn_Cycles_Report_A_Missing_Template_Without_Recursing_Forever()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Controls.Presenters;

            namespace Demo;

            public class BaseButton : Control
            {
            }

            [SemanticPart(
                "content",
                SelectorClass = "semantic-content",
                ContractType = typeof(ContentPresenter),
                Since = "6.0")]
            public partial class DerivedButton : BaseButton
            {
            }
            """;
        var baseTheme = new InMemoryAdditionalText(
            "BaseButton/Themes/BaseButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:BaseButton"
                          BasedOn="{StaticResource {x:Type atom:DerivedButton}}" />
            """);
        var derivedTheme = new InMemoryAdditionalText(
            "DerivedButton/Themes/DerivedButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:DerivedButton"
                          BasedOn="{StaticResource {x:Type atom:BaseButton}}" />
            """);

        _ = RunGenerator(source, out var diagnostics, derivedTheme, baseTheme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN027");
    }

    [Fact]
    public void Template_Diagnostic_Names_Are_Unique_Within_A_Multi_Theme_Asset()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Controls.Presenters;

            namespace Demo;

            [SemanticPart(
                "content",
                SelectorClass = "semantic-content",
                ContractType = typeof(ContentPresenter),
                Since = "6.0")]
            public partial class Button : Control
            {
            }
            """;
        var theme = new InMemoryAdditionalText(
            "Button/Themes/ButtonVariants.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui"
                                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                                xmlns:atom="using:Demo">
                <ControlTheme x:Key="First" TargetType="atom:Button">
                    <Setter Property="Template">
                        <ControlTemplate>
                            <Control />
                        </ControlTemplate>
                    </Setter>
                </ControlTheme>
                <ControlTheme x:Key="Second" TargetType="atom:Button">
                    <Setter Property="Template">
                        <ControlTemplate>
                            <Control />
                        </ControlTemplate>
                    </Setter>
                </ControlTheme>
            </ResourceDictionary>
            """);

        _ = RunGenerator(source, out var diagnostics, theme);

        var templateDiagnostics = diagnostics.Where(diagnostic => diagnostic.Id == "ATOMUIGEN025").ToArray();
        templateDiagnostics.Length.ShouldBe(2);
        templateDiagnostics.ShouldContain(diagnostic => diagnostic.GetMessage().Contains(
            "Button/Themes/ButtonVariants.axaml#1",
            StringComparison.Ordinal));
        templateDiagnostics.ShouldContain(diagnostic => diagnostic.GetMessage().Contains(
            "Button/Themes/ButtonVariants.axaml#2",
            StringComparison.Ordinal));
    }

    [Fact]
    public void Nested_Xml_Namespace_Declarations_Override_Outer_Prefixes()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;
            using Avalonia.Controls.Presenters;

            namespace Demo;

            [SemanticPart(
                "content",
                SelectorClass = "semantic-content",
                ContractType = typeof(ContentPresenter),
                Since = "6.0")]
            public partial class Button : Control
            {
            }
            """;
        var theme = new InMemoryAdditionalText(
            "Button/Themes/ButtonVariants.axaml",
            """
            <ResourceDictionary xmlns="https://github.com/avaloniaui"
                                xmlns:atom="using:Wrong.Namespace">
                <ControlTheme xmlns:atom="using:Demo" TargetType="atom:Button">
                    <Setter Property="Template">
                        <ControlTemplate>
                            <ContentPresenter Classes="semantic-content" />
                        </ControlTemplate>
                    </Setter>
                </ControlTheme>
            </ResourceDictionary>
            """);

        _ = RunGenerator(source, out var diagnostics, theme);

        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public void Diagnostic_Order_Is_Independent_Of_Theme_Asset_Input_Order()
    {
        var firstTheme = CreateThemeWithoutContentMarker("A/Themes/ButtonTheme.axaml");
        var secondTheme = CreateThemeWithoutContentMarker("B/Themes/ButtonTheme.axaml");

        _ = RunGenerator(ButtonSource, out var forward, firstTheme, secondTheme);
        _ = RunGenerator(ButtonSource, out var reversed, secondTheme, firstTheme);

        forward.Select(static diagnostic => diagnostic.ToString())
               .ShouldBe(reversed.Select(static diagnostic => diagnostic.ToString()));
    }

    [Fact]
    public void Reports_One_Template_Node_Assigned_To_Multiple_Semantic_Parts()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;

            namespace Demo;

            [SemanticPart(
                "leading",
                SelectorClass = "semantic-leading",
                ContractType = typeof(Control),
                Since = "6.0")]
            [SemanticPart(
                "trailing",
                SelectorClass = "semantic-trailing",
                ContractType = typeof(Control),
                Since = "6.0")]
            public partial class Button : Control
            {
            }
            """;
        var theme = new InMemoryAdditionalText(
            "Button/Themes/ButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:Button">
                <Setter Property="Template">
                    <ControlTemplate>
                        <Control Classes="semantic-leading semantic-trailing" />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);

        _ = RunGenerator(source, out var diagnostics, theme);

        diagnostics.ShouldContain(diagnostic => diagnostic.Id == "ATOMUIGEN028");
    }

    [Fact]
    public void Optional_Multiple_RuntimeCreated_And_CrossRoot_Metadata_Are_Validated_Without_Theme_Properties()
    {
        const string source = """
            using AtomUI.Theme;
            using Avalonia.Controls;

            namespace Demo;

            [SemanticPart(
                "icon",
                SelectorClass = "semantic-icon",
                ContractType = typeof(Control),
                Cardinality = SemanticPartCardinality.Multiple,
                Since = "6.0")]
            [SemanticPart(
                "description",
                SelectorClass = "semantic-description",
                ContractType = typeof(Control),
                Cardinality = SemanticPartCardinality.Optional,
                Since = "6.0")]
            [SemanticPart(
                "popup",
                SelectorClass = "semantic-popup",
                ContractType = typeof(Control),
                CrossVisualRoot = true,
                RuntimeCreated = true,
                Since = "6.0")]
            public partial class Button : Control
            {
            }
            """;
        var theme = new InMemoryAdditionalText(
            "Button/Themes/ButtonTheme.axaml",
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:Button">
                <Setter Property="Template">
                    <ControlTemplate>
                        <Panel>
                            <Control Classes="semantic-icon" />
                            <Control Classes="semantic-icon" />
                        </Panel>
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);

        var output = RunGenerator(source, out var diagnostics, theme);

        diagnostics.ShouldBeEmpty();
        var manifest = GetGeneratedSource(output, "GeneratedSemanticPartManifest.g.cs");
        manifest.ShouldContain("\"popup\"");
        manifest.ShouldContain("                        true,");
        manifest.ShouldNotContain("PopupTheme");
    }

    private static CSharpCompilation RunGenerator(
        string source,
        out ImmutableArray<Diagnostic> diagnostics,
        params AdditionalText[] additionalTexts)
    {
        var compilation = CreateCompilation(source);
        var driver = CSharpGeneratorDriver.Create(
            [new TokenResourceKeyGenerator().AsSourceGenerator()],
            additionalTexts.ToImmutableArray(),
            (CSharpParseOptions)compilation.SyntaxTrees[0].Options,
            null);

        driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out diagnostics,
            TestContext.Current.CancellationToken);
        return (CSharpCompilation)outputCompilation;
    }

    private static AdditionalText CreateThemeWithoutContentMarker(string path)
    {
        return new InMemoryAdditionalText(
            path,
            """
            <ControlTheme xmlns="https://github.com/avaloniaui"
                          xmlns:atom="using:Demo"
                          TargetType="atom:Button">
                <Setter Property="Template">
                    <ControlTemplate>
                        <Control Classes="semantic-icon" />
                    </ControlTemplate>
                </Setter>
            </ControlTheme>
            """);
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(static path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();

        return CSharpCompilation.Create(
            "SemanticPartGeneratorTests",
            [CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(AtomUIStubs)],
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

    private const string ButtonSource = """
        using AtomUI.Theme;
        using Avalonia.Controls;
        using Avalonia.Controls.Presenters;

        namespace Demo;

        [SemanticPart(
            "icon",
            SelectorClass = "semantic-icon",
            ContractType = typeof(Control),
            Cardinality = SemanticPartCardinality.Multiple,
            Since = "6.0")]
        [SemanticPart(
            "content",
            SelectorClass = "semantic-content",
            ContractType = typeof(ContentPresenter),
            Since = "6.0")]
        public partial class Button : Control
        {
        }
        """;

    private const string ReorderedButtonSource = """
        using AtomUI.Theme;
        using Avalonia.Controls;
        using Avalonia.Controls.Presenters;

        namespace Demo;

        [SemanticPart(
            "content",
            SelectorClass = "semantic-content",
            ContractType = typeof(ContentPresenter),
            Since = "6.0")]
        [SemanticPart(
            "icon",
            SelectorClass = "semantic-icon",
            ContractType = typeof(Control),
            Cardinality = SemanticPartCardinality.Multiple,
            Since = "6.0")]
        public partial class Button : Control
        {
        }
        """;

    private static readonly AdditionalText ButtonTheme = new InMemoryAdditionalText(
        "Button/Themes/ButtonTheme.axaml",
        """
        <ControlTheme xmlns="https://github.com/avaloniaui"
                      xmlns:atom="using:Demo"
                      TargetType="atom:Button">
            <Setter Property="Template">
                <ControlTemplate>
                    <Panel>
                        <Control Classes="semantic-icon" />
                        <Control Classes="semantic-icon" />
                        <ContentPresenter Classes="semantic-content" />
                    </Panel>
                </ControlTemplate>
            </Setter>
        </ControlTheme>
        """);

    private const string AtomUIStubs = """
        namespace Avalonia
        {
            public class StyledElement
            {
            }
        }

        namespace Avalonia.Controls
        {
            public class Control : Avalonia.StyledElement
            {
            }
        }

        namespace Avalonia.Controls.Presenters
        {
            public class ContentPresenter : Avalonia.Controls.Control
            {
            }
        }

        namespace Avalonia.Styling
        {
            public sealed class ControlTheme
            {
            }
        }

        namespace AtomUI.Theme
        {
            public enum SemanticPartCardinality
            {
                Single,
                Optional,
                Multiple
            }

            public enum SemanticPartCustomization
            {
                Root,
                Selector,
                SelectorAndTheme
            }

            [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
            public sealed class SemanticPartAttribute : System.Attribute
            {
                public SemanticPartAttribute(string name)
                {
                }

                public string? Path { get; set; }
                public string? SelectorClass { get; set; }
                public System.Type? ContractType { get; set; }
                public SemanticPartCardinality Cardinality { get; set; }
                public SemanticPartCustomization Customization { get; set; }
                public string? ThemePropertyName { get; set; }
                public bool CrossVisualRoot { get; set; }
                public string? Since { get; set; }
                public bool RuntimeCreated { get; set; }
            }

            public enum ThemeAppearance : byte
            {
                Light,
                Dark
            }

            public interface IThemeManagerBuilder
            {
                void AddControlPackage(ControlPackageRegistration package);
            }

            public sealed class ControlPackageRegistration
            {
                public ControlPackageRegistration(
                    string id,
                    System.Collections.Generic.IEnumerable<AtomUI.Theme.Schema.ControlTokenDescriptor> controls,
                    System.Collections.Generic.IEnumerable<AtomUI.Theme.Schema.ControlThemeAssetDescriptor> assets,
                    AtomUI.Theme.Resources.IControlThemesProvider provider)
                {
                }

                public ControlPackageRegistration(
                    string id,
                    System.Collections.Generic.IEnumerable<AtomUI.Theme.Schema.ControlTokenDescriptor> controls,
                    System.Collections.Generic.IEnumerable<AtomUI.Theme.Schema.ControlThemeAssetDescriptor> assets,
                    System.Collections.Generic.IEnumerable<AtomUI.Theme.Schema.ControlSemanticDescriptor> semanticControls,
                    AtomUI.Theme.Resources.IControlThemesProvider provider)
                {
                }
            }
        }

        namespace AtomUI.Theme.Resources
        {
            public interface IControlThemesProvider
            {
                string Id { get; }
            }

            public abstract class TokenResourceExtension<TTokenKind>
                where TTokenKind : System.Enum
            {
                protected TokenResourceExtension()
                {
                }

                protected TokenResourceExtension(TTokenKind kind)
                {
                }

                protected virtual object GetResourceKey(TTokenKind kind) => kind;
            }

            public enum SharedTokenKind
            {
                ControlHeight
            }

            public static class ControlTokenResourceKey
            {
                public static object Global(
                    AtomUI.Theme.Schema.ControlTokenIdentity identity,
                    SharedTokenKind kind) => kind;
            }
        }

        namespace AtomUI.Theme.Schema
        {
            public enum TokenStage : byte
            {
                Seed,
                Map,
                Alias,
                Control
            }

            public readonly record struct ControlTokenIdentity(string Catalog, string Id);

            public sealed class TokenDescriptor
            {
            }

            public sealed class ControlTokenDescriptor
            {
                public ControlTokenIdentity Identity { get; }

                public ControlTokenDescriptor(System.Type controlType, ControlTokenIdentity identity)
                {
                    Identity = identity;
                }
            }

            public sealed class ControlThemeAssetDescriptor
            {
                public ControlTokenIdentity OwnerIdentity { get; }
                public System.Collections.Generic.IReadOnlyList<ControlTokenIdentity> ReferencedControlIdentities { get; } =
                    System.Array.Empty<ControlTokenIdentity>();
            }

            public sealed class ControlThemeSemanticPartDescriptor
            {
                public ControlThemeSemanticPartDescriptor(string propertyName, string targetTypeName)
                {
                }
            }

            public sealed class SemanticPartDescriptor
            {
                public SemanticPartDescriptor(
                    string name,
                    string path,
                    string? selectorClass,
                    System.Type contractType,
                    AtomUI.Theme.SemanticPartCardinality cardinality,
                    AtomUI.Theme.SemanticPartCustomization customization,
                    ControlThemeSemanticPartDescriptor? theme,
                    bool crossVisualRoot,
                    string since,
                    bool runtimeCreated)
                {
                }
            }

            public sealed class ControlSemanticDescriptor
            {
                public ControlTokenIdentity Identity { get; }

                public ControlSemanticDescriptor(
                    System.Type controlType,
                    ControlTokenIdentity identity,
                    System.Collections.Generic.IEnumerable<SemanticPartDescriptor> parts)
                {
                    Identity = identity;
                }
            }

            public sealed class ThemeAlgorithmDescriptor
            {
            }
        }

        namespace AtomUI.Theme.DesignTokens
        {
            [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false)]
            public sealed class ControlDesignTokenAttribute : System.Attribute
            {
            }

            public abstract class AbstractDesignToken
            {
            }

            public abstract class AbstractControlDesignToken : AbstractDesignToken
            {
                public virtual void CalculateTokenValues(bool isDark)
                {
                }
            }
        }

        namespace AtomUI.Generated.SemanticPartGeneratorTests
        {
            internal static class GeneratedControlThemeAssetManifest
            {
                internal static System.Collections.Generic.IReadOnlyList<AtomUI.Theme.Schema.ControlThemeAssetDescriptor> GetDescriptors()
                {
                    return System.Array.Empty<AtomUI.Theme.Schema.ControlThemeAssetDescriptor>();
                }
            }

            internal static class GeneratedControlThemeAssetResources
            {
                internal static void AddResources(
                    AtomUI.Theme.Resources.IControlThemesProvider provider,
                    System.Collections.Generic.IReadOnlyList<AtomUI.Theme.Schema.ControlThemeAssetDescriptor> assets)
                {
                }
            }
        }
        """;

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly Microsoft.CodeAnalysis.Text.SourceText _text;

        internal InMemoryAdditionalText(string path, string text)
        {
            Path = path;
            _text = Microsoft.CodeAnalysis.Text.SourceText.From(text);
        }

        public override string Path { get; }

        public override Microsoft.CodeAnalysis.Text.SourceText GetText(
            CancellationToken cancellationToken = default)
        {
            return _text;
        }
    }
}
