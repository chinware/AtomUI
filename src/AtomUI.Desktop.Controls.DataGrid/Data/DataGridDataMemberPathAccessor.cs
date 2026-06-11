using System.Collections;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Controls.Data;
using AtomUI.Utils;

namespace AtomUI.Desktop.Controls.Data;

internal interface IDataGridDataMemberPathAccessor
{
    Type? ValueType { get; }

    IComparer? Comparer { get; }

    object? GetValue(object instance);
}

internal static class DataGridDataMemberPathAccessor
{
    public static IDataGridDataMemberPathAccessor Resolve(string propertyPath,
                                                          Type itemType,
                                                          IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                                          bool isDynamicCodeSupported)
    {
        if (TryResolveGeneratedAccessor(propertyPath, itemType, dataMemberAccessorDescriptor, out var accessor))
        {
            return new GeneratedDataMemberPathAccessor(accessor);
        }

        if (!isDynamicCodeSupported)
        {
            throw new InvalidOperationException(
                $"AOT-safe data member accessor for path '{propertyPath}' on type '{itemType.FullName}' was not found. " +
                $"Mark the data type with [{nameof(GenerateDataMemberAccessorsAttribute)}] or pass an {nameof(IDataMemberAccessorDescriptor)}.");
        }

        return RuntimeReflectionDataMemberPathAccessor.Create(propertyPath, itemType);
    }

    private static bool TryResolveGeneratedAccessor(string propertyPath,
                                                    Type itemType,
                                                    IDataMemberAccessorDescriptor? dataMemberAccessorDescriptor,
                                                    out IDataMemberAccessor accessor)
    {
        if (dataMemberAccessorDescriptor != null &&
            IsDescriptorCompatible(dataMemberAccessorDescriptor, itemType) &&
            dataMemberAccessorDescriptor.TryGetAccessor(propertyPath, out accessor!))
        {
            return true;
        }

        if (DataMemberAccessorRegistry.TryGetCompatible(itemType, out var descriptor) &&
            descriptor.TryGetAccessor(propertyPath, out accessor!))
        {
            return true;
        }

        accessor = null!;
        return false;
    }

    private static bool IsDescriptorCompatible(IDataMemberAccessorDescriptor descriptor, Type itemType)
    {
        return descriptor.DataType == itemType || descriptor.DataType.IsAssignableFrom(itemType);
    }

    private sealed class GeneratedDataMemberPathAccessor : IDataGridDataMemberPathAccessor
    {
        private readonly IDataMemberAccessor _accessor;

        public Type ValueType => _accessor.ValueType;

        public IComparer? Comparer => _accessor.Comparer;

        public GeneratedDataMemberPathAccessor(IDataMemberAccessor accessor)
        {
            _accessor = accessor;
        }

        public object? GetValue(object instance)
        {
            return _accessor.GetValue(instance);
        }
    }

    private sealed class RuntimeReflectionDataMemberPathAccessor : IDataGridDataMemberPathAccessor
    {
        private readonly string _propertyPath;

        public Type? ValueType { get; }

        public IComparer? Comparer => null;

        private RuntimeReflectionDataMemberPathAccessor(string propertyPath, Type? valueType)
        {
            _propertyPath = propertyPath;
            ValueType     = valueType;
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026",
            Justification = "Legacy reflection path is created only when dynamic code is available; NativeAOT requires generated data member accessors.")]
        public static RuntimeReflectionDataMemberPathAccessor Create(string propertyPath, Type itemType)
        {
            return new RuntimeReflectionDataMemberPathAccessor(
                propertyPath,
                itemType.GetNestedPropertyType(propertyPath));
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026",
            Justification = "Legacy reflection path is created only when dynamic code is available; NativeAOT requires generated data member accessors.")]
        public object? GetValue(object instance)
        {
            if (ValueType == null)
            {
                return null;
            }

            object? propertyValue =
                TypeHelper.GetNestedPropertyValue(instance, _propertyPath, ValueType, out Exception? exception);
            if (exception != null)
            {
                throw exception;
            }

            return propertyValue;
        }
    }
}
