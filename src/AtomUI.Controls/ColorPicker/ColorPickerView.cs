using Avalonia.Controls;

namespace AtomUI.Controls;

public class ColorPickerView : AbstractColorPickerView
{
    #region 公共属性定义
    #endregion

    #region 公共事件定义
    public event EventHandler<ColorChangedEventArgs>? ColorChanged;
    #endregion

}