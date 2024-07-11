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

namespace AtomUI.Controls;

public enum DotBadgeStatus
{
   Default,
   Success,
   Processing,
   Error,
   Warning
}

public class DotBadge : Control, IControlCustomStyle
{
   public static readonly StyledProperty<string?> DotColorProperty 
      = AvaloniaProperty.Register<DotBadge, string?>(
         nameof(DotColor));
   
   public static readonly StyledProperty<DotBadgeStatus?> StatusProperty 
      = AvaloniaProperty.Register<DotBadge, DotBadgeStatus?>(
         nameof(Status));
   
   public static readonly StyledProperty<string?> TextProperty 
      = AvaloniaProperty.Register<DotBadge, string?>(
         nameof(Text));
   
   public static readonly StyledProperty<Control?> DecoratedTargetProperty =
      AvaloniaProperty.Register<DotBadge, Control?>(nameof(DecoratedTarget));
   
   public static readonly StyledProperty<Point> OffsetProperty =
      AvaloniaProperty.Register<DotBadge, Point>(nameof(Offset));
   
   public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
      AvaloniaProperty.Register<DotBadge, bool>(nameof(BadgeIsVisible));
   
   public string? DotColor
   {
      get => GetValue(DotColorProperty);
      set => SetValue(DotColorProperty, value);
   }

   public DotBadgeStatus? Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }

   public string? Text
   {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
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
   
   public bool BadgeIsVisible
   {
      get => GetValue(BadgeIsVisibleProperty);
      set => SetValue(BadgeIsVisibleProperty, value);
   }
   
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private DotBadgeAdorner? _dotBadgeAdorner;
   private AdornerLayer? _adornerLayer;
   
   public DotBadge()
   {
      _customStyle = this;
   }

   static DotBadge()
   {
      AffectsMeasure<DotBadge>(DecoratedTargetProperty,
                               TextProperty);
      AffectsRender<DotBadge>(DotColorProperty, StatusProperty);
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
      SetupAdorner();
   }

   private void SetupAdorner()
   {
      if (_adornerLayer is null && 
          DecoratedTarget is not null &&
          _dotBadgeAdorner is not null) {
         _adornerLayer = AdornerLayer.GetAdornerLayer(this);
         // 这里需要抛出异常吗？
         if (_adornerLayer == null) {
            return;
         }
         AdornerLayer.SetAdornedElement(_dotBadgeAdorner, this);
         AdornerLayer.SetIsClipEnabled(_dotBadgeAdorner, false);
         _adornerLayer.Children.Add(_dotBadgeAdorner);
      }
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      if (DecoratedTarget is not null && _dotBadgeAdorner is not null) {
         // 这里需要抛出异常吗？
         if (_adornerLayer == null) {
            return;
         }
      
         _adornerLayer.Children.Remove(_dotBadgeAdorner);
         _adornerLayer = null;
      }
   }

   void IControlCustomStyle.SetupUi()
   {
      _dotBadgeAdorner = new DotBadgeAdorner();
      _customStyle.ApplyFixedStyleConfig();
      HandleDecoratedTargetChanged();
      if (DotColor is not null) {
         SetupDotColor(DotColor);
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      if (_dotBadgeAdorner is not null) {
         BindUtils.RelayBind(this, StatusProperty, _dotBadgeAdorner, DotBadgeAdorner.StatusProperty);
         BindUtils.RelayBind(this, TextProperty, _dotBadgeAdorner, DotBadgeAdorner.TextProperty);
         BindUtils.RelayBind(this, OffsetProperty, _dotBadgeAdorner, DotBadgeAdorner.OffsetProperty);
      }
   }

   private void HandleDecoratedTargetChanged()
   {
      if (_dotBadgeAdorner is not null) {
         if (DecoratedTarget is null) {
            VisualChildren.Add(_dotBadgeAdorner);
            LogicalChildren.Add(_dotBadgeAdorner);
            _dotBadgeAdorner.IsAdornerMode = false;
         } else if (DecoratedTarget is not null) {
            _dotBadgeAdorner.IsAdornerMode = true;
            VisualChildren.Add(DecoratedTarget);
            LogicalChildren.Add(DecoratedTarget);
         }
      }
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (e.Property == IsVisibleProperty) {
         SetValue(BadgeIsVisibleProperty, IsVisible, BindingPriority.Inherited);
      } else if (e.Property == BadgeIsVisibleProperty) {
         var badgeIsVisible = e.GetNewValue<bool>();
         if (badgeIsVisible) {
            if (_adornerLayer is not null) {
               return;
            }
            SetupAdorner();
         } else {
            if (_adornerLayer is null || _dotBadgeAdorner is null) {
               return;
            }
      
            _adornerLayer.Children.Remove(_dotBadgeAdorner);
            _adornerLayer = null;
         }
      }
      if (_initialized) {
         if (e.Property == DecoratedTargetProperty) {
            HandleDecoratedTargetChanged();
         }
         
         if (e.Property == DotColorProperty) {
            SetupDotColor(e.GetNewValue<string>());
         }

         if (e.Property == IsVisibleProperty) {
            if (!IsVisible) {
               if (_adornerLayer is null || _dotBadgeAdorner is null) {
                  return;
               }
      
               _adornerLayer.Children.Remove(_dotBadgeAdorner);
               _adornerLayer = null;
            } else {
               SetupAdorner();
            }
         }
      }
   }
   
   private void SetupDotColor(string colorStr)
   {
      colorStr = colorStr.Trim().ToLower();
      
      foreach (var presetColor in PresetPrimaryColor.AllColorTypes()) {
         if (presetColor.Type.ToString().ToLower() == colorStr) {
            _dotBadgeAdorner!.DotColor = new SolidColorBrush(presetColor.Color());
            return;
         }
      }
      if (Color.TryParse(colorStr, out Color color)) {
         _dotBadgeAdorner!.DotColor = new SolidColorBrush(color);
      }
   }
}