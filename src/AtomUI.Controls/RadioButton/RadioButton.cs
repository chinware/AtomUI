using AtomUI.Data;
using AtomUI.Media;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

public partial class RadioButton : AvaloniaRadioButton, ITokenIdProvider, ICustomHitTest
{
   string ITokenIdProvider.TokenId => RadioButtonToken.ID;

   static RadioButton()
   {
      AffectsRender<RadioButton>(
         RadioBorderBrushProperty,
         RadioInnerBackgroundProperty,
         RadioBackgroundProperty,
         RadioBorderThicknessProperty,
         RadioDotEffectSizeProperty);
   }

   public RadioButton()
   {
      _tokenResourceBinder = new TokenResourceBinder(this);
      _customStyle = this;
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

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width + RadioSize + PaddingInline;
      var targetHeight = Math.Max(size.Height, RadioSize);
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var arrangeRect = RadioTextRect();

      var visualChildren = VisualChildren;
      var visualCount = visualChildren.Count;

      for (var i = 0; i < visualCount; i++) {
         Visual visual = visualChildren[i];
         if (visual is Layoutable layoutable) {
            layoutable.Arrange(arrangeRect);
         }
      }

      return finalSize;
   }

   public sealed override void Render(DrawingContext context)
   {
      var radioRect = RadioRect();
      var penWidth = RadioBorderThickness.Top;
      PenUtils.TryModifyOrCreate(ref _cachedPen, RadioBorderBrush, RadioBorderThickness.Top);
      context.DrawEllipse(RadioBackground, _cachedPen, radioRect.Deflate(penWidth / 2));
      if (IsChecked.HasValue && IsChecked.Value) {
         var dotDiameter = RadioDotEffectSize / 2;
         context.DrawEllipse(RadioInnerBackground, null, radioRect.Center, dotDiameter, dotDiameter);
      }
      
   }

   public bool HitTest(Point point)
   {
      return true;
   }
}