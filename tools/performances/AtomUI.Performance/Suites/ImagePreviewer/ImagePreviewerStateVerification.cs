using System.Collections;
using System.Reflection;
using Avalonia.Controls;
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
        VerifyImagePreviewerOpenItemsSourceReplacement(failures);
        VerifyImageGroupPreviewerMaterialization(failures);
        VerifyImagePreviewerCoverReplacement(failures);

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

        previewer.ItemsSource = null;
        RefreshLayout(realized.Window);
        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == 0,
            "Clearing ItemsSource should clear effective dialog sources.",
            failures);
        Expect(GetImagePreviewerEffectiveCoverImage(previewer) == null,
            "Clearing ItemsSource should release the effective cover image when no custom cover or fallback exists.",
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
        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == ImagePreviewerThreeImages.Length,
            "Opening ImagePreviewer should materialize all preview dialog sources.",
            failures);

        previewer.IsOpen = false;
        RefreshLayout(realized.Window);
        Expect(!previewer.IsOpen,
            "Closing ImagePreviewer should set IsOpen=false.",
            failures);
    }

    private static void VerifyImagePreviewerOpenItemsSourceReplacement(ICollection<string> failures)
    {
        var previewer = CreateMultiSourceImagePreviewer();
        using var realized = RealizeControl(previewer);

        previewer.OpenDialog();
        Dispatcher.UIThread.RunJobs();
        previewer.ItemsSource = ImagePreviewerTwoImages;
        RefreshLayout(realized.Window);

        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == ImagePreviewerTwoImages.Length,
            "Replacing ItemsSource while ImagePreviewer is open should keep dialog sources materialized.",
            failures);

        previewer.IsOpen = false;
        RefreshLayout(realized.Window);
    }

    private static void VerifyImageGroupPreviewerMaterialization(ICollection<string> failures)
    {
        AtomImageGroupPreviewer groupPreviewer = CreateImageGroupPreviewer();
        using var realized = RealizeControl(groupPreviewer);

        Expect(GetImagePreviewerEffectiveSourceCount(groupPreviewer) == ImagePreviewerTwoImages.Length,
            "ImageGroupPreviewer closed state should materialize all visible cover images.",
            failures);
        Expect(CountVisualByTypeName(groupPreviewer, "ImagePreviewerCover") == ImagePreviewerTwoImages.Length,
            "ImageGroupPreviewer should create one cover visual per source.",
            failures);
    }

    private static void VerifyImagePreviewerCoverReplacement(ICollection<string> failures)
    {
        AtomImagePreviewer previewer = CreateCustomCoverImagePreviewer();
        using var realized = RealizeControl(previewer);

        var firstCover = GetImagePreviewerEffectiveCoverImage(previewer);
        Expect(firstCover != null,
            "Custom cover ImagePreviewer should materialize its custom cover image.",
            failures);
        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == 0,
            "Custom cover ImagePreviewer should not materialize dialog sources while closed.",
            failures);

        previewer.CoverImageSrc = ImagePreviewerFallbackImage;
        RefreshLayout(realized.Window);
        var secondCover = GetImagePreviewerEffectiveCoverImage(previewer);
        Expect(secondCover != null && !ReferenceEquals(firstCover, secondCover),
            "Replacing CoverImageSrc should replace the effective cover image.",
            failures);

        previewer.CoverImageSrc = null;
        RefreshLayout(realized.Window);
        Expect(GetImagePreviewerEffectiveCoverImage(previewer) != null,
            "Clearing CoverImageSrc should fall back to the first ItemsSource image as the cover.",
            failures);
        Expect(GetImagePreviewerEffectiveSourceCount(previewer) == 0,
            "Clearing CoverImageSrc should not materialize dialog sources while closed.",
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
