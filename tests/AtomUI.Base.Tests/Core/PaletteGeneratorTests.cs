using AtomUI.Media;
using AtomUI.Theme.Palette;
using Avalonia.Media;
using Avalonia.Styling;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AtomDB.UI.Tests.Core;

public class PaletteGeneratorFixture : IDisposable
{
   public IList<string> PresetBlueColors { get; init; }
   public IList<string> PresetBlueDarkColors { get; init; }
   
   public PaletteGeneratorFixture()
   {
      PresetBlueColors = new List<string>
      {
         "#e5f4ff",
         "#bae0ff",
         "#91caff",
         "#69b1ff",
         "#4096ff",
         "#1677ff",
         "#0958d9",
         "#003eb3",
         "#002c8c",
         "#001d66",
      };
      PresetBlueDarkColors = new List<string>
      {
         "#111a2c",
         "#112545",
         "#15325b",
         "#15417e",
         "#1554ad",
         "#1668dc",
         "#3c89e8",
         "#65a9f3",
         "#8dc5f8",
         "#b7dcfa",
      };
   }
   
   public void Dispose()
   {
      PresetBlueColors.Clear();
      PresetBlueDarkColors.Clear();
   }
}

public class PaletteGeneratorTests : IClassFixture<PaletteGeneratorFixture>
{
   private PaletteGeneratorFixture _fixture;
   private ITestOutputHelper _output;

   public PaletteGeneratorTests(PaletteGeneratorFixture fixture, ITestOutputHelper output)
   {
      _fixture = fixture;
      _output = output;
   }

   public static TheoryData<PresetPrimaryColor> TestPresetPaletteColorLengthData()
   {
      var data = new TheoryData<PresetPrimaryColor>();
      data.Add(PresetPrimaryColor.Red);
      data.Add(PresetPrimaryColor.Volcano);
      data.Add(PresetPrimaryColor.Orange);
      data.Add(PresetPrimaryColor.Gold);
      data.Add(PresetPrimaryColor.Yellow);
      data.Add(PresetPrimaryColor.Lime);
      data.Add(PresetPrimaryColor.Green);
      data.Add(PresetPrimaryColor.Cyan);
      data.Add(PresetPrimaryColor.Blue);
      data.Add(PresetPrimaryColor.GeekBlue);
      data.Add(PresetPrimaryColor.Purple);
      data.Add(PresetPrimaryColor.Magenta);
      data.Add(PresetPrimaryColor.Grey);
      return data;
   }
   
   [Theory]
   [MemberData(nameof(TestPresetPaletteColorLengthData))]
   public void TestPresetPaletteColorLength(PresetPrimaryColor presetPrimaryColor)
   {
      var colors = PaletteGenerator.GeneratePalette(presetPrimaryColor.Color());
      colors.Count.ShouldBeEquivalentTo(10);
   }

   [Fact]
   public void TestGeneratePalette()
   {
      IList<string> expected = new List<string>
      {
         "#e6f7ff",
         "#bae7ff",
         "#91d5ff",
         "#69c0ff",
         "#40a9ff",
         "#1890ff",
         "#096dd9",
         "#0050b2",
         "#003a8c",
         "#002766",
      };
      var colors = PaletteGenerator.GeneratePalette(Color.Parse("#1890ff"));
      for (int i = 0; i < colors.Count; i++) {
         colors[i].HexName().ShouldBeEquivalentTo(expected[i]);
      }
   }
   
   [Fact]
   public void TestGenerateDarkPalette()
   {
      IList<string> expected = new List<string>
      {
         "#111d2c",
         "#112a45",
         "#15395b",
         "#164c7e",
         "#1765ad",
         "#177ddc",
         "#3c9ae7",
         "#65b7f3",
         "#8dcff8",
         "#b7e3fa",
      };
      var colors = PaletteGenerator.GeneratePalette(Color.Parse("#1890ff"), new PaletteGenerateOption()
      {
         ThemeVariant = ThemeVariant.Dark,
         BackgroundColor = Color.Parse("#141414")
      });
      for (int i = 0; i < colors.Count; i++) {
         colors[i].HexName().ShouldBeEquivalentTo(expected[i]);
      }
   }
}