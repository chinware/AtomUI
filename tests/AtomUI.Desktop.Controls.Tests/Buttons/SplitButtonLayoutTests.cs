using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Buttons;

public class SplitButtonLayoutTests
{
    static SplitButtonLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SplitButton_Secondary_Button_Overlaps_Primary_Border_By_One_Border_Thickness()
    {
        var splitButton = new SplitButton
        {
            Content = "Hover me"
        };

        ShowInWindow(splitButton, () =>
        {
            var primaryButton   = FindButtonPart(splitButton, "PART_PrimaryButton");
            var secondaryButton = FindButtonPart(splitButton, "PART_SecondaryButton");
            var sharedBorderOverlap = primaryButton.Bounds.Right - secondaryButton.Bounds.Left;

            sharedBorderOverlap.ShouldBe(secondaryButton.BorderThickness.Left, 0.001);
        });
    }

    [Fact]
    public void SplitButton_Secondary_Button_Keeps_Auto_Width_When_Control_Width_Changes()
    {
        var splitButton = new SplitButton
        {
            Content = "Hover me"
        };

        ShowInWindow(splitButton, () =>
        {
            var primaryButton   = FindButtonPart(splitButton, "PART_PrimaryButton");
            var secondaryButton = FindButtonPart(splitButton, "PART_SecondaryButton");
            var secondaryWidth  = secondaryButton.Bounds.Width;
            var secondaryRightOffset = splitButton.Bounds.Width - secondaryButton.Bounds.Right;

            splitButton.Width = splitButton.Bounds.Width + 120;
            Dispatcher.UIThread.RunJobs();

            secondaryButton.Bounds.Width.ShouldBe(secondaryWidth, 0.001);
            (splitButton.Bounds.Width - secondaryButton.Bounds.Right).ShouldBe(secondaryRightOffset, 0.001);

            var sharedBorderOverlap = primaryButton.Bounds.Right - secondaryButton.Bounds.Left;
            sharedBorderOverlap.ShouldBe(secondaryButton.BorderThickness.Left, 0.001);
        });
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SplitButton_Child_Bounds_Remain_Stable_When_Arrange_Allocation_Changes(bool isPrimaryButtonType)
    {
        var splitButton = new SplitButton
        {
            Content             = "Hover me",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Left,
            IsPrimaryButtonType = isPrimaryButtonType
        };

        ShowInWindow(splitButton, () =>
        {
            var primaryButton          = FindButtonPart(splitButton, "PART_PrimaryButton");
            var secondaryButton        = FindButtonPart(splitButton, "PART_SecondaryButton");
            var initialPrimaryBounds   = primaryButton.Bounds;
            var initialSecondaryBounds = secondaryButton.Bounds;
            var controlSize            = splitButton.Bounds.Size;

            for (var pass = 1; pass <= 4; pass++)
            {
                splitButton.Arrange(new Rect(0, 0, controlSize.Width + pass * 40, controlSize.Height));

                primaryButton.Bounds.ShouldBe(initialPrimaryBounds);
                secondaryButton.Bounds.ShouldBe(initialSecondaryBounds);
            }
        });
    }

    [Fact]
    public void Primary_SplitButton_Separator_Uses_Render_Scale_Aware_Thickness()
    {
        var splitButton = new SplitButton
        {
            Content             = "Hover me",
            IsPrimaryButtonType = true,
            BorderThickness     = new Thickness(1)
        };

        ShowInWindow(splitButton, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            var primaryButton   = FindButtonPart(splitButton, "PART_PrimaryButton");
            var secondaryButton = FindButtonPart(splitButton, "PART_SecondaryButton");
            var separatorGap    = secondaryButton.Bounds.Left - primaryButton.Bounds.Right;

            separatorGap.ShouldBe(2d / 3d, 0.001);
        });
    }

    private static Button FindButtonPart(SplitButton splitButton, string name)
    {
        return splitButton.GetVisualDescendants()
                          .OfType<Button>()
                          .Single(button => button.Name == name);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        ShowInWindow(content, _ => assertion());
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 220,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }
}
