using Avalonia.Media;

namespace AtomUI.Controls;

internal interface IShadowAwareLayer
{ 
    BoxShadows MaskShadows { get; set; }
}