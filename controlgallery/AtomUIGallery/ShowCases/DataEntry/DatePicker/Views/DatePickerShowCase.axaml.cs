using System;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.DatePicker;

public partial class DatePickerShowCase : GalleryReactiveUserControl<DatePickerViewModel>
{
    public const string LanguageId = nameof(DatePickerShowCase);

    public DatePickerShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is DatePickerViewModel viewModel)
            {
                PickerSizeTypeOptionGroup.OptionCheckedChanged  += viewModel.HandlePickerSizeTypeOptionCheckedChanged;
                PickerPlacementOptionGroup.OptionCheckedChanged += viewModel.HandlePickerPlacementCheckedChanged;
                viewModel.PickerPlacement                       =  PlacementMode.BottomEdgeAlignedLeft;

                Disposable.Create(() =>
                {
                    PickerSizeTypeOptionGroup.OptionCheckedChanged -=
                        viewModel.HandlePickerSizeTypeOptionCheckedChanged;
                    PickerPlacementOptionGroup.OptionCheckedChanged -= viewModel.HandlePickerPlacementCheckedChanged;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
}
