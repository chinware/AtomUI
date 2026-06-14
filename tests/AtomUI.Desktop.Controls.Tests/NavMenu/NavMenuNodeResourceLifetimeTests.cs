using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
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

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateUnrootedNodeReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Localized header";

        var node = new NavMenuNode();
        BindDynamicResource(node, NavMenuNode.HeaderProperty, resourceKey, Application.Current);

        node.Header.ShouldBe("Localized header");
        return new WeakReference(node);
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
}
