using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia.Controls;

namespace AtomUI.Controls;

public class SkeletonTitle : SkeletonLine, IControlSharedTokenResourcesHost
{
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