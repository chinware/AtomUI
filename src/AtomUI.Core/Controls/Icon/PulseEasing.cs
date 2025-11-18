using Avalonia.Animation.Easings;

namespace AtomUI.Controls;

public class PulseEasing : Easing
{
    private const int Steps = 8;

    private static readonly IEnumerable<double> _steps = Enumerable
                                                         .Range(0, Steps + 1)
                                                         .Select(index => 1.0 / Steps * index)
                                                         .ToArray();

    public override double Ease(double progress)
    {
        return _steps.Last(step => step <= progress);
    }
}