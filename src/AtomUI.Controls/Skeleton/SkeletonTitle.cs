namespace AtomUI.Controls;

public class SkeletonTitle : SkeletonLine
{
    static SkeletonTitle()
    {
        LineWidthProperty.OverrideDefaultValue<SkeletonTitle>(new SkeletonWidth(50, SkeletonUnitType.Percentage));
    }
}