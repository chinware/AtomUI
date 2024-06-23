using AtomUI.Data;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public partial class SegmentedItem : StyledControl, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => SegmentedToken.ID;

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
   internal bool IsCurrentItem { get => _isCurrentItem; set => SetAndRaise(IsCurrentItemProperty, ref _isCurrentItem, value); }

   static SegmentedItem()
   {
      AffectsRender<SegmentedItem>(IsPressedProperty, FontSizeProperty, IsCurrentItemProperty);
      AffectsMeasure<SegmentedItem>(TextProperty, IconProperty, SizeTypeProperty, FontSizeProperty);
   }

   public SegmentedItem()
   {
      _tokenResourceBinder = new TokenResourceBinder(this);
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
         Icon.Arrange(new Rect(new (offsetX, offsetY), new Size(Icon.Width, Icon.Height)));
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
}