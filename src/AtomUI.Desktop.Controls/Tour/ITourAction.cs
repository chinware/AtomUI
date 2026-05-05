namespace AtomUI.Desktop.Controls;

public interface ITourAction
{
    int StepCount { get; set; }
    int ActiveIndex { get; set; }
    TourStyleType StyleType { get; set; }

    public void NotifyAttached(Tour tour) {}
}