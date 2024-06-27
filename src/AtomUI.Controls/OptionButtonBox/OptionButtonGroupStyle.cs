using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class OptionButtonGroup : IControlCustomStyle
{
   private bool _initialized = false;
   private ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private StackPanel? _layout;
   private readonly BorderRenderHelper _borderRenderHelper = new BorderRenderHelper();

   void IControlCustomStyle.InitOnConstruct()
   {
      _layout = new StackPanel
      {
         Orientation = Orientation.Horizontal,
         ClipToBounds = true,
      };
   }

   void IControlCustomStyle.SetupUi()
   {
      HorizontalAlignment = HorizontalAlignment.Left;
      _customStyle.CollectStyleState();
      _customStyle.ApplySizeTypeStyleConfig();
      ApplyButtonSizeConfig();
      ApplyButtonStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.ApplyFixedStyleConfig();
      
      LogicalChildren.Add(_layout!);
      VisualChildren.Add(_layout!);
   }
   
   void IControlCustomStyle.SetupTransitions() {}

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(MotionDurationTokenProperty, GlobalResourceKey.MotionDurationMid);
      _controlTokenBinder.AddControlBinding(ColorBorderTokenProperty, GlobalResourceKey.ColorBorder);
      _controlTokenBinder.AddControlBinding(ColorPrimaryTokenProperty, GlobalResourceKey.ColorPrimary);
      _controlTokenBinder.AddControlBinding(ColorPrimaryHoverTokenProperty, GlobalResourceKey.ColorPrimaryHover);
      _controlTokenBinder.AddControlBinding(ColorPrimaryActiveTokenProperty, GlobalResourceKey.ColorPrimaryActive);
      _controlTokenBinder.AddControlBinding(SelectedOptionBorderColorProperty, GlobalResourceKey.ColorPrimary);
   }

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(BorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style,
         new RenderScaleAwareThicknessConfigure(this));
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == SizeTypeProperty) {
         _customStyle.ApplySizeTypeStyleConfig();
      } else if (e.Property == ButtonStyleProperty) {
         ApplyButtonStyleConfig();
      }
   }
   
   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Small) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
         _controlTokenBinder.AddControlBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightSM);
      } else if (SizeType == SizeType.Middle) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadius);
         _controlTokenBinder.AddControlBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
      } else if (SizeType == SizeType.Large) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
         _controlTokenBinder.AddControlBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightLG);
      }

      ApplyButtonSizeConfig();
   }

   private void ApplyButtonSizeConfig()
   {
      foreach (var optionButton in Options) {
         optionButton.SizeType = SizeType;
      }
   }

   private void ApplyButtonStyleConfig()
   {
      foreach (var optionButton in Options) {
         optionButton.ButtonStyle = ButtonStyle;
      }
   }
   
   public override void Render(DrawingContext context)
   {
      _borderRenderHelper.Render(context, 
         new Size(DesiredSize.Width, DesiredSize.Height), 
         BorderThickness, 
         CornerRadius, 
         BackgroundSizing.InnerBorderEdge, 
         null,
         _colorBorder,
         new BoxShadows());
      for (int i = 0; i < Options.Count; ++i) {
         var optionButton = Options[i];
         if (ButtonStyle == OptionButtonStyle.Solid) {
            if (i <= Options.Count - 2) {
               var nextOption = Options[i + 1];
               if (nextOption == SelectedOption || optionButton == SelectedOption) {
                  continue;
               }
            }
         }
         if (i != Options.Count - 1) {
            var offsetX = optionButton.Bounds.Right;
            var startPoint = new Point(offsetX, 0);
            var endPoint = new Point(offsetX, Bounds.Height);
            using var optionState = context.PushRenderOptions(new RenderOptions()
            {
               EdgeMode = EdgeMode.Aliased
            });
            context.DrawLine(new Pen(_colorBorder, BorderThickness.Left), startPoint, endPoint);
         }

         if (ButtonStyle == OptionButtonStyle.Outline) {
            if (optionButton.IsEnabled && optionButton.IsChecked.HasValue && optionButton.IsChecked.Value) {
               // 绘制选中边框
               var offsetX = optionButton.Bounds.X;
               var width = optionButton.DesiredSize.Width;
               if (i != 0) {
                  offsetX -= BorderThickness.Left;
                  width += BorderThickness.Left;
               }
               var translationMatrix = Matrix.CreateTranslation(offsetX, 0);
               using var state = context.PushTransform(translationMatrix);
               var cornerRadius = new CornerRadius(0);
               if (i == 0) {
                  cornerRadius = new CornerRadius(CornerRadius.TopLeft, 0, 0, CornerRadius.BottomLeft);
               } else if (i == Options.Count - 1) {
                  cornerRadius = new CornerRadius(0, CornerRadius.TopRight, CornerRadius.BottomRight, 0);
               }
               _borderRenderHelper.Render(context, 
                  new Size(width, DesiredSize.Height), 
                  BorderThickness, 
                  cornerRadius, 
                  BackgroundSizing.InnerBorderEdge, 
                  null,
                  SelectedOptionBorderColor,
                  new BoxShadows());
            }
         }
      }
   }
   
   private void HandleOptionPointerEvent(object? sender, OptionButtonPointerEventArgs args)
   {
      if (args.Button == SelectedOption) {
         _controlTokenBinder.ReleaseTriggerBindings(this);
         if (args.IsPressed) {
            _controlTokenBinder.AddControlBinding(SelectedOptionBorderColorProperty, GlobalResourceKey.ColorPrimaryActive, BindingPriority.StyleTrigger);
         } else if (args.IsHovering) {
            _controlTokenBinder.AddControlBinding(SelectedOptionBorderColorProperty, GlobalResourceKey.ColorPrimaryHover, BindingPriority.StyleTrigger);
         }
      }
   }
}