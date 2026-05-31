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

    private TreeNodePath(string[] trustedSegments, bool _)
    {
        _segmentsArray = trustedSegments;
        Segments        = Array.AsReadOnly(_segmentsArray);
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
        if (ReferenceEquals(this, other) || other.Length == 0)
        {
            return true;
        }
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
    
    public TreeNodePath? GetParent()
    {
        if (_segmentsArray.Length == 0)
        {
            return null;
        }
        if (_segmentsArray.Length == 1)
        {
            return Empty;
        }

        var newSegments = new string[_segmentsArray.Length - 1];
        Array.Copy(_segmentsArray, newSegments, newSegments.Length);
        return new TreeNodePath(newSegments, true);
    }
    
    public TreeNodePath Append(string segment)
    {
        ArgumentException.ThrowIfNullOrEmpty(segment);

        if (segment.Contains('/'))
        {
            throw new ArgumentException("Segments cannot contain '/'", nameof(segment));
        }
        
        var newSegments = new string[_segmentsArray.Length + 1];
        Array.Copy(_segmentsArray, newSegments, _segmentsArray.Length);
        newSegments[^1] = segment;
        return new TreeNodePath(newSegments, true);
    }
    
    public TreeNodePath Append(TreeNodePath other)
    {
        if (other.Length == 0)
        {
            return this;
        }
        if (Length == 0)
        {
            return other;
        }

        var newSegments = new string[_segmentsArray.Length + other.Length];
        Array.Copy(_segmentsArray, newSegments, _segmentsArray.Length);
        Array.Copy(other._segmentsArray, 0, newSegments, _segmentsArray.Length, other.Length);
        
        return new TreeNodePath(newSegments, true);
    }
    
    public TreeNodePath WithSegment(int index, string newValue)
    {
        if ((uint)index >= (uint)_segmentsArray.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        ArgumentException.ThrowIfNullOrEmpty(newValue);
        if (newValue.Contains('/'))
        {
            throw new ArgumentException("Segments cannot contain '/'", nameof(newValue));
        }
        if (string.Equals(_segmentsArray[index], newValue, StringComparison.Ordinal))
        {
            return this;
        }

        var newSegments = new string[_segmentsArray.Length];
        Array.Copy(_segmentsArray, newSegments, _segmentsArray.Length);
        newSegments[index] = newValue;
        
        return new TreeNodePath(newSegments, true);
    }
}
