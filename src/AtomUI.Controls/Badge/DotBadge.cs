using AtomUI.ColorSystem;
using AtomUI.Controls.Badge;
using AtomUI.Controls.MotionScene;
using AtomUI.Data;
using AtomUI.MotionScene;
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

public partial class DotBadge : Control, IControlCustomStyle
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
   private bool _animating = false;
   
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
      PrepareAdorner();
   }

   private void PrepareAdorner()
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

   private void PrepareAdornerWithMotion()
   {
      PrepareAdorner();
     
      if (VisualRoot is null || _animating) {
         return;
      }
      _animating = true;
      var director = Director.Instance;
      var motion = new CountBadgeZoomBadgeIn();
      motion.ConfigureOpacity(_motionDurationSlow);
      motion.ConfigureRenderTransform(_motionDurationSlow);
      _dotBadgeAdorner!.AnimationRenderTransformOrigin = motion.MotionRenderTransformOrigin;
      var motionActor = new MotionActor(_dotBadgeAdorner, motion);
      motionActor.DispatchInSceneLayer = false;
      motionActor.Completed += (sender, args) =>
      {
         _dotBadgeAdorner.AnimationRenderTransformOrigin = null;
         _animating = false;
      };
      director?.Schedule(motionActor);
   }

   private void HideAdorner()
   {
      // 这里需要抛出异常吗？
      if (_adornerLayer is null || _dotBadgeAdorner is null) {
         return;
      }
      
      _adornerLayer.Children.Remove(_dotBadgeAdorner);
      _adornerLayer = null;
   }
   
   private void HideAdornerWithMotion()
   {
      if (VisualRoot is null || _animating) {
         return;
      }
      _animating = true;
      var director = Director.Instance;
      var motion = new CountBadgeZoomBadgeOut();
      motion.ConfigureOpacity(_motionDurationSlow);
      motion.ConfigureRenderTransform(_motionDurationSlow);
      _dotBadgeAdorner!.AnimationRenderTransformOrigin = motion.MotionRenderTransformOrigin;
      var motionActor = new MotionActor(_dotBadgeAdorner, motion);
      motionActor.DispatchInSceneLayer = false;
      motionActor.Completed += (sender, args) =>
      {
         HideAdorner();
         _dotBadgeAdorner.AnimationRenderTransformOrigin = null;
         _animating = false;
      };
      director?.Schedule(motionActor);
   }


   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      HideAdorner();
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
      BindUtils.CreateTokenBinding(this, MotionDurationSlowTokenProperty, GlobalResourceKey.MotionDurationSlow);
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
         var badgeIsVisible = e.GetNewValue<bool>();
         if (badgeIsVisible) {
            if (_adornerLayer is not null) {
               return;
            }
            PrepareAdorner();
         } else {
            HideAdorner();
         }
      } else if (e.Property == BadgeIsVisibleProperty) {
         var badgeIsVisible = e.GetNewValue<bool>();
         if (badgeIsVisible) {
            if (_adornerLayer is not null) {
               return;
            }

            if (DecoratedTarget is not null) {
               PrepareAdornerWithMotion();
            } else {
               PrepareAdorner();
            }
           
         } else {
            if (DecoratedTarget is not null) {
               HideAdornerWithMotion();
            } else {
               HideAdorner();
            }
         }
      }
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