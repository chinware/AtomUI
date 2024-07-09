using AtomUI.ColorSystem;
using AtomUI.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Styling;

public class DefaultThemeVariantCalculator : AbstractThemeVariantCalculator
{
   public const string ID = "DefaultAlgorithm";

   public DefaultThemeVariantCalculator()
      : base(null)
   {
      _colorBgBase = Color.FromRgb(255, 255, 255);
      _colorTextBase = Color.FromRgb(0, 0, 0);
   }

   public override MapDesignToken Calculate(SeedDesignToken seedToken, MapDesignToken sourceToken)
   {
      sourceToken.SeedToken = seedToken;

      if (seedToken.ColorBgBase.HasValue) {
         _colorBgBase = seedToken.ColorBgBase.Value;
      }

      if (seedToken.ColorTextBase.HasValue) {
         _colorTextBase = seedToken.ColorTextBase.Value;
      }
      
      SetupColorPalettes(seedToken, sourceToken);
      SetupColorToken(seedToken, sourceToken);

      sourceToken.FontToken = CalculatorUtils.GenerateFontMapToken(seedToken.FontSize);
      sourceToken.SizeToken = CalculatorUtils.GenerateSizeMapToken(seedToken);
      sourceToken.HeightToken = CalculatorUtils.GenerateControlHeightMapToken(seedToken);
      sourceToken.StyleToken = CalculatorUtils.GenerateStyleMapToken(seedToken);

      return sourceToken;
   }

   protected override ColorMap GenerateColorPalettes(Color baseColor)
   {
      var colors = PaletteGenerator.GeneratePalette(baseColor);
      ColorMap colorMap = new ColorMap
      {
         Color1 = colors[0],
         Color2 = colors[1],
         Color3 = colors[2],
         Color4 = colors[3],
         Color5 = colors[4],
         Color6 = colors[5],
         Color7 = colors[6],
         Color8 = colors[4],
         Color9 = colors[5],
         Color10 = colors[6],
         // Color8 = colors[7],
         // Color9 = colors[8],
         // Color10 = colors[9],
      };
      return colorMap;
   }

   protected override ColorNeutralMapDesignToken GenerateNeutralColorPalettes(Color? bgBaseColor, Color? textBaseColor)
   {
      Color colorBgBase = bgBaseColor ?? _colorBgBase;
      Color colorTextBase = textBaseColor ?? _colorTextBase;
      var colorNeutralToken = new ColorNeutralMapDesignToken
      {
         ColorText = AlphaColor(colorTextBase, 0.88),
         ColorTextSecondary = AlphaColor(colorTextBase, 0.65),
         ColorTextTertiary = AlphaColor(colorTextBase, 0.45),
         ColorTextQuaternary = AlphaColor(colorTextBase, 0.25),

         ColorFill = AlphaColor(colorTextBase, 0.15),
         ColorFillSecondary = AlphaColor(colorTextBase, 0.06),
         ColorFillTertiary = AlphaColor(colorTextBase, 0.04),
         ColorFillQuaternary = AlphaColor(colorTextBase, 0.02),

         ColorBgLayout = SolidColor(colorBgBase, 4),
         ColorBgContainer = SolidColor(colorBgBase, 0),
         ColorBgElevated = SolidColor(colorBgBase, 0),
         ColorBgSpotlight = AlphaColor(colorTextBase, 0.85),
         ColorBgBlur = ColorUtils.TransparentColor(),

         ColorBorder = SolidColor(colorBgBase, 15),
         ColorBorderSecondary = SolidColor(colorBgBase, 10),
      };
      return colorNeutralToken;
   }

   private Color AlphaColor(in Color baseColor, double alpha)
   {
      return Color.FromArgb((byte)(alpha * 255), baseColor.R, baseColor.G, baseColor.B);
   }

   private Color SolidColor(Color baseColor, int brightness)
   {
      return baseColor.Darken(brightness);
   }

   private void SetupColorPalettes(SeedDesignToken seedToken, MapDesignToken sourceToken)
   {
      // 生成所有预置颜色的色系
      foreach (var presetColor in PresetPrimaryColor.AllColorTypes()) {
         var colors = PaletteGenerator.GeneratePalette(seedToken.GetPresetPrimaryColor(presetColor.Type).Color());
         ColorMap colorMap = ColorMap.FromColors(colors);
         sourceToken.SetColorPalette(presetColor, colorMap);
      }
   }

   private void SetupColorToken(SeedDesignToken seedToken, MapDesignToken sourceToken)
   {
      ColorMapDesignToken colorMapToken = GenerateColorMapToken(seedToken);
      sourceToken.ColorToken = colorMapToken;
   }
}