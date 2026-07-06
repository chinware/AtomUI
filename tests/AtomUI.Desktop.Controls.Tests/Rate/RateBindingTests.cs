using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Rate;

public class RateBindingTests
{
    static RateBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Value_Is_TwoWay_And_DataValidation_Enabled()
    {
        var metadata = Desktop.Controls.Rate.ValueProperty.GetMetadata(typeof(Desktop.Controls.Rate));

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        metadata.EnableDataValidation.ShouldBe(true);
    }

    [Fact]
    public void Value_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new RateBindingViewModel
        {
            Value = 2.0
        };
        var rate = new Desktop.Controls.Rate();
        rate.Bind(
            Desktop.Controls.Rate.ValueProperty,
            new Binding(nameof(RateBindingViewModel.Value))
            {
                Source = viewModel
            });

        ShowInWindow(rate, () =>
        {
            rate.Value.ShouldBe(2.0);

            rate.Value = 4.0;
            Dispatcher.UIThread.RunJobs();

            viewModel.Value.ShouldBe(4.0);
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

    private sealed class RateBindingViewModel : INotifyPropertyChanged
    {
        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                if (Math.Abs(_value - value) < double.Epsilon)
                {
                    return;
                }

                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
