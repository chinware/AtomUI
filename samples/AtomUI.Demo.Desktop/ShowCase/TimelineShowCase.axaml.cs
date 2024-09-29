using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using RadioButton = Avalonia.Controls.RadioButton;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class TimelineShowCase : UserControl
{
    
    public TimelineShowCase()
    {
        InitializeComponent();

        ModeLeft.Checked += ModeChecked;

        ModeRight.Checked += ModeChecked;

        ModeAlternate.Checked += ModeChecked;
        
        ReverseButton.Click += ReverseButtonClick;
        
    }
    
    private void ReverseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ReverseTimeline.Reverse = !ReverseTimeline.Reverse;
    }
    
    private void ModeChecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton)
        {
            if (radioButton.Content?.ToString() == "Left")
            {
                LabelTimeline.Mode = TimeLineMode.Left;
            }
            else if (radioButton.Content?.ToString() == "Right")
            {
                LabelTimeline.Mode = TimeLineMode.Right;
            }
            else if (radioButton.Content?.ToString() == "Alternate")
            {
                LabelTimeline.Mode = TimeLineMode.Alternate;
            }
        }
    }
}