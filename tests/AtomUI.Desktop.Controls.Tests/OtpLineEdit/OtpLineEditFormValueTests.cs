using AtomUI.Controls;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.OtpLineEdit;

public class OtpLineEditFormValueTests
{
    static OtpLineEditFormValueTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Form_Set_Value_Normalizes_Text()
    {
        var otpLineEdit = new AtomUI.Desktop.Controls.OtpLineEdit
        {
            Length    = 4,
            InputMode = OtpLineEditInputMode.Numeric
        };
        var formItem = (IFormItemAware)otpLineEdit;

        formItem.SetFormValue("1a234");

        otpLineEdit.Text.ShouldBe("1234");
    }

    [Fact]
    public void Form_Get_Value_Returns_Text()
    {
        var otpLineEdit = new AtomUI.Desktop.Controls.OtpLineEdit
        {
            Text = "123456"
        };
        var formItem = (IFormItemAware)otpLineEdit;

        formItem.GetFormValue().ShouldBe("123456");
    }

    [Fact]
    public void Form_Clear_Value_Clears_Text()
    {
        var otpLineEdit = new AtomUI.Desktop.Controls.OtpLineEdit
        {
            Text = "123456"
        };
        var formItem = (IFormItemAware)otpLineEdit;

        formItem.ClearFormValue();

        otpLineEdit.Text.ShouldBeNull();
    }

    [Fact]
    public void Form_ValueChanged_Fires_When_Text_Changes()
    {
        var otpLineEdit = new AtomUI.Desktop.Controls.OtpLineEdit();
        var formItem    = (IFormItemAware)otpLineEdit;
        var changeCount = 0;
        formItem.ValueChanged += (_, _) => changeCount++;

        otpLineEdit.Text = "1";
        otpLineEdit.Clear();

        changeCount.ShouldBe(2);
    }

    [Fact]
    public void Validate_Status_Does_Not_Clear_Native_Validation_Error()
    {
        var otpLineEdit    = new AtomUI.Desktop.Controls.OtpLineEdit();
        var formItem       = (IFormItemAware)otpLineEdit;
        var validationError = new InvalidOperationException("native");

        DataValidationErrors.SetError(otpLineEdit, validationError);
        formItem.NotifyValidateStatus(FormValidateStatus.Success);

        DataValidationErrors.GetHasErrors(otpLineEdit).ShouldBeTrue();
        DataValidationErrors.GetErrors(otpLineEdit).ShouldBe([validationError]);
    }
}
