using AtomUI.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Localization;

public class LanguageResourceExtensionTests
{
    [Fact]
    public void Styled_Target_Uses_The_Enum_As_A_Dynamic_Resource_Key()
    {
        HeadlessTestApp.Run(() =>
        {
            var target = new TextBlock();
            var extension = new TestLanguageResourceExtension(TestResourceKind.Title);

            var value = extension.ProvideValue(
                new ProvideValueServiceProvider(target, TextBlock.TextProperty));

            value.ShouldBeOfType<DynamicResourceExtension>()
                 .ResourceKey.ShouldBe(TestResourceKind.Title);
            extension.Kind.ShouldBe(TestResourceKind.Title);
        });
    }

    [Fact]
    public void Plain_Target_Resolves_The_Current_Application_Resource()
    {
        HeadlessTestApp.Run(() =>
        {
            Application.Current!.Resources[TestResourceKind.Title] = "Resolved title";
            var extension = new TestLanguageResourceExtension(TestResourceKind.Title);

            var value = extension.ProvideValue(
                new ProvideValueServiceProvider(new object(), new object()));

            value.ShouldBe("Resolved title");
        });
    }

    [Fact]
    public void Missing_Resource_Key_Is_Rejected()
    {
        var extension = new TestLanguageResourceExtension();

        var exception = Should.Throw<InvalidOperationException>(() =>
            extension.ProvideValue(
                new ProvideValueServiceProvider(new TextBlock(), TextBlock.TextProperty)));

        exception.Message.ShouldContain("resource key");
    }

    private enum TestResourceKind
    {
        Title = 1
    }

    private sealed class TestLanguageResourceExtension : LanguageResourceExtension<TestResourceKind>
    {
        internal TestLanguageResourceExtension()
        {
        }

        internal TestLanguageResourceExtension(TestResourceKind kind)
            : base(kind)
        {
        }
    }

    private sealed class ProvideValueServiceProvider(
        object targetObject,
        object targetProperty) : IServiceProvider, IProvideValueTarget
    {
        public object TargetObject { get; } = targetObject;

        public object TargetProperty { get; } = targetProperty;

        public object? GetService(Type serviceType)
        {
            return serviceType == typeof(IProvideValueTarget) ? this : null;
        }
    }
}
