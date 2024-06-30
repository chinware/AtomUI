using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class ShadowRenderer : Control
{
   public static readonly StyledProperty<BoxShadows> ShadowsProperty =
      Border.BoxShadowProperty.AddOwner<ShadowRenderer>();

   public static readonly StyledProperty<CornerRadius> MaskCornerRadiusProperty =
      Border.CornerRadiusProperty.AddOwner<ShadowRenderer>();

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

   private Border? _maskContent;
   private bool _initialized = false;
   private Canvas? _layout;

   static ShadowRenderer()
   {
      AffectsMeasure<ShadowRenderer>(ShadowsProperty);
   }

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
         SetupContentSizeAndPos();
         _layout.Children.Add(_maskContent);
         _initialized = true;
      }
   }

   private Border CreateMaskContent()
   {
      var maskContent = new Border
      {
         BorderThickness = new Thickness(0),
         Background = new SolidColorBrush(Colors.Transparent),
         HorizontalAlignment = HorizontalAlignment.Stretch,
         VerticalAlignment = VerticalAlignment.Stretch
      };
      // TODO 这个是否需要资源管理起来
      BindUtils.RelayBind(this, ShadowsProperty, maskContent, ShadowsProperty);
      BindUtils.RelayBind(this, MaskCornerRadiusProperty, maskContent, Border.CornerRadiusProperty);
      return maskContent;
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (e.Property == ShadowsProperty ||
          e.Property == WidthProperty ||
          e.Property == HeightProperty) {
         SetupContentSizeAndPos();
      }
   }

   private void SetupContentSizeAndPos()
   {
      if (_maskContent is not null) {
         var shadowThickness = Shadows.Thickness();
         var targetWidth = Width - shadowThickness.Left - shadowThickness.Right;
         var targetHeight = Height - shadowThickness.Top - shadowThickness.Bottom;
         _maskContent.Width = targetWidth;
         _maskContent.Height = targetHeight;
         Canvas.SetLeft(_maskContent, shadowThickness.Left);
         Canvas.SetTop(_maskContent, shadowThickness.Top);
      }
   }
}