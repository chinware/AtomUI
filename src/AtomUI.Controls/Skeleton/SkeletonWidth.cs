using System.Globalization;
using System.Text.RegularExpressions;
using AtomUI.Utils;

namespace AtomUI.Controls;

public enum SkeletonUnitType
{
    Percentage = 0,
    Pixel = 1,
}

public struct SkeletonWidth : IEquatable<SkeletonWidth>
{
    private readonly SkeletonUnitType _type;
    private readonly double _value;
    
    public SkeletonWidth(double value)
        : this(value, SkeletonUnitType.Pixel)
    {
    }
      
    public SkeletonWidth(double value, SkeletonUnitType type)
    {
        if (value < 0 || double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentException("Invalid value", nameof(value));
        }

        if (type < SkeletonUnitType.Percentage || type > SkeletonUnitType.Pixel)
        {
            throw new ArgumentException("Invalid value", nameof(type));
        }

        _type  = type;
        _value = value;
    }
        
    public SkeletonUnitType SkeletonUnitType => _type;
        
    public bool IsAbsolute => _type == SkeletonUnitType.Pixel;
        
    public bool IsPercentage => _type == SkeletonUnitType.Percentage;
        
    public bool IsPixel => _type == SkeletonUnitType.Pixel;
        
    public double Value => _value;
        
    public static bool operator ==(SkeletonWidth a, SkeletonWidth b)
    {
        return (MathUtils.AreClose(a._value, b._value) && a._type == b._type);
    }
        
    public static bool operator !=(SkeletonWidth gl1, SkeletonWidth gl2)
    {
        return !(gl1 == gl2);
    }
    
    public override bool Equals(object? o)
    {
        if (o == null)
        {
            return false;
        }

        if (!(o is SkeletonWidth))
        {
            return false;
        }

        return this == (SkeletonWidth)o;
    }
        
    public bool Equals(SkeletonWidth gridLength)
    {
        return this == gridLength;
    }
        
    public override int GetHashCode()
    {
        return _value.GetHashCode() ^ _type.GetHashCode();
    }
        
    public override string ToString()
    {
        var vstr = _value.ToString(CultureInfo.InvariantCulture);
        if (IsPercentage)
        {
            return $"{vstr}%";
        }
        return $"{vstr}px";
    }
        
    public static SkeletonWidth Parse(string s)
    {
        s = s.ToUpperInvariant();
        var match = Regex.Match(s.Trim(), @"^([+-]?\d*\.?\d+)\s*([a-zA-Z%]+)?$", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            throw new FormatException($"Invalid width format: '{s}'");
        }

        SkeletonUnitType unit = SkeletonUnitType.Pixel;
        if (match.Groups[2].Value == "%")
        {
            unit = SkeletonUnitType.Percentage;
        }
  
        var value = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        return new SkeletonWidth(value, unit);
    }

    public static IEnumerable<SkeletonWidth> ParseWidths(string s)
    {
        var result = new List<SkeletonWidth>();
        using (var tokenizer = new SpanStringTokenizer(s, CultureInfo.InvariantCulture))
        {
            while (tokenizer.TryReadString(out var item))
            {
                result.Add(Parse(item));
            }
        }

        return result;
    }
}