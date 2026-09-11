using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUILineEdit = AtomUI.Desktop.Controls.LineEdit;
using AtomUIOtpLineEdit = AtomUI.Desktop.Controls.OtpLineEdit;
using AtomUISearchEdit = AtomUI.Desktop.Controls.SearchEdit;
using AtomUITextArea = AtomUI.Desktop.Controls.TextArea;
using AtomUITextBox = AtomUI.Desktop.Controls.TextBox;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Input;

public class TextInputLifecycleTests
{
    static TextInputLifecycleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(typeof(AtomUITextBox))]
    [InlineData(typeof(AtomUILineEdit))]
    [InlineData(typeof(AtomUISearchEdit))]
    [InlineData(typeof(AtomUITextArea))]
    [InlineData(typeof(EmbeddedTextBox))]
    [InlineData(typeof(SelectFilterTextBox))]
    [InlineData(typeof(InfoPickerTextBox))]
    [InlineData(typeof(QuickJumpEdit))]
    [InlineData(typeof(MentionTextArea))]
    [InlineData(typeof(AutoCompleteLineEditBox))]
    [InlineData(typeof(AutoCompleteSearchEditBox))]
    [InlineData(typeof(AutoCompleteTextAreaBox))]
    public void Template_State_And_Interactions_Remain_Connected_After_Reattach(Type inputType)
    {
        var input = CreateTextInput(inputType);
        var window = CreateWindow(input);

        try
        {
            var revealButton = FindOptionalTemplatePart<RevealButton>(input, "PART_RevealButton");
            revealButton?.IsVisible.ShouldBeFalse($"{inputType.Name} reveal button must initially be hidden.");

            Reattach(window, input);

            revealButton = FindOptionalTemplatePart<RevealButton>(input, "PART_RevealButton");
            revealButton?.IsVisible.ShouldBeFalse($"{inputType.Name} reveal button must remain hidden after reattach.");
            if (revealButton is not null)
            {
                input.IsEnableRevealButton = true;
                Dispatcher.UIThread.RunJobs();
                revealButton.IsVisible.ShouldBeTrue($"{inputType.Name} reveal visibility must remain bound after reattach.");

                input.RevealPassword = true;
                Dispatcher.UIThread.RunJobs();
                revealButton.IsChecked.ShouldBe(true,
                    $"{inputType.Name} reveal checked state must remain bound after reattach.");

                input.IsEnableRevealButton = false;
                Dispatcher.UIThread.RunJobs();
                revealButton.IsVisible.ShouldBeFalse();
            }

            input.Text = "value";
            Dispatcher.UIThread.RunJobs();

            var clearButton = FindTemplatePart<InputClearIconButton>(input, "PART_ClearButton");
            clearButton.IsVisible.ShouldBeTrue($"{inputType.Name} clear visibility must remain bound after reattach.");

            if (input is AtomUILineEdit lineEdit)
            {
                var content = new Border();
                lineEdit.InnerRightContent = content;
                Dispatcher.UIThread.RunJobs();

                FindTemplatePart<ContentPresenter>(input, "InnerRightContentPresenter")
                    .Content.ShouldBeSameAs(content);
            }
            else if (input is AtomUITextArea textArea)
            {
                var content = new Border();
                textArea.InnerRightContent = content;
                Dispatcher.UIThread.RunJobs();

                FindTemplatePart<ContentPresenter>(input, "PART_InnerRightContentPresenter")
                    .Content.ShouldBeSameAs(content);
            }

            clearButton.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();
            input.Text.ShouldBeNullOrEmpty($"{inputType.Name} clear interaction must remain connected after reattach.");

            var presenter = FindTemplatePart<TextPresenter>(input, "PART_TextPresenter");
            var placeholder = FindTemplatePart<AtomUI.Desktop.Controls.TextBlock>(input, "Placeholder");
            presenter.SetCurrentValue(TextPresenter.PreeditTextProperty, "preedit");
            Dispatcher.UIThread.RunJobs();
            placeholder.IsVisible.ShouldBeFalse($"{inputType.Name} IME preedit tracking must remain connected after reattach.");
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData(typeof(AtomUI.Desktop.Controls.AutoComplete))]
    [InlineData(typeof(AutoCompleteSearchEdit))]
    [InlineData(typeof(AutoCompleteTextArea))]
    public void AutoComplete_Internal_Input_Remains_Connected_After_Reattach(Type autoCompleteType)
    {
        var autoComplete = (Control)Activator.CreateInstance(autoCompleteType)!;
        autoComplete.Width = 240;
        var window = CreateWindow(autoComplete);

        try
        {
            var input = autoComplete.GetVisualDescendants().OfType<AbstractTextInput>().Single();
            FindOptionalTemplatePart<RevealButton>(input, "PART_RevealButton")?.IsVisible.ShouldBeFalse();

            Reattach(window, autoComplete);

            input = autoComplete.GetVisualDescendants().OfType<AbstractTextInput>().Single();
            FindOptionalTemplatePart<RevealButton>(input, "PART_RevealButton")?.IsVisible.ShouldBeFalse(
                $"{autoCompleteType.Name} must not expose a reveal button after its internal input is reattached.");

            input.IsAllowClear = true;
            input.Text = "value";
            Dispatcher.UIThread.RunJobs();

            var clearButton = FindTemplatePart<InputClearIconButton>(input, "PART_ClearButton");
            clearButton.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();
            input.Text.ShouldBeNullOrEmpty(
                $"{autoCompleteType.Name} internal clear interaction must remain connected after reattach.");
        }
        finally
        {
            window.Close();
        }
    }

    [Theory]
    [InlineData("LineEdit")]
    [InlineData("OtpLineEdit")]
    [InlineData("ComboBox")]
    [InlineData("SelectHandle")]
    [InlineData("PickerClearUpButton")]
    public void Feedback_Subscriptions_Reconnect_After_Reattach(string controlKind)
    {
        var feedback = new FormValidateFeedback();
        var lifecycleCase = CreateFeedbackLifecycleCase(controlKind, feedback);
        var window = CreateWindow(lifecycleCase.Control);

        try
        {
            lifecycleCase.IsFeedbackVisible().ShouldBeFalse();

            window.Content = null;
            Dispatcher.UIThread.RunJobs();
            feedback.ValidateStatus = FormValidateStatus.Warning;
            window.Content = lifecycleCase.Control;
            Dispatcher.UIThread.RunJobs();

            lifecycleCase.IsFeedbackVisible().ShouldBeTrue(
                $"{controlKind} must reconnect to the current feedback state when reattached.");

            feedback.ValidateStatus = FormValidateStatus.Default;
            Dispatcher.UIThread.RunJobs();
            lifecycleCase.IsFeedbackVisible().ShouldBeFalse(
                $"{controlKind} must continue observing feedback changes after reattach.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void OtpLineEdit_Clear_Interaction_Remains_Connected_After_Reattach()
    {
        var otp = new AtomUIOtpLineEdit
        {
            Width        = 240,
            Text         = "123456",
            IsAllowClear = true
        };
        var window = CreateWindow(otp);

        try
        {
            Reattach(window, otp);

            var clearButton = FindTemplatePart<InputClearIconButton>(otp, "PART_ClearButton");
            clearButton.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));
            Dispatcher.UIThread.RunJobs();

            otp.Text.ShouldBeNullOrEmpty("OtpLineEdit clear interaction must remain connected after reattach.");
        }
        finally
        {
            window.Close();
        }
    }

    private static AbstractTextInput CreateTextInput(Type inputType)
    {
        var input = (AbstractTextInput)Activator.CreateInstance(inputType)!;
        input.Width                = 240;
        input.Height               = input is AtomUITextArea ? 100 : double.NaN;
        input.IsAllowClear         = true;
        input.IsEnableRevealButton = false;
        input.IsMotionEnabled      = false;
        return input;
    }

    private static FeedbackLifecycleCase CreateFeedbackLifecycleCase(
        string controlKind,
        FormValidateFeedback feedback)
    {
        return controlKind switch
        {
            "LineEdit" => CreateFormFeedbackLifecycleCase(new AtomUILineEdit { Width = 240 }, feedback),
            "OtpLineEdit" => CreateFormFeedbackLifecycleCase(new AtomUIOtpLineEdit { Width = 240 }, feedback),
            "ComboBox" => CreateFormFeedbackLifecycleCase(
                new AtomUI.Desktop.Controls.ComboBox { Width = 240 }, feedback),
            "SelectHandle" => CreateSelectHandleFeedbackLifecycleCase(feedback),
            "PickerClearUpButton" => CreatePickerClearUpButtonFeedbackLifecycleCase(feedback),
            _ => throw new ArgumentOutOfRangeException(nameof(controlKind), controlKind, null)
        };
    }

    private static FeedbackLifecycleCase CreateFormFeedbackLifecycleCase(
        Control control,
        FormValidateFeedback feedback)
    {
        ((IFormItemFeedbackAware)control).SetFeedbackControl(feedback);
        return control switch
        {
            AbstractTextInput input => new(control, () => input.IsFormFeedbackVisible),
            AtomUIOtpLineEdit otp => new(control, () => otp.IsFormFeedbackVisible),
            AtomUI.Desktop.Controls.ComboBox comboBox => new(control, () => comboBox.IsFormFeedbackVisible),
            _ => throw new ArgumentOutOfRangeException(nameof(control), control.GetType(), null)
        };
    }

    private static FeedbackLifecycleCase CreateSelectHandleFeedbackLifecycleCase(FormValidateFeedback feedback)
    {
        var control = new SelectHandle
        {
            Width        = 240,
            FormFeedback = feedback
        };
        return new(control, () => control.IsFormFeedbackVisible);
    }

    private static FeedbackLifecycleCase CreatePickerClearUpButtonFeedbackLifecycleCase(FormValidateFeedback feedback)
    {
        var control = new PickerClearUpButton
        {
            FormFeedback = feedback
        };
        return new(control, () => control.IsFormFeedbackVisible);
    }

    private static AvaloniaWindow CreateWindow(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 240,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void Reattach(AvaloniaWindow window, Control control)
    {
        window.Content = null;
        Dispatcher.UIThread.RunJobs();
        window.Content = control;
        Dispatcher.UIThread.RunJobs();
    }

    private static T FindTemplatePart<T>(Control control, string name)
        where T : Control
    {
        return control.GetVisualDescendants()
                      .OfType<T>()
                      .Single(item => item.Name == name);
    }

    private static T? FindOptionalTemplatePart<T>(Control control, string name)
        where T : Control
    {
        return control.GetVisualDescendants()
                      .OfType<T>()
                      .SingleOrDefault(item => item.Name == name);
    }

    private sealed record FeedbackLifecycleCase(Control Control, Func<bool> IsFeedbackVisible);
}
