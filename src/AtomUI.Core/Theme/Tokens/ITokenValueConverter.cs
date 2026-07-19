namespace AtomUI.Theme.Tokens;

public interface ITokenValueConverter
{
    public Type TargetType();
    public object Convert(string value);
}