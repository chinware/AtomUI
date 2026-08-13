using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace AtomUI.Generator.LinkedRegistration.Manifest;

internal static class LinkedRegistrationMetadataWriter
{
    internal static void Write(
        StringBuilder source,
        LinkedRegistrationManifestRecord record)
    {
        var envelope = LinkedRegistrationManifestCodec.Encode(record);
        source.Append("[assembly: global::System.Reflection.AssemblyMetadata(")
              .Append(SymbolDisplay.FormatLiteral(envelope.Key, quote: true))
              .Append(", ")
              .Append(SymbolDisplay.FormatLiteral(envelope.Value, quote: true))
              .AppendLine(")]");
    }
}
