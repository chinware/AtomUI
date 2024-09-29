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

    protected virtual void CalculateNeutralColorPalettes(Color? bgBaseColor, Color? textBaseColor, GlobalToken globalToken)
    {
    }

    public abstract void Calculate(GlobalToken globalToken);

    protected void CalculateColorMapTokenValues(GlobalToken globalToken)
    {
        var colorSuccessBase = globalToken.ColorSuccess;
        var colorWarningBase = globalToken.ColorWarning;
        var colorErrorBase   = globalToken.ColorError;
        var colorInfoBase    = globalToken.ColorInfo;
        var colorPrimaryBase = globalToken.ColorPrimary;
        var colorBgBase      = globalToken.ColorBgBase;
        var colorTextBase    = globalToken.ColorTextBase;

        var primaryColors = GenerateColorPalettes(colorPrimaryBase);
        var successColors = GenerateColorPalettes(colorSuccessBase);
        var warningColors = GenerateColorPalettes(colorWarningBase);
        var errorColors   = GenerateColorPalettes(colorErrorBase);
        var infoColors    = GenerateColorPalettes(colorInfoBase);

        CalculateNeutralColorPalettes(colorBgBase, colorTextBase, globalToken);
        var colorLink            = globalToken.ColorLink ?? globalToken.ColorInfo;
        var linkColors           = GenerateColorPalettes(colorLink);

        globalToken.ColorPrimaryBg          = primaryColors.Color1;
        globalToken.ColorPrimaryBgHover     = primaryColors.Color2;
        globalToken.ColorPrimaryBorder      = primaryColors.Color3;
        globalToken.ColorPrimaryBorderHover = primaryColors.Color4;
        globalToken.ColorPrimaryHover       = primaryColors.Color5;
        globalToken.ColorPrimary            = primaryColors.Color6;
        globalToken.ColorPrimaryActive      = primaryColors.Color7;
        globalToken.ColorPrimaryTextHover   = primaryColors.Color8;
        globalToken.ColorPrimaryText        = primaryColors.Color9;
        globalToken.ColorPrimaryTextActive  = primaryColors.Color10;

        globalToken.ColorSuccessBg          = successColors.Color1;
        globalToken.ColorSuccessBgHover     = successColors.Color2;
        globalToken.ColorSuccessBorder      = successColors.Color3;
        globalToken.ColorSuccessBorderHover = successColors.Color4;
        globalToken.ColorSuccessHover       = successColors.Color4;
        globalToken.ColorSuccess            = successColors.Color6;
        globalToken.ColorSuccessActive      = successColors.Color7;
        globalToken.ColorSuccessTextHover   = successColors.Color8;
        globalToken.ColorSuccessText        = successColors.Color9;
        globalToken.ColorSuccessTextActive  = successColors.Color10;

        globalToken.ColorErrorBg          = errorColors.Color1;
        globalToken.ColorErrorBgHover     = errorColors.Color2;
        globalToken.ColorErrorBgActive    = errorColors.Color3;
        globalToken.ColorErrorBorder      = errorColors.Color3;
        globalToken.ColorErrorBorderHover = errorColors.Color4;
        globalToken.ColorErrorHover       = errorColors.Color5;
        globalToken.ColorError            = errorColors.Color6;
        globalToken.ColorErrorActive      = errorColors.Color7;
        globalToken.ColorErrorTextHover   = errorColors.Color8;
        globalToken.ColorErrorText        = errorColors.Color9;
        globalToken.ColorErrorTextActive  = errorColors.Color10;

        globalToken.ColorWarningBg          = warningColors.Color1;
        globalToken.ColorWarningBgHover     = warningColors.Color2;
        globalToken.ColorWarningBorder      = warningColors.Color3;
        globalToken.ColorWarningBorderHover = warningColors.Color4;
        globalToken.ColorWarningHover       = warningColors.Color4;
        globalToken.ColorWarning            = warningColors.Color6;
        globalToken.ColorWarningActive      = warningColors.Color7;
        globalToken.ColorWarningTextHover   = warningColors.Color8;
        globalToken.ColorWarningText        = warningColors.Color9;
        globalToken.ColorWarningTextActive  = warningColors.Color10;

        globalToken.ColorInfoBg          = infoColors.Color1;
        globalToken.ColorInfoBgHover     = infoColors.Color2;
        globalToken.ColorInfoBorder      = infoColors.Color3;
        globalToken.ColorInfoBorderHover = infoColors.Color4;
        globalToken.ColorInfoHover       = infoColors.Color4;
        globalToken.ColorInfo            = infoColors.Color6;
        globalToken.ColorInfoActive      = infoColors.Color7;
        globalToken.ColorInfoTextHover   = infoColors.Color8;
        globalToken.ColorInfoText        = infoColors.Color9;
        globalToken.ColorInfoTextActive  = infoColors.Color10;

        globalToken.ColorLinkHover  = linkColors.Color4;
        globalToken.ColorLink       = linkColors.Color6;
        globalToken.ColorLinkActive = linkColors.Color7;

        globalToken.ColorBgMask         = ColorUtils.FromRgbF(0.45, 0, 0, 0);
        globalToken.ColorWhite         = Color.FromRgb(255, 255, 255);
        globalToken.ColorBlack         = Color.FromRgb(0, 0, 0);
        globalToken.SelectionBackground = primaryColors.Color6;
        globalToken.SelectionForeground = Color.FromRgb(255, 255, 255);
    }
}