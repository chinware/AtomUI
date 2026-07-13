using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuNodeResourceLifetimeTests
{
    private const string HeaderResourceKey = "NavMenuNodeResourceLifetimeTests.Header";

    static NavMenuNodeResourceLifetimeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Dynamic_Resource_Header_Does_Not_Root_Unreferenced_NavMenuNode()
    {
        var nodeReference = CreateUnrootedNodeReference(CreateResourceKey());

        CollectGarbage();

        nodeReference.IsAlive.ShouldBeFalse(
            "NavMenuNode dynamic resources must not be rooted by Application.ResourcesChanged after the page releases the node");
    }

    [Fact]
    public void Dynamic_Resource_Header_Uses_Owner_Menu_Resources()
    {
        var resourceKey = CreateResourceKey();
        Application.Current!.Resources[resourceKey] = "Application header";

        var node = new NavMenuNode();
        BindDynamicResource(node, NavMenuNode.HeaderProperty, resourceKey, Application.Current);

        var menu = new global::AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Resources[resourceKey] = "Menu header";
        menu.Items.Add(node);

        var window = new global::Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            node.Header.ShouldBe("Menu header");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Dynamic_Resource_Header_Tracks_Owner_Menu_Resource_Updates()
    {
        var resourceKey = CreateResourceKey();
        Application.Current!.Resources[resourceKey] = "Application header";

        var node = new NavMenuNode();
        BindDynamicResource(node, NavMenuNode.HeaderProperty, resourceKey, Application.Current);

        var menu = new global::AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Resources[resourceKey] = "Menu header";
        menu.Items.Add(node);

        var window = new global::Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            node.Header.ShouldBe("Menu header");
            menu.Resources[resourceKey] = "Updated menu header";
            Dispatcher.UIThread.RunJobs();

            node.Header.ShouldBe("Updated menu header");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Repeated_Resource_Host_Attachments_Release_Only_After_Last_Token()
    {
        var resourceKey = CreateResourceKey();
        Application.Current!.Resources[resourceKey] = "Application header";

        var node = new NavMenuNode();
        BindDynamicResource(node, NavMenuNode.HeaderProperty, resourceKey, Application.Current);
        var menu = new global::AtomUI.Desktop.Controls.NavMenu();
        menu.Resources[resourceKey] = "Menu header";

        using var firstAttachment = node.AttachResourceHost(menu);
        using var secondAttachment = node.AttachResourceHost(menu);
        node.Header.ShouldBe("Menu header");

        firstAttachment.Dispose();
        menu.Resources[resourceKey] = "Updated menu header";
        Dispatcher.UIThread.RunJobs();
        node.Header.ShouldBe("Updated menu header");

        secondAttachment.Dispose();
        Dispatcher.UIThread.RunJobs();
        node.Header.ShouldBe("Application header");
    }

    [Fact]
    public void Stale_Attachment_Token_Cannot_Detach_Reentered_Resource_Host()
    {
        var resourceKey = CreateResourceKey();
        Application.Current!.Resources[resourceKey] = "Application header";

        var node = new NavMenuNode();
        BindDynamicResource(node, NavMenuNode.HeaderProperty, resourceKey, Application.Current);
        var firstMenu = new global::AtomUI.Desktop.Controls.NavMenu();
        var secondMenu = new global::AtomUI.Desktop.Controls.NavMenu();
        firstMenu.Resources[resourceKey] = "First menu header";
        secondMenu.Resources[resourceKey] = "Second menu header";

        using var staleFirstAttachment = node.AttachResourceHost(firstMenu);
        using var secondAttachment = node.AttachResourceHost(secondMenu);
        using var currentFirstAttachment = node.AttachResourceHost(firstMenu);
        node.Header.ShouldBe("First menu header");

        staleFirstAttachment.Dispose();
        firstMenu.Resources[resourceKey] = "Updated first menu header";
        Dispatcher.UIThread.RunJobs();

        node.Header.ShouldBe("Updated first menu header");
        currentFirstAttachment.Dispose();
        Dispatcher.UIThread.RunJobs();
        node.Header.ShouldBe("Application header");
    }

    [Fact]
    public void Dynamic_Resource_Header_Does_Not_Root_Removed_NavMenuNode()
    {
        var nodeReference = CreateRemovedNodeReference(CreateResourceKey());

        CollectGarbage();

        nodeReference.IsAlive.ShouldBeFalse(
            "NavMenu container release must detach the scoped resource host from removed nodes");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateUnrootedNodeReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Localized header";

        var node = new NavMenuNode();
        BindDynamicResource(node, NavMenuNode.HeaderProperty, resourceKey, Application.Current);

        node.Header.ShouldBe("Localized header");
        return new WeakReference(node);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateRemovedNodeReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Application header";

        NavMenuNode? node = new NavMenuNode();
        BindDynamicResource(node, NavMenuNode.HeaderProperty, resourceKey, Application.Current);
        AvaloniaList<INavMenuNode>? nodes = new AvaloniaList<INavMenuNode> { node };
        global::AtomUI.Desktop.Controls.NavMenu? menu = new global::AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false,
            ItemsSource     = nodes
        };
        menu.Resources[resourceKey] = "Menu header";

        var window = new global::Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            node.Header.ShouldBe("Menu header");
            nodes.Remove(node);
            RunDispatcherJobsUntil(() => menu.ContainerFromItem(node) is null);
            node.Header.ShouldBe("Application header");

            var nodeReference = new WeakReference(node);
            menu.ItemsSource = null;
            window.Content = null;
            node = null;
            nodes = null;
            menu = null;
            return nodeReference;
        }
        finally
        {
            window.Content = null;
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static string CreateResourceKey()
    {
        return $"{HeaderResourceKey}.{Guid.NewGuid():N}";
    }

    private static void BindDynamicResource(AvaloniaObject target, AvaloniaProperty property, object key, object? anchor)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var bindMethod = typeof(AvaloniaObject).GetMethod(
            "Bind",
            flags,
            binder: null,
            types: new[] { typeof(AvaloniaProperty), typeof(BindingBase), typeof(object) },
            modifiers: null);

        bindMethod.ShouldNotBeNull();

        var extension = new DynamicResourceExtension(key);
        var anchorField = typeof(DynamicResourceExtension).GetField("_anchor", flags);
        anchorField.ShouldNotBeNull();
        anchorField.SetValue(extension, anchor);

        bindMethod.Invoke(target, new object?[] { property, extension, anchor });
    }

    private static void CollectGarbage()
    {
        for (var i = 0; i < 3; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        Dispatcher.UIThread.RunJobs();
    }

    private static void RunDispatcherJobsUntil(Func<bool> condition, int maxPasses = 128)
    {
        for (var i = 0; i < maxPasses; i++)
        {
            if (condition())
            {
                return;
            }

            Dispatcher.UIThread.RunJobs();
        }

        condition().ShouldBeTrue("The expected NavMenu container state was not reached before the dispatcher pass limit.");
    }
}
