namespace AtomUI.Desktop.Controls;

public interface IDialogHostProvider
{
    IDialogHost? DialogHost { get; }
    event Action<IDialogHost?>? DialogHostChanged;
}