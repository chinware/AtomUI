using AtomUIGallery.Localization;
using System.Globalization;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Data;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.LineEdit;

public class LineEditViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "LineEdit";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private string? _otpLineEditBoundValue = "123456";
    private string? _otpLineEditBoundValueSummary;

    public string? OtpLineEditBoundValue
    {
        get => _otpLineEditBoundValue;
        set
        {
            this.RaiseAndSetIfChanged(ref _otpLineEditBoundValue, value);
            UpdateOtpLineEditBoundValueSummary();
        }
    }

    public string? OtpLineEditBoundValueSummary
    {
        get => _otpLineEditBoundValueSummary;
        set => this.RaiseAndSetIfChanged(ref _otpLineEditBoundValueSummary, value);
    }

    public ReactiveCommand<Unit, Unit> SetOtpLineEditBoundValueCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearOtpLineEditBoundValueCommand { get; }

    public Func<string, string> OtpLineEditUppercaseFormatter { get; } = value => value.ToUpperInvariant();

    public LineEditViewModel(IScreen screen)
    {
        HostScreen                         = screen;
        SetOtpLineEditBoundValueCommand   = ReactiveCommand.Create(HandleSetOtpLineEditBoundValue);
        ClearOtpLineEditBoundValueCommand = ReactiveCommand.Create(HandleClearOtpLineEditBoundValue);
        UpdateOtpLineEditBoundValueSummary();
    }

    private void HandleSetOtpLineEditBoundValue()
    {
        OtpLineEditBoundValue = "654321";
    }

    private void HandleClearOtpLineEditBoundValue()
    {
        OtpLineEditBoundValue = null;
    }

    private void UpdateOtpLineEditBoundValueSummary()
    {
        var displayValue = string.IsNullOrEmpty(OtpLineEditBoundValue)
            ? Lang(LineEditShowCaseLangResourceKind.OtpLineEditEmptyValueText)
            : OtpLineEditBoundValue;
        OtpLineEditBoundValueSummary = string.Format(
            CultureInfo.CurrentCulture,
            Lang(LineEditShowCaseLangResourceKind.OtpLineEditCurrentValueFormat),
            displayValue);
    }

    private static string Lang(LineEditShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(LineEditShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            LineEditShowCaseLangResourceKind.OtpLineEditCurrentValueFormat       => en_US.OtpLineEditCurrentValueFormat,
            LineEditShowCaseLangResourceKind.OtpLineEditEmptyValueText           => en_US.OtpLineEditEmptyValueText,
            _                                                                    => kind.ToString()
        };
    }
}

public sealed class OtpLineEditSeparatorBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is int cellIndex && cellIndex % 2 != 0
            ? Brushes.Red
            : Brushes.Blue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
