using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Descriptions;

public class DescriptionsViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Descriptions";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private SizeType _descriptionsSizeType;

    public SizeType DescriptionsSizeType
    {
        get => _descriptionsSizeType;
        set => this.RaiseAndSetIfChanged(ref _descriptionsSizeType, value);
    }

    public DescriptionsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
