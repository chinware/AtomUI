using System.Collections.Concurrent;

namespace AtomUI.Controls;

internal static class IconProviderCache
{
    /// <summary>
    /// Maximum number of enum types to cache. When exceeded, oldest entries are removed.
    /// </summary>
    private const int MaxCacheSize = 256;
    
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, Type>> TypeCache = 
        new();
    
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<object, Func<Icon>>> CreatorCache = 
        new();
    
    private static readonly ConcurrentDictionary<Type, Func<Icon>> TypeToCreator = 
        new();

    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, byte>> EnumTypeToIconTypes =
        new();
    
    /// <summary>
    /// Tracks insertion order for FIFO eviction when cache exceeds MaxCacheSize.
    /// </summary>
    private static readonly Queue<Type> CacheInsertionOrder = new();
    
    private static readonly object _lockObject = new();
    
    public static Type GetOrAddType(Type enumType, object enumValue, Func<object, Type> typeFactory)
    {
        EnsureCacheSize();
        
        var cache = TypeCache.GetOrAdd(enumType, _ =>
        {
            lock (_lockObject)
            {
                CacheInsertionOrder.Enqueue(enumType);
            }
            return new ConcurrentDictionary<object, Type>();
        });
        
        return cache.GetOrAdd(enumValue, typeFactory);
    }
    
    public static Func<Icon> GetOrAddCreator(Type enumType, object enumValue, 
        Func<object, Type> typeFactory, Func<Type, Func<Icon>> creatorFactory)
    {
        EnsureCacheSize();
        
        var cache = CreatorCache.GetOrAdd(enumType, _ =>
        {
            lock (_lockObject)
            {
                if (!CacheInsertionOrder.Contains(enumType))
                {
                    CacheInsertionOrder.Enqueue(enumType);
                }
            }
            return new ConcurrentDictionary<object, Func<Icon>>();
        });
        
        return cache.GetOrAdd(enumValue, value =>
        {
            var type = GetOrAddType(enumType, value, typeFactory);
            RegisterIconType(enumType, type);
            return TypeToCreator.GetOrAdd(type, creatorFactory);
        });
    }

    private static void RegisterIconType(Type enumType, Type iconType)
    {
        var iconTypes = EnumTypeToIconTypes.GetOrAdd(enumType, _ => new ConcurrentDictionary<Type, byte>());
        iconTypes.TryAdd(iconType, 0);
    }
    
    /// <summary>
    /// Ensures cache size doesn't exceed MaxCacheSize by removing oldest entries.
    /// </summary>
    private static void EnsureCacheSize()
    {
        lock (_lockObject)
        {
            while (Math.Max(TypeCache.Count, CreatorCache.Count) > MaxCacheSize)
            {
                if (!TryDequeueActiveEnumType(out var oldestType))
                {
                    oldestType = TypeCache.Keys.Concat(CreatorCache.Keys).FirstOrDefault();
                    if (oldestType is null)
                    {
                        break;
                    }
                }

                RemoveEnumCache(oldestType);
            }
        }
    }

    private static bool TryDequeueActiveEnumType(out Type enumType)
    {
        while (CacheInsertionOrder.Count > 0)
        {
            enumType = CacheInsertionOrder.Dequeue();
            if (TypeCache.ContainsKey(enumType) ||
                CreatorCache.ContainsKey(enumType) ||
                EnumTypeToIconTypes.ContainsKey(enumType))
            {
                return true;
            }
        }

        enumType = null!;
        return false;
    }

    private static void RemoveEnumCache(Type enumType)
    {
        var iconTypes = new HashSet<Type>();
        if (EnumTypeToIconTypes.TryRemove(enumType, out var trackedIconTypes))
        {
            iconTypes.UnionWith(trackedIconTypes.Keys);
        }

        if (TypeCache.TryRemove(enumType, out var typeCache))
        {
            iconTypes.UnionWith(typeCache.Values);
        }

        CreatorCache.TryRemove(enumType, out _);

        foreach (var iconType in iconTypes)
        {
            TypeToCreator.TryRemove(iconType, out _);
        }
    }
    
    public static void ClearCache(Type enumType)
    {
        lock (_lockObject)
        {
            RemoveEnumCache(enumType);
            // Note: We don't remove from CacheInsertionOrder to avoid lock contention
            // The queue will naturally be cleaned up during EnsureCacheSize operations
        }
    }
    
    public static void ClearAllCache()
    {
        lock (_lockObject)
        {
            TypeCache.Clear();
            CreatorCache.Clear();
            TypeToCreator.Clear();
            EnumTypeToIconTypes.Clear();
            CacheInsertionOrder.Clear();
        }
    }
}
