namespace AtomUI.Theme.TokenSystem;

public interface ITokenValueConverter
{
   public Type TargetType();
   public object Convert(string value);
}