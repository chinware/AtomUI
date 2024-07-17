using System.Reactive.Linq;
using AtomUI.ColorSystem;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class RibbonBadgeAdorner : Control, IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private TextBlock? _textBlock;
   private Geometry? _cornerGeometry;
   private readonly BorderRenderHelper _borderRenderHelper;

   public static readonly DirectProperty<RibbonBadgeAdorner, IBrush?> RibbonColorProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, IBrush?>(
         nameof(RibbonColor),
         o => o.RibbonColor,
         (o, v) => o.RibbonColor = v);

   private IBrush? _ribbonColor;

   public IBrush? RibbonColor
   {
      get => _ribbonColor;
      set => SetAndRaise(RibbonColorProperty, ref _ribbonColor, value);
   }

   public static readonly DirectProperty<RibbonBadgeAdorner, string?> TextProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, string?>(
         nameof(Text),
         o => o.Text,
         (o, v) => o.Text = v);

   private string? _text;

   public string? Text
   {
      get => _text;
      set => SetAndRaise(TextProperty, ref _text, value);
   }

   public static readonly DirectProperty<RibbonBadgeAdorner, bool> IsAdornerModeProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, bool>(
         nameof(IsAdornerMode),
         o => o.IsAdornerMode,
         (o, v) => o.IsAdornerMode = v);

   private bool _isAdornerMode = false;

   public bool IsAdornerMode
   {
      get => _isAdornerMode;
      set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
   }

   public static readonly DirectProperty<RibbonBadgeAdorner, RibbonBadgePlacement> PlacementProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, RibbonBadgePlacement>(
         nameof(Placement),
         o => o.Placement,
         (o, v) => o.Placement = v);

   private RibbonBadgePlacement _placement;

   // 当前有效的颜色
   public RibbonBadgePlacement Placement
   {
      get => _placement;
      set => SetAndRaise(PlacementProperty, ref _placement, value);
   }

   public static readonly DirectProperty<RibbonBadgeAdorner, Point> OffsetProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, Point>(
         nameof(Offset),
         o => o.Offset,
         (o, v) => o.Offset = v);

   private Point _offset;

   public Point Offset
   {
      get => _offset;
      set => SetAndRaise(OffsetProperty, ref _offset, value);
   }

   static RibbonBadgeAdorner()
   {
      AffectsMeasure<RibbonBadgeAdorner>(TextProperty, IsAdornerModeProperty);
      AffectsMeasure<RibbonBadgeAdorner>(PlacementProperty);
      AffectsRender<RibbonBadgeAdorner>(RibbonColorProperty, OffsetProperty);
   }

   public RibbonBadgeAdorner()
   {
      _customStyle = this;
      _borderRenderHelper = new BorderRenderHelper();
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   void IControlCustomStyle.SetupUi()
   {
      HorizontalAlignment = HorizontalAlignment.Left;
      VerticalAlignment = VerticalAlignment.Top;
      _textBlock = new TextBlock()
      {
         Text = Text,
         HorizontalAlignment = HorizontalAlignment.Left,
         VerticalAlignment = VerticalAlignment.Center,
      };
      LogicalChildren.Add(_textBlock);
      VisualChildren.Add(_textBlock);
      _customStyle.ApplyFixedStyleConfig();
      BuildCornerGeometry();
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, BadgeRibbonOffsetTokenProperty, BadgeResourceKey.BadgeRibbonOffset);
      BindUtils.CreateTokenBinding(this, MarginXSTokenProperty, GlobalResourceKey.MarginXS);
      BindUtils.CreateTokenBinding(this, ColorPrimaryTokenProperty, GlobalResourceKey.ColorPrimary);
      BindUtils.CreateTokenBinding(this, BadgeRibbonCornerTransformTokenProperty,
                                   BadgeResourceKey.BadgeRibbonCornerTransform);
      BindUtils.CreateTokenBinding(this, BadgeRibbonCornerDarkenAmountTokenProperty,
                                   BadgeResourceKey.BadgeRibbonCornerDarkenAmount);
      BindUtils.CreateTokenBinding(this, BorderRadiusSMTokenProperty, GlobalResourceKey.BorderRadiusSM);

      if (_textBlock is not null) {
         BindUtils.CreateTokenBinding(_textBlock, TextBlock.ForegroundProperty,
                                      GlobalResourceKey.ColorTextLightSolid);
         BindUtils.CreateTokenBinding(_textBlock, TextBlock.LineHeightProperty,
                                      BadgeResourceKey.BadgeFontHeight);
         BindUtils.CreateTokenBinding(_textBlock, TextBlock.PaddingProperty, GlobalResourceKey.PaddingXS,
                                      BindingPriority.Template,
                                      o =>
                                      {
                                         if (o is double value) {
                                            return new Thickness(value, 0);
                                         }

                                         return o;
                                      });
      }
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      // TODO 这里是否需要增加一个什么判断？
      _customStyle.ApplyFixedStyleConfig();
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = availableSize.Width;
      var targetHeight = availableSize.Height;
      if (!IsAdornerMode) {
         targetHeight = size.Height + _cornerGeometry?.Bounds.Height ?? 0;
         targetWidth = size.Width;
      }

      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      if (_textBlock is not null) {
         _textBlock.Arrange(GetTextRect());
      }

      return finalSize;
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (_initialized) {
         if (e.Property == PlacementProperty) {
            BuildCornerGeometry(true);
         }
      }
   }

   public override void Render(DrawingContext context)
   {
      var backgroundBrush = (SolidColorBrush)(RibbonColor ?? _colorPrimaryToken)!;
      {
         var textRect = GetTextRect();
         using var state = context.PushTransform(Matrix.CreateTranslation(textRect.X, textRect.Y));

         _borderRenderHelper.Render(context,
                                    borderThickness: new Thickness(0),
                                    backgroundSizing: BackgroundSizing.OuterBorderEdge,
                                    finalSize: textRect.Size,
                                    cornerRadius: new CornerRadius(topLeft: _borderRadiusSMToken.TopLeft,
                                                                   topRight: _borderRadiusSMToken.TopRight,
                                                                   bottomLeft: Placement == RibbonBadgePlacement.Start
                                                                      ? 0
                                                                      : _borderRadiusSMToken.BottomLeft,
                                                                   bottomRight: Placement == RibbonBadgePlacement.End
                                                                      ? 0
                                                                      : _borderRadiusSMToken.BottomRight),
                                    background: backgroundBrush,
                                    borderBrush: null,
                                    boxShadows: new BoxShadows());
      }
      {
         var cornerRect = GetCornerRect();
         using var state = context.PushTransform(Matrix.CreateTranslation(cornerRect.X, cornerRect.Y));
         var backgroundColor = backgroundBrush.Color;
         var cornerBrush = new SolidColorBrush(backgroundColor.Darken(_badgeRibbonCornerDarkenAmountToken));
         context.DrawGeometry(cornerBrush, null, _cornerGeometry!);
      }
   }

   private Rect GetTextRect()
   {
      if (_textBlock is null) {
         return default;
      }

      var offsetX = 0d;
      var offsetY = 0d;
      if (IsAdornerMode) {
         offsetY += _badgeRibbonOffsetToken.Y;
         if (Placement == RibbonBadgePlacement.End) {
            offsetX = DesiredSize.Width - _textBlock.DesiredSize.Width + _badgeRibbonOffsetToken.X;
         } else {
            offsetX = -_badgeRibbonOffsetToken.X;
         }
      }

      return new Rect(new Point(offsetX, offsetY), _textBlock.DesiredSize);
   }

   private Rect GetCornerRect()
   {
      if (_cornerGeometry is null) {
         return default;
      }

      var targetWidth = _cornerGeometry.Bounds.Width;
      var targetHeight = _cornerGeometry.Bounds.Height;
      var offsetX = 0d;
      var offsetY = 0d;
      if (!IsAdornerMode) {
         offsetY = DesiredSize.Height - targetHeight;
         if (Placement == RibbonBadgePlacement.End) {
            offsetX = DesiredSize.Width - targetWidth;
         }
      } else {
         var textRect = GetTextRect();
         if (Placement == RibbonBadgePlacement.End) {
            offsetX = textRect.Right - targetWidth;
         }

         offsetY = textRect.Bottom;
      }

      return new Rect(new Point(offsetX, offsetY), new Size(targetWidth, targetHeight));
   }

   private void BuildCornerGeometry(bool force = false)
   {
      if (force || _cornerGeometry is null) {
         var width = _badgeRibbonOffsetToken.X;
         var height = _badgeRibbonOffsetToken.Y;
         var geometryStream = new StreamGeometry();
         using var context = geometryStream.Open();
         var p1 = new Point(0, 0);
         var p2 = new Point(0, height);
         var p3 = new Point(width, 0);
         context.LineTo(p1, true);
         context.LineTo(p2);
         context.LineTo(p3);
         context.EndFigure(true);
         _cornerGeometry = geometryStream;
         var transforms = new TransformGroup();
         if (_badgeRibbonCornerTransformToken is not null) {
            transforms.Children.Add(_badgeRibbonCornerTransformToken);
         }

         if (Placement == RibbonBadgePlacement.Start) {
            transforms.Children.Add(new ScaleTransform(-1, 1));
         }

         _cornerGeometry.Transform = transforms;
      }
   }
}