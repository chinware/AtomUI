using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ButtonSpinnerTests;

public class ButtonSpinnerSpacingTests
{
    [Fact]
    public void Inline_Right_Handle_Preserves_Content_Right_Padding()
    {
        var decoratedBox = new TestButtonSpinnerDecoratedBox
        {
            IsShowHandle          = true,
            IsHandleFloatable     = false,
            ButtonSpinnerLocation = ButtonSpinnerLocation.Right,
            SpinnerHandleWidth    = 28,
            ContentPadding        = new Thickness(11, 4, 11, 4)
        };

        decoratedBox.RecalculateEffectiveContentPadding();

        decoratedBox.EffectiveContentPadding.ShouldBe(new Thickness(11, 4, 39, 4));
    }

    [Fact]
    public void Inline_Left_Handle_Preserves_Content_Left_Padding()
    {
        var decoratedBox = new TestButtonSpinnerDecoratedBox
        {
            IsShowHandle          = true,
            IsHandleFloatable     = false,
            ButtonSpinnerLocation = ButtonSpinnerLocation.Left,
            SpinnerHandleWidth    = 28,
            ContentPadding        = new Thickness(11, 4, 11, 4)
        };

        decoratedBox.RecalculateEffectiveContentPadding();

        decoratedBox.EffectiveContentPadding.ShouldBe(new Thickness(39, 4, 11, 4));
    }

    private sealed class TestButtonSpinnerDecoratedBox : ButtonSpinnerDecoratedBox
    {
        public void RecalculateEffectiveContentPadding()
        {
            ConfigureEffectiveContentPadding();
        }
    }
}
