using AtomUI.Data;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

using ButtonSizeType = SizeType;

public enum OptionButtonStyle
{
   Outline,
   Solid
}

public enum OptionButtonPositionTrait
{
   First,
   Last,
   Middle,
   OnlyOne
}

public class OptionButtonPointerEventArgs : EventArgs
{
   public OptionButton? Button { get; }
   public bool IsPressed { get; set; }
   public bool IsHovering { get; set; }
   public OptionButtonPointerEventArgs(OptionButton button)
   {
      Button = button;
   }
}

public partial class OptionButton : AvaloniaRadioButton, ITokenIdProvider, ISizeTypeAware
{
   string ITokenIdProvider.TokenId => OptionButtonToken.ID;
   
   public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<OptionButton, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);
   
   public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
      AvaloniaProperty.Register<OptionButton, OptionButtonStyle>(nameof(SizeType), OptionButtonStyle.Outline);
   
   public static readonly StyledProperty<string> TextProperty
      = AvaloniaProperty.Register<OptionButton, string>(nameof(Text), string.Empty);
   
   private static readonly DirectProperty<OptionButton, bool> InOptionGroupProperty =
      AvaloniaProperty.RegisterDirect<OptionButton, bool>(
         nameof(InOptionGroup),
         o => o.InOptionGroup,
         (o, v) => o.InOptionGroup = v);
   
   private static readonly DirectProperty<OptionButton, OptionButtonPositionTrait> GroupPositionTraitProperty =
      AvaloniaProperty.RegisterDirect<OptionButton, OptionButtonPositionTrait>(
         nameof(GroupPositionTrait),
         o => o.GroupPositionTrait,
         (o, v) => o.GroupPositionTrait = v,
         OptionButtonPositionTrait.OnlyOne);
   
   public ButtonSizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public OptionButtonStyle ButtonStyle
   {
      get => GetValue(ButtonStyleProperty);
      set => SetValue(ButtonStyleProperty, value);
   }
   
   public string Text
   {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
   }

   private bool _inOptionGroup = false;
   
   /// <summary>
   /// 是否在 Group 中渲染
   /// </summary>
   internal bool InOptionGroup
   {
      get => _inOptionGroup;
      set => SetAndRaise(InOptionGroupProperty, ref _inOptionGroup, value);
   }

   private OptionButtonPositionTrait _groupPositionTrait = OptionButtonPositionTrait.OnlyOne;
   internal OptionButtonPositionTrait GroupPositionTrait
   {
      get => _groupPositionTrait;
      set => SetAndRaise(GroupPositionTraitProperty, ref _groupPositionTrait, value);
   }
   
   internal event EventHandler<OptionButtonPointerEventArgs>? OptionButtonPointerEvent;
   
   static OptionButton()
   {
      AffectsMeasure<Button>(SizeTypeProperty, ButtonStyleProperty, InOptionGroupProperty);
      AffectsRender<Button>(IsCheckedProperty, CornerRadiusProperty);
   }
   
   public OptionButton()
   {
      _controlTokenBinder = new ControlTokenBinder(this);
      _customStyle = this;
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = Math.Max(size.Height, _controlHeight);
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