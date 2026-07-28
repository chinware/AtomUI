using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Drawer;

public class DrawerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Drawer";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private DrawerPlacement _multiLevelPlacement = DrawerPlacement.Right;

    public DrawerPlacement MultiLevelPlacement
    {
        get => _multiLevelPlacement;
        set => this.RaiseAndSetIfChanged(ref _multiLevelPlacement, value);
    }

    private DrawerPlacement _extraAndFooterPlacement = DrawerPlacement.Right;

    public DrawerPlacement ExtraAndFooterPlacement
    {
        get => _extraAndFooterPlacement;
        set => this.RaiseAndSetIfChanged(ref _extraAndFooterPlacement, value);
    }

    private DrawerPlacement _customPlacement = DrawerPlacement.Right;

    public DrawerPlacement CustomPlacement
    {
        get => _customPlacement;
        set => this.RaiseAndSetIfChanged(ref _customPlacement, value);
    }

    private List<ISelectOption>? _accountOwnerOptions;

    public List<ISelectOption>? AccountOwnerOptions
    {
        get => _accountOwnerOptions;
        set => this.RaiseAndSetIfChanged(ref _accountOwnerOptions, value);
    }

    private List<ISelectOption>? _accountTypeOptions;

    public List<ISelectOption>? AccountTypeOptions
    {
        get => _accountTypeOptions;
        set => this.RaiseAndSetIfChanged(ref _accountTypeOptions, value);
    }

    private List<ISelectOption>? _accountApproverOptions;

    public List<ISelectOption>? AccountApproverOptions
    {
        get => _accountApproverOptions;
        set => this.RaiseAndSetIfChanged(ref _accountApproverOptions, value);
    }

    public DrawerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

}
