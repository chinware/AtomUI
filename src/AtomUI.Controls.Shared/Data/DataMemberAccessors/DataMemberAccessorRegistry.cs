using System.Collections.Concurrent;

namespace AtomUI.Controls.Data;

public static class DataMemberAccessorRegistry
{
    private static readonly ConcurrentDictionary<Type, IDataMemberAccessorDescriptor> s_descriptors = new();
    private static readonly ConcurrentDictionary<Type, DescriptorLookupResult> s_compatibleDescriptorCache = new();

    public static void Register(IDataMemberAccessorDescriptor descriptor)
    {
        s_descriptors[descriptor.DataType] = descriptor;
        s_compatibleDescriptorCache.Clear();
    }

    public static bool TryGet(Type dataType, out IDataMemberAccessorDescriptor descriptor)
    {
        return s_descriptors.TryGetValue(dataType, out descriptor!);
    }

    public static bool TryGetCompatible(Type dataType, out IDataMemberAccessorDescriptor descriptor)
    {
        if (TryGet(dataType, out descriptor!))
        {
            return true;
        }

        var result = s_compatibleDescriptorCache.GetOrAdd(dataType, FindCompatibleDescriptor);
        if (result.Descriptor != null)
        {
            descriptor = result.Descriptor;
            return true;
        }

        descriptor = null!;
        return false;
    }

    private static DescriptorLookupResult FindCompatibleDescriptor(Type dataType)
    {
        IDataMemberAccessorDescriptor? bestDescriptor = null;
        foreach (var descriptor in s_descriptors.Values)
        {
            var descriptorType = descriptor.DataType;
            if (!descriptorType.IsAssignableFrom(dataType))
            {
                continue;
            }

            if (bestDescriptor == null || IsMoreSpecific(descriptorType, bestDescriptor.DataType))
            {
                bestDescriptor = descriptor;
            }
        }

        return new DescriptorLookupResult(bestDescriptor);
    }

    private static bool IsMoreSpecific(Type candidateType, Type currentType)
    {
        if (candidateType == currentType)
        {
            return false;
        }

        if (currentType.IsAssignableFrom(candidateType))
        {
            return true;
        }

        return candidateType.IsClass && currentType.IsInterface;
    }

    private readonly struct DescriptorLookupResult
    {
        public IDataMemberAccessorDescriptor? Descriptor { get; }

        public DescriptorLookupResult(IDataMemberAccessorDescriptor? descriptor)
        {
            Descriptor = descriptor;
        }
    }
}
