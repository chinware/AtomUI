using AtomUI.Data;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class DotBadgeAdorner : Control, IControlCustomStyle
{
   public static readonly DirectProperty<DotBadgeAdorner, IBrush?> DotColorProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(DotColor),
         o => o.DotColor,
         (o, v) => o.DotColor = v);

   private IBrush? _dotColor;
   public IBrush? DotColor
   {
      get => _dotColor;
      set => SetAndRaise(DotColorProperty, ref _dotColor, value);
   }
   
   public static readonly DirectProperty<DotBadgeAdorner, DotBadgeStatus?> StatusProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, DotBadgeStatus?>(
         nameof(Status),
         o => o.Status,
         (o, v) => o.Status = v);

   private DotBadgeStatus? _status;
   public DotBadgeStatus? Status
   {
      get => _status;
      set => SetAndRaise(StatusProperty, ref _status, value);
   }
   
   public static readonly DirectProperty<DotBadgeAdorner, string?> TextProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, string?>(
         nameof(Text),
         o => o.Text,
         (o, v) => o.Text = v);

   private string? _text;
   public string? Text
   {
      get => _text;
      set => SetAndRaise(TextProperty, ref _text, value);
   }
   
   public static readonly DirectProperty<DotBadgeAdorner, bool> IsAdornerModeProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, bool>(
         nameof(IsAdornerMode),
         o => o.IsAdornerMode,
         (o, v) => o.IsAdornerMode = v);

   private bool _isAdornerMode = false;
   public bool IsAdornerMode
   {
      get => _isAdornerMode;
      set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
   }
   
   public static readonly DirectProperty<DotBadgeAdorner, IBrush?> EffectiveDotColorProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(EffectiveDotColor),
         o => o.EffectiveDotColor,
         (o, v) => o.EffectiveDotColor = v);
   
   private IBrush? _effectiveDotColor;
   
   // 当前有效的颜色
   public IBrush? EffectiveDotColor
   {
      get => _effectiveDotColor;
      set => SetAndRaise(EffectiveDotColorProperty, ref _effectiveDotColor, value);
   }
   
   public static readonly DirectProperty<DotBadgeAdorner, Point> OffsetProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, Point>(
         nameof(Offset),
         o => o.Offset,
         (o, v) => o.Offset = v);
   
   private Point _offset;

   public Point Offset
   {
      get => _offset;
      set => SetAndRaise(OffsetProperty, ref _offset, value);
   }
   
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private Label? _textLabel;
   private BoxShadows _boxShadows;

   static DotBadgeAdorner()
   {
      AffectsMeasure<DotBadge>(TextProperty, IsAdornerModeProperty);
      AffectsRender<DotBadge>(EffectiveDotColorProperty, OffsetProperty);
   }

   public DotBadgeAdorner()
   {
      _customStyle = this;
      _controlTokenBinder = new ControlTokenBinder(this, BadgeToken.ID);
   }
   
   void IControlCustomStyle.SetupUi()
   {
      _textLabel = new Label
      {
         Content = Text,
         HorizontalAlignment = HorizontalAlignment.Left,
         VerticalAlignment = VerticalAlignment.Center,
         HorizontalContentAlignment = HorizontalAlignment.Center,
         VerticalContentAlignment = VerticalAlignment.Center,
         Padding = new Thickness(0),
      };
      
      LogicalChildren.Add(_textLabel);
      VisualChildren.Add(_textLabel);
      _customStyle.ApplyFixedStyleConfig();
      SetupEffectiveDotColor();
      BuildBoxShadow();
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(ColorTextPlaceholderTokenProperty, GlobalResourceKey.ColorTextPlaceholder);
      _controlTokenBinder.AddControlBinding(ColorErrorTokenProperty, GlobalResourceKey.ColorError);
      _controlTokenBinder.AddControlBinding(ColorWarningTokenProperty, GlobalResourceKey.ColorWarning);
      _controlTokenBinder.AddControlBinding(ColorSuccessTokenProperty, GlobalResourceKey.ColorSuccess);
      _controlTokenBinder.AddControlBinding(ColorInfoTokenProperty, GlobalResourceKey.ColorInfo);
      _controlTokenBinder.AddControlBinding(MarginXSTokenProperty, GlobalResourceKey.MarginXS);
      
      _controlTokenBinder.AddControlBinding(DotSizeTokenProperty, BadgeResourceKey.DotSize);
      _controlTokenBinder.AddControlBinding(StatusSizeTokenProperty, BadgeResourceKey.StatusSize);
      _controlTokenBinder.AddControlBinding(BadgeColorTokenProperty, BadgeResourceKey.BadgeColor);
      _controlTokenBinder.AddControlBinding(BadgeShadowSizeTokenProperty, BadgeResourceKey.BadgeShadowSize);
      _controlTokenBinder.AddControlBinding(BadgeShadowColorTokenProperty, BadgeResourceKey.BadgeShadowColor);
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   private IBrush? GetStatusColor(DotBadgeStatus status)
   {
      if (status == DotBadgeStatus.Error) {
         return _colorErrorToken;
      } else if (status == DotBadgeStatus.Processing) {
         return _colorInfoToken;
      } else if (status == DotBadgeStatus.Success) {
         return _colorSuccessToken;
      } else if (status == DotBadgeStatus.Warning) {
         return _colorWarningToken;
      } else {
         return _colorTextPlaceholderToken;
      }
   }

   private void SetupEffectiveDotColor()
   {
      if (_dotColor is not null) {
         EffectiveDotColor = _dotColor;
      } else if (Status.HasValue) {
         EffectiveDotColor = GetStatusColor(Status.Value);
      }

      if (EffectiveDotColor is null) {
         EffectiveDotColor = _badgeColorToken;
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var targetWidth = 0d;
      var targetHeight = 0d;
      if (IsAdornerMode) {
         targetWidth = availableSize.Width;
         targetHeight = availableSize.Height;
      } else {
         var textSize = base.MeasureOverride(availableSize);
         targetWidth += _statusSizeToken;
         targetWidth += textSize.Width;
         targetHeight += Math.Max(textSize.Height, _statusSizeToken);
         if (textSize.Width > 0) {
            targetWidth += _marginXSToken;
         }
      }
      return new Size(targetWidth, targetHeight);
   }

   protected override Size ArrangeOverride(Size finalSize)
   {
      if (!IsAdornerMode) {
         double textOffsetX = 0;
         if (IsAdornerMode) {
            textOffsetX += _dotSizeToken;
         } else {
            textOffsetX += _statusSizeToken;
         }

         textOffsetX += _marginXSToken;
         var textRect = new Rect(new Point(textOffsetX, 0), _textLabel!.DesiredSize);
         _textLabel.Arrange(textRect);
      }
      return finalSize;
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (e.Property == IsAdornerModeProperty) {
         var newValue = e.GetNewValue<bool>();
         if (_textLabel is not null) {
            _textLabel.IsVisible = !newValue;
         }
      } else if (e.Property == BadgeShadowSizeTokenProperty ||
                 e.Property == BadgeShadowColorTokenProperty) {
         BuildBoxShadow();
      }

      if (_initialized) {
         if (e.Property == StatusProperty ||
             e.Property == DotColorProperty) {
            SetupEffectiveDotColor();
         }
      }
   }

   public override void Render(DrawingContext context)
   {
      var dotSize = 0d;
      if (IsAdornerMode) {
         dotSize = _dotSizeToken;
      } else {
         dotSize = _statusSizeToken;
      }
      
      var offsetX = 0d;
      var offsetY = 0d;
      if (IsAdornerMode) {
         offsetX = DesiredSize.Width - dotSize / 2;
         offsetY = -dotSize / 2;
         offsetX -= Offset.X;
         offsetY += Offset.Y;
      } else { 
         offsetY = (DesiredSize.Height - dotSize) / 2;
      }
      
      var dotRect = new Rect(new Point(offsetX, offsetY), new Size(dotSize, dotSize));
      context.DrawRectangle(EffectiveDotColor, null, dotRect, dotSize, dotSize, _boxShadows);
   }

   private void BuildBoxShadow()
   {
      _boxShadows = new BoxShadows(new BoxShadow()
      {
         OffsetX = 0,
         OffsetY = 0,
         Blur = 0,
         Spread = _badgeShadowSizeToken,
         Color = ((SolidColorBrush)_badgeShadowColorToken!).Color
      });
   }
}