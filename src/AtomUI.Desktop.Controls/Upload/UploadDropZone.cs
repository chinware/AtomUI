using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class UploadDropZone : ContentControl
{
    static UploadDropZone()
    {
        DragDrop.DropEvent.AddClassHandler<UploadDropZone>((zone, args) => zone.HandleDrop(args));
    }

    public UploadDropZone()
    {
        DragDrop.SetAllowDrop(this, true);
    }

    private void HandleDrop(DragEventArgs e)
    {
        var files = CollectStorageFiles(e);
        if (files.Count == 0)
        {
            return;
        }

        Dispatcher.InvokeAsync(async () =>
        {
            var owner = this.FindAncestorOfType<Upload>();
            if (owner is not null)
            {
                await owner.EnqueueStorageFilesAsync(files);
            }
        });
    }

    private static IReadOnlyList<IStorageFile> CollectStorageFiles(DragEventArgs e)
    {
        List<IStorageFile>? files = null;
        foreach (var item in e.DataTransfer.Items)
        {
            var raw = item.TryGetRaw(DataFormat.File);
            if (raw is IStorageFile file)
            {
                files ??= new List<IStorageFile>(e.DataTransfer.Items.Count);
                files.Add(file);
            }
        }

        return files ?? [];
    }
}
