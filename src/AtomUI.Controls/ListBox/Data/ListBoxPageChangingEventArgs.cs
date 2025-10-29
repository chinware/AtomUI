using System.ComponentModel;

namespace AtomUI.Controls.Data;

public sealed class ListBoxPageChangingEventArgs : CancelEventArgs
{
    /// <summary>
    /// Constructor that takes the target page index
    /// </summary>
    /// <param name="newPageIndex">Index of the requested page</param>
    public ListBoxPageChangingEventArgs(int newPageIndex)
    {
        NewPageIndex = newPageIndex;
    }

    /// <summary>
    /// Gets the index of the requested page
    /// </summary>
    public int NewPageIndex
    {
        get;
        private set;
    }
}