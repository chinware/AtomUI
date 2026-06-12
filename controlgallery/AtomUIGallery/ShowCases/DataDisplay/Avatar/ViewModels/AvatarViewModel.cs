using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Avatar;

public class AvatarViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Avatar";

    public ViewModelActivator Activator  { get; }
    public IScreen            HostScreen { get; }
    public string             UrlPathSegment { get; } = ID.ToString();

    private string? _avatarText;
    private ObservableCollection<AvatarApiRow>? _apiRows;
    private ObservableCollection<AvatarDesignTokenRow>? _designTokenRows;

    public string? AvatarText
    {
        get => _avatarText;
        set => this.RaiseAndSetIfChanged(ref _avatarText, value);
    }

    private double? _avatarGap;

    public double? AvatarGap
    {
        get => _avatarGap;
        set => this.RaiseAndSetIfChanged(ref _avatarGap, value);
    }

    private string? _avatarBackground;

    public string? AvatarBackground
    {
        get => _avatarBackground;
        set => this.RaiseAndSetIfChanged(ref _avatarBackground, value);
    }

    public ObservableCollection<AvatarApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<AvatarDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private int _textCurrentIndex = 0;
    private int _gapCurrentIndex  = 0;

    private readonly List<string> _userList;
    private readonly List<string> _colorList;
    private readonly List<double> _gapList;

    public ReactiveCommand<Unit, Unit> ChangeUserCommand { get; }
    public ReactiveCommand<Unit, Unit> ChangeGapCommand  { get; }

    public AvatarViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
        HostScreen = screen;

        _userList  = ["U", "Lucy", "Tom", "Edward"];
        _colorList = ["#f56a00", "#7265e6", "#ffbf00", "#00a2ae"];
        _gapList   = [4, 3, 2, 1];

        ChangeUserCommand = ReactiveCommand.Create(SetupAvatarText);
        ChangeGapCommand  = ReactiveCommand.Create(SetupAvatarGap);

        Activator.Activated.Subscribe(_ =>
        {
            SetupAvatarText();
            SetupAvatarGap();
        });
    }

    private void SetupAvatarText()
    {
        var index        = (_textCurrentIndex++) % 4;
        AvatarText       = _userList[index];
        AvatarBackground = _colorList[index];
    }

    private void SetupAvatarGap()
    {
        var index = (_gapCurrentIndex++) % 4;
        AvatarGap = _gapList[index];
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new AvatarApiRow("Shape", Lang(AvatarShowCaseLangResourceKind.ApiPropertyShape), "AvatarShape", "blue", "Circle"),
            new AvatarApiRow("Size", Lang(AvatarShowCaseLangResourceKind.ApiPropertySize), "double", "green", "NaN"),
            new AvatarApiRow("SizeType", Lang(AvatarShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
            new AvatarApiRow("Icon", Lang(AvatarShowCaseLangResourceKind.ApiPropertyIcon), "PathIcon?", "cyan", "null"),
            new AvatarApiRow("Src", Lang(AvatarShowCaseLangResourceKind.ApiPropertySrc), "string?", "cyan", "null"),
            new AvatarApiRow("Text", Lang(AvatarShowCaseLangResourceKind.ApiPropertyText), "string?", "cyan", "null"),
            new AvatarApiRow("Gap", Lang(AvatarShowCaseLangResourceKind.ApiPropertyGap), "double", "green", "4"),
            new AvatarApiRow("MaxDisplayCount", Lang(AvatarShowCaseLangResourceKind.ApiPropertyMaxDisplayCount), "int?", "green", "null")
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
            new AvatarDesignTokenRow(
                "AvatarToken",
                Lang(AvatarShowCaseLangResourceKind.TokenNameAvatarToken),
                Lang(AvatarShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(AvatarShowCaseLangResourceKind.TokenStatusMapped),
                "warning"),
            new AvatarDesignTokenRow(
                "ContainerSize",
                Lang(AvatarShowCaseLangResourceKind.TokenNameContainerSize),
                Lang(AvatarShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(AvatarShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new AvatarDesignTokenRow(
                "GroupSpace",
                Lang(AvatarShowCaseLangResourceKind.TokenNameGroupSpace),
                Lang(AvatarShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(AvatarShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new AvatarDesignTokenRow(
                "AvatarColor",
                Lang(AvatarShowCaseLangResourceKind.TokenNameAvatarColor),
                Lang(AvatarShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(AvatarShowCaseLangResourceKind.TokenStatusStable),
                "success")
        ];
    }

    private static string Lang(AvatarShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(AvatarShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            AvatarShowCaseLangResourceKind.ApiPropertyShape           => en_US.ApiPropertyShape,
            AvatarShowCaseLangResourceKind.ApiPropertySize            => en_US.ApiPropertySize,
            AvatarShowCaseLangResourceKind.ApiPropertySizeType        => en_US.ApiPropertySizeType,
            AvatarShowCaseLangResourceKind.ApiPropertyIcon            => en_US.ApiPropertyIcon,
            AvatarShowCaseLangResourceKind.ApiPropertySrc             => en_US.ApiPropertySrc,
            AvatarShowCaseLangResourceKind.ApiPropertyText            => en_US.ApiPropertyText,
            AvatarShowCaseLangResourceKind.ApiPropertyGap             => en_US.ApiPropertyGap,
            AvatarShowCaseLangResourceKind.ApiPropertyMaxDisplayCount => en_US.ApiPropertyMaxDisplayCount,
            AvatarShowCaseLangResourceKind.TokenNameAvatarToken       => en_US.TokenNameAvatarToken,
            AvatarShowCaseLangResourceKind.TokenNameContainerSize     => en_US.TokenNameContainerSize,
            AvatarShowCaseLangResourceKind.TokenNameGroupSpace        => en_US.TokenNameGroupSpace,
            AvatarShowCaseLangResourceKind.TokenNameAvatarColor       => en_US.TokenNameAvatarColor,
            AvatarShowCaseLangResourceKind.TokenScopeComponent        => en_US.TokenScopeComponent,
            AvatarShowCaseLangResourceKind.TokenStatusStable          => en_US.TokenStatusStable,
            AvatarShowCaseLangResourceKind.TokenStatusMapped          => en_US.TokenStatusMapped,
            _                                                         => kind.ToString()
        };
    }
}

public sealed record AvatarApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record AvatarDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
