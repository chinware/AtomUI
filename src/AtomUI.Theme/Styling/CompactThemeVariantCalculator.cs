using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Styling;

public class CompactThemeVariantCalculator : AbstractThemeVariantCalculator
{
    public const string ID = "CompactAlgorithm";

    public CompactThemeVariantCalculator(IThemeVariantCalculator calculator)
        : base(calculator)
    {
    }

    public override void Calculate(DesignToken designToken)
    {
        _compositeGenerator!.Calculate(designToken);

        _colorBgBase   = _compositeGenerator.ColorBgBase;
        _colorTextBase = _compositeGenerator.ColorTextBase;
        
        var controlHeight = designToken.ControlHeight - 4;

        CalculateCompactSizeMapTokenValues(designToken);
        CalculatorUtils.CalculateFontMapTokenValues(designToken);

        designToken.ControlHeight = controlHeight;
        CalculatorUtils.CalculateControlHeightMapTokenValues(designToken);
    }

    private void CalculateCompactSizeMapTokenValues(DesignToken designToken)
    {
        var sizeUnit        = designToken.SizeUnit;
        var sizeStep        = designToken.SizeStep;
        var compactSizeStep = sizeStep - 2;

        designToken.SizeXXL = sizeUnit * (compactSizeStep + 10);
        designToken.SizeXL  = sizeUnit * (compactSizeStep + 6);
        designToken.SizeLG  = sizeUnit * (compactSizeStep + 2);
        designToken.SizeMD  = sizeUnit * (compactSizeStep + 2);
        designToken.SizeMS  = sizeUnit * (compactSizeStep + 1);
        designToken.Size    = sizeUnit * compactSizeStep;
        designToken.SizeSM  = sizeUnit * compactSizeStep;
        designToken.SizeXS  = sizeUnit * (compactSizeStep - 1);
        designToken.SizeXXS = sizeUnit * (compactSizeStep - 1);
    }
}