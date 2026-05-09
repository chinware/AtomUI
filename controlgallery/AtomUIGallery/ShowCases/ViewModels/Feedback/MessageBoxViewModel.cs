using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class MessageBoxViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "MessageBox";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private DialogHostType _messageBoxStyleCaseHostType;

    public DialogHostType MessageBoxStyleCaseHostType
    {
        get => _messageBoxStyleCaseHostType;
        set => this.RaiseAndSetIfChanged(ref _messageBoxStyleCaseHostType, value);
    }

    private bool _isConfirmMsgBoxOpened;

    public bool IsConfirmMsgBoxOpened
    {
        get => _isConfirmMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isConfirmMsgBoxOpened, value);
    }

    private bool _isInformationMsgBoxOpened;

    public bool IsInformationMsgBoxOpened
    {
        get => _isInformationMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isInformationMsgBoxOpened, value);
    }

    private bool _isSuccessMsgBoxOpened;

    public bool IsSuccessMsgBoxOpened
    {
        get => _isSuccessMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isSuccessMsgBoxOpened, value);
    }

    private bool _isErrorMsgBoxOpened;

    public bool IsErrorMsgBoxOpened
    {
        get => _isErrorMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isErrorMsgBoxOpened, value);
    }

    private bool _isWarningMsgBoxOpened;

    public bool IsWarningMsgBoxOpened
    {
        get => _isWarningMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isWarningMsgBoxOpened, value);
    }

    private bool _isCustomFooterMsgBoxOpened;

    public bool IsCustomFooterMsgBoxOpened
    {
        get => _isCustomFooterMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isCustomFooterMsgBoxOpened, value);
    }

    private bool _isDelayedCloseMsgBoxOpened;

    public bool IsDelayedCloseMsgBoxOpened
    {
        get => _isDelayedCloseMsgBoxOpened;
        set => this.RaiseAndSetIfChanged(ref _isDelayedCloseMsgBoxOpened, value);
    }

    private int _countdownSeconds;

    public int CountdownSeconds
    {
        get => _countdownSeconds;
        set => this.RaiseAndSetIfChanged(ref _countdownSeconds, value);
    }

    public MessageBoxViewModel(IScreen screen)
    {
        HostScreen = screen;
    }
}
