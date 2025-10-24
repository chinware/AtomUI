namespace AtomUI.Controls;

internal class SelectInputInnerBox : AddOnDecoratedInnerBox
{
    internal SelectInput? OwningTextBox;
    
    protected override void NotifyClearButtonClicked()
    {
        OwningTextBox?.Clear();
    }
}