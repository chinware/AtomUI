namespace AtomUI.Platform.Win32.Interop;

internal struct ITaskBarList3VTable
{
    public IntPtr IUnknown1;
    public IntPtr IUnknown2;
    public IntPtr IUnknown3;
    public IntPtr HrInit;
    public IntPtr AddTab;
    public IntPtr DeleteTab;
    public IntPtr ActivateTab;
    public IntPtr SetActiveAlt;
    public IntPtr MarkFullscreenWindow;
    public IntPtr SetProgressValue;
    public IntPtr SetProgressState;
    public IntPtr RegisterTab;
    public IntPtr UnregisterTab;
    public IntPtr SetTabOrder;
    public IntPtr SetTabActive;
    public IntPtr ThumbBarAddButtons;
    public IntPtr ThumbBarUpdateButtons;
    public IntPtr ThumbBarSetImageList;
    public IntPtr SetOverlayIcon;
    public IntPtr SetThumbnailTooltip;
    public IntPtr SetThumbnailClip;
}