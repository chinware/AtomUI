using AtomUI.Controls;
using AtomUI.Controls.Commons;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Tag;

public class CheckableTagGroupTests
{
    static CheckableTagGroupTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Selection_Properties_Use_The_Documented_Defaults_And_Binding_Modes()
    {
        var group = new CheckableTagGroup();
        var checkedItemMetadata = CheckableTagGroup.CheckedItemProperty.GetMetadata(typeof(CheckableTagGroup));
        var checkedItemsMetadata = CheckableTagGroup.CheckedItemsProperty.GetMetadata(typeof(CheckableTagGroup));

        group.IsMultiple.ShouldBeFalse();
        group.CheckedItem.ShouldBeNull();
        group.CheckedItems.ShouldBeNull();
        checkedItemMetadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        checkedItemMetadata.EnableDataValidation.ShouldBe(true);
        checkedItemsMetadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        checkedItemsMetadata.EnableDataValidation.ShouldBe(true);
    }

    [Fact]
    public void Selection_Host_Uses_SelectingItemsControl_Without_A_ListBox_Contract()
    {
        var templatePart = typeof(AbstractCheckableTagGroup)
                          .GetCustomAttributes(typeof(TemplatePartAttribute), true)
                          .Cast<TemplatePartAttribute>()
                          .Single(attribute => attribute.Name == "PART_CheckableTagItems");
        templatePart.Type.ShouldBe(typeof(SelectingItemsControl));

        var group = new CheckableTagGroup
        {
            Options = new[] { "Movies", "Books" }
        };

        ShowInWindow(group, () =>
        {
            var selectionHost = group.GetVisualDescendants()
                                     .OfType<SelectingItemsControl>()
                                     .Single();
            (selectionHost is Avalonia.Controls.ListBox).ShouldBeFalse();
        });
    }

    [Fact]
    public void Primitive_Options_Default_To_Cancellable_Single_Selection()
    {
        var group = new CheckableTagGroup
        {
            Options            = new[] { "Movies", "Books", "Music", "Sports" },
            DefaultCheckedItem = "Books"
        };

        ShowInWindow(group, () =>
        {
            group.CheckedItem.ShouldBe("Books");
            FindTag(group, "Books").IsChecked.ShouldBe(true);

            FindTag(group, "Music").IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBe("Music");
            FindTag(group, "Books").IsChecked.ShouldBe(false);

            FindTag(group, "Music").IsChecked = false;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItem.ShouldBeNull();
        });
    }

    [Fact]
    public void Structured_Options_Return_Values_And_Preserve_Multiple_Selection_Order()
    {
        var movies = new CheckableTagOption { Value = "movies", Content = "Movies" };
        var books  = new CheckableTagOption { Value = "books", Content = "Books" };
        var music  = new CheckableTagOption { Value = "music", Content = "Music" };
        var group = new CheckableTagGroup
        {
            IsMultiple         = true,
            Options            = new[] { movies, books, music },
            DefaultCheckedItems = new[] { "movies", "music" }
        };

        ShowInWindow(group, () =>
        {
            group.CheckedItems.ShouldBe(new[] { "movies", "music" });
            FindTag(group, "Movies").IsChecked.ShouldBe(true);
            FindTag(group, "Books").IsChecked.ShouldBe(false);

            FindTag(group, "Books").IsChecked = true;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItems.ShouldBe(new[] { "movies", "music", "books" });
            group.CheckedItems!.Cast<object>().ShouldNotContain(movies);
            group.CheckedItems!.Cast<object>().ShouldNotContain(books);

            FindTag(group, "Movies").IsChecked = false;
            Dispatcher.UIThread.RunJobs();

            group.CheckedItems.ShouldBe(new[] { "music", "books" });
        });
    }

    [Fact]
    public void Structured_Option_ItemTemplate_Receives_The_Original_Option()
    {
        var movies = new CheckableTagOption { Value = "movies", Content = "Movies" };
        var template = new FuncDataTemplate<CheckableTagOption>(
            (option, _) => new TextBlock { Text = $"{option?.Value}:{option?.Content}" });
        var group = new CheckableTagGroup
        {
            Options      = new[] { movies },
            ItemTemplate = template
        };

        ShowInWindow(group, () =>
        {
            var tag = group.GetVisualDescendants().OfType<CheckableTag>().Single();
            tag.Content.ShouldBeSameAs(movies);
            tag.ContentTemplate.ShouldBeSameAs(template);
            tag.GetVisualDescendants()
               .OfType<TextBlock>()
               .Single(textBlock => textBlock.Text == "movies:Movies");
        });
    }

    private static CheckableTag FindTag(Control root, object content)
    {
        return root.GetVisualDescendants()
                   .OfType<CheckableTag>()
                   .Single(tag => Equals(tag.Content, content));
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 240,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
