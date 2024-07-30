using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Transformation;

namespace AtomUI.Controls;

public class TabStrip : BaseTabStrip
{
   private Border? _selectedIndicator;

   public TabStrip()
   {
      SelectionChanged += HandleSelectionChanged;
   }

   private void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
   {
      if (VisualRoot is not null) {
         SetupSelectedIndicator();
      }
   }

   private void SetupSelectedIndicator()
   {
      if (_selectedIndicator is not null && SelectedItem is TabStripItem tabStripItem) {
         var selectedBounds = tabStripItem.Bounds;
         var builder = new TransformOperations.Builder(1);
   
         if (TabStripPlacement == Dock.Top) {
            _selectedIndicator.Width = tabStripItem.DesiredSize.Width;
            builder.AppendTranslate(selectedBounds.Left, 0);
         } else if (TabStripPlacement == Dock.Right) {
            _selectedIndicator.Height = tabStripItem.DesiredSize.Height;
         } else if (TabStripPlacement == Dock.Bottom) {
            _selectedIndicator.Width = tabStripItem.DesiredSize.Width;
         } else {
            _selectedIndicator.Height = tabStripItem.DesiredSize.Height;
         }
         _selectedIndicator.RenderTransform = builder.Build();
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      if (SelectedItem is TabStripItem) {
         SetupSelectedIndicator();
      }
      return size;
   }

   protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
   {
      var tabStripItem = new TabStripItem()
      {
         Shape = TabSharp.Line
      };
      return tabStripItem;
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _selectedIndicator = e.NameScope.Find<Border>(TabStripTheme.SelectedItemIndicatorPart);
      if (_selectedIndicator is not null) {
         if (_selectedIndicator.Transitions is null) {
            var transitions = new Transitions();
            transitions.Add(AnimationUtils.CreateTransition<TransformOperationsTransition>(Border.RenderTransformProperty,
                               GlobalResourceKey.MotionDurationSlow, new ExponentialEaseOut()));
            _selectedIndicator.Transitions = transitions;
         }
      }
   }
}