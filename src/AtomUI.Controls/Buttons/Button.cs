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

public partial class Button : AvaloniaButton, ITokenIdProvider, ISizeTypeAware
{
   // 需要改造
   public static readonly DirectProperty<Button, ButtonType> ButtonTypeProperty =
      AvaloniaProperty.RegisterDirect<Button, ButtonType>(nameof(ButtonType),
                                                          o => o.ButtonType,
                                                          (o, v) => o.ButtonType = v,
                                                          ButtonType.Default);

   public static readonly DirectProperty<Button, ButtonShape> ButtonShapeProperty =
      AvaloniaProperty.RegisterDirect<Button, ButtonShape>(nameof(Shape),
                                                           o => o.Shape,
                                                           (o, v) => o.Shape = v,
                                                           ButtonShape.Default);

   public static readonly DirectProperty<Button, bool> IsDangerProperty =
      AvaloniaProperty.RegisterDirect<Button, bool>(nameof(IsDanger),
                                                    o => o.IsDanger,
                                                    (o, v) => o.IsDanger = v,
                                                    false);

   public static readonly DirectProperty<Button, bool> IsGhostProperty =
      AvaloniaProperty.RegisterDirect<Button, bool>(nameof(IsGhost),
                                                    o => o.IsGhost,
                                                    (o, v) => o.IsGhost = v,
                                                    false);

   public static readonly DirectProperty<Button, ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.RegisterDirect<Button, ButtonSizeType>(nameof(SizeType),
                                                              o => o.SizeType,
                                                              (o, v) => o.SizeType = v,
                                                              ButtonSizeType.Middle);

   public static readonly DirectProperty<Button, PathIcon?> IconProperty
      = AvaloniaProperty.RegisterDirect<Button, PathIcon?>(nameof(Icon),
                                                           o => o.Icon,
                                                           (o, v) => o.Icon = v);

   public static readonly DirectProperty<Button, string> TextProperty
      = AvaloniaProperty.RegisterDirect<Button, string>(nameof(Text),
                                                        o => o.Text,
                                                        (o, v) => o.Text = v,
                                                        string.Empty);

   private ButtonType _buttonType = ButtonType.Default;

   public ButtonType ButtonType
   {
      get => _buttonType;
      set => SetAndRaise(ButtonTypeProperty, ref _buttonType, value);
   }

   private ButtonShape _shape = ButtonShape.Default;

   public ButtonShape Shape
   {
      get => _shape;
      set => SetAndRaise(ButtonShapeProperty, ref _shape, value);
   }

   private bool _isDanger = false;

   public bool IsDanger
   {
      get => _isDanger;
      set => SetAndRaise(IsDangerProperty, ref _isDanger, value);
   }

   private bool _isGhost = false;

   public bool IsGhost
   {
      get => _isGhost;
      set => SetAndRaise(IsGhostProperty, ref _isGhost, value);
   }

   private ButtonSizeType _sizeType = ButtonSizeType.Middle;

   public ButtonSizeType SizeType
   {
      get => _sizeType;
      set => SetAndRaise(SizeTypeProperty, ref _sizeType, value);
   }

   private PathIcon? _icon;

   public PathIcon? Icon
   {
      get => _icon;
      set => SetAndRaise(IconProperty, ref _icon, value);
   }

   private string _text = string.Empty;

   public string Text
   {
      get => _text;
      set => SetAndRaise(TextProperty, ref _text, value);
   }

   string ITokenIdProvider.TokenId => nameof(Button);

   static Button()
   {
      AffectsMeasure<Button>(SizeTypeProperty, ButtonShapeProperty, IconProperty);
      AffectsRender<Button>(ButtonTypeProperty,
                            IsDangerProperty,
                            IsGhostProperty);
   }

   public Button()
   {
      _tokenResourceBinder = new TokenResourceBinder(this);
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