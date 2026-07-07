using System;
using System.Collections.Generic;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.LineEdit;

public partial class LineEditShowCase : GalleryReactiveUserControl<LineEditViewModel>
{
    public const string LanguageId = nameof(LineEditShowCase);

    public LineEditShowCase()
    {
        InitializeComponent();
    }

}
