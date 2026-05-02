using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ViewModels;

public class CheckBoxViewModel : ReactiveObject, IRoutableViewModel
{
    public const string ID = "CheckBox";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID;

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

        CheckStatusBtnText              = "UnCheck";
        EnableStatusBtnText             = "Disable";
        ControlledCheckBoxCheckedStatus = true;
        ControlledCheckBoxEnabledStatus = true;
        SetupControlledCheckBoxText();

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

    private void SetupCheckBtnText()
    {
        if (ControlledCheckBoxCheckedStatus.HasValue)
        {
            if (ControlledCheckBoxCheckedStatus.Value)
            {
                CheckStatusBtnText = "UnCheck";
            }
            else
            {
                CheckStatusBtnText = "Check";
            }
        }
        else
        {
            CheckStatusBtnText = "Check";
        }
    }

    private void SetupEnabledBtnText()
    {
        if (ControlledCheckBoxEnabledStatus)
        {
            EnableStatusBtnText = "Disable";
        }
        else
        {
            EnableStatusBtnText = "Enable";
        }
    }

    private void SetupControlledCheckBoxText()
    {
        var checkedText = "UnChecked";
        if (ControlledCheckBoxCheckedStatus.HasValue && ControlledCheckBoxCheckedStatus.Value)
        {
            checkedText = "Checked";
        }

        var enabledText = "Disabled";
        if (ControlledCheckBoxEnabledStatus)
        {
            enabledText = "Enabled";
        }

        ControlledCheckBoxText = $"{checkedText}-{enabledText}";
    }

    private void HandleCheckedAllStatus()
    {
        if (!CheckedAllStatus.HasValue || !CheckedAllStatus.Value)
        {
            AppleCheckedStatus  = true;
            PearCheckedStatus   = true;
            OrangeCheckedStatus = true;
        }
        else
        {
            AppleCheckedStatus  = false;
            PearCheckedStatus   = false;
            OrangeCheckedStatus = false;
        }
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
}

public class CheckBoxOption
{
    public string? Content { get; set; }
    public bool IsEnabled { get; set; } = true;
}