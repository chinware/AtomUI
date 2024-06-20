using AtomUI.Media;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Data;
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

   protected override void NotifyUiStructureReady()
   {
      base.NotifyUiStructureReady();
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
      NotifyOrientationChanged();
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
         } else if (e.Property == OrientationProperty) {
            NotifyOrientationChanged();
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
         Kind = "CloseCircleFilled",
         HorizontalAlignment = HorizontalAlignment.Left
      };
      _tokenResourceBinder.AddBinding(_exceptionCompletedIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
      _tokenResourceBinder.AddBinding(_exceptionCompletedIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
      _successCompletedIcon = new PathIcon
      {
         Kind = "CheckCircleFilled",
         HorizontalAlignment = HorizontalAlignment.Left
      };
      _tokenResourceBinder.AddBinding(_successCompletedIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorSuccess);
      _tokenResourceBinder.AddBinding(_successCompletedIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
      _successCompletedIcon.IsVisible = false;
      _exceptionCompletedIcon.IsVisible = false;
      AddChildControl(_exceptionCompletedIcon);
      AddChildControl(_successCompletedIcon);
   }
   
   protected override void NotifyEffectSizeTypeChanged()
   {
      base.NotifyEffectSizeTypeChanged();
      if (_initialized) {
         if (EffectiveSizeType == SizeType.Large) {
            _tokenResourceBinder.AddBinding(_exceptionCompletedIcon!, WidthProperty, 
                                            ProgressBarResourceKey.LineInfoIconSize, BindingPriority.LocalValue);
        
            _tokenResourceBinder.AddBinding(_exceptionCompletedIcon!, HeightProperty, 
                                            ProgressBarResourceKey.LineInfoIconSize, BindingPriority.LocalValue);
     
            _tokenResourceBinder.AddBinding(_successCompletedIcon!, WidthProperty, 
                                            ProgressBarResourceKey.LineInfoIconSize, BindingPriority.LocalValue);
            _tokenResourceBinder.AddBinding(_successCompletedIcon!, HeightProperty, 
                                            ProgressBarResourceKey.LineInfoIconSize, BindingPriority.LocalValue);
            _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSize);
         
         } else {
            _tokenResourceBinder.AddBinding(_exceptionCompletedIcon!, WidthProperty,
                                            ProgressBarResourceKey.LineInfoIconSizeSM, BindingPriority.LocalValue);
            _tokenResourceBinder.AddBinding(_exceptionCompletedIcon!, HeightProperty, 
                                            ProgressBarResourceKey.LineInfoIconSizeSM, BindingPriority.LocalValue);
            _tokenResourceBinder.AddBinding(_successCompletedIcon!, WidthProperty,
                                            ProgressBarResourceKey.LineInfoIconSizeSM, BindingPriority.LocalValue);
            _tokenResourceBinder.AddBinding(_successCompletedIcon!, HeightProperty,
                                            ProgressBarResourceKey.LineInfoIconSizeSM, BindingPriority.LocalValue);
            _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSizeSM);
         }
      }
   }
   
   protected virtual void NotifyOrientationChanged() {}
}