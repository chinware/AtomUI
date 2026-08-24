namespace AtomUI.Controls;

internal sealed record ImageFileCacheMetadata(
    string PartitionHash,
    long ContentLength,
    string ContentDigest,
    string? MediaType,
    long StoredAtUtcTicks,
    long? FreshUntilUtcTicks,
    string? ETag,
    long? LastModifiedUtcTicks,
    bool NoCache,
    bool MustRevalidate,
    bool IsRemote,
    bool IsTrustedAsset,
    string? SourceVersion,
    string[]? VaryHeaders,
    string? VaryDigest,
    long? ResponseDateUtcTicks,
    long? ResponseAgeTicks,
    long? ExpiresUtcTicks,
    long? MaxAgeTicks,
    bool IsPrivate,
    int SecurityPolicyVersion)
{
    private const int SchemaVersion = 2;

    internal static ImageFileCacheMetadata FromContent(
        string partitionHash,
        ImageEncodedContent content)
    {
        return new ImageFileCacheMetadata(
            partitionHash,
            content.Bytes.LongLength,
            ImageCacheKey.HashBytes(content.Bytes),
            content.MediaType,
            content.StoredAt.UtcTicks,
            content.FreshUntil?.UtcTicks,
            content.ETag,
            content.LastModified?.UtcTicks,
            content.NoCache,
            content.MustRevalidate,
            content.IsRemote,
            content.IsTrustedAsset,
            content.SourceVersion,
            content.VaryHeaders,
            content.VaryDigest,
            content.ResponseDate?.UtcTicks,
            content.ResponseAge?.Ticks,
            content.Expires?.UtcTicks,
            content.MaxAge?.Ticks,
            content.IsPrivate,
            content.SecurityPolicyVersion);
    }

    internal ImageEncodedContent ToContent(byte[] bytes)
    {
        return new ImageEncodedContent(
            bytes,
            MediaType,
            ImageCacheSource.Persistent,
            new DateTimeOffset(StoredAtUtcTicks, TimeSpan.Zero),
            FreshUntil: FreshUntilUtcTicks is { } fresh ? new DateTimeOffset(fresh, TimeSpan.Zero) : null,
            ETag: ETag,
            LastModified: LastModifiedUtcTicks is { } modified
                ? new DateTimeOffset(modified, TimeSpan.Zero)
                : null,
            NoStore: false,
            NoCache: NoCache,
            MustRevalidate: MustRevalidate,
            IsRemote: IsRemote,
            IsTrustedAsset: IsTrustedAsset,
            SourceVersion: SourceVersion,
            VaryHeaders: VaryHeaders,
            VaryDigest: VaryDigest,
            SecurityPolicyVersion: SecurityPolicyVersion,
            ResponseDate: ResponseDateUtcTicks is { } responseDate
                ? new DateTimeOffset(responseDate, TimeSpan.Zero)
                : null,
            ResponseAge: ResponseAgeTicks is { } responseAge ? TimeSpan.FromTicks(responseAge) : null,
            Expires: ExpiresUtcTicks is { } expires ? new DateTimeOffset(expires, TimeSpan.Zero) : null,
            MaxAge: MaxAgeTicks is { } maxAge ? TimeSpan.FromTicks(maxAge) : null,
            IsPrivate: IsPrivate);
    }

    internal void Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(SchemaVersion);
        writer.Write(PartitionHash);
        writer.Write(ContentLength);
        writer.Write(ContentDigest);
        WriteNullable(writer, MediaType);
        writer.Write(StoredAtUtcTicks);
        WriteNullable(writer, FreshUntilUtcTicks);
        WriteNullable(writer, ETag);
        WriteNullable(writer, LastModifiedUtcTicks);
        writer.Write(NoCache);
        writer.Write(MustRevalidate);
        writer.Write(IsRemote);
        writer.Write(IsTrustedAsset);
        WriteNullable(writer, SourceVersion);
        WriteNullable(writer, VaryHeaders);
        WriteNullable(writer, VaryDigest);
        WriteNullable(writer, ResponseDateUtcTicks);
        WriteNullable(writer, ResponseAgeTicks);
        WriteNullable(writer, ExpiresUtcTicks);
        WriteNullable(writer, MaxAgeTicks);
        writer.Write(IsPrivate);
        writer.Write(SecurityPolicyVersion);
    }

    internal static ImageFileCacheMetadata Read(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        if (reader.ReadInt32() != SchemaVersion)
        {
            throw new InvalidDataException("Unsupported image cache metadata version.");
        }
        return new ImageFileCacheMetadata(
            reader.ReadString(),
            reader.ReadInt64(),
            reader.ReadString(),
            ReadNullableString(reader),
            reader.ReadInt64(),
            ReadNullableInt64(reader),
            ReadNullableString(reader),
            ReadNullableInt64(reader),
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            ReadNullableString(reader),
            ReadNullableStrings(reader),
            ReadNullableString(reader),
            ReadNullableInt64(reader),
            ReadNullableInt64(reader),
            ReadNullableInt64(reader),
            ReadNullableInt64(reader),
            reader.ReadBoolean(),
            reader.ReadInt32());
    }

    private static void WriteNullable(BinaryWriter writer, string? value)
    {
        writer.Write(value is not null);
        if (value is not null)
        {
            writer.Write(value);
        }
    }

    private static void WriteNullable(BinaryWriter writer, long? value)
    {
        writer.Write(value.HasValue);
        if (value.HasValue)
        {
            writer.Write(value.Value);
        }
    }

    private static void WriteNullable(BinaryWriter writer, string[]? values)
    {
        writer.Write(values is not null);
        if (values is null)
        {
            return;
        }
        writer.Write(values.Length);
        foreach (var value in values)
        {
            writer.Write(value);
        }
    }

    private static string? ReadNullableString(BinaryReader reader) =>
        reader.ReadBoolean() ? reader.ReadString() : null;

    private static long? ReadNullableInt64(BinaryReader reader) =>
        reader.ReadBoolean() ? reader.ReadInt64() : null;

    private static string[]? ReadNullableStrings(BinaryReader reader)
    {
        if (!reader.ReadBoolean())
        {
            return null;
        }
        var count = reader.ReadInt32();
        if (count < 0 || count > 256)
        {
            throw new InvalidDataException("Image cache metadata contains an invalid header count.");
        }
        var values = new string[count];
        for (var index = 0; index < count; index++)
        {
            values[index] = reader.ReadString();
        }
        return values;
    }
}
