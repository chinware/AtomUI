using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RadioButton = AtomUI.Desktop.Controls.RadioButton;

namespace AtomUIGallery.ShowCases.Splitter;

public partial class SplitterShowCase : GalleryReactiveUserControl<SplitterViewModel>
{
    public const string LanguageId = nameof(SplitterShowCase);

    public SplitterShowCase()
    {
        InitializeComponent();
        UpdateShowCollapsibleIconMode(SplitterCollapsibleIconDisplayMode.Always);
    }

    private void HandleShowCollapsibleIconChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is not AtomUIRadioButton { IsChecked: true } radioButton)
        {
            return;
        }

        var mode = ParseShowMode(radioButton.Tag);
        if (mode == null)
        {
            return;
        }

        UpdateShowCollapsibleIconMode(mode.Value);
    }

    private SplitterCollapsibleIconDisplayMode? ParseShowMode(object? tag)
    {
        if (tag is SplitterCollapsibleIconDisplayMode mode)
        {
            return mode;
        }

        if (tag is string text &&
            Enum.TryParse<SplitterCollapsibleIconDisplayMode>(text, true, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private void UpdateShowCollapsibleIconMode(SplitterCollapsibleIconDisplayMode mode)
    {
        ApplyShowMode(ShowCollapsiblePanelFirst, mode);
        ApplyShowMode(ShowCollapsiblePanelSecond, mode);
        ApplyShowMode(ShowCollapsiblePanelThird, mode);
    }

    private static void ApplyShowMode(Control panel, SplitterCollapsibleIconDisplayMode mode)
    {
        AtomUISplitter.SetCollapsible(panel, new SplitterPanelCollapsible()
        {
            IsEnabled           = true,
            ShowCollapsibleIcon = mode
        });
    }
}
