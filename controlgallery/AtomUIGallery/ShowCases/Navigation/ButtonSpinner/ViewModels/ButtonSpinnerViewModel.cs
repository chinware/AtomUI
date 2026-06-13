using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using TextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

public class ButtonSpinnerViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "ButtonSpinner";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }
    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<ButtonSpinnerApiRow>? _apiRows;
    private ObservableCollection<ButtonSpinnerDesignTokenRow>? _designTokenRows;

    public ObservableCollection<ButtonSpinnerApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ButtonSpinnerDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ButtonSpinnerViewModel(IScreen screen)
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
            new ButtonSpinnerApiRow("IsSpinEnabled", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyIsSpinEnabled), "bool", "purple", "true"),
            new ButtonSpinnerApiRow("IsButtonSpinnerVisible", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyIsButtonSpinnerVisible), "bool", "purple", "true"),
            new ButtonSpinnerApiRow("ButtonSpinnerLocation", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyButtonSpinnerLocation), "ButtonSpinnerLocation", "blue", "Right"),
            new ButtonSpinnerApiRow("LeftAddOn", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyLeftAddOn), "object?", "cyan", "null"),
            new ButtonSpinnerApiRow("RightAddOn", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyRightAddOn), "object?", "cyan", "null"),
            new ButtonSpinnerApiRow("InnerLeftContent", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyInnerLeftContent), "object?", "cyan", "null"),
            new ButtonSpinnerApiRow("InnerRightContent", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyInnerRightContent), "object?", "cyan", "null"),
            new ButtonSpinnerApiRow("SizeType", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new ButtonSpinnerApiRow("StyleVariant", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyStyleVariant), "InputControlStyleVariant", "blue", "Outlined"),
            new ButtonSpinnerApiRow("Status", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyStatus), "InputControlStatus", "blue", "Default"),
            new ButtonSpinnerApiRow("IsButtonSpinnerFloatable", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyIsButtonSpinnerFloatable), "bool", "purple", "false"),
            new ButtonSpinnerApiRow("IsMotionEnabled", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token"),
            new ButtonSpinnerApiRow("SpinnerHandleWidth", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertySpinnerHandleWidth), "double", "cyan", "token"),
            new ButtonSpinnerApiRow("Spin", Lang(ButtonSpinnerShowCaseLangResourceKind.ApiPropertySpin), "event", "default", "null")
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
            new ButtonSpinnerDesignTokenRow("ControlWidth", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameControlWidth), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("HandleWidth", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleWidth), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("HandleIconSize", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleIconSize), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("HandleBg", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleBg), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("HandleActiveBg", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleActiveBg), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("HandleHoverColor", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleHoverColor), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("HandleBorderColor", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleBorderColor), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("FilledHandleBg", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameFilledHandleBg), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("InputFontSize", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameInputFontSize), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("InputFontSizeLG", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameInputFontSizeLG), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ButtonSpinnerDesignTokenRow("InputFontSizeSM", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenNameInputFontSizeSM), Lang(ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    public void HandleSpin(object? sender, SpinEventArgs e)
    {
        if (sender is AtomUI.Desktop.Controls.ButtonSpinner buttonSpinner)
        {
            if (buttonSpinner.Content is TextBlock textBlock)
            {
                var value = Array.IndexOf(_spinnerItems, textBlock.Text);
                if (e.Direction == SpinDirection.Increase)
                {
                    value++;
                }
                else
                {
                    value--;
                }

                if (value < 0)
                {
                    value = _spinnerItems.Length - 1;
                }
                else if (value >= _spinnerItems.Length)
                {
                    value = 0;
                }

                textBlock.Text = _spinnerItems[value];
            }
        }
    }

    private static string Lang(ButtonSpinnerShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ButtonSpinnerShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyIsSpinEnabled            => en_US.ApiPropertyIsSpinEnabled,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyIsButtonSpinnerVisible   => en_US.ApiPropertyIsButtonSpinnerVisible,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyButtonSpinnerLocation    => en_US.ApiPropertyButtonSpinnerLocation,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyLeftAddOn                => en_US.ApiPropertyLeftAddOn,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyRightAddOn               => en_US.ApiPropertyRightAddOn,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyInnerLeftContent         => en_US.ApiPropertyInnerLeftContent,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyInnerRightContent        => en_US.ApiPropertyInnerRightContent,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertySizeType                 => en_US.ApiPropertySizeType,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyStyleVariant             => en_US.ApiPropertyStyleVariant,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyStatus                   => en_US.ApiPropertyStatus,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyIsButtonSpinnerFloatable => en_US.ApiPropertyIsButtonSpinnerFloatable,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertyIsMotionEnabled          => en_US.ApiPropertyIsMotionEnabled,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertySpinnerHandleWidth       => en_US.ApiPropertySpinnerHandleWidth,
            ButtonSpinnerShowCaseLangResourceKind.ApiPropertySpin                     => en_US.ApiPropertySpin,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameControlWidth               => en_US.TokenNameControlWidth,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleWidth                => en_US.TokenNameHandleWidth,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleIconSize             => en_US.TokenNameHandleIconSize,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleBg                   => en_US.TokenNameHandleBg,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleActiveBg             => en_US.TokenNameHandleActiveBg,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleHoverColor           => en_US.TokenNameHandleHoverColor,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameHandleBorderColor          => en_US.TokenNameHandleBorderColor,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameFilledHandleBg             => en_US.TokenNameFilledHandleBg,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameInputFontSize              => en_US.TokenNameInputFontSize,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameInputFontSizeLG            => en_US.TokenNameInputFontSizeLG,
            ButtonSpinnerShowCaseLangResourceKind.TokenNameInputFontSizeSM            => en_US.TokenNameInputFontSizeSM,
            ButtonSpinnerShowCaseLangResourceKind.TokenScopeComponent                 => en_US.TokenScopeComponent,
            ButtonSpinnerShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            _                                                                         => kind.ToString()
        };
    }

    private readonly string[] _spinnerItems =
    {
        "床前明月光",
        "疑是地上霜",
        "举头望明月",
        "低头思故乡"
    };
}

public sealed record ButtonSpinnerApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ButtonSpinnerDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
