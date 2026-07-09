using System.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

using DesktopAvatar = AtomUI.Desktop.Controls.Avatar;

namespace AtomUI.Desktop.Controls.Tests.Avatar;

public class AvatarGroupFoldInfoTests
{
    static AvatarGroupFoldInfoTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void AvatarGroup_With_Folded_Avatars_Can_Reattach()
    {
        var group = new AvatarGroup
        {
            MaxDisplayCount = 2,
            Children =
            {
                new DesktopAvatar { Text = "A" },
                new DesktopAvatar { Text = "B" },
                new DesktopAvatar { Text = "C" },
                new DesktopAvatar { Text = "D" }
            }
        };

        var window = new Avalonia.Controls.Window
        {
            Width   = 240,
            Height  = 160,
            Content = group
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            Should.NotThrow(() =>
            {
                window.Content = group;
                Dispatcher.UIThread.RunJobs();
            });
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void AvatarGroup_MaxDisplayCount_Change_Rebuilds_Visible_And_Folded_Children()
    {
        var group = CreateFoldedGroup();

        ShowInWindow(group, () =>
        {
            GetPresentedAvatarTexts(group).ShouldBe(["A", "B"]);
            GetPresentedFoldHostCount(group).ShouldBe(1);

            group.MaxDisplayCount = 3;
            Dispatcher.UIThread.RunJobs();

            GetPresentedAvatarTexts(group).ShouldBe(["A", "B", "C"]);
            GetPresentedFoldHostCount(group).ShouldBe(1);
        });
    }

    [Fact]
    public void AvatarGroup_Without_Folding_Does_Not_Create_Fold_Host()
    {
        var group = new AvatarGroup
        {
            Children =
            {
                new DesktopAvatar { Text = "A" },
                new DesktopAvatar { Text = "B" },
                new DesktopAvatar { Text = "C" },
                new DesktopAvatar { Text = "D" }
            }
        };

        ShowInWindow(group, () =>
        {
            GetPresentedAvatarTexts(group).ShouldBe(["A", "B", "C", "D"]);
            GetPresentedFoldHostCount(group).ShouldBe(0);
        });
    }

    [Fact]
    public void AvatarGroup_Disabling_Fold_Restores_All_Children_And_Releases_Fold_Host()
    {
        var group = CreateFoldedGroup();

        ShowInWindow(group, () =>
        {
            GetPresentedFoldHostCount(group).ShouldBe(1);

            group.MaxDisplayCount = null;
            Dispatcher.UIThread.RunJobs();

            GetPresentedAvatarTexts(group).ShouldBe(["A", "B", "C", "D"]);
            GetPresentedFoldHostCount(group).ShouldBe(0);
        });
    }

    [Fact]
    public void AvatarGroup_Detach_Releases_Fold_Host()
    {
        var group = CreateFoldedGroup();
        var window = new Avalonia.Controls.Window
        {
            Width   = 240,
            Height  = 160,
            Content = group
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            GetPresentedFoldHostCount(group).ShouldBe(1);

            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            GetPresentedFoldHostCount(group).ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void AvatarGroup_Move_Across_Fold_Boundary_Rebuilds_Visible_Order()
    {
        var group = CreateFoldedGroup();

        ShowInWindow(group, () =>
        {
            group.Children.Move(2, 0);
            Dispatcher.UIThread.RunJobs();

            GetPresentedAvatarTexts(group).ShouldBe(["C", "A"]);
            GetPresentedFoldHostCount(group).ShouldBe(1);
        });
    }

    [Fact]
    public void AvatarGroup_Replace_Avatar_Inherits_Group_Size_And_Shape()
    {
        var group = new AvatarGroup
        {
            SizeType = CustomizableSizeType.Large,
            Shape    = AvatarShape.Square,
            Children =
            {
                new DesktopAvatar { Text = "A" },
                new DesktopAvatar { Text = "B" }
            }
        };
        var replacement = new DesktopAvatar { Text = "R" };

        ShowInWindow(group, () =>
        {
            group.Children[0] = replacement;
            Dispatcher.UIThread.RunJobs();

            replacement.SizeType.ShouldBe(CustomizableSizeType.Large);
            replacement.Shape.ShouldBe(AvatarShape.Square);
        });
    }

    [Fact]
    public void AvatarGroup_Empty_Measure_Does_Not_Report_Overlap_Width()
    {
        var group = new AvatarGroup
        {
            GroupOverlapping = 8
        };

        group.Measure(new Size(100, 100));

        group.DesiredSize.Width.ShouldBe(0);
    }

    [Fact]
    public void Avatar_Clear_Size_Restores_SizeType_Before_Custom_Size()
    {
        var avatar = new DesktopAvatar
        {
            SizeType = CustomizableSizeType.Large
        };

        avatar.Size = 64;
        avatar.Size = 80;
        avatar.Size = double.NaN;

        avatar.SizeType.ShouldBe(CustomizableSizeType.Large);
    }

    [Fact]
    public void Avatar_BitmapSrc_Content_Is_Clipped_By_Circle_Shape()
    {
        using var bitmap = new RenderTargetBitmap(new PixelSize(4, 4), new Vector(96, 96));
        var avatar = new DesktopAvatar
        {
            Size      = 64,
            Shape     = AvatarShape.Circle,
            BitmapSrc = bitmap
        };

        ShowInWindow(avatar, () =>
        {
            var imagePresenter = avatar.GetVisualDescendants()
                                       .OfType<Image>()
                                       .Single(image => image.Name == "ImagePresenter");

            imagePresenter.IsVisible.ShouldBeTrue();

            var clippingFrame = imagePresenter.GetVisualAncestors()
                                              .OfType<Border>()
                                              .FirstOrDefault(border => border.ClipToBounds);

            clippingFrame.ShouldNotBeNull();
            clippingFrame.CornerRadius.TopLeft.ShouldBe(32);
            clippingFrame.CornerRadius.TopRight.ShouldBe(32);
            clippingFrame.CornerRadius.BottomRight.ShouldBe(32);
            clippingFrame.CornerRadius.BottomLeft.ShouldBe(32);
        });
    }

    private static AvatarGroup CreateFoldedGroup()
    {
        return new AvatarGroup
        {
            MaxDisplayCount = 2,
            Children =
            {
                new DesktopAvatar { Text = "A" },
                new DesktopAvatar { Text = "B" },
                new DesktopAvatar { Text = "C" },
                new DesktopAvatar { Text = "D" }
            }
        };
    }

    private static string?[] GetPresentedAvatarTexts(AvatarGroup group)
    {
        return group.GetLogicalChildren()
                    .OfType<DesktopAvatar>()
                    .Select(avatar => avatar.Text)
                    .ToArray();
    }

    private static int GetPresentedFoldHostCount(AvatarGroup group)
    {
        return group.GetLogicalChildren()
                    .OfType<FlyoutHost>()
                    .Count();
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new Avalonia.Controls.Window
        {
            Width   = 240,
            Height  = 160,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
