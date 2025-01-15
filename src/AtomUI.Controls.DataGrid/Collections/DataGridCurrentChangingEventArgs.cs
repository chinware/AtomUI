// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace AtomUI.Controls.Collections;

public class DataGridCurrentChangingEventArgs : EventArgs
{
    private bool _cancel;
    private bool _isCancelable;

    public bool IsCancelable => _isCancelable;

    public bool Cancel
    {
        get => _cancel;

        set
        {
            if (IsCancelable)
            {
                _cancel = value;
            }
            else if (value)
            {
                throw new InvalidOperationException("CurrentChanging Cannot Be Canceled");
            }
        }
    }

    public DataGridCurrentChangingEventArgs()
    {
        _isCancelable = true;
    }

    public DataGridCurrentChangingEventArgs(bool isCancelable)
    {
        _isCancelable = isCancelable;
    }
}