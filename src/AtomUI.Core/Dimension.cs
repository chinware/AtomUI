using System.Globalization;
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
        if (TryParse(s.AsSpan(), out var dimension))
        {
            return dimension;
        }

        throw new FormatException($"Invalid width format: '{s.ToUpperInvariant()}'");
    }

    public static IEnumerable<Dimension> ParseWidths(string s)
    {
        var result = new List<Dimension>(CountWidthTokens(s.AsSpan()));
        using (var tokenizer = new SpanStringTokenizer(s, CultureInfo.InvariantCulture))
        {
            while (tokenizer.TryReadSpan(out var item))
            {
                result.Add(Parse(item));
            }
        }

        return result;
    }

    private static int CountWidthTokens(ReadOnlySpan<char> s)
    {
        var count   = 0;
        var inToken = false;
        for (var i = 0; i < s.Length; ++i)
        {
            var c = s[i];
            if (char.IsWhiteSpace(c) || c == ',')
            {
                inToken = false;
            }
            else if (!inToken)
            {
                count++;
                inToken = true;
            }
        }

        return count;
    }

    private static Dimension Parse(ReadOnlySpan<char> s)
    {
        if (TryParse(s, out var dimension))
        {
            return dimension;
        }

        throw new FormatException($"Invalid width format: '{s.ToString().ToUpperInvariant()}'");
    }

    private static bool TryParse(ReadOnlySpan<char> s, out Dimension dimension)
    {
        dimension = default;
        var span = s.Trim();
        if (span.IsEmpty)
        {
            return false;
        }

        var index = 0;
        if (span[index] == '+' || span[index] == '-')
        {
            index++;
            if (index == span.Length)
            {
                return false;
            }
        }

        var beforeDecimalDigitCount = 0;
        while (index < span.Length && IsAsciiDigit(span[index]))
        {
            beforeDecimalDigitCount++;
            index++;
        }

        if (index < span.Length && span[index] == '.')
        {
            index++;
            var afterDecimalDigitStart = index;
            while (index < span.Length && IsAsciiDigit(span[index]))
            {
                index++;
            }

            if (index == afterDecimalDigitStart)
            {
                return false;
            }
        }
        else if (beforeDecimalDigitCount == 0)
        {
            return false;
        }

        var valueSpan = span.Slice(0, index);
        while (index < span.Length && char.IsWhiteSpace(span[index]))
        {
            index++;
        }

        var unit = DimensionUnitType.Pixel;
        if (index < span.Length)
        {
            var unitStart = index;
            while (index < span.Length && IsDimensionUnitChar(span[index]))
            {
                index++;
            }

            if (index != span.Length)
            {
                return false;
            }

            var unitSpan = span.Slice(unitStart, index - unitStart);
            if (unitSpan.Length == 1 && unitSpan[0] == '%')
            {
                unit = DimensionUnitType.Percentage;
            }
        }

        var value = double.Parse(valueSpan, CultureInfo.InvariantCulture);
        dimension = new Dimension(value, unit);
        return true;
    }

    private static bool IsAsciiDigit(char value)
    {
        return value >= '0' && value <= '9';
    }

    private static bool IsDimensionUnitChar(char value)
    {
        return value == '%' ||
               value is >= 'a' and <= 'z' ||
               value is >= 'A' and <= 'Z';
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
