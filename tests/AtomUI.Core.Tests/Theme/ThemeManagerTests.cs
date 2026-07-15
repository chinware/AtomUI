using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeManagerTests
{
    [Fact]
    public void ScanThemes_Reports_Throwing_Control_Token_Activation_Before_Catalog_Publication()
    {
        var manager = CreateManager(typeof(ThrowingConstructorCompilerButtonToken));

        var exception = Should.Throw<ThemeLoadException>(() => manager.ScanThemes());

        exception.Message.ShouldContain(nameof(ThrowingConstructorCompilerButtonToken));
        exception.Message.ShouldContain(ThrowingConstructorCompilerButtonToken.ExceptionMessage);
    }

    [Fact]
    public void ScanThemes_Reports_Non_Control_Token_Registration_Before_Catalog_Publication()
    {
        var manager = CreateManager(typeof(InvalidCompilerToken));

        var exception = Should.Throw<ThemeLoadException>(() => manager.ScanThemes());

        exception.Message.ShouldContain(nameof(InvalidCompilerToken));
        exception.Message.ShouldContain(nameof(AbstractControlDesignToken));
    }

    [Fact]
    public void ScanThemes_Reports_Duplicate_Control_Token_Ids_Before_Catalog_Publication()
    {
        var manager = CreateManager(typeof(CompilerButtonToken), typeof(DuplicateCompilerButtonToken));

        var exception = Should.Throw<ThemeLoadException>(() => manager.ScanThemes());

        exception.Message.ShouldContain(CompilerButtonToken.ID);
    }

    [Fact]
    public void ScanThemes_Activates_Each_Registered_Control_Token_Once_For_Schema_Preflight()
    {
        ActivationCountingCompilerButtonToken.ResetActivationCount();
        var manager = CreateManager(typeof(ActivationCountingCompilerButtonToken));

        manager.CreateComponentTokenSchemas();

        ActivationCountingCompilerButtonToken.ActivationCount.ShouldBe(1);
    }

    private static ThemeManager CreateManager(params Type[] tokenTypes)
    {
        var manager = new ThemeManager();
        foreach (var tokenType in tokenTypes)
        {
            manager.RegisterControlTokenType(tokenType);
        }

        return manager;
    }
}
