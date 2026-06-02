using Avalonia.Animation.Easings;

namespace AtomUI.Controls;

public class PulseEasing : Easing
{
    private const int STEPS_COUNT = 8;

    private static readonly double[] Steps =
    {
        0d,
        1d / STEPS_COUNT,
        2d / STEPS_COUNT,
        3d / STEPS_COUNT,
        4d / STEPS_COUNT,
        5d / STEPS_COUNT,
        6d / STEPS_COUNT,
        7d / STEPS_COUNT,
        1d
    };

    public override double Ease(double progress)
    {
        for (var i = Steps.Length - 1; i >= 0; i--)
        {
            if (Steps[i] <= progress)
            {
                return Steps[i];
            }
        }

        throw new InvalidOperationException("Sequence contains no matching element");
    }
}
