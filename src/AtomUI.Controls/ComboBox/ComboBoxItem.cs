using Avalonia;

namespace AtomUI.Controls;

using AvaloniaComboBoxItem = Avalonia.Controls.ComboBoxItem;

public class ComboBoxItem : AvaloniaComboBoxItem
{
    #region 内部属性定义

    internal static readonly DirectProperty<ComboBoxItem, SizeType> SizeTypeProperty =
        AvaloniaProperty.RegisterDirect<ComboBoxItem, SizeType>(nameof(SizeType),
            o => o.SizeType,
            (o, v) => o.SizeType = v);

    private SizeType _sizeType;

    internal SizeType SizeType
    {
        get => _sizeType;
        set => SetAndRaise(SizeTypeProperty, ref _sizeType, value);
    }
    
    internal static readonly DirectProperty<ComboBoxItem, bool> IsMotionEnabledProperty
        = AvaloniaProperty.RegisterDirect<ComboBoxItem, bool>(nameof(IsMotionEnabled), 
            o => o.IsMotionEnabled,
            (o, v) => o.IsMotionEnabled = v);

    private bool _isMotionEnabled = true;

    internal bool IsMotionEnabled
    {
        get => _isMotionEnabled;
        set => SetAndRaise(IsMotionEnabledProperty, ref _isMotionEnabled, value);
    }
    #endregion
}