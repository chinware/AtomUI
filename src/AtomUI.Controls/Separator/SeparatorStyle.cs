using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Math = System.Math;

namespace AtomUI.Controls;

public partial class Separator : IControlCustomStyle
{
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private bool _initialized = false;
   private bool _scalingAwareConfigApplied = false;
   private Label? _titleLabel;
   private const double SEPARATOR_LINE_MIN_PROPORTION = 0.25;
   private double _currentEdgeDistance;

   void IControlCustomStyle.SetupUi()
   {
      _titleLabel = new Label
      {
         Content = Title,
         HorizontalAlignment = HorizontalAlignment.Left,
         VerticalAlignment = VerticalAlignment.Center,
         HorizontalContentAlignment = HorizontalAlignment.Center,
         VerticalContentAlignment = VerticalAlignment.Center,
         Padding = new Thickness(0)
      };
      
      LogicalChildren.Add(_titleLabel);
      VisualChildren.Add(_titleLabel);
      
      _customStyle.ApplyFixedStyleConfig();
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      HandleForOrientation();
      _controlTokenBinder.AddControlBinding(TitleColorProperty, GlobalResourceKey.ColorText);
      _controlTokenBinder.AddControlBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      _controlTokenBinder.AddControlBinding(LineColorProperty, GlobalResourceKey.ColorSplit);
      
      _controlTokenBinder.AddControlBinding(TextPaddingInlineTokenProperty, SeparatorResourceKey.TextPaddingInline);
      _controlTokenBinder.AddControlBinding(OrientationMarginPercentTokenProperty, SeparatorResourceKey.OrientationMarginPercent);
      _controlTokenBinder.AddControlBinding(VerticalMarginInlineTokenProperty, SeparatorResourceKey.VerticalMarginInline);

      if (_titleLabel is not null) {
         BindUtils.RelayBind(this, FontSizeProperty, _titleLabel);
         BindUtils.RelayBind(this, TitleColorProperty, _titleLabel, ForegroundProperty);
         BindUtils.RelayBind(this, FontStyleProperty, _titleLabel, FontStyleProperty);
         BindUtils.RelayBind(this, FontWeightProperty, _titleLabel, FontWeightProperty);
      }
   }

   private void HandleForOrientation()
   {
      if (Orientation == Orientation.Horizontal) {
         HorizontalAlignment = HorizontalAlignment.Stretch;
         VerticalAlignment = VerticalAlignment.Center;
         _titleLabel!.IsVisible = true;
      } else {
         HorizontalAlignment = HorizontalAlignment.Center;
         VerticalAlignment = VerticalAlignment.Center;
         _titleLabel!.IsVisible = false;
      }
   }

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      if (!_scalingAwareConfigApplied) {
         _scalingAwareConfigApplied = true;
         _controlTokenBinder.AddControlBinding(LineWidthProperty, GlobalResourceKey.LineWidth, BindingPriority.Style, 
                                               new RenderScaleAwareDoubleConfigure(this));
      }
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == OrientationProperty) {
         if (_initialized) {
            HandleForOrientation();
         }
      }
   }

   // 当为水平分隔线的时候，我们设置最小的高度，当为垂直分割线的时候我们设置一个合适宽度
   // 然后保持尽可能保持文字尽可能的显示，如果小于最小分隔部分的两倍的时候，文字隐藏。
   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetHeight = size.Height;
      var targetWidth = size.Width;
      if (Orientation == Orientation.Horizontal) {
         if (Title is null || Title?.Length == 0) {
            targetHeight = LineWidth * 3;
         }

         if (!double.IsInfinity(availableSize.Width)) {
            targetWidth = Math.Max(availableSize.Width, targetWidth);
         }
      } else {
         targetWidth = Math.Max(1, LineWidth) + _verticalMarginInline;
         targetHeight = FontUtils.ConvertEmToPixel(1, FontSize, TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0);
         if (!double.IsInfinity(availableSize.Height)) {
            targetHeight = Math.Max(availableSize.Height, targetHeight);
         }
      }
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      if (Orientation == Orientation.Horizontal && _titleLabel!.IsVisible) {
         var titleRect = GetTitleRect(finalSize);
         _titleLabel.Arrange(titleRect);
      }
      return finalSize;
   }

   private double GetTextPaddingInline()
   {
      return FontSize * _textPaddingInline;
   }

   private Rect GetTitleRect(Size finalSize)
   {
      Rect titleRect = default;
      if (Orientation == Orientation.Horizontal && Title?.Length > 0) {
         // 线最小得占到 25 %，拍脑袋
         var lineMinWidth = finalSize.Width * SEPARATOR_LINE_MIN_PROPORTION;
         var titleWidth = _titleLabel!.DesiredSize.Width + 2;
         var remainWidth = finalSize.Width - titleWidth - GetTextPaddingInline() * 2;
         if (lineMinWidth > remainWidth) {
            // 字过多
            titleWidth = finalSize.Width - lineMinWidth;
         }
         // 处理完成之后，字的宽度一定在 width 范围内
         // 计算位置
         if (TitlePosition == SeparatorTitlePosition.Left) {
            if (!double.IsNaN(OrientationMargin)) {
               _currentEdgeDistance = Math.Min((finalSize.Width - titleWidth) / 2, OrientationMargin);
            } else {
               _currentEdgeDistance = finalSize.Width * _orientationMarginPercent;
            }
            titleRect = new Rect(new Point(_currentEdgeDistance + GetTextPaddingInline(), 0), new Size(titleWidth, finalSize.Height));
            var rightDelta = titleRect.Right - finalSize.Width;
            if (MathUtils.GreaterThan(rightDelta, 0)) {
               titleRect = titleRect.WithWidth(finalSize.Width - titleRect.Left);
            }
         } else if (TitlePosition == SeparatorTitlePosition.Right) {
            if (!double.IsNaN(OrientationMargin)) {
               _currentEdgeDistance = Math.Min((finalSize.Width - titleWidth) / 2, OrientationMargin);
            } else {
               _currentEdgeDistance = finalSize.Width * _orientationMarginPercent;
            }
            titleRect = new Rect(new Point(finalSize.Width - _currentEdgeDistance - titleWidth - GetTextPaddingInline() * 2, 0),
               new Size(titleWidth, finalSize.Height));
            double leftDelta = titleRect.Left - 0;
            if (leftDelta < 0) {
               titleRect = titleRect.WithX(0);
            }
         } else {
            // 居中
            titleRect = new Rect(new Point((finalSize.Width - titleWidth) / 2, 0), new Size(titleWidth, finalSize.Height));
         }
      }
      return titleRect;
   }

   public override void Render(DrawingContext context)
   {
      using var state = context.PushRenderOptions(new RenderOptions
      {
         BitmapInterpolationMode = BitmapInterpolationMode.LowQuality,
         TextRenderingMode = TextRenderingMode.Alias
      });

      var linePen = new Pen(LineColor, LineWidth);
      var controlRect = new Rect(new Point(0, 0), DesiredSize);
      if (IsDashedLine) {
         linePen.DashStyle = DashStyle.Dash;
      }
      
      if (Orientation == Orientation.Horizontal) {
         var offsetY = controlRect.Height / 2.0;
         if (Title?.Length > 0) {
            // 画两个线段
            var titleRect = GetTitleRect(DesiredSize);
            if (TitlePosition == SeparatorTitlePosition.Left) {
               if (double.IsNaN(OrientationMargin)) {
                  context.DrawLine(linePen, new Point(0, offsetY), new Point(titleRect.Left - GetTextPaddingInline(), offsetY));
               }
               context.DrawLine(linePen, new Point(titleRect.Right + GetTextPaddingInline(), offsetY), 
                                new Point(controlRect.Right, offsetY));
            } else if (TitlePosition == SeparatorTitlePosition.Right) {
               context.DrawLine(linePen, new Point(0, offsetY), new Point(titleRect.Left - GetTextPaddingInline(), offsetY));
               if (double.IsNaN(OrientationMargin)) {
                  context.DrawLine(linePen, new Point(titleRect.Right + GetTextPaddingInline(), offsetY), new Point(controlRect.Right, offsetY));
               }
            } else {
               context.DrawLine(linePen, new Point(0, offsetY), new Point(titleRect.Left - GetTextPaddingInline(), offsetY));
               context.DrawLine(linePen, new Point(titleRect.Right + GetTextPaddingInline(), offsetY), new Point(controlRect.Right, offsetY));
            }
         } else {
            context.DrawLine(linePen, new Point(0, offsetY), new Point(controlRect.Right, offsetY));
         }
      } else {
         var offsetX = controlRect.Width / 2.0;
         context.DrawLine(linePen, new Point(offsetX, 0), new Point(offsetX, controlRect.Bottom));
      }
   }
}