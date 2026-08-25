using Avalonia.Controls;
using Avalonia.VisualTree;
using AtomUI.Controls.Tests.ImageLoading;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Tests.Avatar;

public class AvatarImageLoadingTests
{
    static AvatarImageLoadingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Loading_Preserves_Text_Then_Loaded_Image_Takes_Precedence()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var source = ImageLoadSource.FromStream(
            async _ =>
            {
                started.TrySetResult();
                await release.Task;
                return new MemoryStream(ImageControlTestHost.CreatePng(32, 32));
            },
            "avatar",
            "v1");
        var avatar = new global::AtomUI.Controls.Avatar
        {
            Size = 48,
            Text = "AB",
            Icon = new PathIcon(),
            Source = source
        };
        using var host = new ImageControlTestHost(avatar);
        ImageControlTestHost.WaitUntil(() => started.Task.IsCompleted, "avatar request start");

        avatar.IsLoading.ShouldBeTrue();
        avatar.ContentType.ShouldBe(AvatarContentType.Text);

        release.TrySetResult();
        ImageControlTestHost.WaitUntil(
            () => avatar.LoadState is ImageLoadState.Loaded or ImageLoadState.Failed,
            "avatar image terminal state");
        avatar.LoadError.ShouldBeNull();

        avatar.ContentType.ShouldBe(AvatarContentType.Image);
        avatar.LoadedImage.ShouldNotBeNull();
        avatar.GetVisualDescendants()
              .OfType<Image>()
              .Single(control => control.Name == "ImagePresenter")
              .IsVisible.ShouldBeTrue();
    }

    [Fact]
    public void Source_Starts_Loading_After_The_First_Window_Layout()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var source = ImageLoadSource.FromStream(
            async token =>
            {
                started.TrySetResult();
                await release.Task.WaitAsync(token);
                return new MemoryStream("""
                    <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 64 64">
                      <circle cx="32" cy="32" r="32"/>
                    </svg>
                    """u8.ToArray());
            },
            "avatar-first-layout-svg",
            "v1");
        var avatar = new global::AtomUI.Controls.Avatar
        {
            Size = 64,
            Source = source
        };
        var window = new Window
        {
            Width = 200,
            Height = 120,
            Content = avatar
        };

        try
        {
            window.Show();
            ImageControlTestHost.WaitUntil(() => started.Task.IsCompleted, "avatar first-layout request start");

            avatar.IsLoading.ShouldBeTrue();
        }
        finally
        {
            release.TrySetResult();
            window.Close();
        }
    }

    [Fact]
    public void Without_Loaded_Image_Text_Precedes_Icon_And_Icon_Is_Final_Fallback()
    {
        var avatar = new global::AtomUI.Controls.Avatar
        {
            Size = 48,
            Text = "AB",
            Icon = new PathIcon()
        };
        using var host = new ImageControlTestHost(avatar);

        avatar.ContentType.ShouldBe(AvatarContentType.Text);
        avatar.Text = null;
        avatar.ContentType.ShouldBe(AvatarContentType.Icon);
    }
}
