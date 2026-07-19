namespace AtomUI.Theme.DesignTokens;

public interface ITokenValueConverter
{
    public Type TargetType();
    public object Convert(string value);
}