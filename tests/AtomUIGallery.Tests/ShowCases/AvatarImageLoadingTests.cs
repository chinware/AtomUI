using System.Diagnostics;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class AvatarImageLoadingTests
{
    static AvatarImageLoadingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData("avares://AtomUIGallery/Assets/AvatarShowCase/AntDesign.svg")]
    [InlineData("avares://AtomUIGallery/Assets/AvatarShowCase/PeopleAvatar1.svg")]
    [InlineData("avares://AtomUIGallery/Assets/AvatarShowCase/PeopleAvatar4.png")]
    public void Gallery_Avatar_Assets_Load_Through_The_Shared_Pipeline(string source)
    {
        var avatar = new Avatar
        {
            Size = 64,
            Source = ImageSource.Parse(source)
        };
        var window = new Window
        {
            Width = 100,
            Height = 100,
            Content = avatar
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            avatar.Measure(new Size(64, 64));
            avatar.Arrange(new Rect(0, 0, 64, 64));
            WaitUntil(
                () => avatar.LoadState is ImageLoadState.Loaded or ImageLoadState.Failed,
                $"avatar source '{source}' to reach a terminal state",
                () =>
                {
                    var snapshot = Avalonia.Application.Current!.GetImageLoader().Snapshot;
                    return $"state={avatar.LoadState}, progress={avatar.LoadProgress}, " +
                           $"reads={snapshot.ActiveReads}/{snapshot.QueuedReads}, " +
                           $"decodes={snapshot.ActiveDecodes}/{snapshot.QueuedDecodes}";
                });

            avatar.LoadState.ShouldBe(
                ImageLoadState.Loaded,
                avatar.LoadError is null
                    ? null
                    : $"{avatar.LoadError.Code}: {avatar.LoadError.Message}");
            avatar.IsLoaded.ShouldBeTrue();
            var imagePresenter = avatar.GetVisualDescendants()
                                       .OfType<Image>()
                                       .Single(control => control.Name == "ImagePresenter");
            imagePresenter.Source.ShouldNotBeNull();
            imagePresenter.IsVisible.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    private static void WaitUntil(
        Func<bool> predicate,
        string description,
        Func<string> describeState)
    {
        var timeout = Stopwatch.StartNew();
        while (!predicate())
        {
            Dispatcher.UIThread.RunJobs();
            if (timeout.Elapsed >= TimeSpan.FromSeconds(5))
            {
                throw new TimeoutException($"Timed out waiting for {description}: {describeState()}.");
            }
            Thread.Yield();
        }
        Dispatcher.UIThread.RunJobs();
    }
}
