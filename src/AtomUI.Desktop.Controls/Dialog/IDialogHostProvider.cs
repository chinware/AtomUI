namespace AtomUI.Controls;

public interface IDialogHostProvider
{
    IDialogHost? DialogHost { get; }
    event Action<IDialogHost?>? DialogHostChanged;
}