using AtomUI.Theme.Palette;
using AtomUI.Theme.Styling;
using Avalonia.Controls;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public class MapDesignToken : AbstractDesignToken
{
   public SeedDesignToken SeedToken { get; set; }
   public StyleMapDesignToken StyleToken { get; set; }
   public ColorMapDesignToken ColorToken { get; set; }
   public SizeMapDesignToken SizeToken { get; set; }
   public HeightMapDesignToken HeightToken { get; set; }
   public FontMapDesignToken FontToken { get; set; }

   public IDictionary<PresetPrimaryColor, ColorMap> ColorPalettes { get; set; }
   
   public MapDesignToken()
   {
      SeedToken = new SeedDesignToken();
      StyleToken = new StyleMapDesignToken();
      ColorToken = new ColorMapDesignToken();
      SizeToken = new SizeMapDesignToken();
      HeightToken = new HeightMapDesignToken();
      FontToken = new FontMapDesignToken();
      ColorPalettes = new Dictionary<PresetPrimaryColor, ColorMap>();
   }

   public void SetColorPalette(PresetPrimaryColor primaryColor, ColorMap colorMap)
   {
      ColorPalettes[primaryColor] = colorMap;
   }
   
   public ColorMap? GetColorPalette(PresetPrimaryColor primaryColor)
   {
      ColorMap? value;
      if (ColorPalettes.TryGetValue(primaryColor, out value)) {
         return value;
      }

      return null;
   }
   
   public override void BuildResourceDictionary(IResourceDictionary dictionary)
   {
      SeedToken.BuildResourceDictionary(dictionary);
      StyleToken.BuildResourceDictionary(dictionary);
      ColorToken.BuildResourceDictionary(dictionary);
      SizeToken.BuildResourceDictionary(dictionary);
      HeightToken.BuildResourceDictionary(dictionary);
      FontToken.BuildResourceDictionary(dictionary);
      base.BuildResourceDictionary(dictionary);
   }

   internal override void LoadConfig(IDictionary<string, string> tokenConfigInfo)
   {
      SeedToken.LoadConfig(tokenConfigInfo);
      StyleToken.LoadConfig(tokenConfigInfo);
      ColorToken.LoadConfig(tokenConfigInfo);
      SizeToken.LoadConfig(tokenConfigInfo);
      HeightToken.LoadConfig(tokenConfigInfo);
      FontToken.LoadConfig(tokenConfigInfo);
      base.LoadConfig(tokenConfigInfo);
   }
}