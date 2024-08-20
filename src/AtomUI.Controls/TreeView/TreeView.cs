using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
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

   #region 内部属性定义

   internal static readonly DirectProperty<TreeView, bool> IsDraggingProperty =
      AvaloniaProperty.RegisterDirect<TreeView, bool>(nameof(IsDragging),
                                                      o => o.IsDragging,
                                                      (o, v) => o.IsDragging = v);
   
   internal static readonly DirectProperty<TreeView, DragIndicatorRenderInfo?> DragIndicatorRenderInfoProperty =
      AvaloniaProperty.RegisterDirect<TreeView, DragIndicatorRenderInfo?>(nameof(DragIndicatorRenderInfo),
                                                                          o => o.DragIndicatorRenderInfo,
                                                                          (o, v) => o.DragIndicatorRenderInfo = v);
   
   internal static readonly DirectProperty<TreeView, double> DragIndicatorLineWidthProperty =
      AvaloniaProperty.RegisterDirect<TreeView, double>(nameof(DragIndicatorLineWidth),
                                                        o => o.DragIndicatorLineWidth,
                                                        (o, v) => o.DragIndicatorLineWidth = v);
   
   internal static readonly DirectProperty<TreeView, IBrush?> DragIndicatorBrushProperty =
      AvaloniaProperty.RegisterDirect<TreeView, IBrush?>(nameof(DragIndicatorBrush),
                                                         o => o.DragIndicatorBrush,
                                                         (o, v) => o.DragIndicatorBrush = v);
   
   private DragIndicatorRenderInfo? _dragIndicatorRenderInfo;

   internal DragIndicatorRenderInfo? DragIndicatorRenderInfo
   {
      get => _dragIndicatorRenderInfo;
      set => SetAndRaise(DragIndicatorRenderInfoProperty, ref _dragIndicatorRenderInfo, value);
   }
   
   private bool _isDragging;

   internal bool IsDragging
   {
      get => _isDragging;
      set => SetAndRaise(IsDraggingProperty, ref _isDragging, value);
   }

   private double _dragIndicatorLineWidth;

   internal double DragIndicatorLineWidth
   {
      get => _dragIndicatorLineWidth;
      set => SetAndRaise(DragIndicatorLineWidthProperty, ref _dragIndicatorLineWidth, value);
   }
   
   private IBrush? _dragIndicatorBrush;

   internal IBrush? DragIndicatorBrush
   {
      get => _dragIndicatorBrush;
      set => SetAndRaise(DragIndicatorBrushProperty, ref _dragIndicatorBrush, value);
   }
   
   #endregion

   internal List<TreeViewItem> DefaultCheckedItems { get; set; }
   private Point? _lastPoint;
   private TreeViewItem? _beingDraggedTreeItem;
   private DragPreviewAdorner? _dragPreview;
   private TreeViewItem? _currentDragOver; // 这个不是目标节点，有可能是在父节点上拖动
   private TreeViewItem? _dropTargetNode; // 目标释放节点
   private DropTargetInfo? _dropTargetInfo;

   static TreeView()
   {
      AffectsRender<TreeView>(DragIndicatorRenderInfoProperty);
   }

   public TreeView()
   {
      UpdatePseudoClasses();
      DefaultCheckedItems = new List<TreeViewItem>();
   }
   
   public void ExpandAll()
   {
      foreach (var item in Items) {
         if (item is TreeViewItem treeItem) {
            this.ExpandSubTree(treeItem);
         }
      }
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
         BindUtils.RelayBind(this, TreeView.IsShowIconProperty, treeViewItem, TreeViewItem.IsShowIconProperty);
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

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalResourceBinding(this, DragIndicatorLineWidthProperty, TreeViewTokenResourceKey.DragIndicatorLineWidth);
      TokenResourceBinder.CreateGlobalResourceBinding(this, DragIndicatorBrushProperty, GlobalTokenResourceKey.ColorPrimary);
   }

   private void ApplyDefaultChecked()
   {
      foreach (var treeItem in DefaultCheckedItems) {
         CheckedSubTree(treeItem);
      }
      DefaultCheckedItems.Clear();
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
   
   // 自己优先的查找，用于确认拖动发生的节点
   internal TreeViewItem? GetNodeByPositionSelfFirst(Point position)
   {
      TreeViewItem? result = null;
      for (var i = 0; i < ItemCount; i++) {
         var current = ContainerFromIndex(i);
         if (current is TreeViewItem currentTreeItem && currentTreeItem.IsVisible) {
            result = GetNodeByPositionSelfFirst(position, currentTreeItem);
         }
         if (result is not null) {
            break;
         }
      }

      return result;
   }

   private TreeViewItem? GetNodeByPositionSelfFirst(Point position, TreeViewItem current)
   {
      if (!IsVisibleInViewport(current)) {
         return null;
      }
      var localPosition = this.TranslatePoint(position, current) ?? default;
      TreeViewItem? result = current.IsInDragHeaderBounds(localPosition) ? current : null;

      if (result is not null) {
         return result;
      }
      
      for (var i = 0; i < current.ItemCount; i++) { 
         var child = current.ContainerFromIndex(i);
         if (child is TreeViewItem childItem) {
            result = GetNodeByPositionSelfFirst(position, childItem);
         }
         
         if (result is not null) {
            break;
         }
      }
      
      return result;
   }

   // 孩子优先的查找
   internal TreeViewItem? GetNodeByPosition(Point position)
   {
      TreeViewItem? result = null;
      for (var i = 0; i < ItemCount; i++) {
         var child = ContainerFromIndex(i);
         if (child is TreeViewItem childItem) {
            result = GetNodeByPosition(position, childItem);
         }
         if (result is not null) {
            break;
         }
      }

      return result;
   }

   private TreeViewItem? GetNodeByPosition(Point position, TreeViewItem current)
   {
      TreeViewItem? result = null;
      
      if (!IsVisibleInViewport(current)) {
         return result;
      }
      
      for (var i = 0; i < current.ItemCount; i++) { 
         var child = current.ContainerFromIndex(i);
         if (child is TreeViewItem childItem) {
            result = GetNodeByPosition(position, childItem);
         }
         
         if (result is not null) {
            break;
         }
      }
      var localPosition = this.TranslatePoint(position, current) ?? default;
      result ??= current.IsInDragBounds(localPosition) ? current : null;
      return result;
   }
   
   internal TreeViewItem? GetNodeByOffsetY(Point position)
   {
      TreeViewItem? result = null;
      for (var i = 0; i < ItemCount; i++) {
         var child = ContainerFromIndex(i);
         if (child is TreeViewItem childItem) {
            result = GetNodeByOffsetY(position, childItem);
         }
         if (result is not null) {
            break;
         }
      }

      return result;
   }

   private TreeViewItem? GetNodeByOffsetY(Point position, TreeViewItem current)
   {
      TreeViewItem? result = null;

      if (!IsVisibleInViewport(current)) {
         return result;
      }
      
      for (var i = 0; i < current.ItemCount; i++) { 
         var child = current.ContainerFromIndex(i);
         if (child is TreeViewItem childItem) {
            result = GetNodeByOffsetY(position, childItem);
         }
         
         if (result is not null) {
            break;
         }
      }
      
      var localPosition = this.TranslatePoint(position, current) ?? default;
      result ??= current.IsDragOverForOffsetY(localPosition) ? current : null;
      return result;
   }
   
   #region 拖动相关处理
   
   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      base.OnPointerPressed(e);
      if (IsDraggable) {
         e.Handled = true;
         _lastPoint = e.GetPosition(this);
         e.PreventGestureRecognition();
      }
   }

   protected override void OnPointerMoved(PointerEventArgs e)
   {
      if (_lastPoint.HasValue) {
         var delta = e.GetPosition(this) - _lastPoint.Value;
         var manhattanDistance = Math.Abs(delta.X) + Math.Abs(delta.Y);
         // 先写死
         if (manhattanDistance > 5) {
            if (!IsDragging) {
               HandlePrepareDrag();
               IsDragging = true;
            }
            HandleDragging(e.GetPosition(this), delta);
         }
      }
   }

   protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
   {
      if (_lastPoint.HasValue) {
         HandleDragCompleted(_lastPoint.Value);
         _lastPoint = null;
         IsDragging = false;
      }

      base.OnPointerCaptureLost(e);
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      base.OnPointerReleased(e);
      if (_lastPoint.HasValue) {
         e.Handled = true;
         HandleDragCompleted(e.GetPosition(this));
         _lastPoint = null;
         IsDragging = false;
      }
   }
   
   private void HandlePrepareDrag()
   {
      _beingDraggedTreeItem = GetNodeByPositionSelfFirst(_lastPoint!.Value);
      if (_beingDraggedTreeItem is not null) {
         Dispatcher.UIThread.Post(() => { _beingDraggedTreeItem.IsDragging = true; });
      }
      var adornerLayer = AdornerLayer.GetAdornerLayer(this);
      if (adornerLayer == null || _beingDraggedTreeItem is null) {
         return;
      }
      
      _dragPreview = _beingDraggedTreeItem.BuildPreviewAdorner();
      AdornerLayer.SetAdornedElement(_dragPreview, TopLevel.GetTopLevel(this));
      AdornerLayer.SetIsClipEnabled(_dragPreview, false);
      adornerLayer.Children.Add(_dragPreview);
   }
   
   private void HandleDragging(Point position, Point delta)
   {
      if (_dragPreview is not null && _beingDraggedTreeItem is not null) {
         var basePosition = _beingDraggedTreeItem.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!) ?? default;
         _dragPreview.OffsetX = basePosition.X + delta.X;
         _dragPreview.OffsetY = basePosition.Y + delta.Y;
         SetupDragOver(position);
         if (_currentDragOver is not null) {
            _dropTargetNode = GetNodeByOffsetY(position);
         }

         SetupDragIndicatorRenderInfo(in position);
      }
   }

   // 正常一个有效的拖动，应该 _currentDragOver 不为空
   private void SetupDragIndicatorRenderInfo(in Point position)
   {
      if (_currentDragOver is null || 
          _currentDragOver == _beingDraggedTreeItem || 
          _dropTargetNode == _beingDraggedTreeItem) {
         _dropTargetInfo = null;
         DragIndicatorRenderInfo = null;
         return;
      }

      var effectiveDropTarget = _currentDragOver;
      if (_dropTargetNode is not null) {
         effectiveDropTarget = _dropTargetNode;
      }
      
      _dropTargetInfo = new DropTargetInfo();
      _dropTargetInfo.IsRoot = false;

      var effectiveDragHeaderLocalBounds = effectiveDropTarget.GetDragBounds();
      var effectiveDragHeaderLocalOffset = effectiveDragHeaderLocalBounds.Position;
      var effectiveDragHeaderBounds = new Rect(effectiveDropTarget.TranslatePoint(effectiveDragHeaderLocalOffset, this) ?? effectiveDragHeaderLocalOffset, 
                                               effectiveDragHeaderLocalBounds.Size);
      
      var effectiveDragHeaderOffset = effectiveDragHeaderBounds.Position;
      var dropTargetHalfOffsetY = effectiveDragHeaderOffset.Y + effectiveDragHeaderBounds.Height / 2;
      var offsetY = position.Y;
      Point startPoint = default;
      Point endPoint = default;
      var offsetYDelta = effectiveDropTarget.FrameDecoratorMargin().Bottom + DragIndicatorLineWidth / 3;

      var minOffsetY = DragIndicatorLineWidth / 2;
      var maxOffsetY = Bounds.Height - DragIndicatorLineWidth / 2;
      
      if (effectiveDropTarget != _currentDragOver) {
         // 这种情况父节点不可能为空
         var effectiveIndex = 0;
         if (effectiveDropTarget.Parent is TreeViewItem parentTreeItem) {
            effectiveIndex = parentTreeItem.IndexFromContainer(effectiveDropTarget);
            _dropTargetInfo.TargetTreeItem = parentTreeItem;
         }
         
         if (offsetY > dropTargetHalfOffsetY) {
            startPoint = effectiveDragHeaderBounds.BottomLeft;
            endPoint = effectiveDragHeaderBounds.BottomRight;
            startPoint = startPoint.WithY(Math.Min(startPoint.Y + offsetYDelta, maxOffsetY));
            endPoint = endPoint.WithY(Math.Min(endPoint.Y + offsetYDelta, maxOffsetY));
            _dropTargetInfo.Index = effectiveIndex + 1;
         } else {
            startPoint = effectiveDragHeaderBounds.TopLeft;
            endPoint = effectiveDragHeaderBounds.TopRight;
            startPoint = startPoint.WithY(Math.Max(startPoint.Y - offsetYDelta, minOffsetY));
            endPoint = endPoint.WithY(Math.Max(endPoint.Y - offsetYDelta, minOffsetY));
            _dropTargetInfo.Index = effectiveIndex;
         }
         
      } else {
         var effectiveIndex = 0;
         if (effectiveDropTarget.Parent is TreeViewItem parentItem) {
            effectiveIndex = parentItem.IndexFromContainer(effectiveDropTarget);
         } else if (effectiveDropTarget.Level == 0) {
            effectiveIndex = IndexFromContainer(effectiveDropTarget);
         }
         
         if (effectiveDropTarget.Parent is TreeViewItem parentTreeItem) {
            _dropTargetInfo.TargetTreeItem = parentTreeItem;
         }
         _dropTargetInfo.IsRoot = effectiveDropTarget.Level == 0;
         
         // TODO 看看是否要固化一下
         var fixedOffset = 25;
         var range = 1.5d;

         if (MathUtils.LessThan(offsetY, dropTargetHalfOffsetY - range)) {
            _dropTargetInfo.Index = effectiveIndex;
            startPoint = effectiveDragHeaderBounds.TopLeft;
            endPoint = effectiveDragHeaderBounds.TopRight;
            startPoint = startPoint.WithY(Math.Max(startPoint.Y - offsetYDelta, minOffsetY));
            endPoint = endPoint.WithY(Math.Max(endPoint.Y - offsetYDelta, minOffsetY));
         } else if (MathUtils.GreaterThan(offsetY, dropTargetHalfOffsetY - range) && 
                    MathUtils.LessThan(offsetY, dropTargetHalfOffsetY + range)) {
            startPoint = effectiveDragHeaderBounds.BottomLeft.WithX(effectiveDragHeaderBounds.Left + fixedOffset);
            endPoint = effectiveDragHeaderBounds.BottomRight;
            startPoint = startPoint.WithY(Math.Min(startPoint.Y + offsetYDelta, maxOffsetY));
            endPoint = endPoint.WithY(Math.Min(endPoint.Y + offsetYDelta, maxOffsetY));
            _dropTargetInfo.Index = effectiveDropTarget.ItemCount;
            _dropTargetInfo.TargetTreeItem = effectiveDropTarget;
            _dropTargetInfo.IsRoot = false;
         } else {
            _dropTargetInfo.Index = effectiveIndex + 1;
            startPoint = effectiveDragHeaderBounds.BottomLeft;
            endPoint = effectiveDragHeaderBounds.BottomRight;
            startPoint = startPoint.WithY(Math.Max(startPoint.Y + offsetYDelta, minOffsetY));
            endPoint = endPoint.WithY(Math.Max(endPoint.Y + offsetYDelta, minOffsetY));
         }
      }
      
      DragIndicatorRenderInfo = new DragIndicatorRenderInfo()
      {
         TargetTreeItem = effectiveDropTarget,
         StartPoint = startPoint,
         EndPoint = endPoint,
      };
   }
   
   private void SetupDragOver(Point position)
   {
      var treeViewItem = GetNodeByPosition(position);
      if (_currentDragOver is not null) {
         if (_currentDragOver != treeViewItem) {
            _currentDragOver.IsDragOver = false;
         } else {
            _currentDragOver.IsDragOver = true;
            return;
         }
      }
   
      if (treeViewItem is not null) {
         _currentDragOver = treeViewItem;
         _currentDragOver.IsDragOver = true;
      } else {
         _currentDragOver = null;
      }
   }
   
   private void HandleDragCompleted(Point point)
   {
      PerformDropOperation();
      if (_dragPreview is not null) {
         AdornerLayer layer = AdornerLayer.GetAdornerLayer(this)!;
         layer.Children.Remove(_dragPreview);
         if (_currentDragOver is not null) {
            _currentDragOver.IsDragOver = false;
            _currentDragOver = null;
         }
         
         _dropTargetNode = null;
      }
      if (_beingDraggedTreeItem is not null) {
         _beingDraggedTreeItem.IsDragging = false;
         _beingDraggedTreeItem = null;
      }

      DragIndicatorRenderInfo = null;
      _dropTargetInfo = null;
   }

   private void PerformDropOperation()
   {
      if (_dropTargetInfo is null || _beingDraggedTreeItem is null) {
         return;
      }

      object? sourceItem = default;
      var beingDraggedTreeItemParent = _beingDraggedTreeItem.Parent;
      var sourceIsRoot = false;
      if (_beingDraggedTreeItem.Parent is TreeViewItem parentItem) {
          sourceItem = parentItem.ItemFromContainer(_beingDraggedTreeItem);
         if (sourceItem is not null) {
            parentItem.Items.Remove(sourceItem);
         }
      } else if (_beingDraggedTreeItem.Level == 0) { 
         sourceItem = ItemFromContainer(_beingDraggedTreeItem);
         sourceIsRoot = true;
         if (sourceItem is not null) {
            Items.Remove(sourceItem);
         }
      }

      if (_dropTargetInfo.IsRoot) {
         var indexDelta = 0;
         if (sourceIsRoot) {
            indexDelta = 1;
         }
         Items.Insert(Math.Max(_dropTargetInfo.Index - indexDelta, 0), sourceItem);
      } else if (_dropTargetInfo.TargetTreeItem is not null) {
         var indexDelta = 0;
         if (_dropTargetInfo.TargetTreeItem == beingDraggedTreeItemParent) {
            indexDelta = 1;
         }
         _dropTargetInfo.TargetTreeItem.Items.Insert(Math.Max(_dropTargetInfo.Index - indexDelta, 0), sourceItem);
      }
   }

   private bool IsVisibleInViewport(TreeViewItem item)
   {
      // 先判断是否展开
      if (item.Level > 0) {
         var isExpaned = true;
         TreeViewItem? currentItem = item.Parent as TreeViewItem;
         while (currentItem is not null) {
            if (!currentItem.IsExpanded) {
               isExpaned = false;
               break;
            }
            currentItem = currentItem.Parent as TreeViewItem;
         }

         if (!isExpaned) {
            return false;
         }
      }
      
      var dragBounds = item.GetDragBounds();
      var offset = item.TranslatePoint(dragBounds.Position, this) ?? default;
      var targetBounds = new Rect(offset, dragBounds.Size);
      return new Rect(Bounds.Size).Contains(targetBounds);
   }
   
   #endregion
   
   public override void Render(DrawingContext context)
   {
      if (IsDragging && _dragIndicatorRenderInfo is not null) {
         var pen = new Pen(DragIndicatorBrush, DragIndicatorLineWidth);
         {
            using var state = context.PushRenderOptions(new RenderOptions
            {
               EdgeMode = EdgeMode.Aliased
            });
            context.DrawLine(pen, _dragIndicatorRenderInfo.StartPoint, _dragIndicatorRenderInfo.EndPoint);
         }
      }
   }
}

internal class DropTargetInfo
{
   public TreeViewItem? TargetTreeItem { get; set; }
   public int Index { get; set; }
   public bool IsRoot { get; set; }
}

internal record DragIndicatorRenderInfo
{
   public TreeViewItem? TargetTreeItem { get; set; }
   public Point StartPoint { get; set; }
   public Point EndPoint { get; set; }
}