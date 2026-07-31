using System.Windows.Input;
using AtomUI.Desktop.Controls;
using AtomUIGallery.ShowCases.Masonry;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class MasonryShowCasePageTests
{
    [Fact]
    public void Masonry_ShowCase_Image_State_Binding_Is_Aot_Safe()
    {
        var source = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Layout/Masonry/Views/MasonryShowCase.axaml.cs");

        source.ShouldContain("BindUtils.RelayBind");
        source.ShouldNotContain("new Binding");
        source.ShouldNotContain("Path      = nameof(Image.Source)");
    }

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
        basicDemoMarkup.ShouldContain("Name=\"SpecialCoverSkeleton\"");
        basicDemoMarkup.ShouldContain("Loaded=\"HandleImageSkeletonLoaded\"");
        basicDemoMarkup.ShouldNotContain("{Binding #");
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
    public void Masonry_ShowCase_Image_Demo_Matches_Ant_Design_Image_Demo()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Masonry/Views/MasonryShowCase.axaml");
        var imageDemoMarkup = ExtractMasonryMarkup(source, "ItemsSource=\"{Binding ImageItems}\"");

        imageDemoMarkup.ShouldContain("ColumnCount=\"4\"");
        imageDemoMarkup.ShouldContain("ColumnGap=\"16\"");
        imageDemoMarkup.ShouldContain("RowGap=\"16\"");
        imageDemoMarkup.ShouldContain("asyncImageLoader:ImageLoader.Source=\"{Binding ImageSource}\"");
        imageDemoMarkup.ShouldContain("Name=\"MasonryImage\"");
        imageDemoMarkup.ShouldContain("Stretch=\"Uniform\"");
        imageDemoMarkup.ShouldContain("HorizontalAlignment=\"Stretch\"");
        imageDemoMarkup.ShouldContain("ClipToBounds=\"True\"");
        imageDemoMarkup.ShouldContain("MinHeight=\"210\"");
        imageDemoMarkup.ShouldContain("Name=\"MasonryImageSkeleton\"");
        imageDemoMarkup.ShouldContain("Loaded=\"HandleImageSkeletonLoaded\"");
        imageDemoMarkup.ShouldNotContain("{Binding #");
        imageDemoMarkup.ShouldContain("<atom:Skeleton IsLoading=\"True\"");
        imageDemoMarkup.ShouldContain("Padding=\"16,16\"");
        imageDemoMarkup.ShouldContain("IsActive=\"True\"");
        imageDemoMarkup.ShouldContain("IsShowAvatar=\"False\"");
        imageDemoMarkup.ShouldContain("IsShowTitle=\"True\"");
        imageDemoMarkup.ShouldContain("ParagraphRows=\"3\"");
        imageDemoMarkup.ShouldContain("IsRound=\"True\"");
        imageDemoMarkup.ShouldNotContain("<atom:Card");
        imageDemoMarkup.ShouldNotContain("<atom:SkeletonImage");
        imageDemoMarkup.ShouldNotContain("<Panel MinHeight=");
        imageDemoMarkup.ShouldNotContain("Height=\"{Binding Height}\"");
    }

    [Fact]
    public void Masonry_ShowCase_Image_Items_Mirror_Ant_Design_Image_Demo()
    {
        var viewModel = new MasonryViewModel(null!);

        viewModel.ImageItems.ShouldNotBeNull();
        viewModel.ImageItems!.Select(item => item.Index).ShouldBe(Enumerable.Range(1, 16));
        viewModel.ImageItems.Select(item => item.ImageSource).ShouldBe(new[]
        {
            "https://images.unsplash.com/photo-1510001618818-4b4e3d86bf0f?w=523&auto=format",
            "https://images.unsplash.com/photo-1507513319174-e556268bb244?w=523&auto=format",
            "https://images.unsplash.com/photo-1474181487882-5abf3f0ba6c2?w=523&auto=format",
            "https://images.unsplash.com/photo-1492778297155-7be4c83960c7?w=523&auto=format",
            "https://images.unsplash.com/photo-1508062878650-88b52897f298?w=523&auto=format",
            "https://images.unsplash.com/photo-1506158278516-d720e72406fc?w=523&auto=format",
            "https://images.unsplash.com/photo-1552203274-e3c7bd771d26?w=523&auto=format",
            "https://images.unsplash.com/photo-1528163186890-de9b86b54b51?w=523&auto=format",
            "https://images.unsplash.com/photo-1727423304224-6d2fd99b864c?w=523&auto=format",
            "https://images.unsplash.com/photo-1675090391405-432434e23595?w=523&auto=format",
            "https://images.unsplash.com/photo-1554196967-97a8602084d9?w=523&auto=format",
            "https://images.unsplash.com/photo-1491961865842-98f7befd1a60?w=523&auto=format",
            "https://images.unsplash.com/photo-1721728613411-d56d2ddda959?w=523&auto=format",
            "https://images.unsplash.com/photo-1731901245099-20ac7f85dbaa?w=523&auto=format",
            "https://images.unsplash.com/photo-1617694455303-59af55af7e58?w=523&auto=format",
            "https://images.unsplash.com/photo-1709198165282-1dab551df890?w=523&auto=format"
        });
    }

    [Fact]
    public void Masonry_ShowCase_Dynamic_Demo_Matches_Ant_Design_Dynamic_Demo()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Masonry/Views/MasonryShowCase.axaml");
        var dynamicDemoMarkup = ExtractMasonryMarkup(source, "ItemsSource=\"{Binding DynamicItems}\"");

        dynamicDemoMarkup.ShouldContain("ColumnCount=\"4\"");
        dynamicDemoMarkup.ShouldContain("ColumnGap=\"16\"");
        dynamicDemoMarkup.ShouldContain("RowGap=\"16\"");
        dynamicDemoMarkup.ShouldContain("ItemContainerTheme");
        dynamicDemoMarkup.ShouldContain("x:DataType=\"vm:MasonryDynamicItem\"");
        dynamicDemoMarkup.ShouldContain("Property=\"atom:Masonry.Column\"");
        dynamicDemoMarkup.ShouldContain("Value=\"{CompiledBinding Column}\"");
        dynamicDemoMarkup.ShouldNotContain("x:CompileBindings=\"False\"");
        dynamicDemoMarkup.ShouldNotContain("Value=\"{Binding Column}\"");
        dynamicDemoMarkup.ShouldContain("LayoutChanged=\"HandleDynamicMasonryLayoutChanged\"");
        dynamicDemoMarkup.ShouldContain("Height=\"{Binding Height}\"");
        dynamicDemoMarkup.ShouldContain("Text=\"{Binding DisplayText}\"");
        dynamicDemoMarkup.ShouldContain("Click=\"HandleRemoveDynamicMasonryItemClick\"");
        dynamicDemoMarkup.ShouldContain("Icon=\"{antdicons:AntDesignIconProvider Kind=CloseOutlined}\"");
        source.ShouldContain("Command=\"{Binding AddDynamicMasonryItemCommand}\"");
        source.ShouldContain("Content=\"{gallery:MasonryShowCaseLangResource DynamicAddItemLabel}\"");
    }

    [Fact]
    public void Masonry_ShowCase_Dynamic_Items_Mirror_Ant_Design_Dynamic_Demo()
    {
        var viewModel = new MasonryViewModel(null!);

        viewModel.DynamicItems.ShouldNotBeNull();
        viewModel.DynamicItems!.Select(item => item.Key).ShouldBe(Enumerable.Range(0, 15));
        viewModel.DynamicItems.Select(item => item.Height).ShouldBe(new[]
        {
            150, 50, 90, 70, 110, 150, 130, 80, 50, 90, 100, 150, 70, 50, 80
        });
        viewModel.DynamicItems.Select(item => item.Column).ShouldBe(new int?[]
        {
            0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2, 3, 0, 1, 2
        });
        viewModel.DynamicItems.Select(item => item.DisplayText).ShouldBe(Enumerable.Range(1, 15).Select(index => index.ToString()));

        ((ICommand)viewModel.RemoveDynamicMasonryItemCommand).Execute(4);
        viewModel.DynamicItems.Count.ShouldBe(14);
        viewModel.DynamicItems.Select(item => item.Key).ShouldNotContain(4);

        ((ICommand)viewModel.AddDynamicMasonryItemCommand).Execute(null);
        viewModel.DynamicItems.Count.ShouldBe(15);
        var addedItem = viewModel.DynamicItems[^1];
        addedItem.Key.ShouldBe(15);
        addedItem.Height.ShouldBeInRange(50, 149);
        addedItem.Column.ShouldBeNull();
        addedItem.DisplayText.ShouldBe("16");

        viewModel.UpdateDynamicMasonryColumns(new[]
        {
            new MasonryItemLayout(new Border(), 0, 3, false),
            new MasonryItemLayout(new Border(), 2, 1, false)
        });
        viewModel.DynamicItems[0].Column.ShouldBe(3);
        viewModel.DynamicItems[2].Column.ShouldBe(1);
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
