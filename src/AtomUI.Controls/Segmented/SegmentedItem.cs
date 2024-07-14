using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public partial class SegmentedItem : StyledControl, IControlCustomStyle
{
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<SegmentedItem, SizeType>(nameof(SizeType), SizeType.Middle);

   public static readonly StyledProperty<string> TextProperty
      = AvaloniaProperty.Register<SegmentedItem, string>(nameof(Text), string.Empty);

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
   public string Text
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

   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private Label? _label;
   private bool _isPressed = false;
   private ControlStyleState _styleState;

   static SegmentedItem()
   {
      AffectsRender<SegmentedItem>(IsPressedProperty, FontSizeProperty, IsCurrentItemProperty);
      AffectsMeasure<SegmentedItem>(TextProperty, IconProperty, SizeTypeProperty, FontSizeProperty);
   }

   public SegmentedItem()
   {
      _controlTokenBinder = new ControlTokenBinder(this, SegmentedToken.ID);
      _customStyle = this;
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      // 由内置的 Box 布局
      var size = base.MeasureOverride(availableSize);
      var targetWidth = _label!.DesiredSize.Width;
      var targetHeight = size.Height;
      if (Icon is not null) {
         targetWidth += Icon.DesiredSize.Width;
         if (Text.Length > 0) {
            targetWidth += _paddingXXS;
         }
      }

      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      var offsetX = 0d;
      if (Icon is not null) {
         var offsetY = (DesiredSize.Height - Icon.Height) / 2;
         if (Text.Length == 0) {
            offsetX += (DesiredSize.Width - Icon.Width) / 2;
         }

         Icon.Arrange(new Rect(new(offsetX, offsetY), new Size(Icon.Width, Icon.Height)));
         offsetX += Icon.DesiredSize.Width + _paddingXXS;
      }

      _label!.Arrange(new Rect(new Point(offsetX, -1), _label.DesiredSize));
      return finalSize;
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

   #region IControlCustomStyle 实现

   void IControlCustomStyle.SetupUi()
   {
      _label = new Label()
      {
         Content = Text,
         Padding = new Thickness(0),
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalContentAlignment = HorizontalAlignment.Center,
         VerticalAlignment = VerticalAlignment.Center,
      };

      _customStyle.CollectStyleState();
      _customStyle.ApplySizeTypeStyleConfig();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.SetupTransitions();

      LogicalChildren.Add(_label);
      VisualChildren.Add(_label);

      ApplyIconStyleConfig();
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
      StyleUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }

      if (IsCurrentItem) {
         _styleState |= ControlStyleState.Selected;
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(PaddingXXSTokenProperty, GlobalResourceKey.PaddingXXS);
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Small) {
         _controlTokenBinder.AddControlBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightSM);
      } else if (SizeType == SizeType.Middle) {
         _controlTokenBinder.AddControlBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
      } else if (SizeType == SizeType.Large) {
         _controlTokenBinder.AddControlBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightLG);
      }
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (_initialized) {
         if (e.Property == IsPointerOverProperty ||
             e.Property == IsPressedProperty ||
             e.Property == IsCurrentItemProperty) {
            _customStyle.CollectStyleState();
            _customStyle.ApplyVariableStyleConfig();
         } else if (e.Property == SizeTypeProperty) {
            _customStyle.ApplySizeTypeStyleConfig();
         } else if (e.Property == TextProperty) {
            _label!.Content = Text;
         } else if (e.Property == IconProperty) {
            var oldIcon = e.GetOldValue<PathIcon?>();
            if (oldIcon is not null) {
               _controlTokenBinder.ReleaseBindings(oldIcon);
               LogicalChildren.Remove(oldIcon);
               VisualChildren.Remove(oldIcon);
            }

            ApplyIconStyleConfig();
         }
      }
   }

   // 设置大小和颜色
   private void ApplyIconStyleConfig()
   {
      if (Icon is not null) {
         if (Icon.ThemeType != IconThemeType.TwoTone) {
            _controlTokenBinder.AddControlBinding(Icon, PathIcon.NormalFillBrushProperty,
                                                  SegmentedResourceKey.ItemColor);
            _controlTokenBinder.AddControlBinding(Icon, PathIcon.ActiveFilledBrushProperty,
                                                  SegmentedResourceKey.ItemHoverColor);
            _controlTokenBinder.AddControlBinding(Icon, PathIcon.SelectedFilledBrushProperty,
                                                  SegmentedResourceKey.ItemSelectedColor);
         }

         if (SizeType == SizeType.Small) {
            _controlTokenBinder.AddControlBinding(Icon, WidthProperty, GlobalResourceKey.IconSizeSM);
            _controlTokenBinder.AddControlBinding(Icon, HeightProperty, GlobalResourceKey.IconSizeSM);
         } else if (SizeType == SizeType.Middle) {
            _controlTokenBinder.AddControlBinding(Icon, WidthProperty, GlobalResourceKey.IconSize);
            _controlTokenBinder.AddControlBinding(Icon, HeightProperty, GlobalResourceKey.IconSize);
         } else if (SizeType == SizeType.Large) {
            _controlTokenBinder.AddControlBinding(Icon, WidthProperty, GlobalResourceKey.IconSizeLG);
            _controlTokenBinder.AddControlBinding(Icon, HeightProperty, GlobalResourceKey.IconSizeLG);
         }

         LogicalChildren.Add(Icon);
         VisualChildren.Add(Icon);
      }
   }

   #endregion
}