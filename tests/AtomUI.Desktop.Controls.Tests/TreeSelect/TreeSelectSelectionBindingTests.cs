using System.Collections.ObjectModel;
using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.TreeSelectControl;

public class TreeSelectSelectionBindingTests
{
    static TreeSelectSelectionBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SelectedValueProperties_Are_TwoWay_And_DataValidation_Enabled()
    {
        var selectedItemMetadata = Desktop.Controls.TreeSelect.SelectedItemProperty.GetMetadata(
            typeof(Desktop.Controls.TreeSelect));
        var selectedItemsMetadata = Desktop.Controls.TreeSelect.SelectedItemsProperty.GetMetadata(
            typeof(Desktop.Controls.TreeSelect));

        selectedItemMetadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        selectedItemMetadata.EnableDataValidation.ShouldBe(true);
        selectedItemsMetadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        selectedItemsMetadata.EnableDataValidation.ShouldBe(true);
    }

    [Fact]
    public void SelectedItem_DefaultBindingMode_Updates_ViewModel()
    {
        var firstNode = new TreeItemNode
        {
            Header = "First",
            Value  = "first"
        };
        var secondNode = new TreeItemNode
        {
            Header = "Second",
            Value  = "second"
        };
        var viewModel = new TreeSelectBindingViewModel
        {
            SelectedItem = firstNode
        };
        var treeSelect = new Desktop.Controls.TreeSelect
        {
            Width                 = 240,
            IsDefaultExpandAll    = true,
            IsMotionEnabled       = false,
            ItemsSource           = [firstNode, secondNode],
            PlaceholderText       = "Please select"
        };
        treeSelect.Bind(
            Desktop.Controls.TreeSelect.SelectedItemProperty,
            new Binding(nameof(TreeSelectBindingViewModel.SelectedItem))
            {
                Source = viewModel
            });

        ShowInWindow(treeSelect, () =>
        {
            treeSelect.SelectedItem.ShouldBeSameAs(firstNode);

            treeSelect.SelectedItem = secondNode;
            Dispatcher.UIThread.RunJobs();

            viewModel.SelectedItem.ShouldBeSameAs(secondNode);
        });
    }

    [Fact]
    public void SelectedItems_DefaultBindingMode_Updates_ViewModel()
    {
        var firstNode = new TreeItemNode
        {
            Header = "First",
            Value  = "first"
        };
        var secondNode = new TreeItemNode
        {
            Header = "Second",
            Value  = "second"
        };
        var viewModel = new TreeSelectBindingViewModel
        {
            SelectedItems = new List<ITreeItemNode> { firstNode }
        };
        var treeSelect = new Desktop.Controls.TreeSelect
        {
            Width                 = 240,
            IsDefaultExpandAll    = true,
            IsMotionEnabled       = false,
            IsMultiple            = true,
            ItemsSource           = [firstNode, secondNode],
            PlaceholderText       = "Please select"
        };
        treeSelect.Bind(
            Desktop.Controls.TreeSelect.SelectedItemsProperty,
            new Binding(nameof(TreeSelectBindingViewModel.SelectedItems))
            {
                Source = viewModel
            });

        ShowInWindow(treeSelect, () =>
        {
            treeSelect.SelectedItems.ShouldBeSameAs(viewModel.SelectedItems);

            treeSelect.SelectedItems = new List<ITreeItemNode> { firstNode, secondNode };
            Dispatcher.UIThread.RunJobs();

            viewModel.SelectedItems.ShouldNotBeNull();
            viewModel.SelectedItems.ShouldBe([firstNode, secondNode]);
        });
    }

    [Fact]
    public void SelectedItems_ObservableCollection_Mutation_Refreshes_Result_And_TreeView_Selection()
    {
        var firstNode = new TreeItemNode
        {
            Header = "First",
            Value  = "first"
        };
        var secondNode = new TreeItemNode
        {
            Header = "Second",
            Value  = "second"
        };
        var viewModel = new TreeSelectBindingViewModel
        {
            SelectedItems = new ObservableCollection<ITreeItemNode>
            {
                firstNode
            }
        };
        var treeSelect = new Desktop.Controls.TreeSelect
        {
            Width                   = 240,
            IsDefaultExpandAll      = true,
            IsMotionEnabled         = false,
            IsMultiple              = true,
            IsAllowClear            = true,
            IsShowMaxCountIndicator = true,
            MaxCount                = 5,
            ItemsSource             = [firstNode, secondNode],
            PlaceholderText         = "Please select"
        };
        treeSelect.Bind(
            Desktop.Controls.TreeSelect.SelectedItemsProperty,
            new Binding(nameof(TreeSelectBindingViewModel.SelectedItems))
            {
                Source = viewModel,
                Mode   = BindingMode.TwoWay
            });

        ShowInWindow(treeSelect, () =>
        {
            FindSelectTag(treeSelect, "First").ShouldNotBeNull();
            treeSelect.SelectedCount.ShouldBe(1);
            var formValueChangedCount = 0;
            ((IFormItemAware)treeSelect).ValueChanged += (_, _) => formValueChangedCount++;

            treeSelect.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var treeView = GetPopupTreeView(treeSelect);
            treeView.SelectedItems.Cast<ITreeItemNode>().ShouldContain(firstNode);

            viewModel.SelectedItems!.Add(secondNode);
            Dispatcher.UIThread.RunJobs();

            formValueChangedCount.ShouldBe(1);
            treeSelect.EffectiveSelectedItems.ShouldNotBeNull();
            treeSelect.EffectiveSelectedItems.ShouldBe([firstNode, secondNode]);
            treeSelect.SelectedCount.ShouldBe(2);
            FindSelectTag(treeSelect, "Second").ShouldNotBeNull();
            treeView.SelectedItems.Cast<ITreeItemNode>().ShouldContain(secondNode);

            viewModel.SelectedItems.Remove(firstNode);
            Dispatcher.UIThread.RunJobs();

            formValueChangedCount.ShouldBe(2);
            treeSelect.EffectiveSelectedItems.ShouldNotBeNull();
            treeSelect.EffectiveSelectedItems.ShouldBe([secondNode]);
            treeSelect.SelectedCount.ShouldBe(1);
            treeSelect.GetVisualDescendants()
                      .OfType<SelectTag>()
                      .ShouldNotContain(tag => tag.Text == "First");
            treeView.SelectedItems.Cast<ITreeItemNode>().ShouldNotContain(firstNode);

            viewModel.SelectedItems.Clear();
            Dispatcher.UIThread.RunJobs();

            formValueChangedCount.ShouldBe(3);
            treeSelect.EffectiveSelectedItems.ShouldNotBeNull();
            treeSelect.EffectiveSelectedItems.ShouldBeEmpty();
            treeSelect.SelectedCount.ShouldBe(0);
            treeSelect.IsSelectionEmpty.ShouldBeTrue();
            treeView.SelectedItems.Cast<ITreeItemNode>().ShouldBeEmpty();
        });
    }

    [Fact]
    public void SelectedItems_Mutation_Before_First_Popup_Open_Preserves_All_Selected_Items()
    {
        var firstNode = new TreeItemNode
        {
            Header = "First",
            Value  = "first"
        };
        var secondNode = new TreeItemNode
        {
            Header = "Second",
            Value  = "second"
        };
        var viewModel = new TreeSelectBindingViewModel
        {
            SelectedItems = new ObservableCollection<ITreeItemNode>()
        };
        var treeSelect = new Desktop.Controls.TreeSelect
        {
            Width              = 240,
            IsDefaultExpandAll = true,
            IsMotionEnabled    = false,
            IsMultiple         = true,
            IsAllowClear       = true,
            ItemsSource        = [firstNode, secondNode],
            PlaceholderText    = "Please select"
        };
        treeSelect.Bind(
            Desktop.Controls.TreeSelect.SelectedItemsProperty,
            new Binding(nameof(TreeSelectBindingViewModel.SelectedItems))
            {
                Source = viewModel,
                Mode   = BindingMode.TwoWay
            });

        ShowInWindow(treeSelect, () =>
        {
            viewModel.SelectedItems!.Add(firstNode);
            viewModel.SelectedItems.Add(secondNode);
            Dispatcher.UIThread.RunJobs();

            treeSelect.SelectedItems.ShouldBe([firstNode, secondNode]);
            FindSelectTag(treeSelect, "First").ShouldNotBeNull();
            FindSelectTag(treeSelect, "Second").ShouldNotBeNull();

            treeSelect.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            treeSelect.SelectedItems.ShouldBe([firstNode, secondNode]);
            FindSelectTag(treeSelect, "First").ShouldNotBeNull();
            FindSelectTag(treeSelect, "Second").ShouldNotBeNull();
            var treeView = GetPopupTreeView(treeSelect);
            treeView.SelectedItems.Cast<ITreeItemNode>().ShouldContain(firstNode);
            treeView.SelectedItems.Cast<ITreeItemNode>().ShouldContain(secondNode);
        });
    }

    [Fact]
    public void SelectedItems_ObservableCollection_Mutation_Refreshes_Checkable_TreeView()
    {
        var firstNode = new TreeItemNode
        {
            Header = "First",
            Value  = "first"
        };
        var secondNode = new TreeItemNode
        {
            Header = "Second",
            Value  = "second"
        };
        var viewModel = new TreeSelectBindingViewModel
        {
            SelectedItems = new ObservableCollection<ITreeItemNode>
            {
                firstNode
            }
        };
        var treeSelect = new Desktop.Controls.TreeSelect
        {
            Width              = 240,
            IsDefaultExpandAll = true,
            IsMotionEnabled    = false,
            IsTreeCheckable    = true,
            ItemsSource        = [firstNode, secondNode],
            PlaceholderText    = "Please select"
        };
        treeSelect.Bind(
            Desktop.Controls.TreeSelect.SelectedItemsProperty,
            new Binding(nameof(TreeSelectBindingViewModel.SelectedItems))
            {
                Source = viewModel,
                Mode   = BindingMode.TwoWay
            });

        ShowInWindow(treeSelect, () =>
        {
            treeSelect.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var treeView = GetPopupTreeView(treeSelect);
            treeView.CheckedItems.Cast<ITreeItemNode>().ShouldContain(firstNode);

            viewModel.SelectedItems!.Add(secondNode);
            Dispatcher.UIThread.RunJobs();

            treeSelect.EffectiveSelectedItems.ShouldNotBeNull();
            treeSelect.EffectiveSelectedItems.ShouldBe([firstNode, secondNode]);
            FindSelectTag(treeSelect, "Second").ShouldNotBeNull();
            treeView.CheckedItems.Cast<ITreeItemNode>().ShouldContain(secondNode);

            viewModel.SelectedItems.Remove(firstNode);
            Dispatcher.UIThread.RunJobs();

            treeSelect.EffectiveSelectedItems.ShouldNotBeNull();
            treeSelect.EffectiveSelectedItems.ShouldBe([secondNode]);
            treeView.CheckedItems.Cast<ITreeItemNode>().ShouldNotContain(firstNode);
        });
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 420,
            Height = 320
        };
        overlayPanel.Children.Add(content);

        var visualLayerManager = new VisualLayerManager
        {
            Child = overlayPanel
        };
        EnablePopupOverlayLayer(visualLayerManager);

        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 320,
            Content = visualLayerManager
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

    private static Desktop.Controls.TreeView GetPopupTreeView(Desktop.Controls.TreeSelect treeSelect)
    {
        var field = typeof(Desktop.Controls.TreeSelect).GetField(
            "_treeView",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        field.ShouldNotBeNull();
        var treeView = field.GetValue(treeSelect) as Desktop.Controls.TreeView;
        treeView.ShouldNotBeNull();
        return treeView!;
    }

    private static SelectTag? FindSelectTag(Visual root, string text)
    {
        return root.GetVisualDescendants()
                   .OfType<SelectTag>()
                   .FirstOrDefault(tag => tag.Text == text);
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }

    private sealed class TreeSelectBindingViewModel : INotifyPropertyChanged
    {
        private ITreeItemNode? _selectedItem;
        private IList<ITreeItemNode>? _selectedItems;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ITreeItemNode? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (ReferenceEquals(_selectedItem, value))
                {
                    return;
                }
                _selectedItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItem)));
            }
        }

        public IList<ITreeItemNode>? SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (ReferenceEquals(_selectedItems, value))
                {
                    return;
                }
                _selectedItems = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItems)));
            }
        }
    }
}
