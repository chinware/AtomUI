using AtomUI.Desktop.Controls;

namespace AtomUIGallery.ShowCases.Form;

public partial class FormControlsShowCase : GalleryReactiveUserControl<FormViewModel>
{
    public FormControlsShowCase()
    {
        InitializeComponent();
        FormSliderItem.Marks = CreateSliderMarks();
    }

    private static List<SliderMark> CreateSliderMarks()
    {
        return new List<SliderMark>
        {
            new("A", 0),
            new("B", 20),
            new("C", 40),
            new("D", 60),
            new("E", 80),
            new("F", 100)
        };
    }
}
