using AtomUI.Media;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Layout;

namespace AtomUI.Controls;

public enum LinePercentAlignment
{
   Start,
   Center,
   End
}

internal class SizeTypeThresholdValue
{
   internal double NormalStateValue { get; set; } 
   internal double InnerStateValue { get; set; }
}

public abstract partial class AbstractLineProgress : AbstractProgressBar
{
   protected const double LARGE_STROKE_THICKNESS = 10;
   protected const double MIDDLE_STROKE_THICKNESS = 8;
   protected const double SMALL_STROKE_THICKNESS = 6;
   
   /// <summary>
   /// Defines the <see cref="Orientation"/> property.
   /// </summary>
   public static readonly StyledProperty<Orientation> OrientationProperty =
      AvaloniaProperty.Register<AbstractProgressBar, Orientation>(nameof(Orientation), Orientation.Horizontal);
   
   /// <summary>
   /// Gets or sets the orientation of the <see cref="ProgressBar"/>.
   /// </summary>
   public Orientation Orientation
   {
      get => GetValue(OrientationProperty);
      set => SetValue(OrientationProperty, value);
   }
   
   internal Dictionary<SizeType, SizeTypeThresholdValue> _sizeTypeThresholdValue;
   protected Size _extraInfoSize;

   public AbstractLineProgress()
   {
      _sizeTypeThresholdValue = new Dictionary<SizeType, SizeTypeThresholdValue>();
   }
   
   protected Size CalculateExtraInfoSize(double fontSize)
   {
      var textSize = TextUtils.CalculateTextSize(string.Format(ProgressTextFormat, 100), FontFamily, fontSize);
      if (Orientation == Orientation.Vertical) {
         textSize = new Size(textSize.Height, textSize.Width);
      }

      return textSize;
   }

   protected override void ApplyEffectiveSizeTypeStyleConfig()
   {
      base.ApplyEffectiveSizeTypeStyleConfig();
      if (EffectiveSizeType == SizeType.Large || EffectiveSizeType == SizeType.Middle) {
         _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      } else {
         _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSizeSM);
      }
   }

   protected override void NotifySetupUi()
   {
      base.NotifySetupUi();
      var calculateEffectiveSize = false;
      double sizeValue = 0;
      if (Orientation == Orientation.Horizontal && !double.IsNaN(Height)) {
         sizeValue = Height;
         calculateEffectiveSize = true;
      } else if (Orientation == Orientation.Vertical && !double.IsNaN(Width)) {
         sizeValue = Width;
         calculateEffectiveSize = true;
      }

      if (calculateEffectiveSize) {
         EffectiveSizeType = CalculateEffectiveSizeType(sizeValue);
      }

      _extraInfoSize = CalculateExtraInfoSize(FontSize);
      SetupAlignment();
   }

   private void SetupAlignment()
   {
      if (Orientation == Orientation.Horizontal) {
         if (!double.IsNaN(Width)) {
            HorizontalAlignment = HorizontalAlignment.Left;
         } else {
            HorizontalAlignment = HorizontalAlignment.Stretch;
         }
      } else {
         if (!double.IsNaN(Height)) {
            VerticalAlignment = VerticalAlignment.Top;
         } else {
            VerticalAlignment = VerticalAlignment.Center;
         }
      }
   }

   protected override void NotifyPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.NotifyPropertyChanged(e);
      if (e.Property == HeightProperty || e.Property == WidthProperty) {
         SetupAlignment();
      }
      if (_initialized) {
         if ((e.Property == WidthProperty && Orientation == Orientation.Vertical) ||
             (e.Property == HeightProperty && Orientation == Orientation.Horizontal)) {
            EffectiveSizeType = CalculateEffectiveSizeType(e.GetNewValue<double>());
            CalculateStrokeThickness();
         } else if (e.Property == EffectiveSizeTypeProperty) {
            _extraInfoSize = CalculateExtraInfoSize(FontSize);
         }
      }
   }

   protected override void NotifyApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(LineProgressPaddingTokenProperty, ProgressBarResourceKey.LineProgressPadding);
      _tokenResourceBinder.AddBinding(LineExtraInfoMarginTokenProperty, ProgressBarResourceKey.LineExtraInfoMargin);
      _tokenResourceBinder.AddBinding(FontSizeTokenProperty, GlobalResourceKey.FontSize);
      _tokenResourceBinder.AddBinding(FontSizeSMTokenProperty, GlobalResourceKey.FontSizeSM);
   }

   protected override void CreateCompletedIcons()
   {
      _exceptionCompletedIcon = new PathIcon
      {
         Kind = "CheckCircleFilled",
      };
      _successCompletedIcon = new PathIcon
      {
         Kind = "CloseCircleFilled"
      };
   }
}