using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;
using static AtomUI.Generator.Tests.Localization.LocalizationGeneratorTestHost;

namespace AtomUI.Generator.Tests.Localization;

public class LanguageCatalogSourceGeneratorTests
{
    [Fact]
    public void Emits_The_Existing_Shape_Markup_Extension()
    {
        var execution = RunGenerator();

        execution.Result.Diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(
            execution.Result,
            "TestApp.Localization.LoginLangResourceKind.LanguageCatalog.g.cs");
        source.ShouldBe(ExpectedCatalogSource + "\n");
    }

    [Fact]
    public void Emits_Strongly_Typed_Catalog_And_Compiled_BuiltIn_Bundles()
    {
        var execution = RunGenerator();

        execution.Result.Diagnostics.ShouldBeEmpty();
        var source = GetGeneratedSource(
            execution.Result,
            "TestApp.Localization.LoginLangResourceKind.LanguageCatalogRegistration.g.cs");
        source.ShouldBe(ExpectedCatalogRegistrationSource + "\n");
        GetGeneratedSource(execution.Result, "GeneratedLanguageModuleRegistration.g.cs")
            .ShouldBe(ExpectedModuleSource + "\n");
        source.ShouldNotContain("Assembly.GetTypes");
        source.ShouldNotContain("Type.GetFields");
        source.ShouldNotContain("Enum.GetNames");
        source.ShouldNotContain("GetCustomAttributes");
        source.ShouldNotContain("Activator.CreateInstance");
        source.ShouldNotContain("System.Xml");
        source.ShouldNotContain("System.IO.File");
    }

    [Fact]
    public void Generated_Catalog_Sources_Compile_Without_Errors()
    {
        var execution = RunGenerator();

        execution.Result.GeneratedSources.Length.ShouldBe(3);
        execution.DriverDiagnostics.ShouldBeEmpty();
        execution.OutputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                 .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                 .ShouldBeEmpty();
    }

    [Fact]
    public void Appends_Extension_When_The_Catalog_Name_Does_Not_End_In_Kind()
    {
        var execution = RunWithOutputCompilation(
            CatalogAndRuntimeSource.Replace("LoginLangResourceKind", "LoginMessages"),
            LanguageFile(
                "Localization/en-US.xlf",
                SourceXliff.Replace("LoginLangResourceKind", "LoginMessages")),
            LanguageFile(
                "Localization/zh-CN.xlf",
                TargetXliff("zh-CN", "标题", "项目 {0}")
                    .Replace("LoginLangResourceKind", "LoginMessages")));

        execution.Result.Diagnostics.ShouldBeEmpty();
        GetGeneratedSource(
                execution.Result,
                "TestApp.Localization.LoginMessages.LanguageCatalog.g.cs")
            .ShouldContain("public sealed class LoginMessagesExtension");
    }

    [Fact]
    public void Escapes_Keyword_Unit_Identifiers_In_The_Generated_Switch()
    {
        var source = CatalogAndRuntimeSource.Replace("Title = 10", "@class = 10");
        var sourceXliff = SourceXliff.Replace("name=\"Title\"", "name=\"class\"");
        var targetXliff = TargetXliff("zh-CN", "类型", "项目 {0}")
            .Replace("name=\"Title\"", "name=\"class\"");
        var execution = RunWithOutputCompilation(
            source,
            LanguageFile("Localization/en-US.xlf", sourceXliff),
            LanguageFile("Localization/zh-CN.xlf", targetXliff));

        var moduleSource = GetGeneratedSource(
            execution.Result,
            "TestApp.Localization.LoginLangResourceKind.LanguageCatalogRegistration.g.cs");
        moduleSource.ShouldContain("LoginLangResourceKind.@class => 0");
        execution.OutputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                 .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                 .ShouldBeEmpty();
    }

    private static TestGeneratorExecution RunGenerator()
    {
        return RunWithOutputCompilation(
            CatalogAndRuntimeSource,
            LanguageFile("Localization/en-US.xlf", SourceXliff),
            LanguageFile("Localization/zh-CN.xlf", TargetXliff("zh-CN", "打开 \"文件\" C:\\临时", "项目 {0}")),
            LanguageFile("Localization/zh-TW.xlf", TargetXliff("zh-TW", "開啟 \"檔案\" C:\\暫存", "項目 {0}")));
    }

    private static string GetGeneratedSource(GeneratorRunResult result, string hintName)
    {
        return result.GeneratedSources
                     .Single(source => source.HintName == hintName)
                     .SourceText
                     .ToString();
    }

    private static TestAdditionalText LanguageFile(string path, string content)
    {
        return new TestAdditionalText(
            path,
            content,
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["build_metadata.AdditionalFiles.AtomUILanguage"] = "true",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceKind"] = "ModuleBuiltIn",
                ["build_metadata.AdditionalFiles.AtomUILanguageSourceIdentity"] = "Test.Package"
            });
    }

    private static string TargetXliff(string language, string title, string itemCount)
    {
        return $$"""
            <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US" trgLang="{{language}}">
              <file id="TestApp.Localization.LoginLangResourceKind">
                <unit id="30" name="ItemCount"><segment><source>Items {0}</source><target state="translated">{{itemCount}}</target></segment></unit>
                <unit id="10" name="Title"><segment><source>Open "file" C:\Temp</source><target state="translated">{{title}}</target></segment></unit>
              </file>
            </xliff>
            """;
    }

    private const string CatalogAndRuntimeSource = """
        #nullable enable
        namespace AtomUI.Localization
        {
            [System.AttributeUsage(System.AttributeTargets.Enum, AllowMultiple = false)]
            public sealed class LanguageCatalogAttribute : System.Attribute
            {
                public int ContractVersion { get; set; } = 1;
            }

            public abstract class LanguageResourceExtension<TResourceKind>
                where TResourceKind : struct, System.Enum
            {
                protected LanguageResourceExtension() { }
                protected LanguageResourceExtension(TResourceKind kind) { }
            }

            public interface ILocalizationBuilder
            {
                void AddCatalog(LanguageCatalogDescriptor descriptor);
                void AddTranslationBundle(TranslationBundleDescriptor descriptor);
            }

            public abstract class LanguageCatalogDescriptor { }

            public sealed class LanguageCatalogDescriptor<TResourceKind> : LanguageCatalogDescriptor
                where TResourceKind : struct, System.Enum
            {
                public LanguageCatalogDescriptor(
                    string catalogId,
                    int contractVersion,
                    System.Collections.Generic.IReadOnlyList<LanguageCatalogUnitDescriptor> units,
                    System.Func<TResourceKind, int> unitSlotResolver) { }
            }

            public sealed class LanguageCatalogUnitDescriptor
            {
                public LanguageCatalogUnitDescriptor(int id, string name, bool isFormatted = false) { }
            }

            public readonly struct LanguageTag
            {
                public static LanguageTag Parse(string value) => default;
            }

            public enum TranslationSourceKind : byte
            {
                ModuleBuiltIn,
                StaticLanguagePack,
                ApplicationOverride
            }

            public sealed class TranslationBundleDescriptor
            {
                public TranslationBundleDescriptor(
                    string catalogId,
                    int contractVersion,
                    LanguageTag language,
                    TranslationSourceKind sourceKind,
                    string sourceIdentity,
                    System.Collections.Generic.IReadOnlyList<string?> values) { }
            }
        }

        namespace TestApp.Localization
        {
            [AtomUI.Localization.LanguageCatalog(ContractVersion = 1)]
            public enum LoginLangResourceKind
            {
                ItemCount = 30,
                Title = 10
            }
        }
        """;

    private const string SourceXliff = """
        <xliff xmlns="urn:oasis:names:tc:xliff:document:2.0" version="2.1" srcLang="en-US">
          <file id="TestApp.Localization.LoginLangResourceKind">
            <unit id="30" name="ItemCount"><segment><source>Items {0}</source></segment></unit>
            <unit id="10" name="Title"><segment><source>Open "file" C:\Temp</source></segment></unit>
          </file>
        </xliff>
        """;

    private const string ExpectedCatalogSource = """
        // <auto-generated />
        #nullable enable

        namespace TestApp.Localization
        {
            public sealed class LoginLangResourceExtension
                : global::AtomUI.Localization.LanguageResourceExtension<global::TestApp.Localization.LoginLangResourceKind>
            {
                public LoginLangResourceExtension()
                {
                }

                public LoginLangResourceExtension(
                    global::TestApp.Localization.LoginLangResourceKind kind)
                    : base(kind)
                {
                }
            }
        }
        """;

    private const string ExpectedCatalogRegistrationSource = """
        // <auto-generated />
        #nullable enable

        namespace AtomUI.Generated.TestApp
        {
            internal static class GeneratedLanguageCatalog_82D78C442594E820
            {
                internal static void Register(global::AtomUI.Localization.ILocalizationBuilder builder)
                {
                    global::System.ArgumentNullException.ThrowIfNull(builder);
                    builder.AddCatalog(
                        new global::AtomUI.Localization.LanguageCatalogDescriptor<global::TestApp.Localization.LoginLangResourceKind>(
                            "Test.Package:TestApp.Localization.LoginLangResourceKind",
                            1,
                            new global::AtomUI.Localization.LanguageCatalogUnitDescriptor[]
                            {
                                new global::AtomUI.Localization.LanguageCatalogUnitDescriptor(10, "Title", false),
                                new global::AtomUI.Localization.LanguageCatalogUnitDescriptor(30, "ItemCount", true),
                            },
                            static kind => kind switch
                            {
                                global::TestApp.Localization.LoginLangResourceKind.Title => 0,
                                global::TestApp.Localization.LoginLangResourceKind.ItemCount => 1,
                                _ => -1,
                            }));
                    builder.AddTranslationBundle(
                        new global::AtomUI.Localization.TranslationBundleDescriptor(
                            "Test.Package:TestApp.Localization.LoginLangResourceKind",
                            1,
                            global::AtomUI.Localization.LanguageTag.Parse("en-US"),
                            global::AtomUI.Localization.TranslationSourceKind.ModuleBuiltIn,
                            "Test.Package",
                            new string?[]
                            {
                                "Open \"file\" C:\\Temp",
                                "Items {0}",
                            }));
                    builder.AddTranslationBundle(
                        new global::AtomUI.Localization.TranslationBundleDescriptor(
                            "Test.Package:TestApp.Localization.LoginLangResourceKind",
                            1,
                            global::AtomUI.Localization.LanguageTag.Parse("zh-CN"),
                            global::AtomUI.Localization.TranslationSourceKind.ModuleBuiltIn,
                            "Test.Package",
                            new string?[]
                            {
                                "打开 \"文件\" C:\\临时",
                                "项目 {0}",
                            }));
                    builder.AddTranslationBundle(
                        new global::AtomUI.Localization.TranslationBundleDescriptor(
                            "Test.Package:TestApp.Localization.LoginLangResourceKind",
                            1,
                            global::AtomUI.Localization.LanguageTag.Parse("zh-TW"),
                            global::AtomUI.Localization.TranslationSourceKind.ModuleBuiltIn,
                            "Test.Package",
                            new string?[]
                            {
                                "開啟 \"檔案\" C:\\暫存",
                                "項目 {0}",
                            }));
                }
            }
        }
        """;

    private const string ExpectedModuleSource = """
        // <auto-generated />
        #nullable enable

        [assembly: global::System.Reflection.AssemblyMetadata("AtomUILanguageModuleId", "Test.Package")]

        namespace AtomUI.Generated.TestApp
        {
            internal static class GeneratedLanguageModuleRegistration
            {
                internal static void Register(global::AtomUI.Localization.ILocalizationBuilder builder)
                {
                    global::System.ArgumentNullException.ThrowIfNull(builder);
                    global::AtomUI.Generated.TestApp.GeneratedLanguageCatalog_82D78C442594E820.Register(builder);
                }
            }
        }
        """;
}
