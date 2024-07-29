using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme.Styling;

public class CompactThemeVariantCalculator : AbstractThemeVariantCalculator
{
   public const string ID = "CompactAlgorithm";

   public CompactThemeVariantCalculator(IThemeVariantCalculator calculator)
      : base(calculator)
   {
   }

   public override MapDesignToken Calculate(SeedDesignToken seedToken, MapDesignToken sourceToken)
   {
      MapDesignToken mergedMapToken = _compositeGenerator!.Calculate(seedToken, sourceToken);

      _colorBgBase = _compositeGenerator.ColorBgBase;
      _colorTextBase = _compositeGenerator.ColorTextBase;
      
      double fontSize = mergedMapToken.FontToken.FontSize; // Smaller size font-size as base
      double controlHeight = mergedMapToken.SeedToken.ControlHeight - 4;

      mergedMapToken.SizeToken = GenerateCompactSizeMapToken(sourceToken.SeedToken);
      mergedMapToken.FontToken = CalculatorUtils.GenerateFontMapToken(fontSize);

      mergedMapToken.SeedToken.ControlHeight = controlHeight;
      mergedMapToken.HeightToken = CalculatorUtils.GenerateControlHeightMapToken(mergedMapToken.SeedToken);
      return mergedMapToken;
   }

   private SizeMapDesignToken GenerateCompactSizeMapToken(SeedDesignToken seedToken)
   {
      double sizeUnit = seedToken.SizeUnit;
      double sizeStep = seedToken.SizeStep;
      double compactSizeStep = sizeStep - 2;

      var token = new SizeMapDesignToken
      {
         SizeXXL = sizeUnit * (compactSizeStep + 10),
         SizeXL = sizeUnit * (compactSizeStep + 6),
         SizeLG = sizeUnit * (compactSizeStep + 2),
         SizeMD = sizeUnit * (compactSizeStep + 2),
         SizeMS = sizeUnit * (compactSizeStep + 1),
         Size = sizeUnit * compactSizeStep,
         SizeSM = sizeUnit * compactSizeStep,
         SizeXS = sizeUnit * (compactSizeStep - 1),
         SizeXXS = sizeUnit * (compactSizeStep - 1),
      };
      return token;
   }
}