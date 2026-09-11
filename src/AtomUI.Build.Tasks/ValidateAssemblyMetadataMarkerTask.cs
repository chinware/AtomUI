using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using Microsoft.Build.Framework;

namespace AtomUI.Build.Tasks;

public sealed class ValidateAssemblyMetadataMarkerTask : ITask
{
    private const string DiagnosticCode = "ATOMUILINK001";
    private const string AssemblyMetadataNamespace = "System.Reflection";
    private const string AssemblyMetadataName = "AssemblyMetadataAttribute";

    [Required]
    public string AssemblyPath { get; set; } = string.Empty;

    [Required]
    public string MarkerKey { get; set; } = string.Empty;

    [Output]
    public int MarkerCount { get; set; }

    public IBuildEngine BuildEngine { get; set; } = null!;

    public ITaskHost HostObject { get; set; } = null!;

    public bool Execute()
    {
        try
        {
            using var stream = File.OpenRead(AssemblyPath);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
            {
                LogReadError("The file does not contain managed metadata.");
                return false;
            }

            var metadataReader = peReader.GetMetadataReader();
            if (!metadataReader.IsAssembly)
            {
                LogReadError("The file is not a managed assembly.");
                return false;
            }

            MarkerCount = CountMarkers(metadataReader, MarkerKey);
            return true;
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException or BadImageFormatException)
        {
            LogReadError(exception.Message);
            return false;
        }
    }

    private static int CountMarkers(MetadataReader reader, string markerKey)
    {
        var count = 0;
        foreach (var attributeHandle in reader.GetAssemblyDefinition().GetCustomAttributes())
        {
            var attribute = reader.GetCustomAttribute(attributeHandle);
            if (!IsAssemblyMetadataAttribute(reader, attribute.Constructor) ||
                !TryReadMarkerKey(reader, attribute, out var key) ||
                !string.Equals(key, markerKey, StringComparison.Ordinal))
            {
                continue;
            }

            count++;
        }

        return count;
    }

    private static bool IsAssemblyMetadataAttribute(MetadataReader reader, EntityHandle constructor)
    {
        EntityHandle declaringType;
        switch (constructor.Kind)
        {
            case HandleKind.MemberReference:
                declaringType = reader.GetMemberReference((MemberReferenceHandle)constructor).Parent;
                break;
            case HandleKind.MethodDefinition:
                declaringType = reader.GetMethodDefinition((MethodDefinitionHandle)constructor).GetDeclaringType();
                break;
            default:
                return false;
        }

        return IsType(reader, declaringType, AssemblyMetadataNamespace, AssemblyMetadataName);
    }

    private static bool IsType(
        MetadataReader reader,
        EntityHandle handle,
        string expectedNamespace,
        string expectedName)
    {
        StringHandle namespaceHandle;
        StringHandle nameHandle;
        switch (handle.Kind)
        {
            case HandleKind.TypeReference:
                var typeReference = reader.GetTypeReference((TypeReferenceHandle)handle);
                namespaceHandle = typeReference.Namespace;
                nameHandle = typeReference.Name;
                break;
            case HandleKind.TypeDefinition:
                var typeDefinition = reader.GetTypeDefinition((TypeDefinitionHandle)handle);
                namespaceHandle = typeDefinition.Namespace;
                nameHandle = typeDefinition.Name;
                break;
            default:
                return false;
        }

        return reader.StringComparer.Equals(namespaceHandle, expectedNamespace) &&
               reader.StringComparer.Equals(nameHandle, expectedName);
    }

    private static bool TryReadMarkerKey(
        MetadataReader reader,
        CustomAttribute attribute,
        out string? markerKey)
    {
        markerKey = null;
        try
        {
            var valueReader = reader.GetBlobReader(attribute.Value);
            if (valueReader.ReadUInt16() != 1)
            {
                return false;
            }

            markerKey = valueReader.ReadSerializedString();
            return markerKey is not null;
        }
        catch (BadImageFormatException)
        {
            return false;
        }
    }

    private void LogReadError(string detail)
    {
        BuildEngine.LogErrorEvent(new BuildErrorEventArgs(
            subcategory: "LinkedRegistration",
            DiagnosticCode,
            AssemblyPath,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            $"Cannot validate the generated Application Plan marker in '{AssemblyPath}': {detail}",
            helpKeyword: null,
            senderName: nameof(ValidateAssemblyMetadataMarkerTask)));
    }
}
