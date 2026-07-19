using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using Avalonia;
using Avalonia.Styling;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeFollowSystemTests
{
    [Theory]
    [InlineData(ThemeAppearance.Light)]
    [InlineData(ThemeAppearance.Dark)]
    public void Initial_System_Appearance_Commits_An_Explicit_Matching_Variant(
        ThemeAppearance initialAppearance)
    {
        HeadlessTestApp.Run(() =>
        {
            var application = Application.Current!;
            var light = new ThemeRequest(
                IThemeManager.DEFAULT_THEME_ID,
                new ThemeConfigBuilder().WithAlgorithms("Default").Build(),
                ThemeTransitionReason.FollowSystem);
            var dark = new ThemeRequest(
                IThemeManager.DEFAULT_THEME_ID,
                new ThemeConfigBuilder().WithAlgorithms("Default", "Dark").Build(),
                ThemeTransitionReason.FollowSystem);
            var builder = new ThemeManagerBuilder();
            builder.WithFollowSystemThemes(light, dark);
            var manager = builder.Build();

            manager.InitializeApplication(application, initialAppearance);

            manager.CurrentTheme!.Appearance.ShouldBe(initialAppearance);
            application.RequestedThemeVariant.ShouldBe(
                initialAppearance == ThemeAppearance.Dark
                    ? ThemeVariant.Dark
                    : ThemeVariant.Light);
            application.RequestedThemeVariant.ShouldNotBe(ThemeVariant.Default);
        });
    }

    [Fact]
    public async Task Rapid_System_Appearance_Changes_Commit_Only_Explicit_Matching_Variants()
    {
        await HeadlessTestApp.RunAsync(async () =>
        {
            var application = Application.Current!;
            var light = new ThemeRequest(
                IThemeManager.DEFAULT_THEME_ID,
                new ThemeConfigBuilder().WithAlgorithms("Default").Build(),
                ThemeTransitionReason.FollowSystem);
            var dark = new ThemeRequest(
                IThemeManager.DEFAULT_THEME_ID,
                new ThemeConfigBuilder().WithAlgorithms("Default", "Dark").Build(),
                ThemeTransitionReason.FollowSystem);
            var builder = new ThemeManagerBuilder();
            builder.WithFollowSystemThemes(light, dark);
            var manager = builder.Build();
            manager.InitializeApplication(application, ThemeAppearance.Light);
            var observed = new List<(ThemeAppearance Appearance, ThemeVariant? Variant)>();
            manager.ThemeChanged += (_, args) => observed.Add((
                args.State.Appearance,
                application.RequestedThemeVariant));

            var darkTask = manager.ApplySystemAppearanceAsync(
                ThemeAppearance.Dark,
                TestContext.Current.CancellationToken);
            var lightTask = manager.ApplySystemAppearanceAsync(
                ThemeAppearance.Light,
                TestContext.Current.CancellationToken);
            await Task.WhenAll(darkTask, lightTask);

            manager.CurrentTheme!.Appearance.ShouldBe(ThemeAppearance.Light);
            application.RequestedThemeVariant.ShouldBe(ThemeVariant.Light);
            application.RequestedThemeVariant.ShouldNotBe(ThemeVariant.Default);
            observed.ShouldAllBe(item =>
                item.Variant == (item.Appearance == ThemeAppearance.Dark
                    ? ThemeVariant.Dark
                    : ThemeVariant.Light));
        });
    }
}
