using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls.Switch;

internal class SwitchKnob : Control, IControlCustomStyle, ITokenIdProvider
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private bool _isLoading = false;
   private CancellationTokenSource? _cancellationTokenSource;
   
   private static readonly StyledProperty<int> RotationProperty
      = AvaloniaProperty.Register<SwitchKnob, int>(nameof(Rotation));

   private int Rotation
   {
      get => GetValue(RotationProperty);
      set => SetValue(RotationProperty, value);
   }
   
   private static readonly StyledProperty<IBrush?> LoadIndicatorBrushProperty
      = AvaloniaProperty.Register<SwitchKnob, IBrush?>(nameof(LoadIndicatorBrush));
   
   private IBrush? LoadIndicatorBrush
   {
      get => GetValue(LoadIndicatorBrushProperty);
      set => SetValue(LoadIndicatorBrushProperty, value);
   }
   
   string ITokenIdProvider.TokenId => nameof(ToggleSwitch);
   
   private TokenResourceBinder _tokenResourceBinder;

   public static readonly StyledProperty<Size> KnobSizeProperty
      = AvaloniaProperty.Register<SwitchKnob, Size>(nameof(KnobSize));

   public Size KnobSize
   {
      get => GetValue(KnobSizeProperty);
      set => SetValue(KnobSizeProperty, value);
   }
   
   public static readonly StyledProperty<bool> IsCheckedStateProperty
      = AvaloniaProperty.Register<SwitchKnob, bool>(nameof(IsCheckedState));

   public bool IsCheckedState
   {
      get => GetValue(IsCheckedStateProperty);
      set => SetValue(IsCheckedStateProperty, value);
   }

   public static readonly StyledProperty<IBrush?> KnobBackgroundColorProperty
      = AvaloniaProperty.Register<SwitchKnob, IBrush?>(nameof(KnobBackgroundColor));

   protected IBrush? KnobBackgroundColor
   {
      get => GetValue(KnobBackgroundColorProperty);
      set => SetValue(KnobBackgroundColorProperty, value);
   }

   public static readonly StyledProperty<BoxShadow> KnobBoxShadowProperty
      = AvaloniaProperty.Register<SwitchKnob, BoxShadow>(nameof(KnobBoxShadow));

   protected BoxShadow KnobBoxShadow
   {
      get => GetValue(KnobBoxShadowProperty);
      set => SetValue(KnobBoxShadowProperty, value);
   }

   private TimeSpan _motionDuration;

   private static readonly DirectProperty<SwitchKnob, TimeSpan> MotionDurationTokenProperty
      = AvaloniaProperty.RegisterDirect<SwitchKnob, TimeSpan>(nameof(_motionDuration),
         (o) => o._motionDuration,
         (o, v) => o._motionDuration = v);
   
   private IBrush? _colorTextQuaternaryTokenProperty;

   private static readonly DirectProperty<SwitchKnob, IBrush?> ColorTextQuaternaryTokenProperty
      = AvaloniaProperty.RegisterDirect<SwitchKnob, IBrush?>(nameof(_colorTextQuaternaryTokenProperty),
         (o) => o._colorTextQuaternaryTokenProperty,
         (o, v) => o._colorTextQuaternaryTokenProperty = v);
   
   private IBrush? _switchColorTokenProperty;

   private static readonly DirectProperty<SwitchKnob, IBrush?> SwitchColorTokenProperty
      = AvaloniaProperty.RegisterDirect<SwitchKnob, IBrush?>(nameof(_switchColorTokenProperty),
         (o) => o._switchColorTokenProperty,
         (o, v) => o._switchColorTokenProperty = v);
   
   private double _loadingBgOpacity;

   private static readonly DirectProperty<SwitchKnob, double> LoadingBgOpacityTokenProperty
      = AvaloniaProperty.RegisterDirect<SwitchKnob, double>(nameof(_loadingBgOpacity),
         (o) => o._loadingBgOpacity,
         (o, v) => o._loadingBgOpacity = v);
   
   private TimeSpan LoadingAnimationDuration { get; set; } = TimeSpan.FromMilliseconds(300);
   
   public SwitchKnob()
   {
      _tokenResourceBinder = new TokenResourceBinder(this);
      _customStyle = this;
   }

   static SwitchKnob()
   {
      AffectsMeasure<SwitchKnob>(KnobSizeProperty);
      AffectsRender<SwitchKnob>(
         RotationProperty);
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }
   
   void IControlCustomStyle.SetupUi()
   {
      Effect = new DropShadowEffect
      {
         OffsetX = KnobBoxShadow.OffsetX,
         OffsetY = KnobBoxShadow.OffsetY,
         Color = KnobBoxShadow.Color,
         BlurRadius = KnobBoxShadow.Blur,
      };
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.SetupTransitions();
      AdjustLoadIndicatorBrush();
   }

   void IControlCustomStyle.SetupTransitions()
   {
      var transitions = new Transitions();
      transitions.Add(new SizeTransition
      {
         Property = KnobSizeProperty,
         Duration = _motionDuration
      });
      Transitions = transitions;
   }

   public void NotifyStartLoading()
   {
      if (_isLoading) {
         return;
      }
      _isLoading = true;
      IsEnabled = false;
      if (VisualRoot != null) {
         StartLoadingAnimation();
      }
   }

   private void StartLoadingAnimation()
   {
      var loadingAnimation = new Animation();
      loadingAnimation.Duration = LoadingAnimationDuration;
      loadingAnimation.IterationCount = IterationCount.Infinite;
      loadingAnimation.Easing = new LinearEasing();
      loadingAnimation.Children.Add(new KeyFrame
      {
         Setters =
         {
            new Setter
            {
               Property = RotationProperty,
               Value = 0
            }
         },
         KeyTime = TimeSpan.FromMilliseconds(0)
      });
      loadingAnimation.Children.Add(new KeyFrame
      {
         Setters =
         {
            new Setter
            {
               Property = RotationProperty,
               Value = 360
            }
         },
         KeyTime = LoadingAnimationDuration
      });
      _cancellationTokenSource = new CancellationTokenSource();
      loadingAnimation.RunAsync(this, _cancellationTokenSource!.Token);
   }

   public void NotifyStopLoading()
   {
      if (!_isLoading) {
         return;
      }
      _cancellationTokenSource?.Cancel();
      _cancellationTokenSource = null;
      _isLoading = false;
      IsEnabled = true;
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      if (_isLoading) {
         StartLoadingAnimation();
      }
   }
   
   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      if (_isLoading) {
         _cancellationTokenSource?.Cancel();
      }
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   void IControlCustomStyle.CollectStyleState()
   {
   }

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(MotionDurationTokenProperty, "MotionDurationMid");
      _tokenResourceBinder.AddBinding(this, ColorTextQuaternaryTokenProperty, "ColorTextQuaternary");
      _tokenResourceBinder.AddBinding(this, SwitchColorTokenProperty, "SwitchColor");
      _tokenResourceBinder.AddBinding(this, LoadingBgOpacityTokenProperty, "SwitchDisabledOpacity");
      LoadingAnimationDuration = TimeSpan.FromMilliseconds(1200);
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsEnabledProperty) {
         AdjustLoadIndicatorBrush();
      }
   }

   private void AdjustLoadIndicatorBrush()
   {
      if (IsCheckedState) {
         LoadIndicatorBrush = _switchColorTokenProperty;
      } else {
         LoadIndicatorBrush = _colorTextQuaternaryTokenProperty;
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      return KnobSize;
   }

   public sealed override void Render(DrawingContext context)
   {
      var targetRect = new Rect(new Point(0, 0), KnobSize);
      if (NumberUtils.FuzzyEqual(KnobSize.Width, KnobSize.Height)) {
         context.DrawEllipse(KnobBackgroundColor, null, targetRect);
      } else {
         context.DrawPilledRect(KnobBackgroundColor, null, targetRect);
      }

      if (_isLoading) {
         var delta = 2.5;
         var loadingRectSize = targetRect.Size.Deflate(new Thickness(delta));
         var loadingRect = new Rect(new Point(-loadingRectSize.Width / 2, -loadingRectSize.Height / 2), loadingRectSize);
         var pen = new Pen(LoadIndicatorBrush, 1, null, PenLineCap.Round);
         var translateToCenterMatrix = Matrix.CreateTranslation(targetRect.Center.X,targetRect.Center.Y);
         var rotationMatrix = Matrix.CreateRotation(Rotation * Math.PI / 180);
         using var translateToCenterState = context.PushTransform(translateToCenterMatrix);
         using var rotationMatrixState = context.PushTransform(rotationMatrix);
         using var bgOpacity = context.PushOpacity(_loadingBgOpacity);
         
         context.DrawArc(pen, loadingRect, 0, 90);
      }
   }
   
}