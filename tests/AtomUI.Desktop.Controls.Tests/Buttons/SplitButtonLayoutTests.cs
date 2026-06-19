using System;
using System.Linq;
using Avalonia.Controls;
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

    private static Button FindButtonPart(SplitButton splitButton, string name)
    {
        return splitButton.GetVisualDescendants()
                          .OfType<Button>()
                          .Single(button => button.Name == name);
    }

    private static void ShowInWindow(Control content, Action assertion)
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
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
