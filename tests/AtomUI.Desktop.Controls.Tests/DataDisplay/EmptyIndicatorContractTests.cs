using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomCascaderView = AtomUI.Desktop.Controls.CascaderView;
using AtomEmpty = AtomUI.Desktop.Controls.Empty;
using AtomListBox = AtomUI.Desktop.Controls.ListBox;
using AtomListView = AtomUI.Desktop.Controls.ListView;
using AtomTreeView = AtomUI.Desktop.Controls.TreeView;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.DataDisplay;

public class EmptyIndicatorContractTests
{
    static EmptyIndicatorContractTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ListBox_EmptyIndicator_Overrides_Default_Empty()
    {
        var customIndicator = CreateCustomIndicator("listbox-empty");
        var listBox = new AtomListBox
        {
            Width          = 240,
            Height         = 160,
            EmptyIndicator = customIndicator
        };

        ShowInWindow(listBox, () => AssertCustomIndicator(listBox, customIndicator));
    }

    [Fact]
    public void ListView_EmptyIndicator_Overrides_Default_Empty()
    {
        var customIndicator = CreateCustomIndicator("listview-empty");
        var listView = new AtomListView
        {
            Width          = 240,
            Height         = 160,
            EmptyIndicator = customIndicator
        };

        ShowInWindow(listView, () => AssertCustomIndicator(listView, customIndicator));
    }

    [Fact]
    public void TreeView_EmptyIndicator_Overrides_Default_Empty()
    {
        var customIndicator = CreateCustomIndicator("treeview-empty");
        var treeView = new AtomTreeView
        {
            Width          = 240,
            Height         = 160,
            EmptyIndicator = customIndicator
        };

        ShowInWindow(treeView, () => AssertCustomIndicator(treeView, customIndicator));
    }

    [Fact]
    public void CascaderView_EmptyIndicator_Overrides_Default_Empty()
    {
        var customIndicator = CreateCustomIndicator("cascader-empty");
        var cascaderView = new AtomCascaderView
        {
            Width          = 240,
            Height         = 160,
            EmptyIndicator = customIndicator
        };

        ShowInWindow(cascaderView, () => AssertCustomIndicator(cascaderView, customIndicator));
    }

    [Fact]
    public void EmptyIndicatorTemplate_Still_Renders_When_Content_Is_Null()
    {
        var listBox = new AtomListBox
        {
            Width                  = 240,
            Height                 = 160,
            EmptyIndicatorTemplate = new FuncDataTemplate<object?>((_, _) => CreateCustomIndicator("template-empty"))
        };

        ShowInWindow(listBox, () =>
        {
            var presenter = FindEmptyIndicatorPresenter(listBox);

            presenter.GetVisualDescendants()
                     .OfType<TextBlock>()
                     .Single(textBlock => textBlock.Text == "template-empty");

            presenter.GetVisualDescendants()
                     .OfType<AtomEmpty>()
                     .ShouldBeEmpty("an explicit EmptyIndicatorTemplate should own the rendered empty content.");
        });
    }

    [Fact]
    public void Default_EmptyIndicator_Renders_Without_ContentPresenter_Wrapper()
    {
        Control[] controls =
        {
            new AtomListBox(),
            new AtomListView(),
            new AtomTreeView(),
            new AtomCascaderView()
        };

        foreach (var control in controls)
        {
            control.Width  = 240;
            control.Height = 160;

            ShowInWindow(control, () =>
            {
                control.GetVisualDescendants()
                       .OfType<AtomEmpty>()
                       .Single(empty => empty.Name == "DefaultEmptyIndicator");

                control.GetVisualDescendants()
                       .OfType<ContentPresenter>()
                       .ShouldNotContain(presenter => presenter.Name == "DefaultEmptyIndicator");
            });
        }
    }

    private static TextBlock CreateCustomIndicator(string text)
    {
        return new TextBlock
        {
            Name = text,
            Text = text
        };
    }

    private static void AssertCustomIndicator(Control control, Control customIndicator)
    {
        var presenter = FindEmptyIndicatorPresenter(control);

        presenter.Content.ShouldBeSameAs(customIndicator);
        presenter.GetVisualDescendants()
                 .ShouldContain(visual => ReferenceEquals(visual, customIndicator));

        presenter.GetVisualDescendants()
                 .OfType<AtomEmpty>()
                 .ShouldBeEmpty("default Empty must not be applied to user-provided EmptyIndicator content.");
    }

    private static ContentPresenter FindEmptyIndicatorPresenter(Control control)
    {
        return control.GetVisualDescendants()
                      .OfType<ContentPresenter>()
                      .Single(presenter => presenter.Name == "EmptyIndicator");
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 320,
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
            window.Content = null;
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
