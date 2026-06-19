using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Button;

public class ButtonViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "Button";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private CustomizableSizeType _buttonSizeType;
    private ObservableCollection<ButtonApiRow>? _apiRows;
    private ObservableCollection<ButtonDesignTokenRow>? _designTokenRows;

    public CustomizableSizeType ButtonSizeType
    {
        get => _buttonSizeType;
        set => this.RaiseAndSetIfChanged(ref _buttonSizeType, value);
    }

    public ObservableCollection<ButtonApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ButtonDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ButtonViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
        HostScreen = screen;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new ButtonApiRow("ButtonType", Lang(ButtonShowCaseLangResourceKind.ApiPropertyButtonType), "ButtonType", "blue", "Default"),
            new ButtonApiRow("SizeType", Lang(ButtonShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
            new ButtonApiRow("Shape", Lang(ButtonShowCaseLangResourceKind.ApiPropertyShape), "ButtonShape", "blue", "Default"),
            new ButtonApiRow("Icon", Lang(ButtonShowCaseLangResourceKind.ApiPropertyIcon), "Icon?", "cyan", "null"),
            new ButtonApiRow("IsLoading", Lang(ButtonShowCaseLangResourceKind.ApiPropertyLoading), "bool", "green", "false"),
            new ButtonApiRow("IsDanger", Lang(ButtonShowCaseLangResourceKind.ApiPropertyDanger), "bool", "green", "false"),
            new ButtonApiRow("Color", Lang(ButtonShowCaseLangResourceKind.ApiPropertyColor), "ButtonColor?", "purple", "null"),
            new ButtonApiRow("Variant", Lang(ButtonShowCaseLangResourceKind.ApiPropertyVariant), "ButtonVariant?", "purple", "null"),
            new ButtonApiRow("CustomBackground", Lang(ButtonShowCaseLangResourceKind.ApiPropertyCustomBackground), "IBrush?", "cyan", "null")
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
            new ButtonDesignTokenRow(
                "ColorPrimary",
                Lang(ButtonShowCaseLangResourceKind.TokenNameColorPrimary),
                Lang(ButtonShowCaseLangResourceKind.TokenScopeShared),
                "blue",
                Lang(ButtonShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new ButtonDesignTokenRow(
                "ControlHeight",
                Lang(ButtonShowCaseLangResourceKind.TokenNameControlHeight),
                Lang(ButtonShowCaseLangResourceKind.TokenScopeShared),
                "blue",
                Lang(ButtonShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new ButtonDesignTokenRow(
                "ButtonToken",
                Lang(ButtonShowCaseLangResourceKind.TokenNameButtonToken),
                Lang(ButtonShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(ButtonShowCaseLangResourceKind.TokenStatusMapped),
                "warning")
        ];
    }

    private static string Lang(ButtonShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ButtonShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ButtonShowCaseLangResourceKind.ApiPropertyButtonType       => en_US.ApiPropertyButtonType,
            ButtonShowCaseLangResourceKind.ApiPropertySizeType         => en_US.ApiPropertySizeType,
            ButtonShowCaseLangResourceKind.ApiPropertyShape            => en_US.ApiPropertyShape,
            ButtonShowCaseLangResourceKind.ApiPropertyIcon             => en_US.ApiPropertyIcon,
            ButtonShowCaseLangResourceKind.ApiPropertyLoading          => en_US.ApiPropertyLoading,
            ButtonShowCaseLangResourceKind.ApiPropertyDanger           => en_US.ApiPropertyDanger,
            ButtonShowCaseLangResourceKind.ApiPropertyColor            => en_US.ApiPropertyColor,
            ButtonShowCaseLangResourceKind.ApiPropertyVariant          => en_US.ApiPropertyVariant,
            ButtonShowCaseLangResourceKind.ApiPropertyCustomBackground => en_US.ApiPropertyCustomBackground,
            ButtonShowCaseLangResourceKind.TokenNameColorPrimary       => en_US.TokenNameColorPrimary,
            ButtonShowCaseLangResourceKind.TokenNameControlHeight      => en_US.TokenNameControlHeight,
            ButtonShowCaseLangResourceKind.TokenNameButtonToken        => en_US.TokenNameButtonToken,
            ButtonShowCaseLangResourceKind.TokenScopeShared            => en_US.TokenScopeShared,
            ButtonShowCaseLangResourceKind.TokenScopeComponent         => en_US.TokenScopeComponent,
            ButtonShowCaseLangResourceKind.TokenStatusStable           => en_US.TokenStatusStable,
            ButtonShowCaseLangResourceKind.TokenStatusMapped           => en_US.TokenStatusMapped,
            _                                                          => kind.ToString()
        };
    }
}

public sealed record ButtonApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ButtonDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
