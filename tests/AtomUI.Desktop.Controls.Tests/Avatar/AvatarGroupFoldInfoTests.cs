using Avalonia.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;

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
                new AtomUI.Desktop.Controls.Avatar { Text = "A" },
                new AtomUI.Desktop.Controls.Avatar { Text = "B" },
                new AtomUI.Desktop.Controls.Avatar { Text = "C" },
                new AtomUI.Desktop.Controls.Avatar { Text = "D" }
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
}
