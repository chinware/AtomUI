using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

public class DrawerContainer : Border
{
    public Drawer? Drawer { get; }

    public bool ShowMask
    {
        get => GetValue(ShowMaskProperty);
        set => SetValue(ShowMaskProperty, value);
    }
    public static readonly StyledProperty<bool> ShowMaskProperty = Drawer.ShowMaskProperty.AddOwner<DrawerContainer>();


    #region Ctor

    static DrawerContainer()
    {
        ShowMaskProperty.Changed.AddClassHandler<DrawerContainer>((container, args) => container.UpdateBackground());
    }

    private void UpdateBackground()
    {
        Background = ShowMask ? new SolidColorBrush(Colors.Black, 0.45) : null;
    }
    
    internal DrawerContainer(Drawer drawer)
    {
        ClipToBounds = true;
        Drawer       = drawer;
        UpdateBackground();
    }

    #endregion
    
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (Drawer == null || e.Handled || Equals(e.Source, this) == false)
        {
            return;
        }

        if (e.Pointer.IsPrimary && Drawer.CloseWhenClickOnMask)
        {
            Drawer.IsOpen = false;
        }
    }
}