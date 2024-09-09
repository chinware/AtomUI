using CommunityToolkit.Mvvm.ComponentModel;

namespace AtomUI.Demo.Desktop.ViewModels;

public class CheckBoxShowCaseModel : ObservableObject
{
    private bool _appleCheckedStatus;

    // CheckAll 例子
    private bool? _checkedAllStatus;

    private string? _checkStatusBtnText;
    public bool? _controlledCheckBoxCheckedStatus;

    public bool _controlledCheckBoxEnabledStatus;

    private string? _controlledCheckBoxText;

    private string? _enableStatusBtnText;

    private bool _orangeCheckedStatus;

    private bool _pearCheckedStatus;

    public CheckBoxShowCaseModel()
    {
        CheckStatusBtnText              = "UnCheck";
        EnableStatusBtnText             = "Disable";
        ControlledCheckBoxCheckedStatus = true;
        ControlledCheckBoxEnabledStatus = true;
        SetupControlledCheckBoxText();

        AppleCheckedStatus  = false;
        PearCheckedStatus   = true;
        OrangeCheckedStatus = true;
        CheckedAllStatus    = null;
    }

    public bool? ControlledCheckBoxCheckedStatus
    {
        get => _controlledCheckBoxCheckedStatus;
        set => SetProperty(ref _controlledCheckBoxCheckedStatus, value);
    }

    public bool ControlledCheckBoxEnabledStatus
    {
        get => _controlledCheckBoxEnabledStatus;
        set => SetProperty(ref _controlledCheckBoxEnabledStatus, value);
    }

    public string? CheckStatusBtnText
    {
        get => _checkStatusBtnText;
        set => SetProperty(ref _checkStatusBtnText, value);
    }

    public string? EnableStatusBtnText
    {
        get => _enableStatusBtnText;
        set => SetProperty(ref _enableStatusBtnText, value);
    }

    public string? ControlledCheckBoxText
    {
        get => _controlledCheckBoxText;
        set => SetProperty(ref _controlledCheckBoxText, value);
    }

    public bool? CheckedAllStatus
    {
        get => _checkedAllStatus;
        set => SetProperty(ref _checkedAllStatus, value);
    }

    public bool AppleCheckedStatus
    {
        get => _appleCheckedStatus;
        set => SetProperty(ref _appleCheckedStatus, value);
    }

    public bool PearCheckedStatus
    {
        get => _pearCheckedStatus;
        set => SetProperty(ref _pearCheckedStatus, value);
    }

    public bool OrangeCheckedStatus
    {
        get => _orangeCheckedStatus;
        set => SetProperty(ref _orangeCheckedStatus, value);
    }

    public void CheckStatusHandler(object arg)
    {
        ControlledCheckBoxCheckedStatus = !ControlledCheckBoxCheckedStatus;
        SetupCheckBtnText();
        SetupControlledCheckBoxText();
    }

    public void EnableStatusHandler(object arg)
    {
        ControlledCheckBoxEnabledStatus = !ControlledCheckBoxEnabledStatus;
        SetupEnabledBtnText();
        SetupControlledCheckBoxText();
    }

    public void CheckBoxHandler(object arg)
    {
        SetupCheckBtnText();
        SetupControlledCheckBoxText();
    }

    private void SetupCheckBtnText()
    {
        if (ControlledCheckBoxCheckedStatus.HasValue)
        {
            if (ControlledCheckBoxCheckedStatus.Value)
                CheckStatusBtnText = "UnCheck";
            else
                CheckStatusBtnText = "Check";
        }
        else
        {
            CheckStatusBtnText = "Check";
        }
    }

    private void SetupEnabledBtnText()
    {
        if (ControlledCheckBoxEnabledStatus)
            EnableStatusBtnText = "Disable";
        else
            EnableStatusBtnText = "Enable";
    }

    private void SetupControlledCheckBoxText()
    {
        var checkedText = "UnChecked";
        if (ControlledCheckBoxCheckedStatus.HasValue && ControlledCheckBoxCheckedStatus.Value) checkedText = "Checked";

        var enabledText                                  = "Disabled";
        if (ControlledCheckBoxEnabledStatus) enabledText = "Enabled";

        ControlledCheckBoxText = $"{checkedText}-{enabledText}";
    }

    public void CheckedAllStatusHandler()
    {
        if (!CheckedAllStatus.HasValue || !CheckedAllStatus.Value)
        {
            AppleCheckedStatus  = false;
            PearCheckedStatus   = false;
            OrangeCheckedStatus = false;
        }
        else
        {
            AppleCheckedStatus  = true;
            PearCheckedStatus   = true;
            OrangeCheckedStatus = true;
        }
    }

    public void CheckedItemStatusHandler(object arg)
    {
        if (OrangeCheckedStatus && PearCheckedStatus && AppleCheckedStatus)
            CheckedAllStatus = true;
        else if (!OrangeCheckedStatus && !PearCheckedStatus && !AppleCheckedStatus)
            CheckedAllStatus = false;
        else
            CheckedAllStatus = null;
    }
}