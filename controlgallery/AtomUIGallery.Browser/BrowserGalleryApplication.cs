using System.Globalization;
using AtomUI;
using AtomUI.Desktop.Controls;
using AtomUI.Fonts.AlibabaPuHuiTi;
using AtomUI.Theme;
using AtomUIGallery;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;

namespace AtomUIGallery.Browser;

public class BrowserGalleryApplication : Application
{
    public override void Initialize()
    {
        this.UseAtomUI(builder =>
        {
            builder.WithDefaultCultureInfo(CultureInfo.CurrentUICulture);
            builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
            builder.UseAlibabaSansFont();
            builder.UseAlibabaPuHuiTiFont();
            builder.WithDefaultFontFamily(FontFamily.Parse(
                $"fonts:AlibabaSans#Alibaba Sans, {AlibabaPuHuiTiFontConstants.FontFamily}, $Default"));
            builder.UseDesktopControls();
            builder.UseDesktopColorPicker();
            builder.UseDesktopDataGrid();
            builder.UseGalleryControls();
        });
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewLifetime)
        {
            singleViewLifetime.MainView = new BrowserGalleryView();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
