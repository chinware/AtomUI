using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUIGallery.ShowCases.ViewModels;
using AtomUIGallery.Utils;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class OsInfoPage : ReactiveUserControl<OsInfoViewModel>
{
    internal static Dictionary<string, string> LinuxDistroLogos = new();

    static OsInfoPage()
    {
        LinuxDistroLogos["linux"]         = "/Assets/OSLogos/Linux.svg";
        LinuxDistroLogos["ubuntu"]        = "/Assets/OSLogos/Ubuntu.svg";
        LinuxDistroLogos["deepin"]        = "/Assets/OSLogos/Deepin.svg";
        LinuxDistroLogos["macOS"]         = "/Assets/OSLogos/MacOS.svg";
        LinuxDistroLogos["windows"]       = "/Assets/OSLogos/Windows.svg";
        LinuxDistroLogos["opensuse-leap"] = "/Assets/OSLogos/OpenSUSE.svg";
    }
    
    public OsInfoPage()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is OsInfoViewModel viewModel)
            {
                InitInfoRecords(viewModel);
                ConfigureOsLogoPath();
                this.OneWayBind(viewModel, vm => vm.SystemInfoRecords, 
                        v => v.OsInfoTable.ItemsSource)
                    .DisposeWith(disposables);
                
                Disposable.Create(() =>
                {
                    viewModel.SystemInfoRecords = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
    
    private void InitInfoRecords(OsInfoViewModel viewModel)
    {
        var systemInfo        = SystemInfoProvider.GetSystemInfo();
        var systemInfoRecords = new List<SystemInfoRecord>();
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.OSName),
            Value = systemInfo.OSName,
        });
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.OSVersion),
            Value = systemInfo.OSVersion,
        });
        
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.AvaloniaVersion),
            Value = systemInfo.AvaloniaVersion
        });
        
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.AtomUIVersion),
            Value = systemInfo.AtomUIVersion,
        });
        
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.Architecture),
            Value = systemInfo.Architecture,
        });
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.RuntimeVersion),
            Value = systemInfo.RuntimeVersion,
        });
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.CPUName),
            Value = systemInfo.CPUName,
        });
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.CPUCores),
            Value = systemInfo.CPUCores.ToString(),
        });
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.MemoryInfo),
            Value = systemInfo.MemoryInfo
        });
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.SystemUptime),
            Value = systemInfo.SystemUptime.ToString()
        });
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.CurrentCulture),
            Value = systemInfo.CurrentCulture
        });
        systemInfoRecords.Add(new SystemInfoRecord()
        {
            Name  = nameof(systemInfo.TimeZone),
            Value = systemInfo.TimeZone
        });
        viewModel.SystemInfoRecords = systemInfoRecords;
    }

    private void ConfigureOsLogoPath()
    {
        if (DataContext is OsInfoViewModel viewModel)
        {
            if (OperatingSystem.IsLinux())
            {
                var distroInfo = LinuxDistributionDetector.DetectDistribution();
                var id         = distroInfo.Id;
                if (LinuxDistroLogos.ContainsKey(id))
                {
                    viewModel.LogoPath =  LinuxDistroLogos[id];
                }
                else
                {
                    viewModel.LogoPath =  LinuxDistroLogos["linux"];
                }

                if (id == "ubuntu" || id == "deepin")
                {
                    OsLogo.Width = 240;
                }
                else
                {
                    OsLogo.Height = 130;
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                viewModel.LogoPath = LinuxDistroLogos["macOS"];
                OsLogo.Height      = 130;
            }
            else if (OperatingSystem.IsWindows())
            {
                viewModel.LogoPath = LinuxDistroLogos["windows"];
                OsLogo.Height      = 130;
            }
        }
    }
}