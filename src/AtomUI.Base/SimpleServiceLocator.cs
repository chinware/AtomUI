namespace AtomUI;

internal interface IDependencyResolver
{
   object? GetService(Type t);
}

internal delegate void ServiceRegisterAction(SimpleServiceLocator locator);

internal class SimpleServiceLocator : IDependencyResolver
{
   private readonly IDependencyResolver? _parentScope;
   public static IDependencyResolver Current { get; set; }
   public static SimpleServiceLocator CurrentMutable { get; set; }
   private readonly Dictionary<Type, Func<object?>> _registry = new Dictionary<Type, Func<object?>>();

   static SimpleServiceLocator()
   {
      Current = CurrentMutable = new SimpleServiceLocator();
   }

   public SimpleServiceLocator() { }

   public SimpleServiceLocator(IDependencyResolver parentScope)
   {
      _parentScope = parentScope;
   }

   public object? GetService(Type t)
   {
      return _registry.TryGetValue(t, out var rv) ? rv() : _parentScope?.GetService(t);
   }

   public class RegistrationHelper<TService>
   {
      private readonly SimpleServiceLocator _locator;

      public RegistrationHelper(SimpleServiceLocator locator)
      {
         _locator = locator;
      }

      public SimpleServiceLocator ToConstant<TImpl>(TImpl constant) where TImpl : TService
      {
         _locator._registry[typeof(TService)] = () => constant;
         return _locator;
      }

      public SimpleServiceLocator ToFunc<TImlp>(Func<TImlp> func) where TImlp : TService
      {
         _locator._registry[typeof(TService)] = () => func();
         return _locator;
      }

      public SimpleServiceLocator ToLazy<TImlp>(Func<TImlp> func) where TImlp : TService
      {
         var constructed = false;
         TImlp? instance = default;
         _locator._registry[typeof(TService)] = () =>
         {
            if (!constructed) {
               instance = func();
               constructed = true;
            }

            return instance;
         };
         return _locator;
      }

      public SimpleServiceLocator ToSingleton<TImpl>() where TImpl : class, TService, new()
      {
         TImpl? instance = null;
         return ToFunc(() => instance ?? (instance = new TImpl()));
      }

      public SimpleServiceLocator ToTransient<TImpl>() where TImpl : class, TService, new() =>
         ToFunc(() => new TImpl());
   }

   public RegistrationHelper<T> Bind<T>() => new RegistrationHelper<T>(this);


   public SimpleServiceLocator BindToSelf<T>(T constant)
      => Bind<T>().ToConstant(constant);

   public SimpleServiceLocator BindToSelfSingleton<T>() where T : class, new() => Bind<T>().ToSingleton<T>();

   class ResolverDisposable : IDisposable
   {
      private readonly IDependencyResolver _resolver;
      private readonly SimpleServiceLocator _mutable;

      public ResolverDisposable(IDependencyResolver resolver, SimpleServiceLocator mutable)
      {
         _resolver = resolver;
         _mutable = mutable;
      }

      public void Dispose()
      {
         Current = _resolver;
         CurrentMutable = _mutable;
      }
   }

   public static IDisposable EnterScope()
   {
      var d = new ResolverDisposable(Current, CurrentMutable);
      Current = CurrentMutable = new SimpleServiceLocator(Current);
      return d;
   }
}