using System.Collections;
using System.Reflection;
using Avalonia.Threading;
using AtomImagePreviewer = AtomUI.Desktop.Controls.ImagePreviewer;
using AtomImageGroupPreviewer = AtomUI.Desktop.Controls.ImageGroupPreviewer;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunImagePreviewerStateVerification()
    {
        var failures = new List<string>();
        VerifyImagePreviewerClosedStateMaterialization(failures);
        VerifyImagePreviewerDialogMaterialization(failures);
        VerifyImagePreviewerOpenSourcesReplacement(failures);
        VerifyImageGroupPreviewerMaterialization(failures);
        VerifyImagePreviewerSourceReplacement(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("ImagePreviewer state verification passed.");
            return true;
        }

        Console.Error.WriteLine("ImagePreviewer state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyImagePreviewerClosedStateMaterialization(ICollection<string> failures)
    {
        var previewer = CreateMultiSourceImagePreviewer();
        using var realized = RealizeControl(previewer);

        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == 0,
            "Single ImagePreviewer closed state should not materialize dialog source list.",
            failures);
        Expect(GetImagePreviewerEffectiveCoverImage(previewer) != null,
            "Single ImagePreviewer closed state should materialize only the visible cover image.",
            failures);
        Expect(CountVisualByTypeName(previewer, "ImagePreviewerCover") == 1,
            "Single ImagePreviewer should keep one cover visual.",
            failures);

        previewer.Sources = null;
        RefreshLayout(realized.Window);
        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == 0,
            "Clearing Sources should clear effective dialog sources.",
            failures);
        Expect(GetImagePreviewerEffectiveCoverImage(previewer) == null,
            "Clearing Sources should release the effective cover image when no source or fallback exists.",
            failures);
    }

    private static void VerifyImagePreviewerDialogMaterialization(ICollection<string> failures)
    {
        var previewer = CreateMultiSourceImagePreviewer();
        using var realized = RealizeControl(previewer);

        previewer.OpenDialog();
        Dispatcher.UIThread.RunJobs();

        Expect(previewer.IsOpen,
            "OpenDialog should set IsOpen=true.",
            failures);
        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == ImagePreviewerThreeImages.Count,
            "Opening ImagePreviewer should materialize all preview dialog sources.",
            failures);

        previewer.IsOpen = false;
        RefreshLayout(realized.Window);
        Expect(!previewer.IsOpen,
            "Closing ImagePreviewer should set IsOpen=false.",
            failures);
    }

    private static void VerifyImagePreviewerOpenSourcesReplacement(ICollection<string> failures)
    {
        var previewer = CreateMultiSourceImagePreviewer();
        using var realized = RealizeControl(previewer);

        previewer.OpenDialog();
        Dispatcher.UIThread.RunJobs();
        previewer.Sources = ImagePreviewerTwoImages;
        RefreshLayout(realized.Window);

        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == ImagePreviewerTwoImages.Count,
            "Replacing Sources while ImagePreviewer is open should keep dialog sources materialized.",
            failures);

        previewer.IsOpen = false;
        RefreshLayout(realized.Window);
    }

    private static void VerifyImageGroupPreviewerMaterialization(ICollection<string> failures)
    {
        AtomImageGroupPreviewer groupPreviewer = CreateImageGroupPreviewer();
        using var realized = RealizeControl(groupPreviewer);

        Expect(GetImagePreviewerEffectiveSourceCount(groupPreviewer) == ImagePreviewerTwoImages.Count,
            "ImageGroupPreviewer closed state should materialize all visible cover images.",
            failures);
        Expect(CountVisualByTypeName(groupPreviewer, "ImagePreviewerCover") == ImagePreviewerTwoImages.Count,
            "ImageGroupPreviewer should create one cover visual per source.",
            failures);
    }

    private static void VerifyImagePreviewerSourceReplacement(ICollection<string> failures)
    {
        AtomImagePreviewer previewer = CreateSingleSourceImagePreviewer();
        using var realized = RealizeControl(previewer);

        var firstCover = GetImagePreviewerEffectiveCoverImage(previewer);
        Expect(firstCover != null,
            "Single-source ImagePreviewer should materialize its source cover image.",
            failures);
        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == 0,
            "Single-source ImagePreviewer should not materialize dialog sources while closed.",
            failures);

        previewer.Source = ImagePreviewerFallbackImage;
        RefreshLayout(realized.Window);
        var secondCover = GetImagePreviewerEffectiveCoverImage(previewer);
        Expect(secondCover != null && !ReferenceEquals(firstCover, secondCover),
            "Replacing Source should replace the effective cover image.",
            failures);

        previewer.Source = null;
        RefreshLayout(realized.Window);
        Expect(GetImagePreviewerEffectiveCoverImage(previewer) == null,
            "Clearing Source should release the effective cover image when no fallback is configured.",
            failures);
        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == 0,
            "Clearing Source should not materialize dialog sources while closed.",
            failures);
    }

    private static int GetImagePreviewerEffectiveSourceCount(object previewer)
    {
        return GetNonPublicProperty(previewer, "AtomUI.Desktop.Controls.AbstractImagePreviewer", "EffectiveSources") is ICollection sources
            ? sources.Count
            : 0;
    }

    private static object? GetImagePreviewerEffectiveCoverImage(AtomImagePreviewer previewer)
    {
        return GetNonPublicProperty(previewer, "AtomUI.Desktop.Controls.ImagePreviewer", "EffectiveCoverImage");
    }

    private static object? GetNonPublicProperty(object target, string declaringTypeName, string propertyName)
    {
        var type = target.GetType();
        while (type is not null)
        {
            if (type.FullName == declaringTypeName)
            {
                return type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(target);
            }

            type = type.BaseType;
        }

        return null;
    }
}
