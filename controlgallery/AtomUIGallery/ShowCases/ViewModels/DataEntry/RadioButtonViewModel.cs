using System.Collections.Generic;
using System.Reactive;
using AtomUI.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class RadioButtonViewModel : ReactiveObject, IRoutableViewModel
{
    public const string ID = "RadioButton";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID;

    private IList<RadioButtonOption>? _radioOptions;

    public IList<RadioButtonOption>? RadioOptions
    {
        get => _radioOptions;
        set => this.RaiseAndSetIfChanged(ref _radioOptions, value);
    }

    private bool _toggleDisabledRadioUnCheckedEnabled;

    public bool ToggleDisabledRadioUnCheckedEnabled
    {
        get => _toggleDisabledRadioUnCheckedEnabled;
        set => this.RaiseAndSetIfChanged(ref _toggleDisabledRadioUnCheckedEnabled, value);
    }

    private bool _toggleDisabledRadioCheckedEnabled;

    public bool ToggleDisabledRadioCheckedEnabled
    {
        get => _toggleDisabledRadioCheckedEnabled;
        set => this.RaiseAndSetIfChanged(ref _toggleDisabledRadioCheckedEnabled, value);
    }

    public ReactiveCommand<Unit, Unit> ToggleDisabledCommand { get; }

    public RadioButtonViewModel(IScreen screen)
    {
        HostScreen = screen;

        _toggleDisabledRadioUnCheckedEnabled = true;
        _toggleDisabledRadioCheckedEnabled = true;

        ToggleDisabledCommand = ReactiveCommand.Create(HandleToggleDisabled);
    }

    private void HandleToggleDisabled()
    {
        ToggleDisabledRadioUnCheckedEnabled = !ToggleDisabledRadioUnCheckedEnabled;
        ToggleDisabledRadioCheckedEnabled = !ToggleDisabledRadioCheckedEnabled;
    }
}
