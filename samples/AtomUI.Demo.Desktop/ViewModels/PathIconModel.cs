using System.Collections.ObjectModel;
using AtomUI.Icon;
using AtomUI.Icon.AntDesign;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AtomUI.Demo.Desktop.ViewModels;

public class IconInfoItemModel : ObservableObject
{
    private string _iconName = null!;

    public string IconName
    {
        get => _iconName;
        set => SetProperty(ref _iconName, value);
    }

    private string _iconKind = null!;

    public string IconKind
    {
        get => _iconKind;
        set => SetProperty(ref _iconKind, value);
    }

    public IconInfoItemModel(string iconName, string iconKind)
    {
        IconName = iconName;
        IconKind = iconKind;
    }
}

public class IconGalleryModel : ObservableObject
{
    private readonly IconThemeType? _iconThemeType;

    private ObservableCollection<IconInfoItemModel>? _iconInfos;

    public ObservableCollection<IconInfoItemModel>? IconInfos
    {
        get => _iconInfos;
        set => SetProperty(ref _iconInfos, value);
    }

    public IconGalleryModel(IconThemeType? iconThemeType = null)
    {
        _iconThemeType = iconThemeType;
        if (_iconThemeType.HasValue)
        {
            LoadThemeIcons(_iconThemeType.Value);
        }
    }

    public void LoadThemeIcons(IconThemeType iconThemeType)
    {
        var iconPackage = IconManager.Current.GetIconProvider<AntDesignIconPackage>();
        if (iconPackage is null)
        {
            return;
        }

        IconInfos = new ObservableCollection<IconInfoItemModel>();
        var iconInfos = iconPackage.GetIconInfos(iconThemeType);
        foreach (var iconInfo in iconInfos)
        {
            var iconInfoModel = new IconInfoItemModel(iconInfo.Name, iconInfo.Name);
            IconInfos.Add(iconInfoModel);
        }
    }
}