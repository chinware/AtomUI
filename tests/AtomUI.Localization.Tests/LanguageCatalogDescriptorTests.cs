using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LanguageCatalogDescriptorTests
{
    [Fact]
    public void LanguageCatalogAttribute_Targets_Enums_And_Defaults_To_Version_One()
    {
        var usage = typeof(LanguageCatalogAttribute)
                    .GetCustomAttributes(typeof(AttributeUsageAttribute), inherit: false)
                    .Cast<AttributeUsageAttribute>()
                    .ShouldHaveSingleItem();
        var attribute = new LanguageCatalogAttribute();

        usage.ValidOn.ShouldBe(AttributeTargets.Enum);
        usage.AllowMultiple.ShouldBeFalse();
        usage.Inherited.ShouldBeFalse();
        attribute.ContractVersion.ShouldBe(1);
    }

    [Fact]
    public void Catalog_Descriptor_Exposes_Strongly_Typed_Unit_Slots()
    {
        var descriptor = CreateCatalog();

        descriptor.CatalogId.ShouldBe("Acme.App:Acme.LoginLangResourceKind");
        descriptor.ContractVersion.ShouldBe(1);
        descriptor.ResourceKindType.ShouldBe(typeof(LoginLangResourceKind));
        descriptor.Units.Select(static unit => unit.Id).ShouldBe([1, 2]);
        descriptor.Units.Select(static unit => unit.Name).ShouldBe(["Title", "ItemCount"]);
        descriptor.Units[1].IsFormatted.ShouldBeTrue();
        descriptor.TryGetUnitSlot(LoginLangResourceKind.Title, out var titleSlot).ShouldBeTrue();
        titleSlot.ShouldBe(0);
        descriptor.TryGetUnitSlot((LoginLangResourceKind)999, out _).ShouldBeFalse();
    }

    [Fact]
    public void Catalog_Descriptor_Defensively_Copies_Units()
    {
        var units = new[]
        {
            new LanguageCatalogUnitDescriptor(1, "Title")
        };
        var descriptor = new LanguageCatalogDescriptor<LoginLangResourceKind>(
            "Acme.App:Acme.LoginLangResourceKind",
            1,
            units,
            static key => key == LoginLangResourceKind.Title ? 0 : -1);

        units[0] = new LanguageCatalogUnitDescriptor(2, "Changed");

        descriptor.Units[0].Id.ShouldBe(1);
        descriptor.Units[0].Name.ShouldBe("Title");
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData(" ", 1)]
    [InlineData("missing-module-separator", 1)]
    [InlineData("Acme.App:Acme.LoginLangResourceKind", 0)]
    [InlineData("Acme.App:Acme.LoginLangResourceKind", -1)]
    public void Catalog_Descriptor_Rejects_Invalid_Identity_Or_Version(
        string catalogId,
        int contractVersion)
    {
        Should.Throw<ArgumentException>(() => new LanguageCatalogDescriptor<LoginLangResourceKind>(
            catalogId,
            contractVersion,
            [new LanguageCatalogUnitDescriptor(1, "Title")],
            static _ => 0));
    }

    [Fact]
    public void Catalog_Descriptor_Rejects_Duplicate_Unit_Ids_And_Names()
    {
        Should.Throw<ArgumentException>(() => new LanguageCatalogDescriptor<LoginLangResourceKind>(
            "Acme.App:Acme.LoginLangResourceKind",
            1,
            [
                new LanguageCatalogUnitDescriptor(1, "Title"),
                new LanguageCatalogUnitDescriptor(1, "Subtitle")
            ],
            static _ => 0));
        Should.Throw<ArgumentException>(() => new LanguageCatalogDescriptor<LoginLangResourceKind>(
            "Acme.App:Acme.LoginLangResourceKind",
            1,
            [
                new LanguageCatalogUnitDescriptor(1, "Title"),
                new LanguageCatalogUnitDescriptor(2, "Title")
            ],
            static _ => 0));
    }

    [Theory]
    [InlineData(0, "Title")]
    [InlineData(-1, "Title")]
    [InlineData(1, "")]
    [InlineData(1, " ")]
    public void Unit_Descriptor_Rejects_Invalid_Identity(int id, string name)
    {
        Should.Throw<ArgumentException>(() => new LanguageCatalogUnitDescriptor(id, name));
    }

    [Fact]
    public void Translation_Bundle_Exposes_Compiled_Partial_Values()
    {
        string?[] values = ["Sign in", null];
        var bundle = new TranslationBundleDescriptor(
            "Acme.App:Acme.LoginLangResourceKind",
            1,
            LanguageTags.EnUS,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme.App",
            values);

        values[0] = "Changed";

        bundle.CatalogId.ShouldBe("Acme.App:Acme.LoginLangResourceKind");
        bundle.ContractVersion.ShouldBe(1);
        bundle.Language.ShouldBe(LanguageTags.EnUS);
        bundle.SourceKind.ShouldBe(TranslationSourceKind.ModuleBuiltIn);
        bundle.SourceIdentity.ShouldBe("Acme.App");
        bundle.Values.ShouldBe(["Sign in", null]);
    }

    [Fact]
    public void Translation_Bundle_Rejects_Invalid_Metadata()
    {
        Should.Throw<ArgumentException>(() => new TranslationBundleDescriptor(
            "invalid",
            1,
            LanguageTags.EnUS,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme.App",
            ["Title"]));
        Should.Throw<ArgumentOutOfRangeException>(() => new TranslationBundleDescriptor(
            "Acme.App:Acme.LoginLangResourceKind",
            0,
            LanguageTags.EnUS,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme.App",
            ["Title"]));
        Should.Throw<ArgumentException>(() => new TranslationBundleDescriptor(
            "Acme.App:Acme.LoginLangResourceKind",
            1,
            default,
            TranslationSourceKind.ModuleBuiltIn,
            "Acme.App",
            ["Title"]));
        Should.Throw<ArgumentOutOfRangeException>(() => new TranslationBundleDescriptor(
            "Acme.App:Acme.LoginLangResourceKind",
            1,
            LanguageTags.EnUS,
            (TranslationSourceKind)byte.MaxValue,
            "Acme.App",
            ["Title"]));
        Should.Throw<ArgumentException>(() => new TranslationBundleDescriptor(
            "Acme.App:Acme.LoginLangResourceKind",
            1,
            LanguageTags.EnUS,
            TranslationSourceKind.ModuleBuiltIn,
            " ",
            ["Title"]));
    }

    private static LanguageCatalogDescriptor<LoginLangResourceKind> CreateCatalog()
    {
        return new LanguageCatalogDescriptor<LoginLangResourceKind>(
            "Acme.App:Acme.LoginLangResourceKind",
            1,
            [
                new LanguageCatalogUnitDescriptor(1, "Title"),
                new LanguageCatalogUnitDescriptor(2, "ItemCount", isFormatted: true)
            ],
            static key => key switch
            {
                LoginLangResourceKind.Title => 0,
                LoginLangResourceKind.ItemCount => 1,
                _ => -1
            });
    }

    [LanguageCatalog(ContractVersion = 1)]
    private enum LoginLangResourceKind
    {
        Title = 1,
        ItemCount = 2
    }
}
