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
   private ControlTokenBinder _controlTokenBinder;
   private ControlStyleState _styleState;
   private bool _isPresetColorTag = false;
   private bool _hasColorSet = false;
   private static Dictionary<PresetColorType, TagCalcColor> _presetColorMap;
   private static Dictionary<TagStatus, TagStatusCalcColor> _statusColorMap;
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
      _controlTokenBinder.AddControlBinding(DefaultBgTokenProperty, TagResourceKey.DefaultBg);
      _controlTokenBinder.AddControlBinding(DefaultForegroundTokenProperty, TagResourceKey.DefaultColor);
      _controlTokenBinder.AddControlBinding(FontSizeProperty, TagResourceKey.TagFontSize);
      _controlTokenBinder.AddControlBinding(TagLineHeightTokenProperty, TagResourceKey.TagLineHeight);
      _controlTokenBinder.AddControlBinding(TagIconSizeTokenProperty, TagResourceKey.TagIconSize);
      _controlTokenBinder.AddControlBinding(TagCloseIconSizeTokenProperty, TagResourceKey.TagCloseIconSize);
      _controlTokenBinder.AddControlBinding(PaddingXXSTokenProperty, GlobalResourceKey.PaddingXXS);
      _controlTokenBinder.AddControlBinding(PaddingProperty, TagResourceKey.TagPadding);
      _controlTokenBinder.AddControlBinding(TagBorderlessBgTokenProperty, TagResourceKey.TagBorderlessBg);
      _controlTokenBinder.AddControlBinding(BorderBrushProperty, GlobalResourceKey.ColorBorder);
      _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      _controlTokenBinder.AddControlBinding(ColorTextLightSolidTokenProperty, GlobalResourceKey.ColorTextLightSolid);

      Background = _defaultBackground;
      Foreground = _defaultForeground;
   }

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(BorderThicknessProperty, "BorderThickness", BindingPriority.Style,
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

         foreach (var entry in PresetPrimaryColor.AllColorTypes()) {
            var colorMap = globalToken.GetColorPalette(entry)!;
            var calcColor = new TagCalcColor()
            {
               LightColor = colorMap.Color1,
               LightBorderColor = colorMap.Color3,
               DarkColor = colorMap.Color6,
               TextColor = colorMap.Color7
            };
            _presetColorMap.Add(entry.Type, calcColor);
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
               _controlTokenBinder.AddControlBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorTextLightSolid);
            } else {
               _controlTokenBinder.AddControlBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorIcon);
               _controlTokenBinder.AddControlBinding(CloseIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
            }
         } else {
            CloseIcon.Width = _tagCloseIconSize;
            CloseIcon.Height = _tagCloseIconSize;
            if (CloseIcon.ThemeType != IconThemeType.TwoTone) {
               if (_hasColorSet && !_isPresetColorTag) {
                  _controlTokenBinder.AddControlBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorTextLightSolid);
               } else {
                  _controlTokenBinder.AddControlBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorIcon);
                  _controlTokenBinder.AddControlBinding(CloseIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
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
            _controlTokenBinder.AddControlBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorTextLightSolid);
         } else if (_isPresetColorTag) {
            Icon.NormalFilledBrush = Foreground;
         }
      }
   }
}