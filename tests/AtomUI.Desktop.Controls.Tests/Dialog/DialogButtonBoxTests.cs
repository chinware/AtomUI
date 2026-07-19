using Avalonia.Interactivity;
using Shouldly;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogButtonBoxTests
{
    static DialogButtonBoxTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Custom_Buttons_Support_Replace_Move_And_Reset()
    {
        var buttonBox   = new DialogButtonBox();
        var first       = new DialogButton { Content = "First" };
        var second      = new DialogButton { Content = "Second" };
        var replacement = new DialogButton { Content = "Replacement" };
        buttonBox.CustomButtons.Add(first);
        buttonBox.CustomButtons.Add(second);

        Should.NotThrow(() => buttonBox.CustomButtons[0] = replacement);
        Should.NotThrow(() => buttonBox.CustomButtons.Move(1, 0));
        Should.NotThrow(() => buttonBox.CustomButtons.Clear());

        buttonBox.CustomButtons.ShouldBeEmpty();
    }

    [Fact]
    public void Default_And_Escape_Button_Changes_Update_Existing_Standard_Buttons()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var buttonBox = new DialogButtonBox
            {
                StandardButtons      = DialogStandardButton.Ok | DialogStandardButton.Cancel,
                DefaultStandardButton = DialogStandardButton.Ok,
                EscapeStandardButton  = DialogStandardButton.Cancel
            };
            var window = new AtomUI.Desktop.Controls.Window { Content = buttonBox };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                buttonBox.ApplyTemplate();
                var okButton = window.GetVisualDescendants()
                                     .OfType<DialogButton>()
                                     .Single(button => button.StandardButtonType == DialogStandardButton.Ok);
                var cancelButton = window.GetVisualDescendants()
                                         .OfType<DialogButton>()
                                         .Single(button => button.StandardButtonType == DialogStandardButton.Cancel);

                buttonBox.DefaultStandardButton = DialogStandardButton.Cancel;
                buttonBox.EscapeStandardButton  = DialogStandardButton.Ok;

                okButton.ButtonType.ShouldBe(AtomUI.Desktop.Controls.ButtonType.Default);
                okButton.IsDefaultConfirmButton.ShouldBeFalse();
                okButton.IsDefaultEscapeButton.ShouldBeTrue();
                cancelButton.ButtonType.ShouldBe(AtomUI.Desktop.Controls.ButtonType.Primary);
                cancelButton.IsDefaultConfirmButton.ShouldBeTrue();
                cancelButton.IsDefaultEscapeButton.ShouldBeFalse();
            }
            finally
            {
                window.Close();
            }
        });
    }

    [Fact]
    public void Custom_Role_Button_Is_Included_In_The_Effective_Visual_Sequence()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var custom = new DialogButton { Content = "Custom" };
            var buttonBox = new DialogButtonBox();
            buttonBox.CustomButtons.Add(custom);
            var window = new AtomUI.Desktop.Controls.Window { Content = buttonBox };

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                buttonBox.ApplyTemplate();

                buttonBox.GetVisualDescendants().OfType<DialogButton>().ShouldContain(custom);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [Fact]
    public void Template_Reapply_Detaches_Old_Visuals_And_Does_Not_Duplicate_Clicks()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var custom = new DialogButton
            {
                Content = "Custom",
                Role    = DialogButtonRole.AcceptRole
            };
            var buttonBox = new DialogButtonBox();
            buttonBox.CustomButtons.Add(custom);
            var window = new AtomUI.Desktop.Controls.Window { Content = buttonBox };
            var clickCount = 0;
            buttonBox.Clicked += (_, _) => clickCount++;

            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                buttonBox.ApplyTemplate();
                var template = buttonBox.Template.ShouldNotBeNull();

                buttonBox.Template = null;
                buttonBox.ApplyTemplate();
                Dispatcher.UIThread.RunJobs();

                custom.Parent.ShouldBeNull();

                buttonBox.Template = template;
                Should.NotThrow(buttonBox.ApplyTemplate);
                Dispatcher.UIThread.RunJobs();

                buttonBox.GetVisualDescendants().OfType<DialogButton>().ShouldContain(custom);
                custom.RaiseEvent(new RoutedEventArgs(AtomUI.Desktop.Controls.Button.ClickEvent));
                clickCount.ShouldBe(1);
            }
            finally
            {
                window.Close();
            }
        });
    }

}
