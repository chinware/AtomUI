using AtomUI.ColorSystem;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;

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
   
   public static readonly StyledProperty<DotBadgeStatus> StatusProperty 
      = AvaloniaProperty.Register<DotBadge, DotBadgeStatus>(
         nameof(Status));
   
   public static readonly StyledProperty<string?> TextProperty 
      = AvaloniaProperty.Register<DotBadge, string?>(
         nameof(Status));
   
   public static readonly StyledProperty<Control?> DecoratedTargetProperty =
      AvaloniaProperty.Register<Decorator, Control?>(nameof(DotBadge));
   
   public string? DotColor
   {
      get => GetValue(DotColorProperty);
      set => SetValue(DotColorProperty, value);
   }

   public DotBadgeStatus Status
   {
      get => GetValue(StatusProperty);
      set => SetValue(StatusProperty, value);
   }

   public string? Text
   {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
   }

   public Control? DecoratedTarget
   {
      get => GetValue(DecoratedTargetProperty);
      set => SetValue(DecoratedTargetProperty, value);
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
   
   void IControlCustomStyle.SetupUi()
   {
      _dotBadgeAdorner = new DotBadgeAdorner();
      _customStyle.ApplyFixedStyleConfig();
      HandleDecoratedTargetChanged();
      if (DotColor is not null) {
         SetupTagColorInfo(DotColor);
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      if (_dotBadgeAdorner is not null) {
         BindUtils.RelayBind(this, StatusProperty, _dotBadgeAdorner, DotBadgeAdorner.StatusProperty);
         BindUtils.RelayBind(this, TextProperty, _dotBadgeAdorner, DotBadgeAdorner.TextProperty);
      }
   }

   private void HandleDecoratedTargetChanged()
   {
      if (DecoratedTarget is null && _dotBadgeAdorner is not null) {
         VisualChildren.Add(_dotBadgeAdorner);
         LogicalChildren.Add(_dotBadgeAdorner);
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
            SetupTagColorInfo(e.GetNewValue<string>());
         }
      }
   }
   
   private void SetupTagColorInfo(string colorStr)
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