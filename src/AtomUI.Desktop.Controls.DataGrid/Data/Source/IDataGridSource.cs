namespace AtomUI.Desktop.Controls;

public interface IDataGridSource
{
    DataGridSourceSchema Schema { get; }

    ValueTask<DataGridRangeResult> FetchAsync(
        DataGridFetchRequest request,
        CancellationToken cancellationToken);

    event EventHandler? Invalidated;
}
