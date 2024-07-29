namespace AtomUI;

public class ThemeDefinitionParserException : SystemException
{
   public ThemeDefinitionParserException(string? message)
      : base(message)
   {
   }

   public ThemeDefinitionParserException(string? message, Exception? innerException)
      : base(message, innerException)
   {
   }
}

public class ThemeLoadException : SystemException
{
   public ThemeLoadException(string? message)
      : base(message)
   {
   }

   public ThemeLoadException(string? message, Exception? innerException)
      : base(message, innerException)
   {
   }
}