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
   
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private DotBadgeAdorner? _dotBadgeAdorner;
   
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
      if (DecoratedTarget is not null && _dotBadgeAdorner is not null) {
         var adornerLayer = AdornerLayer.GetAdornerLayer(this);
         // 这里需要抛出异常吗？
         if (adornerLayer == null) {
            return;
         }
         AdornerLayer.SetAdornedElement(_dotBadgeAdorner, this);
         AdornerLayer.SetIsClipEnabled(_dotBadgeAdorner, false);
         adornerLayer.Children.Add(_dotBadgeAdorner);
      }
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      if (DecoratedTarget is not null && _dotBadgeAdorner is not null) {
         var adornerLayer = AdornerLayer.GetAdornerLayer(this);
         // 这里需要抛出异常吗？
         if (adornerLayer == null) {
            return;
         }

         adornerLayer.Children.Remove(_dotBadgeAdorner);
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
      if (_initialized) {
         if (e.Property == DecoratedTargetProperty) {
            HandleDecoratedTargetChanged();
         }
         
         if (e.Property == DotColorProperty) {
            SetupDotColor(e.GetNewValue<string>());
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