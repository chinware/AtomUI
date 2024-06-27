using AtomUI.Data;
using AtomUI.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Controls;

public enum ProgressStatus
{
   Normal,
   Success,
   Exception,
   Active,
}

public abstract partial class AbstractProgressBar : RangeBaseControl, ISizeTypeAware, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => ProgressBarToken.ID;

   protected const double LARGE_STROKE_THICKNESS = 8;
   protected const double MIDDLE_STROKE_THICKNESS = 6;
   protected const double SMALL_STROKE_THICKNESS = 4;

   /// <summary>
   /// Defines the <see cref="IsIndeterminate"/> property.
   /// </summary>
   public static readonly StyledProperty<bool> IsIndeterminateProperty =
      AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(IsIndeterminate));

   /// <summary>
   /// Defines the <see cref="ShowProgressInfo"/> property.
   /// </summary>
   public static readonly StyledProperty<bool> ShowProgressInfoProperty =
      AvaloniaProperty.Register<AbstractProgressBar, bool>(nameof(ShowProgressInfo), true);

   /// <summary>
   /// Defines the <see cref="ProgressTextFormat"/> property.
   /// </summary>
   public static readonly StyledProperty<string> ProgressTextFormatProperty =
      AvaloniaProperty.Register<AbstractProgressBar, string>(nameof(ProgressTextFormat), "{0:0}%");

   /// <summary>
   /// Defines the <see cref="Percentage"/> property.
   /// </summary>
   public static readonly DirectProperty<AbstractProgressBar, double> PercentageProperty =
      AvaloniaProperty.RegisterDirect<AbstractProgressBar, double>(
         nameof(Percentage),
         o => o.Percentage,
         (o, v) => o.Percentage = v);

   public static readonly StyledProperty<Color?> TrailColorProperty =
      AvaloniaProperty.Register<AbstractProgressBar, Color?>(nameof(TrailColor));

   public static readonly StyledProperty<PenLineCap> StrokeLineCapProperty =
      AvaloniaProperty.Register<AbstractProgressBar, PenLineCap>(nameof(StrokeLineCap), PenLineCap.Round);

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AvaloniaProperty.Register<AbstractProgressBar, SizeType>(nameof(SizeType), SizeType.Large);

   public static readonly StyledProperty<ProgressStatus> StatusProperty =
      AvaloniaProperty.Register<AbstractProgressBar, ProgressStatus>(nameof(Status), ProgressStatus.Normal);

   public static readonly StyledProperty<IBrush?> IndicatorBarBrushProperty =
      AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(IndicatorBarBrush));

   public static readonly StyledProperty<double> IndicatorThicknessProperty =
      AvaloniaProperty.Register<ProgressBar, double>(nameof(IndicatorThickness), double.NaN);

   public static readonly StyledProperty<double> SuccessThresholdProperty =
      AvaloniaProperty.Register<ProgressBar, double>(nameof(SuccessThreshold), double.NaN);

   public static readonly StyledProperty<IBrush?> SuccessThresholdBrushProperty =
      AvaloniaProperty.Register<ProgressBar, IBrush?>(nameof(SuccessThresholdBrush));

   protected static readonly DirectProperty<AbstractProgressBar, SizeType> EffectiveSizeTypeProperty =
      AvaloniaProperty.RegisterDirect<AbstractProgressBar, SizeType>(nameof(EffectiveSizeType),
                                                                     o => o.EffectiveSizeType,
                                                                     (o, v) => o.EffectiveSizeType = v);

   protected static readonly DirectProperty<AbstractProgressBar, double> StrokeThicknessProperty =
      AvaloniaProperty.RegisterDirect<AbstractProgressBar, double>(nameof(StrokeThickness),
                                                                   o => o.StrokeThickness,
                                                                   (o, v) => o.StrokeThickness = v);

   /// <summary>
   /// Gets or sets a value indicating whether the progress bar shows the actual value or a generic,
   /// continues progress indicator (indeterminate state).
   /// </summary>
   public bool IsIndeterminate
   {
      get => GetValue(IsIndeterminateProperty);
      set => SetValue(IsIndeterminateProperty, value);
   }

   /// <summary>
   /// Gets or sets a value indicating whether progress text will be shown.
   /// </summary>
   public bool ShowProgressInfo
   {
      get => GetValue(ShowProgressInfoProperty);
      set => SetValue(ShowProgressInfoProperty, value);
   }

   /// <summary>
   /// Gets or sets the format string applied to the internally calculated progress text before it is shown.
   /// </summary>
   public string ProgressTextFormat
   {
      get => GetValue(ProgressTextFormatProperty);
      set => SetValue(ProgressTextFormatProperty, value);
   }

   public Color? TrailColor
   {
      get => GetValue(TrailColorProperty);
      set => SetValue(TrailColorProperty, value);
   }

   public PenLineCap StrokeLineCap
   {
      get => GetValue(StrokeLineCapProperty);
      set => SetValue(StrokeLineCapProperty, value);
   }

   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }

   public ProgressStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }

   protected double _percentage;

   /// <summary>
   /// Gets the overall percentage complete of the progress 
   /// </summary>
   /// <remarks>
   /// This read-only property is automatically calculated using the current <see cref="RangeBase.Value"/> and
   /// the effective range (<see cref="RangeBase.Maximum"/> - <see cref="RangeBase.Minimum"/>).
   /// </remarks>
   public double Percentage
   {
      get => _percentage;
      private set => SetAndRaise(PercentageProperty, ref _percentage, value);
   }

   public IBrush? IndicatorBarBrush
   {
      get => GetValue(IndicatorBarBrushProperty);
      set => SetValue(IndicatorBarBrushProperty, value);
   }

   public double IndicatorThickness
   {
      get => GetValue(IndicatorThicknessProperty);
      set => SetValue(IndicatorThicknessProperty, value);
   }

   public IBrush? SuccessThresholdBrush
   {
      get => GetValue(SuccessThresholdBrushProperty);
      set => SetValue(SuccessThresholdBrushProperty, value);
   }

   public double SuccessThreshold
   {
      get => GetValue(SuccessThresholdProperty);
      set => SetValue(SuccessThresholdProperty, value);
   }

   private SizeType _effectiveSizeType;

   protected SizeType EffectiveSizeType
   {
      get => _effectiveSizeType;
      set => SetAndRaise(EffectiveSizeTypeProperty, ref _effectiveSizeType, value);
   }

   private double _strokeThickness;

   protected double StrokeThickness
   {
      get => _strokeThickness;
      set => SetAndRaise(StrokeThicknessProperty, ref _strokeThickness, value);
   }

   static AbstractProgressBar()
   {
      AffectsMeasure<AbstractProgressBar>(EffectiveSizeTypeProperty,
                                          ShowProgressInfoProperty,
                                          ProgressTextFormatProperty);
      AffectsRender<AbstractProgressBar>(IndicatorBarBrushProperty,
                                         StrokeLineCapProperty,
                                         TrailColorProperty,
                                         StrokeThicknessProperty,
                                         SuccessThresholdBrushProperty,
                                         SuccessThresholdProperty,
                                         ValueProperty);
   }

   public AbstractProgressBar()
   {
      _controlTokenBinder = new ControlTokenBinder(this);
      _customStyle = this;
      _effectiveSizeType = SizeType;
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _customStyle.AfterUiStructureReady();
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);

      if (e.Property == ValueProperty ||
          e.Property == MinimumProperty ||
          e.Property == MaximumProperty ||
          e.Property == IsIndeterminateProperty ||
          e.Property == ProgressTextFormatProperty) {
         UpdateProgress();
      }

      _customStyle.HandlePropertyChangedForStyle(e);
   }

   protected virtual Label GetOrCreatePercentInfoLabel()
   {
      if (_percentageLabel is null) {
         _percentageLabel = new Label
         {
            Padding = new Thickness(0),
            VerticalContentAlignment = VerticalAlignment.Center,
         };
         BindUtils.RelayBind(this, "IsEnabled", _percentageLabel);
      }

      return _percentageLabel;
   }

   protected abstract SizeType CalculateEffectiveSizeType(double size);
   protected abstract Rect GetProgressBarRect(Rect controlRect);
   protected abstract Rect GetExtraInfoRect(Rect controlRect);
   protected abstract void RenderGroove(DrawingContext context);
   protected abstract void RenderIndicatorBar(DrawingContext context);
   protected abstract void CalculateStrokeThickness();

   protected virtual void NotifyEffectSizeTypeChanged()
   {
      ApplyEffectiveSizeTypeStyleConfig();
      CalculateStrokeThickness();
   }

   private void UpdateProgress()
   {
      var percent = Math.Abs(Maximum - Minimum) < double.Epsilon ? 1.0 : (Value - Minimum) / (Maximum - Minimum);
      Percentage = percent * 100;
      NotifyUpdateProgress();
   }

   protected virtual void NotifyUpdateProgress()
   {
      if (ShowProgressInfo &&
          _percentageLabel != null &&
          _exceptionCompletedIcon != null &&
          _successCompletedIcon != null) {
         if (Status == ProgressStatus.Exception) {
            _percentageLabel.IsVisible = false;
            _exceptionCompletedIcon.IsVisible = true;
            _successCompletedIcon.IsVisible = false;
         } else {
            if (MathUtils.AreClose(100, Percentage)) {
               _percentageLabel.IsVisible = false;
               _successCompletedIcon.IsVisible = true;
            } else {
               _successCompletedIcon.IsVisible = false;
               _exceptionCompletedIcon.IsVisible = false;
               _percentageLabel.IsVisible = true;
            }
            _percentageLabel.Content = string.Format(ProgressTextFormat, _percentage);
         }

         NotifyHandleExtraInfoVisibility();
      }
   }

   protected virtual void NotifyHandleExtraInfoVisibility() { }
}