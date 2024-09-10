using System.Collections.ObjectModel;
using AtomUI.Theme.Palette;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using SolidColorBrush = Avalonia.Media.SolidColorBrush;

namespace AtomUI.Demo.Desktop.ViewModels;

public class PaletteMetaItem
{
    public string Title;
    public string Desc;
    public PresetPrimaryColor PresetPrimaryColor;

    public PaletteMetaItem(string title, string desc, PresetPrimaryColor presetPrimaryColor)
    {
        Title              = title;
        Desc               = desc;
        PresetPrimaryColor = presetPrimaryColor;
    }
}

public class PaletteDemoViewModel : ObservableObject
{
    private readonly PaletteMetaItem[] _presetPaletteInfos =
    {
        new("Dust Red / 薄暮", "斗志、奔放", PresetPrimaryColor.Red),
        new("Volcano / 火山", "醒目、澎湃", PresetPrimaryColor.Volcano),
        new("Sunset Orange / 日暮", "温暖、欢快", PresetPrimaryColor.Orange),
        new("Calendula Gold / 金盏花", "活力、积极", PresetPrimaryColor.Gold),
        new("Sunrise Yellow / 日出", "出生、阳光", PresetPrimaryColor.Yellow),
        new("Lime / 青柠", "自然、生机", PresetPrimaryColor.Lime),
        new("Polar Green / 极光绿", "健康、创新", PresetPrimaryColor.Green),
        new("Cyan / 明青", "希望、坚强", PresetPrimaryColor.Cyan),
        new("Daybreak Blue / 拂晓蓝", "包容、科技、普惠", PresetPrimaryColor.Blue),
        new("Geek Blue / 极客蓝", "探索、钻研", PresetPrimaryColor.GeekBlue),
        new("Golden Purple / 酱紫", "优雅、浪漫", PresetPrimaryColor.Purple),
        new("Magenta / 法式洋红", "明快、感性", PresetPrimaryColor.Magenta)
    };

    private ColorItemViewModel _selectedColor = null!;

    public ColorItemViewModel SelectedColor
    {
        get => _selectedColor;
        set => SetProperty(ref _selectedColor, value);
    }

    private ObservableCollection<ColorGroupViewModel>? _lightLists;

    public ObservableCollection<ColorGroupViewModel>? LightLists
    {
        get => _lightLists;
        set => SetProperty(ref _lightLists, value);
    }

    private ObservableCollection<ColorGroupViewModel>? _darkLists;

    public ObservableCollection<ColorGroupViewModel>? DarkLists
    {
        get => _darkLists;
        set => SetProperty(ref _darkLists, value);
    }

    public PaletteDemoViewModel()
    {
        WeakReferenceMessenger.Default.Register<PaletteDemoViewModel, ColorItemViewModel>(this, OnClickColorItem);
    }

    public void InitializeResources()
    {
        InitializePalette();
    }

    private void InitializePalette()
    {
        LightLists = new ObservableCollection<ColorGroupViewModel>();
        var cycleColorList = new ObservableCollection<ColorListViewModel>();
        var cycleCount     = 0;
        for (var i = 0; i < _presetPaletteInfos.Length; ++i)
        {
            var metaInfo           = _presetPaletteInfos[i];
            var colorListViewModel = new ColorListViewModel();
            colorListViewModel.Title = metaInfo.Title;
            colorListViewModel.Desc  = metaInfo.Desc;
            var paletteInfo         = PresetPalettes.GetPresetPalette(metaInfo.PresetPrimaryColor);
            var colorItemViewModels = new ObservableCollection<ColorItemViewModel>();
            var presetColorName     = metaInfo.PresetPrimaryColor.Name();

            for (var j = 0; j < paletteInfo.ColorSequence.Count; j++)
            {
                var color = paletteInfo.ColorSequence[j];
                var colorItem = new ColorItemViewModel($"{presetColorName}-{j + 1}",
                    new SolidColorBrush(color),
                    true,
                    j);
                colorItemViewModels.Add(colorItem);
            }

            colorListViewModel.Colors = colorItemViewModels;
            cycleColorList.Add(colorListViewModel);
            ++cycleCount;

            if (cycleCount == 3)
            {
                var colorGroupModel = new ColorGroupViewModel();
                colorGroupModel.ColorList = cycleColorList;
                LightLists.Add(colorGroupModel);
                cycleColorList = new ObservableCollection<ColorListViewModel>();
                cycleCount     = 0;
            }
        }

        DarkLists = new ObservableCollection<ColorGroupViewModel>();

        for (var i = 0; i < _presetPaletteInfos.Length; ++i)
        {
            var metaInfo           = _presetPaletteInfos[i];
            var colorListViewModel = new ColorListViewModel();
            colorListViewModel.Title = metaInfo.Title;
            colorListViewModel.Desc  = metaInfo.Desc;
            var paletteInfo         = PresetPalettes.GetPresetPalette(metaInfo.PresetPrimaryColor, true);
            var colorItemViewModels = new ObservableCollection<ColorItemViewModel>();
            var presetColorName     = metaInfo.PresetPrimaryColor.Name();

            for (var j = 0; j < paletteInfo.ColorSequence.Count; j++)
            {
                var color = paletteInfo.ColorSequence[j];
                var colorItem = new ColorItemViewModel($"{presetColorName}-{j + 1}",
                    new SolidColorBrush(color),
                    false,
                    j);
                colorItemViewModels.Add(colorItem);
            }

            colorListViewModel.Colors = colorItemViewModels;
            cycleColorList.Add(colorListViewModel);
            ++cycleCount;

            if (cycleCount == 3)
            {
                var colorGroupModel = new ColorGroupViewModel();
                colorGroupModel.ColorList = cycleColorList;
                DarkLists.Add(colorGroupModel);
                cycleColorList = new ObservableCollection<ColorListViewModel>();
                cycleCount     = 0;
            }
        }
    }

    private void OnClickColorItem(PaletteDemoViewModel vm, ColorItemViewModel item)
    {
        SelectedColor = item;
    }
}

public class ColorGroupViewModel : ObservableObject
{
    private ObservableCollection<ColorListViewModel>? _colorList;

    public ObservableCollection<ColorListViewModel>? ColorList
    {
        get => _colorList;
        set => SetProperty(ref _colorList, value);
    }
}

public class ColorListViewModel : ObservableObject
{
    private ObservableCollection<ColorItemViewModel>? _colors;

    public ObservableCollection<ColorItemViewModel>? Colors
    {
        get => _colors;
        set => SetProperty(ref _colors, value);
    }

    private string? _title;

    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    private string? _desc;

    public string? Desc
    {
        get => _desc;
        set => SetProperty(ref _desc, value);
    }
}

public class ColorItemViewModel : ObservableObject
{
    private IBrush _brush = null!;

    public IBrush Brush
    {
        get => _brush;
        set => SetProperty(ref _brush, value);
    }

    private IBrush _textBrush = null!;

    public IBrush TextBrush
    {
        get => _textBrush;
        set => SetProperty(ref _textBrush, value);
    }

    private string _colorDisplayName = null!;

    public string ColorDisplayName
    {
        get => _colorDisplayName;
        set => SetProperty(ref _colorDisplayName, value);
    }

    private string _hex = null!;

    public string Hex
    {
        get => _hex;
        set => SetProperty(ref _hex, value);
    }

    public ColorItemViewModel(string colorDisplayName, ISolidColorBrush brush, bool light, int index)
    {
        ColorDisplayName = colorDisplayName;
        Brush            = brush;
        Hex              = brush.ToString()!.ToUpperInvariant();
        if ((light && index < 5) || (!light && index >= 5))
        {
            TextBrush = Brushes.Black;
        }
        else
        {
            TextBrush = Brushes.White;
        }
    }
}