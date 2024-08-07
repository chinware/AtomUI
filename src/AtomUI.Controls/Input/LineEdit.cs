using AtomUI.Controls.Utils;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;

namespace AtomUI.Controls;

public enum TextBoxVariant
{
   Outline,
   Filled,
   Borderless
}

public enum TextBoxStatus
{
   Default,
   Warning,
   Error
}

public class LineEdit : TextBox
{
   public const string ErrorPC = ":error";
   public const string WarningPC = ":warning";
   #region 功能属性定义

   public static readonly StyledProperty<object?> LeftAddOnProperty =
      AvaloniaProperty.Register<LineEdit, object?>(nameof(LeftAddOn));

   public static readonly StyledProperty<object?> RightAddOnProperty =
      AvaloniaProperty.Register<LineEdit, object?>(nameof(RightAddOn));
   
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<LineEdit, SizeType>(nameof(SizeType), SizeType.Middle);
   
   public static readonly StyledProperty<TextBoxVariant> StyleVariantProperty =
      AvaloniaProperty.Register<LineEdit, TextBoxVariant>(nameof(StyleVariant), TextBoxVariant.Outline);
   
   public static readonly StyledProperty<TextBoxStatus> StatusProperty =
      AvaloniaProperty.Register<LineEdit, TextBoxStatus>(nameof(Status), TextBoxStatus.Default);
   
   public static readonly StyledProperty<bool> IsEnableClearButtonProperty =
      AvaloniaProperty.Register<LineEdit, bool>(nameof(IsEnableClearButton), false);
   
   public static readonly StyledProperty<bool> IsEnableRevealButtonProperty =
      AvaloniaProperty.Register<LineEdit, bool>(nameof(IsEnableRevealButton), false);
   
   public object? LeftAddOn
   {
      get => GetValue(LeftAddOnProperty);
      set => SetValue(LeftAddOnProperty, value);
   }
   
   public object? RightAddOn
   {
      get => GetValue(RightAddOnProperty);
      set => SetValue(RightAddOnProperty, value);
   }
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public TextBoxVariant StyleVariant
   {
      get => GetValue(StyleVariantProperty);
      set => SetValue(StyleVariantProperty, value);
   }
   
   public TextBoxStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }
   
   public bool IsEnableClearButton
   {
      get => GetValue(IsEnableClearButtonProperty);
      set => SetValue(IsEnableClearButtonProperty, value);
   }
   
   public bool IsEnableRevealButton
   {
      get => GetValue(IsEnableRevealButtonProperty);
      set => SetValue(IsEnableRevealButtonProperty, value);
   }
   
   #endregion

   #region 内部属性定义

   internal static readonly DirectProperty<LineEdit, CornerRadius> EditKernelCornerRadiusProperty =
      AvaloniaProperty.RegisterDirect<LineEdit, CornerRadius>(nameof(EditKernelCornerRadius),
                                                              o => o.EditKernelCornerRadius,
                                                              (o, v) => o.EditKernelCornerRadius = v);
   
   internal static readonly DirectProperty<LineEdit, CornerRadius> LeftAddOnCornerRadiusProperty =
      AvaloniaProperty.RegisterDirect<LineEdit, CornerRadius>(nameof(LeftAddOnCornerRadius), 
                                                              o => o.LeftAddOnCornerRadius,
                                                              (o, v) => o.LeftAddOnCornerRadius = v);
   
   internal static readonly DirectProperty<LineEdit, CornerRadius> RightAddOnCornerRadiusProperty =
      AvaloniaProperty.RegisterDirect<LineEdit, CornerRadius>(nameof(RightAddOnCornerRadius),
                                                              o => o.RightAddOnCornerRadius,
                                                              (o, v) => o.RightAddOnCornerRadius = v);
   
   internal static readonly DirectProperty<LineEdit, Thickness> LeftAddOnBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<LineEdit, Thickness>(nameof(LeftAddOnBorderThickness),
                                                           o => o.LeftAddOnBorderThickness,
                                                           (o, v) => o.LeftAddOnBorderThickness = v);
   
   internal static readonly DirectProperty<LineEdit, Thickness> RightAddOnBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<LineEdit, Thickness>(nameof(RightAddOnBorderThickness),
                                                           o => o.RightAddOnBorderThickness,
                                                           (o, v) => o.RightAddOnBorderThickness = v);
   
   internal static readonly DirectProperty<LineEdit, bool> IsEffectiveShowClearButtonProperty =
      AvaloniaProperty.RegisterDirect<LineEdit, bool>(nameof(IsEffectiveShowClearButton),
                                                      o => o.IsEffectiveShowClearButton,
                                                      (o, v) => o.IsEffectiveShowClearButton = v);

   private CornerRadius _editKernelCornerRadius;
   internal CornerRadius EditKernelCornerRadius
   {
      get => _editKernelCornerRadius;
      set => SetAndRaise(EditKernelCornerRadiusProperty, ref _editKernelCornerRadius, value);
   }
   
   private CornerRadius _leftAddOnCornerRadius;
   internal CornerRadius LeftAddOnCornerRadius
   {
      get => _leftAddOnCornerRadius;
      set => SetAndRaise(LeftAddOnCornerRadiusProperty, ref _leftAddOnCornerRadius, value);
   }

   private CornerRadius _rightAddOnCornerRadius;
   internal CornerRadius RightAddOnCornerRadius
   {
      get => _rightAddOnCornerRadius;
      set => SetAndRaise(RightAddOnCornerRadiusProperty, ref _rightAddOnCornerRadius, value);
   }

   private Thickness _leftAddOnBorderThickness;
   internal Thickness LeftAddOnBorderThickness
   {
      get => _leftAddOnBorderThickness;
      set => SetAndRaise(LeftAddOnBorderThicknessProperty, ref _leftAddOnBorderThickness, value);
   }

   private Thickness _rightAddOnBorderThickness;
   internal Thickness RightAddOnBorderThickness
   {
      get => _rightAddOnBorderThickness;
      set => SetAndRaise(RightAddOnBorderThicknessProperty, ref _rightAddOnBorderThickness, value);
   }
   
   private bool _isEffectiveShowClearButton;
   internal bool IsEffectiveShowClearButton
   {
      get => _isEffectiveShowClearButton;
      set => SetAndRaise(IsEffectiveShowClearButtonProperty, ref _isEffectiveShowClearButton, value);
   }
   
   #endregion
   
   private ContentPresenter? _leftAddOnPresenter;
   private ContentPresenter? _rightAddOnPresenter;
   private Border? _lineEditKernelDecorator;


   static LineEdit()
   {
      AffectsRender<LineEdit>(BorderBrushProperty, BackgroundProperty);
      AffectsMeasure<LineEdit>(LeftAddOnProperty, RightAddOnProperty);
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalResourceBinding(this, BorderThicknessProperty, GlobalResourceKey.BorderThickness, 
                                                      BindingPriority.Template, new RenderScaleAwareThicknessConfigure(this));
      _leftAddOnPresenter = e.NameScope.Find<ContentPresenter>(LineEditTheme.LeftAddOnPart);
      _rightAddOnPresenter = e.NameScope.Find<ContentPresenter>(LineEditTheme.RightAddOnPart);
      _lineEditKernelDecorator = e.NameScope.Find<Border>(LineEditTheme.LineEditKernelDecoratorPart);
      SetupEditKernelCornerRadius();
      SetupEffectiveShowClearButton();
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);

      if (VisualRoot is not null) {
         if (change.Property == LeftAddOnProperty || change.Property == RightAddOnProperty) {
            SetupEditKernelCornerRadius();
         }
      }

      if (change.Property == CornerRadiusProperty || change.Property == BorderThicknessProperty) {
         SetupAddOnBorderInfo();
      }

      if (change.Property == AcceptsReturnProperty ||
          change.Property == IsReadOnlyProperty ||
          change.Property == TextProperty ||
          change.Property == IsEnableClearButtonProperty) {
         SetupEffectiveShowClearButton();
      }
      
      if (change.Property == StatusProperty) {
         UpdatePseudoClasses();
      }

      if (change.Property == InnerLeftContentProperty || 
          change.Property == InnerRightContentProperty) {
         if (change.OldValue is Control oldControl) {
            UIStructureUtils.SetTemplateParent(oldControl, null);
         }

         if (change.NewValue is Control newControl) {
            UIStructureUtils.SetTemplateParent(newControl, this);
         }
      }
   }

   private void SetupAddOnBorderInfo()
   {
      var topLeftRadius = CornerRadius.TopLeft;
      var topRightRadius = CornerRadius.TopRight;
      var bottomLeftRadius = CornerRadius.BottomLeft;
      var bottomRightRadius = CornerRadius.BottomRight;
      
      var topThickness = BorderThickness.Top;
      var rightThickness = BorderThickness.Right;
      var bottomThickness = BorderThickness.Bottom;
      var leftThickness = BorderThickness.Left;

      LeftAddOnCornerRadius = new CornerRadius(topLeft: topLeftRadius,
                                               topRight: 0,
                                               bottomLeft:bottomLeftRadius,
                                               bottomRight:0);
      RightAddOnCornerRadius = new CornerRadius(topLeft: 0,
                                                topRight: topRightRadius,
                                                bottomLeft:0,
                                                bottomRight:bottomRightRadius);

      LeftAddOnBorderThickness = new Thickness(top: topThickness, right:0, bottom:bottomThickness, left: leftThickness);
      RightAddOnBorderThickness = new Thickness(top: topThickness, right:rightThickness, bottom:bottomThickness, left: 0);
   }

   private void SetupEditKernelCornerRadius()
   {
      var topLeftRadius = CornerRadius.TopLeft;
      var topRightRadius = CornerRadius.TopRight;
      var bottomLeftRadius = CornerRadius.BottomLeft;
      var bottomRightRadius = CornerRadius.BottomRight;

      if (_leftAddOnPresenter is not null && _leftAddOnPresenter.IsVisible) {
         topLeftRadius = 0;
         bottomLeftRadius = 0;
      }
      if (_rightAddOnPresenter is not null && _rightAddOnPresenter.IsVisible) {
         topRightRadius = 0;
         bottomRightRadius = 0;
      }

      EditKernelCornerRadius = new CornerRadius(topLeft: topLeftRadius,
                                                topRight: topRightRadius,
                                                bottomLeft:bottomLeftRadius,
                                                bottomRight:bottomRightRadius);
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      return base.MeasureOverride(new Size(availableSize.Width - BorderThickness.Left - BorderThickness.Right, availableSize.Height));
   }
   
   protected override Size ArrangeOverride(Size finalSize)
   {
      return base.ArrangeOverride(new Size(finalSize.Width - BorderThickness.Left - BorderThickness.Right, finalSize.Height));
   }

   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      if (_lineEditKernelDecorator is null) {
         return;
      }
      var targetRect = new Rect(_lineEditKernelDecorator.DesiredSize);
      if (!targetRect.Contains(e.GetPosition(_lineEditKernelDecorator))) {
         return;
      }
      base.OnPointerPressed(e);
   }

   private void SetupEffectiveShowClearButton()
   {
      if (!IsEnableClearButton) {
         IsEffectiveShowClearButton = false;
         return;
      }

      IsEffectiveShowClearButton = !IsReadOnly && !AcceptsReturn && !string.IsNullOrEmpty(Text);
   }
   
   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(ErrorPC, Status == TextBoxStatus.Error);
      PseudoClasses.Set(WarningPC, Status == TextBoxStatus.Warning);
   }
}