using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Controls;

public class SkeletonInput : SkeletonElement,
                             IControlSharedTokenResourcesHost
{
    #region 内部属性定义

    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => SkeletonToken.ID;

    #endregion
    
    public SkeletonInput()
    {
        this.RegisterResources();
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == HeightProperty || change.Property == IsBlockProperty)
        {
            ConfigureWidth();
        }
    }
    
    private void ConfigureWidth()
    {
        if (!double.IsNaN(Height) && !IsBlock)
        {
            SetValue(WidthProperty, Height * 5, BindingPriority.Template);
            SetValue(MinWidthProperty, Height * 5, BindingPriority.Template);
        }
    }
}