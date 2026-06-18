using System;
using System.IO;
using System.Linq;
using AtomUIGallery.ShowCases.Masonry;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class MasonryShowCasePageTests
{
    [Fact]
    public void Masonry_ShowCase_Basic_Demo_Matches_Ant_Design_Structure()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Masonry/Views/MasonryShowCase.axaml");
        var basicDemoMarkup = ExtractBasicDemoMarkup(source);

        basicDemoMarkup.ShouldContain("ItemsSource=\"{Binding BasicItems}\"");
        basicDemoMarkup.ShouldContain("ColumnCount=\"4\"");
        basicDemoMarkup.ShouldContain("ColumnGap=\"16\"");
        basicDemoMarkup.ShouldContain("RowGap=\"16\"");
        basicDemoMarkup.ShouldContain("Height=\"{Binding Height}\"");
        basicDemoMarkup.ShouldContain("IsVisible=\"{Binding !IsSpecial}\"");
        basicDemoMarkup.ShouldContain("IsVisible=\"{Binding IsSpecial}\"");
        basicDemoMarkup.ShouldContain("<atom:CardMetaContent Header=\"{Binding Title}\"");
        basicDemoMarkup.ShouldContain("Content=\"{Binding Description}\"");
        basicDemoMarkup.ShouldContain("Name=\"SpecialCoverImage\"");
        basicDemoMarkup.ShouldContain("<atom:Skeleton IsLoading=\"True\"");
        basicDemoMarkup.ShouldContain("Padding=\"16,16\"");
        basicDemoMarkup.ShouldContain("IsActive=\"True\"");
        basicDemoMarkup.ShouldContain("IsShowAvatar=\"False\"");
        basicDemoMarkup.ShouldContain("IsShowTitle=\"True\"");
        basicDemoMarkup.ShouldContain("ParagraphRows=\"3\"");
        basicDemoMarkup.ShouldContain("IsRound=\"True\"");
        basicDemoMarkup.ShouldContain("IsVisible=\"{Binding #SpecialCoverImage.Source, Converter={x:Static ObjectConverters.IsNull}}\"");
        basicDemoMarkup.ShouldNotContain("IsVisible=\"{Binding #SpecialCoverImage.(asyncImageLoader:ImageLoader.IsLoading)}\"");
        basicDemoMarkup.ShouldNotContain("<atom:SkeletonLine");
        basicDemoMarkup.ShouldNotContain("<atom:SkeletonImage");
        basicDemoMarkup.ShouldNotContain("MaxWidth=\"Infinity\"");
        basicDemoMarkup.ShouldNotContain("HorizontalAlignment=\"Center\"");
        basicDemoMarkup.ShouldNotContain("VerticalAlignment=\"Center\"");
        basicDemoMarkup.ShouldNotContain("Margin=\"20\"");
        basicDemoMarkup.ShouldNotContain("Padding=\"8\"");
        basicDemoMarkup.ShouldNotContain("Padding=\"16,28\"");
    }

    [Fact]
    public void Masonry_ShowCase_Basic_Items_Mirror_Ant_Design_Basic_Demo()
    {
        var viewModel = new MasonryViewModel(null!);

        viewModel.BasicItems.ShouldNotBeNull();
        viewModel.BasicItems!.Select(item => item.Height).ShouldBe(new double[]
        {
            150, 50, 90, 70, 110, 150, 130, 80, 50, 90, 100, 150, 60, 50, 80
        });
        viewModel.BasicItems.Select(item => item.Index).ShouldBe(Enumerable.Range(1, 15));
        viewModel.BasicItems.Count(item => item.IsSpecial).ShouldBe(1);
        viewModel.BasicItems[4].IsSpecial.ShouldBeTrue();
        viewModel.BasicItems[4].CoverSource.ShouldBe("https://images.unsplash.com/photo-1491961865842-98f7befd1a60?w=523&auto=format");
        viewModel.BasicItems[4].Title.ShouldBe("I'm Special");
        viewModel.BasicItems[4].Description.ShouldBe("Let's have a meal");
    }

    [Fact]
    public void Masonry_ShowCase_Responsive_Demo_Matches_Ant_Design_Responsive_Demo()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Masonry/Views/MasonryShowCase.axaml");
        var responsiveDemoMarkup = ExtractMasonryMarkup(source, "ItemsSource=\"{Binding ResponsiveItems}\"");

        responsiveDemoMarkup.ShouldContain("ColumnInfo=\"xs: 1, sm: 2, md: 3, lg: 4\"");
        responsiveDemoMarkup.ShouldContain("Gutter=\"xs: 8, sm: 12, md: 16; xs: 8, sm: 12, md: 16\"");
        responsiveDemoMarkup.ShouldContain("Height=\"{Binding Height}\"");
        responsiveDemoMarkup.ShouldNotContain("ColumnCount=\"4\"");
        responsiveDemoMarkup.ShouldNotContain("ColumnGap=\"16\"");
        responsiveDemoMarkup.ShouldNotContain("RowGap=\"16\"");
        responsiveDemoMarkup.ShouldNotContain("SpecialCoverImage");
        responsiveDemoMarkup.ShouldNotContain("<atom:Skeleton");
    }

    [Fact]
    public void Masonry_ShowCase_Responsive_Items_Mirror_Ant_Design_Responsive_Demo()
    {
        var viewModel = new MasonryViewModel(null!);

        viewModel.ResponsiveItems.ShouldNotBeNull();
        viewModel.ResponsiveItems!.Select(item => item.Height).ShouldBe(new double[]
        {
            120, 55, 85, 160, 95, 140, 75, 110, 65, 130, 90, 145, 55, 100, 80
        });
        viewModel.ResponsiveItems.Select(item => item.Index).ShouldBe(Enumerable.Range(1, 15));
        viewModel.ResponsiveItems.ShouldAllBe(item => !item.IsSpecial);
    }

    [Fact]
    public void Masonry_ShowCase_Api_Table_Documents_Responsive_Properties()
    {
        var viewModel = new MasonryViewModel(null!);

        viewModel.EnsureApiRows();

        viewModel.ApiRows.ShouldNotBeNull();
        viewModel.ApiRows!.Select(row => row.Property).ShouldContain("ColumnInfo");
        viewModel.ApiRows.Select(row => row.Property).ShouldContain("Gutter");
    }

    private static string ExtractBasicDemoMarkup(string source)
    {
        return ExtractMasonryMarkup(source, "ItemsSource=\"{Binding BasicItems}\"");
    }

    private static string ExtractMasonryMarkup(string source, string containsMarker)
    {
        const string masonryStartMarker = "<atom:Masonry";
        const string masonryEndMarker   = "</atom:Masonry>";

        var marker = source.IndexOf(containsMarker, StringComparison.Ordinal);
        marker.ShouldBeGreaterThanOrEqualTo(0);

        var masonryStart = source.LastIndexOf(masonryStartMarker, marker, StringComparison.Ordinal);
        masonryStart.ShouldBeGreaterThanOrEqualTo(0);

        var masonryEnd = source.IndexOf(masonryEndMarker, masonryStart, StringComparison.Ordinal);
        masonryEnd.ShouldBeGreaterThan(masonryStart);

        return source[masonryStart..(masonryEnd + masonryEndMarker.Length)];
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
