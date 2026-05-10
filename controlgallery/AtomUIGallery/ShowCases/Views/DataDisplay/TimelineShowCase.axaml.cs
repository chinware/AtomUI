using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;
using RadioButton = Avalonia.Controls.RadioButton;

namespace AtomUIGallery.ShowCases.Views;

public partial class TimelineShowCase : ReactiveUserControl<TimelineViewModel>
{
    public TimelineShowCase()
    {
        this.WhenActivated(disposables =>
        {
            ModeLeft.IsCheckedChanged      += ModeChecked;
            ModeRight.IsCheckedChanged     += ModeChecked;
            ModeAlternate.IsCheckedChanged += ModeChecked;
            ReverseButton.Click            += ReverseButtonClick;

            Disposable.Create(() =>
            {
                ModeLeft.IsCheckedChanged      -= ModeChecked;
                ModeRight.IsCheckedChanged     -= ModeChecked;
                ModeAlternate.IsCheckedChanged -= ModeChecked;
                ReverseButton.Click            -= ReverseButtonClick;
            }).DisposeWith(disposables);
        });
        InitializeComponent();
    }

    private void ReverseButtonClick(object? sender, RoutedEventArgs e)
    {
        ReverseTimeline.IsReverse = !ReverseTimeline.IsReverse;
    }

    private void ModeChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            if (radioButton == ModeLeft && ModeLeft.IsChecked == true)
            {
                LabelTimeline.Mode = TimelineMode.Left;
            }
            else if (radioButton == ModeRight && ModeRight.IsChecked == true)
            {
                LabelTimeline.Mode = TimelineMode.Right;
            }
            else if (radioButton == ModeAlternate && ModeAlternate.IsChecked == true)
            {
                LabelTimeline.Mode = TimelineMode.Alternate;
            }
        }
    }
}
