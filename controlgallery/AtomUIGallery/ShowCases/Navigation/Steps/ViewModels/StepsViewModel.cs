using System.Collections.ObjectModel;
using System.Globalization;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Steps;

public class StepsViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Steps";

    private const int InteractiveLastStepIndex = 2;

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private int _current;

    public int Current
    {
        get => _current;
        set
        {
            this.RaiseAndSetIfChanged(ref _current, value);
            this.RaisePropertyChanged(nameof(CurrentText));
            PreviousButtonVisible = Current > 0;
            RefreshInteractiveText();
        }
    }

    public string CurrentText => Current.ToString(CultureInfo.CurrentCulture);

    public string InteractivePageContent => Current switch
    {
        0 => Lang(StepsShowCaseLangResourceKind.P2ContentFirstContent),
        1 => Lang(StepsShowCaseLangResourceKind.P2ContentSecondContent),
        2 => Lang(StepsShowCaseLangResourceKind.P2ContentLastContent),
        _ => string.Empty
    };

    private bool _previousButtonVisible;

    public bool PreviousButtonVisible
    {
        get => _previousButtonVisible;
        set => this.RaiseAndSetIfChanged(ref _previousButtonVisible, value);
    }

    private string _nextButtonText = Lang(StepsShowCaseLangResourceKind.P2ContentNext);

    public string NextButtonText
    {
        get => _nextButtonText;
        set => this.RaiseAndSetIfChanged(ref _nextButtonText, value);
    }

    public StepsViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void ResetInteractiveStep()
    {
        Current = 0;
    }

    public void MoveToNextInteractiveStep()
    {
        if (Current < InteractiveLastStepIndex)
        {
            Current++;
        }
    }

    public void MoveToPreviousInteractiveStep()
    {
        if (Current > 0)
        {
            Current--;
        }
    }

    public void RefreshInteractiveText()
    {
        NextButtonText = Current == InteractiveLastStepIndex
            ? Lang(StepsShowCaseLangResourceKind.P2ContentDone)
            : Lang(StepsShowCaseLangResourceKind.P2ContentNext);
        this.RaisePropertyChanged(nameof(InteractivePageContent));
    }

    private static string Lang(StepsShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(StepsShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            StepsShowCaseLangResourceKind.P2ContentDone                         => en_US.P2ContentDone,
            StepsShowCaseLangResourceKind.P2ContentNext                         => en_US.P2ContentNext,
            StepsShowCaseLangResourceKind.P2ContentFirstContent                 => en_US.P2ContentFirstContent,
            StepsShowCaseLangResourceKind.P2ContentSecondContent                => en_US.P2ContentSecondContent,
            StepsShowCaseLangResourceKind.P2ContentLastContent                  => en_US.P2ContentLastContent,
            StepsShowCaseLangResourceKind.P2TextCurrent                         => en_US.P2TextCurrent,
            _                                                                  => kind.ToString()
        };
    }
}
