using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Data;
using Avalonia;
using Avalonia.Threading;
using AtomUIGallery.Localization;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.CheckBox;

public class CheckBoxViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "CheckBox";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<CheckBoxApiRow>? _apiRows;
    private ObservableCollection<CheckBoxDesignTokenRow>? _designTokenRows;

    public ObservableCollection<CheckBoxApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<CheckBoxDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public bool? _controlledCheckBoxCheckedStatus;

    public bool? ControlledCheckBoxCheckedStatus
    {
        get => _controlledCheckBoxCheckedStatus;
        set => this.RaiseAndSetIfChanged(ref _controlledCheckBoxCheckedStatus, value);
    }

    public bool _controlledCheckBoxEnabledStatus;

    public bool ControlledCheckBoxEnabledStatus
    {
        get => _controlledCheckBoxEnabledStatus;
        set => this.RaiseAndSetIfChanged(ref _controlledCheckBoxEnabledStatus, value);
    }

    private string? _checkStatusBtnText;

    public string? CheckStatusBtnText
    {
        get => _checkStatusBtnText;
        set => this.RaiseAndSetIfChanged(ref _checkStatusBtnText, value);
    }

    private string? _enableStatusBtnText;

    public string? EnableStatusBtnText
    {
        get => _enableStatusBtnText;
        set => this.RaiseAndSetIfChanged(ref _enableStatusBtnText, value);
    }

    private string? _controlledCheckBoxText;

    public string? ControlledCheckBoxText
    {
        get => _controlledCheckBoxText;
        set => this.RaiseAndSetIfChanged(ref _controlledCheckBoxText, value);
    }

    // CheckAll 例子
    private bool? _checkedAllStatus;

    public bool? CheckedAllStatus
    {
        get => _checkedAllStatus;
        set => this.RaiseAndSetIfChanged(ref _checkedAllStatus, value);
    }

    private bool _appleCheckedStatus;

    public bool AppleCheckedStatus
    {
        get => _appleCheckedStatus;
        set => this.RaiseAndSetIfChanged(ref _appleCheckedStatus, value);
    }

    private bool _pearCheckedStatus;

    public bool PearCheckedStatus
    {
        get => _pearCheckedStatus;
        set => this.RaiseAndSetIfChanged(ref _pearCheckedStatus, value);
    }

    private bool _orangeCheckedStatus;

    public bool OrangeCheckedStatus
    {
        get => _orangeCheckedStatus;
        set => this.RaiseAndSetIfChanged(ref _orangeCheckedStatus, value);
    }
    
    private IList<CheckBoxOption>? _checkBoxOptions;

    public IList<CheckBoxOption>? CheckBoxOptions
    {
        get => _checkBoxOptions;
        set => this.RaiseAndSetIfChanged(ref _checkBoxOptions, value);
    }
    
    private IList? _defaultCheckBoxOptions;

    public IList? DefaultCheckBoxOptions
    {
        get => _defaultCheckBoxOptions;
        set => this.RaiseAndSetIfChanged(ref _defaultCheckBoxOptions, value);
    }

    public ReactiveCommand<Unit, Unit> CheckStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> EnableStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckBoxCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckedAllStatusCommand { get; }
    public ReactiveCommand<Unit, Unit> CheckedItemStatusCommand1 { get; }
    public ReactiveCommand<Unit, Unit> CheckedItemStatusCommand2 { get; }
    public ReactiveCommand<Unit, Unit> CheckedItemStatusCommand3 { get; }

    public CheckBoxViewModel(IScreen screen)
    {
        HostScreen = screen;

        ControlledCheckBoxCheckedStatus = true;
        ControlledCheckBoxEnabledStatus = true;
        RefreshLocalizedTexts();

        AppleCheckedStatus  = false;
        PearCheckedStatus   = true;
        OrangeCheckedStatus = true;
        CheckedAllStatus    = null;

        EnableStatusCommand = ReactiveCommand.Create(HandleStatus);
        CheckStatusCommand  = ReactiveCommand.Create(HandleCheckStatus);
        CheckBoxCommand     = ReactiveCommand.Create(HandleCheckBox);

        CheckedAllStatusCommand   = ReactiveCommand.Create(HandleCheckedAllStatus);
        CheckedItemStatusCommand1 = ReactiveCommand.Create(HandleCheckedItemStatus);
        CheckedItemStatusCommand2 = ReactiveCommand.Create(HandleCheckedItemStatus);
        CheckedItemStatusCommand3 = ReactiveCommand.Create(HandleCheckedItemStatus);
    }

    private void HandleCheckStatus()
    {
        ControlledCheckBoxCheckedStatus = !ControlledCheckBoxCheckedStatus;
        SetupCheckBtnText();
        SetupControlledCheckBoxText();
    }

    private void HandleStatus()
    {
        ControlledCheckBoxEnabledStatus = !ControlledCheckBoxEnabledStatus;
        SetupEnabledBtnText();
        SetupControlledCheckBoxText();
    }

    private void HandleCheckBox()
    {
        SetupCheckBtnText();
        SetupControlledCheckBoxText();
    }

    public void RefreshLocalizedTexts()
    {
        SetupCheckBtnText();
        SetupEnabledBtnText();
        SetupControlledCheckBoxText();
    }

    private void SetupCheckBtnText()
    {
        if (ControlledCheckBoxCheckedStatus.HasValue)
        {
            if (ControlledCheckBoxCheckedStatus.Value)
            {
                CheckStatusBtnText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentUncheck, "UnCheck");
            }
            else
            {
                CheckStatusBtnText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentCheck, "Check");
            }
        }
        else
        {
            CheckStatusBtnText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentCheck, "Check");
        }
    }

    private void SetupEnabledBtnText()
    {
        if (ControlledCheckBoxEnabledStatus)
        {
            EnableStatusBtnText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentDisable, "Disable");
        }
        else
        {
            EnableStatusBtnText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentEnable, "Enable");
        }
    }

    private void SetupControlledCheckBoxText()
    {
        var checkedText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentUnchecked, "UnChecked");
        if (ControlledCheckBoxCheckedStatus.HasValue && ControlledCheckBoxCheckedStatus.Value)
        {
            checkedText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentChecked, "Checked");
        }

        var enabledText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentDisabled, "Disabled");
        if (ControlledCheckBoxEnabledStatus)
        {
            enabledText = CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ContentEnabled, "Enabled");
        }

        ControlledCheckBoxText = string.Format(CultureInfo.CurrentCulture,
            CheckBoxShowCaseLanguage.Get(CheckBoxShowCaseLangResourceKind.P2ControlledStatusFormat, "{0}-{1}"),
            checkedText,
            enabledText);
    }

    private void HandleCheckedAllStatus()
    {
        var isChecked = CheckedAllStatus == true;
        AppleCheckedStatus  = isChecked;
        PearCheckedStatus   = isChecked;
        OrangeCheckedStatus = isChecked;
    }

    private void HandleCheckedItemStatus()
    {
        if (OrangeCheckedStatus && PearCheckedStatus && AppleCheckedStatus)
        {
            CheckedAllStatus = true;
        }
        else if (!OrangeCheckedStatus && !PearCheckedStatus && !AppleCheckedStatus)
        {
            CheckedAllStatus = false;
        }
        else
        {
            CheckedAllStatus = null;
        }
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new CheckBoxApiRow("IsChecked", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyIsChecked), "bool?", "green", "false"),
            new CheckBoxApiRow("IsThreeState", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyIsThreeState), "bool", "green", "false"),
            new CheckBoxApiRow("Content", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyContent), "object?", "cyan", "null"),
            new CheckBoxApiRow("Command", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyCommand), "ICommand?", "cyan", "null"),
            new CheckBoxApiRow("IsMotionEnabled", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "green", "true"),
            new CheckBoxApiRow("IsWaveSpiritEnabled", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled), "bool", "green", "true"),
            new CheckBoxApiRow("CheckBoxGroup.ItemsSource", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyItemsSource), "IEnumerable?", "cyan", "null"),
            new CheckBoxApiRow("CheckBoxGroup.CheckedItems", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyCheckedItems), "IList?", "cyan", "null"),
            new CheckBoxApiRow("CheckBoxGroup.ItemSpacing", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyItemSpacing), "double", "green", "0"),
            new CheckBoxApiRow("CheckBoxGroup.LineSpacing", Lang(CheckBoxShowCaseLangResourceKind.ApiPropertyLineSpacing), "double", "green", "0")
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
            new CheckBoxDesignTokenRow("CheckIndicatorSize", Lang(CheckBoxShowCaseLangResourceKind.TokenNameCheckIndicatorSize), Lang(CheckBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CheckBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CheckBoxDesignTokenRow("CheckedMarkSize", Lang(CheckBoxShowCaseLangResourceKind.TokenNameCheckedMarkSize), Lang(CheckBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CheckBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CheckBoxDesignTokenRow("IndicatorTristateMarkSize", Lang(CheckBoxShowCaseLangResourceKind.TokenNameIndicatorTristateMarkSize), Lang(CheckBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CheckBoxShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CheckBoxDesignTokenRow("TextMargin", Lang(CheckBoxShowCaseLangResourceKind.TokenNameTextMargin), Lang(CheckBoxShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CheckBoxShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(CheckBoxShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(CheckBoxShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            CheckBoxShowCaseLangResourceKind.ApiPropertyIsChecked                  => en_US.ApiPropertyIsChecked,
            CheckBoxShowCaseLangResourceKind.ApiPropertyIsThreeState              => en_US.ApiPropertyIsThreeState,
            CheckBoxShowCaseLangResourceKind.ApiPropertyContent                   => en_US.ApiPropertyContent,
            CheckBoxShowCaseLangResourceKind.ApiPropertyCommand                   => en_US.ApiPropertyCommand,
            CheckBoxShowCaseLangResourceKind.ApiPropertyIsMotionEnabled           => en_US.ApiPropertyIsMotionEnabled,
            CheckBoxShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled       => en_US.ApiPropertyIsWaveSpiritEnabled,
            CheckBoxShowCaseLangResourceKind.ApiPropertyItemsSource               => en_US.ApiPropertyItemsSource,
            CheckBoxShowCaseLangResourceKind.ApiPropertyCheckedItems              => en_US.ApiPropertyCheckedItems,
            CheckBoxShowCaseLangResourceKind.ApiPropertyItemSpacing               => en_US.ApiPropertyItemSpacing,
            CheckBoxShowCaseLangResourceKind.ApiPropertyLineSpacing               => en_US.ApiPropertyLineSpacing,
            CheckBoxShowCaseLangResourceKind.TokenNameCheckIndicatorSize          => en_US.TokenNameCheckIndicatorSize,
            CheckBoxShowCaseLangResourceKind.TokenNameCheckedMarkSize             => en_US.TokenNameCheckedMarkSize,
            CheckBoxShowCaseLangResourceKind.TokenNameIndicatorTristateMarkSize   => en_US.TokenNameIndicatorTristateMarkSize,
            CheckBoxShowCaseLangResourceKind.TokenNameTextMargin                  => en_US.TokenNameTextMargin,
            CheckBoxShowCaseLangResourceKind.TokenScopeComponent                  => en_US.TokenScopeComponent,
            CheckBoxShowCaseLangResourceKind.TokenStatusStable                    => en_US.TokenStatusStable,
            _                                                                     => kind.ToString()
        };
    }
}

public sealed record CheckBoxApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record CheckBoxDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
