using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public record PercentPosition
{
   public bool IsInner { get; set; } = false;
   public LinePercentAlignment Alignment { get; set; } = LinePercentAlignment.End;
}

public partial class ProgressBar : AbstractLineProgress
{
   public static readonly StyledProperty<PercentPosition> PercentPositionProperty =
      AvaloniaProperty.Register<ProgressBar, PercentPosition>(nameof(PercentPosition), new PercentPosition());
   
   public static readonly StyledProperty<double> IndicatorThicknessProperty =
      AvaloniaProperty.Register<ProgressBar, double>(nameof(IndicatorThickness), double.NaN);
   
   public PercentPosition PercentPosition
   {
      get => GetValue(PercentPositionProperty);
      set => SetValue(PercentPositionProperty, value);
   }
   
   public double IndicatorThickness
   {
      get => GetValue(IndicatorThicknessProperty);
      set => SetValue(IndicatorThicknessProperty, value);
   }

   static ProgressBar()
   {
      AffectsMeasure<ProgressBar>(IndicatorThicknessProperty,
                                  PercentPositionProperty);
   }

   private Rect _grooveRect;
   
   protected override Size MeasureOverride(Size availableSize)
   {
      double targetWidth = 0;
      double targetHeight = 0;
      if (ShowProgressInfo) {
         _percentageLabel!.Measure(availableSize);
      }
      if (Orientation == Orientation.Horizontal) {
         targetHeight = StrokeThickness;
         if (!PercentPosition.IsInner && ShowProgressInfo) {
            targetHeight += _percentageLabel!.DesiredSize.Height;
         }

         targetWidth = availableSize.Width;
         targetHeight = Math.Max(targetHeight, MinHeight);
      } else {
         targetWidth = StrokeThickness;
         if (!PercentPosition.IsInner && ShowProgressInfo) {
            targetWidth += _percentageLabel!.DesiredSize.Height;
         }

         targetWidth = Math.Max(targetWidth, MinWidth);
         targetHeight = availableSize.Height;
      }
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      if (ShowProgressInfo) {
         var extraInfoRect = GetExtraInfoRect(new Rect(new Point(0, 0), finalSize));
         if (_percentageLabel!.IsVisible) {
            _percentageLabel.Arrange(extraInfoRect);
         }
      }
      
      return finalSize;
   }

   protected override void RenderGroove(DrawingContext context)
   {
      var controlRect = new Rect(new Point(0, 0), DesiredSize);
      _grooveRect = GetProgressBarRect(controlRect);
      if (StrokeLineCap == PenLineCap.Round) {
         context.DrawPilledRect(GrooveBrush, null, _grooveRect, Orientation);
      } else {
         context.FillRectangle(GrooveBrush!, _grooveRect);
      }
   }

   protected override void RenderIndicatorBar(DrawingContext context)
   {
      var deflateValue = 0d;
      var range = 0d;
      if (Orientation == Orientation.Horizontal) {
         range = _grooveRect.Width;
      } else {
         range = _grooveRect.Height;
      }

      deflateValue = range * (1 - _percentage / 100);
      Rect indicatorRect = default;
      if (Orientation == Orientation.Horizontal) {
         indicatorRect = _grooveRect.Deflate(new Thickness(0, 0, deflateValue, 0));
      } else {
         indicatorRect = _grooveRect.Deflate(new Thickness(0, 0, 0, deflateValue));
      }
      if (StrokeLineCap == PenLineCap.Round) {
         context.DrawPilledRect(IndicatorBarBrush!, null, indicatorRect, Orientation);
      } else {
         context.FillRectangle(IndicatorBarBrush!, indicatorRect);
      }
   }
   
   protected override void CalculateStrokeThickness()
   {
      double strokeThickness;
      if (EffectiveSizeType == SizeType.Large) {
         strokeThickness = LARGE_STROKE_THICKNESS;
      } else if (EffectiveSizeType == SizeType.Middle) {
         strokeThickness = MIDDLE_STROKE_THICKNESS;
      } else {
         strokeThickness = SMALL_STROKE_THICKNESS;
      }

      if (!double.IsNaN(IndicatorThickness)) {
         strokeThickness = IndicatorThickness;
      }

      if (ShowProgressInfo && PercentPosition.IsInner) {
         if (Orientation == Orientation.Horizontal) {
            strokeThickness = MinHeight;
         } else {
            strokeThickness = MinWidth;
         }
      }
      StrokeThickness = strokeThickness;
   }
   
   protected override void NotifySetupUi()
   {
      CalculateSizeTypeThresholdValue();
      CalculateMinBarThickness();
      base.NotifySetupUi();
   }
   
   protected override SizeType CalculateEffectiveSizeType(double size)
   {
      var middleThresholdValue = _sizeTypeThresholdValue[SizeType.Middle];
      var smallThresholdValue = _sizeTypeThresholdValue[SizeType.Small];
      var sizeType = SizeType.Middle;
      if (PercentPosition.IsInner) {
         if (size < smallThresholdValue.InnerStateValue ||
             NumberUtils.FuzzyCompare(size, smallThresholdValue.InnerStateValue)) {
            sizeType = SizeType.Small;
         } else if (size > smallThresholdValue.InnerStateValue && (size < middleThresholdValue.InnerStateValue ||
                                                                   NumberUtils.FuzzyCompare(size, middleThresholdValue.InnerStateValue))) {
            sizeType = SizeType.Middle;
         } else {
            sizeType = SizeType.Large;
         }
      } else {
         if (size < smallThresholdValue.NormalStateValue ||
             NumberUtils.FuzzyCompare(size, smallThresholdValue.NormalStateValue)) {
            sizeType = SizeType.Small;
         } else if (size > smallThresholdValue.NormalStateValue && (size < middleThresholdValue.NormalStateValue ||
                                                                    NumberUtils.FuzzyCompare(size, middleThresholdValue.NormalStateValue))) {
            sizeType = SizeType.Middle;
         } else {
            sizeType = SizeType.Large;
         }
      }
      return sizeType;
   }
   
   protected void CalculateSizeTypeThresholdValue()
   {
      var defaultExtraInfoSize = CalculateExtraInfoSize(_fontSize);
      var smallExtraInfoSize = CalculateExtraInfoSize(_fontSizeSM);
      if (Orientation == Orientation.Horizontal) {
         var largeSizeTypeThresholdValue = new SizeTypeThresholdValue
         {
            InnerStateValue = defaultExtraInfoSize.Height + _lineProgressPadding * 2,
            NormalStateValue = Math.Max(LARGE_STROKE_THICKNESS, defaultExtraInfoSize.Height)
         };
         _sizeTypeThresholdValue.Add(SizeType.Large, largeSizeTypeThresholdValue);
         var middleSizeTypeThresholdValue = new SizeTypeThresholdValue
         {
            InnerStateValue = defaultExtraInfoSize.Height + _lineProgressPadding * 2,
            NormalStateValue = Math.Max(MIDDLE_STROKE_THICKNESS, defaultExtraInfoSize.Height)
         };
         _sizeTypeThresholdValue.Add(SizeType.Middle, middleSizeTypeThresholdValue);
         
         var smallSizeTypeThresholdValue = new SizeTypeThresholdValue
         {
            InnerStateValue = smallExtraInfoSize.Height + _lineProgressPadding * 2,
            NormalStateValue = Math.Max(SMALL_STROKE_THICKNESS, smallExtraInfoSize.Height)
         };
         _sizeTypeThresholdValue.Add(SizeType.Small, smallSizeTypeThresholdValue);
      } else {
         var largeSizeTypeThresholdValue = new SizeTypeThresholdValue
         {
            InnerStateValue = defaultExtraInfoSize.Width + _lineProgressPadding * 2,
            NormalStateValue = Math.Max(LARGE_STROKE_THICKNESS, defaultExtraInfoSize.Width)
         };
         _sizeTypeThresholdValue.Add(SizeType.Large, largeSizeTypeThresholdValue);
         var middleSizeTypeThresholdValue = new SizeTypeThresholdValue
         {
            InnerStateValue = defaultExtraInfoSize.Width + _lineProgressPadding * 2,
            NormalStateValue = Math.Max(MIDDLE_STROKE_THICKNESS, defaultExtraInfoSize.Width)
         };
         _sizeTypeThresholdValue.Add(SizeType.Middle, middleSizeTypeThresholdValue);
         
         var smallSizeTypeThresholdValue = new SizeTypeThresholdValue
         {
            InnerStateValue = smallExtraInfoSize.Width + _lineProgressPadding * 2,
            NormalStateValue = Math.Max(SMALL_STROKE_THICKNESS, smallExtraInfoSize.Width)
         };
         _sizeTypeThresholdValue.Add(SizeType.Small, smallSizeTypeThresholdValue);
      }
   }

   protected override Rect GetProgressBarRect(Rect controlRect)
   {
      double deflateLeft = 0;
      double deflateTop = 0;
      double deflateRight = 0;
      double deflateBottom = 0;
      var strokeThickness = StrokeThickness;
      if (Orientation == Orientation.Horizontal) {
         if (ShowProgressInfo) {
            if (!PercentPosition.IsInner) {
               var percentLabelWidth = _extraInfoSize.Width;
               var percentLabelHeight = _extraInfoSize.Height;
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  deflateLeft = percentLabelWidth + _lineExtraInfoMargin;
               } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
                  deflateBottom = percentLabelHeight;
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  deflateRight = percentLabelWidth + _lineExtraInfoMargin;;
               }
            }
         }
      } else {
         if (ShowProgressInfo) {
            if (!PercentPosition.IsInner) {
               var percentLabelWidth = _extraInfoSize.Width;
               var percentLabelHeight = _extraInfoSize.Height;
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  deflateTop = percentLabelHeight + _lineExtraInfoMargin;;
               } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
                  deflateRight = percentLabelWidth;
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  deflateBottom = percentLabelHeight + _lineExtraInfoMargin;;
               }
            }
         }
      }

      var deflatedControlRect = controlRect.Deflate(new Thickness(deflateLeft, deflateTop, deflateRight, deflateBottom));
      if (Orientation == Orientation.Horizontal) {
         return new Rect(new Point(0, (deflatedControlRect.Height - strokeThickness) / 2), new Size(deflatedControlRect.Width, strokeThickness));
      }
      return new Rect(new Point((deflatedControlRect.Width - strokeThickness) / 2, 0), new Size(strokeThickness, deflatedControlRect.Height));
   }

   protected override Rect GetExtraInfoRect(Rect controlRect)
   {
      double offsetX = 0;
      double offsetY = 0;
      double targetWidth = 0;
      double targetHeight = 0;
      var extraInfoSize = CalculateExtraInfoSize(FontSize);
      if (ShowProgressInfo) {
         targetWidth = extraInfoSize.Width;
         targetHeight = extraInfoSize.Height;
      }
      if (Orientation == Orientation.Horizontal) {
         if (ShowProgressInfo) {
            if (PercentPosition.IsInner) {
               var grooveRect = GetProgressBarRect(controlRect);
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  
               } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
                  
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  
               }
            } else {
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  offsetX = 0;
                  offsetY = (controlRect.Height - targetHeight) / 2;
               } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
                  offsetX = (controlRect.Width - targetWidth) / 2;
                  offsetY = controlRect.Bottom - targetHeight;
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  offsetX = (controlRect.Right - targetWidth);
                  offsetY = (controlRect.Height - targetHeight) / 2;
               }
            }
         }
      }

      return new Rect(new Point(offsetX, offsetY), extraInfoSize);
   }
   
   protected override void NotifyEffectSizeTypeChanged()
   {
      base.NotifyEffectSizeTypeChanged();
      CalculateMinBarThickness();
   }

   // 需要评估是否需要
   private void CalculateMinBarThickness()
   {
      var thickness = _lineProgressPadding * 2;
      var extraInfoSize = CalculateExtraInfoSize(FontSize);
      if (Orientation == Orientation.Horizontal) {
         thickness += extraInfoSize.Height;
         MinHeight = thickness;
      } else {
         thickness += extraInfoSize.Width;
         MinWidth = thickness;
      }
   }
}