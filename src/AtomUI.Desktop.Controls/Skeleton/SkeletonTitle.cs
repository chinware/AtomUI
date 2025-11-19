using AtomUI.Theme;
using AtomUI.Utils;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public class SkeletonTitle : SkeletonLine, IControlSharedTokenResourcesHost
{
    protected override Type StyleKeyOverride { get; } = typeof(SkeletonLine);
    
    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SkeletonToken.ID;

    #endregion
    
    static SkeletonTitle()
    {
        LineWidthProperty.OverrideDefaultValue<SkeletonTitle>(new Dimension(50, DimensionUnitType.Percentage));
    }
    
    public SkeletonTitle()
    {
        this.RegisterResources();
    }
}