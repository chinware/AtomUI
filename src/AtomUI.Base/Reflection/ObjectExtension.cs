using System.Reflection;
using System.Text;

namespace AtomUI.Reflection;

public static class ObjectExtension
{
   public static bool TryGetProperty<T>(
      this object source,
      string name,
      out T? result,
      BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      PropertyInfo? property = source.GetType().GetProperty(name, flags);
      if (property is not null && property.GetValue(source) is T obj) {
         result = obj;
         return true;
      }

      result = default;
      return false;
   }

   public static bool TryGetProperty<T>(
      this object source,
      Type declareType,
      string name,
      out T? result,
      BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      PropertyInfo? property = declareType.GetProperty(name, flags);
      if (property is not null && property.GetValue(source) is T obj) {
         result = obj;
         return true;
      }

      result = default;
      return false;
   }

   public static bool TryGetProperty<T>(this object source, PropertyInfo info, out T? result)
   {
      if (info.GetValue(source) is T obj) {
         result = obj;
         return true;
      }

      result = default;
      return false;
   }

   public static T? GetPropertyOrThrow<T>(this object source, string name,
                                          BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      object? obj = source.GetType().GetPropertyInfoOrThrow(name, flags).GetValue(source);
      if (obj is T propertyOrThrow) {
         return propertyOrThrow;
      }
      if (obj == null) {
         if (typeof(T).IsValueType) {
            throw new Exception(name + " is a value type but the value is null.");
         }

         return default;
      }

      var stringBuilder = new StringBuilder(30, 3);
      stringBuilder.Append(name);
      stringBuilder.Append("'s type is ");
      stringBuilder.Append(source.GetType().Name);
      stringBuilder.Append(" but the value is ");
      stringBuilder.Append(obj.GetType().Name);
      stringBuilder.Append(".");
      throw new Exception(stringBuilder.ToString());
   }

   public static T? GetPropertyOrThrow<T>(
      this object source,
      Type declareType,
      string name,
      BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      object? obj = declareType.GetPropertyInfoOrThrow(name, flags).GetValue(source);
      if (obj is T propertyOrThrow) {
         return propertyOrThrow;
      }

      if (obj == null) {
         if (typeof(T).IsValueType) {
            throw new Exception(name + " is a value type but the value is null.");
         }

         return default;
      }

      var stringHandler = new StringBuilder(30, 3);
      stringHandler.Append(name);
      stringHandler.Append("'s type is ");
      stringHandler.Append(declareType.Name);
      stringHandler.Append(" but the value is ");
      stringHandler.Append(obj.GetType().Name);
      stringHandler.Append(".");
      throw new Exception(stringHandler.ToString());
   }

   public static bool TrySetProperty<T>(
      this object source,
      Type declareType,
      string name,
      T? value,
      BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      PropertyInfo? property = declareType.GetProperty(name, flags);
      if (property is null) {
         return false;
      }

      property.SetValue(source, value);
      return true;
   }

   public static bool TryGetField<T>(
      this object source,
      string name,
      out T? result,
      BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      FieldInfo? field = source.GetType().GetField(name, flags);
      if (field is not null && field.GetValue(source) is T obj) {
         result = obj;
         return true;
      }

      result = default;
      return false;
   }

   public static bool TryGetField<T>(
      this object source,
      Type declareType,
      string name,
      out T? result,
      BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      FieldInfo? field = declareType.GetField(name, flags);
      if (field is not null && field.GetValue(source) is T obj) {
         result = obj;
         return true;
      }

      result = default;
      return false;
   }

   public static bool TryGetField<T>(this object source, FieldInfo info, out T? result)
   {
      if (info.GetValue(source) is T obj) {
         result = obj;
         return true;
      }

      result = default;
      return false;
   }

   public static T? GetFieldOrThrow<T>(this object source, string name,
                                       BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      object? obj = source.GetType().GetFieldInfoOrThrow(name, flags).GetValue(source);
      if (obj is T fieldOrThrow) {
         return fieldOrThrow;
      }

      if (obj == null) {
         if (typeof(T).IsValueType) {
            throw new Exception(name + " is a value type but the value is null.");
         }

         return default;
      }

      var stringBuilder = new StringBuilder(30, 3);
      stringBuilder.Append(name);
      stringBuilder.Append("'s type is ");
      stringBuilder.Append(source.GetType().Name);
      stringBuilder.Append(" but the value is ");
      stringBuilder.Append(obj.GetType().Name);
      stringBuilder.Append(".");
      throw new Exception(stringBuilder.ToString());
   }

   public static T? GetFieldOrThrow<T>(
      this object source,
      Type declareType,
      string name,
      BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      object? obj = declareType.GetFieldInfoOrThrow(name, flags).GetValue(source);
      if (obj is T fieldOrThrow) return fieldOrThrow;
      if (obj == null) {
         if (typeof(T).IsValueType) {
            throw new Exception(name + " is a value type but the value is null.");
         }

         return default;
      }

      var stringBuilder = new StringBuilder(30, 3);
      stringBuilder.Append(name);
      stringBuilder.Append("'s type is ");
      stringBuilder.Append(declareType);
      stringBuilder.Append(" but the value is ");
      stringBuilder.Append(obj.GetType().Name);
      stringBuilder.Append(".");
      throw new Exception(stringBuilder.ToString());
   }

   public static bool TrySetField<T>(
      this object source,
      Type declareType,
      string name,
      T? value,
      BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic)
   {
      FieldInfo? field = declareType.GetField(name, flags);
      if (field is null) {
         return false;
      }

      field.SetValue(source, value);
      return true;
   }

   public static bool TryInvokeMethod(
      this object source,
      Type declareType,
      string name,
      out object? result,
      params object[] parameters)
   {
      MethodInfo? method = declareType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
      if (method is null) {
         result = null;
         return false;
      }

      result = method.Invoke(source, parameters);
      return true;
   }

   public static object? InvokeMethodOrThrow(
      this object source,
      Type declareType,
      string name,
      params object[] parameters)
   {
      return declareType.GetMethodInfoOrThrow(name).Invoke(source, parameters);
   }
}