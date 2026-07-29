using System.Windows.Input;
using Avalonia;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Tag;

public class CheckableTagTests
{
    static CheckableTagTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void CheckableTag_Uses_ToggleButton_Contract_Without_Ordinary_Tag_Apis()
    {
        var tag = new CheckableTag
        {
            Content         = "Movies",
            Icon            = new PathIcon(),
            IsMotionEnabled = true
        };

        tag.ShouldBeAssignableTo<ToggleButton>();
        tag.Content.ShouldBe("Movies");
        tag.Icon.ShouldNotBeNull();
        tag.IsMotionEnabled.ShouldBeTrue();
        tag.IsChecked.ShouldBe(false);
        CheckableTag.IsCheckedProperty
                    .GetMetadata(typeof(CheckableTag))
                    .DefaultBindingMode
                    .ShouldBe(BindingMode.TwoWay);

        typeof(AbstractCheckableTag).GetProperty("TagColor").ShouldBeNull();
        typeof(AbstractCheckableTag).GetProperty("Variant").ShouldBeNull();
        typeof(AbstractCheckableTag).GetProperty("IsClosable").ShouldBeNull();
        typeof(AbstractCheckableTag).GetProperty("CloseIcon").ShouldBeNull();
        typeof(AbstractCheckableTag).GetProperty("DefaultIsChecked").ShouldBeNull();
    }

    [Fact]
    public void CheckableTag_Exposes_A_Binary_Form_Value()
    {
        var tag = new CheckableTag();
        var formItem = (IFormItemAware)tag;
        var valueChangedCount = 0;
        formItem.ValueChanged += (_, _) => valueChangedCount++;

        formItem.GetFormValue().ShouldBe(false);

        formItem.SetFormValue(true);
        tag.IsChecked.ShouldBe(true);
        formItem.GetFormValue().ShouldBe(true);

        formItem.SetFormValue(null);
        tag.IsChecked.ShouldBe(false);
        formItem.GetFormValue().ShouldBe(false);

        tag.IsChecked = true;
        formItem.ClearFormValue();
        tag.IsChecked.ShouldBe(false);
        formItem.GetFormValue().ShouldBe(false);
        valueChangedCount.ShouldBe(4);
    }

    [Fact]
    public void Null_Is_Normalized_Without_Reporting_A_Change_When_Already_Unchecked()
    {
        var tag = new CheckableTag();
        var formItem = (IFormItemAware)tag;
        var valueChangedCount = 0;
        formItem.ValueChanged += (_, _) => valueChangedCount++;

        tag.IsChecked = null;

        tag.IsChecked.ShouldBe(false);
        valueChangedCount.ShouldBe(0);

        tag.IsChecked = true;
        tag.IsChecked = null;

        tag.IsChecked.ShouldBe(false);
        valueChangedCount.ShouldBe(2);
    }

    [Fact]
    public void Space_Toggles_The_Tag_And_Executes_Its_Command_Once()
    {
        var commandParameter = new object();
        var command = new RecordingCommand();
        var tag = new CheckableTag
        {
            Content          = "Movies",
            Command          = command,
            CommandParameter = commandParameter
        };

        ShowInWindow(tag, window =>
        {
            tag.Focus(NavigationMethod.Tab).ShouldBeTrue();
            window.KeyPress(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            window.KeyRelease(Key.Space, RawInputModifiers.None, PhysicalKey.Space, null);
            Dispatcher.UIThread.RunJobs();

            tag.IsChecked.ShouldBe(true);
            command.ExecuteCount.ShouldBe(1);
            command.LastParameter.ShouldBeSameAs(commandParameter);
        });
    }

    [Fact]
    public void Disabled_Tag_Rejects_Pointer_Activation_And_Command_Execution()
    {
        var command = new RecordingCommand();
        var tag = new CheckableTag
        {
            Content   = "Movies",
            Command   = command,
            IsEnabled = false
        };

        ShowInWindow(tag, window =>
        {
            var point = tag.TranslatePoint(
                new Point(tag.Bounds.Width / 2, tag.Bounds.Height / 2),
                window).ShouldNotBeNull();

            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            tag.IsChecked.ShouldBe(false);
            command.ExecuteCount.ShouldBe(0);
        });
    }

    private static void ShowInWindow(CheckableTag tag, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 160,
            Content = tag
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick(1);
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private sealed class RecordingCommand : ICommand
    {
        public int ExecuteCount { get; private set; }

        public object? LastParameter { get; private set; }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ExecuteCount++;
            LastParameter = parameter;
        }
    }
}
