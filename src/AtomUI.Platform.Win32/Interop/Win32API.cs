#nullable enable

using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
#pragma warning disable 169, 649

namespace AtomUI.Platform.Win32.Interop;

internal static class Win32API
{
    
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLongPtrW", ExactSpelling = true)]
    public static extern uint GetWindowLongPtr(IntPtr hWnd, int nIndex);
    
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "GetWindowLongW", ExactSpelling = true)]
    public static extern uint GetWindowLong32b(IntPtr hWnd, int nIndex);
    
    public static uint GetWindowLong(IntPtr hWnd, int nIndex)
    {
        if (IntPtr.Size == 4)
        {
            return GetWindowLong32b(hWnd, nIndex);
        }
        return GetWindowLongPtr(hWnd, nIndex);
    }
    
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongW", ExactSpelling = true)]
    private static extern uint SetWindowLong32b(IntPtr hWnd, int nIndex, uint value);

    [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowLongPtrW", ExactSpelling = true)]
    private static extern IntPtr SetWindowLong64b(IntPtr hWnd, int nIndex, IntPtr value);
    
    public static uint SetWindowLong(IntPtr hWnd, int nIndex, uint value)
    {
        if (IntPtr.Size == 4)
        {
            return SetWindowLong32b(hWnd, nIndex, value);
        }
        return (uint)SetWindowLong64b(hWnd, nIndex, new IntPtr(value)).ToInt32();
    }
    
    public static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr handle)
    {
        if (IntPtr.Size == 4)
        {
            return new IntPtr(SetWindowLong32b(hWnd, nIndex, (uint)handle.ToInt32()));
        }
        return SetWindowLong64b(hWnd, nIndex, handle);
    }
}