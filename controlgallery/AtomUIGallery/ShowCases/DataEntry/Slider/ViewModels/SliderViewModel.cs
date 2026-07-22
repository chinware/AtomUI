using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Slider;

public class SliderViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Slider";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private List<SliderMark>? _sliderMarks;
    private SliderRangeValue _boundRangeValue = new()
    {
        StartValue = 20,
        EndValue   = 60
    };

    public List<SliderMark>? SliderMarks
    {
        get => _sliderMarks;
        set => this.RaiseAndSetIfChanged(ref _sliderMarks, value);
    }

    private bool _normalEnabled = true;

    public bool NormalEnabled
    {
        get => _normalEnabled;
        set => this.RaiseAndSetIfChanged(ref _normalEnabled, value);
    }

    public SliderRangeValue BoundRangeValue
    {
        get => _boundRangeValue;
        set
        {
            if (_boundRangeValue == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _boundRangeValue, value);
            this.RaisePropertyChanged(nameof(BoundRangeValueText));
        }
    }

    public string BoundRangeValueText => string.Format(
        CultureInfo.CurrentCulture,
        "{0:0.#} - {1:0.#}",
        BoundRangeValue.StartValue,
        BoundRangeValue.EndValue);

    public ReactiveCommand<Unit, Unit> SetBoundRangeValueCommand { get; }

    public ReactiveCommand<Unit, Unit> ClearBoundRangeValueCommand { get; }

    public SliderViewModel(IScreen screen)
    {
        HostScreen                   = screen;
        SetBoundRangeValueCommand    = ReactiveCommand.Create(SetBoundRangeValue);
        ClearBoundRangeValueCommand  = ReactiveCommand.Create(ClearBoundRangeValue);
    }

    private void SetBoundRangeValue()
    {
        BoundRangeValue = new SliderRangeValue
        {
            StartValue = 35,
            EndValue   = 85
        };
    }

    private void ClearBoundRangeValue()
    {
        BoundRangeValue = default;
    }

}
