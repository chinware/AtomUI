using AtomUI.ColorSystem;
using AtomUI.Media;
using AtomUI.Styling;
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
   
   public PercentPosition PercentPosition
   {
      get => GetValue(PercentPositionProperty);
      set => SetValue(PercentPositionProperty, value);
   }
   
   static ProgressBar()
   {
      AffectsMeasure<ProgressBar>(IndicatorThicknessProperty,
                                  PercentPositionProperty);
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      // TODO 实现有问题
      double targetWidth = 0;
      double targetHeight = 0;
      if (ShowProgressInfo) {
         _percentageLabel!.Measure(availableSize);
         // 其他两个 Icon 都是固定的
      }
      if (Orientation == Orientation.Horizontal) {
         targetHeight = StrokeThickness;
         if (!PercentPosition.IsInner && ShowProgressInfo) {
            if (PercentPosition.Alignment == LinePercentAlignment.Center) {
               targetHeight += _extraInfoSize.Height + _lineExtraInfoMargin;
            }
         }
         
         if (!double.IsInfinity(availableSize.Width)) {
            targetWidth = availableSize.Width;
         } else if (!double.IsNaN(MinWidth)) {
            targetWidth = MinHeight;
         }
         
         targetHeight = Math.Max(targetHeight, MinHeight);
      } else {
         targetWidth = StrokeThickness;
         if (!PercentPosition.IsInner && ShowProgressInfo) {
            if (PercentPosition.Alignment == LinePercentAlignment.Center) {
               targetWidth += _extraInfoSize.Width + _lineExtraInfoMargin;
            }
         }

         targetWidth = Math.Max(targetWidth, MinWidth);
         if (!double.IsInfinity(availableSize.Height)) {
            targetHeight = availableSize.Height;
         } else if (!double.IsNaN(MinHeight)) {
            targetHeight = MinHeight;
         }
      }
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      if (ShowProgressInfo) {
         var extraInfoRect = GetExtraInfoRect(new Rect(new Point(0, 0), DesiredSize));
         if (_percentageLabel!.IsVisible) {
            _percentageLabel.Arrange(extraInfoRect);
         }

         if (_successCompletedIcon != null && _successCompletedIcon.IsVisible) {
            _successCompletedIcon.Arrange(extraInfoRect);
         }
         if (_exceptionCompletedIcon != null && _exceptionCompletedIcon.IsVisible) {
            _exceptionCompletedIcon.Arrange(extraInfoRect);
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
      DrawIndicatorBar(context, deflateValue, IndicatorBarBrush!);
      
      // 绘制成功阈值
      if (!double.IsNaN(SuccessThreshold)) {
         var successThreshold = Math.Clamp(SuccessThreshold, Minimum, Maximum);
         var successThresholdDeflateValue = range * (1 - successThreshold / (Maximum - Minimum));
         DrawIndicatorBar(context, successThresholdDeflateValue, SuccessThresholdBrush!);
      }
   }

   protected void DrawIndicatorBar(DrawingContext context, double deflateValue, IBrush brush)
   {
      Rect indicatorRect = default;
      if (Orientation == Orientation.Horizontal) {
         indicatorRect = _grooveRect.Deflate(new Thickness(0, 0, deflateValue, 0));
      } else {
         indicatorRect = _grooveRect.Deflate(new Thickness(0, 0, 0, deflateValue));
      }
      if (StrokeLineCap == PenLineCap.Round) {
         context.DrawPilledRect(brush, null, indicatorRect, Orientation);
      } else {
         context.FillRectangle(brush, indicatorRect);
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
            if (_extraInfoSize == Size.Infinity) {
               _extraInfoSize = CalculateExtraInfoSize(FontSize);
            }

            if (PercentPosition.IsInner) {
               strokeThickness = _extraInfoSize.Width;
            } else {
               strokeThickness = MinWidth;
            }
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

   protected override void NotifyApplyFixedStyleConfig()
   {
      base.NotifyApplyFixedStyleConfig();
      BindUtils.CreateTokenBinding(this, ColorTextLabelTokenProperty, GlobalResourceKey.ColorTextLabel);
      BindUtils.CreateTokenBinding(this, ColorTextLightSolidTokenProperty, GlobalResourceKey.ColorTextLightSolid);
   }
   
   protected override SizeType CalculateEffectiveSizeType(double size)
   {
      var middleThresholdValue = _sizeTypeThresholdValue[SizeType.Middle];
      var smallThresholdValue = _sizeTypeThresholdValue[SizeType.Small];
      var sizeType = SizeType.Middle;
      if (PercentPosition.IsInner) {
         if (size < smallThresholdValue.InnerStateValue ||
             MathUtils.AreClose(size, smallThresholdValue.InnerStateValue)) {
            sizeType = SizeType.Small;
         } else if (size > smallThresholdValue.InnerStateValue && (size < middleThresholdValue.InnerStateValue ||
                                                                   MathUtils.AreClose(size, middleThresholdValue.InnerStateValue))) {
            sizeType = SizeType.Middle;
         } else {
            sizeType = SizeType.Large;
         }
      } else {
         if (size < smallThresholdValue.NormalStateValue ||
             MathUtils.AreClose(size, smallThresholdValue.NormalStateValue)) {
            sizeType = SizeType.Small;
         } else if (size > smallThresholdValue.NormalStateValue && (size < middleThresholdValue.NormalStateValue ||
                                                                    MathUtils.AreClose(size, middleThresholdValue.NormalStateValue))) {
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
      var contentRect = new Rect(new Point(0, 0), controlRect.Size.Deflate(Margin));
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
                  deflateRight = percentLabelWidth + _lineExtraInfoMargin;
               }
            }
         }
      } else {
         if (ShowProgressInfo) {
            if (!PercentPosition.IsInner) {
               var percentLabelWidth = _extraInfoSize.Width;
               var percentLabelHeight = _extraInfoSize.Height;
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  deflateTop = percentLabelHeight + _lineExtraInfoMargin;
               } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
                  deflateRight = percentLabelWidth;
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  deflateBottom = percentLabelHeight + _lineExtraInfoMargin;;
               }
            }
         }
      }

      var deflatedControlRect = contentRect.Deflate(new Thickness(deflateLeft, deflateTop, deflateRight, deflateBottom));
      if (Orientation == Orientation.Horizontal) {
         return new Rect(new Point(deflatedControlRect.X, (deflatedControlRect.Height - strokeThickness) / 2), new Size(deflatedControlRect.Width, strokeThickness));
      }
      return new Rect(new Point((deflatedControlRect.Width - strokeThickness) / 2, deflatedControlRect.Y), new Size(strokeThickness, deflatedControlRect.Height));
   }

   protected override Rect GetExtraInfoRect(Rect controlRect)
   {
      var contentRect = new Rect(new Point(0, 0), controlRect.Size.Deflate(Margin));
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
               var grooveRect = GetProgressBarRect(contentRect);
               offsetY = grooveRect.Y + (grooveRect.Height - targetHeight) / 2;
               var range = grooveRect.Width;
               var deflateValue = range * (1 - Value / (Maximum - Minimum));
               var indicatorRect = grooveRect.Deflate(new Thickness(0, 0, deflateValue, 0));
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  offsetX = _lineProgressPadding * 2;
               } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
                  offsetX = (indicatorRect.Width - targetWidth) / 2;
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  offsetX = indicatorRect.Right - targetWidth - _lineProgressPadding * 2;
               }
            } else {
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  offsetX = 0;
                  offsetY = (contentRect.Height - targetHeight) / 2;
               } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
                  offsetX = (contentRect.Width - targetWidth) / 2;
                  offsetY = contentRect.Bottom - targetHeight;
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  offsetX = contentRect.Right - targetWidth;
                  offsetY = (contentRect.Height - targetHeight) / 2;
               }
            }
         }
      } else {
         if (PercentPosition.IsInner) {
            var grooveRect = GetProgressBarRect(contentRect);
            offsetX = grooveRect.X + (grooveRect.Width - targetWidth) / 2;
            var range = grooveRect.Height;
            var deflateValue = range * (1 - Value / (Maximum - Minimum));
            var indicatorRect = grooveRect.Deflate(new Thickness(0, 0, 0, deflateValue));
            if (PercentPosition.Alignment == LinePercentAlignment.Start) {
               offsetY = _lineExtraInfoMargin;
            } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
               offsetY = (indicatorRect.Height - targetHeight) / 2;
            } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
               offsetY = indicatorRect.Bottom - targetHeight - _lineExtraInfoMargin;
            }
         } else {
            if (PercentPosition.Alignment == LinePercentAlignment.Start) {
               offsetX = (contentRect.Width - targetWidth) / 2;
               offsetY = 0;
            } else if (PercentPosition.Alignment == LinePercentAlignment.Center) {
               offsetX = contentRect.Right - targetWidth;
               offsetY = (contentRect.Height - targetHeight) / 2;
            } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
               offsetX = (contentRect.Width - targetWidth) / 2;
               offsetY = contentRect.Bottom - targetHeight;
            }
         }
      }

      return new Rect(new Point(offsetX, offsetY), extraInfoSize);
   }
   
   protected override Size CalculateExtraInfoSize(double fontSize)
   {
      if ((Status == ProgressStatus.Exception ||
           MathUtils.AreClose(Value, Maximum)) &&
          !PercentPosition.IsInner) {
         // 只要图标
         if (EffectiveSizeType == SizeType.Large || EffectiveSizeType == SizeType.Middle) {
            return new Size(_lineInfoIconSize, _lineInfoIconSize);
         } 
         return new Size(_lineInfoIconSizeSM, _lineInfoIconSizeSM);
      }
      var textSize = TextUtils.CalculateTextSize(string.Format(ProgressTextFormat, Value), FontFamily, fontSize);
      if (ShowProgressInfo && PercentPosition.IsInner) {
         if (Orientation == Orientation.Vertical) {
            textSize = new Size(textSize.Height, textSize.Width);
         }
      }
      return textSize;
   }
   
   protected override void NotifyEffectSizeTypeChanged()
   {
      base.NotifyEffectSizeTypeChanged();
      CalculateMinBarThickness();
   }

   protected override void NotifyOrientationChanged()
   {
      base.NotifyOrientationChanged();
      CalculateMinBarThickness();
      HandlePercentPositionChanged();
   }

   // TODO 当时 inner 的时候要选中 label 的渲染坐标系
   private void HandlePercentPositionChanged()
   {
      if (ShowProgressInfo) {
         if (Orientation == Orientation.Horizontal) {
            if (!PercentPosition.IsInner) {
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  _percentageLabel!.HorizontalAlignment = HorizontalAlignment.Right;
                  _exceptionCompletedIcon!.HorizontalAlignment = HorizontalAlignment.Right;
                  _successCompletedIcon!.HorizontalAlignment = HorizontalAlignment.Right;
                  _successCompletedIcon!.HorizontalAlignment = HorizontalAlignment.Right;
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  _percentageLabel!.HorizontalAlignment = HorizontalAlignment.Left;
                  _exceptionCompletedIcon!.HorizontalAlignment = HorizontalAlignment.Left;
                  _successCompletedIcon!.HorizontalAlignment = HorizontalAlignment.Left;
               } else {
                  _percentageLabel!.HorizontalAlignment = HorizontalAlignment.Center;
                  _exceptionCompletedIcon!.HorizontalAlignment = HorizontalAlignment.Center;
                  _successCompletedIcon!.HorizontalAlignment = HorizontalAlignment.Center;
               }
            }
         } else {
            if (!PercentPosition.IsInner) {
               if (PercentPosition.Alignment == LinePercentAlignment.Start) {
                  _percentageLabel!.VerticalAlignment = VerticalAlignment.Bottom;
                  _exceptionCompletedIcon!.VerticalAlignment = VerticalAlignment.Bottom;
                  _successCompletedIcon!.VerticalAlignment = VerticalAlignment.Bottom;
               } else if (PercentPosition.Alignment == LinePercentAlignment.End) {
                  _percentageLabel!.VerticalAlignment = VerticalAlignment.Top;
                  _exceptionCompletedIcon!.VerticalAlignment = VerticalAlignment.Top;
                  _successCompletedIcon!.VerticalAlignment = VerticalAlignment.Top;
               } else {
                  _percentageLabel!.VerticalAlignment = VerticalAlignment.Center;
                  _exceptionCompletedIcon!.VerticalAlignment = VerticalAlignment.Center;
                  _successCompletedIcon!.VerticalAlignment = VerticalAlignment.Center;
               }
            }
         }
      }
   }

   protected override void NotifyPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.NotifyPropertyChanged(e);
      if (_initialized) {
         if (e.Property == IndicatorBarBrushProperty) {
            SetupPercentLabelForegroundBrush();
         } else if (e.Property == PercentPositionProperty) {
            HandlePercentPositionChanged();
         } else if (e.Property == PercentPositionProperty) {
            SetupRenderRotate();
         }
      }
   }

   // 需要评估是否需要
   private void CalculateMinBarThickness()
   {
      var thickness = _lineProgressPadding * 2;
      var extraInfoSize = CalculateExtraInfoSize(FontSize);
      if (Orientation == Orientation.Horizontal) {
         thickness += extraInfoSize.Height;
         MinHeight = thickness;
         MinWidth = extraInfoSize.Width;
      } else {
         thickness += extraInfoSize.Width;
         MinWidth = thickness;
         MinHeight = extraInfoSize.Height;
      }
   }

   protected override void NotifyUiStructureReady()
   {
      base.NotifyUiStructureReady();
      SetupPercentLabelForegroundBrush();
      SetupRenderRotate();
   }

   private void SetupRenderRotate()
   {
      if (ShowProgressInfo && PercentPosition.IsInner && Orientation == Orientation.Vertical) {
         _percentageLabel!.RenderTransform = new RotateTransform()
         {
            Angle = 90
         };
         _percentageLabel!.RenderTransformOrigin = RelativePoint.Center;
         _percentageLabel!.Width = _extraInfoSize.Height;
         _percentageLabel!.Height = _extraInfoSize.Width;
      } else {
         _percentageLabel!.Width = double.NaN;
         _percentageLabel!.Height = double.NaN;
         _percentageLabel!.RenderTransform = null;
      }
   }

   private void SetupPercentLabelForegroundBrush()
   {
      if (!PercentPosition.IsInner) {
         _percentageLabel!.Foreground = Foreground;
      } else {
         // 根据当前的 Stroke 笔刷计算可读性
         // 但是渐变笔刷就麻烦了，暂时不支持吧
         var colorTextLabel = (_colorTextLabel as SolidColorBrush)!.Color;
         var colorTextLightSolid = (_colorTextLightSolid as SolidColorBrush)!.Color;
         var colors = new List<Color> { colorTextLabel, colorTextLightSolid };
         if (MathUtils.AreClose(Value, 0)) {
            if (GrooveBrush is ISolidColorBrush grooveBrush) {
               var mostReadable = ColorUtils.MostReadable(grooveBrush.Color, colors);
               if (mostReadable.HasValue) {
                  _percentageLabel!.Foreground = new SolidColorBrush(mostReadable.Value);
               }
            }
         } else {
            if (IndicatorBarBrush is ISolidColorBrush indicatorBarBrush) {
               var mostReadable = ColorUtils.MostReadable(indicatorBarBrush.Color, colors);
               if (mostReadable.HasValue) {
                  _percentageLabel!.Foreground = new SolidColorBrush(mostReadable.Value);
               }
            }
         }
      }
   }

   protected override void NotifyHandleExtraInfoVisibility()
   {
      base.NotifyHandleExtraInfoVisibility();
      if (PercentPosition.IsInner) {
         _exceptionCompletedIcon!.IsVisible = false;
         _successCompletedIcon!.IsVisible = false;
         _percentageLabel!.IsVisible = true;
      }
   }
}