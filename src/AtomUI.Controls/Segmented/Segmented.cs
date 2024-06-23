using AtomUI.Data;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
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
public partial class Segmented : StyledControl, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => SegmentedToken.ID;

   public static readonly StyledProperty<SegmentedSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<OptionButtonGroup, SegmentedSizeType>(nameof(SizeType), SegmentedSizeType.Middle);
   
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

   static Segmented()
   {
      AffectsMeasure<Segmented>(ExpandingProperty, SizeTypeProperty);
      AffectsRender<Segmented>(CornerRadiusProperty, CurrentIndexProperty, SelectedThumbPosProperty,
         SelectedThumbSizeProperty);
   }

   public Segmented()
   {
      _tokenResourceBinder = new TokenResourceBinder(this);
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
}