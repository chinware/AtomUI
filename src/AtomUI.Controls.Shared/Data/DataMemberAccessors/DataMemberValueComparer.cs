using System.Collections;

namespace AtomUI.Controls.Data;

internal sealed class DataMemberValueComparer<TValue> : IComparer
{
    public static readonly DataMemberValueComparer<TValue> Default = new();

    private DataMemberValueComparer()
    {
    }

    public int Compare(object? x, object? y)
    {
        if (x is null)
        {
            return y is null ? 0 : -1;
        }

        if (y is null)
        {
            return 1;
        }

        return Comparer<TValue>.Default.Compare((TValue)x, (TValue)y);
    }
}
