using System.Collections.Specialized;
using AtomUI.Data;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class Segmented : IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private TokenResourceBinder _tokenResourceBinder;
   private int _firstVisible = 0;
   private int _lastVisible = -1;
   
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
         AnimationUtils.CreateTransition<SizeTransition>(SelectedThumbSizeProperty, GlobalResourceKey.MotionDurationFast)
      };
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Large) {
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
         _tokenResourceBinder.AddBinding(this, SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadius);
      } else if (SizeType == SizeType.Middle) {
         _tokenResourceBinder.AddBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadius);
         _tokenResourceBinder.AddBinding(this, SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      } else if (SizeType == SizeType.Small) {
         _tokenResourceBinder.AddBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
         _tokenResourceBinder.AddBinding(this, SelectedThumbCornerRadiusProperty, GlobalResourceKey.BorderRadiusXS);
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
      _tokenResourceBinder.AddBinding(this, BackgroundProperty, SegmentedResourceKey.TrackBg);
      _tokenResourceBinder.AddBinding(this, TrackPaddingTokenProperty, SegmentedResourceKey.TrackPadding);
      _tokenResourceBinder.AddBinding(this, ItemSelectedBgTokenProperty, SegmentedResourceKey.ItemSelectedBg);
      _tokenResourceBinder.AddBinding(this, BoxShadowsTertiaryTokenProperty, GlobalResourceKey.BoxShadowsTertiary);
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
         _lastVisible  = Math.Max(index, _lastVisible);
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
      context.DrawRectangle(_itemSelectedBg, null, new RoundedRect(new Rect(SelectedThumbPos, SelectedThumbSize), SelectedThumbCornerRadius), _boxShadowsTertiary);
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
}