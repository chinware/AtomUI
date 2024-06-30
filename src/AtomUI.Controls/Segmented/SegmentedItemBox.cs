using AtomUI.Data;
using AtomUI.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls;

/// <summary>
/// 在内部维护一些额外信息的控件，用户无感知
/// </summary>
internal partial class SegmentedItemBox : BorderedStyleControl, ICustomHitTest
{
   internal Control Item { get; }
   
   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<SegmentedItem, SizeType>(nameof(SizeType), SizeType.Middle);
   
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
}