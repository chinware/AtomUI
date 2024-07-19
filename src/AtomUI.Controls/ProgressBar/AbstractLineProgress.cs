using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
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
   protected Size _extraInfoSize = Size.Infinity;
   protected Rect _grooveRect;

   public AbstractLineProgress()
   {
      _sizeTypeThresholdValue = new Dictionary<SizeType, SizeTypeThresholdValue>();
   }
   
   // 根据当前的状态进行计算
   protected virtual Size CalculateExtraInfoSize(double fontSize)
   {
      if (Status == ProgressStatus.Exception || MathUtils.AreClose(Value, Maximum)) {
         // 只要图标
         if (EffectiveSizeType == SizeType.Large || EffectiveSizeType == SizeType.Middle) {
            return new Size(_lineInfoIconSizeToken, _lineInfoIconSizeToken);
         } 
         return new Size(_lineInfoIconSizeSMToken, _lineInfoIconSizeSMToken);
      }
      var textSize = TextUtils.CalculateTextSize(string.Format(ProgressTextFormat, Value), FontFamily, fontSize);
      return textSize;
   }

   protected override void ApplyEffectiveSizeTypeStyleConfig()
   {
      base.ApplyEffectiveSizeTypeStyleConfig();
      if (EffectiveSizeType == SizeType.Large || EffectiveSizeType == SizeType.Middle) {
         BindUtils.CreateTokenBinding(this, FontSizeProperty, GlobalResourceKey.FontSize);
      } else {
         BindUtils.CreateTokenBinding(this, FontSizeProperty, GlobalResourceKey.FontSizeSM);
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
   
   protected virtual void SetupAlignment()
   {
      HorizontalAlignment = HorizontalAlignment.Left;
      VerticalAlignment = VerticalAlignment.Top;
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
      base.NotifyApplyFixedStyleConfig();
      BindUtils.CreateTokenBinding(this, LineProgressPaddingTokenProperty, ProgressBarResourceKey.LineProgressPadding);
      BindUtils.CreateTokenBinding(this, LineExtraInfoMarginTokenProperty, ProgressBarResourceKey.LineExtraInfoMargin);
      BindUtils.CreateTokenBinding(this, FontSizeTokenProperty, GlobalResourceKey.FontSize);
      BindUtils.CreateTokenBinding(this, FontSizeSMTokenProperty, GlobalResourceKey.FontSizeSM);
      
      BindUtils.CreateTokenBinding(this, LineInfoIconSizeTokenProperty, ProgressBarResourceKey.LineInfoIconSize);
      BindUtils.CreateTokenBinding(this, LineInfoIconSizeSMTokenProperty, ProgressBarResourceKey.LineInfoIconSizeSM);
   }

   protected override void CreateCompletedIcons()
   {
      _exceptionCompletedIcon = new PathIcon
      {
         Kind = "CloseCircleFilled",
         HorizontalAlignment = HorizontalAlignment.Left
      };
      BindUtils.CreateTokenBinding(_exceptionCompletedIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
      BindUtils.CreateTokenBinding(_exceptionCompletedIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
      _successCompletedIcon = new PathIcon
      {
         Kind = "CheckCircleFilled",
         HorizontalAlignment = HorizontalAlignment.Left
      };
      BindUtils.CreateTokenBinding(_successCompletedIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorSuccess);
      BindUtils.CreateTokenBinding(_successCompletedIcon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
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
            BindUtils.CreateTokenBinding(_exceptionCompletedIcon!, WidthProperty, 
                                            ProgressBarResourceKey.LineInfoIconSize, BindingPriority.LocalValue);
        
            BindUtils.CreateTokenBinding(_exceptionCompletedIcon!, HeightProperty, 
                                            ProgressBarResourceKey.LineInfoIconSize, BindingPriority.LocalValue);
     
            BindUtils.CreateTokenBinding(_successCompletedIcon!, WidthProperty, 
                                            ProgressBarResourceKey.LineInfoIconSize, BindingPriority.LocalValue);
            BindUtils.CreateTokenBinding(_successCompletedIcon!, HeightProperty, 
                                            ProgressBarResourceKey.LineInfoIconSize, BindingPriority.LocalValue);
            BindUtils.CreateTokenBinding(this, FontSizeProperty, GlobalResourceKey.FontSize);
         
         } else {
            BindUtils.CreateTokenBinding( _exceptionCompletedIcon!, WidthProperty,
                                            ProgressBarResourceKey.LineInfoIconSizeSM, BindingPriority.LocalValue);
            BindUtils.CreateTokenBinding(_exceptionCompletedIcon!, HeightProperty, 
                                            ProgressBarResourceKey.LineInfoIconSizeSM, BindingPriority.LocalValue);
            BindUtils.CreateTokenBinding(_successCompletedIcon!, WidthProperty,
                                            ProgressBarResourceKey.LineInfoIconSizeSM, BindingPriority.LocalValue);
            BindUtils.CreateTokenBinding(_successCompletedIcon!, HeightProperty,
                                            ProgressBarResourceKey.LineInfoIconSizeSM, BindingPriority.LocalValue);
            BindUtils.CreateTokenBinding(this, FontSizeProperty, GlobalResourceKey.FontSizeSM);
         }
      }
   }
   
   protected virtual void NotifyOrientationChanged() {}
   private bool _lastCompletedStatus = false;
   protected override void NotifyHandleExtraInfoVisibility()
   {
      base.NotifyHandleExtraInfoVisibility();
      var currentStatus = false;
      if (MathUtils.AreClose(Value, Maximum)) {
         currentStatus = true;
        
      } else {
         currentStatus = false;
      }

      if (currentStatus != _lastCompletedStatus) {
         _lastCompletedStatus = currentStatus;
         _extraInfoSize = CalculateExtraInfoSize(FontSize);
      }
   }
}