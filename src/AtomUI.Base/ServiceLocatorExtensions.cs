namespace AtomUI;

internal static class ServiceLocatorExtensions
{
   public static T? GetService<T>(this IDependencyResolver resolver)
   {
      return (T?) resolver.GetService(typeof (T));
   }

   public static object GetRequiredService(this IDependencyResolver resolver, Type t)
   {
      return resolver.GetService(t) ?? throw new InvalidOperationException($"Unable to locate '{t}'.");
   }

   public static T GetRequiredService<T>(this IDependencyResolver resolver)
   {
      return (T?)resolver.GetService(typeof(T)) ?? throw new InvalidOperationException($"Unable to locate '{typeof(T)}'.");
   }
}