using System.Reactive.Subjects;
using System.Reflection;
using AtomUI.Media;
using Avalonia.Animation;
using Avalonia.Media;
using HarmonyLib;

namespace AtomUI.Interceptors;

internal static class TransitionInterceptor<TTransition>
   where TTransition : TransitionBase
{
   private static Dictionary<object, IDisposable> _disposables;

   static TransitionInterceptor()
   {
      _disposables = new Dictionary<object, IDisposable>();
   }

   public static bool DoTransitionPrefix(TTransition __instance, ref IObservable<double> progress)
   {
      if (!_disposables.ContainsKey(__instance) && __instance is INotifyTransitionCompleted notifier) {
         progress = CreateRelayObservable(progress);
         var disposable = progress.Subscribe(onNext: d => { }, onCompleted: () => { HandleCompleted(notifier, true); },
                                             onError: exception => { HandleCompleted(notifier, false); });
         _disposables.Add(notifier, disposable);
      }

      return true;
   }

   // TODO review 不知道是否有内存泄露
   private static IObservable<double> CreateRelayObservable(IObservable<double> progress)
   {
      var subject = new Subject<double>();
      progress.Subscribe(onNext: value => subject.OnNext(value),
                         onError: exception => subject.OnError(exception),
                         onCompleted: () => subject.OnCompleted());
      return subject;
   }

   private static void HandleCompleted(INotifyTransitionCompleted notifier, bool succeed)
   {
      notifier.NotifyTransitionCompleted(succeed);
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
      var prefixInterceptor = typeof(TransitionInterceptor<>)
                              .MakeGenericType(typeof(TransformOperationsTransition))
                              .GetMethod("DoTransitionPrefix", BindingFlags.Static | BindingFlags.Public);
      harmony.Patch(origin, prefix: new HarmonyMethod(prefixInterceptor));
   }

   private static void RegisterDoubleTransition(Harmony harmony)
   {
      var origin = typeof(DoubleTransition).GetMethod("DoTransition", BindingFlags.Instance | BindingFlags.NonPublic);
      var prefixInterceptor = typeof(TransitionInterceptor<>)
                              .MakeGenericType(typeof(DoubleTransition))
                              .GetMethod("DoTransitionPrefix", BindingFlags.Static | BindingFlags.Public);
      harmony.Patch(origin, prefix: new HarmonyMethod(prefixInterceptor));
   }
}