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
    internal static partial void AtomUIMoveWindow(IntPtr windowHandle, [MarshalAs(UnmanagedType.I4)] int x, [MarshalAs(UnmanagedType.I4)] int y);
}