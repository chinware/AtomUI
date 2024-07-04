using System.Reactive.Disposables;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
   private Control _motionTarget;

   private CompositeDisposable? _compositeDisposable;
   
   static MotionGhostControl()
   {
      AffectsMeasure<ShadowRenderer>(ShadowsProperty);
      AffectsRender<ShadowRenderer>(MaskCornerRadiusProperty);
   }

   public MotionGhostControl(Control motionTarget)
   {
      _motionTarget = motionTarget;
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _compositeDisposable = new CompositeDisposable();
         HorizontalAlignment = HorizontalAlignment.Stretch;
         VerticalAlignment = VerticalAlignment.Stretch;
         IsHitTestVisible = false;
         _layout = new Canvas();
         VisualChildren.Add(_layout);
         ((ISetLogicalParent)_layout).SetParent(this);
         _maskRenderer = CreateMaskRenderer();
         _contentRenderer = CreateContentRenderer();
         SetupMaskRenderer(_maskRenderer);
         SetupContentRenderer(_maskRenderer, _contentRenderer);
         _layout.Children.Add(_maskRenderer);
         _layout.Children.Add(_contentRenderer);
         _initialized = true;
      }
   }

   protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromLogicalTree(e);
      _compositeDisposable?.Dispose();
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      Size motionTargetSize = default;
      if (_motionTarget.DesiredSize == default) {
         // Popup may not have been shown yet. Measure content
         motionTargetSize = LayoutHelper.MeasureChild(_motionTarget, availableSize, new Thickness());
      } else {
         motionTargetSize = _motionTarget.DesiredSize;
      }

      var shadowThickness = Shadows.Thickness();
      return motionTargetSize.Inflate(shadowThickness);
   }

   private Border CreateContentRenderer()
   {
      var contentRenderer = new Border
      {
         BorderThickness = new Thickness(0),
         HorizontalAlignment = HorizontalAlignment.Stretch,
         VerticalAlignment = VerticalAlignment.Stretch,
         Background = new VisualBrush
         {
            Visual = _motionTarget,
            Stretch = Stretch.None,
            AlignmentX = AlignmentX.Left,
         }
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
         Background = new VisualBrush
         {
            Visual = _motionTarget,
            Stretch = Stretch.None,
            AlignmentX = AlignmentX.Left,
         }
      };
      
      return maskContent;
   }

   private void SetupMaskRenderer(Border maskRenderer)
   {
      
      CornerRadius cornerRadius = default;
      BoxShadows shadows = default;

      if (Shadows != default) {
         shadows = Shadows;
      }

      if (MaskCornerRadius != default) {
         cornerRadius = MaskCornerRadius;
      }
      var shadowThickness = shadows.Thickness();
      var offsetX = shadowThickness.Left;
      var offsetY = shadowThickness.Top;
      if (_motionTarget is IShadowMaskInfoProvider shadowMaskInfoProvider) {
         var maskCornerRadius = shadowMaskInfoProvider.GetMaskCornerRadius();
         var maskBounds = shadowMaskInfoProvider.GetMaskBounds();
         if (cornerRadius == default) {
            cornerRadius = maskCornerRadius;
         }

         offsetY += maskBounds.Y;
         offsetX += maskBounds.X;
         maskRenderer.Width = maskBounds.Width;
         maskRenderer.Height = maskBounds.Height;
      } else if (_motionTarget is BorderedStyleControl bordered) {
         if (cornerRadius == default) {
            cornerRadius = bordered.CornerRadius;
         }
         maskRenderer.Width = _motionTarget.DesiredSize.Width;
         maskRenderer.Height = _motionTarget.DesiredSize.Height;
      } else if (_motionTarget is TemplatedControl templatedControl) {
         if (cornerRadius == default) {
            cornerRadius = templatedControl.CornerRadius;
         }
         maskRenderer.Width = _motionTarget.DesiredSize.Width;
         maskRenderer.Height = _motionTarget.DesiredSize.Height;
      }

      maskRenderer.BoxShadow = shadows;
      maskRenderer.CornerRadius = cornerRadius;
   
      Canvas.SetLeft(maskRenderer, offsetX);
      Canvas.SetTop(maskRenderer, offsetY);
   }

   private void SetupContentRenderer(Border maskRenderer, Border contentRenderer)
   {
      contentRenderer.Width = _motionTarget.DesiredSize.Width;
      contentRenderer.Height = _motionTarget.DesiredSize.Height;
      var shadowThickness = maskRenderer.BoxShadow.Thickness();
      Canvas.SetLeft(contentRenderer, shadowThickness.Left);
      Canvas.SetTop(contentRenderer, shadowThickness.Top);
   }
}