using AtomUI.ColorSystem;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

internal struct TagCalcColor
{
   public Color LightColor { get; set; } // 1 号色
   public Color LightBorderColor { get; set; } // 3 号色
   public Color DarkColor { get; set; } // 6 号色
   public Color TextColor { get; set; } // 7 号色
}

internal struct TagStatusCalcColor
{
   public Color Color { get; set; }
   public Color Background { get; set; }
   public Color BorderColor { get; set; }
}

public partial class Tag : IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private TokenResourceBinder _tokenResourceBinder;
   private ControlStyleState _styleState;
   private bool _isPresetColorTag = false;
   private bool _hasColorSet = false;
   private static Dictionary<TagPresetColor, TagCalcColor> _presetColorMap;
   private static Dictionary<TagStatus, TagStatusCalcColor> _statusColorMap;
   private static Dictionary<TagPresetColor, PresetColorType> _colorCodeMap;
   private Panel? _layoutPanel;
   private TextBlock? _textBlock;

   void IControlCustomStyle.InitOnConstruct()
   {
      _layoutPanel = new Panel();
   }

   void IControlCustomStyle.SetupUi()
   {
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      if (TagColor is not null) {
         SetupTagColorInfo(TagColor);
      }

      var tagContent = string.Empty;
      if (Content is string) {
         // 只接受字符串
         tagContent = Content as string;
      }

      _textBlock = new TextBlock
      {
         Text = tagContent,
         VerticalAlignment = VerticalAlignment.Center
      };
      _layoutPanel?.Children.Add(_textBlock);
      Content = _layoutPanel;
      SetupTagIcon();
      SetupTagClosable();
      _customStyle.SetupTransitions();
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(DefaultBgTokenProperty, TagResourceKey.DefaultBg);
      _tokenResourceBinder.AddBinding(DefaultForegroundTokenProperty, TagResourceKey.DefaultColor);
      _tokenResourceBinder.AddBinding(FontSizeProperty, TagResourceKey.TagFontSize);
      _tokenResourceBinder.AddBinding(TagLineHeightTokenProperty, TagResourceKey.TagLineHeight);
      _tokenResourceBinder.AddBinding(TagIconSizeTokenProperty, TagResourceKey.TagIconSize);
      _tokenResourceBinder.AddBinding(TagCloseIconSizeTokenProperty, TagResourceKey.TagCloseIconSize);
      _tokenResourceBinder.AddBinding(PaddingXXSTokenProperty, GlobalResourceKey.PaddingXXS);
      _tokenResourceBinder.AddBinding(PaddingProperty, TagResourceKey.TagPadding);
      _tokenResourceBinder.AddBinding(TagBorderlessBgTokenProperty, TagResourceKey.TagBorderlessBg);
      _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorBorder);
      _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      _tokenResourceBinder.AddBinding(ColorTextLightSolidTokenProperty, GlobalResourceKey.ColorTextLightSolid);

      Background = _defaultBackground;
      Foreground = _defaultForeground;
   }

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      _tokenResourceBinder.AddBinding(BorderThicknessProperty, "BorderThickness", BindingPriority.Style,
                                      new RenderScaleAwareThicknessConfigure(this, thickness =>
                                      {
                                         if (!Bordered) {
                                            return new Thickness(0);
                                         }

                                         return thickness;
                                      }));
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == ClosableProperty && _initialized) {
         SetupTagClosable();
      }
   }

   private void SetupPresetColorMap()
   {
      if (_presetColorMap.Count == 0) {
         // TODO 根据当前的主题风格设置，是否需要根据风格不一样进行动态调整呢？
         var activatedTheme = ThemeManager.Current.ActivatedTheme;
         var globalToken = activatedTheme?.GlobalToken;
         if (globalToken == null) {
            // 是否需要输出日志
            return;
         }

         foreach (var entry in _colorCodeMap) {
            var colorMap = globalToken.GetColorPalette(new PresetPrimaryColor(entry.Value))!;
            var calcColor = new TagCalcColor()
            {
               LightColor = colorMap.Color1,
               LightBorderColor = colorMap.Color3,
               DarkColor = colorMap.Color6,
               TextColor = colorMap.Color7
            };
            _presetColorMap.Add(entry.Key, calcColor);
         }
      }
   }

   private void SetupStatusColorMap()
   {
      if (_statusColorMap.Count == 0) {
         var activatedTheme = ThemeManager.Current.ActivatedTheme;
         var globalToken = activatedTheme?.GlobalToken;
         if (globalToken == null) {
            // 是否需要输出日志
            return;
         }

         var colorToken = globalToken.ColorToken;
         var colorSuccessToken = colorToken.ColorSuccessToken;
         var colorInfoToken = colorToken.ColorInfoToken;
         var colorWarningToken = colorToken.ColorWarningToken;
         var colorErrorToken = colorToken.ColorErrorToken;

         _statusColorMap.Add(TagStatus.Success, new TagStatusCalcColor
         {
            Color = colorSuccessToken.ColorSuccess,
            Background = colorSuccessToken.ColorSuccessBg,
            BorderColor = colorSuccessToken.ColorSuccessBorder
         });

         _statusColorMap.Add(TagStatus.Info, new TagStatusCalcColor
         {
            Color = colorInfoToken.ColorInfo,
            Background = colorInfoToken.ColorInfoBg,
            BorderColor = colorInfoToken.ColorInfoBorder
         });

         _statusColorMap.Add(TagStatus.Warning, new TagStatusCalcColor
         {
            Color = colorWarningToken.ColorWarning,
            Background = colorWarningToken.ColorWarningBg,
            BorderColor = colorWarningToken.ColorWarningBorder
         });

         _statusColorMap.Add(TagStatus.Error, new TagStatusCalcColor
         {
            Color = colorErrorToken.ColorError,
            Background = colorErrorToken.ColorErrorBg,
            BorderColor = colorErrorToken.ColorErrorBorder
         });
      }
   }

   private static void SetupColorCodeMap()
   {
      _colorCodeMap.Add(TagPresetColor.Red, PresetColorType.Red);
      _colorCodeMap.Add(TagPresetColor.Volcano, PresetColorType.Volcano);
      _colorCodeMap.Add(TagPresetColor.Orange, PresetColorType.Orange);
      _colorCodeMap.Add(TagPresetColor.Gold, PresetColorType.Gold);
      _colorCodeMap.Add(TagPresetColor.Yellow, PresetColorType.Yellow);
      _colorCodeMap.Add(TagPresetColor.Lime, PresetColorType.Lime);
      _colorCodeMap.Add(TagPresetColor.Green, PresetColorType.Green);
      _colorCodeMap.Add(TagPresetColor.Cyan, PresetColorType.Cyan);
      _colorCodeMap.Add(TagPresetColor.Blue, PresetColorType.Blue);
      _colorCodeMap.Add(TagPresetColor.GeekBlue, PresetColorType.GeekBlue);
      _colorCodeMap.Add(TagPresetColor.Purple, PresetColorType.Purple);
      _colorCodeMap.Add(TagPresetColor.Pink, PresetColorType.Pink);
      _colorCodeMap.Add(TagPresetColor.Magenta, PresetColorType.Magenta);
   }

   private Rect IconRect(Size controlSize)
   {
      var bevelRect = BevelRect(controlSize);
      var offsetX = bevelRect.Left + Padding.Left;
      var offsetY = bevelRect.Y + (bevelRect.Height - _tagIconSize) / 2;
      return new Rect(offsetX, offsetY, _tagIconSize, _tagIconSize);
   }

   private Rect CloseIconRect(Size controlSize)
   {
      var bevelRect = BevelRect(controlSize);
      var offsetX = bevelRect.Right - Padding.Right - _tagCloseIconSize;
      var offsetY = bevelRect.Y + (bevelRect.Height - _tagCloseIconSize) / 2;
      return new Rect(offsetX, offsetY, _tagCloseIconSize, _tagCloseIconSize);
   }

   private Rect BevelRect(Size controlSize)
   {
      var targetRect = new Rect(0, 0, controlSize.Width, controlSize.Height);
      return targetRect.Deflate(BorderThickness);
   }

   private Rect TextRect(Size controlSize)
   {
      var bevelRect = BevelRect(controlSize);
      var offsetX = bevelRect.Left + Padding.Left;
      if (Icon is not null) {
         offsetX += _tagIconSize + _paddingXXS;
      }

      // 这个时候已经算好了
      var textSize = _textBlock!.DesiredSize;
      var offsetY = bevelRect.Y + (bevelRect.Height - textSize.Height) / 2;
      return new Rect(offsetX, offsetY, textSize.Width, textSize.Height);
   }

   private void SetupTagColorInfo(string colorStr)
   {
      _isPresetColorTag = false;
      _hasColorSet = false;
      colorStr = colorStr.Trim().ToLower();

      Background = _defaultBackground;

      foreach (var entry in _presetColorMap) {
         if (entry.Key.ToString().ToLower() == colorStr) {
            var colorInfo = _presetColorMap[entry.Key];
            Foreground = new SolidColorBrush(colorInfo.TextColor);
            BorderBrush = new SolidColorBrush(colorInfo.LightBorderColor);
            Background = new SolidColorBrush(colorInfo.LightColor);
            _isPresetColorTag = true;
            return;
         }
      }

      foreach (var entry in _statusColorMap) {
         if (entry.Key.ToString().ToLower() == colorStr) {
            var colorInfo = _statusColorMap[entry.Key];
            Foreground = new SolidColorBrush(colorInfo.Color);
            BorderBrush = new SolidColorBrush(colorInfo.BorderColor);
            Background = new SolidColorBrush(colorInfo.Background);
            _isPresetColorTag = true;
            return;
         }
      }

      if (Color.TryParse(colorStr, out Color color)) {
         Bordered = false;
         Foreground = _colorTextLightSolid;
         _hasColorSet = true;
         Background = new SolidColorBrush(color);
      }
   }

   private void SetupTagClosable()
   {
      if (Closable) {
         if (CloseIcon is null) {
            CloseIcon = new PathIcon()
            {
               Kind = "CloseOutlined",
               Width = _tagCloseIconSize,
               Height = _tagCloseIconSize,
            };

            if (_hasColorSet && !_isPresetColorTag) {
               _tokenResourceBinder.AddBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorTextLightSolid);
            } else {
               _tokenResourceBinder.AddBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorIcon);
               _tokenResourceBinder.AddBinding(CloseIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
            }
         } else {
            CloseIcon.Width = _tagCloseIconSize;
            CloseIcon.Height = _tagCloseIconSize;
            if (CloseIcon.ThemeType != IconThemeType.TwoTone) {
               if (_hasColorSet && !_isPresetColorTag) {
                  _tokenResourceBinder.AddBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorTextLightSolid);
               } else {
                  _tokenResourceBinder.AddBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorIcon);
                  _tokenResourceBinder.AddBinding(CloseIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
               }
            }
         }

         CloseIcon.Cursor = new Cursor(StandardCursorType.Hand);
         _layoutPanel?.Children.Add(CloseIcon);
      } else {
         if (CloseIcon != null) {
            _layoutPanel?.Children.Remove(CloseIcon);
            CloseIcon = null;
         }
      }
   }

   private void SetupTagIcon()
   {
      if (Icon is not null) {
         Icon.Width = _tagIconSize;
         Icon.Height = _tagIconSize;
         _layoutPanel?.Children.Insert(0, Icon);
         if (_hasColorSet) {
            _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorTextLightSolid);
         } else if (_isPresetColorTag) {
            Icon.NormalFilledBrush = Foreground;
         }
      }
   }
}