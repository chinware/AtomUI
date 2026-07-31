using AtomUI.Theme.Configuration;
using AtomUI.Theme.Resources;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Theme;

public class ThemeConfigProvider : ThemeVariantScope
{
    private static readonly ThemeConfig s_defaultConfig = new ThemeConfigBuilder().Build();

    #region 公共属性定义

    public static readonly StyledProperty<ThemeConfig?> ConfigProperty =
        AvaloniaProperty.Register<ThemeConfigProvider, ThemeConfig?>(nameof(Config));

    public ThemeConfig? Config
    {
        get => GetValue(ConfigProperty);
        set => SetValue(ConfigProperty, value);
    }

    public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
    public event EventHandler<ThemeChangeFailedEventArgs>? ThemeChangeFailed;

    #endregion

    private ThemeScopeRegistration? _registration;
    private ThemeContext? _context;

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (_registration is not null)
        {
            return;
        }

        var parentContext = ThemeScope.ResolveContext(this, e.Root) ??
                            throw new InvalidOperationException(
                                "ThemeConfigProvider requires an inherited ThemeContext.");
        var registration = parentContext.Manager.RegisterScope(
            this,
            parentContext,
            Config ?? s_defaultConfig,
            out var result);
        _registration = registration;
        _context      = registration.Context;
        Resources.MergedDictionaries.Add(registration.Context.ResourceProvider);
        SetValue(ThemeScope.ContextProperty, registration.Context);
        PublishCommittedContext(registration.Context, notifyResources: false);
        DispatchResult(result);
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        var registration = _registration;
        var context = _context;
        _registration = null;
        _context      = null;
        if (context is not null)
        {
            Resources.MergedDictionaries.Remove(context.ResourceProvider);
        }
        ClearValue(ThemeScope.ContextProperty);
        ClearValue(RequestedThemeVariantProperty);
        registration?.Dispose();
        base.OnDetachedFromLogicalTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property != ConfigProperty || _registration is null)
        {
            return;
        }

        _registration.Context.Manager.ReplaceScopeConfig(
            this,
            change.GetNewValue<ThemeConfig?>() ?? s_defaultConfig);
    }

    internal void PublishCommittedContext(ThemeContext context, bool notifyResources)
    {
        if (!ReferenceEquals(_context, context))
        {
            return;
        }

        SetCurrentValue(
            RequestedThemeVariantProperty,
            context.Appearance == ThemeAppearance.Dark
                ? Avalonia.Styling.ThemeVariant.Dark
                : Avalonia.Styling.ThemeVariant.Light);
        context.Publish(notifyResources);
    }

    internal void DispatchResult(ThemeScopeUpdateResult result)
    {
        var publishDiagnostics = new List<ThemeDiagnostic>();
        if (result.Success)
        {
            ThemeEventDispatcher.Dispatch(
                ThemeChanged,
                this,
                new ThemeChangedEventArgs(
                    result.Request,
                    result.State,
                    result.PublishDiagnostics),
                publishDiagnostics,
                nameof(ThemeChanged));
            return;
        }

        ThemeEventDispatcher.Dispatch(
            ThemeChangeFailed,
            this,
            new ThemeChangeFailedEventArgs(
                result.Request,
                result.Diagnostics,
                result.Exception),
            publishDiagnostics,
            nameof(ThemeChangeFailed));
    }
}
