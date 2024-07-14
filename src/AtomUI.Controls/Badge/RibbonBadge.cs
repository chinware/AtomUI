using AtomUI.ColorSystem;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public enum RibbonBadgePlacement
{
   Start,
   End
}

public class RibbonBadge : Control, IControlCustomStyle
{
   public static readonly StyledProperty<string?> RibbonColorProperty
      = AvaloniaProperty.Register<RibbonBadge, string?>(nameof(RibbonColor));

   public static readonly StyledProperty<Control?> DecoratedTargetProperty =
      AvaloniaProperty.Register<RibbonBadge, Control?>(nameof(DecoratedTarget));

   public static readonly StyledProperty<Point> OffsetProperty =
      AvaloniaProperty.Register<RibbonBadge, Point>(nameof(Offset));

   public static readonly StyledProperty<string?> TextProperty
      = AvaloniaProperty.Register<RibbonBadge, string?>(nameof(Text));

   public static readonly StyledProperty<RibbonBadgePlacement> PlacementProperty
      = AvaloniaProperty.Register<RibbonBadge, RibbonBadgePlacement>(
         nameof(Text),
         RibbonBadgePlacement.End);
   
   public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
      AvaloniaProperty.Register<RibbonBadge, bool>(nameof(BadgeIsVisible));

   [Content]
   public Control? DecoratedTarget
   {
      get => GetValue(DecoratedTargetProperty);
      set => SetValue(DecoratedTargetProperty, value);
   }

   public string? RibbonColor
   {
      get => GetValue(RibbonColorProperty);
      set => SetValue(RibbonColorProperty, value);
   }

   public Point Offset
   {
      get => GetValue(OffsetProperty);
      set => SetValue(OffsetProperty, value);
   }

   public string? Text
   {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
   }

   public RibbonBadgePlacement Placement
   {
      get => GetValue(PlacementProperty);
      set => SetValue(PlacementProperty, value);
   }
   
   public bool BadgeIsVisible
   {
      get => GetValue(BadgeIsVisibleProperty);
      set => SetValue(BadgeIsVisibleProperty, value);
   }

   public RibbonBadge()
   {
      _customStyle = this;
   }

   static RibbonBadge()
   {
      AffectsMeasure<RibbonBadge>(DecoratedTargetProperty,
                               TextProperty);
      AffectsRender<RibbonBadge>(RibbonColorProperty, PlacementProperty);
   }

   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private RibbonBadgeAdorner? _ribbonBadgeAdorner;
   private AdornerLayer? _adornerLayer;

   void IControlCustomStyle.SetupUi()
   {
      _ribbonBadgeAdorner = new RibbonBadgeAdorner();
      _customStyle.ApplyFixedStyleConfig();
      HandleDecoratedTargetChanged();
      if (RibbonColor is not null) {
         SetupRibbonColor(RibbonColor);
      }
   }

   private void HandleDecoratedTargetChanged()
   {
      if (_ribbonBadgeAdorner is not null) {
         if (DecoratedTarget is null) {
            VisualChildren.Add(_ribbonBadgeAdorner);
            LogicalChildren.Add(_ribbonBadgeAdorner);
            _ribbonBadgeAdorner.IsAdornerMode = false;
         } else if (DecoratedTarget is not null) {
            _ribbonBadgeAdorner.IsAdornerMode = true;
            VisualChildren.Add(DecoratedTarget);
            LogicalChildren.Add(DecoratedTarget);
         }
      }
   }

   private void SetupRibbonColor(string colorStr)
   {
      colorStr = colorStr.Trim().ToLower();

      foreach (var presetColor in PresetPrimaryColor.AllColorTypes()) {
         if (presetColor.Type.ToString().ToLower() == colorStr) {
            _ribbonBadgeAdorner!.RibbonColor = new SolidColorBrush(presetColor.Color());
            return;
         }
      }

      if (Color.TryParse(colorStr, out Color color)) {
         _ribbonBadgeAdorner!.RibbonColor = new SolidColorBrush(color);
      }
   }
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      if (_ribbonBadgeAdorner is not null) {
         BindUtils.RelayBind(this, TextProperty, _ribbonBadgeAdorner, RibbonBadgeAdorner.TextProperty);
         BindUtils.RelayBind(this, OffsetProperty, _ribbonBadgeAdorner, RibbonBadgeAdorner.OffsetProperty);
         BindUtils.RelayBind(this, PlacementProperty, _ribbonBadgeAdorner, RibbonBadgeAdorner.PlacementProperty);
      }
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsVisibleProperty ||
          e.Property == BadgeIsVisibleProperty) {
         var badgeIsVisible = e.GetNewValue<bool>();
         if (badgeIsVisible) {
            if (_adornerLayer is not null) {
               return;
            }
            PrepareAdorner();
         } else {
            HideAdorner();
         }
      }
      if (_initialized) {
         if (e.Property == DecoratedTargetProperty) {
            HandleDecoratedTargetChanged();
         }
         
         if (e.Property == RibbonColorProperty) {
            SetupRibbonColor(e.GetNewValue<string>());
         }
      }
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   private void PrepareAdorner()
   {
      if (_adornerLayer is null &&
          DecoratedTarget is not null && 
          _ribbonBadgeAdorner is not null) {
         _adornerLayer = AdornerLayer.GetAdornerLayer(this);
         // 这里需要抛出异常吗？
         if (_adornerLayer == null) {
            return;
         }
         AdornerLayer.SetAdornedElement(_ribbonBadgeAdorner, this);
         AdornerLayer.SetIsClipEnabled(_ribbonBadgeAdorner, false);
         Console.WriteLine(_ribbonBadgeAdorner.GetVisualParent());
         _adornerLayer.Children.Add(_ribbonBadgeAdorner);
      }
   }
   
   private void HideAdorner()
   {
      // 这里需要抛出异常吗？
      if (_adornerLayer is null || _ribbonBadgeAdorner is null) {
         return;
      }
      
      _adornerLayer.Children.Remove(_ribbonBadgeAdorner);
      _adornerLayer = null;
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      PrepareAdorner();
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      HideAdorner();
   }
}