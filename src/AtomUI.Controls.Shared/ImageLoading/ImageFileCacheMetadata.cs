namespace AtomUI.Controls;

internal sealed record ImageStoredContentMetadata(
    string PartitionHash,
    string ContentId,
    long ContentLength,
    string ContentDigest,
    string? MediaType,
    long StoredAtUtcTicks,
    int SecurityPolicyVersion)
{
    private const int FormatRevision = 1;

    internal static ImageStoredContentMetadata FromContent(
        ImageEncodedContentKey key,
        ImageEncodedContent content) =>
        new(
            key.PartitionHash,
            key.ContentId.Value,
            content.Bytes.LongLength,
            ImageCacheKey.HashBytes(content.Bytes),
            content.MediaType,
            content.StoredAt.UtcTicks,
            content.SecurityPolicyVersion);

    internal ImageEncodedContent ToContent(byte[] bytes) =>
        new(
            bytes,
            MediaType,
            ImageLoadOrigin.Persistent,
            new DateTimeOffset(StoredAtUtcTicks, TimeSpan.Zero),
            SecurityPolicyVersion: SecurityPolicyVersion,
            ContentId: new ImageContentId(ContentId));

    internal void Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(FormatRevision);
        writer.Write(PartitionHash);
        writer.Write(ContentId);
        writer.Write(ContentLength);
        writer.Write(ContentDigest);
        WriteNullable(writer, MediaType);
        writer.Write(StoredAtUtcTicks);
        writer.Write(SecurityPolicyVersion);
    }

    internal static ImageStoredContentMetadata Read(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        if (reader.ReadInt32() != FormatRevision)
        {
            throw new InvalidDataException("Unsupported image content metadata format revision.");
        }
        return new ImageStoredContentMetadata(
            reader.ReadString(),
            reader.ReadString(),
            reader.ReadInt64(),
            reader.ReadString(),
            ReadNullableString(reader),
            reader.ReadInt64(),
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

    private static string? ReadNullableString(BinaryReader reader) =>
        reader.ReadBoolean() ? reader.ReadString() : null;

}

internal sealed record ImageStoredSourceSnapshot(
    string PartitionHash,
    string SourceKey,
    string SourceVersion,
    string ContentId,
    long CommitGeneration,
    long StoredAtUtcTicks,
    long? FreshUntilUtcTicks,
    string? ETag,
    long? LastModifiedUtcTicks,
    bool NoCache,
    bool MustRevalidate,
    bool IsRemote,
    bool IsTrustedAsset,
    string[]? VaryHeaders,
    string? VaryDigest,
    long? ResponseDateUtcTicks,
    long? ResponseAgeTicks,
    long? ExpiresUtcTicks,
    long? MaxAgeTicks,
    bool IsPrivate,
    int SecurityPolicyVersion)
{
    private const int FormatRevision = 1;

    internal static ImageStoredSourceSnapshot FromSnapshot(ImageSourceSnapshot snapshot) =>
        new(
            snapshot.SourceKey.PartitionHash,
            snapshot.SourceKey.Value,
            snapshot.SourceVersion.Value,
            snapshot.ContentId.Value,
            snapshot.CommitGeneration,
            snapshot.StoredAt.UtcTicks,
            snapshot.FreshUntil?.UtcTicks,
            snapshot.ETag,
            snapshot.LastModified?.UtcTicks,
            snapshot.NoCache,
            snapshot.MustRevalidate,
            snapshot.IsRemote,
            snapshot.IsTrustedAsset,
            snapshot.VaryHeaders,
            snapshot.VaryDigest,
            snapshot.ResponseDate?.UtcTicks,
            snapshot.ResponseAge?.Ticks,
            snapshot.Expires?.UtcTicks,
            snapshot.MaxAge?.Ticks,
            snapshot.IsPrivate,
            snapshot.SecurityPolicyVersion);

    internal ImageSourceSnapshot ToSnapshot() =>
        new(
            new ImageSourceKey(SourceKey, PartitionHash),
            new ImageSourceVersion(SourceVersion),
            new ImageContentId(ContentId),
            CommitGeneration,
            new DateTimeOffset(StoredAtUtcTicks, TimeSpan.Zero),
            FreshUntilUtcTicks is { } fresh ? new DateTimeOffset(fresh, TimeSpan.Zero) : null,
            ETag,
            LastModifiedUtcTicks is { } modified ? new DateTimeOffset(modified, TimeSpan.Zero) : null,
            NoCache,
            MustRevalidate,
            IsRemote,
            IsTrustedAsset,
            VaryHeaders,
            VaryDigest,
            ResponseDateUtcTicks is { } responseDate ? new DateTimeOffset(responseDate, TimeSpan.Zero) : null,
            ResponseAgeTicks is { } responseAge ? TimeSpan.FromTicks(responseAge) : null,
            ExpiresUtcTicks is { } expires ? new DateTimeOffset(expires, TimeSpan.Zero) : null,
            MaxAgeTicks is { } maxAge ? TimeSpan.FromTicks(maxAge) : null,
            IsPrivate,
            SecurityPolicyVersion);

    internal void Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        writer.Write(FormatRevision);
        writer.Write(PartitionHash);
        writer.Write(SourceKey);
        writer.Write(SourceVersion);
        writer.Write(ContentId);
        writer.Write(CommitGeneration);
        writer.Write(StoredAtUtcTicks);
        WriteNullable(writer, FreshUntilUtcTicks);
        WriteNullable(writer, ETag);
        WriteNullable(writer, LastModifiedUtcTicks);
        writer.Write(NoCache);
        writer.Write(MustRevalidate);
        writer.Write(IsRemote);
        writer.Write(IsTrustedAsset);
        WriteNullable(writer, VaryHeaders);
        WriteNullable(writer, VaryDigest);
        WriteNullable(writer, ResponseDateUtcTicks);
        WriteNullable(writer, ResponseAgeTicks);
        WriteNullable(writer, ExpiresUtcTicks);
        WriteNullable(writer, MaxAgeTicks);
        writer.Write(IsPrivate);
        writer.Write(SecurityPolicyVersion);
    }

    internal static ImageStoredSourceSnapshot Read(Stream stream)
    {
        using var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true);
        if (reader.ReadInt32() != FormatRevision)
        {
            throw new InvalidDataException("Unsupported image source metadata format revision.");
        }
        return new ImageStoredSourceSnapshot(
            reader.ReadString(),
            reader.ReadString(),
            reader.ReadString(),
            reader.ReadString(),
            reader.ReadInt64(),
            reader.ReadInt64(),
            ReadNullableInt64(reader),
            ReadNullableString(reader),
            ReadNullableInt64(reader),
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            reader.ReadBoolean(),
            reader.ReadBoolean(),
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
        if (count is < 0 or > 256)
        {
            throw new InvalidDataException("Image source metadata contains an invalid header count.");
        }
        var result = new string[count];
        for (var index = 0; index < count; index++)
        {
            result[index] = reader.ReadString();
        }
        return result;
    }
}
