using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.DatePickers;

public class DatePickerBehaviorTests
{
    static DatePickerBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SelectedDateTime_Is_TwoWay_And_DataValidation_Enabled()
    {
        var metadata = Desktop.Controls.DatePicker.SelectedDateTimeProperty.GetMetadata(
            typeof(Desktop.Controls.DatePicker));

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        metadata.EnableDataValidation.ShouldBe(true);
    }

    [Fact]
    public void Date_Boundary_Properties_Default_To_Null_And_Are_Shared_With_RangeDatePicker()
    {
        var datePicker  = new Desktop.Controls.DatePicker();
        var rangePicker = new Desktop.Controls.RangeDatePicker();

        datePicker.MinDate.ShouldBeNull();
        datePicker.MaxDate.ShouldBeNull();
        rangePicker.MinDate.ShouldBeNull();
        rangePicker.MaxDate.ShouldBeNull();
        Desktop.Controls.RangeDatePicker.MinDateProperty.ShouldBeSameAs(
            Desktop.Controls.DatePicker.MinDateProperty);
        Desktop.Controls.RangeDatePicker.MaxDateProperty.ShouldBeSameAs(
            Desktop.Controls.DatePicker.MaxDateProperty);
    }

    [Fact]
    public void SelectedDateTime_DefaultBindingMode_Updates_ViewModel()
    {
        var initialDate = new DateTime(2026, 7, 5);
        var updatedDate = new DateTime(2026, 7, 6);
        var viewModel = new DatePickerBindingViewModel
        {
            SelectedDateTime = initialDate
        };
        var datePicker = new Desktop.Controls.DatePicker
        {
            Width = 240
        };
        datePicker.Bind(
            Desktop.Controls.DatePicker.SelectedDateTimeProperty,
            new Binding(nameof(DatePickerBindingViewModel.SelectedDateTime))
            {
                Source = viewModel
            });

        ShowInWindow(datePicker, () =>
        {
            datePicker.SelectedDateTime.ShouldBe(initialDate);

            datePicker.SelectedDateTime = updatedDate;
            Dispatcher.UIThread.RunJobs();

            viewModel.SelectedDateTime.ShouldBe(updatedDate);
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

    private sealed class DatePickerBindingViewModel : INotifyPropertyChanged
    {
        private DateTime? _selectedDateTime;

        public DateTime? SelectedDateTime
        {
            get => _selectedDateTime;
            set
            {
                if (_selectedDateTime == value)
                {
                    return;
                }

                _selectedDateTime = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedDateTime)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
