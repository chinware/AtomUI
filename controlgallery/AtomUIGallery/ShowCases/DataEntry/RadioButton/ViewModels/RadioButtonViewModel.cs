using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.RadioButton;

public class RadioButtonViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "RadioButton";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<RadioButtonApiRow>? _apiRows;
    private ObservableCollection<RadioButtonDesignTokenRow>? _designTokenRows;

    public ObservableCollection<RadioButtonApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<RadioButtonDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private IList<RadioButtonOption>? _radioOptions;

    public IList<RadioButtonOption>? RadioOptions
    {
        get => _radioOptions;
        set => this.RaiseAndSetIfChanged(ref _radioOptions, value);
    }

    private IList<RadioButtonOption>? _twoWayRadioOptions;

    public IList<RadioButtonOption>? TwoWayRadioOptions
    {
        get => _twoWayRadioOptions;
        set => this.RaiseAndSetIfChanged(ref _twoWayRadioOptions, value);
    }

    private object? _twoWayCheckedItem;

    public object? TwoWayCheckedItem
    {
        get => _twoWayCheckedItem;
        set
        {
            if (ReferenceEquals(_twoWayCheckedItem, value))
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _twoWayCheckedItem, value);
            UpdateTwoWayCheckedSummary();
        }
    }

    private string? _twoWayCheckedSummary;

    public string? TwoWayCheckedSummary
    {
        get => _twoWayCheckedSummary;
        set => this.RaiseAndSetIfChanged(ref _twoWayCheckedSummary, value);
    }

    private RadioButtonOption? _twoWayChengduOption;

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
    public ReactiveCommand<Unit, Unit> SelectChengduCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearTwoWayCheckedItemCommand { get; }

    public RadioButtonViewModel(IScreen screen)
    {
        HostScreen = screen;

        _toggleDisabledRadioUnCheckedEnabled = true;
        _toggleDisabledRadioCheckedEnabled = true;

        ToggleDisabledCommand = ReactiveCommand.Create(HandleToggleDisabled);
        SelectChengduCommand  = ReactiveCommand.Create(HandleSelectChengdu);
        ClearTwoWayCheckedItemCommand = ReactiveCommand.Create(HandleClearTwoWayCheckedItem);
    }

    private void HandleToggleDisabled()
    {
        ToggleDisabledRadioUnCheckedEnabled = !ToggleDisabledRadioUnCheckedEnabled;
        ToggleDisabledRadioCheckedEnabled = !ToggleDisabledRadioCheckedEnabled;
    }

    public void ConfigureTwoWayRadioOptions(
        RadioButtonOption hangzhou,
        RadioButtonOption shanghai,
        RadioButtonOption beijing,
        RadioButtonOption chengdu)
    {
        _twoWayChengduOption = chengdu;
        TwoWayRadioOptions =
        [
            hangzhou,
            shanghai,
            beijing,
            chengdu
        ];
        TwoWayCheckedItem = shanghai;
    }

    public void ClearTwoWayRadioOptions()
    {
        _twoWayChengduOption = null;
        TwoWayCheckedItem    = null;
        TwoWayRadioOptions   = null;
        TwoWayCheckedSummary = null;
    }

    private void HandleSelectChengdu()
    {
        if (_twoWayChengduOption != null)
        {
            TwoWayCheckedItem = _twoWayChengduOption;
        }
    }

    private void HandleClearTwoWayCheckedItem()
    {
        TwoWayCheckedItem = null;
    }

    private void UpdateTwoWayCheckedSummary()
    {
        var selectedText = RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2ContentNone, "None");
        if (TwoWayCheckedItem is RadioButtonOption { Content: not null } option)
        {
            selectedText = option.Content.ToString() ?? selectedText;
        }

        TwoWayCheckedSummary = string.Format(CultureInfo.CurrentCulture,
            RadioButtonShowCaseLanguage.Get(RadioButtonShowCaseLangResourceKind.P2CheckedItemSummaryFormat, "Selected: {0}"),
            selectedText);
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new RadioButtonApiRow("RadioButton.IsChecked", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyIsChecked), "bool?", "green", "false"),
            new RadioButtonApiRow("RadioButton.Content", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyContent), "object?", "cyan", "null"),
            new RadioButtonApiRow("RadioButton.IsEnabled", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyIsEnabled), "bool", "green", "true"),
            new RadioButtonApiRow("RadioButton.IsMotionEnabled", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "green", "token"),
            new RadioButtonApiRow("RadioButton.IsWaveSpiritEnabled", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled), "bool", "green", "token"),
            new RadioButtonApiRow("RadioButtonGroup.CheckedItem", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyCheckedItem), "object?", "cyan", "null"),
            new RadioButtonApiRow("RadioButtonGroup.ItemsSource", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "cyan", "null"),
            new RadioButtonApiRow("RadioButtonGroup.ItemTemplate", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyItemTemplate), "IDataTemplate?", "cyan", "null"),
            new RadioButtonApiRow("RadioButtonGroup.Orientation", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyOrientation), "Orientation", "blue", "Horizontal"),
            new RadioButtonApiRow("RadioButtonGroup.ItemSpacing", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyItemSpacing), "double", "cyan", "0"),
            new RadioButtonApiRow("RadioButtonGroup.LineSpacing", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyLineSpacing), "double", "cyan", "0"),
            new RadioButtonApiRow("OptionButtonGroup.ButtonStyle", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyButtonStyle), "OptionButtonStyle", "purple", "Outline"),
            new RadioButtonApiRow("OptionButtonGroup.SizeType", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
            new RadioButtonApiRow("OptionButtonGroup.SelectedItem", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertySelectedItem), "object?", "cyan", "null"),
            new RadioButtonApiRow("OptionButton.Icon", Lang(RadioButtonShowCaseLangResourceKind.ApiPropertyIcon), "PathIcon?", "cyan", "null")
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
            new RadioButtonDesignTokenRow("RadioButton.RadioSize", Lang(RadioButtonShowCaseLangResourceKind.TokenNameRadioSize), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeRadioButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("RadioButton.DotSize", Lang(RadioButtonShowCaseLangResourceKind.TokenNameDotSize), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeRadioButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("RadioButton.DotColorDisabled", Lang(RadioButtonShowCaseLangResourceKind.TokenNameDotColorDisabled), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeRadioButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("RadioButton.TextMargin", Lang(RadioButtonShowCaseLangResourceKind.TokenNameTextMargin), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeRadioButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("OptionButton.ButtonBackground", Lang(RadioButtonShowCaseLangResourceKind.TokenNameButtonBackground), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeOptionButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("OptionButton.ButtonCheckedBackground", Lang(RadioButtonShowCaseLangResourceKind.TokenNameButtonCheckedBackground), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeOptionButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("OptionButton.ButtonColor", Lang(RadioButtonShowCaseLangResourceKind.TokenNameButtonColor), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeOptionButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("OptionButton.ButtonPadding", Lang(RadioButtonShowCaseLangResourceKind.TokenNameButtonPadding), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeOptionButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("OptionButton.ButtonSolidCheckedBackground", Lang(RadioButtonShowCaseLangResourceKind.TokenNameButtonSolidCheckedBackground), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeOptionButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("OptionButton.ContentFontSize", Lang(RadioButtonShowCaseLangResourceKind.TokenNameContentFontSize), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeOptionButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new RadioButtonDesignTokenRow("OptionButton.Padding", Lang(RadioButtonShowCaseLangResourceKind.TokenNamePadding), Lang(RadioButtonShowCaseLangResourceKind.TokenScopeOptionButton), "cyan", Lang(RadioButtonShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(RadioButtonShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(RadioButtonShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            RadioButtonShowCaseLangResourceKind.ApiPropertyIsChecked                    => en_US.ApiPropertyIsChecked,
            RadioButtonShowCaseLangResourceKind.ApiPropertyContent                      => en_US.ApiPropertyContent,
            RadioButtonShowCaseLangResourceKind.ApiPropertyIsEnabled                    => en_US.ApiPropertyIsEnabled,
            RadioButtonShowCaseLangResourceKind.ApiPropertyIsMotionEnabled              => en_US.ApiPropertyIsMotionEnabled,
            RadioButtonShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled          => en_US.ApiPropertyIsWaveSpiritEnabled,
            RadioButtonShowCaseLangResourceKind.ApiPropertyCheckedItem                  => en_US.ApiPropertyCheckedItem,
            RadioButtonShowCaseLangResourceKind.ApiPropertyItemsSource                  => en_US.ApiPropertyItemsSource,
            RadioButtonShowCaseLangResourceKind.ApiPropertyItemTemplate                 => en_US.ApiPropertyItemTemplate,
            RadioButtonShowCaseLangResourceKind.ApiPropertyOrientation                  => en_US.ApiPropertyOrientation,
            RadioButtonShowCaseLangResourceKind.ApiPropertyItemSpacing                  => en_US.ApiPropertyItemSpacing,
            RadioButtonShowCaseLangResourceKind.ApiPropertyLineSpacing                  => en_US.ApiPropertyLineSpacing,
            RadioButtonShowCaseLangResourceKind.ApiPropertyButtonStyle                  => en_US.ApiPropertyButtonStyle,
            RadioButtonShowCaseLangResourceKind.ApiPropertySizeType                     => en_US.ApiPropertySizeType,
            RadioButtonShowCaseLangResourceKind.ApiPropertySelectedItem                 => en_US.ApiPropertySelectedItem,
            RadioButtonShowCaseLangResourceKind.ApiPropertyIcon                         => en_US.ApiPropertyIcon,
            RadioButtonShowCaseLangResourceKind.TokenNameRadioSize                      => en_US.TokenNameRadioSize,
            RadioButtonShowCaseLangResourceKind.TokenNameDotSize                        => en_US.TokenNameDotSize,
            RadioButtonShowCaseLangResourceKind.TokenNameDotColorDisabled               => en_US.TokenNameDotColorDisabled,
            RadioButtonShowCaseLangResourceKind.TokenNameTextMargin                     => en_US.TokenNameTextMargin,
            RadioButtonShowCaseLangResourceKind.TokenNameButtonBackground               => en_US.TokenNameButtonBackground,
            RadioButtonShowCaseLangResourceKind.TokenNameButtonCheckedBackground        => en_US.TokenNameButtonCheckedBackground,
            RadioButtonShowCaseLangResourceKind.TokenNameButtonColor                    => en_US.TokenNameButtonColor,
            RadioButtonShowCaseLangResourceKind.TokenNameButtonPadding                  => en_US.TokenNameButtonPadding,
            RadioButtonShowCaseLangResourceKind.TokenNameButtonSolidCheckedBackground   => en_US.TokenNameButtonSolidCheckedBackground,
            RadioButtonShowCaseLangResourceKind.TokenNameContentFontSize                => en_US.TokenNameContentFontSize,
            RadioButtonShowCaseLangResourceKind.TokenNamePadding                        => en_US.TokenNamePadding,
            RadioButtonShowCaseLangResourceKind.TokenScopeRadioButton                   => en_US.TokenScopeRadioButton,
            RadioButtonShowCaseLangResourceKind.TokenScopeOptionButton                  => en_US.TokenScopeOptionButton,
            RadioButtonShowCaseLangResourceKind.TokenStatusStable                       => en_US.TokenStatusStable,
            _                                                                           => kind.ToString()
        };
    }
}

public sealed record RadioButtonApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record RadioButtonDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
