using System.Reflection;
using System.Text;
using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Resources;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Platform;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeSnapshotCacheTests
{
    private static readonly ControlTokenIdentity s_buttonIdentity = new(null, CompilerButtonToken.ID);

    [Fact]
    public void Equal_Compile_Request_Returns_The_Same_Snapshot_And_Compiles_Once()
    {
        ActivationCountingCompilerButtonToken.ResetActivationCount();
        var cache = new ThemeSnapshotCache();
        var compiler = new ThemeCompiler();
        var request = CreateRequest(registrations:
        [
            new ControlTokenRegistration(typeof(ActivationCountingCompilerButtonToken))
        ]);

        var first = cache.GetOrCompile(request, compiler);
        var second = cache.GetOrCompile(CreateRequest(registrations:
        [
            new ControlTokenRegistration(typeof(ActivationCountingCompilerButtonToken))
        ]), compiler);

        first.Success.ShouldBeTrue();
        second.Success.ShouldBeTrue();
        second.Snapshot.ShouldBeSameAs(first.Snapshot);
        ActivationCountingCompilerButtonToken.ActivationCount.ShouldBe(1);
    }

    [Fact]
    public void Algorithm_Order_And_Override_Differences_Create_Different_Snapshots()
    {
        var cache = new ThemeSnapshotCache();
        var compiler = new ThemeCompiler();

        var darkCompact = cache.GetOrCompile(CreateRequest(algorithms:
        [
            ThemeAlgorithm.Default,
            ThemeAlgorithm.Dark,
            ThemeAlgorithm.Compact
        ]), compiler).Snapshot!;
        var compactDark = cache.GetOrCompile(CreateRequest(algorithms:
        [
            ThemeAlgorithm.Default,
            ThemeAlgorithm.Compact,
            ThemeAlgorithm.Dark
        ]), compiler).Snapshot!;
        var red = cache.GetOrCompile(CreateRequest(
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#ff0000"))), compiler).Snapshot!;
        var green = cache.GetOrCompile(CreateRequest(
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#00b96b"))), compiler).Snapshot!;
        var runtimeRed = cache.GetOrCompile(CreateRequest(
            runtimeOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#ff0000"))), compiler).Snapshot!;
        var control44 = cache.GetOrCompile(CreateRequest(
            controlOverrides: ControlOverride(44)), compiler).Snapshot!;
        var control48 = cache.GetOrCompile(CreateRequest(
            controlOverrides: ControlOverride(48)), compiler).Snapshot!;

        compactDark.ShouldNotBeSameAs(darkCompact);
        green.ShouldNotBeSameAs(red);
        runtimeRed.ShouldNotBeSameAs(red);
        control48.ShouldNotBeSameAs(control44);
    }

    [Fact]
    public void Parent_Version_Is_Part_Of_The_Cache_Key()
    {
        var compiler = new ThemeCompiler();
        var cache = new ThemeSnapshotCache();
        var firstParent = compiler.Compile(CreateRequest(
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#ff0000")))).Snapshot!;
        var secondParent = compiler.Compile(CreateRequest(
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#ff0000")))).Snapshot!;

        var firstChild = cache.GetOrCompile(CreateRequest(
            parent: firstParent,
            algorithms: [],
            sharedOverrides: Tokens((nameof(DesignToken.BorderRadius), "12"))), compiler).Snapshot!;
        var secondChild = cache.GetOrCompile(CreateRequest(
            parent: secondParent,
            algorithms: [],
            sharedOverrides: Tokens((nameof(DesignToken.BorderRadius), "12"))), compiler).Snapshot!;

        secondParent.Version.ShouldNotBe(firstParent.Version);
        secondChild.ShouldNotBeSameAs(firstChild);
    }

    [Fact]
    public void Inactive_Entries_Are_Bounded_But_Pinned_Active_Snapshots_Are_Not_Evicted()
    {
        var cache = new ThemeSnapshotCache(inactiveCapacity: 2);
        var compiler = new ThemeCompiler();
        var inactiveRequest = CreateRequest(
            sharedOverrides: Tokens((nameof(DesignToken.BorderRadius), "4")));
        var activeRequest = CreateRequest(
            sharedOverrides: Tokens((nameof(DesignToken.BorderRadius), "8")));
        var inactive = cache.GetOrCompile(inactiveRequest, compiler).Snapshot!;
        var active = cache.GetOrCompile(activeRequest, compiler).Snapshot!;
        using var pin = cache.Pin(active);

        for (var i = 0; i < 8; i++)
        {
            cache.GetOrCompile(CreateRequest(
                sharedOverrides: Tokens((nameof(DesignToken.BorderRadius), (20 + i).ToString()))), compiler)
                 .Success
                 .ShouldBeTrue();
        }

        cache.GetOrCompile(activeRequest, compiler).Snapshot.ShouldBeSameAs(active);
        cache.GetOrCompile(inactiveRequest, compiler).Snapshot.ShouldNotBeSameAs(inactive);
    }

    [Fact]
    public void Identical_Config_Providers_Share_Snapshot_But_Not_Resource_Provider_Instances()
    {
        using var _ = UseThemeManager();
        var providers = Enumerable.Range(0, 20)
                                  .Select(_ => Provider(Token(nameof(DesignToken.ColorPrimary), "#ff0000")))
                                  .ToArray();
        var roots = providers.Select(Attach).ToArray();
        FlushThemeUpdates();

        var tokenProviders = providers.Select(GetTokenResourceProvider).ToArray();
        var snapshots = tokenProviders.Select(provider => provider.Snapshot).ToArray();

        snapshots.Distinct(ReferenceEqualityComparer.Instance).Count().ShouldBe(1);
        tokenProviders.Distinct(ReferenceEqualityComparer.Instance).Count().ShouldBe(20);
        providers.ShouldAllBe(provider =>
            provider.Resources.MergedDictionaries.OfType<ThemeTokenResourceProvider>().Count() == 1);
        foreach (var root in roots)
        {
            root.Child.ShouldNotBeNull();
        }
    }

    [Fact]
    public void Repeated_Global_Requests_Compile_Once_And_Emit_One_Changed_Event()
    {
        using var _ = AvaloniaLocator.EnterScope();
        AvaloniaLocator.CurrentMutable
                       .Bind<IAssetLoader>()
                       .ToConstant(new TestAssetLoader());
        var manager = new ThemeManager(static () => true);
        manager.RegisterControlTokenType(typeof(GlobalCompileCountingToken));
        manager.ScanThemes();
        GlobalCompileCountingToken.ResetActivationCount();
        var changed = 0;
        manager.ThemeCoordinator.ThemeChanged += (_, _) => changed++;
        var request = new ThemeRequest(
            IThemeManager.DEFAULT_THEME_ID,
            [ThemeAlgorithm.Default],
            ThemeTransitionReason.UserRequest);

        for (var i = 0; i < 100; i++)
        {
            manager.ThemeCoordinator.Request(request);
        }

        changed.ShouldBe(1);
        GlobalCompileCountingToken.ActivationCount.ShouldBe(1);
    }

    private static ThemeCompileRequest CreateRequest(
        ThemeSnapshot? parent = null,
        IReadOnlyList<ThemeAlgorithm>? algorithms = null,
        IReadOnlyDictionary<string, string>? sharedOverrides = null,
        IReadOnlyDictionary<ControlTokenIdentity, ControlTokenConfigInfo>? controlOverrides = null,
        IReadOnlyList<ControlTokenRegistration>? registrations = null,
        IReadOnlyDictionary<string, string>? runtimeOverrides = null)
    {
        var effectiveAlgorithms = algorithms ?? [ThemeAlgorithm.Default];
        var definition = new ThemeDefinition(
            "TestTheme",
            "Test Theme",
            false,
            effectiveAlgorithms,
            new Dictionary<string, string>(),
            new Dictionary<string, ThemeControlTokenDefinition>());
        return new ThemeCompileRequest(
            "TestTheme",
            definition,
            parent,
            effectiveAlgorithms,
            sharedOverrides ?? new Dictionary<string, string>(),
            controlOverrides ?? new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>(),
            registrations ?? [new ControlTokenRegistration(typeof(CompilerButtonToken))],
            runtimeOverrides ?? new Dictionary<string, string>());
    }

    private static Dictionary<ControlTokenIdentity, ControlTokenConfigInfo> ControlOverride(double height)
    {
        return new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>
        {
            [s_buttonIdentity] = new ControlTokenConfigInfo
            {
                TokenId = CompilerButtonToken.ID,
                Tokens = Tokens((nameof(CompilerButtonToken.Height), height.ToString(System.Globalization.CultureInfo.InvariantCulture)))
            }
        };
    }

    private static Dictionary<string, string> Tokens(params (string Name, string Value)[] values)
    {
        return values.ToDictionary(value => value.Name, value => value.Value, StringComparer.Ordinal);
    }

    private static ThemeConfigProvider Provider(params TokenSetter[] sharedTokenSetters)
    {
        var provider = new ThemeConfigProvider
        {
            Content = new Border()
        };
        foreach (var setter in sharedTokenSetters)
        {
            provider.SharedTokenSetters.Add(setter);
        }
        return provider;
    }

    private static TokenSetter Token(string key, string value)
    {
        return new TokenSetter(null, key, value);
    }

    private static ThemeTokenResourceProvider GetTokenResourceProvider(ThemeConfigProvider provider)
    {
        return provider.Resources.MergedDictionaries
                       .OfType<ThemeTokenResourceProvider>()
                       .ShouldHaveSingleItem();
    }

    private static IDisposable UseThemeManager()
    {
        var scope = AvaloniaLocator.EnterScope();
        var manager = new ThemeManager();
        manager.RegisterControlTokenType(typeof(CompilerButtonToken));
        AvaloniaLocator.CurrentMutable.BindToSelf(manager);
        return scope;
    }

    private static void FlushThemeUpdates()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.RunJobs();
            return;
        }

        Dispatcher.UIThread.Invoke(static () => Dispatcher.UIThread.RunJobs());
    }

    private static LogicalTestRoot Attach(Control control)
    {
        var root = new LogicalTestRoot
        {
            Child = control
        };
        FlushThemeUpdates();
        return root;
    }

    private sealed class LogicalTestRoot : Decorator, ILogicalRoot
    {
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

internal sealed class GlobalCompileCountingToken : AbstractControlDesignToken
{
    internal const string ID = "GlobalCompileCounting";

    internal static int ActivationCount { get; private set; }

    public double Height { get; set; }

    public GlobalCompileCountingToken()
        : this(true)
    {
    }

    private GlobalCompileCountingToken(bool countActivation)
        : base(ID)
    {
        if (countActivation)
        {
            ActivationCount++;
        }
    }

    internal static void ResetActivationCount()
    {
        ActivationCount = 0;
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        Height = SharedToken.ControlHeight;
    }

    public override AbstractDesignToken Clone()
    {
        var cloned = new GlobalCompileCountingToken(false)
        {
            Height = Height
        };
        cloned.AssignSharedToken(SharedToken);
        cloned.SetHasCustomTokenConfig(HasCustomTokenConfig());
        cloned.SetCustomTokens(GetCustomTokens().ToList());
        return cloned;
    }

    protected override Type GetTokenKindType()
    {
        return typeof(CompilerButtonTokenKind);
    }
}
