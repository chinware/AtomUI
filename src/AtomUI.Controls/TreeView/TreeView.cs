using AtomUI.Controls.Utils;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaTreeView = Avalonia.Controls.TreeView;

public enum TreeItemHoverMode
{
   Default,
   Block,
   WholeLine
}

[PseudoClasses(DraggablePC)]
public class TreeView : AvaloniaTreeView
{
   public const string DraggablePC = ":draggable";

   #region 公共属性定义

   public static readonly StyledProperty<bool> IsDraggableProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsDraggable));
   
   public static readonly StyledProperty<bool> IsCheckableProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsCheckable), false);
   
   public static readonly StyledProperty<bool> IsShowIconProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowIcon));
   
   public static readonly StyledProperty<bool> IsShowLineProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowLine), false);
   
   public static readonly StyledProperty<TreeItemHoverMode> NodeHoverModeProperty =
      AvaloniaProperty.Register<TreeView, TreeItemHoverMode>(nameof(NodeHoverMode), TreeItemHoverMode.Default);
   
   public static readonly StyledProperty<bool> IsShowLeafSwitcherProperty =
      AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowLeafSwitcher), false);

   public bool IsDraggable
   {
      get => GetValue(IsDraggableProperty);
      set => SetValue(IsDraggableProperty, value);
   }
   
   public bool IsCheckable
   {
      get => GetValue(IsCheckableProperty);
      set => SetValue(IsCheckableProperty, value);
   }
   
   public bool IsShowIcon
   {
      get => GetValue(IsShowIconProperty);
      set => SetValue(IsShowIconProperty, value);
   }
   
   public bool IsShowLine
   {
      get => GetValue(IsShowLineProperty);
      set => SetValue(IsShowLineProperty, value);
   }
   
   public TreeItemHoverMode NodeHoverMode
   {
      get => GetValue(NodeHoverModeProperty);
      set => SetValue(NodeHoverModeProperty, value);
   }
   
   public bool IsShowLeafSwitcher
   {
      get => GetValue(IsShowLeafSwitcherProperty);
      set => SetValue(IsShowLeafSwitcherProperty, value);
   }

   public bool IsDefaultExpandAll { get; set; } = false;

   #endregion

   internal List<TreeViewItem> DefaultCheckedItems { get; set; }

   public TreeView()
   {
      UpdatePseudoClasses();
      DefaultCheckedItems = new List<TreeViewItem>();
   }
   
   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(DraggablePC, IsDraggable);
   }
   
   protected override Control CreateContainerForItemOverride(
      object? item,
      int index,
      object? recycleKey)
   {
      return new TreeViewItem();
   }

   protected override bool NeedsContainerOverride(
      object? item,
      int index,
      out object? recycleKey)
   {
      return NeedsContainer<TreeViewItem>(item, out recycleKey);
   }

   protected override void ContainerForItemPreparedOverride(
      Control container,
      object? item,
      int index)
   {
      base.ContainerForItemPreparedOverride(container, item, index);
      if (container is TreeViewItem treeViewItem) {
         treeViewItem.OwnerTreeView = this;
         BindUtils.RelayBind(this, TreeView.NodeHoverModeProperty, treeViewItem, TreeViewItem.NodeHoverModeProperty);
         BindUtils.RelayBind(this, TreeView.IsShowLineProperty, treeViewItem, TreeViewItem.IsShowLineProperty);
         BindUtils.RelayBind(this, TreeView.IsShowLeafSwitcherProperty, treeViewItem, TreeViewItem.IsShowLeafSwitcherProperty);
         BindUtils.RelayBind(this, TreeView.IsCheckableProperty, treeViewItem, TreeViewItem.IsCheckboxVisibleProperty);
      }
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      if (IsDefaultExpandAll) {
         Dispatcher.UIThread.Post(() =>
         {
            ExpandAll();
         });
      }
      ApplyDefaultChecked();
   }

   public void ExpandAll()
   {
      foreach (var item in Items) {
         if (item is TreeViewItem treeItem) {
            this.ExpandSubTree(treeItem);
         }
      }
   }

   private void ApplyDefaultChecked()
   {
      foreach (var treeItem in DefaultCheckedItems) {
         CheckedSubTree(treeItem);
      }
      DefaultCheckedItems.Clear();
   }
   
   public void CheckedSubTree(TreeViewItem item)
   {
      if (!IsCheckable) {
         return;
      }

      if (!item.IsEnabled || !item.IsCheckable) {
         return;
      }

      item.IsChecked = true;
      if (item.Presenter?.Panel == null && this.GetVisualRoot() is ILayoutRoot visualRoot) {
         var layoutManager = LayoutUtils.GetLayoutManager(visualRoot);
         layoutManager.ExecuteLayoutPass();
      }
      foreach (var childItem in item.Items) {
         if (childItem is TreeViewItem treeViewItem) {
            CheckedSubTree(treeViewItem);
         }
      }

      if (item.Parent is TreeViewItem itemParent) {
         SetupParentNodeCheckedStatus(itemParent);
      }

   }
   
   public void UnCheckedSubTree(TreeViewItem item)
   {
      if (!IsCheckable) {
         return;
      }
      if (!item.IsEnabled || !item.IsCheckable) {
         return;
      }
      item.IsChecked = false;
      if (item.Presenter?.Panel == null && this.GetVisualRoot() is ILayoutRoot visualRoot) {
         var layoutManager = LayoutUtils.GetLayoutManager(visualRoot);
         layoutManager.ExecuteLayoutPass();
      }
      foreach (var childItem in item.Items) {
         if (childItem is TreeViewItem treeViewItem) {
            UnCheckedSubTree(treeViewItem);
         }
      }
      if (item.Parent is TreeViewItem itemParent) {
         SetupParentNodeCheckedStatus(itemParent);
      }
   }

   private void SetupParentNodeCheckedStatus(TreeViewItem parent)
   {
      var isAllChecked = parent.Items.All(item =>
      {
         if (item is TreeViewItem treeViewItem) {
            return treeViewItem.IsChecked.HasValue && treeViewItem.IsChecked.Value;
         }

         return false;
      });
      
      var isAnyChecked = parent.Items.Any(item =>
      {
         if (item is TreeViewItem treeViewItem) {
            return treeViewItem.IsChecked.HasValue && treeViewItem.IsChecked.Value;
         }

         return false;
      });

      if (isAllChecked) {
         parent.IsChecked = true;
      } else if (isAnyChecked) {
         parent.IsChecked = null;
      } else {
         parent.IsChecked = false;
      }
   }
}