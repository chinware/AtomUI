using AtomUI.Data;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls.Primitives;

internal class ShadowRenderer : Control
{
   public static readonly StyledProperty<BoxShadows> ShadowsProperty =
      Border.BoxShadowProperty.AddOwner<ShadowRenderer>();

   public static readonly StyledProperty<CornerRadius> MaskCornerRadiusProperty =
      Border.CornerRadiusProperty.AddOwner<ShadowRenderer>();
   
   public static readonly StyledProperty<IBrush?> MaskContentBackgroundProperty =
      Border.BackgroundProperty.AddOwner<ShadowRenderer>();

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

   public IBrush? MaskContentBackground
   {
      get => GetValue(MaskContentBackgroundProperty);
      set => SetValue(MaskContentBackgroundProperty, value);
   }

   protected Border? _maskContent;
   protected bool _initialized = false;
   protected Canvas? _layout;

   static ShadowRenderer()
   {
      AffectsMeasure<ShadowRenderer>(ShadowsProperty);
      MaskContentBackgroundProperty.OverrideDefaultValue<ShadowRenderer>(new SolidColorBrush(Colors.White));
   }

   public sealed override void ApplyTemplate()
   {
      base.ApplyTemplate();
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
         HorizontalAlignment = HorizontalAlignment.Stretch,
         VerticalAlignment = VerticalAlignment.Stretch
      };
      // TODO 这个是否需要资源管理起来
      BindUtils.RelayBind(this, ShadowsProperty, maskContent, ShadowsProperty);
      BindUtils.RelayBind(this, MaskCornerRadiusProperty, maskContent, Border.CornerRadiusProperty);
      BindUtils.RelayBind(this, MaskContentBackgroundProperty, maskContent, Border.BackgroundProperty);
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