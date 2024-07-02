using System.Reflection;
using AtomUI.Media;
using Avalonia.Animation;
using Avalonia.Media;
using HarmonyLib;

namespace AtomUI.Interceptors;

internal static class TransitionInterceptor<TTransition, TValue>
   where TTransition : TransitionBase
{
   private static Dictionary<object, IDisposable> _disposables;

   static TransitionInterceptor()
   {
      _disposables = new Dictionary<object, IDisposable>();
   }

   public static bool DoTransitionPrefix(TTransition __instance, IObservable<TValue> progress)
   {
      if (!_disposables.ContainsKey(__instance) && __instance is INotifyTransitionCompleted notifier) {
         var disposable = progress.Subscribe(onNext: d => { }, onCompleted: () => { HandleCompleted(notifier, true); },
                                             onError: exception => { HandleCompleted(notifier, false); });
         _disposables.Add(notifier, disposable);
      }

      return true;
   }

   private static void HandleCompleted(INotifyTransitionCompleted notifier, bool succeed)
   {
      if (succeed) {
         notifier.NotifyTransitionCompleted();
      }

      _disposables[notifier].Dispose();
      _disposables.Remove(notifier);
   }
}

internal static class TransitionInterceptorsRegister
{
   public static void Register(Harmony harmony)
   {
      RegisterTransformOperationsTransition(harmony);
      RegisterDoubleTransition(harmony);
   }

   private static void RegisterTransformOperationsTransition(Harmony harmony)
   {
      var origin =
         typeof(TransformOperationsTransition).GetMethod("DoTransition",
                                                         BindingFlags.Instance | BindingFlags.NonPublic);
      var prefixInterceptor = typeof(TransitionInterceptor<,>)
                              .MakeGenericType(typeof(TransformOperationsTransition), typeof(ITransform))
                              .GetMethod("DoTransitionPrefix", BindingFlags.Static | BindingFlags.Public);
      harmony.Patch(origin, prefix: new HarmonyMethod(prefixInterceptor));
   }

   private static void RegisterDoubleTransition(Harmony harmony)
   {
      var origin = typeof(DoubleTransition).GetMethod("DoTransition", BindingFlags.Instance | BindingFlags.NonPublic);
      var prefixInterceptor = typeof(TransitionInterceptor<,>)
                              .MakeGenericType(typeof(DoubleTransition), typeof(double))
                              .GetMethod("DoTransitionPrefix", BindingFlags.Static | BindingFlags.Public);
      harmony.Patch(origin, prefix: new HarmonyMethod(prefixInterceptor));
   }
}