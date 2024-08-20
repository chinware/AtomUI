using AtomUI.Controls.Badge;
using AtomUI.Controls.MotionScene;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Theme.Palette;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
   
   internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
      AvaloniaProperty.Register<CountBadge, TimeSpan>(
         nameof(MotionDuration));
   
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
   
   public TimeSpan MotionDuration
   {
      get => GetValue(MotionDurationProperty);
      set => SetValue(MotionDurationProperty, value);
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
      AffectsMeasure<DotBadge>(DecoratedTargetProperty, TextProperty);
      AffectsRender<DotBadge>(DotColorProperty, StatusProperty);
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      PrepareAdorner();
   }

   private DotBadgeAdorner CreateDotBadgeAdorner()
   {
      if (_dotBadgeAdorner is null) {
         _dotBadgeAdorner = new DotBadgeAdorner();
         _customStyle.SetupTokenBindings();
         HandleDecoratedTargetChanged();
         if (DotColor is not null) {
            SetupDotColor(DotColor);
         }
      }

      return _dotBadgeAdorner;
   }

   private void PrepareAdorner()
   {
      if (_adornerLayer is null && DecoratedTarget is not null) {
         var dotBadgeAdorner = CreateDotBadgeAdorner();
         _adornerLayer = AdornerLayer.GetAdornerLayer(this);
         // 这里需要抛出异常吗？
         if (_adornerLayer == null) {
            return;
         }
         AdornerLayer.SetAdornedElement(dotBadgeAdorner, this);
         AdornerLayer.SetIsClipEnabled(dotBadgeAdorner, false);
         _adornerLayer.Children.Add(dotBadgeAdorner);
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
      motion.ConfigureOpacity(MotionDuration);
      motion.ConfigureRenderTransform(MotionDuration);
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
      motion.ConfigureOpacity(MotionDuration);
      motion.ConfigureRenderTransform(MotionDuration);
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

   void IControlCustomStyle.SetupTokenBindings()
   {
      if (_dotBadgeAdorner is not null) {
         BindUtils.RelayBind(this, StatusProperty, _dotBadgeAdorner, DotBadgeAdorner.StatusProperty);
         BindUtils.RelayBind(this, TextProperty, _dotBadgeAdorner, DotBadgeAdorner.TextProperty);
         BindUtils.RelayBind(this, OffsetProperty, _dotBadgeAdorner, DotBadgeAdorner.OffsetProperty);
      }
      TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, GlobalTokenResourceKey.MotionDurationSlow);
   }

   private void HandleDecoratedTargetChanged()
   {
      if (_dotBadgeAdorner is not null) {
         if (DecoratedTarget is null) {
            _dotBadgeAdorner.IsAdornerMode = false;
            ((ISetLogicalParent)_dotBadgeAdorner).SetParent(this);
            VisualChildren.Add(_dotBadgeAdorner);
         } else if (DecoratedTarget is not null) {
            _dotBadgeAdorner.IsAdornerMode = true;
            VisualChildren.Add(DecoratedTarget);
            ((ISetLogicalParent)DecoratedTarget).SetParent(this);
         }
      }
   }

   public sealed override void ApplyTemplate()
   {
      base.ApplyTemplate();
      if (DecoratedTarget is null) {
         CreateDotBadgeAdorner();
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
            _dotBadgeAdorner!.BadgeDotColor = new SolidColorBrush(presetColor.Color());
            return;
         }
      }
      if (Color.TryParse(colorStr, out Color color)) {
         _dotBadgeAdorner!.BadgeDotColor = new SolidColorBrush(color);
      }
   }
}