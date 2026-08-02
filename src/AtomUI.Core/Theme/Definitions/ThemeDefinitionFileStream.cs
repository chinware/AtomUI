using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace AtomUI.Theme.Definitions;

internal static class ThemeDefinitionFileStream
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

    internal static FileStream OpenReadNoFollow(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return OperatingSystem.IsWindows()
            ? OpenWindows(path)
            : OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()
                ? OpenUnix(path)
                : OpenPortable(path);
    }

    internal static string GetCanonicalDirectoryPath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (OperatingSystem.IsWindows())
        {
            return GetCanonicalWindowsPath(path);
        }
        if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
        {
            var resolved = RealPath(path, IntPtr.Zero);
            if (resolved == IntPtr.Zero)
            {
                throw IoError(path, Marshal.GetLastPInvokeError());
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

    private static string GetCanonicalWindowsPath(string path)
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
            throw IoError(path, error);
        }

        using (handle)
        {
            var buffer = new StringBuilder(512);
            var length = GetFinalPathNameByHandleW(handle, buffer, (uint)buffer.Capacity, 0);
            if (length == 0)
            {
                throw IoError(path, Marshal.GetLastPInvokeError());
            }
            if (length >= (uint)buffer.Capacity)
            {
                buffer.EnsureCapacity(checked((int)length + 1));
                length = GetFinalPathNameByHandleW(handle, buffer, (uint)buffer.Capacity, 0);
                if (length == 0 || length >= (uint)buffer.Capacity)
                {
                    throw IoError(path, Marshal.GetLastPInvokeError());
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

    private static FileStream OpenWindows(string path)
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
            throw IoError(path, error);
        }

        if (!GetFileInformationByHandleEx(
                handle,
                FileAttributeTagInfo,
                out var information,
                (uint)Marshal.SizeOf<FileAttributeTagInformation>()))
        {
            var error = Marshal.GetLastPInvokeError();
            handle.Dispose();
            throw IoError(path, error);
        }
        if ((information.FileAttributes & (FileAttributeDirectory | FileAttributeReparsePoint)) != 0)
        {
            handle.Dispose();
            throw new IOException(
                $"Theme definition file '{path}' must be a regular file and cannot be a reparse point.");
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

    private static FileStream OpenUnix(string path)
    {
        const int linuxNoFollow = 0x00020000;
        const int linuxCloseOnExec = 0x00080000;
        const int linuxNonBlocking = 0x00000800;
        const int macNoFollow = 0x00000100;
        const int macCloseOnExec = 0x01000000;
        const int macNonBlocking = 0x00000004;
        var flags = OperatingSystem.IsMacOS()
            ? macNoFollow | macCloseOnExec | macNonBlocking
            : linuxNoFollow | linuxCloseOnExec | linuxNonBlocking;
        var descriptor = Open(path, flags);
        if (descriptor < 0)
        {
            throw IoError(path, Marshal.GetLastPInvokeError());
        }

        var handle = new SafeFileHandle((IntPtr)descriptor, ownsHandle: true);
        try
        {
            if (!TryGetUnixFileMode(descriptor, out var mode))
            {
                throw IoError(path, Marshal.GetLastPInvokeError());
            }
            if (!IsUnixRegularFile(mode))
            {
                throw new IOException(
                    $"Theme definition file '{path}' must be a regular file and cannot be a pipe or device.");
            }

            return new FileStream(handle, FileAccess.Read, 4096, isAsync: false);
        }
        catch
        {
            handle.Dispose();
            throw;
        }
    }

    private static bool TryGetUnixFileMode(int descriptor, out uint mode)
    {
        var buffer = Marshal.AllocHGlobal(UnixStatBufferBytes);
        try
        {
            if (FStat(descriptor, buffer) != 0)
            {
                mode = 0;
                return false;
            }

            mode = OperatingSystem.IsMacOS()
                ? (ushort)Marshal.ReadInt16(buffer, MacStatModeOffset)
                : unchecked((uint)Marshal.ReadInt32(buffer, LinuxStatModeOffset));
            return true;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    private static bool IsUnixRegularFile(uint mode)
    {
        return (mode & UnixFileTypeMask) == UnixRegularFileType;
    }

    private static FileStream OpenPortable(string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            throw new IOException(
                $"Theme definition file '{path}' cannot be a symbolic link or reparse point.");
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
                    $"Theme definition file '{path}' became a symbolic link or reparse point.");
            }
            return stream;
        }
        catch
        {
            stream.Dispose();
            throw;
        }
    }

    private static IOException IoError(string path, int error)
    {
        return new IOException(
            $"Theme definition file '{path}' could not be opened without following links: " +
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

    [DllImport("libc", EntryPoint = "fstat", SetLastError = true)]
    private static extern int FStat(int descriptor, IntPtr information);

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

    private const int UnixStatBufferBytes = 256;
    private const int LinuxStatModeOffset = 24;
    private const int MacStatModeOffset = 4;
    private const uint UnixFileTypeMask = 0xF000;
    private const uint UnixRegularFileType = 0x8000;
}
