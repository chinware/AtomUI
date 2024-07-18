using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls;

/// <summary>
/// 在内部维护一些额外信息的控件，用户无感知
/// 绘制圆角什么的
/// </summary>
internal partial class SegmentedItemBox : TemplatedControl, 
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
      _customStyle = this;
      Item = item;
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
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.SetupTransitions();
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
      var padding = _trackPaddingToken.Top + _trackPaddingToken.Bottom;
      if (sizeType == SizeType.Small) {
         itemHeight = _controlHeightSMToken - padding;
      } else if (sizeType == SizeType.Middle) {
         itemHeight = _controlHeightToken - padding;
      } else {
         itemHeight = _controlHeightLGToken - padding;
      }
      return itemHeight;
   }

   private Thickness ItemThickness(SizeType sizeType)
   {
      if (sizeType == SizeType.Large || sizeType == SizeType.Middle) {
         return _segmentedItemPaddingToken;
      }

      return _segmentedItemPaddingSMToken;
   }
   
   public bool HitTest(Point point)
   {
      return true;
   }
   
   #region IControlCustomStyle 实现
   void IControlCustomStyle.SetupUi()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.CollectStyleState();
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
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, ControlHeightSMTokenProperty, GlobalResourceKey.ControlHeightSM);
      BindUtils.CreateTokenBinding(this, ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
      BindUtils.CreateTokenBinding(this, ControlHeightLGTokenProperty, GlobalResourceKey.ControlHeightLG);
      BindUtils.CreateTokenBinding(this, TrackPaddingTokenProperty, SegmentedResourceKey.TrackPadding);
      BindUtils.CreateTokenBinding(this, SegmentedItemPaddingSMTokenProperty, SegmentedResourceKey.SegmentedItemPaddingSM);
      BindUtils.CreateTokenBinding(this, SegmentedItemPaddingTokenProperty, SegmentedResourceKey.SegmentedItemPadding);
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPressedProperty ||
          e.Property == IsPointerOverProperty ||
          e.Property == IsCurrentItemProperty) {
         _customStyle.CollectStyleState();
      }
   }
   
   void IControlCustomStyle.UpdatePseudoClasses()
   {
      PseudoClasses.Set(StdPseudoClass.Pressed, IsPressed);
      PseudoClasses.Set(StdPseudoClass.Selected, IsCurrentItem);
   }
   #endregion
}