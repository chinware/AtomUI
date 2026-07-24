using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace AtomUI.Native;

internal static class NativeFileSystem
{
    private const uint GenericRead = 0x80000000;
    private const uint ShareReadWriteDelete = 0x00000001 | 0x00000002 | 0x00000004;
    private const uint OpenExisting = 3;
    private const uint FileFlagOpenReparsePoint = 0x00200000;
    private const uint FileFlagSequentialScan = 0x08000000;
    private const uint FileFlagBackupSemantics = 0x02000000;
    private const uint FileAttributeDirectory = 0x00000010;
    private const uint FileAttributeReparsePoint = 0x00000400;
    private const int FileAttributeTagInfo = 9;

    internal static FileStream OpenReadNoFollow(string path, string sourceDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDescription);
        return OperatingSystem.IsWindows()
            ? OpenWindows(path, sourceDescription)
            : OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()
                ? OpenUnix(path, sourceDescription)
                : OpenPortable(path, sourceDescription);
    }

    internal static string GetCanonicalDirectoryPath(string path, string sourceDescription)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceDescription);
        if (OperatingSystem.IsWindows())
        {
            return GetCanonicalWindowsPath(path, sourceDescription);
        }
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var resolved = RealPath(path, IntPtr.Zero);
            if (resolved == IntPtr.Zero)
            {
                throw IoError(path, Marshal.GetLastPInvokeError(), sourceDescription);
            }
            try
            {
                return Marshal.PtrToStringUTF8(resolved) ??
                       throw new IOException($"Directory '{path}' did not resolve to a path.");
            }
            finally
            {
                Free(resolved);
            }
        }
        return Path.GetFullPath(path);
    }

    private static string GetCanonicalWindowsPath(string path, string sourceDescription)
    {
        var handle = CreateFileW(
            path,
            0,
            ShareReadWriteDelete,
            IntPtr.Zero,
            OpenExisting,
            FileFlagBackupSemantics,
            IntPtr.Zero);
        if (handle.IsInvalid)
        {
            var error = Marshal.GetLastPInvokeError();
            handle.Dispose();
            throw IoError(path, error, sourceDescription);
        }

        using (handle)
        {
            var buffer = new StringBuilder(512);
            var length = GetFinalPathNameByHandleW(handle, buffer, (uint)buffer.Capacity, 0);
            if (length == 0)
            {
                throw IoError(path, Marshal.GetLastPInvokeError(), sourceDescription);
            }
            if (length >= (uint)buffer.Capacity)
            {
                buffer.EnsureCapacity(checked((int)length + 1));
                length = GetFinalPathNameByHandleW(handle, buffer, (uint)buffer.Capacity, 0);
                if (length == 0 || length >= (uint)buffer.Capacity)
                {
                    throw IoError(path, Marshal.GetLastPInvokeError(), sourceDescription);
                }
            }

            var resolved = buffer.ToString();
            if (resolved.StartsWith(@"\\?\UNC\", StringComparison.OrdinalIgnoreCase))
            {
                return @"\\" + resolved[8..];
            }
            return resolved.StartsWith(@"\\?\", StringComparison.Ordinal)
                ? resolved[4..]
                : resolved;
        }
    }

    private static FileStream OpenWindows(string path, string sourceDescription)
    {
        var handle = CreateFileW(
            path,
            GenericRead,
            ShareReadWriteDelete,
            IntPtr.Zero,
            OpenExisting,
            FileFlagOpenReparsePoint | FileFlagSequentialScan,
            IntPtr.Zero);
        if (handle.IsInvalid)
        {
            var error = Marshal.GetLastPInvokeError();
            handle.Dispose();
            throw IoError(path, error, sourceDescription);
        }

        if (!GetFileInformationByHandleEx(
                handle,
                FileAttributeTagInfo,
                out var information,
                (uint)Marshal.SizeOf<FileAttributeTagInformation>()))
        {
            var error = Marshal.GetLastPInvokeError();
            handle.Dispose();
            throw IoError(path, error, sourceDescription);
        }
        if ((information.FileAttributes & (FileAttributeDirectory | FileAttributeReparsePoint)) != 0)
        {
            handle.Dispose();
            throw new IOException(
                $"{sourceDescription} '{path}' must be a regular file and cannot be a reparse point.");
        }

        try
        {
            return new FileStream(handle, FileAccess.Read, 4096, isAsync: false);
        }
        catch
        {
            handle.Dispose();
            throw;
        }
    }

    private static FileStream OpenUnix(string path, string sourceDescription)
    {
        const int linuxNoFollow = 0x00020000;
        const int linuxCloseOnExec = 0x00080000;
        const int macNoFollow = 0x00000100;
        const int macCloseOnExec = 0x01000000;
        var flags = OperatingSystem.IsMacOS()
            ? macNoFollow | macCloseOnExec
            : linuxNoFollow | linuxCloseOnExec;
        var descriptor = Open(path, flags);
        if (descriptor < 0)
        {
            throw IoError(path, Marshal.GetLastPInvokeError(), sourceDescription);
        }

        var handle = new SafeFileHandle((IntPtr)descriptor, ownsHandle: true);
        try
        {
            return new FileStream(handle, FileAccess.Read, 4096, isAsync: false);
        }
        catch
        {
            handle.Dispose();
            throw;
        }
    }

    private static FileStream OpenPortable(string path, string sourceDescription)
    {
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            throw new IOException(
                $"{sourceDescription} '{path}' cannot be a symbolic link or reparse point.");
        }

        var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            FileOptions.SequentialScan);
        try
        {
            if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
            {
                throw new IOException(
                    $"{sourceDescription} '{path}' became a symbolic link or reparse point.");
            }
            return stream;
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }

    private static IOException IoError(string path, int error, string sourceDescription)
    {
        return new IOException(
            $"{sourceDescription} '{path}' could not be opened without following links: " +
            new Win32Exception(error).Message,
            new Win32Exception(error));
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFileW(
        string fileName,
        uint desiredAccess,
        uint shareMode,
        IntPtr securityAttributes,
        uint creationDisposition,
        uint flagsAndAttributes,
        IntPtr templateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetFileInformationByHandleEx(
        SafeFileHandle file,
        int fileInformationClass,
        out FileAttributeTagInformation fileInformation,
        uint bufferSize);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetFinalPathNameByHandleW(
        SafeFileHandle file,
        StringBuilder filePath,
        uint filePathLength,
        uint flags);

    [DllImport("libc", EntryPoint = "open", SetLastError = true)]
    private static extern int Open(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string path,
        int flags);

    [DllImport("libc", EntryPoint = "realpath", SetLastError = true)]
    private static extern IntPtr RealPath(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string path,
        IntPtr resolvedPath);

    [DllImport("libc", EntryPoint = "free")]
    private static extern void Free(IntPtr pointer);

    [StructLayout(LayoutKind.Sequential)]
    private struct FileAttributeTagInformation
    {
        internal uint FileAttributes;
        internal uint ReparseTag;
    }
}
