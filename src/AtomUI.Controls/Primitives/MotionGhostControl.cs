using AtomUI.ColorSystem;
using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace AtomUI.Controls.Primitives;

internal class MotionGhostControl : Control, INotifyCaptureGhostBitmap
{
   public static readonly StyledProperty<BoxShadows> ShadowsProperty =
      Border.BoxShadowProperty.AddOwner<MotionGhostControl>();

   public static readonly StyledProperty<CornerRadius> MaskCornerRadiusProperty =
      Border.CornerRadiusProperty.AddOwner<MotionGhostControl>();

   /// <summary>
   /// 渲染的阴影值
   /// </summary>
   public BoxShadows Shadows
   {
      get => GetValue(ShadowsProperty);
      set => SetValue(ShadowsProperty, value);
   }

   /// <summary>
   /// mask 的圆角大小
   /// </summary>
   public CornerRadius MaskCornerRadius
   {
      get => GetValue(MaskCornerRadiusProperty);
      set => SetValue(MaskCornerRadiusProperty, value);
   }

   protected Border? _maskRenderer;
   protected Border? _contentRenderer;
   protected bool _initialized = false;
   protected Canvas? _layout;
   protected Control _motionTarget;
   protected RenderTargetBitmap? _ghostBitmap;
   protected Size _motionTargetSize;

   static MotionGhostControl()
   {
      AffectsMeasure<ShadowRenderer>(ShadowsProperty);
      AffectsRender<ShadowRenderer>(MaskCornerRadiusProperty);
   }

   public MotionGhostControl(Control motionTarget, BoxShadows shadows)
   {
      _motionTarget = motionTarget;
      if (_motionTarget.DesiredSize == default) {
         _motionTargetSize = LayoutHelper.MeasureChild(_motionTarget, Size.Infinity, new Thickness());
      } else {
         _motionTargetSize = _motionTarget.DesiredSize;
      }

      foreach (var boxShadow in shadows) {
         
      }
      Shadows =  new BoxShadows(new BoxShadow
      {
         OffsetX = 0,
         OffsetY = 6 * 2,
         Blur = 16 * 2,
         Spread = 0,
         Color = ColorUtils.FromRgbF(0.10, 0, 0, 0)
      }, new []
      {
         new BoxShadow
         {
            OffsetX = 0,
            OffsetY = 3 * 2,
            Blur = 6 * 2,
            Spread = -4 * 2,
            Color = ColorUtils.FromRgbF(0.12, 0, 0, 0)
         },
         new BoxShadow
         {
            OffsetX = 0,
            OffsetY = 9 * 2,
            Blur = 28 * 2,
            Spread = 8 * 2,
            Color = ColorUtils.FromRgbF(0.07, 0, 0, 0)
         }
      });;
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         IsHitTestVisible = false;
         
         _layout = new Canvas();
         VisualChildren.Add(_layout);
          LogicalChildren.Add(_layout);
         
         var shadowThickness = Shadows.Thickness();
         var offsetX = shadowThickness.Left;
         var offsetY = shadowThickness.Top;
         
         _maskRenderer = new Border
         {
            BorderThickness = new Thickness(0),
            BoxShadow = Shadows,
            CornerRadius = MaskCornerRadius,
            Width = _motionTargetSize.Width,
            Height = _motionTargetSize.Height
         };
         
         _layout.Children.Add(_maskRenderer);
         _layout.Children.Add(_motionTarget);
         
         Canvas.SetLeft(_maskRenderer, offsetX);
         Canvas.SetTop(_maskRenderer, offsetY);
         
         Canvas.SetLeft(_motionTarget, offsetX);
         Canvas.SetTop(_motionTarget, offsetY);
         
         _initialized = true;
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      base.MeasureOverride(availableSize);
      var shadowThickness = Shadows.Thickness();
      Size motionTargetSize;
      if (_motionTarget.DesiredSize == default) {
         motionTargetSize = LayoutHelper.MeasureChild(_motionTarget, Size.Infinity, new Thickness());
      } else {
         motionTargetSize = _motionTarget.DesiredSize;
      }
      return motionTargetSize.Inflate(shadowThickness);
   }

   public override void Render(DrawingContext context)
   {
      if (_ghostBitmap is not null) {
         var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
         context.DrawImage(_ghostBitmap, new Rect(new Point(0, 0), DesiredSize * scaling), 
                           new Rect(new Point(0, 0), DesiredSize));
      }
   }

   public void NotifyCaptureGhostBitmap()
   {
      if (_ghostBitmap is null) {
         var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
         _ghostBitmap = new RenderTargetBitmap(new PixelSize((int)(DesiredSize.Width * scaling), (int)(DesiredSize.Height * scaling)),
            new Vector(96, 96));
         _ghostBitmap.Render(this);
         _layout!.Children.Clear();
      }
   }
}