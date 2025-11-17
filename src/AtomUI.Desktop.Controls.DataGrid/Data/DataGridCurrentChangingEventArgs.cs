// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace AtomUI.Controls.Data;

public class DataGridCurrentChangingEventArgs : EventArgs
{
    private bool _cancel;
    private bool _isCancelable;
    
    /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.CurrentChangingEventArgs" /> class and sets the <see cref="P:System.ComponentModel.CurrentChangingEventArgs.IsCancelable" /> property to true.</summary>
    public DataGridCurrentChangingEventArgs()
    {
        Initialize(true);
    }
    
    /// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.CurrentChangingEventArgs" /> class and sets the <see cref="P:System.ComponentModel.CurrentChangingEventArgs.IsCancelable" /> property to the specified value.</summary>
    /// <param name="isCancelable">true to disable the ability to cancel a <see cref="P:System.ComponentModel.ICollectionView.CurrentItem" /> change; false to enable cancellation.</param>
    public DataGridCurrentChangingEventArgs(bool isCancelable)
    {
        Initialize(isCancelable);
    }

    private void Initialize(bool isCancelable)
    {
        _isCancelable = isCancelable;
    }
    
    /// <summary>Gets a value that indicates whether the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem" /> change can be canceled. </summary>
    /// <returns>true if the event can be canceled; false if the event cannot be canceled.</returns>
    public bool IsCancelable
    {
        get
        {
            return _isCancelable;
        }
    }

    /// <summary>Gets or sets a value that indicates whether the <see cref="P:System.ComponentModel.ICollectionView.CurrentItem" /> change should be canceled. </summary>
    /// <returns>true if the event should be canceled; otherwise, false. The default is false.</returns>
    /// <exception cref="T:System.InvalidOperationException">The <see cref="P:System.ComponentModel.CurrentChangingEventArgs.IsCancelable" /> property value is false.</exception>
    public bool Cancel
    {
        get
        {
            return _cancel;
        }
        set
        {
            if (IsCancelable)
                _cancel = value;
            else if (value)
                throw new InvalidOperationException("CurrentChanging Cannot Be Canceled");
        }
    }
}