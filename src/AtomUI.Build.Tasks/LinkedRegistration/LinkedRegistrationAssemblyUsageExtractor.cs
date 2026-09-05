using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using AtomUI.Generator.LinkedRegistration;

namespace AtomUI.LinkedRegistration.Protocol;

internal static class LinkedRegistrationPackageCatalogReader
{
    internal static bool TryRead(
        IEnumerable<string> sidecarPaths,
        out LinkedRegistrationPackageCatalog catalog,
        out string error)
    {
        var packages = new Dictionary<string, LinkedRegistrationPackageDefinition>(StringComparer.Ordinal);
        var sidecarAssemblies = new HashSet<string>(StringComparer.Ordinal);
        error = string.Empty;
        long totalBytes = 0;

        foreach (var path in sidecarPaths.Distinct(StringComparer.Ordinal))
        {
            try
            {
                var bytes = File.ReadAllBytes(path);
                totalBytes += bytes.Length;
                if (totalBytes > LinkedRegistrationAnalysisBudget.MaxTotalSidecarBytes)
                {
                    catalog = LinkedRegistrationPackageCatalog.Empty;
                    error = $"catalog Sidecars exceed the {LinkedRegistrationAnalysisBudget.MaxTotalSidecarBytes} byte analysis limit";
                    return false;
                }
                if (!LinkedRegistrationSidecarCodec.TryRead(bytes, out var sidecar, out error) ||
                    sidecar is null)
                {
                    catalog = LinkedRegistrationPackageCatalog.Empty;
                    error = $"catalog Sidecar '{path}' is invalid: {error}";
                    return false;
                }

                sidecarAssemblies.Add(sidecar.Assembly.Name);
                foreach (var package in sidecar.Packages)
                {
                    var definition = new LinkedRegistrationPackageDefinition(
                        package.Id,
                        package.AssemblyName,
                        package.EntryMethods);
                    if (packages.TryGetValue(package.Id, out var existing) &&
                        !existing.Equals(definition))
                    {
                        catalog = LinkedRegistrationPackageCatalog.Empty;
                        error = $"package '{package.Id}' has conflicting catalog definitions";
                        return false;
                    }
                    packages[package.Id] = definition;
                }
            }
            catch (Exception exception) when (
                exception is IOException or UnauthorizedAccessException or ArgumentException)
            {
                catalog = LinkedRegistrationPackageCatalog.Empty;
                error = $"cannot read catalog Sidecar '{path}': {exception.Message}";
                return false;
            }
        }

        catalog = new LinkedRegistrationPackageCatalog(
            packages.Values.OrderBy(static package => package.PackageId, StringComparer.Ordinal).ToArray(),
            sidecarAssemblies);
        return true;
    }
}

internal sealed class LinkedRegistrationPackageCatalog
{
    internal static LinkedRegistrationPackageCatalog Empty { get; } = new(
        [],
        new HashSet<string>(StringComparer.Ordinal));

    internal LinkedRegistrationPackageCatalog(
        IReadOnlyList<LinkedRegistrationPackageDefinition> packages,
        HashSet<string> sidecarAssemblyNames)
    {
        Packages = packages;
        SidecarAssemblyNames = sidecarAssemblyNames;
    }

    internal IReadOnlyList<LinkedRegistrationPackageDefinition> Packages { get; }

    internal HashSet<string> SidecarAssemblyNames { get; }
}

internal sealed class LinkedRegistrationPackageDefinition : IEquatable<LinkedRegistrationPackageDefinition>
{
    internal LinkedRegistrationPackageDefinition(
        string packageId,
        string assemblyName,
        IEnumerable<string> entryMethods)
    {
        PackageId = packageId;
        AssemblyName = assemblyName;
        EntryMethods = entryMethods.Distinct(StringComparer.Ordinal)
            .OrderBy(static entry => entry, StringComparer.Ordinal)
            .ToArray();
    }

    internal string PackageId { get; }

    internal string AssemblyName { get; }

    internal IReadOnlyList<string> EntryMethods { get; }

    public bool Equals(LinkedRegistrationPackageDefinition? other)
    {
        return other is not null &&
               PackageId == other.PackageId &&
               AssemblyName == other.AssemblyName &&
               EntryMethods.SequenceEqual(other.EntryMethods, StringComparer.Ordinal);
    }

    public override bool Equals(object? obj) => Equals(obj as LinkedRegistrationPackageDefinition);

    public override int GetHashCode()
    {
        unchecked
        {
            var hash = StringComparer.Ordinal.GetHashCode(PackageId);
            hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(AssemblyName);
            foreach (var entry in EntryMethods)
            {
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(entry);
            }
            return hash;
        }
    }
}

internal static class LinkedRegistrationAssemblyUsageExtractor
{
    private static readonly IReadOnlyDictionary<ushort, OpCode> s_opCodes = CreateOpCodeMap();

    internal static bool TryReadAssemblyReferences(
        string assemblyPath,
        out string assemblyName,
        out HashSet<string> referencedAssemblies,
        out string error)
    {
        assemblyName = string.Empty;
        referencedAssemblies = new HashSet<string>(StringComparer.Ordinal);
        error = string.Empty;
        try
        {
            using var stream = File.OpenRead(assemblyPath);
            using var peReader = new PEReader(stream);
            if (!peReader.HasMetadata)
            {
                return false;
            }
            var reader = peReader.GetMetadataReader();
            if (!reader.IsAssembly)
            {
                return false;
            }
            assemblyName = reader.GetString(reader.GetAssemblyDefinition().Name);
            referencedAssemblies = new HashSet<string>(
                reader.AssemblyReferences.Select(handle =>
                    reader.GetString(reader.GetAssemblyReference(handle).Name)),
                StringComparer.Ordinal);
            return true;
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or ArgumentException or
            BadImageFormatException or InvalidOperationException)
        {
            error = exception.Message;
            return false;
        }
    }

    internal static bool TryExtract(
        PEReader peReader,
        MetadataReader reader,
        LinkedRegistrationPackageCatalog catalog,
        out LinkedSidecarUsage[] usages,
        out LinkedSidecarFallback[] fallbacks,
        out string error)
    {
        usages = [];
        fallbacks = [];
        error = string.Empty;
        var assemblyName = reader.GetString(reader.GetAssemblyDefinition().Name);
        var source = assemblyName + ".dll";
        var referencedAssemblies = new HashSet<string>(
            reader.AssemblyReferences.Select(handle =>
                reader.GetString(reader.GetAssemblyReference(handle).Name)),
            StringComparer.Ordinal);
        var referencedPackages = catalog.Packages
            .Where(package => referencedAssemblies.Contains(package.AssemblyName))
            .ToArray();
        if (referencedPackages.Length == 0)
        {
            return true;
        }

        var entryPackages = new Dictionary<MethodIdentity, string>();
        foreach (var package in referencedPackages)
        {
            foreach (var entry in package.EntryMethods)
            {
                var separator = entry.LastIndexOf('.');
                if (separator <= 0 || separator == entry.Length - 1)
                {
                    error = $"package '{package.PackageId}' contains invalid entry method '{entry}'";
                    return false;
                }
                var identity = new MethodIdentity(
                    package.AssemblyName,
                    entry.Substring(0, separator),
                    entry.Substring(separator + 1));
                if (entryPackages.TryGetValue(identity, out var existingPackage) &&
                    !string.Equals(existingPackage, package.PackageId, StringComparison.Ordinal))
                {
                    error = $"entry method '{entry}' maps to both '{existingPackage}' and '{package.PackageId}'";
                    return false;
                }
                entryPackages[identity] = package.PackageId;
            }
        }

        var invokedPackages = new HashSet<string>(StringComparer.Ordinal);
        try
        {
            if (reader.MethodDefinitions.Count > LinkedRegistrationAnalysisBudget.MaxConsumerAssemblyMethods)
            {
                error = $"assembly '{assemblyName}' contains more than the " +
                        $"{LinkedRegistrationAnalysisBudget.MaxConsumerAssemblyMethods} method analysis limit";
                return false;
            }
            long analyzedIlBytes = 0;
            foreach (var methodHandle in reader.MethodDefinitions)
            {
                var method = reader.GetMethodDefinition(methodHandle);
                if (method.RelativeVirtualAddress == 0)
                {
                    continue;
                }
                var il = peReader.GetMethodBody(method.RelativeVirtualAddress).GetILReader();
                analyzedIlBytes += il.Length;
                if (analyzedIlBytes > LinkedRegistrationAnalysisBudget.MaxConsumerAssemblyIlBytes)
                {
                    error = $"assembly '{assemblyName}' exceeds the " +
                            $"{LinkedRegistrationAnalysisBudget.MaxConsumerAssemblyIlBytes} byte IL analysis limit";
                    return false;
                }
                while (il.RemainingBytes > 0)
                {
                    if (!TryReadInstruction(ref il, out var opCode, out var methodToken, out error))
                    {
                        return false;
                    }
                    if (methodToken == 0 || !IsEntryCallOpCode(opCode))
                    {
                        continue;
                    }
                    var handle = MetadataTokens.EntityHandle(methodToken);
                    if (TryGetMethodIdentity(reader, handle, assemblyName, out var identity) &&
                        entryPackages.TryGetValue(identity, out var packageId))
                    {
                        invokedPackages.Add(packageId);
                    }
                }
            }
        }
        catch (Exception exception) when (
            exception is BadImageFormatException or ArgumentException or InvalidOperationException)
        {
            error = exception.Message;
            return false;
        }

        usages = referencedPackages.Select(static package => package.PackageId)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static packageId => packageId, StringComparer.Ordinal)
            .Select(packageId => new LinkedSidecarUsage
            {
                Kind = "PackageRoot",
                Identity = packageId,
                Source = source,
                Line = 0,
                Column = 0
            })
            .Concat(invokedPackages.OrderBy(static packageId => packageId, StringComparer.Ordinal)
                .Select(packageId => new LinkedSidecarUsage
                {
                    Kind = "Entry",
                    Identity = packageId,
                    Source = source,
                    Line = 0,
                    Column = 0
                }))
            .ToArray();
        fallbacks = referencedPackages.Select(static package => package.PackageId)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static packageId => packageId, StringComparer.Ordinal)
            .Select(packageId => new LinkedSidecarFallback
            {
                PackageId = packageId,
                Reason = LinkedRegistrationProtocol.FallbackReasonExtractedConsumerAssembly,
                Source = source,
                Line = 0,
                Column = 0
            })
            .ToArray();
        return true;
    }

    private static bool TryReadInstruction(
        ref BlobReader reader,
        out OpCode opCode,
        out int methodToken,
        out string error)
    {
        opCode = default;
        methodToken = 0;
        error = string.Empty;
        var first = reader.ReadByte();
        var value = first == 0xfe
            ? (ushort)(0xfe00 | reader.ReadByte())
            : first;
        if (!s_opCodes.TryGetValue(value, out opCode))
        {
            error = $"unknown IL opcode 0x{value:x4}";
            return false;
        }

        var operandSize = GetFixedOperandSize(opCode.OperandType);
        if (opCode.OperandType == OperandType.InlineSwitch)
        {
            if (reader.RemainingBytes < sizeof(int))
            {
                error = "truncated IL switch operand";
                return false;
            }
            var count = reader.ReadInt32();
            if (count < 0 || count > reader.RemainingBytes / sizeof(int))
            {
                error = "invalid IL switch operand";
                return false;
            }
            reader.Offset += count * sizeof(int);
            return true;
        }
        if (operandSize < 0 || reader.RemainingBytes < operandSize)
        {
            error = $"invalid or truncated operand for IL opcode '{opCode.Name}'";
            return false;
        }
        if (opCode.OperandType == OperandType.InlineMethod)
        {
            methodToken = reader.ReadInt32();
        }
        else
        {
            reader.Offset += operandSize;
        }
        return true;
    }

    private static int GetFixedOperandSize(OperandType operandType)
    {
        return operandType switch
        {
            OperandType.InlineNone => 0,
            OperandType.ShortInlineBrTarget or OperandType.ShortInlineI or
                OperandType.ShortInlineVar => 1,
            OperandType.InlineVar => 2,
            OperandType.InlineBrTarget or OperandType.InlineField or OperandType.InlineI or
                OperandType.InlineMethod or OperandType.InlineSig or OperandType.InlineString or
                OperandType.InlineTok or OperandType.InlineType or OperandType.ShortInlineR => 4,
            OperandType.InlineI8 or OperandType.InlineR => 8,
            OperandType.InlineSwitch => -1,
            _ => -1
        };
    }

    private static bool IsEntryCallOpCode(OpCode opCode)
    {
        return opCode.Value == OpCodes.Call.Value ||
               opCode.Value == OpCodes.Callvirt.Value ||
               opCode.Value == OpCodes.Ldftn.Value ||
               opCode.Value == OpCodes.Ldvirtftn.Value;
    }

    private static bool TryGetMethodIdentity(
        MetadataReader reader,
        EntityHandle handle,
        string currentAssembly,
        out MethodIdentity identity)
    {
        if (handle.Kind == HandleKind.MethodSpecification)
        {
            return TryGetMethodIdentity(
                reader,
                reader.GetMethodSpecification((MethodSpecificationHandle)handle).Method,
                currentAssembly,
                out identity);
        }

        EntityHandle declaringType;
        StringHandle methodName;
        switch (handle.Kind)
        {
            case HandleKind.MemberReference:
                var member = reader.GetMemberReference((MemberReferenceHandle)handle);
                declaringType = member.Parent;
                methodName = member.Name;
                break;
            case HandleKind.MethodDefinition:
                var method = reader.GetMethodDefinition((MethodDefinitionHandle)handle);
                declaringType = method.GetDeclaringType();
                methodName = method.Name;
                break;
            default:
                identity = default;
                return false;
        }
        if (!TryGetTypeIdentity(
                reader,
                declaringType,
                currentAssembly,
                out var assemblyName,
                out var typeName))
        {
            identity = default;
            return false;
        }
        identity = new MethodIdentity(assemblyName, typeName, reader.GetString(methodName));
        return true;
    }

    private static bool TryGetTypeIdentity(
        MetadataReader reader,
        EntityHandle handle,
        string currentAssembly,
        out string assemblyName,
        out string typeName)
    {
        switch (handle.Kind)
        {
            case HandleKind.TypeReference:
                var reference = reader.GetTypeReference((TypeReferenceHandle)handle);
                var name = reader.GetString(reference.Name);
                if (reference.ResolutionScope.Kind == HandleKind.TypeReference)
                {
                    if (!TryGetTypeIdentity(
                            reader,
                            reference.ResolutionScope,
                            currentAssembly,
                            out assemblyName,
                            out var declaringType))
                    {
                        typeName = string.Empty;
                        return false;
                    }
                    typeName = declaringType + "+" + name;
                    return true;
                }
                assemblyName = reference.ResolutionScope.Kind == HandleKind.AssemblyReference
                    ? reader.GetString(reader.GetAssemblyReference(
                        (AssemblyReferenceHandle)reference.ResolutionScope).Name)
                    : currentAssembly;
                var typeNamespace = reader.GetString(reference.Namespace);
                typeName = typeNamespace.Length == 0 ? name : typeNamespace + "." + name;
                return true;
            case HandleKind.TypeDefinition:
                var definition = reader.GetTypeDefinition((TypeDefinitionHandle)handle);
                var definitionName = reader.GetString(definition.Name);
                var declaringHandle = definition.GetDeclaringType();
                assemblyName = currentAssembly;
                if (!declaringHandle.IsNil)
                {
                    if (!TryGetTypeIdentity(
                            reader,
                            declaringHandle,
                            currentAssembly,
                            out _,
                            out var declaringType))
                    {
                        typeName = string.Empty;
                        return false;
                    }
                    typeName = declaringType + "+" + definitionName;
                    return true;
                }
                var definitionNamespace = reader.GetString(definition.Namespace);
                typeName = definitionNamespace.Length == 0
                    ? definitionName
                    : definitionNamespace + "." + definitionName;
                return true;
            default:
                assemblyName = string.Empty;
                typeName = string.Empty;
                return false;
        }
    }

    private static IReadOnlyDictionary<ushort, OpCode> CreateOpCodeMap()
    {
        var result = new Dictionary<ushort, OpCode>();
        foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            if (field.GetValue(null) is OpCode opCode)
            {
                result[(ushort)opCode.Value] = opCode;
            }
        }
        return result;
    }

    private readonly struct MethodIdentity : IEquatable<MethodIdentity>
    {
        internal MethodIdentity(string assemblyName, string typeName, string methodName)
        {
            AssemblyName = assemblyName;
            TypeName = typeName;
            MethodName = methodName;
        }

        private string AssemblyName { get; }

        private string TypeName { get; }

        private string MethodName { get; }

        public bool Equals(MethodIdentity other)
        {
            return AssemblyName == other.AssemblyName &&
                   TypeName == other.TypeName &&
                   MethodName == other.MethodName;
        }

        public override bool Equals(object? obj)
        {
            return obj is MethodIdentity other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = StringComparer.Ordinal.GetHashCode(AssemblyName);
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(TypeName);
                return (hash * 397) ^ StringComparer.Ordinal.GetHashCode(MethodName);
            }
        }
    }
}
