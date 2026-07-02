using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Subjects;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.QRCode;

public class QRCodeViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "QRCode";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();
    private const double MinSize = 48;
    private const double MaxSize = 300;

    private string _qrCodeInput = "https://atomui.net";

    public string QRCodeInput
    {
        get => _qrCodeInput;
        set => this.RaiseAndSetIfChanged(ref _qrCodeInput, value);
    }

    private int _size = 160;

    public int Size
    {
        get => _size;
        set
        {
            if (_size == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _size, value);
            this.RaisePropertyChanged(nameof(IconSize));
            UpdateSizeCommandState();
        }
    }

    private readonly BehaviorSubject<bool> _smallerCanExecute;
    private readonly BehaviorSubject<bool> _largerCanExecute;

    public int IconSize => Size / 4;

    private IList<QRCodeEccLevel>? _eccLevels;

    public IList<QRCodeEccLevel>? EccLevels
    {
        get => _eccLevels;
        set => this.RaiseAndSetIfChanged(ref _eccLevels, value);
    }

    private QRCodeEccLevel _selectedEccLevel = QRCodeEccLevel.M;

    public QRCodeEccLevel SelectedEccLevel
    {
        get => _selectedEccLevel;
        set => this.RaiseAndSetIfChanged(ref _selectedEccLevel, value);
    }

    public ReactiveCommand<Unit, Unit> SmallerCommand { get; }
    public ReactiveCommand<Unit, Unit> LargerCommand { get; }

    private ObservableCollection<QRCodeApiRow>? _apiRows;
    private ObservableCollection<QRCodeDesignTokenRow>? _designTokenRows;

    public ObservableCollection<QRCodeApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<QRCodeDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public QRCodeViewModel(IScreen screen)
    {
        HostScreen = screen;
        _smallerCanExecute = new BehaviorSubject<bool>(CanDecreaseSize());
        _largerCanExecute  = new BehaviorSubject<bool>(CanIncreaseSize());
        SmallerCommand     = ReactiveCommand.Create(() => { Size -= 10; }, _smallerCanExecute);
        LargerCommand      = ReactiveCommand.Create(() => { Size += 10; }, _largerCanExecute);
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new QRCodeApiRow("Value", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyValue), "string", "cyan", "null"),
            new QRCodeApiRow("IsBordered", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyIsBordered), "bool", "green", "true"),
            new QRCodeApiRow("Color", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyColor), "IBrush?", "cyan", "QRCodeTextColor"),
            new QRCodeApiRow("Size", Lang(QRCodeShowCaseLangResourceKind.ApiPropertySize), "int", "green", "160"),
            new QRCodeApiRow("EccLevel", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyEccLevel), "QRCodeEccLevel", "blue", "M"),
            new QRCodeApiRow("IconSize", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyIconSize), "int", "green", "40"),
            new QRCodeApiRow("Icon", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyIcon), "IImage?", "cyan", "null"),
            new QRCodeApiRow("IconBgColor", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyIconBgColor), "IBrush?", "cyan", "ColorBgContainer"),
            new QRCodeApiRow("Status", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyStatus), "QRCodeStatus", "blue", "Active"),
            new QRCodeApiRow("LoadingContent", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyLoadingContent), "object?", "cyan", "null"),
            new QRCodeApiRow("ExpiredContent", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyExpiredContent), "object?", "cyan", "null"),
            new QRCodeApiRow("ScannedContent", Lang(QRCodeShowCaseLangResourceKind.ApiPropertyScannedContent), "object?", "cyan", "null"),
            new QRCodeApiRow("RefreshRequested", Lang(QRCodeShowCaseLangResourceKind.ApiEventRefreshRequested), "event EventHandler?", "purple", "-")
        ];
    }

    public void EnsureDesignTokenRows()
    {
        if (DesignTokenRows is not null)
        {
            return;
        }

        DesignTokenRows =
        [
            new QRCodeDesignTokenRow("QRCodeTextColor", Lang(QRCodeShowCaseLangResourceKind.TokenNameQRCodeTextColor), Lang(QRCodeShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(QRCodeShowCaseLangResourceKind.TokenStatusStable), "success"),
            new QRCodeDesignTokenRow("QRCodeMaskBackgroundColor", Lang(QRCodeShowCaseLangResourceKind.TokenNameQRCodeMaskBackgroundColor), Lang(QRCodeShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(QRCodeShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(QRCodeShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(QRCodeShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            QRCodeShowCaseLangResourceKind.ApiPropertyValue                    => en_US.ApiPropertyValue,
            QRCodeShowCaseLangResourceKind.ApiPropertyIsBordered               => en_US.ApiPropertyIsBordered,
            QRCodeShowCaseLangResourceKind.ApiPropertyColor                    => en_US.ApiPropertyColor,
            QRCodeShowCaseLangResourceKind.ApiPropertySize                     => en_US.ApiPropertySize,
            QRCodeShowCaseLangResourceKind.ApiPropertyEccLevel                 => en_US.ApiPropertyEccLevel,
            QRCodeShowCaseLangResourceKind.ApiPropertyIconSize                 => en_US.ApiPropertyIconSize,
            QRCodeShowCaseLangResourceKind.ApiPropertyIcon                     => en_US.ApiPropertyIcon,
            QRCodeShowCaseLangResourceKind.ApiPropertyIconBgColor              => en_US.ApiPropertyIconBgColor,
            QRCodeShowCaseLangResourceKind.ApiPropertyStatus                   => en_US.ApiPropertyStatus,
            QRCodeShowCaseLangResourceKind.ApiPropertyLoadingContent           => en_US.ApiPropertyLoadingContent,
            QRCodeShowCaseLangResourceKind.ApiPropertyExpiredContent           => en_US.ApiPropertyExpiredContent,
            QRCodeShowCaseLangResourceKind.ApiPropertyScannedContent           => en_US.ApiPropertyScannedContent,
            QRCodeShowCaseLangResourceKind.ApiEventRefreshRequested            => en_US.ApiEventRefreshRequested,
            QRCodeShowCaseLangResourceKind.TokenNameQRCodeTextColor            => en_US.TokenNameQRCodeTextColor,
            QRCodeShowCaseLangResourceKind.TokenNameQRCodeMaskBackgroundColor  => en_US.TokenNameQRCodeMaskBackgroundColor,
            QRCodeShowCaseLangResourceKind.TokenScopeComponent                 => en_US.TokenScopeComponent,
            QRCodeShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            _                                                                  => kind.ToString()
        };
    }

    private bool CanDecreaseSize()
    {
        return Size > MinSize;
    }

    private bool CanIncreaseSize()
    {
        return Size < MaxSize;
    }

    private void UpdateSizeCommandState()
    {
        _smallerCanExecute.OnNext(CanDecreaseSize());
        _largerCanExecute.OnNext(CanIncreaseSize());
    }
}

public sealed record QRCodeApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record QRCodeDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
