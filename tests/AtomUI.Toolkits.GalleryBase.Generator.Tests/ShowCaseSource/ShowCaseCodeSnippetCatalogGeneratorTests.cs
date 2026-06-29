using System.Collections.Immutable;
using System.Text;
using AtomUI.Toolkits.GalleryBase.Generator.ShowCaseSource;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Generator.Tests.ShowCaseSource;

public class ShowCaseCodeSnippetCatalogGeneratorTests
{
    [Fact]
    public void GeneratesCatalogFromDeferredShowCaseItemContent()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    x:Class="AtomUIGallery.ShowCases.General.Button.Views.ButtonShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate>
                                    <StackPanel Spacing="8">
                                        <Button Content="Primary" />
                                    </StackPanel>
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("namespace AtomUIGallery.Generated");
        generatedSource.ShouldContain("viewTypeName == \"AtomUIGallery.ShowCases.General.Button.Views.ButtonShowCase\"");
        generatedSource.ShouldContain("panelKey == \"ExamplesContent\"");
        generatedSource.ShouldContain("itemIndex == 0");
        generatedSource.ShouldContain("TabTitle: \"AXAML\"");
        generatedSource.ShouldContain("Language: \"axaml\"");
        generatedSource.ShouldContain("SourceFilePath: \"controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml\"");
        generatedSource.ShouldContain("<StackPanel Spacing=\\\"8\\\">");
        generatedSource.ShouldContain("<Button Content=\\\"Primary\\\" />");
    }

    [Fact]
    public void GeneratesCodeBehindAndViewModelSnippetsFromShowCaseItemDependencies()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    xmlns:vm="using:AtomUIGallery.ShowCases.General.Foo.ViewModels"
                    x:Class="AtomUIGallery.ShowCases.General.Foo.Views.FooShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate x:DataType="vm:FooViewModel">
                                    <StackPanel Spacing="8">
                                        <TextBlock Text="{Binding SelectedValue}" />
                                        <Button Content="Change" Click="HandleChangeClick" />
                                    </StackPanel>
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """),
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml.cs",
                """
                namespace AtomUIGallery.ShowCases.General.Foo.Views;

                public partial class FooShowCase
                {
                    private void HandleChangeClick(object? sender, RoutedEventArgs e)
                    {
                        if (DataContext is AtomUIGallery.ShowCases.General.Foo.ViewModels.FooViewModel viewModel)
                        {
                            viewModel.HandleChangeClick();
                        }
                    }

                    private string GetLabel()
                    {
                        return "Basic";
                    }
                }
                """),
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/ViewModels/FooViewModel.cs",
                """
                using ReactiveUI;

                namespace AtomUIGallery.ShowCases.General.Foo.ViewModels;

                public class FooViewModel : ReactiveObject
                {
                    private string _selectedValue = "Default";

                    public string SelectedValue
                    {
                        get => _selectedValue;
                        set => this.RaiseAndSetIfChanged(ref _selectedValue, value);
                    }

                    public void HandleChangeClick()
                    {
                        SelectedValue = GetNextValue();
                    }

                    private string GetNextValue()
                    {
                        return "Updated";
                    }
                }
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("TabTitle: \"AXAML\"");
        generatedSource.ShouldContain("TabTitle: \"Code-behind\"");
        generatedSource.ShouldContain("TabTitle: \"ViewModel\"");
        generatedSource.ShouldContain("HandleChangeClick");
        generatedSource.ShouldContain("SelectedValue");
        generatedSource.ShouldContain("GetNextValue");
        generatedSource.ShouldContain("_selectedValue");
        generatedSource.ShouldNotContain("GetLabel");
    }

    [Fact]
    public void GeneratesCodeBehindAndViewModelSnippetsFromAttachedToVisualTreeDependency()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    xmlns:atom="https://atomui.net"
                    xmlns:vm="using:AtomUIGallery.ShowCases.DataDisplay.DataGrid.ViewModels"
                    x:Class="AtomUIGallery.ShowCases.DataDisplay.DataGrid.Views.DataGridShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate x:DataType="vm:DataGridViewModel">
                                    <atom:DataGrid x:Name="BasicCaseGrid"
                                                   x:DataType="vm:DataGridBaseInfo"
                                                   AttachedToVisualTree="HandleExampleDataGridAttached"
                                                   DetachedFromVisualTree="HandleExampleDataGridDetached">
                                        <atom:DataGrid.Columns>
                                            <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />
                                        </atom:DataGrid.Columns>
                                    </atom:DataGrid>
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """),
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml.cs",
                """
                namespace AtomUIGallery.ShowCases.DataDisplay.DataGrid.Views;

                public partial class DataGridShowCase
                {
                    private void HandleExampleDataGridAttached(object? sender, VisualTreeAttachmentEventArgs args)
                    {
                        if (sender is AtomDataGrid dataGrid)
                        {
                            SetExampleDataGridItemsSource(dataGrid);
                        }
                    }

                    private void HandleExampleDataGridDetached(object? sender, VisualTreeAttachmentEventArgs args)
                    {
                        if (sender is AtomDataGrid dataGrid)
                        {
                            dataGrid.ItemsSource = null;
                        }
                    }

                    private void SetExampleDataGridItemsSource(AtomDataGrid dataGrid)
                    {
                        if (DataContext is not AtomUIGallery.ShowCases.DataDisplay.DataGrid.ViewModels.DataGridViewModel viewModel)
                        {
                            return;
                        }

                        viewModel.BasicCaseDataSource = [];
                        dataGrid.ItemsSource = viewModel.BasicCaseDataSource;
                    }
                }
                """),
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/ViewModels/DataGridViewModel.cs",
                """
                namespace AtomUIGallery.ShowCases.DataDisplay.DataGrid.ViewModels;

                public class DataGridViewModel
                {
                    public List<DataGridBaseInfo>? BasicCaseDataSource { get; set; }
                }

                public class DataGridBaseInfo
                {
                    public string Name { get; set; } = string.Empty;
                }
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("TabTitle: \"AXAML\"");
        generatedSource.ShouldContain("TabTitle: \"Code-behind\"");
        generatedSource.ShouldContain("TabTitle: \"ViewModel\"");
        generatedSource.ShouldContain("HandleExampleDataGridAttached");
        generatedSource.ShouldContain("HandleExampleDataGridDetached");
        generatedSource.ShouldContain("SetExampleDataGridItemsSource");
        generatedSource.ShouldContain("BasicCaseDataSource");
        generatedSource.ShouldContain("DataGridBaseInfo");
    }

    [Fact]
    public void GeneratesSnippetsFromMirroredAdditionalFilesUsingOriginalSourcePathMetadata()
    {
        var compilation = CreateCompilation();
        var axamlOriginalPath = "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml";
        var codeBehindOriginalPath = "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml.cs";
        var axamlMirrorPath = "/repo/output/AtomUIGallery/obj/Debug/GallerySourceCodeDisplay/ShowCases/General/Foo/Views/FooShowCase.axaml.gallerysource";
        var codeBehindMirrorPath = "/repo/output/AtomUIGallery/obj/Debug/GallerySourceCodeDisplay/ShowCases/General/Foo/Views/FooShowCase.axaml.cs.gallerysource";
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                axamlMirrorPath,
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    x:Class="AtomUIGallery.ShowCases.General.Foo.Views.FooShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate>
                                    <Button Content="Change" Click="HandleChangeClick" />
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """),
            new InMemoryAdditionalText(
                codeBehindMirrorPath,
                """
                namespace AtomUIGallery.ShowCases.General.Foo.Views;

                public partial class FooShowCase
                {
                    private void HandleChangeClick(object? sender, RoutedEventArgs e)
                    {
                        DoWork();
                    }

                    private void DoWork()
                    {
                    }
                }
                """));

        var outputCompilation = RunGenerator(
            compilation,
            additionalFiles,
            out var diagnostics,
            new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                [axamlMirrorPath] = new Dictionary<string, string>
                {
                    ["build_metadata.AdditionalFiles.GallerySourceOriginalPath"] = axamlOriginalPath
                },
                [codeBehindMirrorPath] = new Dictionary<string, string>
                {
                    ["build_metadata.AdditionalFiles.GallerySourceOriginalPath"] = codeBehindOriginalPath
                }
            });

        diagnostics.ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("TabTitle: \"AXAML\"");
        generatedSource.ShouldContain("TabTitle: \"Code-behind\"");
        generatedSource.ShouldContain("SourceFilePath: \"controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml\"");
        generatedSource.ShouldContain("SourceFilePath: \"controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml.cs\"");
        generatedSource.ShouldContain("HandleChangeClick");
        generatedSource.ShouldContain("DoWork");
    }

    [Fact]
    public void ExtractsFullMultiLineAxamlElementWithPrefixedClosingTag()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    xmlns:atom="https://atomui.net"
                    x:Class="AtomUIGallery.ShowCases.Navigation.Steps.Views.StepsShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate>
                                    <atom:Steps CurrentStep="0">
                                        <atom:StepsItem Header="Finished" />
                                        <atom:StepsItem Header="In Progress" />
                                        <atom:StepsItem Header="Waiting" />
                                    </atom:Steps>
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        // The whole element must be captured, not just the opening line.
        generatedSource.ShouldContain("<atom:Steps CurrentStep=\\\"0\\\">");
        generatedSource.ShouldContain("<atom:StepsItem Header=\\\"Finished\\\" />");
        generatedSource.ShouldContain("<atom:StepsItem Header=\\\"Waiting\\\" />");
        generatedSource.ShouldContain("</atom:Steps>");
    }

    [Fact]
    public void ExtractsFullMultiLineSelfClosingAxamlElement()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    xmlns:atom="https://atomui.net"
                    xmlns:vm="using:AtomUIGallery.ShowCases.DataEntry.Select.ViewModels"
                    x:Class="AtomUIGallery.ShowCases.DataEntry.Select.Views.SelectShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Custom Search">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate x:DataType="vm:SelectViewModel">
                                    <atom:Select Name="CustomSearchSelect"
                                                 AttachedToVisualTree="HandleCustomSearchSelectAttached"
                                                 Mode="Single"
                                                 IsFilterEnabled="True"
                                                 OptionsSource="{Binding SearchOptions}" />
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("<atom:Select Name=\\\"CustomSearchSelect\\\"");
        generatedSource.ShouldContain("AttachedToVisualTree=\\\"HandleCustomSearchSelectAttached\\\"");
        generatedSource.ShouldContain("OptionsSource=\\\"{Binding SearchOptions}\\\" />");
    }

    [Fact]
    public void ExcludesPageLevelLocalizationHelpersFromViewModelSnippet()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    xmlns:vm="using:AtomUIGallery.ShowCases.Navigation.Steps.ViewModels"
                    x:Class="AtomUIGallery.ShowCases.Navigation.Steps.Views.StepsShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate x:DataType="vm:StepsViewModel">
                                    <TextBlock Text="{Binding NextButtonText}" />
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """),
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/ViewModels/StepsViewModel.cs",
                """
                using ReactiveUI;

                namespace AtomUIGallery.ShowCases.Navigation.Steps.ViewModels;

                public class StepsViewModel : ReactiveObject
                {
                    private string _nextButtonText = Lang(StepsLangResourceKind.Next);

                    public string NextButtonText
                    {
                        get => _nextButtonText;
                        set => this.RaiseAndSetIfChanged(ref _nextButtonText, value);
                    }

                    private static string Lang(StepsLangResourceKind kind)
                    {
                        return FallbackLang(kind);
                    }

                    private static string FallbackLang(StepsLangResourceKind kind)
                    {
                        return kind switch
                        {
                            StepsLangResourceKind.Next => "Next",
                            StepsLangResourceKind.Done => "Done",
                            _                          => kind.ToString()
                        };
                    }
                }
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("TabTitle: \"ViewModel\"");
        generatedSource.ShouldContain("NextButtonText");
        // The localization lookup table is page-level data and must not be dragged in.
        generatedSource.ShouldNotContain("FallbackLang");
        generatedSource.ShouldNotContain("StepsLangResourceKind.Done");
    }

    [Fact]
    public void NormalizesMemberIndentationInCSharpSnippet()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    x:Class="AtomUIGallery.ShowCases.General.Foo.Views.FooShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate>
                                    <Button Click="HandleChangeClick" />
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """),
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml.cs",
                """
                namespace AtomUIGallery.ShowCases.General.Foo.Views;

                public partial class FooShowCase
                {
                    private void HandleChangeClick(object? sender, RoutedEventArgs e)
                    {
                        DoWork();
                    }
                }
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        // Member is indented exactly one level (4 spaces) under the type; the opening brace
        // of the body sits at the same 4-space level, not stacked to 8.
        generatedSource.ShouldContain("    private void HandleChangeClick(object? sender, RoutedEventArgs e)\\n    {\\n        DoWork();\\n    }");
    }

    [Fact]
    public void OmitsEmptyCodeBehindAndViewModelTabsWhenItemHasNoCSharpDependencies()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    x:Class="AtomUIGallery.ShowCases.General.Foo.Views.FooShowCase">
                    <gallery:ShowCasePanel Name="ExamplesContent">
                        <gallery:ShowCaseItem Title="Basic">
                            <gallery:ShowCaseItem.DeferredContentTemplate>
                                <DataTemplate>
                                    <StackPanel Spacing="8">
                                        <Button Content="Primary" />
                                    </StackPanel>
                                </DataTemplate>
                            </gallery:ShowCaseItem.DeferredContentTemplate>
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """),
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/Views/FooShowCase.axaml.cs",
                """
                namespace AtomUIGallery.ShowCases.General.Foo.Views;

                public partial class FooShowCase
                {
                    private void HandleUnusedClick(object? sender, object e)
                    {
                    }
                }
                """),
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Foo/ViewModels/FooViewModel.cs",
                """
                namespace AtomUIGallery.ShowCases.General.Foo.ViewModels;

                public class FooViewModel
                {
                    public string UnusedText { get; set; } = "Unused";
                }
                """));

        var outputCompilation = RunGenerator(compilation, additionalFiles, out var diagnostics);

        diagnostics.ShouldBeEmpty();
        outputCompilation.GetDiagnostics(TestContext.Current.CancellationToken)
                         .Where(static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error)
                         .ShouldBeEmpty();

        var generatedSource = outputCompilation.SyntaxTrees
                                               .Single(tree => tree.FilePath.EndsWith("ShowCaseCodeSnippetCatalog.g.cs"))
                                               .GetText(TestContext.Current.CancellationToken)
                                               .ToString();

        generatedSource.ShouldContain("TabTitle: \"AXAML\"");
        generatedSource.ShouldNotContain("TabTitle: \"Code-behind\"");
        generatedSource.ShouldNotContain("TabTitle: \"ViewModel\"");
        generatedSource.ShouldNotContain("HandleUnusedClick");
        generatedSource.ShouldNotContain("UnusedText");
    }

    [Fact]
    public void ReportsDiagnosticForShowCasePanelWithoutName()
    {
        var compilation = CreateCompilation();
        var additionalFiles = ImmutableArray.Create<AdditionalText>(
            new InMemoryAdditionalText(
                "/repo/controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml",
                """
                <UserControl
                    xmlns="https://github.com/avaloniaui"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                    xmlns:gallery="https://atomui.net/toolkits/gallery-base"
                    x:Class="AtomUIGallery.ShowCases.General.Button.Views.ButtonShowCase">
                    <gallery:ShowCasePanel>
                        <gallery:ShowCaseItem Title="Basic">
                            <Button Content="Primary" />
                        </gallery:ShowCaseItem>
                    </gallery:ShowCasePanel>
                </UserControl>
                """));

        RunGenerator(compilation, additionalFiles, out var diagnostics);

        var diagnostic = diagnostics.ShouldHaveSingleItem();
        diagnostic.Id.ShouldBe("ATOMUIGEN101");
        diagnostic.GetMessage().ShouldContain("ShowCasePanel");
        diagnostic.GetMessage().ShouldContain("Name");
    }

    private static CSharpCompilation RunGenerator(CSharpCompilation compilation,
                                                  ImmutableArray<AdditionalText> additionalFiles,
                                                  out ImmutableArray<Diagnostic> diagnostics,
                                                  IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? additionalFileOptions = null)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var driver = CSharpGeneratorDriver.Create(
            [new ShowCaseCodeSnippetCatalogGenerator().AsSourceGenerator()],
            additionalTexts: additionalFiles,
            parseOptions: CSharpParseOptions.Default,
            optionsProvider: new InMemoryAnalyzerConfigOptionsProvider(
                new Dictionary<string, string>
                {
                    ["build_property.RootNamespace"] = "AtomUIGallery",
                    ["build_property.ProjectDir"]    = "/repo/"
                },
                additionalFileOptions));

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out diagnostics, cancellationToken);
        return (CSharpCompilation)outputCompilation;
    }

    private static CSharpCompilation CreateCompilation()
    {
        var references = ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
                         .Split(Path.PathSeparator)
                         .Select(path => MetadataReference.CreateFromFile(path))
                         .Cast<MetadataReference>()
                         .ToImmutableArray();

        return CSharpCompilation.Create(
            "ShowCaseCodeSnippetCatalogGeneratorTests",
            [CSharpSyntaxTree.ParseText(RuntimeStubs)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    private sealed class InMemoryAdditionalText : AdditionalText
    {
        private readonly SourceText _text;

        public InMemoryAdditionalText(string path, string text)
        {
            Path  = path;
            _text = SourceText.From(text, Encoding.UTF8);
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default)
        {
            return _text;
        }
    }

    private sealed class InMemoryAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
    {
        private readonly AnalyzerConfigOptions _globalOptions;

        private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? _additionalFileOptions;

        public InMemoryAnalyzerConfigOptionsProvider(
            IReadOnlyDictionary<string, string> globalOptions,
            IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>>? additionalFileOptions = null)
        {
            _globalOptions = new InMemoryAnalyzerConfigOptions(globalOptions);
            _additionalFileOptions = additionalFileOptions;
        }

        public override AnalyzerConfigOptions GlobalOptions => _globalOptions;

        public override AnalyzerConfigOptions GetOptions(SyntaxTree tree)
        {
            return _globalOptions;
        }

        public override AnalyzerConfigOptions GetOptions(AdditionalText textFile)
        {
            if (_additionalFileOptions is not null &&
                _additionalFileOptions.TryGetValue(textFile.Path, out var options))
            {
                return new InMemoryAnalyzerConfigOptions(options);
            }

            return _globalOptions;
        }
    }

    private sealed class InMemoryAnalyzerConfigOptions : AnalyzerConfigOptions
    {
        private readonly IReadOnlyDictionary<string, string> _options;

        public InMemoryAnalyzerConfigOptions(IReadOnlyDictionary<string, string> options)
        {
            _options = options;
        }

        public override bool TryGetValue(string key, out string value)
        {
            return _options.TryGetValue(key, out value!);
        }
    }

    private const string RuntimeStubs = """
        namespace AtomUI.Toolkits.GalleryBase.SourceCode
        {
            public sealed record ShowCaseCodeSnippetGroup(
                string Title,
                System.Collections.Generic.IReadOnlyList<ShowCaseCodeSnippet> Snippets);

            public sealed record ShowCaseCodeSnippet(
                string TabTitle,
                string Language,
                string Text,
                string? SourceFilePath,
                int StartLine,
                int EndLine);
        }
        """;
}
