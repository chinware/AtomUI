using AtomUI.Theme;
using Avalonia;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeManagerTests
{
    [Fact]
    public async Task Unknown_Theme_Fails_Without_Changing_The_Committed_State()
    {
        await HeadlessTestApp.RunAsync(async () =>
        {
            var manager = new ThemeManagerBuilder().Build();
            manager.InitializeApplication(Application.Current!);
            var state = manager.CurrentTheme;
            var snapshot = manager.CurrentSnapshot;

            var result = await manager.ApplyThemeAsync(
                new ThemeRequest("Missing", null, ThemeTransitionReason.UserRequest),
                TestContext.Current.CancellationToken);

            result.Status.ShouldBe(ThemeTransitionStatus.Failed);
            manager.CurrentTheme.ShouldBeSameAs(state);
            manager.CurrentSnapshot.ShouldBeSameAs(snapshot);
        });
    }

    [Fact]
    public void Available_Themes_Are_Read_From_The_Bound_V1_Catalog()
    {
        HeadlessTestApp.Run(() =>
        {
            var manager = new ThemeManagerBuilder().Build();

            manager.InitializeApplication(Application.Current!);

            manager.AvailableThemes.ShouldBe(
            [
                new ThemeInfo(
                    IThemeManager.DEFAULT_THEME_ID,
                    "Daybreak Blue",
                    ThemeAppearance.Light,
                    true,
                    Color.Parse("#1677FF"))
            ]);
        });
    }
}
