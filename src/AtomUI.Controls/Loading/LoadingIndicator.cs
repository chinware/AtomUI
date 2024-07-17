using AtomUI.Icon;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Avalonia.Styling;

namespace AtomUI.Controls;

public partial class LoadingIndicator : Control, ISizeTypeAware, IControlCustomStyle
{
   #region 公共属性定义
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<LoadingIndicator, SizeType>(nameof(SizeType), SizeType.Middle);

   public static readonly StyledProperty<string?> LoadingMsgProperty =
      AvaloniaProperty.Register<LoadingIndicator, string?>(nameof(LoadingMsg));
   
   public static readonly StyledProperty<bool> IsShowLoadingMsgProperty =
      AvaloniaProperty.Register<LoadingIndicator, bool>(nameof(IsShowLoadingMsg), false);
   
   public static readonly StyledProperty<PathIcon?> CustomIndicatorIconProperty =
      AvaloniaProperty.Register<LoadingIndicator, PathIcon?>(nameof(CustomIndicatorIcon));
   
   public static readonly StyledProperty<TimeSpan?> MotionDurationProperty =
      AvaloniaProperty.Register<LoadingIndicator, TimeSpan?>(nameof(MotionDuration));
   
   public static readonly StyledProperty<Easing?> MotionEasingCurveProperty =
      AvaloniaProperty.Register<LoadingIndicator, Easing?>(nameof(MotionEasingCurve));
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public string? LoadingMsg
   {
      get => GetValue(LoadingMsgProperty);
      set => SetValue(LoadingMsgProperty, value);
   }
   
   public bool IsShowLoadingMsg
   {
      get => GetValue(IsShowLoadingMsgProperty);
      set => SetValue(IsShowLoadingMsgProperty, value);
   }
   
   public PathIcon? CustomIndicatorIcon
   {
      get => GetValue(CustomIndicatorIconProperty);
      set => SetValue(CustomIndicatorIconProperty, value);
   }
      
   public TimeSpan? MotionDuration
   {
      get => GetValue(MotionDurationProperty);
      set => SetValue(MotionDurationProperty, value);
   }
   
   public Easing? MotionEasingCurve
   {
      get => GetValue(MotionEasingCurveProperty);
      set => SetValue(MotionEasingCurveProperty, value);
   }
   
   #endregion

   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private Animation? _animation;
   private TextBlock? _textBlock;
   private RenderInfo? _renderInfo;
   private CancellationTokenSource?_cancellationTokenSource;
   
   private const double LARGE_INDICATOR_SIZE = 48;
   private const double MIDDLE_INDICATOR_SIZE = 32;
   private const double SMALL_INDICATOR_SIZE = 16;
   private const double MAX_CONTENT_WIDTH = 120; // 拍脑袋的决定
   private const double MAX_CONTENT_HEIGHT = 400;
   private const double DOT_START_OPACITY = 0.3;
   
   static LoadingIndicator()
   {
      AffectsMeasure<LoadingIndicator>(SizeTypeProperty, 
                                       LoadingMsgProperty, 
                                       IsShowLoadingMsgProperty,
                                       CustomIndicatorIconProperty);
      AffectsRender<LoadingIndicator>(IndicatorAngleProperty);
   }

   public LoadingIndicator()
   {
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
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      BuildIndicatorAnimation();
      _cancellationTokenSource?.Cancel();
      _cancellationTokenSource = new CancellationTokenSource();
      _animation!.RunAsync(this, _cancellationTokenSource.Token);
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      _cancellationTokenSource?.Cancel();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == CustomIndicatorIconProperty) {
         if (_initialized) {
            var oldCustomIcon = e.GetOldValue<PathIcon?>();
            if (oldCustomIcon is not null) {
               LogicalChildren.Remove(oldCustomIcon);
               VisualChildren.Remove(oldCustomIcon);
            }
            SetupCustomIndicator();
         }
      } else if (e.Property == SizeTypeProperty) {
         HandleSizeTypeChanged();
      } else if (e.Property == IndicatorAngleProperty) {
         if (CustomIndicatorIcon is not null) {
            HandleIndicatorAngleChanged();
         }
      }
   }

   // 只在使用自定义的 Icon 的时候有效
   private void HandleIndicatorAngleChanged()
   {
      if (CustomIndicatorIcon is not null) {
         var builder = new TransformOperations.Builder(1);
         builder.AppendRotate(MathUtils.Deg2Rad(IndicatorAngle));
         CustomIndicatorIcon.RenderTransform = builder.Build();
      }
   }

   private void SetupCustomIndicator()
   {
      if (CustomIndicatorIcon is not null) {
         VisualChildren.Add(CustomIndicatorIcon);
         LogicalChildren.Add(CustomIndicatorIcon);
         // 暂时为了简单起见，我们在这里先只能使用 SizeType 的大小
         var indicatorSize = GetIndicatorSize(SizeType);
         CustomIndicatorIcon.Width = indicatorSize;
         CustomIndicatorIcon.Height = indicatorSize;
         CustomIndicatorIcon.IconMode = IconMode.Normal;
         CustomIndicatorIcon.VerticalAlignment = VerticalAlignment.Center;
         CustomIndicatorIcon.HorizontalAlignment = HorizontalAlignment.Center;
      }
   }

   private void HandleSizeTypeChanged()
   {
      if (CustomIndicatorIcon is not null) {
         var indicatorSize = GetIndicatorSize(SizeType);
         CustomIndicatorIcon.Width = indicatorSize;
         CustomIndicatorIcon.Height = indicatorSize;
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var targetWidth = 0d;
      var targetHeight = 0d;
      if (IsShowLoadingMsg) {
         var size = base.MeasureOverride(new Size(Math.Min(availableSize.Width, MAX_CONTENT_WIDTH),
                                                  Math.Min(availableSize.Height, MAX_CONTENT_HEIGHT)));
         targetWidth += size.Width;
         targetHeight += size.Height;
         if (size.Height > 0) {
            targetHeight += GetLoadMsgPaddingTop();
         }
      }

      var indicatorSize = GetIndicatorSize(SizeType);
      targetWidth = Math.Max(indicatorSize, targetWidth);
      targetHeight += indicatorSize;
      
      return new Size(targetWidth, targetHeight);
   }

   private double GetLoadMsgPaddingTop()
   {
      return (GetEffectiveDotSize() - _fontSizeToken) / 2 + 2;
   }

   private double GetEffectiveDotSize()
   {
      var dotSize = 0d;
      if (SizeType == SizeType.Large) {
         dotSize = _dotSizeLGToken;
      } else if (SizeType == SizeType.Middle) {
         dotSize = _dotSizeToken;
      } else {
         dotSize = _dotSizeSMToken;
      }

      return dotSize;
   }
   
   protected override Size ArrangeOverride(Size finalSize)
   {
      if (IsShowLoadingMsg) {
         var msgRect = GetLoadingMsgRect();
         _textBlock!.Arrange(msgRect);
      }

      if (CustomIndicatorIcon is not null) {
         var indicatorRect = GetIndicatorRect();
         CustomIndicatorIcon.Arrange(indicatorRect);
      }

      return finalSize;
   }

   #region IControlCustomStyle 实现
   
   void IControlCustomStyle.SetupUi()
   {
      SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left, BindingPriority.Style);
      SetValue(VerticalAlignmentProperty, VerticalAlignment.Top, BindingPriority.Style);
      
      _textBlock = new TextBlock()
      {
         Text = LoadingMsg,
         HorizontalAlignment = HorizontalAlignment.Center,
         VerticalAlignment = VerticalAlignment.Center,
      };
      
      LogicalChildren.Add(_textBlock);
      VisualChildren.Add(_textBlock);
      
      SetupCustomIndicator();
      
      _customStyle.ApplyFixedStyleConfig();
   }
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, DotSizeTokenProperty, LoadingIndicatorResourceKey.DotSize);
      BindUtils.CreateTokenBinding(this, DotSizeSMTokenProperty, LoadingIndicatorResourceKey.DotSizeSM);
      BindUtils.CreateTokenBinding(this, DotSizeLGTokenProperty, LoadingIndicatorResourceKey.DotSizeLG);
      BindUtils.CreateTokenBinding(this, IndicatorDurationTokenProperty, LoadingIndicatorResourceKey.IndicatorDuration);
      BindUtils.CreateTokenBinding(this, FontSizeTokenProperty, GlobalResourceKey.FontSize);
      BindUtils.CreateTokenBinding(this, MarginXXSTokenProperty, GlobalResourceKey.MarginXXS);
      BindUtils.CreateTokenBinding(this, ColorPrimaryTokenProperty, GlobalResourceKey.ColorPrimary);
      BindUtils.CreateTokenBinding(_textBlock!, TextBlock.ForegroundProperty, GlobalResourceKey.ColorPrimary);
   }

   private void BuildIndicatorAnimation(bool force = false)
   {
      if (force || _animation is null) {
         _cancellationTokenSource?.Cancel();
         _animation = new Animation()
         {
            IterationCount = IterationCount.Infinite,
            Easing = MotionEasingCurve ?? new LinearEasing(),
            Duration = MotionDuration ?? _indicatorDurationToken,
            Children =
            {
               new KeyFrame
               {
                  Setters = { new Setter(IndicatorAngleProperty, 0d), }, Cue = new Cue(0.0d)
               },
               new KeyFrame
               {
                  Setters = { new Setter(IndicatorAngleProperty, 360d), }, Cue = new Cue(1.0d)
               }
            }
         };
         _cancellationTokenSource = null;
      }
   }

   private Rect GetIndicatorRect()
   {
      var indicatorSize = GetIndicatorSize(SizeType);
      var offsetX = (DesiredSize.Width - indicatorSize) / 2;
      var offsetY = (DesiredSize.Height - indicatorSize) / 2;
      if (IsShowLoadingMsg && LoadingMsg is not null) {
         offsetY -= _textBlock!.DesiredSize.Height / 2;
      }

      return new Rect(new Point(offsetX, offsetY), new Size(indicatorSize, indicatorSize));
   }

   private Rect GetLoadingMsgRect()
   {
      if (!IsShowLoadingMsg) {
         return default;
      }

      var indicatorRect = GetIndicatorRect();
      var offsetX = indicatorRect.Left;
      var offsetY = indicatorRect.Bottom;
      offsetX -= (_textBlock!.DesiredSize.Width - indicatorRect.Width) / 2;
      return new Rect(new Point(offsetX, offsetY), _textBlock.DesiredSize);
   }

   private static double GetIndicatorSize(SizeType sizeType)
   {
      return sizeType switch
      {
         SizeType.Small => SMALL_INDICATOR_SIZE,
         SizeType.Middle => MIDDLE_INDICATOR_SIZE,
         SizeType.Large => LARGE_INDICATOR_SIZE,
         _ => throw new ArgumentOutOfRangeException(nameof(sizeType), sizeType, "Invalid value for SizeType")
      };
   }

   private static double GetOpacityForAngle(double degree)
   {
      var mappedValue = (Math.Sin(MathUtils.Deg2Rad(degree)) + 1) / 2; // 将正弦波的范围从[-1, 1]映射到[0, 1]
      return DOT_START_OPACITY + (1 - DOT_START_OPACITY) * mappedValue;
   }

   public override void Render(DrawingContext context)
   {
      _customStyle.PrepareRenderInfo();
      if (CustomIndicatorIcon is null) {
         RenderBuiltInIndicator(context);
      }

      _renderInfo = null;
   }

   private void RenderBuiltInIndicator(DrawingContext context)
   {
      if (_renderInfo is not null) {
         var itemSize = _renderInfo.IndicatorItemSize;
         var rightItemOpacity = GetOpacityForAngle(_indicatorAngle);
         var bottomItemOpacity = GetOpacityForAngle(_indicatorAngle + 90);
         var leftItemOpacity = GetOpacityForAngle(_indicatorAngle + 180);
         var topItemOpacity = GetOpacityForAngle(_indicatorAngle + 270);

         var itemEdgeMargin = _renderInfo.ItemEdgeMargin;
         
         var indicatorRect = GetIndicatorRect();
         var centerPoint = indicatorRect.Center;
         
         var rightItemOffset = new Point(indicatorRect.Right - itemEdgeMargin - itemSize, centerPoint.Y - itemSize / 2);
         var bottomItemOffset = new Point(centerPoint.X - itemSize / 2, indicatorRect.Bottom - itemEdgeMargin - itemSize);
         var leftItemOffset = new Point(indicatorRect.Left + itemEdgeMargin, centerPoint.Y - itemSize / 2);
         var topItemOffset = new Point(centerPoint.X - itemSize / 2, indicatorRect.Top + itemEdgeMargin);
   
         var matrix = Matrix.CreateTranslation(-indicatorRect.Center.X, -indicatorRect.Center.Y);
         matrix *= Matrix.CreateRotation(MathUtils.Deg2Rad(IndicatorAngle));
         matrix *= Matrix.CreateTranslation(indicatorRect.Center.X, indicatorRect.Center.Y);
         using var translateState = context.PushTransform(matrix);
         
         {
            using var opacityState = context.PushOpacity(rightItemOpacity);
            var itemRect = new Rect(rightItemOffset, new Size(itemSize, itemSize));
            context.DrawEllipse(_renderInfo.DotBgBrush, null, itemRect);
         }
         {
            using var opacityState = context.PushOpacity(bottomItemOpacity);
            var itemRect = new Rect(bottomItemOffset, new Size(itemSize, itemSize));
            context.DrawEllipse(_renderInfo.DotBgBrush, null, itemRect);
         }
         {
            using var opacityState = context.PushOpacity(leftItemOpacity);
            var itemRect = new Rect(leftItemOffset, new Size(itemSize, itemSize));
            context.DrawEllipse(_renderInfo.DotBgBrush, null, itemRect);
         }
         {
            using var opacityState = context.PushOpacity(topItemOpacity);
            var itemRect = new Rect(topItemOffset, new Size(itemSize, itemSize));
            context.DrawEllipse(_renderInfo.DotBgBrush, null, itemRect);
         }
      }
   }

   void IControlCustomStyle.PrepareRenderInfo()
   {
      _renderInfo = new RenderInfo();
      if (SizeType == SizeType.Large) {
         _renderInfo.DotSize = _dotSizeLGToken;
         _renderInfo.IndicatorItemSize = (_dotSizeLGToken - _marginXXSToken) / 2.5;
      } else if (SizeType == SizeType.Middle) {
         _renderInfo.DotSize = _dotSizeToken;
         _renderInfo.IndicatorItemSize = (_dotSizeToken - _marginXXSToken) / 2;
      } else {
         _renderInfo.DotSize = _dotSizeSMToken;
         _renderInfo.IndicatorItemSize = (_dotSizeSMToken - _marginXXSToken) / 2;
      }
      _renderInfo.IndicatorItemSize *= 0.9;
      if (SizeType == SizeType.Large) {
         _renderInfo.ItemEdgeMargin = _renderInfo.IndicatorItemSize / 1.5;
      } else if (SizeType == SizeType.Middle) {
         _renderInfo.ItemEdgeMargin = _renderInfo.IndicatorItemSize / 1.8;
      } else {
         _renderInfo.ItemEdgeMargin = 0.5;
      }
 
      _renderInfo.DotBgBrush = _colorPrimaryToken;
   }

   // 跟渲染相关的数据
   private class RenderInfo
   {
      public double DotSize { get; set; }
      public double IndicatorItemSize { get; set; }
      public double ItemEdgeMargin { get; set; }
      public IBrush? DotBgBrush { get; set; }
   };

   #endregion
}