namespace AtomUI;

public interface IThemeManager
{
   public IReadOnlyCollection<ITheme> AvailableThemes { get; }
   public ITheme? ActivatedTheme { get; }
   
   public ITheme LoadTheme(string id);
   public void UnLoadTheme(string id);
   
   public void SetActiveTheme(string id);
   public void RegisterDynamicTheme(DynamicTheme dynamicTheme);

   public void ScanThemes();
}