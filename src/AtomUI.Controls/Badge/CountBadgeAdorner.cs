using System.Globalization;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace AtomUI.Controls;

internal partial class CountBadgeAdorner : Control, IControlCustomStyle
{
   public static readonly DirectProperty<CountBadgeAdorner, IBrush?> BadgeColorProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, IBrush?>(
         nameof(BadgeColor),
         o => o.BadgeColor,
         (o, v) => o.BadgeColor = v);

   private IBrush? _badgeColor;

   public IBrush? BadgeColor
   {
      get => _badgeColor;
      set => SetAndRaise(BadgeColorProperty, ref _badgeColor, value);
   }

   public static readonly DirectProperty<CountBadgeAdorner, bool> IsAdornerModeProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, bool>(
         nameof(IsAdornerMode),
         o => o.IsAdornerMode,
         (o, v) => o.IsAdornerMode = v);

   private bool _isAdornerMode = false;

   public bool IsAdornerMode
   {
      get => _isAdornerMode;
      set => SetAndRaise(IsAdornerModeProperty, ref _isAdornerMode, value);
   }

   public static readonly DirectProperty<CountBadgeAdorner, Point> OffsetProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, Point>(
         nameof(Offset),
         o => o.Offset,
         (o, v) => o.Offset = v);

   private Point _offset;

   public Point Offset
   {
      get => _offset;
      set => SetAndRaise(OffsetProperty, ref _offset, value);
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

   public static readonly DirectProperty<CountBadgeAdorner, int> CountProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, int>(
         nameof(Count),
         o => o.Count,
         (o, v) => o.Count = v);

   private int _count;

   public int Count
   {
      get => _count;
      set => SetAndRaise(CountProperty, ref _count, value);
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
   
   public static readonly StyledProperty<FontFamily> FontFamilyProperty =
      TextElement.FontFamilyProperty.AddOwner<CountBadgeAdorner>();
   
   public FontFamily FontFamily
   {
      get => GetValue(FontFamilyProperty);
      set => SetValue(FontFamilyProperty, value);
   }

   // 不知道为什么这个值会被 AdornerLayer 重写
   // 非常不优美，但是能工作
   internal RelativePoint? AnimationRenderTransformOrigin;

   static CountBadgeAdorner()
   {
      AffectsMeasure<CountBadgeAdorner>(OverflowCountProperty,
                                        SizeProperty,
                                        CountProperty);
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

   void IControlCustomStyle.SetupUi()
   {
      _customStyle.ApplyFixedStyleConfig();
      BuildBoxShadow();
      BuildCountText();
      CalculateCountTextSize();
      BuildFormattedTexts();
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, IndicatorHeightTokenProperty, BadgeResourceKey.IndicatorHeight);
      BindUtils.CreateTokenBinding(this, IndicatorHeightSMTokenProperty, BadgeResourceKey.IndicatorHeightSM);
      BindUtils.CreateTokenBinding(this, TextFontSizeTokenProperty, BadgeResourceKey.TextFontSize);
      BindUtils.CreateTokenBinding(this, TextFontSizeSMTokenProperty, BadgeResourceKey.TextFontSizeSM);
      BindUtils.CreateTokenBinding(this, TextFontWeightTokenProperty, BadgeResourceKey.TextFontWeight);
      BindUtils.CreateTokenBinding(this, BadgeColorTokenProperty, BadgeResourceKey.BadgeColor);
      BindUtils.CreateTokenBinding(this, BadgeShadowSizeTokenProperty, BadgeResourceKey.BadgeShadowSize);
      BindUtils.CreateTokenBinding(this, BadgeShadowColorTokenProperty, BadgeResourceKey.BadgeShadowColor);
      BindUtils.CreateTokenBinding(this, BadgeTextColorTokenProperty, BadgeResourceKey.BadgeTextColor);
      BindUtils.CreateTokenBinding(this, PaddingXSTokenProperty, GlobalResourceKey.PaddingXS);
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

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (_initialized) {
         if (e.Property == BadgeShadowSizeTokenProperty ||
             e.Property == BadgeShadowColorTokenProperty) {
            BuildBoxShadow();
         }
         if (Parent is not null && (e.Property == CountProperty || e.Property == OverflowCountProperty)) {
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
      var lineHeight = GetTextLineHeight();
      var targetWidth = lineHeight;
      var targetHeight = lineHeight;
      if (_countText?.Length > 1) {
         targetWidth += _paddingXSToken;
         if (_count > _overflowCount) {
            targetWidth += _paddingXSToken;
         }
      }
      targetWidth = Math.Max(targetWidth, _countTextSize.Width);
      targetHeight = Math.Max(targetHeight, _countTextSize.Height);
      return new Size(targetWidth, targetHeight);
   }

   private void CalculateCountTextSize(bool force = false)
   {
      if (force || _countTextSize == default) {
         var fontSize = GetTextFontSize();
         var typeface = new Typeface(FontFamily, FontStyle.Normal, _textFontWeightToken, FontStretch.Normal);
         var textLayout = new TextLayout(_countText, 
                                         typeface:typeface,
                                         fontFeatures: null,
                                         fontSize:fontSize,
                                         foreground:null,
                                         lineHeight:GetTextLineHeight());
         _countTextSize = new Size(Math.Round(textLayout.Width), Math.Round(textLayout.Height));
      }
   }

   private void BuildCountText()
   {
      if (_count > _overflowCount) {
         _countText = $"{_overflowCount}+";
      } else {
         _countText = $"{_count}";
      }
   }

   private double GetTextFontSize()
   {
      if (Size == CountBadgeSize.Small) {
         return _textFontSizeSMToken;
      }

      return _textFontSizeToken;
   }

   protected double GetTextLineHeight()
   {
      if (Size == CountBadgeSize.Small) {
         return _indicatorHeightSMToken;
      }

      return _indicatorHeightToken;
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
      
      context.DrawPilledRect(BadgeColor ?? _badgeColorToken, null, badgeRect, Orientation.Horizontal, _boxShadows);
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
            if (_count > _overflowCount) {
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
      var typeface = new Typeface(FontFamily, FontStyle.Normal, _textFontWeightToken, FontStretch.Normal);
      var formattedText = new FormattedText(text, CultureInfo.CurrentUICulture, GetFlowDirection(this),
                                            typeface, 1, _badgeTextColorToken);
      formattedText.SetFontSize(GetTextFontSize());
      formattedText.TextAlignment = TextAlignment.Left;
      formattedText.LineHeight = GetTextLineHeight();
      return formattedText;
   }
}