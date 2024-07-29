using AtomUI.Controls.Utils;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Metadata;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Pressed, StdPseudoClass.Selected)]
public class SegmentedItem : TemplatedControl, IControlCustomStyle
{
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      Segmented.SizeTypeProperty.AddOwner<SegmentedItem>();

   public static readonly StyledProperty<string?> TextProperty
      = AvaloniaProperty.Register<SegmentedItem, string?>(nameof(Text));

   public static readonly StyledProperty<PathIcon?> IconProperty
      = AvaloniaProperty.Register<SegmentedItem, PathIcon?>(nameof(Icon));

   public static readonly DirectProperty<SegmentedItem, bool> IsPressedProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItem, bool>(nameof(IsPressed), o => o.IsPressed);

   public static readonly DirectProperty<SegmentedItem, bool> IsCurrentItemProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItem, bool>(nameof(IsCurrentItem),
                                                           o => o.IsCurrentItem,
                                                           (o, v) => o.IsCurrentItem = v);

   internal SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   [Content]
   public string? Text
   {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
   }

   public PathIcon? Icon
   {
      get => GetValue(IconProperty);
      set => SetValue(IconProperty, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether the button is currently pressed.
   /// </summary>
   public bool IsPressed
   {
      get => _isPressed;
      private set => SetAndRaise(IsPressedProperty, ref _isPressed, value);
   }

   // 内部属性
   private bool _isCurrentItem = false;

   internal bool IsCurrentItem
   {
      get => _isCurrentItem;
      set => SetAndRaise(IsCurrentItemProperty, ref _isCurrentItem, value);
   }
   
   private IControlCustomStyle _customStyle;
   private StackPanel? _mainLayout;
   private bool _isPressed = false;
   private ControlStyleState _styleState;

   static SegmentedItem()
   {
      AffectsRender<SegmentedItem>(IsPressedProperty, FontSizeProperty, IsCurrentItemProperty);
      AffectsMeasure<SegmentedItem>(TextProperty, IconProperty, SizeTypeProperty, FontSizeProperty);
   }

   public SegmentedItem()
   {
      _customStyle = this;
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      base.OnPointerPressed(e);
      if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) {
         IsPressed = true;
      }
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      base.OnPointerReleased(e);

      if (IsPressed && e.InitialPressMouseButton == MouseButton.Left) {
         IsPressed = false;
      }
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _mainLayout = scope.Find<StackPanel>(SegmentedItemTheme.MainLayoutPart);
      
      HorizontalAlignment = HorizontalAlignment.Left;
      VerticalAlignment = VerticalAlignment.Center;

      _customStyle.SetupTokenBindings();
      SetupItemIcon();
      _customStyle.SetupTransitions();
      _customStyle.CollectStyleState();
   }

   #region IControlCustomStyle 实现
   
   void IControlCustomStyle.UpdatePseudoClasses()
   {
      PseudoClasses.Set(StdPseudoClass.Pressed, IsPressed);
      PseudoClasses.Set(StdPseudoClass.Selected, IsCurrentItem);
   }

   void IControlCustomStyle.SetupTransitions()
   {
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty)
      };
   }

   void IControlCustomStyle.CollectStyleState()
   {
      ControlStateUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }
      if (IsCurrentItem) {
         _styleState |= ControlStyleState.Selected;
      }
      _customStyle.UpdatePseudoClasses();
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (VisualRoot is not null) {
         if (e.Property == IsPointerOverProperty ||
             e.Property == IsPressedProperty ||
             e.Property == IsCurrentItemProperty) {
            _customStyle.CollectStyleState();
         } else if (e.Property == IconProperty) {
            SetupItemIcon();
         }
      }
   }
   
   private void SetupItemIcon()
   {
      if (Icon is not null) {
         if (Icon.ThemeType != IconThemeType.TwoTone) {
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.NormalFilledBrushProperty, SegmentedResourceKey.ItemColor);
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty, SegmentedResourceKey.ItemHoverColor);
            TokenResourceBinder.CreateTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty, SegmentedResourceKey.ItemSelectedColor);
         }

         if (_mainLayout is not null) {
            UIStructureUtils.SetTemplateParent(Icon, this);
            _mainLayout.Children.Insert(0, Icon);
         }
      }
   }

   #endregion
}