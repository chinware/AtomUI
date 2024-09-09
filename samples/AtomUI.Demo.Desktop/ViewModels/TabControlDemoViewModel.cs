using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AtomUI.Demo.Desktop.ViewModels;

public class TabControlDemoViewModel: ObservableObject
{
    public ObservableCollection<string> Items { get; set; }

    public TabControlDemoViewModel()
    {
        Items = new ObservableCollection<string>(Enumerable.Range(1, 200).Select(a => "Tab " + a));
    }
}