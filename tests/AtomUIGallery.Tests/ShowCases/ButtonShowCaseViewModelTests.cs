using AtomUIGallery.ShowCases.Button;
using ReactiveUI;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ButtonShowCaseViewModelTests
{
    [Fact]
    public void Button_Documentation_Rows_Are_Created_Lazily_Per_Tab()
    {
        var viewModel = new ButtonViewModel(new TestScreen());

        viewModel.ApiRows.ShouldBeNull();
        viewModel.DesignTokenRows.ShouldBeNull();

        viewModel.EnsureApiRows();

        viewModel.ApiRows.ShouldNotBeNull();
        viewModel.ApiRows.Count.ShouldBe(6);
        viewModel.ApiRows[0].Property.ShouldBe("ButtonType");
        viewModel.ApiRows[0].Type.ShouldBe("ButtonType");
        viewModel.DesignTokenRows.ShouldBeNull();

        var apiRows = viewModel.ApiRows;
        viewModel.EnsureApiRows();
        viewModel.ApiRows.ShouldBeSameAs(apiRows);

        viewModel.EnsureDesignTokenRows();

        viewModel.DesignTokenRows.ShouldNotBeNull();
        viewModel.DesignTokenRows.Count.ShouldBe(3);
        viewModel.DesignTokenRows[0].Token.ShouldBe("ColorPrimary");
        viewModel.DesignTokenRows[0].ScopeTagColor.ShouldBe("blue");

        var designTokenRows = viewModel.DesignTokenRows;
        viewModel.EnsureDesignTokenRows();
        viewModel.DesignTokenRows.ShouldBeSameAs(designTokenRows);
    }

    private sealed class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }
}
