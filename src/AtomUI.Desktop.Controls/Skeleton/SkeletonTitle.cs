using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class SkeletonTitle : SkeletonLine
{
    protected override Type StyleKeyOverride { get; } = typeof(SkeletonLine);
    
    static SkeletonTitle()
    {
        LineWidthProperty.OverrideDefaultValue<SkeletonTitle>(new Dimension(50, DimensionUnitType.Percentage));
    }
    
    public SkeletonTitle()
    {
        this.RegisterTokenResourceScope(SkeletonToken.ScopeProvider);
    }
}