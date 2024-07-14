using System.Collections.Specialized;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

using ButtonSizeType = SizeType;
using OptionButtons = AvaloniaList<OptionButton>;

public partial class OptionButtonGroup : StyledControl,
                                         ISizeTypeAware,
                                         IControlCustomStyle
{
   public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<OptionButtonGroup, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);

   public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
      AvaloniaProperty.Register<OptionButtonGroup, OptionButtonStyle>(nameof(SizeType), OptionButtonStyle.Outline);
   
   /// <summary>
   /// Defines the <see cref="CornerRadius"/> property.
   /// </summary>
   public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
      AvaloniaProperty.Register<OptionButtonGroup, CornerRadius>(nameof(CornerRadius));
   
   /// <summary>
   /// Defines the <see cref="BorderThickness"/> property.
   /// </summary>
   public static readonly StyledProperty<Thickness> BorderThicknessProperty =
      AvaloniaProperty.Register<StyledControl, Thickness>(nameof(BorderThickness));
   
   
   public static readonly DirectProperty<OptionButtonGroup, OptionButton?> SelectedOptionProperty =
      AvaloniaProperty.RegisterDirect<OptionButtonGroup, OptionButton?>(nameof(SelectedOption),
         o => o.SelectedOption,
         (o, v) => o.SelectedOption = v);
   
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
   
   /// <summary>
   /// Gets or sets the radius of the border rounded corners.
   /// </summary>
   public CornerRadius CornerRadius
   {
      get => GetValue(CornerRadiusProperty);
      set => SetValue(CornerRadiusProperty, value);
   }
   
   /// <summary>
   /// Gets or sets the thickness of the border.
   /// </summary>
   public Thickness BorderThickness
   {
      get => GetValue(BorderThicknessProperty);
      set => SetValue(BorderThicknessProperty, value);
   }

   private OptionButton? _optionButton;
   public OptionButton? SelectedOption
   {
      get => _optionButton;
      set => SetAndRaise(SelectedOptionProperty, ref _optionButton, value);
   }

   [Content] public OptionButtons Options { get; } = new OptionButtons();
   
   private bool _initialized = false;
   private ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private StackPanel? _layout;
   private readonly BorderRenderHelper _borderRenderHelper = new BorderRenderHelper();

   static OptionButtonGroup()
   {
      AffectsMeasure<OptionButtonGroup>(SizeTypeProperty);
      AffectsRender<OptionButtonGroup>(SelectedOptionProperty, SelectedOptionBorderColorProperty,
         ButtonStyleProperty);
   }

   public OptionButtonGroup()
   {
      _controlTokenBinder = new ControlTokenBinder(this, OptionButtonToken.ID);
      _customStyle = this;
      _customStyle.InitOnConstruct();
      Options.CollectionChanged += OptionsChanged;
   }

   protected virtual void OptionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
   {
      switch (e.Action) {
         case NotifyCollectionChangedAction.Add:
            var newOptions = e.NewItems!.OfType<OptionButton>().ToList();
            ApplyInButtonGroupFlag(newOptions, true);
            _layout!.Children.AddRange(newOptions);
            break;

         case NotifyCollectionChangedAction.Move:
            _layout!.Children.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
            break;

         case NotifyCollectionChangedAction.Remove:
            var removedOptions = e.OldItems!.OfType<OptionButton>().ToList();
            ApplyInButtonGroupFlag(removedOptions, false);
            _layout!.Children.RemoveAll(removedOptions);
            break;

         case NotifyCollectionChangedAction.Replace:
            for (var i = 0; i < e.OldItems!.Count; ++i) {
               var index = i + e.OldStartingIndex;
               var oldChild = (OptionButton)e.OldItems![i]!;
               oldChild.InOptionGroup = false;
               var child = (OptionButton)e.NewItems![i]!;
               child.InOptionGroup = true;
               _layout!.Children[index] = child;
            }
            break;

         case NotifyCollectionChangedAction.Reset:
            throw new NotSupportedException();
      }

      UpdateOptionButtonsPosition();
      InvalidateMeasureOnOptionsChanged();
   }

   private void UpdateOptionButtonsPosition()
   {
      for (int i = 0; i < Options.Count; i++) {
         var button = Options[i];
         if (Options.Count > 1) {
            if (i == 0) {
               button.GroupPositionTrait = OptionButtonPositionTrait.First;
            } else if (i == Options.Count - 1) {
               button.GroupPositionTrait = OptionButtonPositionTrait.Last;
            } else {
               button.GroupPositionTrait = OptionButtonPositionTrait.Middle;
            }
         }
      }
   }

   private void ApplyInButtonGroupFlag(List<OptionButton> buttons, bool inGroup)
   {
      for (int i = 0; i < buttons.Count; i++) {
         var button = buttons[i];
         button.InOptionGroup = inGroup;
         if (inGroup) {
            button.IsCheckedChanged += HandleOptionSelected;
            button.OptionButtonPointerEvent += HandleOptionPointerEvent;
         } else {
            button.IsCheckedChanged -= HandleOptionSelected;
            button.OptionButtonPointerEvent -= HandleOptionPointerEvent;
            button.GroupPositionTrait = OptionButtonPositionTrait.OnlyOne;
         }
      }
   }

   private void HandleOptionSelected(object? sender, RoutedEventArgs args)
   {
      if (sender is OptionButton optionButton) {
         if (optionButton.IsChecked.HasValue && optionButton.IsChecked.Value) {
            SelectedOption = optionButton;
         }
      }
   }

   private protected virtual void InvalidateMeasureOnOptionsChanged()
   {
      InvalidateMeasure();
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
   
   #region IControlCustomStyle 实现
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
   #endregion
}