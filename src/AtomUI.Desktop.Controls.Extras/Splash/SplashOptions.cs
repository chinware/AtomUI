using Avalonia.Controls.Templates;

namespace AtomUI.Desktop.Controls;

public class SplashOptions
{
    public object? Logo { get; set; }
    public IDataTemplate? LogoTemplate { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public string? Message { get; set; }
    public string? Detail { get; set; }
    public double? Progress { get; set; }
    public bool IsIndeterminate { get; set; } = true;
    public SplashStatus Status { get; set; } = SplashStatus.Loading;
    public bool IsMotionEnabled { get; set; } = true;
    public object? Content { get; set; }
    public IDataTemplate? ContentTemplate { get; set; }
    public object? Footer { get; set; }
    public IDataTemplate? FooterTemplate { get; set; }
    public TimeSpan MinimumShowDuration { get; set; } = TimeSpan.FromMilliseconds(500);
    public TimeSpan CloseDelay { get; set; } = TimeSpan.Zero;
    public TimeSpan FadeOutDuration { get; set; } = TimeSpan.FromMilliseconds(180);
    public bool Topmost { get; set; } = true;
    public double Width { get; set; } = 420;
    public double MinHeight { get; set; } = 280;

    internal void ApplyTo(Splash splash)
    {
        splash.Logo              = Logo;
        splash.LogoTemplate      = LogoTemplate;
        splash.Title             = Title;
        splash.Subtitle          = Subtitle;
        splash.Message           = Message;
        splash.Detail            = Detail;
        splash.Progress          = Progress;
        splash.IsIndeterminate   = IsIndeterminate;
        splash.Status            = Status;
        splash.IsMotionEnabled   = IsMotionEnabled;
        splash.Content           = Content;
        splash.ContentTemplate   = ContentTemplate;
        splash.Footer            = Footer;
        splash.FooterTemplate    = FooterTemplate;
    }
}
