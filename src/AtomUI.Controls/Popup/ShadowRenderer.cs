using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class ShadowRenderer : Control
{
   public static readonly DirectProperty<ShadowRenderer, BoxShadows> ShadowsProperty =
      AvaloniaProperty.RegisterDirect<ShadowRenderer, BoxShadows>(
         nameof(Shadows),
         o => o.Shadows,
         (o, v) => o.Shadows = v);

   public static readonly DirectProperty<ShadowRenderer, Point> MaskOffsetProperty =
      AvaloniaProperty.RegisterDirect<ShadowRenderer, Point>(
         nameof(MaskOffset),
         o => o.MaskOffset,
         (o, v) => o.MaskOffset = v);

   public static readonly DirectProperty<ShadowRenderer, Size> MaskSizeProperty =
      AvaloniaProperty.RegisterDirect<ShadowRenderer, Size>(
         nameof(MaskSize),
         o => o.MaskSize,
         (o, v) => o.MaskSize = v);

   public static readonly DirectProperty<ShadowRenderer, CornerRadius> MaskCornerRadiusProperty =
      AvaloniaProperty.RegisterDirect<ShadowRenderer, CornerRadius>(
         nameof(MaskCornerRadius),
         o => o.MaskCornerRadius,
         (o, v) => o.MaskCornerRadius = v);

   private BoxShadows _shadows;

   /// <summary>
   /// 渲染的阴影值
   /// </summary>
   public BoxShadows Shadows
   {
      get => _shadows;
      set => SetAndRaise(ShadowsProperty, ref _shadows, value);
   }

   private Point _maskOffset;

   /// <summary>
   /// mask 渲染的位移
   /// </summary>
   public Point MaskOffset
   {
      get => _maskOffset;
      set => SetAndRaise(MaskOffsetProperty, ref _maskOffset, value);
   }

   private Size _maskSize;

   /// <summary>
   /// mask 渲染大小
   /// </summary>
   public Size MaskSize
   {
      get => _maskSize;
      set => SetAndRaise(MaskSizeProperty, ref _maskSize, value);
   }

   private CornerRadius _maskCornerRadius;
   /// <summary>
   /// mask 的圆角大小
   /// </summary>
   public CornerRadius MaskCornerRadius
   {
      get => _maskCornerRadius;
      set => SetAndRaise(MaskCornerRadiusProperty, ref _maskCornerRadius, value);
   }

   private Border? _maskContent;
   private bool _initialized = false;
   private Canvas? _layout;

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         HorizontalAlignment = HorizontalAlignment.Stretch;
         VerticalAlignment = VerticalAlignment.Stretch;
         IsHitTestVisible = false;
         CreateMaskContent();
         _layout = new Canvas();
         VisualChildren.Add(_layout);
         ((ISetLogicalParent)_layout).SetParent(this);
         _maskContent = CreateMaskContent();
         _layout.Children.Add(_maskContent);
         _initialized = true;
      }
   }

   private Border CreateMaskContent()
   {
      var maskContent = new Border()
      {
         Width = 100,
         Height = 40,
         BorderThickness = new Thickness(0),
         Background = new SolidColorBrush(Colors.White)
      };
      
      Canvas.SetLeft(maskContent,30);
      Canvas.SetTop(maskContent, 20);
      
      // TODO 需要考虑释放
      BindUtils.RelayBind(this, ShadowsProperty, maskContent, Border.BoxShadowProperty);
      BindUtils.RelayBind(this, MaskCornerRadiusProperty, maskContent, Border.CornerRadiusProperty);
      
      return maskContent;
   }
}