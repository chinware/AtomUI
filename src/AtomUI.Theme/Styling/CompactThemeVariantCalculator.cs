using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Styling;

public class CompactThemeVariantCalculator : AbstractThemeVariantCalculator
{
    public const string ID = "CompactAlgorithm";

    public CompactThemeVariantCalculator(IThemeVariantCalculator calculator)
        : base(calculator)
    {
    }

    public override void Calculate(GlobalToken globalToken)
    {
        _compositeGenerator!.Calculate(globalToken);

        _colorBgBase   = _compositeGenerator.ColorBgBase;
        _colorTextBase = _compositeGenerator.ColorTextBase;
        
        var controlHeight = globalToken.ControlHeight - 4;

        CalculateCompactSizeMapTokenValues(globalToken);
        CalculatorUtils.CalculateFontMapTokenValues(globalToken);

        globalToken.ControlHeight = controlHeight;
        CalculatorUtils.CalculateControlHeightMapTokenValues(globalToken);
    }

    private void CalculateCompactSizeMapTokenValues(GlobalToken globalToken)
    {
        var sizeUnit        = globalToken.SizeUnit;
        var sizeStep        = globalToken.SizeStep;
        var compactSizeStep = sizeStep - 2;

        globalToken.SizeXXL = sizeUnit * (compactSizeStep + 10);
        globalToken.SizeXL  = sizeUnit * (compactSizeStep + 6);
        globalToken.SizeLG  = sizeUnit * (compactSizeStep + 2);
        globalToken.SizeMD  = sizeUnit * (compactSizeStep + 2);
        globalToken.SizeMS  = sizeUnit * (compactSizeStep + 1);
        globalToken.Size    = sizeUnit * compactSizeStep;
        globalToken.SizeSM  = sizeUnit * compactSizeStep;
        globalToken.SizeXS  = sizeUnit * (compactSizeStep - 1);
        globalToken.SizeXXS = sizeUnit * (compactSizeStep - 1);
    }
}