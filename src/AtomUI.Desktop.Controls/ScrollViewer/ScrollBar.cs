using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Theme;
using Avalonia;

namespace AtomUI.Desktop.Controls;

public class ScrollBar : AbstractScrollBar
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsLiteModeProperty =
        ScrollViewer.IsLiteModeProperty.AddOwner<ScrollBar>();

    public bool IsLiteMode
    {
        get => GetValue(IsLiteModeProperty);
        set => SetValue(IsLiteModeProperty, value);
    }

    #endregion
    
    public ScrollBar()
    {
        this.RegisterTokenResourceScope(ScrollViewerToken.ScopeProvider);
    }
    
    protected override void UpdateIsExpandedState()
    {
        if (!IsLiteMode)
        {
            if (!AllowAutoHide)
            {
                var timer = this.GetTimer();
                timer?.Stop();
                IsEffectiveExpanded = false;
            }
        }
        else
        {
            if (!AllowAutoHide)
            {
                var timer = this.GetTimer();
                timer?.Stop();
            }
        }
    }
}