using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;

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

public partial class OptionButton : AvaloniaRadioButton, 
                                    ISizeTypeAware,
                                    IWaveAdornerInfoProvider, 
                                    IControlCustomStyle
{
   private bool _initialized = false;
   private ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private StackPanel? _stackPanel;
   private Label? _label;
   private CornerRadius? _originCornerRadius;
   private readonly BorderRenderHelper _borderRenderHelper;
   
   public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<OptionButton, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);
   
   public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
      AvaloniaProperty.Register<OptionButton, OptionButtonStyle>(nameof(ButtonStyle), OptionButtonStyle.Outline);
   
   public static readonly StyledProperty<string?> TextProperty
      = AvaloniaProperty.Register<OptionButton, string?>(nameof(Text));

   internal static readonly DirectProperty<OptionButton, bool> InOptionGroupProperty =
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
   
   public string? Text
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
      AffectsMeasure<OptionButton>(SizeTypeProperty, ButtonStyleProperty, InOptionGroupProperty);
      AffectsRender<OptionButton>(IsCheckedProperty, CornerRadiusProperty, ForegroundProperty, BackgroundProperty);
   }
   
   public OptionButton()
   {
      _customStyle = this;
      _borderRenderHelper = new BorderRenderHelper();
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = size.Height;
      targetHeight += Padding.Top + Padding.Bottom;
      targetWidth += Padding.Left + Padding.Right;
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
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _customStyle.SetupTransitions();
   }

   private void HandleSizeTypeChanged()
   {
      _originCornerRadius = CornerRadius;
      CornerRadius = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
   }
   
   #region IControlCustomStyle 实现
   public Rect WaveGeometry()
   {
      return new Rect(0, 0, Bounds.Width, Bounds.Height);
   }

   public CornerRadius WaveBorderRadius()
   {
      return CornerRadius;
   }

   void IControlCustomStyle.SetupUi()
   {
      if (Text is null && Content is string content) {
         Text = content;
      }
      
      Cursor = new Cursor(StandardCursorType.Hand);
      HandleSizeTypeChanged();
      _customStyle.CollectStyleState();
   }
   
   void IControlCustomStyle.SetupTransitions()
   {
      var transitions = new Transitions();
      if (ButtonStyle == OptionButtonStyle.Solid) {
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
      } else if (ButtonStyle == OptionButtonStyle.Outline) {
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
      }
      
      transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
      Transitions = transitions;
   }

   void IControlCustomStyle.CollectStyleState()
   {
      ControlStateUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }

      if (IsChecked.HasValue && IsChecked.Value) {
         _styleState |= ControlStyleState.Selected;
      }
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPointerOverProperty ||
          e.Property == IsPressedProperty ||
          e.Property == IsCheckedProperty) {
         _customStyle.CollectStyleState();
         if (e.Property == IsPressedProperty) {
            if (_styleState.HasFlag(ControlStyleState.Raised)) {
               WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
            }
         }
      }

      if (e.Property == GroupPositionTraitProperty) {
         if (_originCornerRadius.HasValue) {
            CornerRadius = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
         }
      }
   }

   private CornerRadius BuildCornerRadius(OptionButtonPositionTrait positionTrait, CornerRadius cornerRadius)
   {
      if (positionTrait == OptionButtonPositionTrait.First) {
         return new CornerRadius(cornerRadius.TopLeft,
                                 0,
                                 0,
                                 cornerRadius.BottomLeft);
      } else if (positionTrait == OptionButtonPositionTrait.Last) {
         return new CornerRadius(0,
                                 cornerRadius.TopRight,
                                 cornerRadius.BottomRight,
                                 0);
      } else if (positionTrait == OptionButtonPositionTrait.Middle) {
         return new CornerRadius(0);
      }

      return cornerRadius;
   }
 
   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      base.OnPointerPressed(e);
      OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
      {
         IsHovering = true,
         IsPressed = true
      });
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      base.OnPointerReleased(e);
      OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
      {
         IsHovering = true,
         IsPressed = false
      });
   }

   protected override void OnPointerEntered(PointerEventArgs e)
   {
      base.OnPointerEntered(e);
      OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
      {
         IsHovering = true,
         IsPressed = false
      });
   }

   protected override void OnPointerExited(PointerEventArgs e)
   {
      base.OnPointerExited(e);
      OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
      {
         IsHovering = false,
         IsPressed = false
      });
   }

   public override void Render(DrawingContext context)
   {
      _borderRenderHelper.Render(context, 
                                 Bounds.Size, 
                                 BorderThickness, 
                                 CornerRadius, 
                                 BackgroundSizing.InnerBorderEdge, 
                                 Background,
                                 BorderBrush,
                                 default);
   }

   #endregion

}