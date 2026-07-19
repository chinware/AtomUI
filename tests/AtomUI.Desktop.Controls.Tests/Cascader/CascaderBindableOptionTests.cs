using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Cascader;

public class CascaderBindableOptionTests
{
    private const string HeaderResourceKey = "CascaderBindableOptionTests.Header";

    static CascaderBindableOptionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void BindableCascaderOption_Children_Update_ParentNode()
    {
        var parent = new BindableCascaderOption { Header = "Parent" };
        var child  = new BindableCascaderOption { Header = "Child" };

        parent.Children.Add(child);

        child.ParentNode.ShouldBeSameAs(parent);

        parent.Children.Remove(child);

        child.ParentNode.ShouldBeNull();
    }

    [Fact]
    public void BindableCascaderOption_Children_Clear_Detaches_Previous_ParentNode()
    {
        var parent = new BindableCascaderOption { Header = "Parent" };
        var child  = new BindableCascaderOption { Header = "Child" };
        parent.Children.Add(child);

        parent.Children.Clear();

        child.ParentNode.ShouldBeNull();
    }

    [Fact]
    public void BindableCascaderOption_Property_Changes_Update_Realized_Container()
    {
        var option = new BindableCascaderOption
        {
            Header            = "Root",
            ItemKey           = "root",
            IsChecked         = false,
            IsEnabled         = true,
            IsExpanded        = false,
            IsCheckBoxEnabled = true,
            IsLeaf            = false,
            Value             = "root-value"
        };
        var cascaderView = new CascaderView
        {
            IsCheckable          = true,
            IsMotionEnabled      = false,
            IsShowEmptyIndicator = false,
            OptionsSource        = new[] { option }
        };

        ShowInWindow(cascaderView, () =>
        {
            var container = FindCascaderViewItem(cascaderView, option);
            container.ShouldNotBeNull();

            option.ItemKey           = "updated-root";
            option.IsChecked         = true;
            option.IsEnabled         = false;
            option.IsExpanded        = true;
            option.IsCheckBoxEnabled = false;
            option.IsLeaf            = true;
            option.Value             = "updated-value";
            Dispatcher.UIThread.RunJobs();

            container.Header.ShouldBeSameAs(option);
            container.ItemKey.ShouldBe("updated-root");
            container.IsChecked.ShouldBe(true);
            container.IsEnabled.ShouldBeFalse();
            container.IsExpanded.ShouldBeTrue();
            container.IsCheckBoxEnabled.ShouldBeFalse();
            container.IsLeaf.ShouldBeTrue();
            container.Value.ShouldBe("updated-value");
        });
    }

    [Fact]
    public void Container_State_Changes_Write_Back_To_BindableCascaderOption()
    {
        var option = new BindableCascaderOption
        {
            Header     = "Root",
            IsChecked  = false,
            IsExpanded = false
        };
        var cascaderView = new CascaderView
        {
            IsCheckable          = true,
            IsMotionEnabled      = false,
            IsShowEmptyIndicator = false,
            OptionsSource        = new[] { option }
        };

        ShowInWindow(cascaderView, () =>
        {
            var container = FindCascaderViewItem(cascaderView, option);
            container.ShouldNotBeNull();

            container.SetCurrentValue(CascaderViewItem.IsCheckedProperty, true);
            container.SetCurrentValue(CascaderViewItem.IsExpandedProperty, true);
            Dispatcher.UIThread.RunJobs();

            option.IsChecked.ShouldBe(true);
            option.IsExpanded.ShouldBeTrue();
        });
    }

    [Fact]
    public void Recycled_Container_Restores_Expansion_From_Option_Instead_Of_Stale_Context()
    {
        var option = new CascaderOption
        {
            Header     = "Root",
            IsExpanded = true
        };
        var cascaderView = new CascaderView
        {
            IsMotionEnabled      = false,
            IsShowEmptyIndicator = false
        };
        var levelList = new TestCascaderViewLevelList
        {
            OwnerView = cascaderView
        };
        var container = new CascaderViewItem
        {
            DataContext = option
        };
        CascaderViewItem.ApplyOptionData(container, option);

        var savedContext = new Dictionary<object, object?>();
        levelList.SaveContainerContext(container, savedContext);

        option.IsExpanded = false;
        levelList.ClearContainerContext(container);
        levelList.RestoreDefaultContainerContext(container, option);
        levelList.RestoreContainerContext(container, savedContext);

        container.IsExpanded.ShouldBeFalse();
    }

    [Fact]
    public void BindableCascaderOption_Children_Changes_Update_Realized_Container_Leaf_State()
    {
        var option = new BindableCascaderOption
        {
            Header = "Root"
        };
        var child = new BindableCascaderOption
        {
            Header = "Child"
        };
        var cascaderView = new CascaderView
        {
            IsMotionEnabled      = false,
            IsShowEmptyIndicator = false,
            OptionsSource        = new[] { option }
        };

        ShowInWindow(cascaderView, () =>
        {
            var container = FindCascaderViewItem(cascaderView, option);
            container.ShouldNotBeNull();
            container.IsLeaf.ShouldBeTrue();

            option.Children.Add(child);
            Dispatcher.UIThread.RunJobs();
            container.IsLeaf.ShouldBeFalse();

            option.Children.Remove(child);
            Dispatcher.UIThread.RunJobs();
            container.IsLeaf.ShouldBeTrue();
        });
    }

    [Fact]
    public void Dynamic_Resource_Header_Uses_Owner_CascaderView_Resources()
    {
        var resourceKey = CreateResourceKey();
        Application.Current!.Resources[resourceKey] = "Application header";

        var option = new BindableCascaderOption();
        BindDynamicResource(option, BindableCascaderOption.HeaderProperty, resourceKey, Application.Current);

        var cascaderView = new CascaderView
        {
            IsShowEmptyIndicator = false,
            OptionsSource        = new[] { option }
        };
        cascaderView.Resources[resourceKey] = "CascaderView header";

        ShowInWindow(cascaderView, () =>
        {
            option.Header.ShouldBe("CascaderView header");
        });
    }

    [Fact]
    public void Removed_BindableCascaderOption_Without_DynamicResource_Is_Not_Rooted()
    {
        var optionReference = CreateRemovedPlainOptionReference();

        CollectGarbage();

        optionReference.IsAlive.ShouldBeFalse(
            "CascaderView container recycling must not keep a removed BindableCascaderOption alive.");
    }

    [Fact]
    public void Dynamic_Resource_Header_Does_Not_Root_Unowned_BindableCascaderOption()
    {
        var optionReference = CreateUnownedOptionReference(CreateResourceKey());

        CollectGarbage();

        optionReference.IsAlive.ShouldBeFalse(
            "BindableCascaderOption dynamic resources must not be rooted when no owner is attached.");
    }

    [Fact]
    public void Dynamic_Resource_Header_Does_Not_Root_Removed_BindableCascaderOption()
    {
        var optionReference = CreateRemovedOptionReference(CreateResourceKey());

        CollectGarbage();

        optionReference.IsAlive.ShouldBeFalse(
            "BindableCascaderOption dynamic resources must not keep a removed option alive through Application.ResourcesChanged.");
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateRemovedPlainOptionReference()
    {
        BindableCascaderOption? option = new BindableCascaderOption { Header = "Plain header" };
        AvaloniaList<BindableCascaderOption>? options = new AvaloniaList<BindableCascaderOption> { option };
        CascaderView? cascaderView = new CascaderView
        {
            IsShowEmptyIndicator = false,
            OptionsSource        = options
        };

        var window = CreateWindow(cascaderView);

        try
        {
            options.Remove(option);
            RunDispatcherJobsUntil(() => FindCascaderViewItem(cascaderView, option) is null);

            var optionReference = new WeakReference(option);
            cascaderView.OptionsSource = null;
            option                     = null;
            options                    = null;
            cascaderView               = null;
            return optionReference;
        }
        finally
        {
            window.Content = null;
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateUnownedOptionReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Application header";

        var option = new BindableCascaderOption();
        BindDynamicResource(option, BindableCascaderOption.HeaderProperty, resourceKey, Application.Current);

        option.Header.ShouldBe("Application header");
        return new WeakReference(option);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference CreateRemovedOptionReference(string resourceKey)
    {
        Application.Current!.Resources[resourceKey] = "Application header";

        BindableCascaderOption? option = new BindableCascaderOption();
        BindDynamicResource(option, BindableCascaderOption.HeaderProperty, resourceKey, Application.Current);

        AvaloniaList<BindableCascaderOption>? options = new AvaloniaList<BindableCascaderOption> { option };
        CascaderView? cascaderView = new CascaderView
        {
            IsShowEmptyIndicator = false,
            OptionsSource        = options
        };
        cascaderView.Resources[resourceKey] = "CascaderView header";

        var window = CreateWindow(cascaderView);

        try
        {
            option.Header.ShouldBe("CascaderView header");
            options.Remove(option);
            RunDispatcherJobsUntil(() => FindCascaderViewItem(cascaderView, option) is null);
            option.Header.ShouldBe("Application header");

            var optionReference = new WeakReference(option);
            cascaderView.OptionsSource = null;
            option                     = null;
            options                    = null;
            cascaderView               = null;
            return optionReference;
        }
        finally
        {
            window.Content = null;
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = CreateWindow(content);

        try
        {
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private static AvaloniaWindow CreateWindow(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 320,
            Content = content
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static CascaderViewItem? FindCascaderViewItem(Visual root, ICascaderOption option)
    {
        return root.GetVisualDescendants()
                   .OfType<CascaderViewItem>()
                   .FirstOrDefault(x => ReferenceEquals(x.DataContext, option));
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
            types: [typeof(AvaloniaProperty), typeof(BindingBase), typeof(object)],
            modifiers: null);

        bindMethod.ShouldNotBeNull();

        var extension   = new DynamicResourceExtension(key);
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
            Dispatcher.UIThread.RunJobs();
            if (condition())
            {
                return;
            }
        }
    }

    private sealed class TestCascaderViewLevelList : CascaderViewLevelList
    {
        public void SaveContainerContext(CascaderViewItem item, IDictionary<object, object?> context)
        {
            NotifySaveVirtualizingContext(item, context);
        }

        public void RestoreContainerContext(CascaderViewItem item, IDictionary<object, object?> context)
        {
            NotifyRestoreVirtualizingContext(item, context);
        }

        public void RestoreDefaultContainerContext(CascaderViewItem item, ICascaderOption option)
        {
            NotifyRestoreDefaultContext(item, option);
        }

        public void ClearContainerContext(CascaderViewItem item)
        {
            NotifyClearContainerForVirtualizingContext(item);
        }
    }
}
