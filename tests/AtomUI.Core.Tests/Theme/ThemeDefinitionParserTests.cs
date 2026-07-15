using System.Reflection;
using System.Text;
using AtomUI.Theme;
using AtomUI.Theme.Definitions;
using Shouldly;
using Xunit;
using AtomUITheme = AtomUI.Theme.Theme;

namespace AtomUI.Core.Tests.Theme;

public class ThemeDefinitionParserTests
{
    private const string FilePath = "themes/test.xml";

    private static readonly IReadOnlySet<string> s_sharedTokenNames =
        new HashSet<string>(StringComparer.Ordinal)
        {
            "ColorPrimary",
            "BorderRadius"
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> s_componentTokenNames =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            ["Button"] = new HashSet<string>(StringComparer.Ordinal)
            {
                "BorderColor",
                "ContentFontSize",
                "ColorPrimary"
            }
        };

    [Theory]
    [InlineData("<Theme Name='T' IsDefault='true'><Algorithms>Default,Dark</Algorithms><SharedTokens><Token Name='ColorPrimary' Value='#ff0000'/></SharedTokens><ControlTokens/></Theme>")]
    [InlineData("""
                <Theme Name="T" IsDefault="true">
                  <Algorithms>Default, Dark</Algorithms>
                  <SharedTokens>
                    <Token Name="ColorPrimary">#ff0000</Token>
                  </SharedTokens>
                  <ControlTokens />
                </Theme>
                """)]
    public void Parse_Is_Independent_Of_Formatting(string xml)
    {
        var result = Parse(xml);

        result.Success.ShouldBeTrue();
        result.Diagnostics.ShouldBeEmpty();
        result.Definition!.Algorithms.ShouldBe(
            [ThemeAlgorithm.Default, ThemeAlgorithm.Dark]);
        result.Definition.SharedTokens["ColorPrimary"].ShouldBe("#ff0000");
    }

    [Fact]
    public void Parse_Preserves_First_Seen_Algorithm_Order()
    {
        var result = Parse("""
                           <Theme Name="T" IsDefault="false">
                             <Algorithms>Dark, Default, Dark, Compact, Default</Algorithms>
                           </Theme>
                           """);

        result.Success.ShouldBeTrue();
        result.Definition!.Algorithms.ShouldBe(
            [ThemeAlgorithm.Dark, ThemeAlgorithm.Default, ThemeAlgorithm.Compact]);
    }

    [Fact]
    public void Parse_Reads_IsShared_Before_Body_Content_And_Uses_The_Correct_Schema()
    {
        var result = Parse("""
                           <Theme Name="T" IsDefault="true">
                             <ControlTokens>
                               <ControlToken Id="Button" EnableAlgorithm="true">
                                 <Token Name="ColorPrimary" IsShared="true">#112233</Token>
                                 <Token Name="BorderColor">#445566</Token>
                               </ControlToken>
                             </ControlTokens>
                           </Theme>
                           """);

        result.Success.ShouldBeTrue();
        var button = result.Definition!.ControlTokens["Button"];
        button.EnableAlgorithm.ShouldBeTrue();
        button.SharedTokens["ColorPrimary"].ShouldBe("#112233");
        button.Tokens["BorderColor"].ShouldBe("#445566");
    }

    [Fact]
    public void Parse_Accepts_Self_Closing_Containers()
    {
        var result = Parse("<Theme Name='T' IsDefault='false'><Algorithms/><SharedTokens/><ControlTokens/></Theme>");

        result.Success.ShouldBeTrue();
        result.Definition!.Algorithms.ShouldBeEmpty();
        result.Definition.SharedTokens.ShouldBeEmpty();
        result.Definition.ControlTokens.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("<Theme Name='T' IsDefault='true'><SharedTokens><Token Name='ColorPrimary' Value='a'/><Token Name='ColorPrimary' Value='b'/></SharedTokens></Theme>", "/Theme/SharedTokens/Token[@Name='ColorPrimary']")]
    [InlineData("<Theme Name='T' IsDefault='true'><ControlTokens><ControlToken Id='Button'/><ControlToken Id='Button'/></ControlTokens></Theme>", "/Theme/ControlTokens/ControlToken[@Id='Button']")]
    [InlineData("<Theme Name='T' IsDefault='true'><ControlTokens><ControlToken Id='Button'><Token Name='BorderColor' Value='a'/><Token Name='BorderColor' Value='b'/></ControlToken></ControlTokens></Theme>", "/Theme/ControlTokens/ControlToken[@Id='Button']/Token[@Name='BorderColor']")]
    public void Parse_Reports_Duplicate_Keys(string xml, string expectedPath)
    {
        var diagnostic = Parse(xml).Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM007");
        diagnostic.Severity.ShouldBe(ThemeDiagnosticSeverity.Error);
        diagnostic.Path.ShouldBe(expectedPath);
    }

    [Theory]
    [InlineData("<Theme Name='T' IsDefault='true'><SharedTokens><Token Name='ColorPrimary'/><Token Name='ColorPrimary' Value='b'/></SharedTokens></Theme>", "ATMTHM006")]
    [InlineData("<Theme Name='T' IsDefault='true'><SharedTokens><Token Name='Unknown' Value='a'/><Token Name='Unknown' Value='b'/></SharedTokens></Theme>", "ATMTHM008")]
    [InlineData("<Theme Name='T' IsDefault='true'><ControlTokens><ControlToken Id='Button'><Token Name='Unknown' Value='a'/><Token Name='Unknown' Value='b'/></ControlToken></ControlTokens></Theme>", "ATMTHM009")]
    public void Parse_Reports_Duplicate_After_Invalid_Or_Unknown_First_Occurrence(
        string xml,
        string firstDiagnosticCode)
    {
        var diagnosticCodes = Parse(xml).Diagnostics.Select(static diagnostic => diagnostic.Code);

        diagnosticCodes.ShouldBe([firstDiagnosticCode, "ATMTHM007"]);
    }

    [Fact]
    public void Parse_Tracks_Component_Own_And_Shared_Names_In_Separate_Scopes()
    {
        var result = Parse("""
                           <Theme Name="T" IsDefault="true">
                             <ControlTokens>
                               <ControlToken Id="Button">
                                 <Token Name="ColorPrimary" IsShared="true" Value="shared" />
                                 <Token Name="ColorPrimary" Value="own" />
                               </ControlToken>
                             </ControlTokens>
                           </Theme>
                           """);

        result.Success.ShouldBeTrue();
        var button = result.Definition!.ControlTokens["Button"];
        button.SharedTokens["ColorPrimary"].ShouldBe("shared");
        button.Tokens["ColorPrimary"].ShouldBe("own");
    }

    [Fact]
    public void Parse_Reports_Missing_Document_Root()
    {
        var result = Parse(string.Empty);
        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();

        result.Success.ShouldBeFalse();
        result.Definition.ShouldBeNull();
        diagnostic.ShouldBe(new ThemeDefinitionDiagnostic(
                                "ATMTHM001",
                                ThemeDiagnosticSeverity.Error,
                                FilePath,
                                0,
                                0,
                                "/",
                                "The theme definition is not well-formed XML."));
    }

    [Fact]
    public void Parse_Reports_An_Invalid_Root_Element()
    {
        var diagnostic = Parse("<NotTheme />").Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM002");
        diagnostic.Path.ShouldBe("/NotTheme");
        diagnostic.Message.ShouldBe("Expected root element 'Theme', but found 'NotTheme'.");
    }

    [Fact]
    public void Parse_Reports_Unsupported_Structure()
    {
        var diagnostic = Parse("<Theme Name='T' IsDefault='true'><Unknown /></Theme>")
                         .Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM002");
        diagnostic.Path.ShouldBe("/Theme/Unknown");
        diagnostic.Message.ShouldBe("Element 'Unknown' is not valid under 'Theme'.");
    }

    [Fact]
    public void Parse_Rejects_Namespace_Qualified_Lookalike_Attributes()
    {
        var result = Parse("<Theme xmlns:fake='urn:test' Name='T' IsDefault='true' fake:IsDefault='false' />");
        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();

        result.Success.ShouldBeFalse();
        diagnostic.Code.ShouldBe("ATMTHM002");
        diagnostic.Path.ShouldBe("/Theme/@{urn:test}IsDefault");
        diagnostic.Message.ShouldBe("Attribute '{urn:test}IsDefault' is not valid on element 'Theme'.");
    }

    [Fact]
    public void Parse_Reports_Missing_Required_Attributes()
    {
        var diagnostic = Parse("<Theme IsDefault='true' />").Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM003");
        diagnostic.Path.ShouldBe("/Theme/@Name");
        diagnostic.Message.ShouldBe("Required attribute 'Name' is missing or empty.");
    }

    [Theory]
    [InlineData("<Theme Name='T' IsDefault='sometimes' />", "/Theme/@IsDefault", "IsDefault")]
    [InlineData("<Theme Name='T' IsDefault='true'><ControlTokens><ControlToken Id='Button' EnableAlgorithm='sometimes'/></ControlTokens></Theme>", "/Theme/ControlTokens/ControlToken[@Id='Button']/@EnableAlgorithm", "EnableAlgorithm")]
    [InlineData("<Theme Name='T' IsDefault='true'><ControlTokens><ControlToken Id='Button'><Token Name='BorderColor' IsShared='sometimes' Value='x'/></ControlToken></ControlTokens></Theme>", "/Theme/ControlTokens/ControlToken[@Id='Button']/Token[@Name='BorderColor']/@IsShared", "IsShared")]
    public void Parse_Reports_Invalid_Boolean_Attributes(string xml, string expectedPath, string attributeName)
    {
        var diagnostic = Parse(xml).Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM004");
        diagnostic.Path.ShouldBe(expectedPath);
        diagnostic.Message.ShouldBe($"Attribute '{attributeName}' must be 'true' or 'false'.");
    }

    [Fact]
    public void Parse_Reports_Unknown_Algorithms()
    {
        var diagnostic = Parse("<Theme Name='T' IsDefault='true'><Algorithms>Default, Solarized</Algorithms></Theme>")
                         .Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM005");
        diagnostic.Path.ShouldBe("/Theme/Algorithms");
        diagnostic.Message.ShouldBe("Algorithm 'Solarized' is not supported.");
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    public void Parse_Rejects_Numeric_Algorithm_Values(string algorithmName)
    {
        var diagnostic = Parse($"<Theme Name='T' IsDefault='true'><Algorithms>{algorithmName}</Algorithms></Theme>")
                         .Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM005");
        diagnostic.Path.ShouldBe("/Theme/Algorithms");
        diagnostic.Message.ShouldBe($"Algorithm '{algorithmName}' is not supported.");
    }

    [Theory]
    [InlineData("<Token Name='ColorPrimary'/>")]
    [InlineData("<Token Name='ColorPrimary' Value='#ffffff'>#000000</Token>")]
    public void Parse_Requires_Exactly_One_Non_Empty_Token_Value_Source(string token)
    {
        var diagnostic = Parse($"<Theme Name='T' IsDefault='true'><SharedTokens>{token}</SharedTokens></Theme>")
                         .Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM006");
        diagnostic.Path.ShouldBe("/Theme/SharedTokens/Token[@Name='ColorPrimary']");
        diagnostic.Message.ShouldBe(
            "Token 'ColorPrimary' must specify exactly one non-empty value using either the 'Value' attribute or element content.");
    }

    [Fact]
    public void Parse_Reports_Unknown_Global_Tokens()
    {
        var diagnostic = Parse("<Theme Name='T' IsDefault='true'><SharedTokens><Token Name='Unknown' Value='x'/></SharedTokens></Theme>")
                         .Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM008");
        diagnostic.Path.ShouldBe("/Theme/SharedTokens/Token[@Name='Unknown']");
        diagnostic.Message.ShouldBe("Shared token 'Unknown' is not registered.");
    }

    [Fact]
    public void Parse_Reports_Unknown_Own_Tokens_For_Known_Components()
    {
        var diagnostic = Parse("<Theme Name='T' IsDefault='true'><ControlTokens><ControlToken Id='Button'><Token Name='Unknown' Value='x'/></ControlToken></ControlTokens></Theme>")
                         .Diagnostics.ShouldHaveSingleItem();

        diagnostic.Code.ShouldBe("ATMTHM009");
        diagnostic.Path.ShouldBe("/Theme/ControlTokens/ControlToken[@Id='Button']/Token[@Name='Unknown']");
        diagnostic.Message.ShouldBe("Token 'Unknown' is not registered for component 'Button'.");
    }

    [Fact]
    public void Parse_Warns_And_Preserves_Optional_Unregistered_Components()
    {
        var result = Parse("<Theme Name='T' IsDefault='true'><ControlTokens><ControlToken Id='OptionalWidget' EnableAlgorithm='true'><Token Name='Accent' Value='x'/></ControlToken></ControlTokens></Theme>");
        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();

        result.Success.ShouldBeTrue();
        diagnostic.Code.ShouldBe("ATMTHM010");
        diagnostic.Severity.ShouldBe(ThemeDiagnosticSeverity.Warning);
        diagnostic.Path.ShouldBe("/Theme/ControlTokens/ControlToken[@Id='OptionalWidget']");
        diagnostic.Message.ShouldBe(
            "Component 'OptionalWidget' is not registered; its own tokens were preserved without schema validation.");
        result.Definition!.ControlTokens["OptionalWidget"].Tokens["Accent"].ShouldBe("x");
    }

    [Fact]
    public void Parse_Reports_File_Line_Column_And_Path()
    {
        var result = Parse("""
                           <Theme Name="T" IsDefault="true">
                             <SharedTokens>
                               <Token Name="Unknown" Value="x" />
                             </SharedTokens>
                           </Theme>
                           """);
        var diagnostic = result.Diagnostics.ShouldHaveSingleItem();

        diagnostic.FilePath.ShouldBe(FilePath);
        diagnostic.Line.ShouldBe(3);
        diagnostic.Column.ShouldBe(6);
        diagnostic.Path.ShouldBe("/Theme/SharedTokens/Token[@Name='Unknown']");
    }

    [Fact]
    public void Parse_Prohibits_Dtds_Without_Closing_The_Input_Stream()
    {
        using var stream = CreateStream("""
                                        <!DOCTYPE Theme [<!ENTITY primary "#ff0000">]>
                                        <Theme Name="T" IsDefault="true">
                                          <SharedTokens>
                                            <Token Name="ColorPrimary">&primary;</Token>
                                          </SharedTokens>
                                        </Theme>
                                        """);

        var result = Parse(stream);

        result.Diagnostics.ShouldHaveSingleItem().Code.ShouldBe("ATMTHM001");
        stream.CanRead.ShouldBeTrue();
    }

    [Fact]
    public void Parse_Does_Not_Close_The_Input_Stream_On_Success()
    {
        using var stream = CreateStream("<Theme Name='T' IsDefault='true' />");

        Parse(stream).Success.ShouldBeTrue();

        stream.CanRead.ShouldBeTrue();
    }

    [Fact]
    public void ThemeDefinition_Copies_Inputs_And_Exposes_Read_Only_Collections()
    {
        var algorithms = new List<ThemeAlgorithm> { ThemeAlgorithm.Dark };
        var sharedTokens = new Dictionary<string, string> { ["ColorPrimary"] = "red" };
        var buttonTokens = new Dictionary<string, string> { ["BorderColor"] = "blue" };
        var button = new ThemeControlTokenDefinition(
            "Button",
            false,
            buttonTokens,
            new Dictionary<string, string>());
        var controlTokens = new Dictionary<string, ThemeControlTokenDefinition> { ["Button"] = button };

        var definition = new ThemeDefinition(
            "test",
            "T",
            true,
            algorithms,
            sharedTokens,
            controlTokens);

        algorithms.Clear();
        sharedTokens["ColorPrimary"] = "changed";
        buttonTokens["BorderColor"] = "changed";

        definition.Algorithms.ShouldBe([ThemeAlgorithm.Dark]);
        definition.SharedTokens["ColorPrimary"].ShouldBe("red");
        definition.ControlTokens["Button"].Tokens["BorderColor"].ShouldBe("blue");
        Should.Throw<NotSupportedException>(() =>
            ((IList<ThemeAlgorithm>)definition.Algorithms).Add(ThemeAlgorithm.Compact));
        Should.Throw<NotSupportedException>(() =>
            ((IDictionary<string, string>)definition.SharedTokens).Add("BorderRadius", "8"));
        Should.Throw<NotSupportedException>(() =>
            definition.ControlTokens["Button"].Tokens.Add("ContentFontSize", "14"));
        typeof(ThemeControlTokenDefinition).GetProperty(nameof(ThemeControlTokenDefinition.EnableAlgorithm))!
                                           .SetMethod.ShouldBeNull();
        typeof(ThemeControlTokenDefinition).GetProperty(nameof(ThemeControlTokenDefinition.Tokens))!
                                           .SetMethod.ShouldBeNull();
    }

    [Fact]
    public void ThemeDefinition_Has_No_PostConstruction_Mutation_Surface()
    {
        var legacyMutationMethods = typeof(ThemeDefinition)
                                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                                    .Where(static method => method.Name.StartsWith("Legacy", StringComparison.Ordinal))
                                    .Select(static method => method.Name);

        legacyMutationMethods.ShouldBeEmpty();
    }

    [Fact]
    public void Legacy_Theme_Definition_Loading_Type_Is_Absent_After_Catalog_Migration()
    {
        var legacyTypeName = string.Concat("AtomUI.Theme.ThemeDefinition", "Reader");

        typeof(AtomUITheme).Assembly.GetType(legacyTypeName).ShouldBeNull();
    }

    private static ThemeDefinitionParseResult Parse(string xml)
    {
        using var stream = CreateStream(xml);
        return Parse(stream);
    }

    private static ThemeDefinitionParseResult Parse(Stream stream)
    {
        return ThemeDefinitionParser.Parse(new ThemeDefinitionParseRequest(
                                               stream,
                                               FilePath,
                                               s_sharedTokenNames,
                                               s_componentTokenNames));
    }

    private static MemoryStream CreateStream(string xml)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(xml));
    }
}
