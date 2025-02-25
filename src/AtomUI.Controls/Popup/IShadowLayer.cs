using Avalonia.Media;

namespace AtomUI.Controls;

internal interface IShadowDecorator
{ 
    BoxShadows MaskShadows { get; set; }
    void Open();
    void Close();
}