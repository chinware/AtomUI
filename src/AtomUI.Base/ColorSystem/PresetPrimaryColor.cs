using Avalonia.Media;

namespace AtomUI.ColorSystem;

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
   Grey,
}

public class PresetPrimaryColor : IEquatable<PresetPrimaryColor>
{


   public static readonly PresetPrimaryColor Red = new PresetPrimaryColor(PresetColorType.Red);
   public static readonly PresetPrimaryColor Volcano = new PresetPrimaryColor(PresetColorType.Volcano);
   public static readonly PresetPrimaryColor Orange = new PresetPrimaryColor(PresetColorType.Orange);
   public static readonly PresetPrimaryColor Gold = new PresetPrimaryColor(PresetColorType.Gold);
   public static readonly PresetPrimaryColor Yellow = new PresetPrimaryColor(PresetColorType.Yellow);
   public static readonly PresetPrimaryColor Lime = new PresetPrimaryColor(PresetColorType.Lime);
   public static readonly PresetPrimaryColor Green = new PresetPrimaryColor(PresetColorType.Green);
   public static readonly PresetPrimaryColor Cyan = new PresetPrimaryColor(PresetColorType.Cyan);
   public static readonly PresetPrimaryColor Blue = new PresetPrimaryColor(PresetColorType.Blue);
   public static readonly PresetPrimaryColor GeekBlue = new PresetPrimaryColor(PresetColorType.GeekBlue);
   public static readonly PresetPrimaryColor Purple = new PresetPrimaryColor(PresetColorType.Purple);
   public static readonly PresetPrimaryColor Pink = new PresetPrimaryColor(PresetColorType.Pink);
   public static readonly PresetPrimaryColor Magenta = new PresetPrimaryColor(PresetColorType.Magenta);
   public static readonly PresetPrimaryColor Grey = new PresetPrimaryColor(PresetColorType.Grey);
   
   public PresetColorType Type { get; }

   public PresetPrimaryColor(PresetColorType colorType)
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

   public static IList<PresetPrimaryColor> AllColorTypes()
   {
      return new List<PresetPrimaryColor>
      {
         new PresetPrimaryColor(PresetColorType.Red),
         new PresetPrimaryColor(PresetColorType.Volcano),
         new PresetPrimaryColor(PresetColorType.Orange),
         new PresetPrimaryColor(PresetColorType.Gold),
         new PresetPrimaryColor(PresetColorType.Yellow),
         new PresetPrimaryColor(PresetColorType.Lime),
         new PresetPrimaryColor(PresetColorType.Green),
         new PresetPrimaryColor(PresetColorType.Cyan),
         new PresetPrimaryColor(PresetColorType.Blue),
         new PresetPrimaryColor(PresetColorType.GeekBlue),
         new PresetPrimaryColor(PresetColorType.Purple),
         new PresetPrimaryColor(PresetColorType.Pink),
         new PresetPrimaryColor(PresetColorType.Magenta),
         new PresetPrimaryColor(PresetColorType.Grey),
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
