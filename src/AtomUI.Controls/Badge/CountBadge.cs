using AtomUI.ColorSystem;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum CountBadgeSize
{
   Default,
   Small
}

public partial class CountBadge : Control, IControlCustomStyle
{
   public static readonly StyledProperty<string?> BadgeColorProperty 
      = AvaloniaProperty.Register<CountBadge, string?>(
         nameof(BadgeColor));
   
   public static readonly StyledProperty<int> CountProperty 
      = AvaloniaProperty.Register<CountBadge, int>(nameof(Count),
                                                   coerce:(o, v) => Math.Max(0, v));
   
   public static readonly StyledProperty<Control?> DecoratedTargetProperty =
      AvaloniaProperty.Register<CountBadge, Control?>(nameof(DecoratedTarget));
   
   public static readonly StyledProperty<Point> OffsetProperty =
      AvaloniaProperty.Register<CountBadge, Point>(nameof(Offset));
   
   public static readonly StyledProperty<int> OverflowCountProperty =
      AvaloniaProperty.Register<CountBadge, int>(nameof(OverflowCount), 99,
                                                 coerce:(o, v) => Math.Max(0, v));
   
   public static readonly StyledProperty<bool> ShowZeroProperty =
      AvaloniaProperty.Register<CountBadge, bool>(nameof(ShowZero), false);
      
   public static readonly StyledProperty<CountBadgeSize> SizeProperty =
      AvaloniaProperty.Register<CountBadge, CountBadgeSize>(nameof(Size), CountBadgeSize.Default);
   
   public string? BadgeColor
   {
      get => GetValue(BadgeColorProperty);
      set => SetValue(BadgeColorProperty, value);
   }
   
   public int Count
   {
      get => GetValue(CountProperty);
      set => SetValue(CountProperty, value);
   }
   
   [Content]
   public Control? DecoratedTarget
   {
      get => GetValue(DecoratedTargetProperty);
      set => SetValue(DecoratedTargetProperty, value);
   }
   
   public Point Offset
   {
      get => GetValue(OffsetProperty);
      set => SetValue(OffsetProperty, value);
   }
   
   public int OverflowCount
   {
      get => GetValue(OverflowCountProperty);
      set => SetValue(OverflowCountProperty, value);
   }
   
   public bool ShowZero
   {
      get => GetValue(ShowZeroProperty);
      set => SetValue(ShowZeroProperty, value);
   }
   
   public CountBadgeSize Size
   {
      get => GetValue(SizeProperty);
      set => SetValue(SizeProperty, value);
   }
   
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private CountBadgeAdorner? _badgeAdorner;
   private AdornerLayer? _adornerLayer;
   
   public CountBadge()
   {
      _customStyle = this;
   }

   static CountBadge()
   {
      AffectsMeasure<CountBadge>(DecoratedTargetProperty, 
                                 CountProperty, 
                                 OverflowCountProperty,
                                 SizeProperty);
      AffectsRender<CountBadge>(BadgeColorProperty, OffsetProperty);
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      if (DecoratedTarget is not null && _badgeAdorner is not null) {
         _adornerLayer = AdornerLayer.GetAdornerLayer(this);
         // 这里需要抛出异常吗？
         if (_adornerLayer == null) {
            return;
         }
         AdornerLayer.SetAdornedElement(_badgeAdorner, this);
         AdornerLayer.SetIsClipEnabled(_badgeAdorner, false);
         _adornerLayer.Children.Add(_badgeAdorner);
      }
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      if (DecoratedTarget is not null && _badgeAdorner is not null) {
         // 这里需要抛出异常吗？
         if (_adornerLayer == null) {
            return;
         }
      
         _adornerLayer.Children.Remove(_badgeAdorner);
      }
   }

   void IControlCustomStyle.SetupUi()
   {
      _badgeAdorner = new CountBadgeAdorner();
      _customStyle.ApplyFixedStyleConfig();
      HandleDecoratedTargetChanged();
      if (BadgeColor is not null) {
         SetupBadgeColor(BadgeColor);
      }
   }
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      if (_badgeAdorner is not null) {
         BindUtils.RelayBind(this, OffsetProperty, _badgeAdorner, CountBadgeAdorner.OffsetProperty);
         BindUtils.RelayBind(this, SizeProperty, _badgeAdorner, CountBadgeAdorner.SizeProperty);
         BindUtils.RelayBind(this, OverflowCountProperty, _badgeAdorner, CountBadgeAdorner.OverflowCountProperty);
         BindUtils.RelayBind(this, CountProperty, _badgeAdorner, CountBadgeAdorner.CountProperty);
      }
   }
   
   private void HandleDecoratedTargetChanged()
   {
      if (_badgeAdorner is not null) {
         if (DecoratedTarget is null) {
            VisualChildren.Add(_badgeAdorner);
            LogicalChildren.Add(_badgeAdorner);
            _badgeAdorner.IsAdornerMode = false;
         } else if (DecoratedTarget is not null) {
            _badgeAdorner.IsAdornerMode = true;
            VisualChildren.Add(DecoratedTarget);
            LogicalChildren.Add(DecoratedTarget);
         }
      }
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (_initialized) {
         if (e.Property == DecoratedTargetProperty) {
            HandleDecoratedTargetChanged();
         }
         
         if (e.Property == BadgeColorProperty) {
            SetupBadgeColor(e.GetNewValue<string>());
         }
      }
   }
   
   private void SetupBadgeColor(string colorStr)
   {
      colorStr = colorStr.Trim().ToLower();
      
      foreach (var presetColor in PresetPrimaryColor.AllColorTypes()) {
         if (presetColor.Type.ToString().ToLower() == colorStr) {
            _badgeAdorner!.BadgeColor = new SolidColorBrush(presetColor.Color());
            return;
         }
      }
      if (Color.TryParse(colorStr, out Color color)) {
         _badgeAdorner!.BadgeColor = new SolidColorBrush(color);
      }
   }
}