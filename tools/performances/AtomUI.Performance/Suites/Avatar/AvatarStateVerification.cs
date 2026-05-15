using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunAvatarStateVerification()
    {
        var failures = new List<string>();
        VerifyAvatarCreatesOnlyActivePresenter(failures);
        VerifyAvatarContentTypeSwitching(failures);
        VerifyAvatarGroupFoldLifecycle(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("Avatar state verification passed.");
            return true;
        }

        Console.Error.WriteLine("Avatar state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyAvatarCreatesOnlyActivePresenter(ICollection<string> failures)
    {
        VerifyAvatarPresenterShape(
            new Avatar { Icon = new UserOutlined() },
            "Icon Avatar",
            iconPresenter: 1,
            image: 0,
            svg: 0,
            textBlock: 0,
            failures);
        VerifyAvatarPresenterShape(
            new Avatar { Text = "U" },
            "Text Avatar",
            iconPresenter: 0,
            image: 0,
            svg: 0,
            textBlock: 1,
            failures);
        VerifyAvatarPresenterShape(
            new Avatar { Src = GetAvatarSvgPath() },
            "Svg Avatar",
            iconPresenter: 0,
            image: 0,
            svg: 1,
            textBlock: 0,
            failures);
        VerifyAvatarPresenterShape(
            new Avatar { BitmapSrc = AvatarBitmap.Value },
            "Bitmap Avatar",
            iconPresenter: 0,
            image: 1,
            svg: 0,
            textBlock: 0,
            failures);
    }

    private static void VerifyAvatarPresenterShape(Avatar avatar,
                                                   string label,
                                                   int iconPresenter,
                                                   int image,
                                                   int svg,
                                                   int textBlock,
                                                   ICollection<string> failures)
    {
        using var realized = RealizeControl(avatar);
        Expect(CountVisualByTypeName(avatar, "IconPresenter") == iconPresenter,
            $"{label} should create {iconPresenter} IconPresenter.",
            failures);
        Expect(CountVisualByType<Image>(avatar) == image,
            $"{label} should create {image} Image presenter.",
            failures);
        Expect(CountVisualByTypeName(avatar, "Svg") == svg,
            $"{label} should create {svg} Svg presenter.",
            failures);
        Expect(CountVisualByTypeName(avatar, "TextBlock") == textBlock,
            $"{label} should create {textBlock} TextBlock presenter.",
            failures);
    }

    private static void VerifyAvatarContentTypeSwitching(ICollection<string> failures)
    {
        var avatar = new Avatar
        {
            Icon = new UserOutlined()
        };

        using var realized = RealizeControl(avatar);
        var firstIconPresenter = FindVisualByName<IconPresenter>(avatar, "IconPresenter");
        Expect(firstIconPresenter != null,
            "Avatar should start with IconPresenter for Icon content.",
            failures);

        avatar.SetCurrentValue(Avatar.TextProperty, "USER");
        RefreshLayout(realized.Window);
        Expect(firstIconPresenter?.GetVisualParent() == null,
            "Avatar should detach IconPresenter when switching to Text.",
            failures);
        Expect(CountVisualByTypeName(avatar, "TextBlock") == 1 &&
               CountVisualByTypeName(avatar, "IconPresenter") == 0,
            "Avatar should keep only TextBlock after switching to Text.",
            failures);

        var textPresenter = FindVisualByName<Avalonia.Controls.TextBlock>(avatar, "PART_TextPresenter");
        avatar.SetCurrentValue(Avatar.SrcProperty, GetAvatarSvgPath());
        RefreshLayout(realized.Window);
        Expect(textPresenter?.GetVisualParent() == null,
            "Avatar should detach TextBlock when switching to Svg.",
            failures);
        Expect(CountVisualByTypeName(avatar, "Svg") == 1 &&
               CountVisualByTypeName(avatar, "TextBlock") == 0,
            "Avatar should keep only Svg after switching to Src.",
            failures);

        var svgPresenter = FindVisualByTypeName(avatar, "Svg", "SvgPresenter");
        avatar.SetCurrentValue(Avatar.SrcProperty, null);
        avatar.SetCurrentValue(Avatar.BitmapSrcProperty, AvatarBitmap.Value);
        RefreshLayout(realized.Window);
        Expect(svgPresenter?.GetVisualParent() == null,
            "Avatar should detach Svg when switching to BitmapSrc.",
            failures);
        Expect(CountVisualByType<Image>(avatar) == 1 &&
               CountVisualByTypeName(avatar, "Svg") == 0,
            "Avatar should keep only Image after switching to BitmapSrc.",
            failures);
    }

    private static void VerifyAvatarGroupFoldLifecycle(ICollection<string> failures)
    {
        var noFoldGroup = CreateAvatarGroup(maxDisplayCount: null);
        using (var realized = RealizeControl(noFoldGroup))
        {
            Expect(CountVisualByTypeName(noFoldGroup, "FlyoutHost") == 0,
                "AvatarGroup without folding should not create FlyoutHost visuals.",
                failures);
            Expect(GetPrivateField(noFoldGroup, "AtomUI.Desktop.Controls.AvatarGroup", "_foldCountFlyout") == null,
                "AvatarGroup without folding should keep _foldCountFlyout null.",
                failures);
            Expect(CountVisualByTypeName(noFoldGroup, "Avatar") == 4,
                "AvatarGroup without folding should show all four avatars.",
                failures);
        }

        var foldGroup = CreateAvatarGroup(maxDisplayCount: 2);
        using (var realized = RealizeControl(foldGroup))
        {
            Expect(CountVisualByTypeName(foldGroup, "FlyoutHost") == 1,
                "Folded AvatarGroup should create one FlyoutHost.",
                failures);
            Expect(CountVisualByTypeName(foldGroup, "Avatar") == 3,
                "Folded AvatarGroup should show two children plus one fold avatar.",
                failures);

            foldGroup.SetCurrentValue(AvatarGroup.MaxDisplayCountProperty, null);
            RefreshLayout(realized.Window);
            Expect(CountVisualByTypeName(foldGroup, "FlyoutHost") == 0,
                "AvatarGroup should remove FlyoutHost when folding is disabled.",
                failures);
            Expect(GetPrivateField(foldGroup, "AtomUI.Desktop.Controls.AvatarGroup", "_foldCountFlyout") == null,
                "AvatarGroup should release _foldCountFlyout when folding is disabled.",
                failures);
            Expect(CountVisualByTypeName(foldGroup, "Avatar") == 4,
                "AvatarGroup should restore all child avatars when folding is disabled.",
                failures);

            foldGroup.SetCurrentValue(AvatarGroup.MaxDisplayCountProperty, 2);
            RefreshLayout(realized.Window);
            Expect(CountVisualByTypeName(foldGroup, "FlyoutHost") == 1,
                "AvatarGroup should recreate FlyoutHost when folding is re-enabled.",
                failures);
            Expect(CountVisualByTypeName(foldGroup, "Avatar") == 3,
                "AvatarGroup should fold back to visible children plus fold avatar.",
                failures);
        }

        Expect(GetPrivateField(foldGroup, "AtomUI.Desktop.Controls.AvatarGroup", "_foldCountFlyout") == null,
            "Detached folded AvatarGroup should release _foldCountFlyout.",
            failures);
    }

    private static int CountVisualByType<T>(Control root)
        where T : Control
    {
        return root.GetSelfAndVisualDescendants().OfType<T>().Count();
    }

    private static int CountVisualByTypeName(Control root, string typeName)
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<Control>()
                   .Count(control => control.GetType().Name == typeName);
    }
}
