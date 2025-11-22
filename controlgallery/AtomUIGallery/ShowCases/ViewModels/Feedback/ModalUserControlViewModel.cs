using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class ModalUserControlViewModel : ReactiveObject
{
    private string? _name;

    public string? Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    private int _age;

    public int Age
    {
        get => _age;
        set => this.RaiseAndSetIfChanged(ref _age, value);
    }
}