using System.Globalization;
using System.Text.RegularExpressions;
using AtomUI.Utils;

namespace AtomUI;

public enum DimensionUnitType
{
    Percentage = 0,
    Pixel = 1,
}

public struct Dimension : IEquatable<Dimension>
{
    public DimensionUnitType UnitType => _type;

    public bool IsPercentage => _type == DimensionUnitType.Percentage;

    public bool IsAbsolute => _type == DimensionUnitType.Pixel;

    public double Value => _value;

    private readonly DimensionUnitType _type = DimensionUnitType.Pixel;
    private readonly double _value = 0;

    public Dimension()
    {
    }

    public Dimension(double value)
        : this(value, DimensionUnitType.Pixel)
    {
    }

    public Dimension(double value, DimensionUnitType type)
    {
        if (type == DimensionUnitType.Percentage &&
            (double.IsNaN(value) || double.IsInfinity(value) || value < 0 || value > 100))
        {
            throw new ArgumentException("Invalid value for Percentage unit, value must in [0, 100]", nameof(value));
        }

        _type  = type;
        _value = value;
    }

    public double Resolve(double referenceSize)
    {
        return _type switch
        {
            DimensionUnitType.Percentage => _value * referenceSize / 100.0,
            DimensionUnitType.Pixel => _value,
            _ => _value
        };
    }

    private static void ValidateSameUnit(Dimension a, Dimension b)
    {
        if (a._type != b._type)
        {
            throw new InvalidOperationException("Cannot perform operation on dimensions with different units");
        }
    }

    #region 自定义四则运算符

    public static Dimension operator +(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return new Dimension(a._value + b._value, a._type);
    }

    public static Dimension operator +(Dimension a, double b)
    {
        return new Dimension(a._value + b, a._type);
    }

    public static Dimension operator +(double a, Dimension b)
    {
        return new Dimension(a + b._value, b._type);
    }

    public static Dimension operator -(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return new Dimension(a._value - b._value, a._type);
    }

    public static Dimension operator -(Dimension a, double b)
    {
        return new Dimension(a._value - b, a._type);
    }

    public static Dimension operator -(double a, Dimension b)
    {
        return new Dimension(a - b._value, b._type);
    }

    public static Dimension operator *(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return new Dimension(a._value * b._value, a._type);
    }

    public static Dimension operator *(Dimension a, double b)
    {
        return new Dimension(a._value * b, a._type);
    }

    public static Dimension operator *(double a, Dimension b)
    {
        return new Dimension(a * b._value, b._type);
    }

    public static Dimension operator /(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        if (Math.Abs(b._value) < double.Epsilon)
        {
            throw new DivideByZeroException("Division by zero");
        }

        return new Dimension(a._value / b._value, a._type);
    }

    public static Dimension operator /(Dimension a, double b)
    {
        if (Math.Abs(b) < double.Epsilon)
        {
            throw new DivideByZeroException("Division by zero");
        }

        return new Dimension(a._value / b, a._type);
    }

    public static Dimension operator /(double a, Dimension b)
    {
        if (Math.Abs(b._value) < double.Epsilon)
        {
            throw new DivideByZeroException("Division by zero");
        }

        return new Dimension(a / b._value, b._type);
    }

    public static Dimension operator %(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        if (Math.Abs(b._value) < double.Epsilon)
        {
            throw new DivideByZeroException("Modulo by zero");
        }

        return new Dimension(a._value % b._value, a._type);
    }

    public static Dimension operator %(Dimension a, double b)
    {
        if (Math.Abs(b) < double.Epsilon)
        {
            throw new DivideByZeroException("Modulo by zero");
        }

        return new Dimension(a._value % b, a._type);
    }

    public static Dimension operator +(Dimension a) => a;

    public static Dimension operator -(Dimension a)
    {
        return new Dimension(-a._value, a._type);
    }

    #endregion

    public static bool operator ==(Dimension a, Dimension b)
    {
        return (MathUtils.AreClose(a._value, b._value) && a._type == b._type);
    }

    public static bool operator !=(Dimension gl1, Dimension gl2)
    {
        return !(gl1 == gl2);
    }

    public static bool operator <(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return a._value < b._value;
    }

    public static bool operator >(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return a._value > b._value;
    }

    public static bool operator <=(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return a._value <= b._value;
    }

    public static bool operator >=(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return a._value >= b._value;
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
        return HashCode.Combine(_value, _type);
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

    public static Dimension Min(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return new Dimension(Math.Min(a._value, b._value), a._type);
    }

    public static Dimension Max(Dimension a, Dimension b)
    {
        ValidateSameUnit(a, b);
        return new Dimension(Math.Max(a._value, b._value), a._type);
    }

    public static Dimension Clamp(Dimension value, Dimension min, Dimension max)
    {
        ValidateSameUnit(value, min);
        ValidateSameUnit(value, max);
        return new Dimension(Math.Clamp(value._value, min._value, max._value), value._type);
    }
}