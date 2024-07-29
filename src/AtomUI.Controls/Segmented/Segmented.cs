using System.Collections.Specialized;
using AtomUI.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
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
public partial class Segmented : TemplatedControl, IControlCustomStyle
{
   #region 公共属性定义

   public static readonly StyledProperty<CornerRadius> SelectedThumbCornerRadiusProperty =
      AvaloniaProperty.Register<Segmented, CornerRadius>(nameof(SelectedThumbCornerRadius));

   public static readonly StyledProperty<SegmentedSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<Segmented, SegmentedSizeType>(nameof(SizeType), SegmentedSizeType.Middle);

   public static readonly DirectProperty<Segmented, int> CurrentIndexProperty =
      AvaloniaProperty.RegisterDirect<Segmented, int>(
         nameof(CurrentIndex),
         o => o.CurrentIndex,
         (o, v) => o.CurrentIndex = v);

   public static readonly StyledProperty<bool> IsExpandingProperty =
      AvaloniaProperty.Register<Segmented, bool>(nameof(IsExpanding));

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

   #endregion

   #region 内部属性定义

   internal static readonly StyledProperty<IBrush?> SelectedThumbBgProperty =
      AvaloniaProperty.Register<Segmented, IBrush?>(
         nameof(SelectedThumbBg));
   
   internal static readonly StyledProperty<BoxShadows> SelectedThumbBoxShadowsProperty =
      AvaloniaProperty.Register<Segmented, BoxShadows>(
         nameof(SelectedThumbBoxShadows));
   
   internal IBrush? SelectedThumbBg
   {
      get => GetValue(SelectedThumbBgProperty);
      set => SetValue(SelectedThumbBgProperty, value);
   }
   
   internal BoxShadows SelectedThumbBoxShadows
   {
      get => GetValue(SelectedThumbBoxShadowsProperty);
      set => SetValue(SelectedThumbBoxShadowsProperty, value);
   }
   
   // 内部动画属性
   internal static readonly StyledProperty<Size> SelectedThumbSizeProperty =
      AvaloniaProperty.Register<Segmented, Size>(nameof(SelectedThumbSize));

   internal Size SelectedThumbSize
   {
      get => GetValue(SelectedThumbSizeProperty);
      set => SetValue(SelectedThumbSizeProperty, value);
   }
   
   internal static readonly StyledProperty<Point> SelectedThumbPosProperty =
      AvaloniaProperty.Register<Segmented, Point>(nameof(SelectedThumbPos));

   internal Point SelectedThumbPos
   {
      get => GetValue(SelectedThumbPosProperty);
      set => SetValue(SelectedThumbPosProperty, value);
   }

   #endregion
   
   private int _currentIndex = 0;
   private Canvas? _mainContainer;

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

   public bool IsExpanding
   {
      get => GetValue(IsExpandingProperty);
      set => SetValue(IsExpandingProperty, value);
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
   
   private IControlCustomStyle _customStyle;
   private int _firstVisible = 0;
   private int _lastVisible = -1;

   static Segmented()
   {
      AffectsMeasure<Segmented>(IsExpandingProperty, SizeTypeProperty);
      AffectsRender<Segmented>(CornerRadiusProperty, CurrentIndexProperty, SelectedThumbPosProperty,
                               SelectedThumbSizeProperty);
   }

   public Segmented()
   {
      _customStyle = this;
      _customStyle.InitOnConstruct();
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

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _mainContainer = scope.Find<Canvas>(SegmentedTheme.MainContainerPart);
      ClipToBounds = true;
      Items.CollectionChanged += HandleItemsChanged;
      
      // 首次初始化
      for (var i = 0; i < Items.Count; ++i) {
         HandleAddItem(i, new SegmentedItemBox(Items[i])
         {
            HorizontalAlignment = HorizontalAlignment.Stretch
         });
      }
      NotifyCurrentChanged();
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
      
      return (_mainContainer?.Children[index] as SegmentedItemBox)!;
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      if (!IsExpanding) {
         return MeasureOverrideNoExpanding(availableSize);
      }
      return MeasureOverrideExpanding(availableSize);
   }
   
   private Size MeasureOverrideNoExpanding(Size availableSize)
   {
      Size layoutSlotSize = availableSize;
      layoutSlotSize = layoutSlotSize.WithWidth(Double.PositiveInfinity);
      bool hasVisibleChild = false;
      double targetWidth = 0d;
      double targetHeight = 0d;
      foreach (var child in _mainContainer!.Children) {
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
   
      targetWidth += Padding.Left + Padding.Right;
      targetHeight += Padding.Top + Padding.Bottom;
      return new Size(targetWidth, targetHeight);
   }
   
   private Size MeasureOverrideExpanding(Size availableSize)
   {
      var maxHeight = 0d;
      var columns = _mainContainer!.Children.Count;
      var availableWidth = availableSize.Width - Padding.Left - Padding.Right;
      var childAvailableSize = new Size(availableWidth / columns, availableSize.Height);
      foreach (var child in _mainContainer!.Children) {
         if (child is SegmentedItemBox box) {
            box.Measure(childAvailableSize);
            if (box.DesiredSize.Height > maxHeight) {
               maxHeight = box.DesiredSize.Height;
            }
         }
      }
   
      return new Size(availableSize.Width, maxHeight + Padding.Top + Padding.Bottom);
   }
   
   protected override Size ArrangeOverride(Size finalSize)
   {
      if (!IsExpanding) {
         ArrangeOverrideNoExpanding(finalSize);
      } else {
         ArrangeOverrideExpanding(finalSize);
      }
      if (Transitions is null) {
         SetupSelectedThumbRect();
         _customStyle.SetupTransitions();
      }
      return finalSize;
   }

   private Size ArrangeOverrideNoExpanding(Size finalSize)
   {
      double previousChildSize = 0.0;
      var offsetX = Padding.Left;
      var offsetY = Padding.Top;
      foreach (var child in _mainContainer!.Children) {
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
      var offsetX = Padding.Left;
      var offsetY = Padding.Top;
      var columns = _mainContainer!.Children.Count;
      var availableWidth = finalSize.Width - Padding.Left - Padding.Right;
      var width = availableWidth / columns;

      var x = 0;

      foreach (var child in _mainContainer!.Children) {
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
      for (int i = 0; i < _mainContainer!.Children.Count; ++i) {
         if (_mainContainer!.Children[i] is SegmentedItemBox box) {
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

   void IControlCustomStyle.SetupTransitions()
   {
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<PointTransition>(SelectedThumbPosProperty),
         AnimationUtils.CreateTransition<SizeTransition>(SelectedThumbSizeProperty, GlobalResourceKey.MotionDurationFast)
      };
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
      if (_mainContainer?.Children.Count == 1) {
         CurrentIndex = index;
      } else if (index <= _currentIndex) {
         CurrentIndex += 1;
      }

      if (index <= _lastVisible) {
         ++_lastVisible;
      } else {
         _lastVisible = index;
      }

      if (_mainContainer is not null) {
         foreach (var control in _mainContainer.Children) {
            if (control is SegmentedItemBox box) {
               if (box.LastItem >= index) {
                  ++box.LastItem;
               }
            }
         }
      }
   }

   private void HandleRemoveItem(int index, SegmentedItemBox removedBox)
   {
      int newIndex = removedBox.LastItem;
      RemoveItemBox(removedBox);
      foreach (var control in _mainContainer!.Children) {
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
         if (_mainContainer!.Children.Count > 0) {
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
      foreach (var control in _mainContainer!.Children) {
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
         var items = _mainContainer!.Children;
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
      var items = _mainContainer!.Children;
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
      for (int i = 0; i < _mainContainer!.Children.Count; ++i) {
         if (_mainContainer!.Children[i] is SegmentedItemBox box) {
            if (i == _currentIndex) {
               box.IsCurrentItem = true;
            } else {
               box.IsCurrentItem = false;
            }
         }
      }
      SetupSelectedThumbRect();
   }

   private void SetupSelectedThumbRect()
   {
      if (VisualRoot is not null) {
         for (int i = 0; i < _mainContainer!.Children.Count; ++i) {
            if (_mainContainer!.Children[i] is SegmentedItemBox box) {
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
      context.DrawRectangle(Background, null, new RoundedRect(new Rect(new Point(0, 0), Bounds.Size), CornerRadius));
      context.DrawRectangle(SelectedThumbBg, null,
                            new RoundedRect(new Rect(SelectedThumbPos, SelectedThumbSize), SelectedThumbCornerRadius),
                            SelectedThumbBoxShadows);
   }

   private void AddItemBox(int index, SegmentedItemBox box)
   {
      if (_mainContainer is not null) {
         _mainContainer.Children.Insert(index, box);
         // TODO 这个在移除的时候是否需要释放？
         BindUtils.RelayBind(this, SizeTypeProperty, box, SegmentedItemBox.SizeTypeProperty);
         BindUtils.RelayBind(this, SelectedThumbCornerRadiusProperty, box, SegmentedItemBox.CornerRadiusProperty);
      }
   }

   private void RemoveItemBoxAt(int index)
   {
      if (_mainContainer is not null) {
         _mainContainer.Children.RemoveAt(index);
      }
   }

   private void RemoveItemBox(SegmentedItemBox box)
   {
      if (_mainContainer is not null) {
         _mainContainer.Children.Remove(box);
      }
   }

   private void MoveItemBox(int from, int to)
   {
      if (_mainContainer is not null) {
         _mainContainer.Children.Move(from, to);
      }
   }

   #endregion
}