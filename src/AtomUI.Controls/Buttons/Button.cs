using AtomUI.Data;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;
using ButtonSizeType = SizeType;

public enum ButtonType
{
   Default,
   Primary,
   Link,
   Text
}

public enum ButtonShape
{
   Default,
   Circle,
   Round,
}

public partial class Button : AvaloniaButton, ISizeTypeAware
{
   #region 公共属性定义
   public static readonly StyledProperty<ButtonType> ButtonTypeProperty =
      AvaloniaProperty.Register<Button, ButtonType>(nameof(ButtonType), ButtonType.Default);

   public static readonly StyledProperty<ButtonShape> ButtonShapeProperty =
      AvaloniaProperty.Register<Button, ButtonShape>(nameof(Shape), ButtonShape.Default);

   public static readonly StyledProperty<bool> IsDangerProperty =
      AvaloniaProperty.Register<Button, bool>(nameof(IsDanger), false);

   public static readonly StyledProperty<bool> IsGhostProperty =
      AvaloniaProperty.Register<Button, bool>(nameof(IsGhost), false);

   public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<Button, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);

   public static readonly StyledProperty<PathIcon?> IconProperty
      = AvaloniaProperty.Register<Button, PathIcon?>(nameof(Icon));

   public static readonly StyledProperty<string?> TextProperty
      = AvaloniaProperty.Register<Button, string?>(nameof(Text));

   public ButtonType ButtonType
   {
      get => GetValue(ButtonTypeProperty);
      set => SetValue(ButtonTypeProperty, value);
   }

   public ButtonShape Shape
   {
      get => GetValue(ButtonShapeProperty);
      set => SetValue(ButtonShapeProperty, value);
   }
   
   public bool IsDanger
   {
      get => GetValue(IsDangerProperty);
      set => SetValue(IsDangerProperty, value);
   }
   
   public bool IsGhost
   {
      get => GetValue(IsGhostProperty);
      set => SetValue(IsGhostProperty, value);
   }
   
   public ButtonSizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public PathIcon? Icon
   {
      get => GetValue(IconProperty);
      set => SetValue(IconProperty, value);
   }
   
   public string? Text
   {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
   }

   #endregion

   static Button()
   {
      AffectsMeasure<Button>(SizeTypeProperty,
                             ButtonShapeProperty,
                             IconProperty,
                             WidthProperty,
                             HeightProperty);
      AffectsRender<Button>(ButtonTypeProperty,
                            IsDangerProperty,
                            IsGhostProperty);
   }

   public Button()
   {
      _controlTokenBinder = new ControlTokenBinder(this, ButtonToken.ID);
      _customStyle = this;
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = Math.Max(size.Height, _controlHeight);
      if (Shape == ButtonShape.Circle) {
         targetWidth = targetHeight;
         CornerRadius = new CornerRadius(targetHeight);
      } else if (Shape == ButtonShape.Round) {
         CornerRadius = new CornerRadius(targetHeight);
      }

      return new Size(targetWidth, targetHeight);
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _customStyle.ApplyRenderScalingAwareStyleConfig();
   }
}