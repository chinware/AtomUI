namespace AtomUI.Desktop.Controls;

public readonly struct DataGridColumnSortState : IEquatable<DataGridColumnSortState>
{
    private readonly int _priorityPlusOne;

    internal DataGridColumnSortState(DataGridSortDirection direction, int priority)
    {
        DataGridQueryValidation.ValidateDirection(direction, nameof(direction));
        if (priority < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(priority));
        }

        Direction = direction;
        _priorityPlusOne = checked(priority + 1);
    }

    public static DataGridColumnSortState None => default;

    public DataGridSortDirection? Direction { get; }

    public int Priority => Direction.HasValue ? _priorityPlusOne - 1 : -1;

    public bool Equals(DataGridColumnSortState other) =>
        Direction == other.Direction && Priority == other.Priority;

    public override bool Equals(object? obj) =>
        obj is DataGridColumnSortState other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Direction, Priority);

    public static bool operator ==(
        DataGridColumnSortState left,
        DataGridColumnSortState right) => left.Equals(right);

    public static bool operator !=(
        DataGridColumnSortState left,
        DataGridColumnSortState right) => !left.Equals(right);
}
