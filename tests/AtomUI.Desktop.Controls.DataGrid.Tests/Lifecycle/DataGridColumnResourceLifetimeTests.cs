using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.DataGrid.Lifecycle;

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class DataGridColumnResourceLifetimeCollection
{
    public const string Name = nameof(DataGridColumnResourceLifetimeCollection);
}

[Collection(DataGridColumnResourceLifetimeCollection.Name)]
public class DataGridColumnResourceLifetimeTests
{
    private const string HeaderResourceKey = "DataGridColumnResourceLifetimeTests.Header";

    static DataGridColumnResourceLifetimeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Dynamic_Resource_Header_Does_Not_Root_Unreferenced_DataGrid_Column()
    {
        var columnReference = CreateUnrootedColumnReference(CreateResourceKey());

        CollectGarbage();

        columnReference.IsAlive.ShouldBeFalse(
            "DataGridColumn dynamic resources must not be rooted by Application.ResourcesChanged after the page releases the column");
    }

    [Fact]
    public void Dynamic_Resource_Header_Updates_When_Application_Resource_Changes()
    {
        var resourceKey = CreateResourceKey();
        Application.Current!.Resources[resourceKey] = "Initial header";

        var column = new DataGridTextColumn();
        BindDynamicResource(column, DataGridColumn.HeaderProperty, resourceKey, Application.Current);

        var grid = new global::AtomUI.Desktop.Controls.DataGrid
        {
            AutoGenerateColumns = false,
            Width               = 500,
            Height              = 220
        };
        grid.Columns.Add(column);

        var window = new Window
        {
            Width   = 640,
            Height  = 320,
            Content = grid
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            column.Header.ShouldBe("Initial header");

            Application.Current.Resources[resourceKey] = "Updated header";
            Dispatcher.UIThread.RunJobs();

            column.Header.ShouldBe("Updated header");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Dynamic_Resource_Header_Does_Not_Root_Unreferenced_DataGrid_Column_Group_Item()
    {
        var groupItemReference = CreateUnrootedColumnGroupItemReference(CreateResourceKey());

        CollectGarbage();

        groupItemReference.IsAlive.ShouldBeFalse(
            "DataGridColumnGroupItem dynamic resources must not be rooted by Application.ResourcesChanged after the page releases the group item");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateUnrootedColumnReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Localized header";

        var column = new DataGridTextColumn
        {
            Binding = new Binding("Name")
            {
                Mode = BindingMode.OneWay
            }
        };
        BindDynamicResource(column, DataGridColumn.HeaderProperty, resourceKey, Application.Current);

        column.Header.ShouldBe("Localized header");
        return new WeakReference(column);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateUnrootedColumnGroupItemReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Localized group header";

        var groupItem = new DataGridColumnGroupItem();
        BindDynamicResource(groupItem, DataGridColumnGroupItem.HeaderProperty, resourceKey, Application.Current);

        groupItem.Header.ShouldBe("Localized group header");
        return new WeakReference(groupItem);
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
