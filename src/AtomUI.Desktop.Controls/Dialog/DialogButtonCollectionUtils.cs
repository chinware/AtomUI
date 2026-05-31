using System.Collections;
using System.Collections.Generic;
using Avalonia.Collections;

namespace AtomUI.Desktop.Controls;

internal static class DialogButtonCollectionUtils
{
    public static void AddRange(AvaloniaList<DialogButton> target, IList source)
    {
        var buttons = CollectDialogButtons(source);
        if (buttons is not null)
        {
            target.AddRange(buttons);
        }
    }

    public static void RemoveAll(AvaloniaList<DialogButton> target, IList source)
    {
        var buttons = CollectDialogButtons(source);
        if (buttons is not null)
        {
            target.RemoveAll(buttons);
        }
    }

    private static List<DialogButton>? CollectDialogButtons(IList source)
    {
        List<DialogButton>? buttons = null;

        for (var i = 0; i < source.Count; i++)
        {
            if (source[i] is DialogButton button)
            {
                buttons ??= new List<DialogButton>(source.Count - i);
                buttons.Add(button);
            }
        }

        return buttons;
    }
}
