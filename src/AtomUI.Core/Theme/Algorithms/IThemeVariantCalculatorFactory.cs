namespace AtomUI.Theme.Styling;

public interface IThemeVariantCalculatorFactory
{
    IThemeVariantCalculator Create(ThemeAlgorithm algorithm, IThemeVariantCalculator? baseAlgorithm);
}