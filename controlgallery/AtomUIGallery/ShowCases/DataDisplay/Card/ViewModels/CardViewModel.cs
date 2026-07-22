using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Card;

public class CardViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Card";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private IBrush? _borderlessFrameBg;

    public IBrush? BorderlessFrameBg
    {
        get => _borderlessFrameBg;
        set => this.RaiseAndSetIfChanged(ref _borderlessFrameBg, value);
    }

    private bool _isLoading;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public CardViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
