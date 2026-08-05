using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LanguageResourceProviderTests
{
    [Fact]
    public void Provider_Resolves_The_Exact_Boxed_Enum_Key_From_The_Current_Snapshot()
    {
        var runtime = LanguageManagerTestRuntime.Create();

        runtime.Provider.TryGetResource(RuntimeResourceKind.Value, null, out var english).ShouldBeTrue();
        runtime.Manager.ChangeLanguage(LanguageTags.ZhCN);
        runtime.Provider.TryGetResource(RuntimeResourceKind.Value, null, out var chinese).ShouldBeTrue();

        english.ShouldBe("English");
        chinese.ShouldBe("中文");
    }

    [Fact]
    public void Provider_Rejects_Unknown_Keys_And_Different_Enum_Types()
    {
        var runtime = LanguageManagerTestRuntime.Create();

        runtime.Provider.TryGetResource("Acme:Value", null, out var stringValue).ShouldBeFalse();
        runtime.Provider.TryGetResource(OtherResourceKind.Value, null, out var enumValue).ShouldBeFalse();

        stringValue.ShouldBeNull();
        enumValue.ShouldBeNull();
    }

    [Fact]
    public void Provider_Raises_Exactly_One_Host_Notification_Per_Committed_Change()
    {
        var runtime = LanguageManagerTestRuntime.Create();
        var host = new Border();
        host.Resources.MergedDictionaries.Add(runtime.Provider);
        var notifications = 0;
        ((IResourceHost)host).ResourcesChanged += (_, _) => notifications++;

        runtime.Manager.ChangeLanguage(LanguageTags.ZhCN);
        runtime.Manager.ChangeLanguage(LanguageTags.ZhCN);
        runtime.Manager.ChangeLanguage(LanguageTags.ArSA);

        notifications.ShouldBe(2);
    }

    private enum OtherResourceKind
    {
        Value = 1
    }
}
