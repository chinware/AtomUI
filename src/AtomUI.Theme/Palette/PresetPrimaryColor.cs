using Avalonia.Media;

namespace AtomUI.Theme.Palette;

public enum PresetColorType
{
    Red,
    Volcano,
    Orange,
    Gold,
    Yellow,
    Lime,
    Green,
    Cyan,
    Blue,
    GeekBlue,
    Purple,
    Pink,
    Magenta,
    Grey
}

public class PresetPrimaryColor : IEquatable<PresetPrimaryColor>
{
    public static readonly PresetPrimaryColor Red = new(PresetColorType.Red);
    public static readonly PresetPrimaryColor Volcano = new(PresetColorType.Volcano);
    public static readonly PresetPrimaryColor Orange = new(PresetColorType.Orange);
    public static readonly PresetPrimaryColor Gold = new(PresetColorType.Gold);
    public static readonly PresetPrimaryColor Yellow = new(PresetColorType.Yellow);
    public static readonly PresetPrimaryColor Lime = new(PresetColorType.Lime);
    public static readonly PresetPrimaryColor Green = new(PresetColorType.Green);
    public static readonly PresetPrimaryColor Cyan = new(PresetColorType.Cyan);
    public static readonly PresetPrimaryColor Blue = new(PresetColorType.Blue);
    public static readonly PresetPrimaryColor GeekBlue = new(PresetColorType.GeekBlue);
    public static readonly PresetPrimaryColor Purple = new(PresetColorType.Purple);
    public static readonly PresetPrimaryColor Pink = new(PresetColorType.Pink);
    public static readonly PresetPrimaryColor Magenta = new(PresetColorType.Magenta);
    public static readonly PresetPrimaryColor Grey = new(PresetColorType.Grey);

    public PresetColorType Type { get; }

    private PresetPrimaryColor(PresetColorType colorType)
    {
        Type = colorType;
    }

    public string Name()
    {
        return Enum.GetName(typeof(PresetColorType), Type)!;
    }

    public string RgbHex()
    {
        return Type switch
        {
            PresetColorType.Red => "#F5222D",
            PresetColorType.Volcano => "#FA541C",
            PresetColorType.Orange => "#FA8C16",
            PresetColorType.Gold => "#FAAD14",
            PresetColorType.Yellow => "#FADB14",
            PresetColorType.Lime => "#A0D911",
            PresetColorType.Green => "#52C41A",
            PresetColorType.Cyan => "#13C2C2",
            PresetColorType.Blue => "#1677FF",
            PresetColorType.GeekBlue => "#2F54EB",
            PresetColorType.Purple => "#722ED1",
            PresetColorType.Magenta => "#EB2F96",
            PresetColorType.Pink => "#EB2F96",
            PresetColorType.Grey => "#666666",
            _ => "#666666"
        };
    }

    public Color Color()
    {
        return Avalonia.Media.Color.Parse(RgbHex());
    }

    public static IEnumerable<PresetPrimaryColor> AllColorTypes()
    {
        yield return Red;
        yield return Volcano;
        yield return Orange;
        yield return Gold;
        yield return Yellow;
        yield return Lime;
        yield return Green;
        yield return Cyan;
        yield return Blue;
        yield return GeekBlue;
        yield return Purple;
        yield return Pink;
        yield return Magenta;
        yield return Grey;
    }

    public static PresetPrimaryColor GetColor(PresetColorType type)
    {
        return type switch
        {
            PresetColorType.Red => Red,
            PresetColorType.Volcano => Volcano,
            PresetColorType.Orange => Orange,
            PresetColorType.Gold => Gold,
            PresetColorType.Yellow => Yellow,
            PresetColorType.Lime => Lime,
            PresetColorType.Green => Green,
            PresetColorType.Cyan => Cyan,
            PresetColorType.Blue => Blue,
            PresetColorType.GeekBlue => GeekBlue,
            PresetColorType.Purple => Purple,
            PresetColorType.Pink => Pink,
            PresetColorType.Magenta => Magenta,
            PresetColorType.Grey => Grey,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid value for PresetColorType")
        };
    }

    public bool Equals(PresetPrimaryColor? other)
    {
        return other is not null && Type == other.Type;
    }

    public override bool Equals(object? obj)
    {
        return obj is PresetPrimaryColor other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (int)Type;
    }

    public static bool operator ==(PresetPrimaryColor left, PresetPrimaryColor right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(PresetPrimaryColor left, PresetPrimaryColor right)
    {
        return !left.Equals(right);
    }
}