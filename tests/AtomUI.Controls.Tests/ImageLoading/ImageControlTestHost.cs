using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace AtomUI.Controls.Tests.ImageLoading;

internal sealed class ImageControlTestHost : IDisposable
{
    private readonly Window _window;

    internal ImageControlTestHost(Control control, double width = 160, double height = 120)
    {
        var arrangedWidth = double.IsFinite(control.Width) ? control.Width : width;
        var arrangedHeight = double.IsFinite(control.Height) ? control.Height : height;
        control.Measure(new Size(arrangedWidth, arrangedHeight));
        control.Arrange(new Rect(0, 0, arrangedWidth, arrangedHeight));
        _window = new Window
        {
            Width = width,
            Height = height,
            Content = control
        };
        _window.Show();
        Dispatcher.UIThread.RunJobs();
    }

    internal void Detach()
    {
        _window.Content = null;
        Dispatcher.UIThread.RunJobs();
    }

    internal void Attach(Control control)
    {
        _window.Content = control;
        Dispatcher.UIThread.RunJobs();
    }

    internal static void WaitUntil(Func<bool> predicate, string description)
    {
        var timeout = Stopwatch.StartNew();
        while (!predicate())
        {
            Dispatcher.UIThread.RunJobs();
            if (timeout.Elapsed >= TimeSpan.FromSeconds(5))
            {
                throw new TimeoutException($"Timed out waiting for {description}.");
            }
            Thread.Yield();
        }
        Dispatcher.UIThread.RunJobs();
    }

    internal static byte[] CreatePng(int width, int height)
    {
        using var output = new MemoryStream();
        output.Write([0x89, (byte)'P', (byte)'N', (byte)'G', 0x0d, 0x0a, 0x1a, 0x0a]);

        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteUInt32BigEndian(header[..4], checked((uint)width));
        BinaryPrimitives.WriteUInt32BigEndian(header[4..8], checked((uint)height));
        header[8] = 8;
        header[9] = 6;
        WriteChunk(output, "IHDR", header);

        using var raw = new MemoryStream();
        var row = new byte[checked(width * 4 + 1)];
        for (var y = 0; y < height; y++)
        {
            raw.Write(row);
        }
        raw.Position = 0;
        using var compressed = new MemoryStream();
        using (var zlib = new ZLibStream(compressed, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            raw.CopyTo(zlib);
        }
        WriteChunk(output, "IDAT", compressed.ToArray());
        WriteChunk(output, "IEND", []);
        return output.ToArray();
    }

    private static void WriteChunk(Stream output, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(length, checked((uint)data.Length));
        output.Write(length);
        var typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        var crcBytes = new byte[typeBytes.Length + data.Length];
        typeBytes.CopyTo(crcBytes, 0);
        data.CopyTo(crcBytes.AsSpan(typeBytes.Length));
        Span<byte> crc = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(crc, CalculateCrc32(crcBytes));
        output.Write(crc);
    }

    private static uint CalculateCrc32(ReadOnlySpan<byte> bytes)
    {
        var crc = uint.MaxValue;
        foreach (var value in bytes)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) != 0 ? 0xedb88320u ^ (crc >> 1) : crc >> 1;
            }
        }
        return ~crc;
    }

    public void Dispose()
    {
        _window.Close();
    }
}
