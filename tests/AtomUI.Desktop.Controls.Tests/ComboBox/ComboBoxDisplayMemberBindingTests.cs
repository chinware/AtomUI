using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AtomUIComboBox = AtomUI.Desktop.Controls.ComboBox;
using AtomUIComboBoxItem = AtomUI.Desktop.Controls.ComboBoxItem;

namespace AtomUI.Desktop.Controls.Tests.ComboBox;

public class ComboBoxDisplayMemberBindingTests
{
    static ComboBoxDisplayMemberBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void DisplayMemberBinding_DropDown_Item_Content_Is_Vertically_Centered()
    {
        var comboBox = new TestComboBox
        {
            Width                = 200,
            ItemsSource          = new List<DataItem> { new("Avalonia") },
            DisplayMemberBinding = new Binding(nameof(DataItem.Name)),
            OptionFontSize       = 17,
            IsMotionEnabled      = false
        };
        var item      = new DataItem("Avalonia");
        var container = new AtomUIComboBoxItem();

        ShowInWindow(container, () =>
        {
            comboBox.PrepareContainerForTest(container, item, 0);
            container.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();

            container.Content.ShouldBe(item);
            container.ContentTemplate.ShouldNotBeNull();
            container.FontSize.ShouldBe(17);
            container.VerticalContentAlignment.ShouldBe(
                VerticalAlignment.Center,
                "DisplayMemberBinding uses Avalonia's generated item template, so the ComboBoxItem itself must center presented content.");
        });
    }

    [Fact]
    public void Selected_Content_Presenter_Uses_SelectionBoxItemTemplate()
    {
        var selectedTemplate = new FuncDataTemplate<DataItem>((_, _) => new TextBlock { Text = "Selected value" });
        var comboBox = new AtomUIComboBox
        {
            Width                    = 200,
            ItemsSource              = new List<DataItem> { new("Avalonia") },
            SelectedIndex            = 0,
            SelectionBoxItemTemplate = selectedTemplate,
            IsMotionEnabled          = false
        };

        ShowInWindow(comboBox, () =>
        {
            var presenter = comboBox.GetVisualDescendants()
                                    .OfType<ContentPresenter>()
                                    .Single(x => x.Name == "SelectedContentPresenter");

            presenter.ContentTemplate.ShouldBeSameAs(
                selectedTemplate,
                "ComboBox must preserve Avalonia's SelectionBoxItemTemplate semantics instead of hard-wiring selected display to ItemTemplate.");
        });
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 220,
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

    private sealed record DataItem(string Name);

    private sealed class TestComboBox : AtomUIComboBox
    {
        public void PrepareContainerForTest(Control container, object? item, int index)
        {
            PrepareContainerForItemOverride(container, item, index);
        }
    }
}
