using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.MessageBox;

public class MessageBoxArchitectureTests
{
    [Fact]
    public void MessageBox_Is_A_Dialog_Specialization()
    {
        typeof(AtomUI.Desktop.Controls.MessageBox).IsSubclassOf(typeof(AtomUI.Desktop.Controls.Dialog))
                                                  .ShouldBeTrue();
    }

    [Fact]
    public void MessageBox_Does_Not_Own_A_Hidden_Dialog()
    {
        typeof(AtomUI.Desktop.Controls.MessageBox)
            .GetFields(System.Reflection.BindingFlags.Instance |
                       System.Reflection.BindingFlags.Public |
                       System.Reflection.BindingFlags.NonPublic)
            .ShouldNotContain(field => typeof(AtomUI.Desktop.Controls.Dialog).IsAssignableFrom(field.FieldType));
    }
}
