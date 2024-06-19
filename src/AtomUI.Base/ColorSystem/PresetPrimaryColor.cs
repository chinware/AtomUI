using Avalonia.Media;

namespace AtomUI.ColorSystem;

public class PresetPrimaryColor : IEquatable<PresetPrimaryColor>
{
   public enum ColorType
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

   public static readonly PresetPrimaryColor Red = new PresetPrimaryColor(ColorType.Red);
   public static readonly PresetPrimaryColor Volcano = new PresetPrimaryColor(ColorType.Volcano);
   public static readonly PresetPrimaryColor Orange = new PresetPrimaryColor(ColorType.Orange);
   public static readonly PresetPrimaryColor Gold = new PresetPrimaryColor(ColorType.Gold);
   public static readonly PresetPrimaryColor Yellow = new PresetPrimaryColor(ColorType.Yellow);
   public static readonly PresetPrimaryColor Lime = new PresetPrimaryColor(ColorType.Lime);
   public static readonly PresetPrimaryColor Green = new PresetPrimaryColor(ColorType.Green);
   public static readonly PresetPrimaryColor Cyan = new PresetPrimaryColor(ColorType.Cyan);
   public static readonly PresetPrimaryColor Blue = new PresetPrimaryColor(ColorType.Blue);
   public static readonly PresetPrimaryColor GeekBlue = new PresetPrimaryColor(ColorType.GeekBlue);
   public static readonly PresetPrimaryColor Purple = new PresetPrimaryColor(ColorType.Purple);
   public static readonly PresetPrimaryColor Pink = new PresetPrimaryColor(ColorType.Pink);
   public static readonly PresetPrimaryColor Magenta = new PresetPrimaryColor(ColorType.Magenta);
   public static readonly PresetPrimaryColor Grey = new PresetPrimaryColor(ColorType.Grey);
   
   public ColorType Type { get; }

   public PresetPrimaryColor(ColorType colorType)
   {
      Type = colorType;
   }

   public string Name()
   {
      return Enum.GetName(typeof(ColorType), Type)!;
   }

   public string RgbHex()
   {
      return Type switch
      {
         ColorType.Red => "#F5222D",
         ColorType.Volcano => "#FA541C",
         ColorType.Orange => "#FA8C16",
         ColorType.Gold => "#FAAD14",
         ColorType.Yellow => "#FADB14",
         ColorType.Lime => "#A0D911",
         ColorType.Green => "#52C41A",
         ColorType.Cyan => "#13C2C2",
         ColorType.Blue => "#1677FF",
         ColorType.GeekBlue => "#2F54EB",
         ColorType.Purple => "#722ED1",
         ColorType.Magenta => "#EB2F96",
         ColorType.Pink => "#EB2F96",
         ColorType.Grey => "#666666",
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
         new PresetPrimaryColor(ColorType.Red),
         new PresetPrimaryColor(ColorType.Volcano),
         new PresetPrimaryColor(ColorType.Orange),
         new PresetPrimaryColor(ColorType.Gold),
         new PresetPrimaryColor(ColorType.Yellow),
         new PresetPrimaryColor(ColorType.Lime),
         new PresetPrimaryColor(ColorType.Green),
         new PresetPrimaryColor(ColorType.Cyan),
         new PresetPrimaryColor(ColorType.Blue),
         new PresetPrimaryColor(ColorType.GeekBlue),
         new PresetPrimaryColor(ColorType.Purple),
         new PresetPrimaryColor(ColorType.Pink),
         new PresetPrimaryColor(ColorType.Magenta),
         new PresetPrimaryColor(ColorType.Grey),
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
