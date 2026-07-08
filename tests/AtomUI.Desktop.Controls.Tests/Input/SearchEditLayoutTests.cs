using System;
using System.Linq;
using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUISearchEdit = AtomUI.Desktop.Controls.SearchEdit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Input;

public class SearchEditLayoutTests
{
    static SearchEditLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Custom_Size_Search_Button_Frame_Matches_Input_Frame_Height()
    {
        var searchEdit = new AtomUISearchEdit
        {
            Width      = 360,
            Height     = 38,
            FontSize   = 15,
            SizeType   = CustomizableSizeType.Custom,
            SearchButtonText = "Search"
        };

        ShowInWindow(searchEdit, () =>
        {
            var contentFrame = FindTemplatePart<Border>(searchEdit, "PART_ContentFrame");
            var searchButton = FindTemplatePart<global::AtomUI.Desktop.Controls.Button>(searchEdit, "PART_RightAddOn");
            var buttonFrame  = FindTemplatePart<Control>(searchButton, "Frame");

            contentFrame.Bounds.Height.ShouldBe(38, 0.5);
            buttonFrame.Bounds.Height.ShouldBe(contentFrame.Bounds.Height, 0.5);
        });
    }

    [Fact]
    public void Built_In_Size_Search_Button_Frame_Matches_Input_Frame_Height()
    {
        var sizeTypes = new[]
        {
            CustomizableSizeType.Large,
            CustomizableSizeType.Middle,
            CustomizableSizeType.Small
        };

        foreach (var sizeType in sizeTypes)
        {
            var searchEdit = new AtomUISearchEdit
            {
                Width            = 360,
                SizeType         = sizeType,
                SearchButtonText = "Search"
            };

            ShowInWindow(searchEdit, () =>
            {
                var contentFrame = FindTemplatePart<Border>(searchEdit, "PART_ContentFrame");
                var searchButton = FindTemplatePart<global::AtomUI.Desktop.Controls.Button>(searchEdit, "PART_RightAddOn");
                var buttonFrame  = FindTemplatePart<Control>(searchButton, "Frame");

                contentFrame.Bounds.Height.ShouldBeGreaterThan(0);
                searchButton.Bounds.Height.ShouldBe(contentFrame.Bounds.Height, 0.5);
                buttonFrame.Bounds.Height.ShouldBe(contentFrame.Bounds.Height, 0.5);
            });
        }
    }

    private static T FindTemplatePart<T>(Control control, string name)
        where T : Control
    {
        var part = control.GetVisualDescendants()
                          .OfType<T>()
                          .SingleOrDefault(item => item.Name == name);
        part.ShouldNotBeNull();
        return part!;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 160,
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
        }
    }
}
