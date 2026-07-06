using System.ComponentModel;
using System.Reflection;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomColorPicker = AtomUI.Desktop.Controls.ColorPicker;
using AtomGradientColorPicker = AtomUI.Desktop.Controls.GradientColorPicker;
using AtomMentions = AtomUI.Desktop.Controls.Mentions;
using AtomSlider = AtomUI.Desktop.Controls.Slider;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.SelectionSemantics;

public class RemainingFormValueBindingTests
{
    static RemainingFormValueBindingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Remaining_Form_Value_Properties_Are_TwoWay_And_DataValidation_Enabled()
    {
        AssertTwoWayAndDataValidation(AtomSlider.RangeValueProperty, typeof(AtomSlider));
        AssertTwoWayAndDataValidation(AtomColorPicker.ValueProperty, typeof(AtomColorPicker));
        AssertTwoWayAndDataValidation(AtomGradientColorPicker.ValueProperty, typeof(AtomGradientColorPicker));
        AssertTwoWayAndDataValidation(AtomMentions.ValueProperty, typeof(AtomMentions));
    }

    [Fact]
    public void Slider_RangeValue_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new RemainingFormValueBindingViewModel
        {
            RangeValue = new SliderRangeValue { StartValue = 10, EndValue = 30 }
        };
        var slider = new AtomSlider
        {
            IsRangeMode = true
        };
        slider.Bind(
            AtomSlider.RangeValueProperty,
            new Binding(nameof(RemainingFormValueBindingViewModel.RangeValue))
            {
                Source = viewModel
            });

        ShowInWindow(slider, () =>
        {
            slider.RangeValue.ShouldBe(viewModel.RangeValue);

            var updated = new SliderRangeValue { StartValue = 20, EndValue = 80 };
            slider.RangeValue = updated;
            Dispatcher.UIThread.RunJobs();

            viewModel.RangeValue.ShouldBe(updated);
        });
    }

    [Fact]
    public void Slider_RangeValue_DataValidationError_Is_Written_To_DataValidationErrors()
    {
        var slider = new DataValidationProbeSlider
        {
            IsRangeMode = true
        };
        var validationError = new InvalidOperationException("Range is required");

        slider.ApplyDataValidation(AtomSlider.RangeValueProperty, validationError);

        DataValidationErrors.GetHasErrors(slider).ShouldBeTrue();
        DataValidationErrors.GetErrors(slider).ShouldBe([validationError]);

        slider.ApplyDataValidation(AtomSlider.RangeValueProperty, null);

        DataValidationErrors.GetHasErrors(slider).ShouldBeFalse();
    }

    [Fact]
    public void ColorPicker_Value_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new RemainingFormValueBindingViewModel
        {
            ColorValue = Colors.Red
        };
        var colorPicker = new AtomColorPicker();
        colorPicker.Bind(
            AtomColorPicker.ValueProperty,
            new Binding(nameof(RemainingFormValueBindingViewModel.ColorValue))
            {
                Source = viewModel
            });

        Dispatcher.UIThread.RunJobs();
        colorPicker.Value.ShouldBe(Colors.Red);

        colorPicker.SetCurrentValue(AtomColorPicker.ValueProperty, Colors.Blue);
        Dispatcher.UIThread.RunJobs();

        viewModel.ColorValue.ShouldBe(Colors.Blue);
    }

    [Fact]
    public void ColorPicker_Value_Clr_Wrappers_Are_Publicly_Settable()
    {
        AssertPublicSetter(typeof(AtomColorPicker), nameof(AtomColorPicker.Value));
        AssertPublicSetter(typeof(AtomGradientColorPicker), nameof(AtomGradientColorPicker.Value));
    }

    [Fact]
    public void ColorPicker_FormValue_Clear_Sets_Value_To_Null()
    {
        var colorPicker = new AtomColorPicker();
        var formItemAware = (IFormItemAware)colorPicker;

        formItemAware.SetFormValue(Colors.Red);
        formItemAware.GetFormValue().ShouldBe(Colors.Red);

        formItemAware.ClearFormValue();

        colorPicker.Value.ShouldBeNull();
        formItemAware.GetFormValue().ShouldBeNull();
    }

    [Fact]
    public void GradientColorPicker_Value_DefaultBindingMode_Updates_ViewModel()
    {
        var initial = CreateGradient(Colors.Red, Colors.Blue);
        var viewModel = new RemainingFormValueBindingViewModel
        {
            GradientValue = initial
        };
        var colorPicker = new AtomGradientColorPicker();
        colorPicker.Bind(
            AtomGradientColorPicker.ValueProperty,
            new Binding(nameof(RemainingFormValueBindingViewModel.GradientValue))
            {
                Source = viewModel
            });

        Dispatcher.UIThread.RunJobs();
        colorPicker.Value.ShouldBeSameAs(initial);

        var updated = CreateGradient(Colors.Green, Colors.Yellow);
        colorPicker.SetCurrentValue(AtomGradientColorPicker.ValueProperty, updated);
        Dispatcher.UIThread.RunJobs();

        viewModel.GradientValue.ShouldBeSameAs(updated);
    }

    [Fact]
    public void GradientColorPicker_FormValue_Clear_Sets_Value_To_Null()
    {
        var initial = CreateGradient(Colors.Red, Colors.Blue);
        var colorPicker = new AtomGradientColorPicker();
        var formItemAware = (IFormItemAware)colorPicker;

        formItemAware.SetFormValue(initial);
        formItemAware.GetFormValue().ShouldBeSameAs(initial);

        formItemAware.ClearFormValue();

        colorPicker.Value.ShouldBeNull();
        formItemAware.GetFormValue().ShouldBeNull();
    }

    [Fact]
    public void Mentions_Value_DefaultBindingMode_Updates_ViewModel()
    {
        var viewModel = new RemainingFormValueBindingViewModel
        {
            MentionValue = "@atom"
        };
        var mentions = new AtomMentions();
        mentions.Bind(
            AtomMentions.ValueProperty,
            new Binding(nameof(RemainingFormValueBindingViewModel.MentionValue))
            {
                Source = viewModel
            });

        ShowInWindow(mentions, () =>
        {
            mentions.Value.ShouldBe("@atom");

            mentions.Value = "@atomui";
            Dispatcher.UIThread.RunJobs();

            viewModel.MentionValue.ShouldBe("@atomui");
        });
    }

    private static void AssertTwoWayAndDataValidation<TValue>(
        StyledProperty<TValue> property,
        Type ownerType)
    {
        var metadata = property.GetMetadata(ownerType);

        metadata.DefaultBindingMode.ShouldBe(BindingMode.TwoWay);
        metadata.EnableDataValidation.ShouldBe(true);
    }

    private static LinearGradientBrush CreateGradient(Color startColor, Color endColor)
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            GradientStops =
            [
                new GradientStop(startColor, 0),
                new GradientStop(endColor, 1)
            ]
        };
    }

    private static void AssertPublicSetter(Type ownerType, string propertyName)
    {
        var property = ownerType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        property.ShouldNotBeNull();
        property.SetMethod.ShouldNotBeNull();
        property.SetMethod!.IsPublic.ShouldBeTrue();
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

    private sealed class RemainingFormValueBindingViewModel : INotifyPropertyChanged
    {
        private SliderRangeValue _rangeValue;
        private Color? _colorValue;
        private LinearGradientBrush? _gradientValue;
        private string? _mentionValue;

        public SliderRangeValue RangeValue
        {
            get => _rangeValue;
            set
            {
                if (_rangeValue == value)
                {
                    return;
                }

                _rangeValue = value;
                RaisePropertyChanged(nameof(RangeValue));
            }
        }

        public Color? ColorValue
        {
            get => _colorValue;
            set
            {
                if (_colorValue == value)
                {
                    return;
                }

                _colorValue = value;
                RaisePropertyChanged(nameof(ColorValue));
            }
        }

        public LinearGradientBrush? GradientValue
        {
            get => _gradientValue;
            set
            {
                if (ReferenceEquals(_gradientValue, value))
                {
                    return;
                }

                _gradientValue = value;
                RaisePropertyChanged(nameof(GradientValue));
            }
        }

        public string? MentionValue
        {
            get => _mentionValue;
            set
            {
                if (_mentionValue == value)
                {
                    return;
                }

                _mentionValue = value;
                RaisePropertyChanged(nameof(MentionValue));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private sealed class DataValidationProbeSlider : AtomSlider
    {
        public void ApplyDataValidation(AvaloniaProperty property, Exception? error)
        {
            UpdateDataValidation(property, BindingValueType.BindingError, error);
        }
    }
}
