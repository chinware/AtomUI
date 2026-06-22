using System.Collections.Immutable;
using AtomUI.Generator.ResourceHost;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.ResourceHost;

public class ScopedResourceHostGeneratorTests
{
    [Fact]
    public void Generates_Scoped_Resource_Host_For_Non_Visual_AvaloniaObject()
    {
        var compilation = CreateCompilation("""
            using AtomUI.Controls;
            using Avalonia;

            namespace Demo
            {
                [GenerateScopedResourceHost]
                public partial class DemoItem : AvaloniaObject
                {
                }
            }
            """);

        var outputCompilation = RunGenerator(compilation, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("DemoItem.ScopedResourceHost.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("public partial class DemoItem : global::Avalonia.Controls.IResourceHost, global::Avalonia.Styling.IThemeVariantHost");
        generatedSource.ShouldContain("internal global::System.IDisposable AttachResourceHost(global::Avalonia.Controls.IResourceHost resourceHost)");
        generatedSource.ShouldContain("global::Avalonia.Controls.ResourceNodeExtensions.TryFindResource(__atomuiResourceHost, key, theme, out value)");
        generatedSource.ShouldContain("global::Avalonia.Application.Current?.TryGetResource(key, theme, out value) == true");
    }

    [Fact]
    public void Reports_Diagnostic_For_Non_Partial_Target()
    {
        var compilation = CreateCompilation("""
            using AtomUI.Controls;
            using Avalonia;

            namespace Demo
            {
                [GenerateScopedResourceHost]
                public class DemoItem : AvaloniaObject
                {
                }
            }
            """);

        RunGenerator(compilation, out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN001");
    }

    [Fact]
    public void Reports_Diagnostic_For_Visual_Target()
    {
        var compilation = CreateCompilation("""
            using AtomUI.Controls;
            using Avalonia.Controls;

            namespace Demo
            {
                [GenerateScopedResourceHost]
                public partial class DemoItem : Control
                {
                }
            }
            """);

        RunGenerator(compilation, out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN003");
    }

    [Fact]
    public void Reports_Diagnostic_For_Non_AvaloniaObject_Target()
    {
        var compilation = CreateCompilation("""
            using AtomUI.Controls;

            namespace Demo
            {
                [GenerateScopedResourceHost]
                public partial class DemoItem
                {
                }
            }
            """);

        RunGenerator(compilation, out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN002");
    }

    [Fact]
    public void Reports_Diagnostic_For_Target_With_Existing_ResourceHost()
    {
        var compilation = CreateCompilation("""
            using AtomUI.Controls;
            using Avalonia;
            using Avalonia.Controls;
            using Avalonia.Styling;

            namespace Demo
            {
                [GenerateScopedResourceHost]
                public partial class DemoItem : AvaloniaObject, IResourceHost, IThemeVariantHost
                {
                    public bool HasResources => true;

                    public ThemeVariant ActualThemeVariant => ThemeVariant.Default;

                    public event System.EventHandler<ResourcesChangedEventArgs>? ResourcesChanged;

                    public event System.EventHandler? ActualThemeVariantChanged;

                    public bool TryGetResource(object key, ThemeVariant? theme, out object? value)
                    {
                        value = null;
                        return false;
                    }

                    void IResourceHost.NotifyHostedResourcesChanged(ResourcesChangedEventArgs e)
                    {
                        ResourcesChanged?.Invoke(this, e);
                    }
                }
            }
            """);

        RunGenerator(compilation, out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN004");
    }

    private static CSharpCompilation RunGenerator(CSharpCompilation compilation,
                                                  out ImmutableArray<Diagnostic> diagnostics)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var driver            = CSharpGeneratorDriver.Create(new ScopedResourceHostGenerator());

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out diagnostics, cancellationToken);
        return (CSharpCompilation)outputCompilation;
    }

    private static CSharpCompilation CreateCompilation(string source)
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();

        return CSharpCompilation.Create(
            "ScopedResourceHostGeneratorTests",
            [CSharpSyntaxTree.ParseText(source), CSharpSyntaxTree.ParseText(AvaloniaStubs)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private const string AvaloniaStubs = """
        namespace Avalonia
        {
            public class AvaloniaObject
            {
            }

            public class Application : Controls.IResourceHost, Styling.IThemeVariantHost
            {
                public static Application? Current { get; set; }

                public bool HasResources => true;

                public Styling.ThemeVariant ActualThemeVariant => Styling.ThemeVariant.Default;

                public event System.EventHandler<Controls.ResourcesChangedEventArgs>? ResourcesChanged;

                public event System.EventHandler? ActualThemeVariantChanged;

                public bool TryGetResource(object key, Styling.ThemeVariant? theme, out object? value)
                {
                    value = null;
                    return false;
                }

                void Controls.IResourceHost.NotifyHostedResourcesChanged(Controls.ResourcesChangedEventArgs e)
                {
                    ResourcesChanged?.Invoke(this, e);
                }
            }
        }

        namespace Avalonia.Controls
        {
            public class Control : Avalonia.AvaloniaObject, IResourceHost, Avalonia.Styling.IThemeVariantHost
            {
                public bool HasResources => true;

                public Avalonia.Styling.ThemeVariant ActualThemeVariant => Avalonia.Styling.ThemeVariant.Default;

                public event System.EventHandler<ResourcesChangedEventArgs>? ResourcesChanged;

                public event System.EventHandler? ActualThemeVariantChanged;

                public bool TryGetResource(object key, Avalonia.Styling.ThemeVariant? theme, out object? value)
                {
                    value = null;
                    return false;
                }

                void IResourceHost.NotifyHostedResourcesChanged(ResourcesChangedEventArgs e)
                {
                    ResourcesChanged?.Invoke(this, e);
                }
            }

            public class StyledElement : Control
            {
            }

            public interface IResourceNode
            {
                bool HasResources { get; }

                bool TryGetResource(object key, Avalonia.Styling.ThemeVariant? theme, out object? value);
            }

            public interface IResourceHost : IResourceNode
            {
                event System.EventHandler<ResourcesChangedEventArgs>? ResourcesChanged;

                void NotifyHostedResourcesChanged(ResourcesChangedEventArgs e);
            }

            public sealed class ResourcesChangedEventArgs : System.EventArgs
            {
                public static ResourcesChangedEventArgs Create()
                {
                    return new ResourcesChangedEventArgs();
                }
            }

            public static class ResourceNodeExtensions
            {
                public static bool TryFindResource(this IResourceHost control,
                                                   object key,
                                                   Avalonia.Styling.ThemeVariant? theme,
                                                   out object? value)
                {
                    return control.TryGetResource(key, theme, out value);
                }
            }
        }

        namespace Avalonia.Styling
        {
            public class ThemeVariant
            {
                public static ThemeVariant Default { get; } = new ThemeVariant();
            }

            public interface IThemeVariantHost : Avalonia.Controls.IResourceHost
            {
                ThemeVariant ActualThemeVariant { get; }

                event System.EventHandler? ActualThemeVariantChanged;
            }
        }
        """;
}
