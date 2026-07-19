namespace AtomUI.Theme.Algorithms;

public interface IThemeVariantCalculatorFactory
{
    IThemeVariantCalculator Create(ThemeAlgorithm algorithm);
}
