using System.Reactive.Linq;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;

namespace AtomUI.Input;

/// <summary>
/// See <see cref="InputManager"/>.
/// </summary>
internal static class InputManagerEx
{
   private static readonly PropertyInfo RootInfo;
   private static readonly PropertyInfo KeyInfo;
   private static readonly PropertyInfo ModifiersInfo;
   private static readonly PropertyInfo PositionInfo;
   private static readonly PropertyInfo TypeInfo;
   
   static InputManagerEx()
   {
      // Input
      RootInfo = typeof(RawInputEventArgs).GetPropertyInfoOrThrow("Root");
        
      // Key
      KeyInfo = typeof(RawKeyEventArgs).GetPropertyInfoOrThrow("Key");
      ModifiersInfo = typeof(RawKeyEventArgs).GetPropertyInfoOrThrow("Modifiers");
      TypeInfo = typeof(RawKeyEventArgs).GetPropertyInfoOrThrow("Type");

      // Pointer
      PositionInfo = typeof(RawPointerEventArgs).GetPropertyInfoOrThrow("Position");
   }
   
   public static IInputRoot? Root(this RawInputEventArgs e) => RootInfo.GetValue(e) as IInputRoot;

   public static Key Key(this RawKeyEventArgs e) => (Key)KeyInfo.GetValue(e)!;

   public static RawInputModifiers Modifiers(this RawKeyEventArgs e) => (RawInputModifiers)ModifiersInfo.GetValue(e)!;

   public static Point Position(this RawPointerEventArgs e) => (Point)PositionInfo.GetValue(e)!;

   public static RawKeyEventType Type(this RawKeyEventArgs e) => (RawKeyEventType)TypeInfo.GetValue(e)!;

   private static IObservable<TInputArgs> GetRawInputEventObservable<TInputArgs>(string processName)
      where TInputArgs : RawInputEventArgs
   {
      // Avalonia.Base
      var assembly = Assembly.GetAssembly(typeof(RawInputEventArgs));

      // InputManager Type
      var type = assembly?.GetType("Avalonia.Input.InputManager")
                 ?? throw new NotSupportedException($"Can not find the type InputManager in assembly {assembly}.");

      // InputManager.Instance
      var manager = type.GetPropertyOrThrow<IInputManager>(type, "Instance", BindingFlags.Public | BindingFlags.Static)
                    ?? throw new NotSupportedException($"Can not find the 'Instance' in type of InputManager.");

      // InputManager.PreProcess | InputManager.Process | InputManager.PostProcess
      var process = manager.GetPropertyOrThrow<IObservable<RawInputEventArgs>>(type, processName, BindingFlags.Public | BindingFlags.Instance)
                    ?? throw new NotSupportedException($"Can not find the '{processName}' in InputManager.");

      return process.OfType<TInputArgs>();
   }

   private static IDisposable SubscribeRawInputEventCore<TInputArgs, TEnumType>(string processName, Func<TEnumType, bool>? filter, 
      Action<TInputArgs> next)
      where TInputArgs : RawInputEventArgs
      where TEnumType : Enum
   {
      var observable = GetRawInputEventObservable<TInputArgs>(processName);

      return observable
             .Where(x => filter == null || filter(x.GetPropertyOrThrow<TEnumType>(typeof(TInputArgs), "Type", BindingFlags.Public | BindingFlags.Instance)!))
             .Subscribe(next);
   }
    
   public static IDisposable SubscribeRawKeyEvent(Func<RawKeyEventType, bool>? filter, 
                                                  Action<RawKeyEventArgs> next)
   {
      return SubscribeRawInputEventCore("Process", filter, next);
   }

   public static IDisposable SubscribePreRawKeyEvent(Func<RawKeyEventType, bool>? filter,
                                                     Action<RawKeyEventArgs> next)
   {
      return SubscribeRawInputEventCore("PreProcess", filter, next);
   }

   public static IDisposable SubscribePostRawKeyEvent(Func<RawKeyEventType, bool>? filter, 
                                                      Action<RawKeyEventArgs> next)
   {
      return SubscribeRawInputEventCore("PostProcess", filter, next);
   }
    
   public static IDisposable SubscribeRawPointerEvent(Func<RawPointerEventType, bool>? filter, 
                                                      Action<RawPointerEventArgs> next)
   {
      return SubscribeRawInputEventCore("Process", filter, next);
   }

   public static IDisposable SubscribePreRawPointerEvent(Func<RawPointerEventType, bool>? filter, 
                                                         Action<RawPointerEventArgs> next)
   {
      return SubscribeRawInputEventCore("PreProcess", filter, next);
   }

   public static IDisposable SubscribePostRawPointerEvent(Func<RawPointerEventType, bool>? filter,
                                                          Action<RawPointerEventArgs> next)
   {
      return SubscribeRawInputEventCore("PostProcess", filter, next);
   }
    
}