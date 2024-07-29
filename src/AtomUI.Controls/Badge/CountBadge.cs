using AtomUI.Controls.Badge;
using AtomUI.Controls.MotionScene;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Palette;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum CountBadgeSize
{
   Default,
   Small
}

public class CountBadge : Control, IControlCustomStyle
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
   
   public static readonly StyledProperty<bool> BadgeIsVisibleProperty =
      AvaloniaProperty.Register<CountBadge, bool>(nameof(BadgeIsVisible));
   
   internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
      AvaloniaProperty.Register<CountBadge, TimeSpan>(
         nameof(MotionDuration));
   
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
   
   private IControlCustomStyle _customStyle;
   private CountBadgeAdorner? _badgeAdorner;
   private AdornerLayer? _adornerLayer;
   private bool _animating = false;
   
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
   
   public sealed override void ApplyTemplate()
   {
      base.ApplyTemplate();
      if (DecoratedTarget is null) {
         CreateBadgeAdorner();
      }
   }

   private CountBadgeAdorner CreateBadgeAdorner()
   {
      if (_badgeAdorner is null) {
         _badgeAdorner = new CountBadgeAdorner();
         _customStyle.SetupTokenBindings();
         HandleDecoratedTargetChanged();
         if (BadgeColor is not null) {
            SetupBadgeColor(BadgeColor);
         }
      }

      return _badgeAdorner;
   }

   private void PrepareAdorner()
   {
      if (_adornerLayer is null && DecoratedTarget is not null) {
         var badgeAdorner = CreateBadgeAdorner();
         _adornerLayer = AdornerLayer.GetAdornerLayer(this);
         // 这里需要抛出异常吗？
         if (_adornerLayer == null) {
            return;
         }
         AdornerLayer.SetAdornedElement(badgeAdorner, this);
         AdornerLayer.SetIsClipEnabled(badgeAdorner, false);
         _adornerLayer.Children.Add(badgeAdorner);
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
    
      AbstractMotion motion;
      var adorner = _badgeAdorner!;
      if (DecoratedTarget is not null) {
         var countBadgeZoomBadgeIn = new CountBadgeZoomBadgeIn();
         countBadgeZoomBadgeIn.ConfigureOpacity(MotionDuration);
         countBadgeZoomBadgeIn.ConfigureRenderTransform(MotionDuration);
         motion = countBadgeZoomBadgeIn;
         adorner.AnimationRenderTransformOrigin = motion.MotionRenderTransformOrigin;
      } else {
         var countBadgeNoWrapperZoomBadgeIn = new CountBadgeNoWrapperZoomBadgeIn();
         countBadgeNoWrapperZoomBadgeIn.ConfigureOpacity(MotionDuration);
         countBadgeNoWrapperZoomBadgeIn.ConfigureRenderTransform(MotionDuration);
         motion = countBadgeNoWrapperZoomBadgeIn;
      }
      
      var motionActor = new MotionActor(adorner, motion);
      motionActor.DispatchInSceneLayer = false;
      motionActor.Completed += (sender, args) =>
      {
         adorner.AnimationRenderTransformOrigin = null;
         _animating = false;
      };
      director?.Schedule(motionActor);
   }

   private void HideAdorner()
   {
      // 这里需要抛出异常吗？
      if (_adornerLayer is null || _badgeAdorner is null) {
         return;
      }
      _adornerLayer.Children.Remove(_badgeAdorner);
      _adornerLayer = null;
   }

   private void HideAdornerWithMotion()
   {
      if (VisualRoot is null || _animating) {
         return;
      }
      _animating = true;
      var director = Director.Instance;
      AbstractMotion motion;
      var adorner = _badgeAdorner!;
      if (DecoratedTarget is not null) {
          var countBadgeZoomBadgeOut = new CountBadgeZoomBadgeOut();
          countBadgeZoomBadgeOut.ConfigureOpacity(MotionDuration);
          countBadgeZoomBadgeOut.ConfigureRenderTransform(MotionDuration);
          motion = countBadgeZoomBadgeOut;
          adorner.AnimationRenderTransformOrigin = motion.MotionRenderTransformOrigin;
      } else {
         var countBadgeNoWrapperZoomBadgeOut = new CountBadgeNoWrapperZoomBadgeOut();
         countBadgeNoWrapperZoomBadgeOut.ConfigureOpacity(MotionDuration);
         countBadgeNoWrapperZoomBadgeOut.ConfigureRenderTransform(MotionDuration);
         motion = countBadgeNoWrapperZoomBadgeOut;
      }

      var motionActor = new MotionActor(adorner, motion);
      motionActor.DispatchInSceneLayer = false;
      motionActor.Completed += (sender, args) =>
      {
         HideAdorner();
         adorner.AnimationRenderTransformOrigin = null;
         _animating = false;
      };
      director?.Schedule(motionActor);
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
   
   void IControlCustomStyle.SetupTokenBindings()
   {
      if (_badgeAdorner is not null) {
         BindUtils.RelayBind(this, OffsetProperty, _badgeAdorner, CountBadgeAdorner.OffsetProperty);
         BindUtils.RelayBind(this, SizeProperty, _badgeAdorner, CountBadgeAdorner.SizeProperty);
         BindUtils.RelayBind(this, OverflowCountProperty, _badgeAdorner, CountBadgeAdorner.OverflowCountProperty);
         BindUtils.RelayBind(this, CountProperty, _badgeAdorner, CountBadgeAdorner.CountProperty);
      }
      TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, GlobalResourceKey.MotionDurationSlow);
   }
   
   private void HandleDecoratedTargetChanged()
   {
      if (_badgeAdorner is not null) {
         if (DecoratedTarget is null) {
            ((ISetLogicalParent)_badgeAdorner).SetParent(this);
            VisualChildren.Add(_badgeAdorner);
            _badgeAdorner.IsAdornerMode = false;
         } else if (DecoratedTarget is not null) {
            _badgeAdorner.IsAdornerMode = true;
            ((ISetLogicalParent)DecoratedTarget).SetParent(this);
            VisualChildren.Add(DecoratedTarget);
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
            PrepareAdornerWithMotion();
         } else {
            HideAdornerWithMotion();
         }
      }
      if (VisualRoot is not null) {
         if (e.Property == DecoratedTargetProperty) {
            HandleDecoratedTargetChanged();
         }
         
         if (e.Property == BadgeColorProperty) {
            SetupBadgeColor(e.GetNewValue<string>());
         }
      }

      if (e.Property == CountProperty) {
         var newCount = e.GetNewValue<int>();
         if (newCount == 0 && !ShowZero) {
            BadgeIsVisible = false;
         } else if (newCount > 0) {
            BadgeIsVisible = true;
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