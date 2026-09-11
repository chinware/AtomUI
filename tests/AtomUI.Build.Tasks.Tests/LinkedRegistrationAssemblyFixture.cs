using AtomUI.LinkedRegistration.Protocol;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AtomUI.Build.Tasks.Tests;

internal sealed class LinkedRegistrationAssemblyFixture : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        "AtomUI-LinkedAssemblyTests-" + Guid.NewGuid().ToString("N"));

    internal LinkedRegistrationAssemblyFixture()
    {
        Directory.CreateDirectory(_directory);
        PackageAssemblyPath = Compile(
            "Fixture.AtomUI.Package",
            """
            namespace Fixture.AtomUI;

            public static class ThemeManagerBuilderExtensions
            {
                public static object UseFixtureControls(this object builder) => builder;
            }

            public sealed class FixtureControl;
            """);
        CatalogSidecarPath = CreateCatalogSidecar();
    }

    internal string PackageAssemblyPath { get; }

    internal string CatalogSidecarPath { get; }

    internal string CompileConsumer(string assemblyName, bool callsEntry)
    {
        var registrationBody = callsEntry
            ? "return builder.UseFixtureControls();"
            : "return builder;";
        return Compile(
            assemblyName,
            $$"""
            using Fixture.AtomUI;

            namespace Fixture.Consumer;

            public static class ConsumerRegistration
            {
                public static object Register(object builder)
                {
                    {{registrationBody}}
                }

                public static FixtureControl CreateControl() => new();
            }
            """,
            MetadataReference.CreateFromFile(PackageAssemblyPath));
    }

    internal string CompileUnrelated(string assemblyName)
    {
        return Compile(
            assemblyName,
            "namespace Fixture.Unrelated; public sealed class Marker;");
    }

    internal string CompileDelegateConsumer(string assemblyName)
    {
        return Compile(
            assemblyName,
            """
            using System;
            using Fixture.AtomUI;

            namespace Fixture.Consumer;

            public static class ConsumerRegistration
            {
                public static object Register(object builder)
                {
                    Func<object, object> register = ThemeManagerBuilderExtensions.UseFixtureControls;
                    return register(builder);
                }

                public static FixtureControl CreateControl() => new();
            }
            """,
            MetadataReference.CreateFromFile(PackageAssemblyPath));
    }

    internal string CompileConsumerWithTypeOperand(string assemblyName)
    {
        return Compile(
            assemblyName,
            """
            using Fixture.AtomUI;

            namespace Fixture.Consumer;

            public static class ConsumerRegistration
            {
                public static object Register(object builder)
                {
                    _ = (FixtureControl)builder;
                    return builder.UseFixtureControls();
                }
            }
            """,
            MetadataReference.CreateFromFile(PackageAssemblyPath));
    }

    internal string GetOutputPath(string fileName)
    {
        return Path.Combine(_directory, fileName);
    }

    private string Compile(
        string assemblyName,
        string source,
        params MetadataReference[] additionalReferences)
    {
        var trustedAssemblies = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))
            ?.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            ?? throw new InvalidOperationException("Trusted platform assemblies are unavailable.");
        var references = trustedAssemblies.Select(static path => MetadataReference.CreateFromFile(path))
            .Concat(additionalReferences);
        var compilation = CSharpCompilation.Create(
            assemblyName,
            [CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest))],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var path = Path.Combine(_directory, assemblyName + ".dll");
        var result = compilation.Emit(path);
        if (!result.Success)
        {
            throw new InvalidOperationException(string.Join(
                Environment.NewLine,
                result.Diagnostics.Select(static diagnostic => diagnostic.ToString())));
        }
        return path;
    }

    private string CreateCatalogSidecar()
    {
        var sidecar = new LinkedRegistrationSidecar
        {
            Producer = "AtomUI.Build.Tasks.Tests",
            Assembly = new LinkedSidecarAssembly
            {
                Name = "Fixture.AtomUI.Package",
                TargetFramework = "net10.0"
            },
            Packages =
            [
                new LinkedSidecarPackage
                {
                    Id = "Fixture.AtomUI.Package",
                    AssemblyName = "Fixture.AtomUI.Package",
                    Granularity = "Directory",
                    EntryMethods =
                    [
                        "Fixture.AtomUI.ThemeManagerBuilderExtensions.UseFixtureControls"
                    ],
                    FullFragment = new LinkedSidecarFragment
                    {
                        Type = "Fixture.AtomUI.GeneratedFullRegistration",
                        Method = "Register"
                    }
                }
            ]
        };
        var path = Path.Combine(_directory, "Fixture.AtomUI.Package.atomui-link.json");
        File.WriteAllBytes(path, LinkedRegistrationSidecarCodec.Write(sidecar));
        return path;
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
