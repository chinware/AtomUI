
using System;
using System.Globalization;
using Avalonia.Utilities;

namespace AtomUI.Controls;

/**
 * Bootstrap的默认断点是xs（<576px）、sm（≥576px）、md（≥768px）、lg（≥992px）、xl（≥1200px）、xxl（≥1400px）
 *
 * "24 32 40 64 80 100"
 *
 */

public enum AvatarStatus
{
    STATUS_NUM,
    STATUS_ENUM,
    STATUS_RESPONSIVE
    
}

public readonly struct AvatarSize : IEquatable<AvatarSize>
{
    
    private readonly double _xs;
    private readonly double _sm;
    private readonly double _md;
    private readonly double _lg;
    private readonly double _xl;
    private readonly double _xxl;

    private readonly double _size;

    private readonly AvatarSizeType _type;

    public readonly AvatarStatus Status;
    
    public AvatarSize(double size)
    {
        _size = size;
        _type = AvatarSizeType.Default;
        _xs = 0;
        _sm = 0;
        _md = 0;
        _lg = 0;
        _xl = 0;
        _xxl = 0;
        Status = AvatarStatus.STATUS_NUM;
    }
    
    public AvatarSize(double xs, double sm, double md, double lg, double xl, double xxl)
    {
        _type = AvatarSizeType.Default;
        _size = md;
        _xs = xs;
        _sm = sm;
        _md = md;
        _lg = lg;
        _xl = xl;
        _xxl = xxl;
        Status = Status = AvatarStatus.STATUS_RESPONSIVE;
    }
    
    public AvatarSize(AvatarSizeType sizeType)
    {
        _type = sizeType;
        /*switch (sizeType)
        {
            case AvatarSizeType.Large:
                _size = 40;
                break;
            case AvatarSizeType.Small:
                _size = 24;
                break;
            default:
                _size = 32;
                break;
        }*/
       
        _xs = 0;
        _sm = 0;
        _md = 0;
        _lg = 0;
        _xl = 0;
        _xxl = 0;
        Status = AvatarStatus.STATUS_ENUM;
    }

    public bool Equals(AvatarSize other)
    {
        return _type == other._type &&
               _size ==other._size &&
               _xs == other._xs &&
               _sm == other._sm &&
               _md == other._md &&
               _lg == other._lg &&
               _xl == other._xl &&
               _xxl == other._xxl;
    }

    public double Xs => _xs;
    public double Sm => _sm;
    public double Md => _md;
    public double Lg => _lg;
    public double Xl => _xl;
    public double Xxl => _xxl;

    public double Size => _size;

    public AvatarSizeType Type => _type;
    
    public static bool operator ==(AvatarSize a, AvatarSize b)
    {
        return a.Equals(b);
    }
    
    public static bool operator !=(AvatarSize a, AvatarSize b)
    {
        return !a.Equals(b);
    }
    
    public static AvatarSize Parse(string s)
    {
        if ("large".Equals(s.ToLower()))
        {
            return new AvatarSize(AvatarSizeType.Large);
        } 
        
        if ("small".Equals(s.ToLower()))
        {
            return new AvatarSize(AvatarSizeType.Small);
        }
        
        if ("default".Equals(s.ToLower()))
        {
            return new AvatarSize(AvatarSizeType.Default);
        }
        const string exceptionMessage = "Invalid AvatarSize.";
        
        using (var tokenizer = new StringTokenizer(s, CultureInfo.InvariantCulture, exceptionMessage))
        {
            if (tokenizer.TryReadDouble(out var a))
            {
                if (tokenizer.TryReadDouble(out var b))
                {
                    if (tokenizer.TryReadDouble(out var c))
                    {
                        if (tokenizer.TryReadDouble(out var d))
                        {
                            if (tokenizer.TryReadDouble(out var e))
                            {
                                return new AvatarSize(a, b, c, d,e,tokenizer.ReadDouble());
                            }
                        }
                    }
                }
                else
                {
                    return new AvatarSize(a);
                }
            }

            throw new FormatException(exceptionMessage);
        }
    }
    
    public override string ToString()
    {
        return FormattableString.Invariant($"{_size},{_xs},{_sm},{_md},{_lg},{_xl},{_xxl},{_type}");
    }
}