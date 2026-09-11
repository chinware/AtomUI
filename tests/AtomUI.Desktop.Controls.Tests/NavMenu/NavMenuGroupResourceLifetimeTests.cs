using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuGroupResourceLifetimeTests
{
    static NavMenuGroupResourceLifetimeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Dynamic_Resource_Header_Does_Not_Root_Unreferenced_Group()
    {
        var reference = CreateUnrootedGroupReference(CreateResourceKey());

        CollectGarbage();

        reference.IsAlive.ShouldBeFalse();
    }

    [Fact]
    public void Dynamic_Resource_Header_Uses_And_Tracks_Owner_Menu_Resources()
    {
        var resourceKey = CreateResourceKey();
        Application.Current!.Resources[resourceKey] = "Application header";
        var group = new NavMenuGroup();
        BindDynamicResource(group, NavMenuGroup.HeaderProperty, resourceKey, Application.Current);
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Resources[resourceKey] = "Menu header";
        menu.Items.Add(group);

        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            group.Header.ShouldBe("Menu header");

            menu.Resources[resourceKey] = "Updated menu header";
            Dispatcher.UIThread.RunJobs();
            group.Header.ShouldBe("Updated menu header");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Dynamic_Resource_Header_Does_Not_Root_Removed_Group()
    {
        var reference = CreateRemovedGroupReference(CreateResourceKey());

        CollectGarbage();

        reference.IsAlive.ShouldBeFalse(
            "NavMenu group container release must detach the scoped resource host from removed groups");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateUnrootedGroupReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Localized header";
        var group = new NavMenuGroup();
        BindDynamicResource(group, NavMenuGroup.HeaderProperty, resourceKey, Application.Current);
        group.Header.ShouldBe("Localized header");
        return new WeakReference(group);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateRemovedGroupReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Application header";
        NavMenuGroup? group = new NavMenuGroup();
        BindDynamicResource(group, NavMenuGroup.HeaderProperty, resourceKey, Application.Current);
        AvaloniaList<INavMenuEntry>? entries = [group];
        AtomUI.Desktop.Controls.NavMenu? menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false,
            ItemsSource     = entries
        };
        menu.Resources[resourceKey] = "Menu header";
        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            group.Header.ShouldBe("Menu header");

            entries.Remove(group);
            RunDispatcherJobsUntil(() => menu.ContainerFromItem(group) is null);
            group.Header.ShouldBe("Application header");

            var reference = new WeakReference(group);
            menu.ItemsSource = null;
            window.Content = null;
            group = null;
            entries = null;
            menu = null;
            return reference;
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
        return $"NavMenuGroupResourceLifetimeTests.Header.{Guid.NewGuid():N}";
    }

    private static void BindDynamicResource(
        AvaloniaObject target,
        AvaloniaProperty property,
        object key,
        object? anchor)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var bindMethod = typeof(AvaloniaObject).GetMethod(
            "Bind",
            flags,
            binder: null,
            types: [typeof(AvaloniaProperty), typeof(BindingBase), typeof(object)],
            modifiers: null);
        bindMethod.ShouldNotBeNull();

        var extension = new DynamicResourceExtension(key);
        var anchorField = typeof(DynamicResourceExtension).GetField("_anchor", flags);
        anchorField.ShouldNotBeNull();
        anchorField.SetValue(extension, anchor);
        bindMethod.Invoke(target, [property, extension, anchor]);
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

        condition().ShouldBeTrue();
    }
}
