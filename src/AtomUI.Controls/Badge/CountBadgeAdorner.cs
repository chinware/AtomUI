using System.Globalization;
using AtomUI.Media;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Styling;

namespace AtomUI.Controls;

internal class CountBadgeAdorner : Control, IControlCustomStyle
{
   public static readonly StyledProperty<IBrush?> BadgeColorProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, IBrush?>(
         nameof(BadgeColor));

   internal static readonly StyledProperty<IBrush?> BadgeTextColorProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, IBrush?>(
         nameof(BadgeTextColor));

   internal static readonly StyledProperty<double> TextFontSizeProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, double>(
         nameof(TextFontSize));

   internal static readonly StyledProperty<double> IndicatorHeightProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, double>(
         nameof(IndicatorHeight));

   internal static readonly StyledProperty<int> CountProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, int>(
         nameof(Count));

   internal static readonly StyledProperty<FontFamily> FontFamilyProperty =
      TextElement.FontFamilyProperty.AddOwner<CountBadgeAdorner>();

   internal static readonly StyledProperty<FontWeight> TextFontWeightProperty =
      TextElement.FontWeightProperty.AddOwner<CountBadgeAdorner>();

   internal static readonly StyledProperty<IBrush?> BadgeShadowColorProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, IBrush?>(
         nameof(BadgeShadowColor));

   internal static readonly StyledProperty<double> BadgeShadowSizeProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, double>(
         nameof(BadgeShadowSize));

   internal static readonly StyledProperty<double> PaddingInlineProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, double>(
         nameof(PaddingInline));

   public IBrush? BadgeColor
   {
      get => GetValue(BadgeColorProperty);
      set => SetValue(BadgeColorProperty, value);
   }

   internal IBrush? BadgeTextColor
   {
      get => GetValue(BadgeTextColorProperty);
      set => SetValue(BadgeTextColorProperty, value);
   }

   internal double TextFontSize
   {
      get => GetValue(TextFontSizeProperty);
      set => SetValue(TextFontSizeProperty, value);
   }

   internal double IndicatorHeight
   {
      get => GetValue(IndicatorHeightProperty);
      set => SetValue(IndicatorHeightProperty, value);
   }

   internal int Count
   {
      get => GetValue(CountProperty);
      set => SetValue(CountProperty, value);
   }

   internal FontFamily FontFamily
   {
      get => GetValue(FontFamilyProperty);
      set => SetValue(FontFamilyProperty, value);
   }

   internal FontWeight TextFontWeight
   {
      get => GetValue(TextFontWeightProperty);
      set => SetValue(TextFontWeightProperty, value);
   }

   internal IBrush? BadgeShadowColor
   {
      get => GetValue(BadgeShadowColorProperty);
      set => SetValue(BadgeShadowColorProperty, value);
   }

   internal double BadgeShadowSize
   {
      get => GetValue(BadgeShadowSizeProperty);
      set => SetValue(BadgeShadowSizeProperty, value);
   }

   internal double PaddingInline
   {
      get => GetValue(PaddingInlineProperty);
      set => SetValue(PaddingInlineProperty, value);
   }

   internal static readonly DirectProperty<DotBadgeAdorner, bool> IsAdornerModeProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, bool>(
         nameof(IsAdornerMode),
         o => o.IsAdornerMode,
         (o, v) => o.IsAdornerMode = v);

   private bool _isAdornerMode = false;

   internal bool IsAdornerMode
   {
      get => _isAdornerMode;
      set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
   }

   internal static readonly StyledProperty<Point> OffsetProperty =
      AvaloniaProperty.Register<CountBadgeAdorner, Point>(
         nameof(Offset));

   public Point Offset
   {
      get => GetValue(OffsetProperty);
      set => SetValue(OffsetProperty, value);
   }

   public static readonly DirectProperty<CountBadgeAdorner, int> OverflowCountProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, int>(
         nameof(OverflowCount),
         o => o.OverflowCount,
         (o, v) => o.OverflowCount = v);

   private int _overflowCount;

   public int OverflowCount
   {
      get => _overflowCount;
      set => SetAndRaise(OverflowCountProperty, ref _overflowCount, value);
   }

   public static readonly DirectProperty<CountBadgeAdorner, CountBadgeSize> SizeProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, CountBadgeSize>(
         nameof(Size),
         o => o.Size,
         (o, v) => o.Size = v);

   private CountBadgeSize _size;

   public CountBadgeSize Size
   {
      get => _size;
      set => SetAndRaise(SizeProperty, ref _size, value);
   }

   // 不知道为什么这个值会被 AdornerLayer 重写
   // 非常不优美，但是能工作
   internal RelativePoint? AnimationRenderTransformOrigin;

   static CountBadgeAdorner()
   {
      AffectsMeasure<CountBadgeAdorner>(OverflowCountProperty,
                                        SizeProperty,
                                        CountProperty,
                                        IsAdornerModeProperty);
      AffectsRender<CountBadgeAdorner>(BadgeColorProperty, OffsetProperty);
   }

   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private BoxShadows _boxShadows;
   private Size _countTextSize;
   private string? _countText;
   private List<FormattedText> _formattedTexts;

   public CountBadgeAdorner()
   {
      _customStyle = this;
      _formattedTexts = new List<FormattedText>();
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (Styles.Count == 0) {
         _customStyle.BuildStyles();
      }
   }

   void IControlCustomStyle.BuildStyles()
   {
      var commonStyle = new Style();
      commonStyle.Add(TextFontWeightProperty, BadgeResourceKey.TextFontWeight);
      commonStyle.Add(BadgeColorProperty, BadgeResourceKey.BadgeColor);
      commonStyle.Add(BadgeShadowSizeProperty, BadgeResourceKey.BadgeShadowSize);
      commonStyle.Add(BadgeShadowColorProperty, BadgeResourceKey.BadgeShadowColor);
      commonStyle.Add(BadgeTextColorProperty, BadgeResourceKey.BadgeTextColor);
      commonStyle.Add(PaddingInlineProperty, GlobalResourceKey.PaddingXS);
      Styles.Add(commonStyle);

      var defaultSizeStyle =
         new Style(selector => selector.PropertyEquals(SizeProperty, CountBadgeSize.Default));
      defaultSizeStyle.Add(TextFontSizeProperty, BadgeResourceKey.TextFontSize);
      defaultSizeStyle.Add(IndicatorHeightProperty, BadgeResourceKey.IndicatorHeight);
      Styles.Add(defaultSizeStyle);

      var smallSizeStyle = new Style(selector => selector.PropertyEquals(SizeProperty, CountBadgeSize.Small));
      smallSizeStyle.Add(TextFontSizeProperty, BadgeResourceKey.TextFontSizeSM);
      smallSizeStyle.Add(IndicatorHeightProperty, BadgeResourceKey.IndicatorHeightSM);
      Styles.Add(smallSizeStyle);
   }

   public override void ApplyTemplate()
   {
      if (!_initialized) {
         _customStyle.SetupTokenBindings();
         BuildBoxShadow();
         BuildCountText();
         CalculateCountTextSize();
         BuildFormattedTexts();
         _initialized = true;
      }
   }

   private void BuildBoxShadow()
   {
      if (BadgeShadowColor is not null) {
         _boxShadows = new BoxShadows(new BoxShadow()
         {
            OffsetX = 0,
            OffsetY = 0,
            Blur = 0,
            Spread = BadgeShadowSize,
            Color = ((SolidColorBrush)BadgeShadowColor).Color
         });
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (VisualRoot is not null) {
         if (e.Property == BadgeShadowSizeProperty ||
             e.Property == BadgeShadowColorProperty) {
            BuildBoxShadow();
         }

         if (e.Property == CountProperty || e.Property == OverflowCountProperty) {
            BuildCountText();
            CalculateCountTextSize(true);
            BuildFormattedTexts(true);
         }
      }
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      if (IsAdornerMode) {
         return availableSize;
      }

      return GetBadgePillSize();
   }

   protected Size GetBadgePillSize()
   {
      var targetWidth = IndicatorHeight;
      var targetHeight = IndicatorHeight;
      if (_countText?.Length > 1) {
         targetWidth += PaddingInline;
         if (Count > _overflowCount) {
            targetWidth += PaddingInline;
         }
      }

      targetWidth = Math.Max(targetWidth, _countTextSize.Width);
      targetHeight = Math.Max(targetHeight, _countTextSize.Height);
      return new Size(targetWidth, targetHeight);
   }

   private void CalculateCountTextSize(bool force = false)
   {
      if (force || _countTextSize == default) {
         var fontSize = TextFontSize;
         var typeface = new Typeface(FontFamily, FontStyle.Normal, TextFontWeight, FontStretch.Normal);
         var textLayout = new TextLayout(_countText,
                                         typeface: typeface,
                                         fontFeatures: null,
                                         fontSize: fontSize,
                                         foreground: null,
                                         lineHeight: IndicatorHeight);
         _countTextSize = new Size(Math.Round(textLayout.Width), Math.Round(textLayout.Height));
      }
   }

   private void BuildCountText()
   {
      if (Count > _overflowCount) {
         _countText = $"{_overflowCount}+";
      } else {
         _countText = $"{Count}";
      }
   }

   public override void Render(DrawingContext context)
   {
      var offsetX = 0d;
      var offsetY = 0d;
      var badgeSize = GetBadgePillSize();
      if (IsAdornerMode) {
         offsetX = DesiredSize.Width - badgeSize.Width / 2;
         offsetY = -badgeSize.Height / 2;
         offsetX += Offset.X;
         offsetY += Offset.Y;
      }

      var badgeRect = new Rect(new Point(offsetX, offsetY), badgeSize);

      if (RenderTransform is not null) {
         Point origin;
         if (AnimationRenderTransformOrigin.HasValue) {
            origin = AnimationRenderTransformOrigin.Value.ToPixels(badgeRect.Size);
         } else {
            origin = RenderTransformOrigin.ToPixels(badgeRect.Size);
         }

         var offset = Matrix.CreateTranslation(new Point(origin.X + offsetX, origin.Y + offsetY));
         var renderTransform = (-offset) * RenderTransform.Value * (offset);
         context.PushTransform(renderTransform);
      }

      context.DrawPilledRect(BadgeColor, null, badgeRect, Orientation.Horizontal, _boxShadows);
      // 计算合适的文字 x 坐标
      var textOffsetX = offsetX + (badgeSize.Width - _countTextSize.Width) / 2;
      var textOffsetY = offsetY + (badgeSize.Height - _countTextSize.Height) / 2;
      foreach (var formattedText in _formattedTexts) {
         context.DrawText(formattedText, new Point(textOffsetX, textOffsetY));
         textOffsetX += formattedText.Width;
      }
   }

   private void BuildFormattedTexts(bool force = false)
   {
      if (_formattedTexts.Count == 0 || force) {
         _formattedTexts.Clear();
         if (_countText is not null) {
            if (Count > _overflowCount) {
               // 生成一个即可
               _formattedTexts.Add(BuildFormattedText(_countText));
            } else {
               // 没有数字都生成一个
               foreach (var c in _countText) {
                  _formattedTexts.Add(BuildFormattedText(c.ToString()));
               }
            }
         }
      }
   }

   private FormattedText BuildFormattedText(string text)
   {
      var typeface = new Typeface(FontFamily, FontStyle.Normal, TextFontWeight, FontStretch.Normal);
      var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, GetFlowDirection(this),
                                            typeface, 1, BadgeTextColor);
      formattedText.SetFontSize(TextFontSize);
      formattedText.TextAlignment = TextAlignment.Left;
      formattedText.LineHeight = IndicatorHeight;
      return formattedText;
   }
}