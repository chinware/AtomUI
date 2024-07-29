namespace AtomUI.TokenSystem;

public interface ITokenValueConverter
{
   public Type TargetType();
   public object Convert(string value);
}