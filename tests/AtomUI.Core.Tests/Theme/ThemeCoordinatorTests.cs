using AtomUI.Theme;
using AtomUI.Theme.Transitions;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Shouldly;
using System.Reflection;
using System.Text;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeCoordinatorTests
{
    [Fact]
    public void Dark_Compact_Startup_Commits_Exactly_Once()
    {
        var coordinator = CreateCoordinator();
        var changed = new List<ThemeTransitionEventArgs>();
        coordinator.ThemeChanged += (_, args) => changed.Add(args);

        coordinator.Request(new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            [ThemeAlgorithm.Default, ThemeAlgorithm.Dark, ThemeAlgorithm.Compact],
            ThemeTransitionReason.Startup));

        changed.Count.ShouldBe(1);
        changed[0].OldTheme.ShouldBeNull();
        changed[0].NewTheme.Id.ShouldBe(IThemeManager.DEFAULT_THEME_ID);
        changed[0].NewSnapshot.Algorithms.ShouldBe(
        [
            ThemeAlgorithm.Default,
            ThemeAlgorithm.Dark,
            ThemeAlgorithm.Compact
        ]);
    }

    [Fact]
    public void Identical_Request_Is_A_No_Op_After_First_Commit()
    {
        var coordinator = CreateCoordinator();
        var changed = new List<ThemeTransitionEventArgs>();
        var request = new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            [ThemeAlgorithm.Default, ThemeAlgorithm.Dark],
            ThemeTransitionReason.UserRequest);
        coordinator.ThemeChanged += (_, args) => changed.Add(args);

        coordinator.Request(request);
        coordinator.Request(request);

        changed.Count.ShouldBe(1);
    }

    [Fact]
    public void Request_From_Changed_Observer_Is_Queued_Until_Current_Commit_Finishes()
    {
        var coordinator = CreateCoordinator();
        var changed = new List<ThemeTransitionEventArgs>();
        coordinator.ThemeChanged += (_, args) =>
        {
            changed.Add(args);
            if (changed.Count == 1)
            {
                coordinator.Request(new ThemeRequest(
                    IThemeManager.DEFAULT_THEME_ID,
                    [ThemeAlgorithm.Default, ThemeAlgorithm.Compact],
                    ThemeTransitionReason.UserRequest));
            }
        };

        coordinator.Request(new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            [ThemeAlgorithm.Default, ThemeAlgorithm.Dark],
            ThemeTransitionReason.UserRequest));

        changed.Count.ShouldBe(2);
        changed[0].NewSnapshot.Algorithms.ShouldBe([ThemeAlgorithm.Default, ThemeAlgorithm.Dark]);
        changed[1].NewSnapshot.Algorithms.ShouldBe([ThemeAlgorithm.Default, ThemeAlgorithm.Compact]);
    }

    [Fact]
    public void Failed_Request_Retains_Previous_Active_Theme()
    {
        var manager = CreateManager();
        var coordinator = CreateCoordinator(manager);
        coordinator.Request(new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            [ThemeAlgorithm.Default],
            ThemeTransitionReason.Startup));
        var activeTheme = manager.ActivatedTheme;

        Should.Throw<ThemeNotFoundException>(() => coordinator.Request(new ThemeRequest(
            "MissingTheme",
            [ThemeAlgorithm.Default],
            ThemeTransitionReason.UserRequest)));

        manager.ActivatedTheme.ShouldBeSameAs(activeTheme);
    }

    [Fact]
    public void Changed_Observer_Exception_Does_Not_Roll_Back_Commit()
    {
        var manager = CreateManager();
        var coordinator = CreateCoordinator(manager);
        coordinator.ThemeChanged += (_, _) => throw new InvalidOperationException("observer failure");

        Should.NotThrow(() => coordinator.Request(new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            [ThemeAlgorithm.Default, ThemeAlgorithm.Dark],
            ThemeTransitionReason.UserRequest)));

        manager.ActivatedTheme.ShouldNotBeNull();
        manager.ActivatedTheme!.Algorithms.ShouldBe([ThemeAlgorithm.Default, ThemeAlgorithm.Dark]);
    }

    [Fact]
    public void Default_Font_Is_Compiled_Into_Transition_Snapshot()
    {
        var manager = CreateManager();
        var fontFamily = FontFamily.Parse("Arial");
        manager.FontFamily = fontFamily;
        var coordinator = CreateCoordinator(manager);
        var changed = new List<ThemeTransitionEventArgs>();
        coordinator.ThemeChanged += (_, args) => changed.Add(args);

        coordinator.Request(new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            [ThemeAlgorithm.Default],
            ThemeTransitionReason.Startup));

        changed.Single().NewSnapshot.SharedToken.FontFamily.ShouldBe(fontFamily);
        manager.ActivatedTheme.ShouldNotBeNull();
        manager.ActivatedTheme!.SharedToken.FontFamily.ShouldBe(fontFamily);
    }

    [Fact]
    public async Task Request_From_Non_UI_Thread_Is_Rejected()
    {
        var coordinator = CreateManager(useInjectedAccess: false).ThemeCoordinator;

        await Task.Run(() =>
        {
            Should.Throw<InvalidOperationException>(() => coordinator.Request(new ThemeRequest(
                IThemeManager.DEFAULT_THEME_ID,
                [ThemeAlgorithm.Default],
                ThemeTransitionReason.UserRequest)));
        }, TestContext.Current.CancellationToken);
    }

    private static ThemeCoordinator CreateCoordinator(ThemeManager? manager = null)
    {
        return (manager ?? CreateManager()).ThemeCoordinator;
    }

    private static ThemeManager CreateManager(bool useInjectedAccess = true)
    {
        using var _ = AvaloniaLocator.EnterScope();
        AvaloniaLocator.CurrentMutable
                       .Bind<IAssetLoader>()
                       .ToConstant(new TestAssetLoader());
        var manager = useInjectedAccess
            ? new ThemeManager(static () => true)
            : new ThemeManager();
        manager.ScanThemes();
        return manager;
    }

    private sealed class TestAssetLoader : IAssetLoader
    {
        private static readonly Uri s_themeUri =
            new("avares://AtomUI.Core/Assets/Themes/DaybreakBlue.xml");

        public bool Exists(Uri uri, Uri? baseUri = null)
        {
            return string.Equals(uri.ToString(), s_themeUri.ToString(), StringComparison.Ordinal);
        }

        public Stream Open(Uri uri, Uri? baseUri = null)
        {
            if (!Exists(uri, baseUri))
            {
                throw new FileNotFoundException(uri.ToString());
            }

            return new MemoryStream(Encoding.UTF8.GetBytes("""
                <?xml version="1.0" encoding="UTF-8"?>
                <Theme Name="Daybreak Blue" IsDefault="true">
                    <SharedTokens />
                    <ControlTokens />
                </Theme>
                """));
        }

        public (Stream stream, Assembly assembly) OpenAndGetAssembly(Uri uri, Uri? baseUri = null)
        {
            return (Open(uri, baseUri), typeof(TestAssetLoader).Assembly);
        }

        public IEnumerable<Uri> GetAssets(Uri uri, Uri? baseUri)
        {
            return [s_themeUri];
        }

        public Assembly? GetAssembly(Uri uri, Uri? baseUri = null)
        {
            return typeof(TestAssetLoader).Assembly;
        }

        public void SetDefaultAssembly(Assembly assembly)
        {
        }

        public void InvalidateAssemblyCache(string name)
        {
        }

        public void InvalidateAssemblyCache()
        {
        }
    }
}
