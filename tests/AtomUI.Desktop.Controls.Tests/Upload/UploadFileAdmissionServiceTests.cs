using AtomUI.Controls;
using Avalonia.Platform.Storage;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadFileAdmissionServiceTests
{
    [Fact]
    public async Task Null_Or_Empty_File_Types_Accept_The_File()
    {
        var file = File("readme.txt");

        var nullResult = await EvaluateAsync(file, null);
        var emptyResult = await EvaluateAsync(file, []);

        nullResult.IsAccepted.ShouldBeTrue();
        emptyResult.IsAccepted.ShouldBeTrue();
    }

    [Theory]
    [InlineData("image.PNG", "*.png")]
    [InlineData("archive.tar.gz", "*.tar.gz")]
    [InlineData("readme", "*.*")]
    public async Task Patterns_Match_File_Names_Case_Insensitively(string fileName, string pattern)
    {
        var result = await EvaluateAsync(
            File(fileName),
            [new FilePickerFileType("allowed") { Patterns = [pattern] }]);

        result.IsAccepted.ShouldBeTrue();
    }

    [Fact]
    public async Task Any_File_Type_Or_Pattern_Can_Accept_The_File()
    {
        var result = await EvaluateAsync(
            File("image.png"),
            [
                new FilePickerFileType("text") { Patterns = ["*.txt"] },
                new FilePickerFileType("images") { Patterns = ["*.jpg", "*.png"] }
            ]);

        result.IsAccepted.ShouldBeTrue();
    }

    [Theory]
    [InlineData("image/png", "image/png")]
    [InlineData("image/png", "image/*")]
    [InlineData("APPLICATION/PDF", "application/pdf")]
    public async Task Reliable_Content_Type_Supports_Exact_And_Wildcard_Mime_Matches(
        string contentType,
        string allowedMime)
    {
        var result = await EvaluateAsync(
            File("content.bin", contentType),
            [new FilePickerFileType("mime") { MimeTypes = [allowedMime] }]);

        result.IsAccepted.ShouldBeTrue();
    }

    [Fact]
    public async Task Missing_Content_Type_Does_Not_Match_Mime_Only_Rules()
    {
        var result = await EvaluateAsync(
            File("content.bin"),
            [new FilePickerFileType("mime") { MimeTypes = ["application/octet-stream"] }]);

        result.IsAccepted.ShouldBeFalse();
        result.Rejection.ShouldNotBeNull();
        result.Rejection.Reason.ShouldBe(UploadRejectionReason.FileTypeNotAllowed);
    }

    [Fact]
    public async Task Apple_Uti_Only_Rules_Do_Not_Match_Without_Reliable_Uti_Metadata()
    {
        var result = await EvaluateAsync(
            File("image.png"),
            [new FilePickerFileType("uti") { AppleUniformTypeIdentifiers = ["public.png"] }]);

        result.IsAccepted.ShouldBeFalse();
    }

    [Fact]
    public async Task Admission_Policy_Rejection_Preserves_Code_And_Message()
    {
        var policy = new DelegateAdmissionPolicy((_, _) =>
            ValueTask.FromResult(UploadAdmissionDecision.Reject("virus", "Rejected by scanner.")));

        var result = await UploadFileAdmissionService.EvaluateAsync(
            Guid.NewGuid(),
            UploadInputSource.DragDrop,
            File("unsafe.exe"),
            [],
            policy,
            TestContext.Current.CancellationToken);

        result.IsAccepted.ShouldBeFalse();
        result.Rejection.ShouldNotBeNull();
        result.Rejection.Reason.ShouldBe(UploadRejectionReason.AdmissionRejected);
        result.Rejection.RejectionCode.ShouldBe("virus");
        result.Rejection.Message.ShouldBe("Rejected by scanner.");
    }

    [Fact]
    public async Task Admission_Policy_Receives_Batch_Source_And_File()
    {
        UploadAdmissionContext? observed = null;
        var policy = new DelegateAdmissionPolicy((context, _) =>
        {
            observed = context;
            return ValueTask.FromResult(UploadAdmissionDecision.Accept());
        });
        var batchId = Guid.NewGuid();
        var file = File("safe.txt");

        var result = await UploadFileAdmissionService.EvaluateAsync(
            batchId,
            UploadInputSource.Programmatic,
            file,
            [],
            policy,
            TestContext.Current.CancellationToken);

        result.IsAccepted.ShouldBeTrue();
        observed.ShouldNotBeNull();
        observed.BatchId.ShouldBe(batchId);
        observed.Source.ShouldBe(UploadInputSource.Programmatic);
        observed.File.ShouldBeSameAs(file);
    }

    [Fact]
    public async Task Admission_Policy_Exception_Becomes_A_Per_Item_Rejection()
    {
        var exception = new InvalidOperationException("scanner unavailable");
        var policy = new DelegateAdmissionPolicy((_, _) => ValueTask.FromException<UploadAdmissionDecision>(exception));

        var result = await UploadFileAdmissionService.EvaluateAsync(
            Guid.NewGuid(),
            UploadInputSource.FilePicker,
            File("file.txt"),
            [],
            policy,
            TestContext.Current.CancellationToken);

        result.IsAccepted.ShouldBeFalse();
        result.Rejection.ShouldNotBeNull();
        result.Rejection.Reason.ShouldBe(UploadRejectionReason.AdmissionPolicyFailed);
        result.Rejection.RejectionCode.ShouldBeNull();
        result.Rejection.Message.ShouldBeNull();
    }

    [Fact]
    public async Task Admission_Policy_Cancellation_Propagates()
    {
        var policy = new DelegateAdmissionPolicy((_, cancellationToken) =>
            ValueTask.FromCanceled<UploadAdmissionDecision>(cancellationToken));
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await UploadFileAdmissionService.EvaluateAsync(
                Guid.NewGuid(),
                UploadInputSource.DirectoryPicker,
                File("file.txt"),
                [],
                policy,
                cancellationTokenSource.Token));
    }

    private static Task<UploadFileAdmissionResult> EvaluateAsync(
        UploadFileInfo file,
        IReadOnlyList<FilePickerFileType>? allowedFileTypes)
    {
        var rules = allowedFileTypes?.Select(fileType => new UploadFileTypeRule(
            fileType.Patterns?.ToArray() ?? [],
            fileType.MimeTypes?.ToArray() ?? [])).ToArray() ?? [];
        return UploadFileAdmissionService.EvaluateAsync(
            Guid.NewGuid(),
            UploadInputSource.Programmatic,
            file,
            rules,
            null,
            TestContext.Current.CancellationToken).AsTask();
    }

    private static UploadFileInfo File(string name, string? contentType = null)
    {
        return new UploadFileInfo(
            name,
            new UploadTestFileSource(),
            new Uri($"file:///tmp/{name}"),
            3,
            contentType);
    }

    private sealed class DelegateAdmissionPolicy : IUploadAdmissionPolicy
    {
        private readonly Func<UploadAdmissionContext, CancellationToken, ValueTask<UploadAdmissionDecision>> _evaluate;

        internal DelegateAdmissionPolicy(
            Func<UploadAdmissionContext, CancellationToken, ValueTask<UploadAdmissionDecision>> evaluate)
        {
            _evaluate = evaluate;
        }

        public ValueTask<UploadAdmissionDecision> EvaluateAsync(
            UploadAdmissionContext context,
            CancellationToken cancellationToken = default)
        {
            return _evaluate(context, cancellationToken);
        }
    }
}
