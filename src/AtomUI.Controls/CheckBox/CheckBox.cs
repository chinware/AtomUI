using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

using AvaloniaCheckBox = Avalonia.Controls.CheckBox;

public partial class CheckBox : AvaloniaCheckBox, ITokenIdProvider, ICustomHitTest
{
   string ITokenIdProvider.TokenId => CheckBoxToken.ID;
   
   public CheckBox()
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
      var targetWidth = size.Width + CheckIndicatorSize + PaddingInline;
      var targetHeight = Math.Max(size.Height, CheckIndicatorSize);
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var arrangeRect = TextRect();

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
      var indicatorRect = IndicatorRect();
      var penWidth = IndicatorBorderThickness.Top;
      PenUtils.TryModifyOrCreate(ref _cachedPen, IndicatorBorderBrush, penWidth);
      var borderRadius = GeometryUtils.CornerRadiusScalarValue(IndicatorBorderRadius);
      context.DrawRectangle(IndicatorBackground, _cachedPen, indicatorRect.Deflate(penWidth / 2), 
         borderRadius, borderRadius);
      if (_styleState.HasFlag(ControlStyleState.On)) {
         var checkMarkGeometry = CommonShapeBuilder.BuildCheckMark(new Size(IndicatorCheckedMarkEffectSize, IndicatorCheckedMarkEffectSize));
         var checkMarkPenWidth = 2;
         var checkMarkPen = new Pen(IndicatorCheckedMarkBrush, 2);
         var checkMarkBounds = checkMarkGeometry.GetRenderBounds(checkMarkPen);
         var deltaSize = (CheckIndicatorSize - checkMarkBounds.Width) / 2;
         var offsetX = deltaSize - checkMarkPenWidth - penWidth;
         var offsetY = deltaSize - checkMarkPenWidth - penWidth;
         checkMarkGeometry.Transform = new TranslateTransform(offsetX, offsetY);
         context.DrawGeometry(null, checkMarkPen, checkMarkGeometry);
      } else if (_styleState.HasFlag(ControlStyleState.Indeterminate)) {
         double deltaSize = (CheckIndicatorSize - IndicatorTristateMarkSize) / 2.0;
         var offsetX = indicatorRect.X + deltaSize;
         var offsetY = indicatorRect.Y + deltaSize;
         var indicatorTristateRect = new Rect(offsetX, offsetY, IndicatorTristateMarkSize, IndicatorTristateMarkSize);
         context.FillRectangle(IndicatorTristateMarkBrush!, indicatorTristateRect);
      }
   }
   
   public bool HitTest(Point point)
   {
      return true;
   }
}