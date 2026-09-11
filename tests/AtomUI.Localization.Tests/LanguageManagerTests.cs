using System.Globalization;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LanguageManagerTests
{
    [Fact]
    public void Manager_Starts_At_Revision_Zero_And_Preserves_Supported_Language_Order()
    {
        var runtime = LanguageManagerTestRuntime.Create();

        runtime.Manager.Current.CurrentLanguage.ShouldBe(LanguageTags.EnUS);
        runtime.Manager.Current.Revision.ShouldBe(0);
        runtime.Manager.SupportedLanguages.Select(static definition => definition.Tag)
               .ShouldBe([LanguageTags.EnUS, LanguageTags.ZhCN, LanguageTags.ArSA]);
    }

    [Fact]
    public void ChangeLanguage_Commits_One_Atomic_Revision_With_Culture_And_Direction()
    {
        var runtime = LanguageManagerTestRuntime.Create();

        var result = runtime.Manager.ChangeLanguage(LanguageTags.ArSA);

        result.Status.ShouldBe(LanguageChangeStatus.Committed);
        result.OldState.CurrentLanguage.ShouldBe(LanguageTags.EnUS);
        result.OldState.Revision.ShouldBe(0);
        result.NewState.ShouldBeSameAs(runtime.Manager.Current);
        result.NewState.CurrentLanguage.ShouldBe(LanguageTags.ArSA);
        result.NewState.FormattingCulture.Name.ShouldBe("ar-SA");
        result.NewState.TextDirection.ShouldBe(LanguageTextDirection.RightToLeft);
        result.NewState.Revision.ShouldBe(1);
        runtime.Localizer.Get(RuntimeResourceKind.Value).ShouldBe("العربية");
    }

    [Fact]
    public void ChangeLanguage_To_Current_Language_Is_A_Silent_NoOp()
    {
        var runtime = LanguageManagerTestRuntime.Create();
        var resourceNotifications = 0;
        var languageEvents = 0;
        var host = new Border();
        host.Resources.MergedDictionaries.Add(runtime.Provider);
        ((IResourceHost)host).ResourcesChanged += (_, _) => resourceNotifications++;
        runtime.Manager.LanguageChanged += (_, _) => languageEvents++;

        var result = runtime.Manager.ChangeLanguage(LanguageTags.EnUS);

        result.Status.ShouldBe(LanguageChangeStatus.NoOp);
        result.OldState.ShouldBeSameAs(result.NewState);
        result.NewState.ShouldBeSameAs(runtime.Manager.Current);
        result.NewState.Revision.ShouldBe(0);
        resourceNotifications.ShouldBe(0);
        languageEvents.ShouldBe(0);
    }

    [Fact]
    public void ChangeLanguage_Rejects_Default_And_Unsupported_Tags_Without_Publishing()
    {
        var runtime = LanguageManagerTestRuntime.Create();
        var events = 0;
        runtime.Manager.LanguageChanged += (_, _) => events++;

        Should.Throw<ArgumentException>(() => runtime.Manager.ChangeLanguage(default));
        var exception = Should.Throw<LanguageNotSupportedException>(() =>
            runtime.Manager.ChangeLanguage(LanguageTags.FrFR));

        exception.Language.ShouldBe(LanguageTags.FrFR);
        runtime.Manager.Current.CurrentLanguage.ShouldBe(LanguageTags.EnUS);
        runtime.Manager.Current.Revision.ShouldBe(0);
        events.ShouldBe(0);
    }

    [Fact]
    public void ChangeLanguage_Requires_The_UI_Thread()
    {
        var runtime = LanguageManagerTestRuntime.Create(checkAccess: static () => false);

        var exception = Should.Throw<InvalidOperationException>(() =>
            runtime.Manager.ChangeLanguage(LanguageTags.ZhCN));

        exception.Message.ShouldContain("UI thread");
        runtime.Manager.Current.CurrentLanguage.ShouldBe(LanguageTags.EnUS);
    }

    [Fact]
    public void Disposed_Manager_Rejects_Changes_And_Clears_Event_Subscribers()
    {
        var runtime = LanguageManagerTestRuntime.Create();
        var events = 0;
        runtime.Manager.LanguageChanged += (_, _) => events++;

        runtime.Manager.Dispose();

        var exception = Should.Throw<InvalidOperationException>(() =>
            runtime.Manager.ChangeLanguage(LanguageTags.ZhCN));
        exception.Message.ShouldContain("disposed");
        events.ShouldBe(0);
    }

    [Fact]
    public void Committed_Change_Publishes_Resources_Before_One_Language_Event()
    {
        var runtime = LanguageManagerTestRuntime.Create();
        var order = new List<string>();
        var host = new Border();
        host.Resources.MergedDictionaries.Add(runtime.Provider);
        ((IResourceHost)host).ResourcesChanged += (_, _) =>
        {
            order.Add("resources");
            runtime.Manager.Current.CurrentLanguage.ShouldBe(LanguageTags.ZhCN);
            runtime.Provider.TryGetResource(RuntimeResourceKind.Value, null, out var value).ShouldBeTrue();
            value.ShouldBe("中文");
        };
        runtime.Manager.LanguageChanged += (_, args) =>
        {
            order.Add("event");
            args.Result.NewState.ShouldBeSameAs(runtime.Manager.Current);
        };

        runtime.Manager.ChangeLanguage(LanguageTags.ZhCN);

        order.ShouldBe(["resources", "event"]);
    }

    [Fact]
    public void Throwing_Language_Event_Subscriber_Does_Not_Block_Later_Subscribers()
    {
        var runtime = LanguageManagerTestRuntime.Create();
        var laterSubscriberCalled = false;
        runtime.Manager.LanguageChanged += (_, _) => throw new TestSubscriberException();
        runtime.Manager.LanguageChanged += (_, _) => laterSubscriberCalled = true;

        var result = runtime.Manager.ChangeLanguage(LanguageTags.ZhCN);

        result.Status.ShouldBe(LanguageChangeStatus.Committed);
        runtime.Manager.Current.CurrentLanguage.ShouldBe(LanguageTags.ZhCN);
        laterSubscriberCalled.ShouldBeTrue();
    }

    [Fact]
    public void Manager_Reuses_The_Same_Resource_Provider_Across_Changes()
    {
        var runtime = LanguageManagerTestRuntime.Create();
        var provider = runtime.Provider;
        var host = new Border();
        host.Resources.MergedDictionaries.Add(provider);

        runtime.Manager.ChangeLanguage(LanguageTags.ZhCN);
        runtime.Manager.ChangeLanguage(LanguageTags.ArSA);

        runtime.Provider.ShouldBeSameAs(provider);
        provider.Owner.ShouldBeSameAs(host);
        provider.TryGetResource(RuntimeResourceKind.Value, null, out var value).ShouldBeTrue();
        value.ShouldBe("العربية");
    }

    private sealed class TestSubscriberException : Exception;
}

internal sealed record LanguageManagerTestRuntime(
    LanguageManager Manager,
    LanguageResourceProvider Provider,
    Localizer Localizer)
{
    internal static LanguageManagerTestRuntime Create(Func<bool>? checkAccess = null)
    {
        var catalog = new LanguageCatalogDescriptor<RuntimeResourceKind>(
            "Acme:Acme.RuntimeResourceKind",
            [new LanguageCatalogUnitDescriptor("Value")],
            static key => key == RuntimeResourceKind.Value ? 0 : -1);
        var builder = new LocalizationBuilder();
        builder.AddCatalog(catalog);
        builder.AddTranslationBundle(CreateBundle(catalog, LanguageTags.EnUS, "English"));
        builder.AddTranslationBundle(CreateBundle(catalog, LanguageTags.ZhCN, "中文"));
        builder.AddTranslationBundle(CreateBundle(catalog, LanguageTags.ArSA, "العربية"));
        var registry = builder.FreezeRegistry();

        LanguageDefinition[] definitions =
        [
            new(
                LanguageTags.EnUS,
                CultureInfo.GetCultureInfo("en-US"),
                "English",
                LanguageTextDirection.LeftToRight),
            new(
                LanguageTags.ZhCN,
                CultureInfo.GetCultureInfo("zh-CN"),
                "简体中文",
                LanguageTextDirection.LeftToRight),
            new(
                LanguageTags.ArSA,
                CultureInfo.GetCultureInfo("ar-SA"),
                "العربية",
                LanguageTextDirection.RightToLeft)
        ];
        var snapshots = definitions.ToDictionary(
            static definition => definition.Tag,
            definition => LanguageSnapshotBuilder.Build(registry, definition.Tag, definition));
        var initialState = new LanguageState(
            LanguageTags.EnUS,
            definitions[0].FormattingCulture,
            definitions[0].TextDirection,
            revision: 0);
        var context = new LanguageContext(
            registry,
            new LanguageRevision(snapshots[LanguageTags.EnUS], initialState));
        var provider = new LanguageResourceProvider(context);
        var manager = new LanguageManager(
            context,
            snapshots,
            definitions,
            provider,
            checkAccess ?? (static () => true));

        return new LanguageManagerTestRuntime(manager, provider, new Localizer(context));
    }

    private static TranslationBundleDescriptor CreateBundle(
        LanguageCatalogDescriptor catalog,
        LanguageTag language,
        string value)
    {
        return new TranslationBundleDescriptor(
            catalog.CatalogId,
            language,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme",
            [value]);
    }
}

internal enum RuntimeResourceKind
{
    Value
}
