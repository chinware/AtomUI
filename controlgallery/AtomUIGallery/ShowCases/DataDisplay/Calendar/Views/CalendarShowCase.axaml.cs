using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using Avalonia;

namespace AtomUIGallery.ShowCases.Calendar;

public partial class CalendarShowCase : GalleryReactiveUserControl<CalendarViewModel>
{
    public const string LanguageId = nameof(CalendarShowCase);

    public CalendarShowCase()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            RefreshSelectableCalendarText();

            var languageManager = Application.Current?.GetLanguageManager();
            if (languageManager is not null)
            {
                EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshSelectableCalendarText();
                languageManager.LanguageVariantChanged += handler;
                Disposable.Create(() => languageManager.LanguageVariantChanged -= handler)
                          .DisposeWith(disposables);
            }
        });
    }

    private void OnSelectableCalendarSelected(object? sender, CalendarSelectedEventArgs e)
    {
        if (DataContext is CalendarViewModel vm)
        {
            vm.SelectSelectableCalendarDate(e.Value);
        }
    }

    private void RefreshSelectableCalendarText()
    {
        if (DataContext is CalendarViewModel vm)
        {
            vm.RefreshSelectableCalendarText();
        }
    }
}
