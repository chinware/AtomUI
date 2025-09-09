using System.Globalization;
using System.Text.RegularExpressions;
using AtomUI.Utils;

namespace AtomUI;

public enum DimensionUnitType
{
    Percentage = 0,
    Pixel = 1,
}

public struct Dimension: IEquatable<Dimension>
{
    private readonly DimensionUnitType _type;
    private readonly double _value;
    
    public Dimension(double value)
        : this(value, DimensionUnitType.Pixel)
    {
    }
      
    public Dimension(double value, DimensionUnitType type)
    {
        if (value < 0 || double.IsNaN(value) || double.IsInfinity(value))
        {
            throw new ArgumentException("Invalid value", nameof(value));
        }

        if (type < DimensionUnitType.Percentage || type > DimensionUnitType.Pixel)
        {
            throw new ArgumentException("Invalid value", nameof(type));
        }

        _type  = type;
        _value = value;
    }
        
    public DimensionUnitType UnitType => _type;
        
    public bool IsAbsolute => _type == DimensionUnitType.Pixel;
        
    public bool IsPercentage => _type == DimensionUnitType.Percentage;
        
    public bool IsPixel => _type == DimensionUnitType.Pixel;
        
    public double Value => _value;
        
    public static bool operator ==(Dimension a, Dimension b)
    {
        return (MathUtils.AreClose(a._value, b._value) && a._type == b._type);
    }
        
    public static bool operator !=(Dimension gl1, Dimension gl2)
    {
        return !(gl1 == gl2);
    }
    
    public override bool Equals(object? o)
    {
        if (o == null)
        {
            return false;
        }

        if (!(o is Dimension))
        {
            return false;
        }

        return this == (Dimension)o;
    }
        
    public bool Equals(Dimension gridLength)
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
        
    public static Dimension Parse(string s)
    {
        s = s.ToUpperInvariant();
        var match = Regex.Match(s.Trim(), @"^([+-]?\d*\.?\d+)\s*([a-zA-Z%]+)?$", RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            throw new FormatException($"Invalid width format: '{s}'");
        }

        DimensionUnitType unit = DimensionUnitType.Pixel;
        if (match.Groups[2].Value == "%")
        {
            unit = DimensionUnitType.Percentage;
        }
  
        var value = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
        return new Dimension(value, unit);
    }

    public static IEnumerable<Dimension> ParseWidths(string s)
    {
        var result = new List<Dimension>();
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