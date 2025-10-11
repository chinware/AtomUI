namespace AtomUI.Controls.Primitives;

public sealed record TreeNodePath
{
    private readonly string[] _segmentsArray;
    
    public IReadOnlyList<string> Segments { get; }
    
    public static TreeNodePath Empty { get; } = new TreeNodePath([]);
    
    public TreeNodePath(string[] segments)
    {
        _segmentsArray = ValidateAndCopySegments(segments);
        Segments = Array.AsReadOnly(_segmentsArray);
    }
    
    public TreeNodePath(string pathString)
    {
        if (pathString.Length == 0)
        {
            _segmentsArray = [];
            Segments = Array.AsReadOnly(_segmentsArray);
            return;
        }
        
        _segmentsArray = ValidateAndCopySegments(
            pathString.Split('/', StringSplitOptions.RemoveEmptyEntries));
        Segments = Array.AsReadOnly(_segmentsArray);
    }

    private static string[] ValidateAndCopySegments(string[] segments)
    {
        ArgumentNullException.ThrowIfNull(segments);
        
        var result = new string[segments.Length];
        
        for (int i = 0; i < segments.Length; i++)
        {
            var segment = segments[i];

            if (segment.Length == 0)
            {
                throw new ArgumentException($"The {i}th path segment is an empty string", nameof(segments));
            }

            if (segment.Contains('/'))
            {
                throw new ArgumentException($"Path segment '{segment}' {i}th contains illegal character '/'", nameof(segments));
            }
            
            result[i] = segment;
        }
        
        return result;
    }
    
    public string this[int index] => _segmentsArray[index];

    public int Length => _segmentsArray.Length;

    public override string ToString() => string.Join("/", _segmentsArray);


    public bool StartsWith(TreeNodePath other)
    {
        if (other.Length > Length)
        {
            return false;
        }
        
        for (int i = 0; i < other.Length; i++)
        {
            if (!string.Equals(_segmentsArray[i], other._segmentsArray[i], StringComparison.Ordinal))
            {
                return false;
            }
        }
        return true;
    }
    
    public TreeNodePath? GetParent() =>
        _segmentsArray.Length > 0 
            ? new TreeNodePath(_segmentsArray[..^1]) 
            : null;
    
    public TreeNodePath Append(string segment)
    {
        ArgumentException.ThrowIfNullOrEmpty(segment);

        if (segment.Contains('/'))
        {
            throw new ArgumentException("Segments cannot contain '/'", nameof(segment));
        }
        
        var newSegments = new string[_segmentsArray.Length + 1];
        _segmentsArray.CopyTo(newSegments, 0);
        newSegments[^1] = segment;
        return new TreeNodePath(newSegments);
    }
    
    public TreeNodePath Append(TreeNodePath other)
    {
        if (other.Length == 0)
        {
            return this;
        }
        var newSegments = new string[_segmentsArray.Length + other.Length];
        _segmentsArray.CopyTo(newSegments, 0);
        other._segmentsArray.CopyTo(newSegments, _segmentsArray.Length);
        
        return new TreeNodePath(newSegments);
    }
    
    public TreeNodePath WithSegment(int index, string newValue)
    {
        if ((uint)index >= (uint)_segmentsArray.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        var newSegments = _segmentsArray.ToArray();
        newSegments[index] = newValue ?? throw new ArgumentNullException(nameof(newValue));
        
        return new TreeNodePath(newSegments);
    }
}