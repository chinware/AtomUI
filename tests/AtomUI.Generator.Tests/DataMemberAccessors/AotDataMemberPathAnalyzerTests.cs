using System.Collections.Immutable;
using AtomUI.Generator.DataMemberAccessors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.DataMemberAccessors;

public class AotDataMemberPathAnalyzerTests
{
    [Fact]
    public async Task Reports_ListSortDescription_Nameof_Path_When_Model_Has_No_Generated_Accessor()
    {
        var diagnostics = await AnalyzeAsync("""
            using AtomUI.Controls.Data;

            namespace AtomUI.Controls.Data
            {
                public sealed class ListSortDescription
                {
                    public static ListSortDescription FromPath(string propertyPath) => new();
                }
            }

            namespace Demo
            {
                internal sealed class Row
                {
                    public int Score { get; set; }
                }

                internal sealed class DemoUsage
                {
                    public void UseSort()
                    {
                        _ = ListSortDescription.FromPath(nameof(Row.Score));
                    }
                }
            }
            """);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIAOT001");
        diagnostic.GetMessage().ShouldContain("Demo.Row");
        diagnostic.GetMessage().ShouldContain("Score");
    }

    [Fact]
    public async Task Does_Not_Report_When_Model_Has_Generated_Accessor()
    {
        var diagnostics = await AnalyzeAsync("""
            using System;
            using AtomUI.Controls.Data;

            namespace AtomUI.Controls.Data
            {
                [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
                public sealed class GenerateDataMemberAccessorsAttribute : Attribute
                {
                }

                public sealed class ListSortDescription
                {
                    public static ListSortDescription FromPath(string propertyPath) => new();
                }
            }

            namespace Demo
            {
                [GenerateDataMemberAccessors]
                internal sealed partial class Row
                {
                    public int Score { get; set; }
                }

                internal sealed class DemoUsage
                {
                    public void UseSort()
                    {
                        _ = ListSortDescription.FromPath(nameof(Row.Score));
                    }
                }
            }
            """);

        diagnostics.ShouldBeEmpty();
    }

    [Fact]
    public async Task Reports_DataGridSortDescription_Nameof_Path_When_Model_Has_No_Generated_Accessor()
    {
        var diagnostics = await AnalyzeAsync("""
            using AtomUI.Desktop.Controls.Data;

            namespace AtomUI.Desktop.Controls.Data
            {
                public abstract class DataGridSortDescription
                {
                    public static DataGridSortDescription FromPath(string propertyPath) => null!;
                }
            }

            namespace Demo
            {
                internal sealed class Row
                {
                    public string? Name { get; set; }
                }

                internal sealed class DemoUsage
                {
                    public void UseSort()
                    {
                        _ = DataGridSortDescription.FromPath(nameof(Row.Name));
                    }
                }
            }
            """);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIAOT001");
        diagnostic.GetMessage().ShouldContain("Demo.Row");
        diagnostic.GetMessage().ShouldContain("Name");
    }

    [Fact]
    public async Task Reports_DataGridColumn_SortMemberPath_Nameof_When_Model_Has_No_Generated_Accessor()
    {
        var diagnostics = await AnalyzeAsync("""
            namespace AtomUI.Desktop.Controls
            {
                public class DataGridColumn
                {
                    public string? SortMemberPath { get; set; }
                }

                public sealed class DataGridTextColumn : DataGridColumn
                {
                }
            }

            namespace Demo
            {
                using AtomUI.Desktop.Controls;

                internal sealed class Row
                {
                    public string? Name { get; set; }
                }

                internal sealed class DemoUsage
                {
                    public void BuildColumn()
                    {
                        _ = new DataGridTextColumn
                        {
                            SortMemberPath = nameof(Row.Name)
                        };
                    }
                }
            }
            """);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIAOT001");
        diagnostic.GetMessage().ShouldContain("Demo.Row");
        diagnostic.GetMessage().ShouldContain("Name");
    }

    [Fact]
    public async Task Reports_String_Literal_Path_When_Model_Cannot_Be_Verified()
    {
        var diagnostics = await AnalyzeAsync("""
            using AtomUI.Controls.Data;

            namespace AtomUI.Controls.Data
            {
                public sealed class ListSortDescription
                {
                    public static ListSortDescription FromPath(string propertyPath) => new();
                }
            }

            namespace Demo
            {
                internal sealed class DemoUsage
                {
                    public void UseSort()
                    {
                        _ = ListSortDescription.FromPath("Content");
                    }
                }
            }
            """);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIAOT003");
        diagnostic.GetMessage().ShouldContain("Content");
    }

    [Fact]
    public async Task Reports_When_Annotated_Model_Does_Not_Generate_Path()
    {
        var diagnostics = await AnalyzeAsync("""
            using System;
            using AtomUI.Controls.Data;

            namespace AtomUI.Controls.Data
            {
                [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
                public sealed class GenerateDataMemberAccessorsAttribute : Attribute
                {
                }

                public sealed class ListSortDescription
                {
                    public static ListSortDescription FromPath(string propertyPath) => new();
                }
            }

            namespace Demo
            {
                [GenerateDataMemberAccessors]
                internal sealed partial class Row
                {
                    private int Score { get; set; }

                    public void UseSort()
                    {
                        _ = ListSortDescription.FromPath(nameof(Score));
                    }
                }
            }
            """);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIAOT002");
        diagnostic.GetMessage().ShouldContain("Demo.Row");
        diagnostic.GetMessage().ShouldContain("Score");
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source)
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();

        var compilation = CSharpCompilation.Create(
            "AotDataMemberPathAnalyzerTests",
            [CSharpSyntaxTree.ParseText(source)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationDiagnostics = compilation.GetDiagnostics(TestContext.Current.CancellationToken);
        compilationDiagnostics.Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                              .ShouldBeEmpty();

        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new AotDataMemberPathAnalyzer());
        var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);

        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);
    }
}
