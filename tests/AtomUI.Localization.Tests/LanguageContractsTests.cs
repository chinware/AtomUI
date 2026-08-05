using System.Globalization;
using Shouldly;
using Xunit;

namespace AtomUI.Localization.Tests;

public class LanguageContractsTests
{
    [Fact]
    public void LanguageDefinition_Exposes_Validated_Metadata()
    {
        var definition = new LanguageDefinition(
            LanguageTag.Parse("ar-SA"),
            CultureInfo.GetCultureInfo("ar-SA"),
            "العربية",
            LanguageTextDirection.RightToLeft);

        definition.Tag.Value.ShouldBe("ar-SA");
        definition.FormattingCulture.Name.ShouldBe("ar-SA");
        definition.NativeName.ShouldBe("العربية");
        definition.TextDirection.ShouldBe(LanguageTextDirection.RightToLeft);
    }

    [Fact]
    public void LanguageDefinition_Clones_And_Freezes_FormattingCulture()
    {
        var culture = new CultureInfo("en-US");
        culture.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";

        var definition = new LanguageDefinition(
            LanguageTag.Parse("en-US"),
            culture,
            "English",
            LanguageTextDirection.LeftToRight);

        culture.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";

        definition.FormattingCulture.DateTimeFormat.ShortDatePattern.ShouldBe("yyyy/MM/dd");
        definition.FormattingCulture.IsReadOnly.ShouldBeTrue();
        Should.Throw<InvalidOperationException>(() =>
            definition.FormattingCulture.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy");
    }

    [Fact]
    public void LanguageDefinition_Rejects_Default_Tag()
    {
        Should.Throw<ArgumentException>(() => new LanguageDefinition(
            default,
            CultureInfo.GetCultureInfo("en-US"),
            "English",
            LanguageTextDirection.LeftToRight));
    }

    [Fact]
    public void LanguageDefinition_Rejects_Null_Culture()
    {
        Should.Throw<ArgumentNullException>(() => new LanguageDefinition(
            LanguageTag.Parse("en-US"),
            null!,
            "English",
            LanguageTextDirection.LeftToRight));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void LanguageDefinition_Rejects_Missing_NativeName(string nativeName)
    {
        Should.Throw<ArgumentException>(() => new LanguageDefinition(
            LanguageTag.Parse("en-US"),
            CultureInfo.GetCultureInfo("en-US"),
            nativeName,
            LanguageTextDirection.LeftToRight));
    }

    [Fact]
    public void LanguageDefinition_Rejects_Null_NativeName()
    {
        Should.Throw<ArgumentNullException>(() => new LanguageDefinition(
            LanguageTag.Parse("en-US"),
            CultureInfo.GetCultureInfo("en-US"),
            null!,
            LanguageTextDirection.LeftToRight));
    }

    [Fact]
    public void LanguageDefinition_Rejects_Undefined_TextDirection()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new LanguageDefinition(
            LanguageTag.Parse("en-US"),
            CultureInfo.GetCultureInfo("en-US"),
            "English",
            (LanguageTextDirection)byte.MaxValue));
    }

    [Fact]
    public void LanguageState_Exposes_Immutable_Committed_State()
    {
        var culture = new CultureInfo("zh-CN");
        culture.NumberFormat.NumberDecimalSeparator = ",";

        var state = new LanguageState(
            LanguageTag.Parse("zh-CN"),
            culture,
            LanguageTextDirection.LeftToRight,
            12);

        culture.NumberFormat.NumberDecimalSeparator = ".";

        state.CurrentLanguage.Value.ShouldBe("zh-CN");
        state.FormattingCulture.NumberFormat.NumberDecimalSeparator.ShouldBe(",");
        state.FormattingCulture.IsReadOnly.ShouldBeTrue();
        state.TextDirection.ShouldBe(LanguageTextDirection.LeftToRight);
        state.Revision.ShouldBe(12);
    }

    [Fact]
    public void LanguageState_Rejects_Default_Tag()
    {
        Should.Throw<ArgumentException>(() => new LanguageState(
            default,
            CultureInfo.GetCultureInfo("en-US"),
            LanguageTextDirection.LeftToRight,
            0));
    }

    [Fact]
    public void LanguageState_Rejects_Null_Culture()
    {
        Should.Throw<ArgumentNullException>(() => new LanguageState(
            LanguageTag.Parse("en-US"),
            null!,
            LanguageTextDirection.LeftToRight,
            0));
    }

    [Fact]
    public void LanguageState_Rejects_Undefined_TextDirection()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new LanguageState(
            LanguageTag.Parse("en-US"),
            CultureInfo.GetCultureInfo("en-US"),
            (LanguageTextDirection)byte.MaxValue,
            0));
    }

    [Fact]
    public void LanguageState_Rejects_Negative_Revision()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => new LanguageState(
            LanguageTag.Parse("en-US"),
            CultureInfo.GetCultureInfo("en-US"),
            LanguageTextDirection.LeftToRight,
            -1));
    }

    [Fact]
    public void Committed_Result_Exposes_Old_And_New_State()
    {
        var oldState = CreateState("en-US", 0);
        var newState = CreateState("zh-CN", 1);

        var result = LanguageChangeResult.Committed(oldState, newState);

        result.Status.ShouldBe(LanguageChangeStatus.Committed);
        result.OldState.ShouldBeSameAs(oldState);
        result.NewState.ShouldBeSameAs(newState);
    }

    [Fact]
    public void NoOp_Result_Uses_The_Same_State_For_Both_Sides()
    {
        var state = CreateState("en-US", 0);

        var result = LanguageChangeResult.NoOp(state);

        result.Status.ShouldBe(LanguageChangeStatus.NoOp);
        result.OldState.ShouldBeSameAs(state);
        result.NewState.ShouldBeSameAs(state);
    }

    [Fact]
    public void LanguageChangeResult_Rejects_Null_States()
    {
        var state = CreateState("en-US", 0);

        Should.Throw<ArgumentNullException>(() => LanguageChangeResult.Committed(null!, state));
        Should.Throw<ArgumentNullException>(() => LanguageChangeResult.Committed(state, null!));
        Should.Throw<ArgumentNullException>(() => LanguageChangeResult.NoOp(null!));
    }

    [Fact]
    public void LanguageChangedEventArgs_Exposes_Only_A_Committed_Result()
    {
        var result = LanguageChangeResult.Committed(
            CreateState("en-US", 0),
            CreateState("zh-CN", 1));

        var eventArgs = new LanguageChangedEventArgs(result);

        eventArgs.Result.ShouldBeSameAs(result);
    }

    [Fact]
    public void LanguageChangedEventArgs_Rejects_Null_Or_NoOp_Result()
    {
        Should.Throw<ArgumentNullException>(() => new LanguageChangedEventArgs(null!));
        Should.Throw<ArgumentException>(() => new LanguageChangedEventArgs(
            LanguageChangeResult.NoOp(CreateState("en-US", 0))));
    }

    [Fact]
    public void LanguageExceptions_Preserve_Message_And_Inner_Exception()
    {
        var innerException = new InvalidOperationException("cause");

        var exceptions = new Exception[]
        {
            new LanguageConfigurationException("configuration", innerException),
            new LanguageCatalogException("catalog", innerException),
            new LanguageCoverageException("coverage", innerException)
        };

        exceptions.Select(exception => exception.InnerException).ShouldAllBe(
            inner => ReferenceEquals(inner, innerException));
        exceptions.Select(exception => exception.Message).ShouldBe(
            ["configuration", "catalog", "coverage"]);
    }

    [Fact]
    public void LanguageNotSupportedException_Exposes_Requested_Language()
    {
        var language = LanguageTag.Parse("ja-JP");

        var exception = new LanguageNotSupportedException(language);

        exception.Language.ShouldBe(language);
        exception.ParamName.ShouldBe("language");
        exception.Message.ShouldContain("ja-JP");
    }

    private static LanguageState CreateState(string language, long revision)
    {
        return new LanguageState(
            LanguageTag.Parse(language),
            CultureInfo.GetCultureInfo(language),
            LanguageTextDirection.LeftToRight,
            revision);
    }
}
