using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.TimePickers;

public class TimePickerBehaviorTests
{
    static TimePickerBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SelectedTime_Is_TwoWay_And_DataValidation_Enabled()
    {
        var metadata = TimePicker.SelectedTimeProperty.GetMetadata(typeof(TimePicker));

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        metadata.EnableDataValidation.ShouldBe(true);
    }

    [Fact]
    public void SelectedTime_DefaultBindingMode_Updates_ViewModel()
    {
        var initialTime = new TimeSpan(10, 9, 20);
        var updatedTime = new TimeSpan(12, 12, 20);
        var viewModel = new TimePickerBindingViewModel
        {
            SelectedTime = initialTime
        };
        var timePicker = new TimePicker
        {
            Width = 240
        };
        timePicker.Bind(
            TimePicker.SelectedTimeProperty,
            new Binding(nameof(TimePickerBindingViewModel.SelectedTime))
            {
                Source = viewModel
            });

        ShowInWindow(timePicker, () =>
        {
            timePicker.SelectedTime.ShouldBe(initialTime);

            timePicker.SelectedTime = updatedTime;
            Dispatcher.UIThread.RunJobs();

            viewModel.SelectedTime.ShouldBe(updatedTime);
        });
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 240,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private sealed class TimePickerBindingViewModel : INotifyPropertyChanged
    {
        private TimeSpan? _selectedTime;

        public TimeSpan? SelectedTime
        {
            get => _selectedTime;
            set
            {
                if (_selectedTime == value)
                {
                    return;
                }

                _selectedTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedTime)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
