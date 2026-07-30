namespace AtomUI.Desktop.Controls;

internal class TimedRangeDatePickerPresenter : RangeDatePickerPresenter
{
    protected override void NotifyNowButtonClicked()
    {
        if (!EffectiveDateRange.Contains(DateTime.Now))
        {
            return;
        }

        SelectNowForActiveRangePart();

        if (!IsNeedConfirm)
        {
            OnConfirmed();
        }
    }
}
