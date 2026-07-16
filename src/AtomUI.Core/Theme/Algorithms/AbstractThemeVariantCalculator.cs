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

    protected virtual void CalculateNeutralColorPalettes(Color? bgBaseColor, Color? textBaseColor, DesignToken designToken)
    {
    }

    public abstract void Calculate(DesignToken designToken);

    protected void CalculateColorMapTokenValues(DesignToken designToken)
    {
        var colorSuccessBase = designToken.ColorSuccess;
        var colorWarningBase = designToken.ColorWarning;
        var colorErrorBase   = designToken.ColorError;
        var colorInfoBase    = designToken.ColorInfo;
        var colorPrimaryBase = designToken.ColorPrimary;
        var colorBgBase      = designToken.ColorBgBase;
        var colorTextBase    = designToken.ColorTextBase;

        var primaryColors = GenerateColorPalettes(colorPrimaryBase);
        var successColors = GenerateColorPalettes(colorSuccessBase);
        var warningColors = GenerateColorPalettes(colorWarningBase);
        var errorColors   = GenerateColorPalettes(colorErrorBase);
        var infoColors    = GenerateColorPalettes(colorInfoBase);

        CalculateNeutralColorPalettes(colorBgBase, colorTextBase, designToken);
        var colorLink  = designToken.ColorLink ?? designToken.ColorInfo;
        var linkColors = GenerateColorPalettes(colorLink);

        designToken.ColorPrimaryBg          = primaryColors.Color1;
        designToken.ColorPrimaryBgHover     = primaryColors.Color2;
        designToken.ColorPrimaryBorder      = primaryColors.Color3;
        designToken.ColorPrimaryBorderHover = primaryColors.Color4;
        designToken.ColorPrimaryHover       = primaryColors.Color5;
        designToken.ColorPrimary            = primaryColors.Color6;
        designToken.ColorPrimaryActive      = primaryColors.Color7;
        designToken.ColorPrimaryTextHover   = primaryColors.Color8;
        designToken.ColorPrimaryText        = primaryColors.Color9;
        designToken.ColorPrimaryTextActive  = primaryColors.Color10;

        designToken.ColorSuccessBg          = successColors.Color1;
        designToken.ColorSuccessBgHover     = successColors.Color2;
        designToken.ColorSuccessBorder      = successColors.Color3;
        designToken.ColorSuccessBorderHover = successColors.Color4;
        designToken.ColorSuccessHover       = successColors.Color4;
        designToken.ColorSuccess            = successColors.Color6;
        designToken.ColorSuccessActive      = successColors.Color7;
        designToken.ColorSuccessTextHover   = successColors.Color8;
        designToken.ColorSuccessText        = successColors.Color9;
        designToken.ColorSuccessTextActive  = successColors.Color10;

        designToken.ColorErrorBg          = errorColors.Color1;
        designToken.ColorErrorBgHover     = errorColors.Color2;
        designToken.ColorErrorBgActive    = errorColors.Color3;
        designToken.ColorErrorBorder      = errorColors.Color3;
        designToken.ColorErrorBorderHover = errorColors.Color4;
        designToken.ColorErrorHover       = errorColors.Color5;
        designToken.ColorError            = errorColors.Color6;
        designToken.ColorErrorActive      = errorColors.Color7;
        designToken.ColorErrorTextHover   = errorColors.Color8;
        designToken.ColorErrorText        = errorColors.Color9;
        designToken.ColorErrorTextActive  = errorColors.Color10;

        designToken.ColorWarningBg          = warningColors.Color1;
        designToken.ColorWarningBgHover     = warningColors.Color2;
        designToken.ColorWarningBorder      = warningColors.Color3;
        designToken.ColorWarningBorderHover = warningColors.Color4;
        designToken.ColorWarningHover       = warningColors.Color4;
        designToken.ColorWarning            = warningColors.Color6;
        designToken.ColorWarningActive      = warningColors.Color7;
        designToken.ColorWarningTextHover   = warningColors.Color8;
        designToken.ColorWarningText        = warningColors.Color9;
        designToken.ColorWarningTextActive  = warningColors.Color10;

        designToken.ColorInfoBg          = infoColors.Color1;
        designToken.ColorInfoBgHover     = infoColors.Color2;
        designToken.ColorInfoBorder      = infoColors.Color3;
        designToken.ColorInfoBorderHover = infoColors.Color4;
        designToken.ColorInfoHover       = infoColors.Color4;
        designToken.ColorInfo            = infoColors.Color6;
        designToken.ColorInfoActive      = infoColors.Color7;
        designToken.ColorInfoTextHover   = infoColors.Color8;
        designToken.ColorInfoText        = infoColors.Color9;
        designToken.ColorInfoTextActive  = infoColors.Color10;

        designToken.ColorLinkHover  = linkColors.Color4;
        designToken.ColorLink       = linkColors.Color6;
        designToken.ColorLinkActive = linkColors.Color7;

        designToken.ColorBgMask         = ColorUtils.FromRgbF(0.45, 0, 0, 0);
        designToken.ColorWhite          = Color.FromRgb(255, 255, 255);
        designToken.ColorBlack          = Color.FromRgb(0, 0, 0);
        designToken.SelectionBackground = primaryColors.Color6;
        designToken.SelectionForeground = Color.FromRgb(255, 255, 255);
    }
}