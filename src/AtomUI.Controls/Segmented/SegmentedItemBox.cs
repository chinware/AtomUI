using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls;

/// <summary>
/// 在内部维护一些额外信息的控件，用户无感知
/// 绘制圆角什么的
/// </summary>
internal partial class SegmentedItemBox : BorderedStyleControl, 
                                          ICustomHitTest,
                                          IControlCustomStyle
{
   internal Control Item { get; }
   
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      Segmented.SizeTypeProperty.AddOwner<SegmentedItemBox>();
   
   public static readonly DirectProperty<SegmentedItemBox, bool> IsPressedProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItemBox, bool>(nameof(IsPressed), o => o.IsPressed);
   
   public static readonly DirectProperty<SegmentedItemBox, bool> IsCurrentItemProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItemBox, bool>(nameof(IsCurrentItem), 
         o => o.IsCurrentItem,
         (o, v) => o.IsCurrentItem = v);
   
   /// <summary>
   /// Gets or sets a value indicating whether the button is currently pressed.
   /// </summary>
   public bool IsPressed
   {
      get => _isPressed;
      private set => SetAndRaise(IsPressedProperty, ref _isPressed, value);
   }
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   private bool _isCurrentItem;
   internal bool IsCurrentItem { get => _isCurrentItem; set => SetCurrentItem(value); }
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private bool _isPressed = false;
   private ControlStyleState _styleState;
   
   public int LastItem { get; set; } = -1;

   static SegmentedItemBox()
   {
      FocusableProperty.OverrideDefaultValue(typeof(SegmentedItemBox), true);
      AffectsMeasure<SegmentedItemBox>(SizeTypeProperty);
      AffectsRender<SegmentedItemBox>(BackgroundProperty);
   }
   
   public SegmentedItemBox(Control item)
   {
      _controlTokenBinder = new ControlTokenBinder(this, SegmentedToken.ID);
      _customStyle = this;
      Item = item;
      Child = Item;
      BindUtils.RelayBind(Item, IsEnabledProperty, this);
      if (item is SegmentedItem segmentedItem) {
         BindUtils.RelayBind(this, SizeTypeProperty, segmentedItem);
      }
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      // 由内置的 Box 布局
      var size = base.MeasureOverride(availableSize);
      var itemHeight = ItemHeight(SizeType);
      var targetWidth = size.Width;
      var targetHeight = Math.Max(size.Height, itemHeight);
      var thickness = ItemThickness(SizeType);
      targetWidth += thickness.Left + thickness.Right;
      targetHeight += thickness.Top + thickness.Bottom;
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      base.ArrangeOverride(finalSize);
      var offsetX = (finalSize.Width - Item.DesiredSize.Width) / 2;
      var offsetY = (finalSize.Height - Item.DesiredSize.Height) / 2;
      Item.Arrange(new Rect(new Point(offsetX, offsetY), Item.DesiredSize));
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

   private void SetCurrentItem(bool flag)
   {
      SetAndRaise(IsCurrentItemProperty, ref _isCurrentItem, flag);
      // 其他类型先不通知
      if (Item is SegmentedItem segmentedItem) {
         segmentedItem.IsCurrentItem = flag;
      }
   }

   private double ItemHeight(SizeType sizeType)
   {
      var itemHeight = 0d;
      var padding = _trackPadding.Top + _trackPadding.Bottom;
      if (sizeType == SizeType.Small) {
         itemHeight = _controlHeightSM - padding;
      } else if (sizeType == SizeType.Middle) {
         itemHeight = _controlHeight - padding;
      } else {
         itemHeight = _controlHeightLG - padding;
      }
      return itemHeight;
   }

   private Thickness ItemThickness(SizeType sizeType)
   {
      if (sizeType == SizeType.Large || sizeType == SizeType.Middle) {
         return _segmentedItemPadding;
      }

      return _segmentedItemPaddingSM;
   }
   
   public bool HitTest(Point point)
   {
      return true;
   }
   
   #region IControlCustomStyle 实现
   void IControlCustomStyle.SetupUi()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.ApplySizeTypeStyleConfig();
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.SetupTransitions();
   }

   void IControlCustomStyle.SetupTransitions()
   { 
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty),
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

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Large) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadius);
         _controlTokenBinder.AddControlBinding(FontSizeProperty, GlobalResourceKey.FontSizeLG);
      } else if (SizeType == SizeType.Middle) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
         _controlTokenBinder.AddControlBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      } else if (SizeType == SizeType.Small) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusXS);
         _controlTokenBinder.AddControlBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      }
   }

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _controlTokenBinder.ReleaseTriggerBindings(this);
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (!_styleState.HasFlag(ControlStyleState.Selected)) {
            _controlTokenBinder.AddControlBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);
            _controlTokenBinder.AddControlBinding(ForegroundProperty, SegmentedResourceKey.ItemColor);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _controlTokenBinder.AddControlBinding(BackgroundProperty, SegmentedResourceKey.ItemActiveBg, BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(BackgroundProperty, SegmentedResourceKey.ItemHoverBg, BindingPriority.StyleTrigger);
               _controlTokenBinder.AddControlBinding(ForegroundProperty, SegmentedResourceKey.ItemHoverColor, BindingPriority.StyleTrigger);
            }
         } else {
            _controlTokenBinder.AddControlBinding(ForegroundProperty, SegmentedResourceKey.ItemSelectedColor);
         }
      } else {
        _controlTokenBinder.AddControlBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(ControlHeightSMTokenProperty, GlobalResourceKey.ControlHeightSM);
      _controlTokenBinder.AddControlBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
      _controlTokenBinder.AddControlBinding(ControlHeightLGTokenProperty, GlobalResourceKey.ControlHeightLG);
      _controlTokenBinder.AddControlBinding(TrackPaddingTokenProperty, SegmentedResourceKey.TrackPadding);
      _controlTokenBinder.AddControlBinding(SegmentedItemPaddingSMTokenProperty, SegmentedResourceKey.SegmentedItemPaddingSM);
      _controlTokenBinder.AddControlBinding(SegmentedItemPaddingTokenProperty, SegmentedResourceKey.SegmentedItemPadding);
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPressedProperty ||
          e.Property == IsPointerOverProperty ||
          e.Property == IsCurrentItemProperty) {
         _customStyle.CollectStyleState();
         _customStyle.ApplyVariableStyleConfig();
      }

      if (_initialized) {
         if (e.Property == SizeTypeProperty) {
            _customStyle.ApplySizeTypeStyleConfig();
         }
      }
   }
   #endregion
}