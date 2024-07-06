using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Media;
using AtomUI.MotionScene;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls.Primitives;

internal class MotionGhostControl : Control
{
   public static readonly StyledProperty<BoxShadows> ShadowsProperty =
      Border.BoxShadowProperty.AddOwner<MotionGhostControl>();

   public static readonly StyledProperty<CornerRadius> MaskCornerRadiusProperty =
      Border.CornerRadiusProperty.AddOwner<MotionGhostControl>();

   public static readonly StyledProperty<VisualBrush> GhostBrushProperty =
      AvaloniaProperty.Register<MotionGhostControl, VisualBrush>(nameof(GhostBrush));

   public static readonly StyledProperty<Size> MaskContentSizeProperty =
      AvaloniaProperty.Register<MotionGhostControl, Size>(nameof(MaskContentSize));

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

   public VisualBrush GhostBrush
   {
      get => GetValue(GhostBrushProperty);
      set => SetValue(GhostBrushProperty, value);
   }

   public Size MaskContentSize
   {
      get => GetValue(MaskContentSizeProperty);
      set => SetValue(MaskContentSizeProperty, value);
   }

   protected Border? _maskRenderer;
   protected Border? _contentRenderer;
   protected bool _initialized = false;
   protected Canvas? _layout;

   static MotionGhostControl()
   {
      AffectsMeasure<ShadowRenderer>(ShadowsProperty);
      AffectsRender<ShadowRenderer>(MaskCornerRadiusProperty);
   }

   public MotionGhostControl(VisualBrush ghostBrush)
   {
      GhostBrush = ghostBrush;
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         HorizontalAlignment = HorizontalAlignment.Stretch;
         VerticalAlignment = VerticalAlignment.Stretch;
         IsHitTestVisible = false;
         _layout = new Canvas();
         VisualChildren.Add(_layout);
         ((ISetLogicalParent)_layout).SetParent(this);
         _maskRenderer = CreateMaskRenderer();
         _contentRenderer = CreateContentRenderer();
         SetupMaskRenderer(_maskRenderer);
         SetupContentRenderer(_contentRenderer);
         _layout.Children.Add(_maskRenderer);
         _layout.Children.Add(_contentRenderer);
         _initialized = true;
      }
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      base.MeasureOverride(availableSize);
      var shadowThickness = Shadows.Thickness();
      return MaskContentSize.Inflate(shadowThickness);
   }

   private Border CreateContentRenderer()
   {
      var contentRenderer = new Border
      {
         BorderThickness = new Thickness(0),
         HorizontalAlignment = HorizontalAlignment.Stretch,
         VerticalAlignment = VerticalAlignment.Stretch,
         Background = GhostBrush
      };

      return contentRenderer;
   }

   private Border CreateMaskRenderer()
   {
      var maskContent = new Border
      {
         BorderThickness = new Thickness(0),
         HorizontalAlignment = HorizontalAlignment.Stretch,
         VerticalAlignment = VerticalAlignment.Stretch,
      };

      return maskContent;
   }

   private void SetupMaskRenderer(Border maskRenderer)
   {
      var shadowThickness = Shadows.Thickness();
      var offsetX = shadowThickness.Left;
      var offsetY = shadowThickness.Top;

      maskRenderer.BoxShadow = Shadows;
      maskRenderer.CornerRadius = MaskCornerRadius;

      maskRenderer.Width = MaskContentSize.Width;
      maskRenderer.Height = MaskContentSize.Height;

      Canvas.SetLeft(maskRenderer, offsetX);
      Canvas.SetTop(maskRenderer, offsetY);
   }

   private void SetupContentRenderer(Border contentRenderer)
   {
      var shadowThickness = Shadows.Thickness();
      contentRenderer.Width = MaskContentSize.Width;
      contentRenderer.Height = MaskContentSize.Height;

      Canvas.SetLeft(contentRenderer, shadowThickness.Left);
      Canvas.SetTop(contentRenderer, shadowThickness.Top);
   }
   
   public override void Render(DrawingContext context)
   {
      //context.FillRectangle(new SolidColorBrush(Colors.Aqua), new Rect(new Point(0, 0), DesiredSize));
   }
}