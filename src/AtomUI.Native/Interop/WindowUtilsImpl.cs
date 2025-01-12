using System.Runtime.InteropServices;

namespace AtomUI.Native.Interop;

internal static partial class WindowUtilsImpl
{
    private const string NativeLibName = "AtomUI";

    [LibraryImport(NativeLibName)]
    internal static partial void AtomUISetWindowIgnoresMouseEvents(IntPtr handle,
                                                                   [MarshalAs(UnmanagedType.Bool)] bool flag);
}