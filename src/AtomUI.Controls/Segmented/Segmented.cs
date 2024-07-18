using System.Collections.Specialized;
using AtomUI.Data;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

using SegmentedSizeType = SizeType;
using SegmentedItems = AvaloniaList<Control>;

public enum SegmentedSelectionBehavior
{
   SelectLeftItem,
   SelectRightItem,
   SelectPreviousItem
}

public class SegmentedEventArgs : RoutedEventArgs
{
   public int ItemIndex { get; set; }
   public Control Item { get; set; }
   public Segmented Segmented { get; set; }

   public SegmentedEventArgs(Segmented segmented, Control item, int itemIndex)
   {
      ItemIndex = itemIndex;
      Item = item;
      Segmented = segmented;
   }
}

public class SegmentedMovedEventArgs : RoutedEventArgs
{
   public int From { get; set; }
   public int To { get; set; }
   public Segmented Segmented { get; set; }

   public SegmentedMovedEventArgs(Segmented segmented, int from, int to)
   {
      From = from;
      To = to;
      Segmented = segmented;
   }
}

/// <summary>
/// TODO 现在还没有支持 Tooltip, 还没有做点击事件
/// </summary>
public partial class Segmented : Control, IControlCustomStyle
{
   public static readonly StyledProperty<IBrush?> BackgroundProperty =
      Border.BackgroundProperty.AddOwner<Segmented>();
   
   public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
      Border.CornerRadiusProperty.AddOwner<Segmented>();
   
   public static readonly StyledProperty<CornerRadius> SelectedThumbCornerRadiusProperty =
      Border.CornerRadiusProperty.AddOwner<Segmented>();

   public static readonly StyledProperty<SegmentedSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<Segmented, SegmentedSizeType>(nameof(SizeType), SegmentedSizeType.Middle);

   public static readonly DirectProperty<Segmented, int> CurrentIndexProperty =
      AvaloniaProperty.RegisterDirect<Segmented, int>(
         nameof(CurrentIndex),
         o => o.CurrentIndex,
         (o, v) => o.CurrentIndex = v);

   public static readonly DirectProperty<Segmented, bool> ExpandingProperty =
      AvaloniaProperty.RegisterDirect<Segmented, bool>(
         nameof(Expanding),
         o => o.Expanding,
         (o, v) => o.Expanding = v);

   /// <summary>
   /// 当前选中变化事件
   /// </summary>
   public static readonly RoutedEvent<SegmentedEventArgs> CurrentChangedEvent =
      RoutedEvent.Register<Segmented, SegmentedEventArgs>(
         nameof(Segmented),
         RoutingStrategies.Direct);

   /// <summary>
   /// Item 被单击派发的事件
   /// </summary>
   public static readonly RoutedEvent<SegmentedEventArgs> ItemClickedEvent =
      RoutedEvent.Register<Segmented, SegmentedEventArgs>(
         nameof(Segmented),
         RoutingStrategies.Direct);

   /// <summary>
   /// Item 被双击派发的事件
   /// </summary>
   public static readonly RoutedEvent<SegmentedEventArgs> ItemDoubleClickedEvent =
      RoutedEvent.Register<Segmented, SegmentedEventArgs>(
         nameof(Segmented),
         RoutingStrategies.Direct);

   public static readonly RoutedEvent<SegmentedMovedEventArgs> ItemMovedEvent =
      RoutedEvent.Register<Segmented, SegmentedMovedEventArgs>(
         nameof(Segmented),
         RoutingStrategies.Direct);

   public IBrush? Background
   {
      get => GetValue(BackgroundProperty);
      set => SetValue(BackgroundProperty, value);
   }

   public CornerRadius CornerRadius
   {
      get => GetValue(CornerRadiusProperty);
      set => SetValue(CornerRadiusProperty, value);
   }
   
   public CornerRadius SelectedThumbCornerRadius
   {
      get => GetValue(SelectedThumbCornerRadiusProperty);
      set => SetValue(SelectedThumbCornerRadiusProperty, value);
   }
   
   public SegmentedSizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   private int _currentIndex = 0;

   /// <summary>
   /// 当前选择项
   /// </summary>
   public int CurrentIndex
   {
      get => _currentIndex;
      set
      {
         if (value < 0 || value >= Items.Count) {
            throw new ArgumentOutOfRangeException(nameof(CurrentIndex));
         }

         var oldIndex = _currentIndex;
         if (SetAndRaise(CurrentIndexProperty, ref _currentIndex, value)) {
            var item = ItemBox(_currentIndex);
            if (IsValidIndex(oldIndex)) {
               item.LastItem = oldIndex;
            }

            NotifyCurrentChanged();
            RaiseEvent(new SegmentedEventArgs(this, item.Item, _currentIndex)
            {
               RoutedEvent = CurrentChangedEvent,
               Source = this
            });
         }
      }
   }

   private bool _expanding;

   public bool Expanding
   {
      get => _expanding;
      set => SetAndRaise(ExpandingProperty, ref _expanding, value);
   }

   public SegmentedSelectionBehavior SelectionBehaviorOnRemove { get; set; }

   public event EventHandler<SegmentedEventArgs>? CurrentChanged
   {
      add { AddHandler(CurrentChangedEvent, value); }
      remove { RemoveHandler(CurrentChangedEvent, value); }
   }

   public event EventHandler<SegmentedEventArgs>? ItemClicked
   {
      add { AddHandler(ItemClickedEvent, value); }
      remove { RemoveHandler(ItemClickedEvent, value); }
   }

   public event EventHandler<SegmentedEventArgs>? ItemDoubleClicked
   {
      add { AddHandler(ItemDoubleClickedEvent, value); }
      remove { RemoveHandler(ItemDoubleClickedEvent, value); }
   }

   [Content] public SegmentedItems Items { get; } = new SegmentedItems();
   public int ItemCount => Items.Count;

   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private int _firstVisible = 0;
   private int _lastVisible = -1;

   static Segmented()
   {
      AffectsMeasure<Segmented>(ExpandingProperty, SizeTypeProperty);
      AffectsRender<Segmented>(CornerRadiusProperty, CurrentIndexProperty, SelectedThumbPosProperty,
                               SelectedThumbSizeProperty);
   }

   public Segmented()
   {
      _customStyle = this;
      _customStyle.InitOnConstruct();
      Items.CollectionChanged += HandleItemsChanged;
   }

   public void AddItem(string text)
   {
      AddItem(new SegmentedItem()
      {
         Text = text
      });
   }

   public void AddItem(PathIcon icon)
   {
      AddItem(new SegmentedItem()
      {
         Icon = icon
      });
   }

   public void AddItem(PathIcon icon, string text)
   {
      AddItem(new SegmentedItem()
      {
         Icon = icon,
         Text = text
      });
   }

   public void AddItem(Control item)
   {
      if (Items.Contains(item)) {
         return;
      }

      Items.Add(item);
   }

   public void InsertItem(int index, string text)
   {
      InsertItem(index, new SegmentedItem()
      {
         Text = text
      });
   }

   public void InsertItem(int index, PathIcon icon)
   {
      InsertItem(index, new SegmentedItem()
      {
         Icon = icon
      });
   }

   public void InsertItem(int index, PathIcon icon, string text)
   {
      InsertItem(index, new SegmentedItem()
      {
         Icon = icon,
         Text = text
      });
   }

   public void InsertItem(int index, Control segmentItem)
   {
      if (index > Items.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      if (Items.Contains(segmentItem)) {
         var oldIndex = Items.IndexOf(segmentItem);
         Items.Move(oldIndex, index);
         return;
      }

      Items.Insert(index, segmentItem);
   }

   public bool IsItemEnabled(int index)
   {
      if (index < 0 || index >= Items.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      return ItemBox(index).IsEnabled;
   }

   public void SetItemEnabled(int index, bool enabled)
   {
      if (index < 0 || index >= Items.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      ItemBox(index).IsEnabled = enabled;
      if (!enabled && index == _currentIndex) {
         CurrentIndex = SelectNewCurrentIndexFrom(index + 1);
      } else if (enabled && !IsItemVisible(_currentIndex)) {
         CurrentIndex = SelectNewCurrentIndexFrom(index);
      }
   }

   public bool IsItemVisible(int index)
   {
      if (index < 0 || index >= Items.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      return ItemBox(index).IsVisible;
   }

   public void SetItemVisible(int index, bool visible)
   {
      if (index < 0 || index >= Items.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      ItemBox(index).IsVisible = true;
      CalculateFirstLastVisible(index, visible, false);
      if (!visible && index == _currentIndex) {
         CurrentIndex = SelectNewCurrentIndexFrom(index + 1);
      }
   }

   public Control GetItem(int index)
   {
      if (index < 0 || index >= Items.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      return Items[index];
   }

   public bool TryGetItem(int index, out Control? item)
   {
      if (index < 0 || index >= Items.Count) {
         item = default;
         return false;
      }

      item = Items[index];
      return true;
   }

   public void SetItem(int index, Control item)
   {
      if (index < 0 || index >= Items.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      Items[index] = item;
   }

   public bool TrySetItem(int index, Control item)
   {
      if (index < 0 || index >= Items.Count) {
         return false;
      }

      Items[index] = item;
      return true;
   }

   public void RemoveItem(int index)
   {
      if (index < 0 || index >= Items.Count) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      Items.RemoveAt(index);
   }

   public void RemoveItem(Control item)
   {
      if (!Items.Contains(item)) {
         throw new ArgumentOutOfRangeException(nameof(item));
      }

      Items.Remove(item);
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   private bool IsValidIndex(int index)
   {
      return index >= 0 && index < Items.Count;
   }

   private SegmentedItemBox ItemBox(int index)
   {
      if (!IsValidIndex(index)) {
         throw new ArgumentOutOfRangeException(nameof(index));
      }

      return (LogicalChildren[index] as SegmentedItemBox)!;
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      Transitions = null;
      // 由内置的 Box 布局
      if (!_expanding) {
         return MeasureOverrideNoExpanding(availableSize);
      }

      return MeasureOverrideExpanding(availableSize);
   }

   private Size MeasureOverrideNoExpanding(Size availableSize)
   {
      // 由内置的 Box 布局
      Size layoutSlotSize = availableSize;
      layoutSlotSize = layoutSlotSize.WithWidth(Double.PositiveInfinity);
      bool hasVisibleChild = false;
      double targetWidth = 0d;
      double targetHeight = 0d;
      foreach (var child in LogicalChildren) {
         if (child is SegmentedItemBox box) {
            bool isVisible = box.IsVisible;
            if (isVisible && !hasVisibleChild) {
               hasVisibleChild = true;
            }

            box.Measure(layoutSlotSize);
            Size childDesiredSize = box.DesiredSize;
            targetWidth += childDesiredSize.Width;
            targetHeight = Math.Max(targetHeight, childDesiredSize.Height);
         }
      }

      targetWidth += _trackPadding.Left + _trackPadding.Right;
      targetHeight += _trackPadding.Top + _trackPadding.Bottom;
      return new Size(targetWidth, targetHeight);
   }

   private Size MeasureOverrideExpanding(Size availableSize)
   {
      var maxHeight = 0d;
      var columns = LogicalChildren.Count;
      var availableWidth = availableSize.Width - _trackPadding.Left - _trackPadding.Right;
      var childAvailableSize = new Size(availableWidth / columns, availableSize.Height);
      foreach (var child in LogicalChildren) {
         if (child is SegmentedItemBox box) {
            box.Measure(childAvailableSize);

            if (box.DesiredSize.Height > maxHeight) {
               maxHeight = box.DesiredSize.Height;
            }
         }
      }

      return new Size(availableSize.Width,
                      maxHeight + _trackPadding.Top + _trackPadding.Bottom);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      if (!_expanding) {
         ArrangeOverrideNoExpanding(finalSize);
      } else {
         ArrangeOverrideExpanding(finalSize);
      }

      SetupSelectedThumbRect();
      return finalSize;
   }

   private Size ArrangeOverrideNoExpanding(Size finalSize)
   {
      double previousChildSize = 0.0;
      var offsetX = _trackPadding.Left;
      var offsetY = _trackPadding.Top;
      foreach (var child in LogicalChildren) {
         if (child is SegmentedItemBox box) {
            if (!box.IsVisible) {
               continue;
            }

            previousChildSize = box.DesiredSize.Width;

            box.Arrange(new Rect(new Point(offsetX, offsetY), box.DesiredSize));
            offsetX += previousChildSize;
         }
      }

      return finalSize;
   }

   private Size ArrangeOverrideExpanding(Size finalSize)
   {
      var offsetX = _trackPadding.Left;
      var offsetY = _trackPadding.Top;
      var columns = LogicalChildren.Count;
      var availableWidth = finalSize.Width - _trackPadding.Left - _trackPadding.Right;
      var width = availableWidth / columns;

      var x = 0;

      foreach (var child in LogicalChildren) {
         if (child is SegmentedItemBox box) {
            if (!box.IsVisible) {
               continue;
            }

            box.Arrange(new Rect(x * width + offsetX, offsetY, width, box.DesiredSize.Height));
            x++;
         }
      }

      return finalSize;
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      base.OnPointerReleased(e);
      var targetIndex = -1;
      for (int i = 0; i < LogicalChildren.Count; ++i) {
         if (LogicalChildren[i] is SegmentedItemBox box) {
            if (box.Bounds.Contains(e.GetPosition(this)) && box.IsEnabled) {
               targetIndex = i;
            }
         }
      }

      if (targetIndex != -1) {
         CurrentIndex = targetIndex;
      }
   }

   #region IControlCustomStyle 实现

   void IControlCustomStyle.SetupUi()
   {
      _customStyle.ApplySizeTypeStyleConfig();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      NotifyCurrentChanged();
      ClipToBounds = true;
   }

   void IControlCustomStyle.SetupTransitions()
   {
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<PointTransition>(SelectedThumbPosProperty),
         AnimationUtils.CreateTransition<SizeTransition>(SelectedThumbSizeProperty,
                                                         GlobalResourceKey.MotionDurationFast)
      };
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Large) {
         BindUtils.CreateTokenBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
         BindUtils.CreateTokenBinding(this, SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadius);
      } else if (SizeType == SizeType.Middle) {
         BindUtils.CreateTokenBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadius);
         BindUtils.CreateTokenBinding(this, SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      } else if (SizeType == SizeType.Small) {
         BindUtils.CreateTokenBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
         BindUtils.CreateTokenBinding(this, SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadiusXS);
      }

      ApplyItemSizeConfig();
   }

   private void ApplyItemSizeConfig()
   {
      foreach (var control in LogicalChildren) {
         if (control is SegmentedItemBox box) {
            box.SizeType = SizeType;
         }
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, BackgroundProperty, SegmentedResourceKey.TrackBg);
      BindUtils.CreateTokenBinding(this, TrackPaddingTokenProperty, SegmentedResourceKey.TrackPadding);
      BindUtils.CreateTokenBinding(this, ItemSelectedBgTokenProperty, SegmentedResourceKey.ItemSelectedBg);
      BindUtils.CreateTokenBinding(this, BoxShadowsTertiaryTokenProperty, GlobalResourceKey.BoxShadowsTertiary);
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (_initialized) {
         if (e.Property == SizeTypeProperty) {
            _customStyle.ApplySizeTypeStyleConfig();
         }
      }
   }

   protected void HandleItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
   {
      switch (e.Action) {
         case NotifyCollectionChangedAction.Add:
            var newItems = e.NewItems!.OfType<Control>().ToList();
            var newItemBoxes = newItems.Select(item => new SegmentedItemBox(item));
            var targetAddIndex = e.NewStartingIndex;
            foreach (var box in newItemBoxes) {
               HandleAddItem(targetAddIndex++, box);
            }

            break;

         case NotifyCollectionChangedAction.Move:
            for (int moveIndex = 0; moveIndex < e.OldItems!.Count; ++moveIndex) {
               HandleMoveItem(e.OldStartingIndex + moveIndex, e.NewStartingIndex + moveIndex);
            }

            break;

         case NotifyCollectionChangedAction.Remove:
            var removedItems = e.OldItems!.OfType<Control>().ToHashSet();
            var removeItemBoxes = removedItems.Select(item => new SegmentedItemBox(item));
            var targetRemoveIndex = e.NewStartingIndex;
            foreach (var box in removeItemBoxes) {
               HandleRemoveItem(targetRemoveIndex++, box);
            }

            break;

         case NotifyCollectionChangedAction.Replace:
            for (var i = 0; i < e.OldItems!.Count; ++i) {
               var index = i + e.OldStartingIndex;
               var child = (Control)e.NewItems![i]!;
               AddItemBox(index, new SegmentedItemBox(child));
            }

            break;

         case NotifyCollectionChangedAction.Reset:
            throw new NotSupportedException();
      }

      InvalidateMeasureOnItemsChanged();
   }

   private void HandleAddItem(int index, SegmentedItemBox itemBox)
   {
      AddItemBox(index, itemBox);
      _firstVisible = Math.Max(Math.Min(_currentIndex, _firstVisible), 0);
      if (Items.Count == 1) {
         CurrentIndex = index;
      } else if (index <= _currentIndex) {
         CurrentIndex += 1;
      }

      if (index <= _lastVisible) {
         ++_lastVisible;
      } else {
         _lastVisible = index;
      }

      foreach (var control in LogicalChildren) {
         if (control is SegmentedItemBox box) {
            if (box.LastItem >= index) {
               ++box.LastItem;
            }
         }
      }
   }

   private void HandleRemoveItem(int index, SegmentedItemBox removedBox)
   {
      int newIndex = removedBox.LastItem;
      RemoveItemBox(removedBox);
      foreach (var control in LogicalChildren) {
         if (control is SegmentedItemBox box) {
            if (box.LastItem == index) {
               box.LastItem = -1;
            }

            if (box.LastItem > index) {
               --box.LastItem;
            }
         }
      }

      CalculateFirstLastVisible(index, false, true);
      if (index == _currentIndex) {
         // The current segment item is going away, in order to make sure
         // we emit that "current has changed", we need to reset this
         // around.
         if (LogicalChildren.Count > 0) {
            if (SelectionBehaviorOnRemove == SegmentedSelectionBehavior.SelectPreviousItem) {
               if (newIndex > index) {
                  newIndex--;
               }

               if (!IsValidIndex(newIndex) || !IsItemVisible(newIndex)) {
                  newIndex = Math.Clamp(index, _firstVisible, _lastVisible);
               }
            } else if (SelectionBehaviorOnRemove == SegmentedSelectionBehavior.SelectRightItem) {
               newIndex = Math.Clamp(index, _firstVisible, _lastVisible);
            } else if (SelectionBehaviorOnRemove == SegmentedSelectionBehavior.SelectLeftItem) {
               newIndex = Math.Clamp(index - 1, _firstVisible, _lastVisible);
            }

            if (IsItemVisible(newIndex)) {
               // don't loose newIndex's old through setCurrentIndex
               int bump = ItemBox(newIndex).LastItem;
               CurrentIndex = newIndex;
               ItemBox(newIndex).LastItem = bump;
            } else {
               CurrentIndex = 0;
            }
         } else {
            CurrentIndex = 0;
         }
      } else if (index < _currentIndex) {
         CurrentIndex = _currentIndex - 1;
      }
   }

   private void HandleMoveItem(int from, int to)
   {
      if (from == to ||
          !IsValidIndex(from) ||
          !IsValidIndex(to)) {
         return;
      }

      MoveItemBox(from, to);
      // update lastTab locations
      foreach (var control in LogicalChildren) {
         if (control is SegmentedItemBox box) {
            box.LastItem = CalculateNewPosition(from, to, box.LastItem);
         }
      }

      // update external variables
      CurrentIndex = CalculateNewPosition(from, to, _currentIndex);
      RaiseEvent(new SegmentedMovedEventArgs(this, from, to)
      {
         RoutedEvent = ItemMovedEvent,
         Source = this
      });
   }

   private void CalculateFirstLastVisible(int index, bool visible, bool remove)
   {
      if (visible) {
         _firstVisible = Math.Min(index, _firstVisible);
         _lastVisible = Math.Max(index, _lastVisible);
      } else {
         var items = VisualChildren;
         if (remove || (index == _firstVisible)) {
            _firstVisible = -1;
            for (var i = 0; i < items.Count; ++i) {
               if (items[i].IsVisible) {
                  _firstVisible = i;
                  break;
               }
            }
         }

         if (remove || (index == _lastVisible)) {
            _lastVisible = -1;
            for (var i = items.Count() - 1; i >= 0; --i) {
               if (items[i].IsVisible) {
                  _lastVisible = i;
                  break;
               }
            }
         }
      }
   }

   /// <summary>
   /// Selects the new current index starting at "fromIndex". If "fromIndex" is visible we're done.
   /// Else it tries any index AFTER fromIndex, then any BEFORE fromIndex and, if everything fails,
   /// it returns -1 indicating that no index is available
   /// </summary>
   private int SelectNewCurrentIndexFrom(int fromIndex)
   {
      var newindex = -1;
      var items = VisualChildren;
      for (var i = fromIndex; i < items.Count; ++i) {
         if (items[i] is SegmentedItemBox box) {
            if (box.IsVisible && box.IsEnabled) {
               newindex = i;
               break;
            }
         }
      }

      if (newindex < 0) {
         for (var i = fromIndex - 1; i > -1; --i) {
            if (items[i] is SegmentedItemBox box) {
               if (box.IsVisible && box.IsEnabled) {
                  newindex = i;
                  break;
               }
            }
         }
      }

      return newindex;
   }

   /// <summary>
   /// Given that index at position from moved to position to where return where index goes.
   /// </summary>
   /// <returns></returns>
   private int CalculateNewPosition(int from, int to, int index)
   {
      if (index == from) {
         return to;
      }

      var start = Math.Min(from, to);
      var end = Math.Max(from, to);
      if (index >= start && index <= end) {
         index += (from < to) ? -1 : 1;
      }

      return index;
   }

   private protected virtual void InvalidateMeasureOnItemsChanged()
   {
      InvalidateMeasure();
   }

   private void NotifyCurrentChanged()
   {
      for (int i = 0; i < LogicalChildren.Count; ++i) {
         if (LogicalChildren[i] is SegmentedItemBox box) {
            if (i == _currentIndex) {
               box.IsCurrentItem = true;
            } else {
               box.IsCurrentItem = false;
            }
         }
      }

      if (SelectedThumbSize.Width != 0 && SelectedThumbSize.Height != 0 &&
          Transitions == null) {
         _customStyle.SetupTransitions();
      }

      SetupSelectedThumbRect();
   }

   private void SetupSelectedThumbRect()
   {
      if (VisualRoot is not null) {
         for (int i = 0; i < LogicalChildren.Count; ++i) {
            if (LogicalChildren[i] is SegmentedItemBox box) {
               if (i == _currentIndex) {
                  var offsetX = box.Bounds.X;
                  var offsetY = (DesiredSize.Height - box.DesiredSize.Height) / 2;
                  var targetPos = new Point(offsetX, offsetY);
                  SelectedThumbPos = targetPos;
                  SelectedThumbSize = box.Bounds.Size;
               }
            }
         }
      }
   }

   public sealed override void Render(DrawingContext context)
   {
      context.DrawRectangle(Background, null, new RoundedRect(new Rect(new Point(0, 0), DesiredSize), CornerRadius));
      context.DrawRectangle(_itemSelectedBg, null,
                            new RoundedRect(new Rect(SelectedThumbPos, SelectedThumbSize), SelectedThumbCornerRadius),
                            _boxShadowsTertiary);
   }

   private void AddItemBox(int index, SegmentedItemBox box)
   {
      LogicalChildren.Insert(index, box);
      VisualChildren.Insert(index, box);
   }

   private void RemoveItemBoxAt(int index)
   {
      LogicalChildren.RemoveAt(index);
      VisualChildren.RemoveAt(index);
   }

   private void RemoveItemBox(SegmentedItemBox box)
   {
      LogicalChildren.Remove(box);
      VisualChildren.Remove(box);
   }

   private void MoveItemBox(int from, int to)
   {
      LogicalChildren.Move(from, to);
      VisualChildren.Move(from, to);
   }

   #endregion
}