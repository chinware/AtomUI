using AtomUI.Media;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Theme.Styling;

public abstract class AbstractThemeVariantCalculator : IThemeVariantCalculator
{
   protected IThemeVariantCalculator? _compositeGenerator;
   protected Color _colorBgBase;
   protected Color _colorTextBase;

   public Color ColorBgBase => _colorBgBase;
   public Color ColorTextBase => _colorTextBase;

   protected AbstractThemeVariantCalculator(IThemeVariantCalculator? calculator)
   {
      _compositeGenerator = calculator;
   }

   protected virtual ColorMap GenerateColorPalettes(Color baseColor)
   {
      return default!;
   }

   protected virtual ColorNeutralMapDesignToken GenerateNeutralColorPalettes(Color? bgBaseColor, Color? textBaseColor)
   {
      return default!;
   }

   public abstract MapDesignToken Calculate(SeedDesignToken seedToken, MapDesignToken sourceToken);

   protected ColorMapDesignToken GenerateColorMapToken(SeedDesignToken seedToken)
   {
      Color colorSuccessBase = seedToken.ColorSuccess;
      Color colorWarningBase = seedToken.ColorWarning;
      Color colorErrorBase = seedToken.ColorError;
      Color colorInfoBase = seedToken.ColorInfo;
      Color colorPrimaryBase = seedToken.ColorPrimary;
      Color? colorBgBase = seedToken.ColorBgBase;
      Color? colorTextBase = seedToken.ColorTextBase;

      ColorMap primaryColors = GenerateColorPalettes(colorPrimaryBase);
      ColorMap successColors = GenerateColorPalettes(colorSuccessBase);
      ColorMap warningColors = GenerateColorPalettes(colorWarningBase);
      ColorMap errorColors = GenerateColorPalettes(colorErrorBase);
      ColorMap infoColors = GenerateColorPalettes(colorInfoBase);

      ColorNeutralMapDesignToken colorNeutralMapToken = GenerateNeutralColorPalettes(colorBgBase, colorTextBase);
      Color colorLink = seedToken.ColorLink ?? seedToken.ColorInfo;
      ColorMap linkColors = GenerateColorPalettes(colorLink);
      var colorMapDesignToken = new ColorMapDesignToken
      {
         ColorNeutralToken = colorNeutralMapToken,
         ColorPrimaryToken = new ColorPrimaryMapDesignToken
         {
            ColorPrimaryBg = primaryColors.Color1,
            ColorPrimaryBgHover = primaryColors.Color2,
            ColorPrimaryBorder = primaryColors.Color3,
            ColorPrimaryBorderHover = primaryColors.Color4,
            ColorPrimaryHover = primaryColors.Color5,
            ColorPrimary = primaryColors.Color6,
            ColorPrimaryActive = primaryColors.Color7,
            ColorPrimaryTextHover = primaryColors.Color8,
            ColorPrimaryText = primaryColors.Color9,
            ColorPrimaryTextActive = primaryColors.Color10
         },

         ColorSuccessToken = new ColorSuccessMapDesignToken
         {
            ColorSuccessBg = successColors.Color1,
            ColorSuccessBgHover = successColors.Color2,
            ColorSuccessBorder = successColors.Color3,
            ColorSuccessBorderHover = successColors.Color4,
            ColorSuccessHover = successColors.Color4,
            ColorSuccess = successColors.Color6,
            ColorSuccessActive = successColors.Color7,
            ColorSuccessTextHover = successColors.Color8,
            ColorSuccessText = successColors.Color9,
            ColorSuccessTextActive = successColors.Color10
         },

         ColorErrorToken = new ColorErrorMapDesignToken
         {
            ColorErrorBg = errorColors.Color1,
            ColorErrorBgHover = errorColors.Color2,
            ColorErrorBgActive = errorColors.Color3,
            ColorErrorBorder = errorColors.Color3,
            ColorErrorBorderHover = errorColors.Color4,
            ColorErrorHover = errorColors.Color5,
            ColorError = errorColors.Color6,
            ColorErrorActive = errorColors.Color7,
            ColorErrorTextHover = errorColors.Color8,
            ColorErrorText = errorColors.Color9,
            ColorErrorTextActive = errorColors.Color10
         },

         ColorWarningToken = new ColorWarningMapDesignToken
         {
            ColorWarningBg = warningColors.Color1,
            ColorWarningBgHover = warningColors.Color2,
            ColorWarningBorder = warningColors.Color3,
            ColorWarningBorderHover = warningColors.Color4,
            ColorWarningHover = warningColors.Color4,
            ColorWarning = warningColors.Color6,
            ColorWarningActive = warningColors.Color7,
            ColorWarningTextHover = warningColors.Color8,
            ColorWarningText = warningColors.Color9,
            ColorWarningTextActive = warningColors.Color10
         },

         ColorInfoToken = new ColorInfoMapDesignToken
         {
            ColorInfoBg = infoColors.Color1,
            ColorInfoBgHover = infoColors.Color2,
            ColorInfoBorder = infoColors.Color3,
            ColorInfoBorderHover = infoColors.Color4,
            ColorInfoHover = infoColors.Color4,
            ColorInfo = infoColors.Color6,
            ColorInfoActive = infoColors.Color7,
            ColorInfoTextHover = infoColors.Color8,
            ColorInfoText = infoColors.Color9,
            ColorInfoTextActive = infoColors.Color10
         },

         ColorLinkToken = new ColorLinkMapDesignToken
         {
            ColorLinkHover = linkColors.Color4,
            ColorLink = linkColors.Color6,
            ColorLinkActive = linkColors.Color7
         },

         ColorBgMask = ColorUtils.FromRgbF(0.45, 0, 0, 0),
         ColorWhite = Color.FromRgb(255, 255, 255),
         ColorBlack = Color.FromRgb(0, 0, 0),
         SelectionBackground = primaryColors.Color6,
         SelectionForeground = Color.FromRgb(255, 255, 255)
      };

      return colorMapDesignToken;
   }
}