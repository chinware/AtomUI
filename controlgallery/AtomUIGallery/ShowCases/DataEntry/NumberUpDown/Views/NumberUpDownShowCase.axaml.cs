using System;
using System.Collections.Generic;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.NumberUpDown;

public partial class NumberUpDownShowCase : GalleryReactiveUserControl<NumberUpDownViewModel>
{
    public const string LanguageId = nameof(NumberUpDownShowCase);

    public NumberUpDownShowCase()
    {
        InitializeComponent();
    }

}
