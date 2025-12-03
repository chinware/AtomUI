using System.Collections.Concurrent;

namespace AtomUI.Controls;

internal static class IconProviderCache
{
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, Type>> TypeCache = 
        new();
    
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, Func<Icon>>> CreatorCache = 
        new();
    
    private static readonly ConcurrentDictionary<Type, Func<Icon>> TypeToCreator = 
        new();
    
    public static Type GetOrAddType(Type enumType, object enumValue, Func<object, Type> typeFactory)
    {
        var cache = TypeCache.GetOrAdd(enumType, 
            _ => new ConcurrentDictionary<object, Type>());
        
        return cache.GetOrAdd(enumValue, typeFactory);
    }
    
    public static Func<Icon> GetOrAddCreator(Type enumType, object enumValue, 
        Func<object, Type> typeFactory, Func<Type, Func<Icon>> creatorFactory)
    {
        var cache = CreatorCache.GetOrAdd(enumType,
            _ => new ConcurrentDictionary<object, Func<Icon>>());
        
        return cache.GetOrAdd(enumValue, value =>
        {
            var type = GetOrAddType(enumType, value, typeFactory);
            return TypeToCreator.GetOrAdd(type, creatorFactory);
        });
    }
    
    public static void ClearCache(Type enumType)
    {
        TypeCache.TryRemove(enumType, out _);
        CreatorCache.TryRemove(enumType, out _);
    }
    
    public static void ClearAllCache()
    {
        TypeCache.Clear();
        CreatorCache.Clear();
        TypeToCreator.Clear();
    }
}