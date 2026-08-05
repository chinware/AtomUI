using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.Language;

public class LanguageProviderConstructorGeneratorTests
{
    [Fact]
    public void GeneratesExplicitBaseConstructorForPartialLanguageProvider()
    {
        var compilation = CreateCompilation("""
            using System;
            using AtomUI.Theme.Language;

            [assembly: LanguageSgMetaInfo("TestApp.Localization")]

            namespace AtomUI.Theme.Language
            {
                public enum LanguageCode
                {
                    zh_CN,
                    zh_TW,
                    en_US,
                }

                [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
                public sealed class LanguageSgMetaInfoAttribute : Attribute
                {
                    public LanguageSgMetaInfoAttribute(string? targetNamespace)
                    {
                        TargetNamespace = targetNamespace;
                    }

                    public string? TargetNamespace { get; }
                }

                [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                public sealed class LanguageProviderAttribute : Attribute
                {
                    public LanguageProviderAttribute(LanguageCode languageCode, string languageId = "Default")
                    {
                        LanguageCode = languageCode;
                        LanguageId = languageId;
                    }

                    public LanguageCode LanguageCode { get; }

                    public string LanguageId { get; }
                }

                public class LanguageResourceExtension<T>
                {
                    public LanguageResourceExtension()
                    {
                    }

                    public LanguageResourceExtension(T key)
                    {
                    }
                }

                public abstract class LanguageProvider
                {
                    protected LanguageProvider()
                    {
                    }

                    protected LanguageProvider(LanguageCode langCode, string langId)
                    {
                    }

                    protected virtual Type GetResourceKindType()
                    {
                        return typeof(object);
                    }

                    protected void LogBuildResourceDictionaryError(Type resourceKindType)
                    {
                    }

                    public virtual void BuildResourceDictionary(Avalonia.Controls.IResourceDictionary dictionary)
                    {
                    }
                }
            }

            namespace Avalonia.Controls
            {
                public interface IResourceDictionary
                {
                    object? this[object key] { get; set; }
                }
            }

            namespace TestApp.ShowCases.Demo
            {
                internal static class DemoShowCase
                {
                    public const string LanguageId = "DemoShowCase";
                }
            }

            namespace TestApp.ShowCases.Demo.Localization
            {
                [LanguageProvider(LanguageCode.en_US, DemoShowCase.LanguageId)]
                internal partial class en_US : LanguageProvider
                {
                    public const string Title = "Title";
                }
            }
            """);

        var cancellationToken = TestContext.Current.CancellationToken;
        var driver            = CSharpGeneratorDriver.Create(new LanguageGenerator());

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics, cancellationToken);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(cancellationToken)
                         .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("LanguageProviderConstructors.g.cs"))
                                               .GetText(cancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("namespace TestApp.ShowCases.Demo.Localization");
        generatedSource.ShouldContain("internal partial class en_US");
        generatedSource.ShouldContain("public en_US()");
        generatedSource.ShouldContain(": base(global::AtomUI.Theme.Language.LanguageCode.en_US, \"DemoShowCase\")");
    }

    [Fact]
    public void SupportsPartialLanguageConstantsWithoutLanguageProviderInheritance()
    {
        var compilation = CreateCompilation("""
            using System;
            using AtomUI.Theme.Language;

            [assembly: LanguageSgMetaInfo("TestApp.Localization")]

            namespace AtomUI.Theme.Language
            {
                public enum LanguageCode
                {
                    zh_CN,
                    zh_TW,
                    en_US,
                }

                [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
                public sealed class LanguageSgMetaInfoAttribute : Attribute
                {
                    public LanguageSgMetaInfoAttribute(string? targetNamespace)
                    {
                        TargetNamespace = targetNamespace;
                    }

                    public string? TargetNamespace { get; }
                }

                [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                public sealed class LanguageProviderAttribute : Attribute
                {
                    public LanguageProviderAttribute(LanguageCode languageCode, string languageId = "Default")
                    {
                        LanguageCode = languageCode;
                        LanguageId = languageId;
                    }

                    public LanguageCode LanguageCode { get; }

                    public string LanguageId { get; }
                }

                public class LanguageResourceExtension<T>
                {
                    public LanguageResourceExtension()
                    {
                    }

                    public LanguageResourceExtension(T key)
                    {
                    }
                }

                public abstract class LanguageProvider
                {
                    protected LanguageProvider()
                    {
                    }

                    protected LanguageProvider(LanguageCode langCode, string langId)
                    {
                    }

                    protected virtual Type GetResourceKindType()
                    {
                        return typeof(object);
                    }

                    protected void LogBuildResourceDictionaryError(Type resourceKindType)
                    {
                    }

                    public virtual void BuildResourceDictionary(Avalonia.Controls.IResourceDictionary dictionary)
                    {
                    }
                }
            }

            namespace Avalonia.Controls
            {
                public interface IResourceDictionary
                {
                    object? this[object key] { get; set; }
                }
            }

            namespace TestApp.ShowCases.Demo
            {
                internal static class DemoShowCase
                {
                    public const string LanguageId = "DemoShowCase";
                }
            }

            namespace TestApp.ShowCases.Demo.Localization
            {
                [LanguageProvider(LanguageCode.en_US, DemoShowCase.LanguageId)]
                internal partial class en_US
                {
                    public const string Title = "Title";
                }
            }
            """);

        var cancellationToken = TestContext.Current.CancellationToken;
        var driver            = CSharpGeneratorDriver.Create(new LanguageGenerator());

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics, cancellationToken);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(cancellationToken)
                         .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedConstructorsSource = outputCompilation.SyntaxTrees
                                                           .Single(tree => tree.FilePath.EndsWith("LanguageProviderConstructors.g.cs"))
                                                           .GetText(cancellationToken)
                                                           .ToString();

        generatedConstructorsSource.ShouldContain("// <auto-generated />");
        generatedConstructorsSource.ShouldNotContain("public en_US()");

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("LanguageProviderPool.g.cs"))
                                               .GetText(cancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("internal sealed class DemoShowCaseEnUSLanguageProvider : LanguageProvider");
        generatedSource.ShouldContain("dictionary[global::TestApp.Localization.DemoShowCaseLangResourceKind.Title] = global::TestApp.ShowCases.Demo.Localization.en_US.Title;");
    }

    [Fact]
    public void DoesNotEmitLanguageFilesWhenProjectHasNoLanguageProviders()
    {
        var compilation = CreateCompilation("""
            using System;

            namespace AtomUI.Theme.Language
            {
                public enum LanguageCode
                {
                    zh_CN,
                    zh_TW,
                    en_US,
                }

                [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
                public sealed class LanguageSgMetaInfoAttribute : Attribute
                {
                    public LanguageSgMetaInfoAttribute(string? targetNamespace)
                    {
                        TargetNamespace = targetNamespace;
                    }

                    public string? TargetNamespace { get; }
                }

                [AttributeUsage(AttributeTargets.Class, Inherited = false)]
                public sealed class LanguageProviderAttribute : Attribute
                {
                    public LanguageProviderAttribute(LanguageCode languageCode, string languageId = "Default")
                    {
                        LanguageCode = languageCode;
                        LanguageId = languageId;
                    }

                    public LanguageCode LanguageCode { get; }

                    public string LanguageId { get; }
                }

                public abstract class LanguageProvider
                {
                    protected LanguageProvider()
                    {
                    }
                }
            }

            namespace Avalonia.Controls
            {
                public interface IResourceDictionary
                {
                    object? this[object key] { get; set; }
                }
            }

            namespace TestApp
            {
                public class PlainType
                {
                }
            }
            """);

        var cancellationToken = TestContext.Current.CancellationToken;
        var driver            = CSharpGeneratorDriver.Create(new LanguageGenerator());

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics, cancellationToken);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(cancellationToken)
                         .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedPaths = outputCompilation.SyntaxTrees.Select(static tree => tree.FilePath).ToArray();

        generatedPaths.ShouldNotContain(static path => path.EndsWith("LanguageResourceConst.g.cs"));
        generatedPaths.ShouldNotContain(static path => path.EndsWith("LanguageProviderPool.g.cs"));
        generatedPaths.ShouldNotContain(static path => path.EndsWith("LanguageProviderConstructors.g.cs"));
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .Cast<MetadataReference>()
            .ToImmutableArray();

        return CSharpCompilation.Create(
            "LanguageProviderConstructorGeneratorTests",
            [CSharpSyntaxTree.ParseText(source)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
