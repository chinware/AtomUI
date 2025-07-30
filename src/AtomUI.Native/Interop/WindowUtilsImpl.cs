using System.Runtime.InteropServices;

namespace AtomUI.Native.Interop;

internal static partial class WindowUtilsImpl
{
    private const string NativeLibName = "AtomUINative";

    [LibraryImport(NativeLibName)]
    internal static partial void AtomUISetWindowIgnoresMouseEvents(IntPtr handle,
                                                                   [MarshalAs(UnmanagedType.Bool)] bool flag);
    
    [LibraryImport(NativeLibName)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AtomUIGetWindowIgnoresMouseEvents(IntPtr handle);
    
    [LibraryImport(NativeLibName)]
    internal static partial void AtomUILockWindowBuddyLayer(IntPtr windowHandle, IntPtr buddyHandle);
    
    [LibraryImport(NativeLibName)]
    internal static partial void AtomUISetMacOSCaptionButtonsPosition(IntPtr windowHandle, double x, double y, double spacing);
    
    [LibraryImport(NativeLibName)]
    internal static partial CGSize AtomUIMacOSCaptionsSize(IntPtr windowHandle, double spacing);
}

// 定义与 CGSize 对应的结构体
[StructLayout(LayoutKind.Sequential)]
internal struct CGSize
{
    public double Width;
    public double Height;
    
    public override string ToString() => 
        $"Width: {Width}, Height: {Height}";
}