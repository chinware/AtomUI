using System.Diagnostics;
using System.IO.Enumeration;
using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed record UploadFileAdmissionResult(
    bool IsAccepted,
    UploadRejectedItem? Rejection = null);

internal static class UploadFileAdmissionService
{
    internal static async ValueTask<UploadFileAdmissionResult> EvaluateAsync(
        Guid batchId,
        UploadInputSource inputSource,
        UploadFileInfo file,
        IReadOnlyList<UploadFileTypeRule> allowedFileTypes,
        IUploadAdmissionPolicy? admissionPolicy,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);
        cancellationToken.ThrowIfCancellationRequested();

        if (!IsFileTypeAllowed(file, allowedFileTypes))
        {
            return new UploadFileAdmissionResult(
                false,
                new UploadRejectedItem(
                    file.Name,
                    file.Path,
                    UploadRejectionReason.FileTypeNotAllowed,
                    message: "The file does not match AllowedFileTypes."));
        }

        if (admissionPolicy is null)
        {
            return new UploadFileAdmissionResult(true);
        }

        try
        {
            var decision = await admissionPolicy.EvaluateAsync(
                new UploadAdmissionContext(batchId, inputSource, file),
                cancellationToken);
            if (decision.IsAccepted)
            {
                return new UploadFileAdmissionResult(true);
            }

            return new UploadFileAdmissionResult(
                false,
                new UploadRejectedItem(
                    file.Name,
                    file.Path,
                    UploadRejectionReason.AdmissionRejected,
                    decision.RejectionCode,
                    decision.Message));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Upload admission policy failed for '{file.Name}': {ex.Message}");
            return new UploadFileAdmissionResult(
                false,
                new UploadRejectedItem(
                    file.Name,
                    file.Path,
                    UploadRejectionReason.AdmissionPolicyFailed));
        }
    }

    private static bool IsFileTypeAllowed(
        UploadFileInfo file,
        IReadOnlyList<UploadFileTypeRule> allowedFileTypes)
    {
        if (allowedFileTypes.Count == 0)
        {
            return true;
        }

        foreach (var fileType in allowedFileTypes)
        {
            if (MatchesAnyPattern(file.Name, fileType.Patterns) ||
                MatchesAnyMimeType(file.ContentType, fileType.MimeTypes))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesAnyPattern(string fileName, IReadOnlyList<string>? patterns)
    {
        if (patterns is null)
        {
            return false;
        }

        foreach (var pattern in patterns)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                continue;
            }

            if (pattern is "*" or "*.*" ||
                FileSystemName.MatchesSimpleExpression(pattern, fileName, ignoreCase: true))
            {
                return true;
            }
        }

        return false;
    }

    private static bool MatchesAnyMimeType(string? contentType, IReadOnlyList<string>? mimeTypes)
    {
        if (string.IsNullOrWhiteSpace(contentType) || mimeTypes is null)
        {
            return false;
        }

        foreach (var mimeType in mimeTypes)
        {
            if (string.IsNullOrWhiteSpace(mimeType))
            {
                continue;
            }

            if (string.Equals(mimeType, contentType, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (mimeType.EndsWith("/*", StringComparison.Ordinal) &&
                contentType.StartsWith(mimeType.AsSpan(0, mimeType.Length - 1), StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
